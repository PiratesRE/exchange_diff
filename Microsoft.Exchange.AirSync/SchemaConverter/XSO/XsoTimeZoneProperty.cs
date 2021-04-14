using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoTimeZoneProperty : XsoProperty, ITimeZoneProperty, IProperty
	{
		public XsoTimeZoneProperty() : base(null, new PropertyDefinition[]
		{
			CalendarItemInstanceSchema.StartTime,
			CalendarItemBaseSchema.TimeZoneBlob,
			CalendarItemBaseSchema.TimeZoneDefinitionRecurring,
			ItemSchema.TimeZoneDefinitionStart,
			CalendarItemBaseSchema.TimeZoneDefinitionEnd
		})
		{
		}

		public XsoTimeZoneProperty(PropertyType type) : base(null, type, new PropertyDefinition[]
		{
			CalendarItemInstanceSchema.StartTime,
			CalendarItemBaseSchema.TimeZoneBlob,
			CalendarItemBaseSchema.TimeZoneDefinitionRecurring,
			ItemSchema.TimeZoneDefinitionStart,
			CalendarItemBaseSchema.TimeZoneDefinitionEnd
		})
		{
		}

		public ExTimeZone TimeZone
		{
			get
			{
				ExTimeZone promotedTimeZoneFromItem = TimeZoneHelper.GetPromotedTimeZoneFromItem(base.XsoItem as Item);
				if (promotedTimeZoneFromItem != null)
				{
					return promotedTimeZoneFromItem;
				}
				return ExTimeZone.CurrentTimeZone;
			}
		}

		public ExDateTime EffectiveTime
		{
			get
			{
				return (ExDateTime)base.XsoItem.TryGetProperty(CalendarItemInstanceSchema.StartTime);
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			ITimeZoneProperty timeZoneProperty = srcProperty as ITimeZoneProperty;
			if (timeZoneProperty == null)
			{
				throw new UnexpectedTypeException("ITimeZoneProperty", srcProperty);
			}
			if (!this.TimeZoneExistsOnItem() || !TimeZoneConverter.IsClientTimeZoneEquivalentToServerTimeZoneRule(timeZoneProperty.TimeZone, this.TimeZone, this.EffectiveTime))
			{
				base.XsoItem[CalendarItemBaseSchema.StartTimeZone] = timeZoneProperty.TimeZone;
				base.XsoItem[CalendarItemBaseSchema.EndTimeZone] = timeZoneProperty.TimeZone;
				base.XsoItem.Delete(CalendarItemBaseSchema.TimeZoneBlob);
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			if (!(srcProperty is ITimeZoneProperty))
			{
				throw new UnexpectedTypeException("ITimeZoneProperty", srcProperty);
			}
		}

		internal bool TimeZoneExistsOnItem()
		{
			return base.XsoItem.GetValueOrDefault<byte[]>(ItemSchema.TimeZoneDefinitionStart) != null;
		}
	}
}
