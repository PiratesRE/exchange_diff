using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class RejectPersonaLinkSuggestionCommand : ServiceCommand<Persona>
	{
		public RejectPersonaLinkSuggestionCommand(CallContext callContext, ItemId personaId, ItemId suggestedPersonaId) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(personaId, "personaId", "RejectPersonaLinkSuggestionCommand::RejectPersonaLinkSuggestionCommand");
			WcfServiceCommandBase.ThrowIfNull(suggestedPersonaId, "suggestedPersonaId", "RejectPersonaLinkSuggestionCommand::RejectPersonaLinkSuggestionCommand");
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.personaId = personaId;
			this.suggestedPersonaId = suggestedPersonaId;
		}

		protected override Persona InternalExecute()
		{
			PersonId personId = IdConverter.EwsIdToPersonId(this.personaId.GetId());
			PersonId suggestionPersonId = IdConverter.EwsIdToPersonId(this.suggestedPersonaId.GetId());
			MailboxInfoForLinking mailboxInfo = MailboxInfoForLinking.CreateFromMailboxSession(this.session);
			ContactLinkingPerformanceTracker performanceTracker = new ContactLinkingPerformanceTracker(this.session);
			ContactLinkingLogger logger = new ContactLinkingLogger("RejectPersonaLinkSuggestionCommand", mailboxInfo);
			ManualLink manualLink = new ManualLink(mailboxInfo, logger, performanceTracker);
			manualLink.RejectSuggestion(this.session, personId, suggestionPersonId);
			return Persona.LoadFromPersonIdWithGalAggregation(this.session, personId, Persona.FullPersonaShape, null);
		}

		private readonly MailboxSession session;

		private readonly ItemId personaId;

		private readonly ItemId suggestedPersonaId;
	}
}
