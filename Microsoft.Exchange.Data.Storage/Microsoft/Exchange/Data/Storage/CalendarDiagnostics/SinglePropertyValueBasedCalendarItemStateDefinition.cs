using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SinglePropertyValueBasedCalendarItemStateDefinition<TValue> : AtomicCalendarItemStateDefinition<TValue>, IEquatable<SinglePropertyValueBasedCalendarItemStateDefinition<TValue>>
	{
		public StorePropertyDefinition TargetProperty { get; private set; }

		public HashSet<TValue> TargetValueSet { get; private set; }

		private static HashSet<TValue> GetValueSetFromSingleValue(TValue singleTargetValue, IEqualityComparer<TValue> equalityComparer)
		{
			return new HashSet<TValue>(equalityComparer)
			{
				singleTargetValue
			};
		}

		public SinglePropertyValueBasedCalendarItemStateDefinition(StorePropertyDefinition targetProperty, HashSet<TValue> targetValueSet) : base(targetProperty.Name)
		{
			Util.ThrowOnNullArgument(targetValueSet, "targetValueSet");
			this.TargetProperty = targetProperty;
			this.TargetValueSet = targetValueSet;
		}

		public SinglePropertyValueBasedCalendarItemStateDefinition(StorePropertyDefinition targetProperty, TValue targetValue) : this(targetProperty, targetValue, null)
		{
		}

		public SinglePropertyValueBasedCalendarItemStateDefinition(StorePropertyDefinition targetProperty, TValue targetValue, IEqualityComparer<TValue> equalityComparer) : this(targetProperty, SinglePropertyValueBasedCalendarItemStateDefinition<TValue>.GetValueSetFromSingleValue(targetValue, equalityComparer))
		{
		}

		private bool IsTargetValueSetsEqual(SinglePropertyValueBasedCalendarItemStateDefinition<TValue> first, SinglePropertyValueBasedCalendarItemStateDefinition<TValue> second)
		{
			if (first.TargetValueSet.Count == second.TargetValueSet.Count)
			{
				foreach (TValue item in first.TargetValueSet)
				{
					if (!second.TargetValueSet.Contains(item))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		protected UnderlyingType GetUnderlyingValue<UnderlyingType>(PropertyBag propertyBag)
		{
			return propertyBag.GetValueOrDefault<UnderlyingType>(this.TargetProperty);
		}

		protected override TValue GetValueFromPropertyBag(PropertyBag propertyBag, MailboxSession session)
		{
			return this.GetUnderlyingValue<TValue>(propertyBag);
		}

		public override bool IsRecurringMasterSpecific
		{
			get
			{
				return false;
			}
		}

		protected override bool Evaluate(TValue value)
		{
			return this.TargetValueSet.Contains(value);
		}

		public override bool Equals(ICalendarItemStateDefinition other)
		{
			return other is SinglePropertyValueBasedCalendarItemStateDefinition<TValue> && this.Equals((SinglePropertyValueBasedCalendarItemStateDefinition<TValue>)other);
		}

		public bool Equals(SinglePropertyValueBasedCalendarItemStateDefinition<TValue> other)
		{
			return other != null && (object.ReferenceEquals(this, other) || (this.TargetProperty.Equals(other.TargetProperty) && this.IsTargetValueSetsEqual(this, other)));
		}
	}
}
