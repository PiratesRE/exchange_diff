using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoBusyStatusProperty : XsoProperty, IBusyStatusProperty, IProperty
	{
		public XsoBusyStatusProperty() : base(CalendarItemBaseSchema.FreeBusyStatus)
		{
		}

		public XsoBusyStatusProperty(BusyType defaultValue) : base(CalendarItemBaseSchema.FreeBusyStatus)
		{
			this.defaultValue = defaultValue;
		}

		public XsoBusyStatusProperty(BusyType defaultValue, PropertyType type) : base(CalendarItemBaseSchema.FreeBusyStatus, type)
		{
			this.defaultValue = defaultValue;
		}

		public BusyType BusyStatus
		{
			get
			{
				return (BusyType)base.XsoItem.TryGetProperty(base.PropertyDef);
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			base.XsoItem[base.PropertyDef] = ((IBusyStatusProperty)srcProperty).BusyStatus;
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			base.XsoItem[base.PropertyDef] = this.defaultValue;
		}

		private BusyType defaultValue;
	}
}
