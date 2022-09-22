using CommandLine.KVStore;
using Core;

record Phase1A(int ballot) : IRequest
{
}

record Ballot(int ballotNum, string sender);

record Phase1B : IResult
{
}

record Proposal(AMORequest Request, int slotNumber);
record Decision(AMORequest Request, int slotNumber);