using K4os.Template.Orleans.Core;
using K4os.Template.Orleans.Interfaces;
using K4os.Template.Orleans.Interfaces.Messages;
using Orleans;
using Orleans.Runtime;

namespace K4os.Template.Orleans.Grains;

public class DistributedLockGrain: Grain, IDistributedLock
{
    private static readonly TimeSpan DefaultLockExpiration = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan MaximumLockExpiration = TimeSpan.FromMinutes(5);
    
    private readonly IPersistentState<DistributedLockState?> _state;

    private string GrainKey => this.GetPrimaryKeyString();
    private DistributedLockState? State
    {
        get => _state.State;
        set => _state.State = value;
    }

    public DistributedLockGrain(
        [PersistentState("DistributedLockGrain")] IPersistentState<DistributedLockState?> state)
    {
        _state = state;
    }
    
    private Task WriteState() => _state.WriteStateAsync();
    private Task ClearState() => _state.ClearStateAsync();
    
    public async Task<DistributedLockReceipt> Acquire(TimeSpan? timeout = null)
    {
        // check if it is already held or expired
        if (State is not null && State.ExpirationTime > DateTime.UtcNow)
            throw new ForbiddenError("Lock is already held by another process");

        State = new DistributedLockState {
            ReceiptId = Guid.NewGuid(),
            ExpirationTime = GetExpiration(timeout)
        };
        await WriteState();
        
        return ToReceipt(State);
    }

    public async Task<DistributedLockReceipt> Renew(Guid receiptId, TimeSpan? timeout = null)
    {
        if (State is null || State.ReceiptId != receiptId)
            throw new ForbiddenError("Provided receipt does not match the current lock holder");
        
        State.ExpirationTime = GetExpiration(timeout);
        await WriteState();
        
        return ToReceipt(State);
    }

    public async Task Release(Guid receiptId)
    {
        // you cannot release a lock you do not hold it
        // but also it is not a problem if you think you did
        if (State is null || State.ReceiptId != receiptId) 
            return;

        State = null;
        await ClearState();
    }
    
    private static DateTime GetExpiration(TimeSpan? timeout) => 
        DateTime.UtcNow.Add((timeout ?? DefaultLockExpiration).NotMoreThan(MaximumLockExpiration));

    private DistributedLockReceipt ToReceipt(DistributedLockState state) => 
        new(GrainKey, state.ReceiptId, state.ExpirationTime);
}