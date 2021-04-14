using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class FlexibleSchemaStrategy : IAirSyncMissingPropertyStrategy
	{
		public FlexibleSchemaStrategy(Dictionary<string, bool> schemaTags)
		{
			this.schemaTags = schemaTags;
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
			if (dstAirSyncProperty.AirSyncTagNames == null)
			{
				throw new ArgumentNullException("dstAirSyncProperty.AirSyncTagNames");
			}
			if (PropertyState.Modified == srcProperty.State && (this.schemaTags == null || this.schemaTags.ContainsKey(dstAirSyncProperty.Namespace + dstAirSyncProperty.AirSyncTagNames[0])))
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
				AirSyncDiagnostics.TraceInfo<AirSyncProperty, PropertyState>(ExTraceGlobals.AirSyncTracer, this, "Property={0} State={1}", airSyncProperty, airSyncProperty.State);
			}
		}

		public void Validate(AirSyncDataObject airSyncDataObject)
		{
		}

		private Dictionary<string, bool> schemaTags;
	}
}
