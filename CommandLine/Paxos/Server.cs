using Core;

namespace CommandLine.Paxos;

internal class Server : Node
{
    private readonly string[] servers;

    public Server(params string[] servers)
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
