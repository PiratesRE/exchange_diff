using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class AcceptPersonaLinkSuggestionCommand : ServiceCommand<Persona>
	{
		public AcceptPersonaLinkSuggestionCommand(CallContext callContext, ItemId personaId, ItemId suggestedPersonaId) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(personaId, "personaId", "AcceptPersonaLinkSuggestionCommand::AcceptPersonaLinkSuggestionCommand");
			WcfServiceCommandBase.ThrowIfNull(suggestedPersonaId, "suggestedPersonaId", "AcceptPersonaLinkSuggestionCommand::AcceptPersonaLinkSuggestionCommand");
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.personaId = personaId;
			this.suggestedPersonaId = suggestedPersonaId;
		}

		protected override Persona InternalExecute()
		{
			if (IdConverter.EwsIdIsActiveDirectoryObject(this.personaId.GetId()))
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)3142918589U);
			}
			PersonId personId = IdConverter.EwsIdToPersonId(this.personaId.GetId());
			PersonId linkingPersonId = IdConverter.EwsIdToPersonId(this.suggestedPersonaId.GetId());
			MailboxInfoForLinking mailboxInfo = MailboxInfoForLinking.CreateFromMailboxSession(this.session);
			ContactLinkingPerformanceTracker performanceTracker = new ContactLinkingPerformanceTracker(this.session);
			ContactLinkingLogger logger = new ContactLinkingLogger("AcceptPersonaLinkSuggestionCommand", mailboxInfo);
			ManualLink manualLink = new ManualLink(mailboxInfo, logger, performanceTracker);
			manualLink.Link(this.session, linkingPersonId, personId);
			return Persona.LoadFromPersonIdWithGalAggregation(this.session, personId, Persona.FullPersonaShape, null);
		}

		private readonly MailboxSession session;

		private readonly ItemId personaId;

		private readonly ItemId suggestedPersonaId;
	}
}
