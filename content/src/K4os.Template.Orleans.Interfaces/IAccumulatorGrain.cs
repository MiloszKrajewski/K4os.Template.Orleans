using Orleans;

namespace K4os.Template.Orleans.Interfaces;

public interface IAccumulatorGrain: IGrainWithStringKey
{
	public Task<double> Add(double value);
	public Task<double> Sum();
	public Task<double> Avg();
}
