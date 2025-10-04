using Orleans;

namespace K4os.Template.Orleans.Grains;

[GenerateSerializer, Alias("DistributedLockState.v1")]
public class DistributedLockState
{
    [Id(0)]
    public Guid ReceiptId { get; set; }
    
    [Id(1)]
    public DateTime ExpirationTime { get; set; }
}
