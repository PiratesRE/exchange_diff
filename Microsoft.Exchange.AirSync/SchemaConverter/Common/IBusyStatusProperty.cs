using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IBusyStatusProperty : IProperty
	{
		BusyType BusyStatus { get; }
	}
}
