using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoUtcDtStampProperty : XsoUtcDateTimeProperty
	{
		public XsoUtcDtStampProperty() : base(null)
		{
		}

		public XsoUtcDtStampProperty(PropertyType type) : base(null, type)
		{
		}

		public override ExDateTime DateTime
		{
			get
			{
				CalendarItemBase calendarItemBase = base.XsoItem as CalendarItemBase;
				if (calendarItemBase == null)
				{
					string message = string.Format("[XsoUtcDtStampProperty.get_DateTime] XsoItem is NOT a CalendarItemBase (cast resulted in null).  Actual item type: {0}", (base.XsoItem == null) ? "<NULL>" : base.XsoItem.GetType().FullName);
					AirSyncDiagnostics.TraceError(ExTraceGlobals.XsoTracer, this, message);
					throw new InvalidOperationException(message);
				}
				object obj = calendarItemBase.TryGetProperty(CalendarItemBaseSchema.OwnerCriticalChangeTime);
				if (obj != null && obj is ExDateTime)
				{
					return ExTimeZone.UtcTimeZone.ConvertDateTime((ExDateTime)obj);
				}
				return ExTimeZone.UtcTimeZone.ConvertDateTime(calendarItemBase.LastModifiedTime);
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			if (PropertyState.SetToDefault == srcProperty.State)
			{
				throw new ConversionException("Object type does not support setting to default");
			}
			ExDateTime exDateTime = ((IDateTimeProperty)srcProperty).DateTime;
			exDateTime = ExTimeZone.UtcTimeZone.ConvertDateTime(exDateTime);
			base.XsoItem[StoreObjectSchema.LastModifiedTime] = exDateTime;
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			base.XsoItem.DeleteProperties(new PropertyDefinition[]
			{
				StoreObjectSchema.LastModifiedTime
			});
		}
	}
}
