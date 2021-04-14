using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface ICountedEntityValue<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		ICountedEntity<TEntityType> Entity { get; }

		ICount<TEntityType, TCountType> GetUsage(TCountType measure);

		bool HasUsage(TCountType measure);
	}
}
