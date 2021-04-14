using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal interface IAirSyncMissingPropertyStrategy
	{
		void ExecuteCopyProperty(IProperty srcProperty, AirSyncProperty dstAirSyncProperty);

		void PostProcessPropertyBag(AirSyncDataObject propertyBag);

		void Validate(AirSyncDataObject airSyncDataObject);
	}
}
