using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class AppointmentStateProperty : SmartPropertyDefinition
	{
		internal AppointmentStateProperty() : base("AppointmentState", typeof(AppointmentStateFlags), PropertyFlags.None, Array<PropertyDefinitionConstraint>.Empty, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.AppointmentStateInternal, PropertyDependencyType.AllRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.GetValue(InternalSchema.AppointmentStateInternal);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			AppointmentStateFlags? valueAsNullable = propertyBag.GetValueAsNullable<AppointmentStateFlags>(InternalSchema.AppointmentStateInternal);
			AppointmentStateFlags appointmentStateFlags = (AppointmentStateFlags)value;
			if (valueAsNullable != null && (valueAsNullable.Value & AppointmentStateFlags.Received) == AppointmentStateFlags.Received && (appointmentStateFlags & AppointmentStateFlags.Received) != AppointmentStateFlags.Received)
			{
				propertyBag.SetLocationIdentifier(63651U, LastChangeAction.SmartPropertyFixup);
				ExTraceGlobals.StorageTracer.TraceInformation(63651, (long)propertyBag.GetHashCode(), "Prevent from removing Received flag on AppointmentState");
				appointmentStateFlags |= AppointmentStateFlags.Received;
			}
			propertyBag.SetValueWithFixup(InternalSchema.AppointmentStateInternal, appointmentStateFlags);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return base.SinglePropertySmartFilterToNativeFilter(filter, InternalSchema.AppointmentStateInternal);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return base.SinglePropertyNativeFilterToSmartFilter(filter, InternalSchema.AppointmentStateInternal);
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
			return InternalSchema.AppointmentStateInternal;
		}
	}
}
