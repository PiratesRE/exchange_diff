using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SyncPeople : SyncPersonaContactsBase<SyncPeopleRequest, SyncPeopleResponseMessage>
	{
		public SyncPeople(CallContext callContext, SyncPeopleRequest request) : base(callContext, request, SyncPeople.syncStateInfo)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			if (base.Result.Value == null)
			{
				return new SyncPeopleResponseMessage(base.Result.Code, base.Result.Error);
			}
			return base.Result.Value;
		}

		protected override SyncPeopleResponseMessage ExecuteAndGetResult(AllContactsCursor cursor)
		{
			SyncPeople.tracer.TraceDebug((long)this.GetHashCode(), "SyncPeople: Begin executing");
			base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.SyncPersonaContactsType, "SyncPeople");
			base.SplitSyncStates(base.Request.SyncState, null);
			base.PopulateFolderListAndCheckIfChanged();
			if (string.IsNullOrEmpty(base.Request.SyncState))
			{
				this.querySyncInProgress = true;
				base.GetIcsCatchUpStates();
			}
			if (this.querySyncInProgress)
			{
				base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.QuerySync, true);
				base.DoQuerySync(cursor);
			}
			if (!this.querySyncInProgress)
			{
				base.CallContext.ProtocolLog.Set(SyncPersonaContactsMetadata.IcsSync, true);
				base.DoIcsSync(cursor);
			}
			SyncPeopleResponseMessage syncPeopleResponseMessage = new SyncPeopleResponseMessage();
			syncPeopleResponseMessage.SyncState = base.JoinSyncStates(new KeyValuePair<int, string>[0]);
			syncPeopleResponseMessage.People = new Persona[this.people.Count];
			this.people.Values.CopyTo(syncPeopleResponseMessage.People, 0);
			syncPeopleResponseMessage.DeletedPeople = this.deletedPeople.ToArray();
			syncPeopleResponseMessage.JumpHeaderSortKeys = base.GetJumpHeaderSortKeys();
			syncPeopleResponseMessage.SortKeyVersion = PeopleStringUtils.ComputeSortVersion(this.mailboxSession.PreferedCulture);
			syncPeopleResponseMessage.IncludesLastItemInRange = this.includesLastItemInRange;
			SyncPeople.tracer.TraceDebug((long)this.GetHashCode(), "SyncPeople: Done executing");
			return syncPeopleResponseMessage;
		}

		protected override void AddContacts(PersonId personId, List<IStorePropertyBag> contacts)
		{
			SyncPeople.tracer.TraceDebug<int, PersonId>((long)this.GetHashCode(), "SyncPeople: Adding {0} contacts for {1}", contacts.Count, personId);
			Person person = Person.LoadFromContacts(personId, contacts, this.mailboxSession, this.contactPropertyList, null);
			Dictionary<PropertyDefinition, object> properties = Persona.GetProperties(person.PropertyBag, this.personaPropertyDefinitions);
			Persona persona = Persona.LoadFromPropertyBag(this.mailboxSession, properties, this.personaPropertyList);
			this.people[persona.PersonaId] = persona;
			this.currentReturnSize++;
		}

		private const string ClassName = "SyncPeople";

		private static Trace tracer = ExTraceGlobals.SyncPeopleCallTracer;

		private readonly Dictionary<ItemId, Persona> people = new Dictionary<ItemId, Persona>();

		private static readonly SyncPersonaContactsBase<SyncPeopleRequest, SyncPeopleResponseMessage>.SyncStateInfo syncStateInfo = new SyncPersonaContactsBase<SyncPeopleRequest, SyncPeopleResponseMessage>.SyncStateInfo
		{
			VersionPrefix = "SR1",
			VersionIndex = 0,
			QuerySyncInProgressIndex = 1,
			LastPersonIdIndex = 2,
			MyContactsHashIndex = 3,
			SyncStatesStartIndex = 4
		};
	}
}
