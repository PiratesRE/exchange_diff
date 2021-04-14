using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetPersonaSuggestionsCommand : ServiceCommand<ItemId[]>
	{
		public GetPersonaSuggestionsCommand(CallContext callContext, ItemId personaId) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(personaId, "personaId", "GetPersonaSuggestions::GetPersonaSuggestions");
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.personaId = personaId;
		}

		protected override ItemId[] InternalExecute()
		{
			if (this.personaId == null || string.IsNullOrEmpty(this.personaId.Id))
			{
				throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
			}
			ItemId[] result;
			if (IdConverter.EwsIdIsActiveDirectoryObject(this.personaId.GetId()))
			{
				result = Array<ItemId>.Empty;
			}
			else
			{
				PersonId personId = IdConverter.EwsIdToPersonId(this.personaId.GetId());
				Person person = Person.Load(this.session, personId, null, Person.SuggestionProperties, null);
				result = GetPersonaSuggestionsCommand.ConvertToItemIdArray(this.session, person.GetSuggestions());
			}
			return result;
		}

		private static ItemId[] ConvertToItemIdArray(MailboxSession mailboxSession, IEnumerable<PersonId> personIds)
		{
			if (personIds == null)
			{
				return GetPersonaSuggestionsCommand.NoSuggestionsResult;
			}
			List<ItemId> list = new List<ItemId>();
			foreach (PersonId personId in personIds)
			{
				string id = IdConverter.PersonIdToEwsId(mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, personId);
				ItemId item = new ItemId(id, null);
				list.Add(item);
			}
			if (list.Count > 0)
			{
				return list.ToArray();
			}
			return GetPersonaSuggestionsCommand.NoSuggestionsResult;
		}

		private static readonly ItemId[] NoSuggestionsResult = new ItemId[0];

		private readonly MailboxSession session;

		private readonly ItemId personaId;
	}
}
