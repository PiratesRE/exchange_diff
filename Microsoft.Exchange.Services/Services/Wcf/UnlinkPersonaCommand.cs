using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class UnlinkPersonaCommand : ServiceCommand<Persona>
	{
		public UnlinkPersonaCommand(CallContext callContext, ItemId personaId, ItemId contactId) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(personaId, "personaId", "UnlinkPersona::UnlinkPersona");
			WcfServiceCommandBase.ThrowIfNull(contactId, "contactId", "UnlinkPersona::UnlinkPersona");
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.personaId = personaId;
			this.contactId = contactId;
		}

		protected override Persona InternalExecute()
		{
			PersonId personId = IdConverter.EwsIdToPersonId(this.personaId.GetId());
			MailboxInfoForLinking mailboxInfo = MailboxInfoForLinking.CreateFromMailboxSession(this.session);
			ContactLinkingPerformanceTracker performanceTracker = new ContactLinkingPerformanceTracker(this.session);
			ContactLinkingLogger logger = new ContactLinkingLogger("UnlinkPersonaCommand", mailboxInfo);
			ManualLink manualLink = new ManualLink(mailboxInfo, logger, performanceTracker);
			if (IdConverter.EwsIdIsActiveDirectoryObject(this.contactId.Id))
			{
				ADObjectId adobjectId = IdConverter.EwsIdToADObjectId(this.contactId.Id);
				manualLink.Unlink(this.session, personId, adobjectId.ObjectGuid);
			}
			else
			{
				StoreId storeId = IdConverter.EwsIdToMessageStoreObjectId(this.contactId.Id);
				VersionedId versionedId = VersionedId.Deserialize(storeId.ToBase64String(), this.contactId.ChangeKey);
				manualLink.Unlink(this.session, personId, versionedId);
			}
			return Persona.LoadFromPersonIdWithGalAggregation(this.session, personId, Persona.FullPersonaShape, null);
		}

		private readonly MailboxSession session;

		private readonly ItemId personaId;

		private readonly ItemId contactId;
	}
}
