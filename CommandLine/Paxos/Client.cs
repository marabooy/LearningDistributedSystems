using Core;

namespace CommandLine.Paxos;

public class Client : Node
{
    private readonly string[] servers;

    public Client(params string[] servers)
    {
        this.servers = servers;
    }

    public override void ReceiveTimer(ITimer timer)
    {
        throw new NotImplementedException();
    }

    protected override void HandleRequest(IRequest request, string v)
    {
        throw new NotImplementedException();
    }

    protected override void ReceiveResponse(IResult message, string sender)
    {
        throw new NotImplementedException();
    }
}
