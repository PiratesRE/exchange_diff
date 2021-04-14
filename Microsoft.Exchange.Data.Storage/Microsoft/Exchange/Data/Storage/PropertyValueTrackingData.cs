using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PropertyValueTrackingData
	{
		public PropertyValueTrackingData(PropertyTrackingInformation changeType, object originalValue)
		{
			EnumValidator.ThrowIfInvalid<PropertyTrackingInformation>(changeType, "changeType");
			this.propertyValueState = changeType;
			this.originalPropertyValue = originalValue;
		}

		internal PropertyTrackingInformation PropertyValueState
		{
			get
			{
				return this.propertyValueState;
			}
		}

		internal object OriginalPropertyValue
		{
			get
			{
				return this.originalPropertyValue;
			}
		}

		public static readonly PropertyValueTrackingData PropertyValueTrackDataNotTracked = new PropertyValueTrackingData(PropertyTrackingInformation.NotTracked, null);

		public static readonly PropertyValueTrackingData PropertyValueTrackDataUnchanged = new PropertyValueTrackingData(PropertyTrackingInformation.Unchanged, null);

		private readonly PropertyTrackingInformation propertyValueState;

		private readonly object originalPropertyValue;
	}
}
