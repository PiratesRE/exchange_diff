using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IDateTimeProperty : IProperty
	{
		ExDateTime DateTime { get; }
	}
}
