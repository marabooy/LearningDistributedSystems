using CommandLine.KVStore;
using Core;

record Phase1A(int ballot) : IRequest
{
}

record Ballot(int ballotNum, string sender);

record BallotValue(int ballotNum, int ballotValue);

record Phase1B : IResult
{
}
record Phase2A(int ballotNumber, int ballotValue) : IRequest
{
}

record Phase2B(int ballotNumber, int slotNumber, string command) : IResult
{ 
}

record Proposal(AMORequest Request, int slotNumber);
record Decision(AMORequest Request, int slotNumber);

record P1BResult(Ballot Ballot) : IResult { }
record P2AResult(Ballot Ballot, int slotNumber) : IResult { }
record ScoutTimer() : ITimer { }
record CommandTimer() : ITimer { }
