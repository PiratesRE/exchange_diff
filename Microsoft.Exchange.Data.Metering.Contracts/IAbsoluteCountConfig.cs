using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface IAbsoluteCountConfig : ICountedConfig
	{
		TimeSpan HistoryLookbackWindow { get; }
	}
}
