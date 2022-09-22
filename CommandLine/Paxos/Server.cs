using Akka.Event;
using CommandLine.KVStore;
using Core;

namespace CommandLine.Paxos;

internal class Server : Node
{
    private readonly IServiceProvider sp;
    private readonly string address;
    private readonly string[] peers;

    // Server can be leader, replica, acceptor.
    // leader election happens through phase 1 messages. 
    // leaders are elected during start and after current leader timeout.
    // acceptors and replicas are always online.
    // accetors reacieve phase 1a and reply with phase 1b , 2a and reply 2b
    private readonly Ballot acceptorBallot;
    private List<BallotValue> acceptedBallots;

    private AMOApplication app = new();
    private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

    public Server(IServiceProvider sp, string address, string[] peers)
    {
        this.sp = sp;
        this.address = address;
        this.peers = peers;
        this.acceptorBallot = new Ballot(0, address);
    }

    public override void ReceiveTimer(ITimer timer)
    {
        throw new NotImplementedException();
    }

    protected override void HandleRequest(IRequest request, string sender)
    {
        if (request is Phase1A p1a)
        {
            HandlePhase1A(p1a, sender);
        }
        if (request is AMORequest aMORequest)
        {
            HandleAMORequest(aMORequest);
        }

        if (request is Phase2A p2a)
        {
            HandlePhase2A(p2a, sender);
        }
        throw new NotImplementedException();
    }

    protected override void ReceiveResponse(IResult message, string sender)
    {
        throw new NotImplementedException();
    }

    private void HandlePhase1A(Phase1A message, string sender)
    {
        // reply with phase1b if ballot is bigger than mine.
        if (this.acceptorBallot.ballotNum < message.ballot)
        {
            // respond with phase1b
            this.SendToPeer(sender, new Phase1B());
        }
    }

    private void HandlePhase2A(Phase2A message, string sender)
    {
        if (message.ballotNumber == this.acceptorBallot.ballotNum)
        {
            this.acceptedBallots.Add(new BallotValue(message.ballotNumber, message.ballotValue));
        }
        else 
        {
            this.SendToPeer(sender, new Phase2B(this.acceptorBallot.ballotNum, message.ballotValue, "accepted"));
        }
    }

    #region Leader

    #endregion

    #region Replica
    private readonly int window = 10;
    private int slotIn = 1, slotOut = 1;
    protected Dictionary<int, Proposal> proposals = new();
    protected Dictionary<int, Decision> decisions = new();

    protected Queue<AMORequest> requests = new();

    public string LeaderAddress { get; set; }

    public void HandleAMORequest(AMORequest request)
    {
        requests.Enqueue(request);
    }

    public void HandleDecision(Decision decision)
    {
        decisions[decision.slotNumber] = decision;
        while (decisions.ContainsKey(slotOut))
        {
            if (proposals.ContainsKey(slotOut))
            {
                // Proposed message at a certain slot was not chosen.
                if (!proposals[slotOut].Request.Equals(decisions[slotOut].Request))
                {
                    requests.Enqueue(proposals[slotOut].Request);
                    proposals.Remove(slotOut);
                }
            }

            this.Perform(decisions[slotOut]);
        }

        this.Propose();
    }

    private void Propose()
    {
        if (LeaderAddress == null)
        {
            return;
        }

        while (requests.Count > 0 && slotIn < slotOut + window)
        {
            if (!decisions.ContainsKey(slotIn))
            {
                var request = requests.Dequeue();
                this.proposals[slotIn] = new Proposal(request, slotIn);
                this.SendToPeer(LeaderAddress, proposals[slotIn]);
                ++slotIn;
            }
        }
    }

    private void Perform(Decision decision)
    {
        for (int i = 1; i < slotOut; i++)
        {
            if (decisions[i].Request.Equals(decision.Request))
            {
                // Message has been chosen again.
                ++slotOut;
                return;
            }
        }

        var response = this.app.Execute(decision.Request);
        this.SendToPeer(decision.Request.address, response);
        ++slotOut;
    }
    #endregion
}
