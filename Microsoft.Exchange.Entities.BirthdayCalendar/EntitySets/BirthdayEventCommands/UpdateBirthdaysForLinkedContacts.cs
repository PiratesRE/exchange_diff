using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets.BirthdayEventCommands
{
	internal class UpdateBirthdaysForLinkedContacts : EntityCommand<IBirthdayEvents, IBirthdayEvent>, IBirthdayEventCommand
	{
		internal UpdateBirthdaysForLinkedContacts(IEnumerable<IBirthdayContact> linkedContacts, IBirthdayEvents scope)
		{
			this.Trace.TraceDebug((long)this.GetHashCode(), "UpdateBirthdaysForLinkedContacts:Constructor");
			this.LinkedContacts = linkedContacts;
			this.Scope = scope;
		}

		public IEnumerable<IBirthdayContact> LinkedContacts { get; private set; }

		protected override ITracer Trace
		{
			get
			{
				return UpdateBirthdaysForLinkedContacts.UpdateBirthdaysForLinkedContactsTracer;
			}
		}

		private BirthdayEventCommandResult Result { get; set; }

		public BirthdayEventCommandResult ExecuteAndGetResult()
		{
			base.Execute(null);
			return this.Result;
		}

		protected override IBirthdayEvent OnExecute()
		{
			this.Trace.TraceDebug((long)this.GetHashCode(), "Updating birthdays for linked contacts");
			this.Result = new BirthdayEventCommandResult();
			IEnumerable<IGrouping<ExDateTime?, IBirthdayContact>> enumerable = from eachContact in this.LinkedContacts
			group eachContact by eachContact.Birthday;
			foreach (IGrouping<ExDateTime?, IBirthdayContact> grouping in enumerable)
			{
				bool flag = false;
				foreach (IBirthdayContact birthdayContact in grouping)
				{
					if (birthdayContact.Birthday != null && !birthdayContact.ShouldHideBirthday && !flag)
					{
						flag = true;
						IBirthdayEvent birthdayEvent = this.FindBirthdayEventForContact(birthdayContact);
						this.Result.MergeWith((birthdayEvent == null) ? this.Scope.CreateBirthdayEventForContact(birthdayContact) : this.Scope.UpdateBirthdayEventForContact(birthdayEvent, birthdayContact));
					}
					else
					{
						this.Result.MergeWith(this.Scope.DeleteBirthdayEventForContact(birthdayContact));
					}
				}
			}
			return null;
		}

		private IBirthdayEvent FindBirthdayEventForContact(IBirthdayContact contact)
		{
			IEnumerable<BirthdayEvent> enumerable = this.Scope.BirthdayEventDataProvider.FindBirthdayEventsForContact(contact);
			IBirthdayEvent result = null;
			if (enumerable != null)
			{
				BirthdayEvent[] array = (enumerable as BirthdayEvent[]) ?? enumerable.ToArray<BirthdayEvent>();
				if (array.Length > 1)
				{
					this.Trace.TraceError((long)this.GetHashCode(), "Found multiple birthday events for a single contact.");
					this.Scope.BirthdayEventDataProvider.DeleteBirthdayEvents(array);
				}
				else
				{
					result = array.FirstOrDefault<BirthdayEvent>();
				}
			}
			return result;
		}

		private static readonly ITracer UpdateBirthdaysForLinkedContactsTracer = ExTraceGlobals.UpdateBirthdaysForLinkedContactsTracer;
	}
}
