using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncSetToUnmodifiedStrategy : IAirSyncMissingPropertyStrategy
	{
		public void ExecuteCopyProperty(IProperty srcProperty, AirSyncProperty dstAirSyncProperty)
		{
			if (srcProperty == null)
			{
				throw new ArgumentNullException("srcProperty");
			}
			if (dstAirSyncProperty == null)
			{
				throw new ArgumentNullException("dstAirSyncProperty");
			}
			if (PropertyState.SetToDefault == srcProperty.State)
			{
				dstAirSyncProperty.OutputEmptyNode();
				return;
			}
			if (PropertyState.Unmodified != srcProperty.State)
			{
				dstAirSyncProperty.CopyFrom(srcProperty);
			}
		}

		public void PostProcessPropertyBag(AirSyncDataObject airSyncDataObject)
		{
			if (airSyncDataObject == null)
			{
				throw new ArgumentNullException("airSyncDataObject");
			}
			foreach (IProperty property in airSyncDataObject.Children)
			{
				AirSyncProperty airSyncProperty = (AirSyncProperty)property;
				if (airSyncProperty.State == PropertyState.Uninitialized)
				{
					airSyncProperty.State = PropertyState.Unmodified;
				}
				else if (airSyncProperty.IsBoundToEmptyTag())
				{
					airSyncProperty.State = PropertyState.SetToDefault;
				}
			}
		}

		public void Validate(AirSyncDataObject airSyncDataObject)
		{
		}
	}
}
