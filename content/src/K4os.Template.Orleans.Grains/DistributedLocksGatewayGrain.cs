using K4os.Template.Orleans.Interfaces;
using K4os.Template.Orleans.Interfaces.Messages;
using Orleans;
using Orleans.Concurrency;

namespace K4os.Template.Orleans.Grains;

[StatelessWorker, Reentrant]
public class DistributedLocksGatewayGrain: Grain, IDistributedLocksGateway
{
    public Task<DistributedLockReceipt> Acquire(string name, TimeSpan? timeout = null) => 
        GrainFactory.GetGrain<IDistributedLock>(name).Acquire(timeout);

    public Task<DistributedLockReceipt> Renew(string name, Guid receiptId, TimeSpan? timeout = null) => 
        GrainFactory.GetGrain<IDistributedLock>(name).Renew(receiptId, timeout);

    public Task Release(string name, Guid receiptId) => 
        GrainFactory.GetGrain<IDistributedLock>(name).Release(receiptId);
}
