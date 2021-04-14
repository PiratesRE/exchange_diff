using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncBusyStatusProperty : AirSyncProperty, IBusyStatusProperty, IProperty
	{
		public AirSyncBusyStatusProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public BusyType BusyStatus
		{
			get
			{
				string innerText;
				if ((innerText = base.XmlNode.InnerText) != null)
				{
					if (innerText == "0")
					{
						return BusyType.Free;
					}
					if (innerText == "1")
					{
						return BusyType.Tentative;
					}
					if (innerText == "2")
					{
						return BusyType.Busy;
					}
					if (innerText == "3")
					{
						return BusyType.OOF;
					}
				}
				throw new ConversionException("Incorrectly-formatted busy status");
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IBusyStatusProperty busyStatusProperty = srcProperty as IBusyStatusProperty;
			if (busyStatusProperty == null)
			{
				throw new UnexpectedTypeException("IBusyStatusProperty", srcProperty);
			}
			switch (busyStatusProperty.BusyStatus)
			{
			case BusyType.Free:
				base.CreateAirSyncNode("0");
				return;
			case BusyType.Tentative:
				base.CreateAirSyncNode("1");
				return;
			case BusyType.Busy:
				base.CreateAirSyncNode("2");
				return;
			case BusyType.OOF:
				base.CreateAirSyncNode("3");
				return;
			default:
				AirSyncDiagnostics.TraceDebug<BusyType>(ExTraceGlobals.CommonTracer, this, "Remapping unknown BusyStatus {0} to 0 (Free).", busyStatusProperty.BusyStatus);
				base.CreateAirSyncNode("0");
				return;
			}
		}
	}
}
