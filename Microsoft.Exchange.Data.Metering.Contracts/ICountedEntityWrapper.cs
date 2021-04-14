using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface ICountedEntityWrapper<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		ICountedEntity<TEntityType> Entity { get; }

		ICountedConfig GetConfig(TCountType measure);

		void AddUsage(TCountType measure, ICountedConfig config, int increment);

		Task AddUsageAsync(TCountType measure, ICountedConfig config, int increment);

		bool TrySetUsage(TCountType measure, int value);

		Task<bool> SetUsageAsync(TCountType measure, int value);

		ICount<TEntityType, TCountType> GetUsage(TCountType measure);

		Task<ICount<TEntityType, TCountType>> GetUsageAsync(TCountType measure);

		IDictionary<TCountType, ICount<TEntityType, TCountType>> GetUsage(TCountType[] measures);

		Task<IDictionary<TCountType, ICount<TEntityType, TCountType>>> GetUsageAsync(TCountType[] measures);

		IEnumerable<ICount<TEntityType, TCountType>> GetAllUsages();

		Task<IEnumerable<ICount<TEntityType, TCountType>>> GetAllUsagesAsync();
	}
}
