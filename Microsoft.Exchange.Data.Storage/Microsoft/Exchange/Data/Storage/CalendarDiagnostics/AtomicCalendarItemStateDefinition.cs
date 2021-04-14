using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AtomicCalendarItemStateDefinition<TValue> : ICalendarItemStateDefinition, IEquatable<ICalendarItemStateDefinition>, IEqualityComparer<CalendarItemState>
	{
		public AtomicCalendarItemStateDefinition(string keyPart)
		{
			this.stateKey = string.Format("{0}({1})", this.SchemaKey, keyPart);
		}

		protected abstract TValue GetValueFromPropertyBag(PropertyBag propertyBag, MailboxSession session);

		protected abstract bool Evaluate(TValue value);

		public abstract bool IsRecurringMasterSpecific { get; }

		public abstract string SchemaKey { get; }

		public abstract StorePropertyDefinition[] RequiredProperties { get; }

		public bool Evaluate(CalendarItemState state, PropertyBag propertyBag, MailboxSession session)
		{
			Util.ThrowOnNullArgument(state, "state");
			Util.ThrowOnNullArgument(propertyBag, "propertyBag");
			Util.ThrowOnNullArgument(session, "session");
			TValue tvalue;
			if (state.ContainsKey(this.stateKey))
			{
				tvalue = (TValue)((object)state[this.stateKey]);
			}
			else
			{
				tvalue = this.GetValueFromPropertyBag(propertyBag, session);
				state[this.stateKey] = tvalue;
			}
			return this.Evaluate(tvalue);
		}

		public abstract bool Equals(ICalendarItemStateDefinition other);

		public bool Equals(CalendarItemState x, CalendarItemState y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			if (!x.ContainsKey(this.stateKey) || !y.ContainsKey(this.stateKey))
			{
				throw new ArgumentException("The states don't have the required data.");
			}
			object obj = x[this.stateKey];
			object obj2 = y[this.stateKey];
			if (obj == null)
			{
				return obj2 == null;
			}
			return obj.Equals(obj2);
		}

		public int GetHashCode(CalendarItemState obj)
		{
			if (obj == null || !obj.ContainsKey(this.stateKey))
			{
				return 0;
			}
			return obj[this.stateKey].GetHashCode();
		}

		private string stateKey;
	}
}
