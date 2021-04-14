using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface ITimeZoneProperty : IProperty
	{
		ExTimeZone TimeZone { get; }

		ExDateTime EffectiveTime { get; }
	}
}
