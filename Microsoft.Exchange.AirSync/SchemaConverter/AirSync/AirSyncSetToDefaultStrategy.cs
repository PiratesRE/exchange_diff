using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncSetToDefaultStrategy : IAirSyncMissingPropertyStrategy
	{
		public AirSyncSetToDefaultStrategy(Dictionary<string, bool> supportedTags)
		{
			this.supportedTags = supportedTags;
		}

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
			if (PropertyState.Modified == srcProperty.State || PropertyState.Stream == srcProperty.State || PropertyState.SetToDefault == srcProperty.State)
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
					if (this.supportedTags != null && !this.supportedTags.ContainsKey(airSyncProperty.AirSyncTagNames[0]))
					{
						airSyncProperty.State = PropertyState.Unmodified;
					}
					else
					{
						airSyncProperty.State = (airSyncProperty.ClientChangeTracked ? PropertyState.Unmodified : PropertyState.SetToDefault);
					}
				}
			}
		}

		public void Validate(AirSyncDataObject airSyncDataObject)
		{
			if (this.supportedTags != null && airSyncDataObject == null)
			{
				throw new ArgumentNullException("airSyncDataObject");
			}
			if (this.supportedTags != null)
			{
				foreach (IProperty property in airSyncDataObject.Children)
				{
					AirSyncProperty airSyncProperty = (AirSyncProperty)property;
					if (airSyncProperty.RequiresClientSupport && !this.supportedTags.ContainsKey(airSyncProperty.AirSyncTagNames[0]))
					{
						throw new ConversionException("Client must support property: " + airSyncProperty.AirSyncTagNames[0]);
					}
				}
			}
		}

		private Dictionary<string, bool> supportedTags;
	}
}
