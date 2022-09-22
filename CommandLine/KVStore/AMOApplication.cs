using Core;

namespace CommandLine.KVStore;

/// <summary>
/// At most once application.
/// </summary>
public class AMOApplication : IDistributedApplication
{
    private readonly KVStore application;

    private readonly Dictionary<string, Info> recorder;
    public AMOApplication()
    {
        this.application = new();
        recorder = new();
    }

    public AMOApplication(AMOApplication application)
    {
        var app = application.application;

        this.application = new(app);

        recorder = new(application.recorder);
    }

    public IResult Execute(IRequest request)
    {
        if (request is not AMORequest)
        {
            throw new ArgumentException("Unexpected response");
        }

        AMORequest req = (AMORequest)request;
        if (this.alreadyExecuted(req))
        {
            var recorded = recorder[req.address];
            return new AMOResponse(recorded.sequenceNumber, req.address, recorded.Result);
        }

        var appResponse = application.Execute(req.request);
        var info = new Info(req.sequenceNumber, appResponse);
        recorder[req.address] = info;
        return new AMOResponse(info.sequenceNumber, req.address, info.Result);
    }

    private bool alreadyExecuted(AMORequest req)
    {
        Info info = this.recorder.GetValueOrDefault(req.address);

        return info != null && info.sequenceNumber >= req.sequenceNumber;
    }

    private record Info(int sequenceNumber, IResult Result);
}
