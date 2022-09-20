using Core;

namespace CommandLine.KVStore;

public interface KVStoreCommand : IRequest
{
}

public interface SingleKeyCommand : KVStoreCommand
{
    string Key { get; }
}

public record Get(string Key) : SingleKeyCommand
{
}

public record Put(string Key, string Value) : SingleKeyCommand
{

}

public record Append(string Key, string Value) : SingleKeyCommand
{
}


public interface KVStoreResult : IResult
{
}

public record PutOk : KVStoreResult { }

public record AppendResult(string Value) : KVStoreResult { }

public record KeyNotFound : KVStoreResult { }

public record GetResult(string Value) : KVStoreResult { }

public record AMORequest(int sequenceNumber, string address, IRequest request) : IRequest { }
public record AMOResponse(int sequenceNumber, string address, IResult request) : IResult { }
