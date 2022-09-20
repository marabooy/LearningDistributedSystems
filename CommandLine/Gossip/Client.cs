using Core;

namespace CommandLine.Gossip;

public class Client : Node
{
	public Client(params string[] servers)
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
