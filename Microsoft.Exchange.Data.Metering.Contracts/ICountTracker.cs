using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface ICountTracker<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		void AddUsage(ICountedEntity<TEntityType> entity, TCountType measure, ICountedConfig countConfig, long increment);

		Task AddUsageAsync(ICountedEntity<TEntityType> entity, TCountType measure, ICountedConfig countConfig, long increment);

		bool TrySetUsage(ICountedEntity<TEntityType> entity, TCountType measure, long value);

		Task<bool> SetUsageAsync(ICountedEntity<TEntityType> entity, TCountType measure, long value);

		ICount<TEntityType, TCountType> GetUsage(ICountedEntity<TEntityType> entity, TCountType measure);

		Task<ICount<TEntityType, TCountType>> GetUsageAsync(ICountedEntity<TEntityType> entity, TCountType measure);

		IDictionary<TCountType, ICount<TEntityType, TCountType>> GetUsage(ICountedEntity<TEntityType> entity, TCountType[] measures);

		Task<IDictionary<TCountType, ICount<TEntityType, TCountType>>> GetUsageAsync(ICountedEntity<TEntityType> entity, TCountType[] measures);

		IEnumerable<ICount<TEntityType, TCountType>> GetAllUsages(ICountedEntity<TEntityType> entity);

		Task<IEnumerable<ICount<TEntityType, TCountType>>> GetAllUsagesAsync(ICountedEntity<TEntityType> entity);

		bool TryGetEntityObject(ICountedEntity<TEntityType> entity, out ICountedEntityWrapper<TEntityType, TCountType> wrapper);

		ICountedConfig GetConfig(ICountedEntity<TEntityType> entity, TCountType measure);

		IEnumerable<ICount<TEntityType, TCountType>> Filter(Func<ICount<TEntityType, TCountType>, bool> isMatch);

		Task<IEnumerable<ICount<TEntityType, TCountType>>> FilterAsync(Func<ICount<TEntityType, TCountType>, bool> isMatch);

		IEnumerable<ICountedEntityValue<TEntityType, TCountType>> Filter(Func<ICountedEntityValue<TEntityType, TCountType>, bool> isMatch);

		Task<IEnumerable<ICountedEntityValue<TEntityType, TCountType>>> FilterAsync(Func<ICountedEntityValue<TEntityType, TCountType>, bool> isMatch);

		void GetDiagnosticInfo(string argument, XElement element);

		XElement GetDiagnosticInfo(IEntityName<TEntityType> entity);
	}
}
