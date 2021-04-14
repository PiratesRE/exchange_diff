using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal interface IAirSyncDataObjectGenerator : IDataObjectGenerator
	{
		AirSyncDataObject GetInnerAirSyncDataObject(IAirSyncMissingPropertyStrategy strategy);
	}
}
