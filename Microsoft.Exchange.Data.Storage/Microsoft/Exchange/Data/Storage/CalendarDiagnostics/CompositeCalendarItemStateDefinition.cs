using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CompositeCalendarItemStateDefinition : ICalendarItemStateDefinition, IEquatable<ICalendarItemStateDefinition>, IEqualityComparer<CalendarItemState>, IEquatable<CompositeCalendarItemStateDefinition>
	{
		private CompositeCalendarItemStateDefinition(CompositeCalendarItemStateDefinition.Operator stateOperator, params ICalendarItemStateDefinition[] states)
		{
			Util.ThrowOnNullArgument(states, "states");
			this.stateOperator = stateOperator;
			HashSet<StorePropertyDefinition> hashSet = new HashSet<StorePropertyDefinition>();
			StringBuilder stringBuilder = new StringBuilder(this.stateOperator.ToString());
			this.operands = new List<ICalendarItemStateDefinition>(states.Length);
			foreach (ICalendarItemStateDefinition calendarItemStateDefinition in states)
			{
				if (calendarItemStateDefinition == null)
				{
					throw new InvalidOperationException("Cannot combine null definitions.");
				}
				if (calendarItemStateDefinition.IsRecurringMasterSpecific)
				{
					throw new NotSupportedException("Recurring specific state definitions are not supported.");
				}
				this.operands.Add(calendarItemStateDefinition);
				stringBuilder.Append(calendarItemStateDefinition.SchemaKey);
				foreach (StorePropertyDefinition item in calendarItemStateDefinition.RequiredProperties)
				{
					if (!hashSet.Contains(item))
					{
						hashSet.Add(item);
					}
				}
			}
			this.schemaKey = stringBuilder.ToString();
			int num = 0;
			this.requiredProperties = new StorePropertyDefinition[hashSet.Count];
			foreach (StorePropertyDefinition storePropertyDefinition in hashSet)
			{
				this.requiredProperties[num] = storePropertyDefinition;
				num++;
			}
		}

		public static CompositeCalendarItemStateDefinition GetDeletedFromFolderStateDefinition(byte[] folderId)
		{
			ICalendarItemStateDefinition calendarItemStateDefinition = ActionBasedCalendarItemStateDefinition.CreateDeletedNoneOccurrenceCalendarItemStateDefinition();
			ICalendarItemStateDefinition calendarItemStateDefinition2 = new FolderBasedCalendarItemStateDefinition(folderId);
			return new CompositeCalendarItemStateDefinition(CompositeCalendarItemStateDefinition.Operator.And, new ICalendarItemStateDefinition[]
			{
				calendarItemStateDefinition,
				calendarItemStateDefinition2
			});
		}

		public bool IsRecurringMasterSpecific
		{
			get
			{
				return false;
			}
		}

		public string SchemaKey
		{
			get
			{
				return this.schemaKey;
			}
		}

		public StorePropertyDefinition[] RequiredProperties
		{
			get
			{
				return this.requiredProperties;
			}
		}

		public bool Evaluate(CalendarItemState state, PropertyBag propertyBag, MailboxSession session)
		{
			return this.operands.TrueForAll((ICalendarItemStateDefinition definition) => definition.Evaluate(state, propertyBag, session));
		}

		public bool Equals(ICalendarItemStateDefinition other)
		{
			return other is CompositeCalendarItemStateDefinition && this.Equals((CompositeCalendarItemStateDefinition)other);
		}

		public bool Equals(CompositeCalendarItemStateDefinition other)
		{
			if (other == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (this.stateOperator == other.stateOperator && this.operands.Count == other.operands.Count)
			{
				return this.operands.TrueForAll((ICalendarItemStateDefinition definition) => other.operands.Contains(definition));
			}
			return false;
		}

		public bool Equals(CalendarItemState x, CalendarItemState y)
		{
			return this.operands.TrueForAll((ICalendarItemStateDefinition definition) => definition.Equals(x, y));
		}

		public int GetHashCode(CalendarItemState obj)
		{
			if (obj == null)
			{
				return 0;
			}
			int num = 0;
			foreach (ICalendarItemStateDefinition calendarItemStateDefinition in this.operands)
			{
				num = (num << 1 ^ calendarItemStateDefinition.GetHashCode(obj));
			}
			return num;
		}

		private List<ICalendarItemStateDefinition> operands;

		private CompositeCalendarItemStateDefinition.Operator stateOperator;

		private string schemaKey;

		private StorePropertyDefinition[] requiredProperties;

		internal enum Operator
		{
			And
		}
	}
}
