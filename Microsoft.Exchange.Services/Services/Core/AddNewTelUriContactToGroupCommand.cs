using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class AddNewTelUriContactToGroupCommand : SingleStepServiceCommand<AddNewTelUriContactToGroupRequest, Persona>
	{
		public AddNewTelUriContactToGroupCommand(CallContext callContext, AddNewTelUriContactToGroupRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new AddNewTelUriContactToGroupResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<Persona> Execute()
		{
			string telUriAddress = base.Request.TelUriAddress;
			string imContactSipUriAddress = base.Request.ImContactSipUriAddress;
			string imTelephoneNumber = base.Request.ImTelephoneNumber;
			StoreId groupId = null;
			if (base.Request.GroupId != null)
			{
				IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(base.Request.GroupId);
				groupId = idAndSession.Id;
			}
			MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			PersonId personId = new AddNewTelUriContactToGroup(mailboxIdentityMailboxSession, telUriAddress, imContactSipUriAddress, imTelephoneNumber, groupId, new XSOFactory(), Global.UnifiedContactStoreConfiguration).Execute();
			ItemId personaId = IdConverter.PersonaIdFromPersonId(mailboxIdentityMailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, personId);
			Persona value = Persona.LoadFromPersonaId(mailboxIdentityMailboxSession, base.CallContext.ADRecipientSessionContext.GetGALScopedADRecipientSession(base.CallContext.EffectiveCaller.ClientSecurityContext), personaId, Persona.FullPersonaShape, null, null);
			return new ServiceResult<Persona>(value);
		}
	}
}
