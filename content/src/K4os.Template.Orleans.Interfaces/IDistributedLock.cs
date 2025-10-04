using K4os.Template.Orleans.Interfaces.Messages;
using Orleans;
using Orleans.Concurrency;

namespace K4os.Template.Orleans.Interfaces;

public interface IDistributedLock: IGrainWithStringKey
{
    Task<DistributedLockReceipt> Acquire(TimeSpan? timeout = null);
    Task<DistributedLockReceipt> Renew(Guid receiptId, TimeSpan? timeout = null);
    Task Release(Guid receiptId);
}

public interface IDistributedLocksGateway: IGrainWithIntegerKey
{
    Task<DistributedLockReceipt> Acquire(string name, TimeSpan? timeout = null);
    Task<DistributedLockReceipt> Renew(string name, Guid receiptId, TimeSpan? timeout = null);
    Task Release(string name, Guid receiptId);
}