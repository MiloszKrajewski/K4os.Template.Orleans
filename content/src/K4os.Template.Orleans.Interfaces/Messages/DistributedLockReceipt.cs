using Orleans;

namespace K4os.Template.Orleans.Interfaces.Messages;

[Alias("DistributedLockReceipt.v1"), GenerateSerializer, Immutable]
public class DistributedLockReceipt
{
    [Id(0)]
    public string LockName { get; private set; }
    
    [Id(1)]
    public Guid ReceiptId { get; private set; }
    
    [Id(2)]
    public DateTime ExpirationTime { get; private set; }

    public DistributedLockReceipt(string name, Guid id, DateTime expires)
    {
        LockName = name;
        ReceiptId = id;
        ExpirationTime = expires;
    }
}
