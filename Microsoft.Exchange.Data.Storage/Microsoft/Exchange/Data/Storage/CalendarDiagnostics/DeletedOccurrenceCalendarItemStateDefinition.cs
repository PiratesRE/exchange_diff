using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DeletedOccurrenceCalendarItemStateDefinition : AtomicCalendarItemStateDefinition<bool>, IEquatable<DeletedOccurrenceCalendarItemStateDefinition>
	{
		public ExDateTime OccurrenceDateId { get; private set; }

		public bool IsOccurrencePresent { get; private set; }

		public DeletedOccurrenceCalendarItemStateDefinition(ExDateTime occurrenceDateId, bool isOccurrencePresent) : base(occurrenceDateId.ToShortDateString())
		{
			this.OccurrenceDateId = occurrenceDateId;
			this.IsOccurrencePresent = isOccurrencePresent;
		}

		public override bool IsRecurringMasterSpecific
		{
			get
			{
				return true;
			}
		}

		public override string SchemaKey
		{
			get
			{
				return "{77D2323B-C6EE-44b7-A00E-CEB0465DC109}";
			}
		}

		public override StorePropertyDefinition[] RequiredProperties
		{
			get
			{
				return DeletedOccurrenceCalendarItemStateDefinition.requiredProperties;
			}
		}

		protected override bool GetValueFromPropertyBag(PropertyBag propertyBag, MailboxSession session)
		{
			Recurrence recurrence;
			return Recurrence.TryFromMasterPropertyBag(propertyBag, session, out recurrence) && recurrence.IsValidOccurrenceId(this.OccurrenceDateId) && !recurrence.IsOccurrenceDeleted(this.OccurrenceDateId);
		}

		protected override bool Evaluate(bool value)
		{
			return this.IsOccurrencePresent == value;
		}

		public override bool Equals(ICalendarItemStateDefinition other)
		{
			return other is DeletedOccurrenceCalendarItemStateDefinition && this.Equals((DeletedOccurrenceCalendarItemStateDefinition)other);
		}

		public bool Equals(DeletedOccurrenceCalendarItemStateDefinition other)
		{
			return other != null && (object.ReferenceEquals(this, other) || this.OccurrenceDateId.Equals(other.OccurrenceDateId));
		}

		private static readonly StorePropertyDefinition[] requiredProperties = ((IEnumerable<StorePropertyDefinition>)Recurrence.RequiredRecurrenceProperties).Concat(new StorePropertyDefinition[]
		{
			CalendarItemBaseSchema.ClientIntent
		}).ToArray<StorePropertyDefinition>();
	}
}
