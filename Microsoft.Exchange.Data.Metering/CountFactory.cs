using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal static class CountFactory
	{
		public static Count<TEntityType, TCountType> CreateCount<TEntityType, TCountType>(ICountedEntity<TEntityType> entity, TCountType measure, ICountedConfig config) where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
		{
			return CountFactory.CreateCount<TEntityType, TCountType>(entity, measure, config, () => DateTime.UtcNow);
		}

		public static Count<TEntityType, TCountType> CreateCount<TEntityType, TCountType>(ICountedEntity<TEntityType> entity, TCountType measure, ICountedConfig config, Func<DateTime> timeProvider) where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
		{
			if (config is IRollingCountConfig)
			{
				return new RollingCount<TEntityType, TCountType>(entity, (IRollingCountConfig)config, measure, timeProvider);
			}
			if (config is IAbsoluteCountConfig)
			{
				return new AbsoluteCount<TEntityType, TCountType>(entity, (IAbsoluteCountConfig)config, measure, timeProvider);
			}
			throw new InvalidOperationException("Need to create a config of a subtype class of ICountedConfig");
		}
	}
}
