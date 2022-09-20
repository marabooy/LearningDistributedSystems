using Core;

namespace CommandLine.Gossip;

internal class Server : Node
{
    public Server(string id, string[] adjacent)
    {

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
