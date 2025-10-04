namespace K4os.Template.Orleans.Hosting.Configuration;

public class TelemetryConfig
{
    public string[]? Meters { get; set; }
    public string[]? Traces { get; set; }
    public double? SamplingRatio { get; set; }
}
