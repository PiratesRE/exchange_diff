using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class LinkPersonaCommand : ServiceCommand<Persona>
	{
		public LinkPersonaCommand(CallContext callContext, ItemId linkToPersonaId, ItemId personaIdToBeLinked) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(linkToPersonaId, "linkToPersonaId", "LinkPersonaCommand::LinkPersonaCommand");
			WcfServiceCommandBase.ThrowIfNull(personaIdToBeLinked, "personaIdToBeLinked", "LinkPersonaCommand::LinkPersonaCommand");
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.linkToPersonaId = linkToPersonaId;
			this.personaIdToBeLinked = personaIdToBeLinked;
		}

		protected override Persona InternalExecute()
		{
			if (IdConverter.EwsIdIsActiveDirectoryObject(this.linkToPersonaId.GetId()) && IdConverter.EwsIdIsActiveDirectoryObject(this.personaIdToBeLinked.GetId()))
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)3142918589U);
			}
			ItemId itemId = this.linkToPersonaId;
			MailboxInfoForLinking mailboxInfo = MailboxInfoForLinking.CreateFromMailboxSession(this.session);
			ContactLinkingPerformanceTracker performanceTracker = new ContactLinkingPerformanceTracker(this.session);
			ContactLinkingLogger logger = new ContactLinkingLogger("LinkPersonaCommand", mailboxInfo);
			ManualLink manualLink = new ManualLink(mailboxInfo, logger, performanceTracker);
			if (IdConverter.EwsIdIsActiveDirectoryObject(this.personaIdToBeLinked.GetId()))
			{
				IRecipientSession galscopedADRecipientSession = base.CallContext.ADRecipientSessionContext.GetGALScopedADRecipientSession(base.CallContext.EffectiveCaller.ClientSecurityContext);
				manualLink.Link(this.session, galscopedADRecipientSession, IdConverter.EwsIdToPersonId(this.linkToPersonaId.GetId()), IdConverter.EwsIdToADObjectId(this.personaIdToBeLinked.GetId()));
			}
			else if (IdConverter.EwsIdIsActiveDirectoryObject(this.linkToPersonaId.GetId()))
			{
				IRecipientSession galscopedADRecipientSession2 = base.CallContext.ADRecipientSessionContext.GetGALScopedADRecipientSession(base.CallContext.EffectiveCaller.ClientSecurityContext);
				manualLink.Link(this.session, galscopedADRecipientSession2, IdConverter.EwsIdToPersonId(this.personaIdToBeLinked.GetId()), IdConverter.EwsIdToADObjectId(this.linkToPersonaId.GetId()));
				itemId = this.personaIdToBeLinked;
			}
			else
			{
				manualLink.Link(this.session, IdConverter.EwsIdToPersonId(this.personaIdToBeLinked.GetId()), IdConverter.EwsIdToPersonId(this.linkToPersonaId.GetId()));
			}
			return Persona.LoadFromPersonIdWithGalAggregation(this.session, IdConverter.EwsIdToPersonId(itemId.GetId()), Persona.FullPersonaShape, null);
		}

		private readonly MailboxSession session;

		private readonly ItemId linkToPersonaId;

		private readonly ItemId personaIdToBeLinked;
	}
}
