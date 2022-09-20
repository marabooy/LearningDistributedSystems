using Core;
using System.Collections.Concurrent;

namespace CommandLine.KVStore;

public class KVStore : IDistributedApplication
{
    private ConcurrentDictionary<string, string> backingStore = new();

    public KVStore()
    {
    }

    public KVStore(KVStore app)
    {
        this.backingStore = new(app.backingStore);
    }

    public IResult Execute(IRequest command)
    {
        if (command is Get g)
        {
            String key = g.Key;
            if (!this.backingStore.ContainsKey(key))
            {
                return new KeyNotFound();
            }
            return new GetResult(this.backingStore[key]);
        }

        if (command is Put p)
        {
            this.backingStore[p.Key] = p.Value;
            return new PutOk();
        }

        if (command is Append append)
        {
            String key = append.Key;

            String newValue = this.backingStore.GetValueOrDefault(key, string.Empty) + append.Value;
            this.backingStore[key] = newValue;
            return new AppendResult(newValue);
        }

        throw new ArgumentException("Illegal argument sent");
    }
}
