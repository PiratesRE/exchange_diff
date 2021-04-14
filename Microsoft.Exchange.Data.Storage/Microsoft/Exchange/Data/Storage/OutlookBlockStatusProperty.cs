using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class OutlookBlockStatusProperty : SmartPropertyDefinition
	{
		internal OutlookBlockStatusProperty() : base("OutlookBlockStatusProperty", typeof(BlockStatus), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.NativeBlockStatus, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ReceivedTime, PropertyDependencyType.AllRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			int valueOrDefault = propertyBag.GetValueOrDefault<int>(InternalSchema.NativeBlockStatus);
			if (valueOrDefault < 0)
			{
				return new PropertyError(this, PropertyErrorCode.GetCalculatedPropertyError);
			}
			if (valueOrDefault <= 3)
			{
				return (BlockStatus)valueOrDefault;
			}
			ExDateTime valueOrDefault2 = propertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.ReceivedTime, ExDateTime.MinValue);
			if (!(valueOrDefault2 != ExDateTime.MinValue))
			{
				return BlockStatus.DontKnow;
			}
			double floatDate = ExTimeZone.UtcTimeZone.ConvertDateTime(valueOrDefault2).ToOADate();
			int num = OutlookBlockStatusProperty.ComputeBlockStatus(floatDate);
			int num2 = (num >= valueOrDefault) ? (num - valueOrDefault) : (valueOrDefault - num);
			if (num2 < 1)
			{
				return BlockStatus.NoNeverAgain;
			}
			return BlockStatus.DontKnow;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("OutlookBlockStatus");
			}
			EnumValidator.ThrowIfInvalid<BlockStatus>((BlockStatus)value, "value");
			int num = (int)value;
			if (num < 3)
			{
				propertyBag.SetValueWithFixup(InternalSchema.NativeBlockStatus, (BlockStatus)num);
				return;
			}
			ExDateTime valueOrDefault = propertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.ReceivedTime, ExDateTime.MinValue);
			if (valueOrDefault != ExDateTime.MinValue)
			{
				double floatDate = ExTimeZone.UtcTimeZone.ConvertDateTime(valueOrDefault).ToOADate();
				int num2 = OutlookBlockStatusProperty.ComputeBlockStatus(floatDate);
				propertyBag.SetValueWithFixup(InternalSchema.NativeBlockStatus, (BlockStatus)num2);
				return;
			}
			propertyBag.SetValueWithFixup(InternalSchema.NativeBlockStatus, BlockStatus.DontKnow);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, InternalSchema.NativeBlockStatus);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, InternalSchema.NativeBlockStatus);
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ComparisonFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ExistsFilter));
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return InternalSchema.NativeBlockStatus;
		}

		private static int ComputeBlockStatus(double floatDate)
		{
			return (int)((floatDate - Math.Floor(floatDate)) * 100000000.0) + 3;
		}

		private const int BlockStatusErr = 1;
	}
}
