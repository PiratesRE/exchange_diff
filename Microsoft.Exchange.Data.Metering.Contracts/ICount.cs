using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface ICount<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		ICountedEntity<TEntityType> Entity { get; }

		ICountedConfig Config { get; }

		TCountType Measure { get; }

		bool IsPromoted { get; }

		long Total { get; }

		long Average { get; }

		ITrendline Trend { get; }

		bool TryGetObject(string key, out object value);

		void SetObject(string key, object value);
	}
}
