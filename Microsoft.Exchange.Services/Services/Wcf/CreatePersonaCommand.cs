using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class CreatePersonaCommand : UpdateCreatePersonaCommandBase
	{
		public CreatePersonaCommand(CallContext callContext, CreatePersonaRequest request) : base(callContext, request)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "CreatePersonaRequest", "CreatePersonaCommand::CreatePersonaCommand");
			WcfServiceCommandBase.ThrowIfNull(request.ParentFolderId, "CreatePersonaRequest.parentFolderId", "CreatePersonaCommand::CreatePersonaCommand");
			this.personaId = request.PersonaId;
			this.parentFolderIdAndSession = base.GetIdAndSession(request.ParentFolderId.BaseFolderId);
		}

		protected override Persona InternalExecute()
		{
			MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			Persona result = Persona.CreatePersona(this.parentFolderIdAndSession.Session, this.propertyChanges, this.parentFolderIdAndSession.Id, this.personaId, this.personType);
			if (base.CallContext.AccessingPrincipal != null && base.CallContext.AccessingPrincipal.GetConfiguration().OwaClientServer.PeopleCentricTriage.Enabled)
			{
				StoreId defaultFolderId = mailboxIdentityMailboxSession.GetDefaultFolderId(DefaultFolderType.RecipientCache);
				if (defaultFolderId != null && defaultFolderId.Equals(this.parentFolderIdAndSession.Id))
				{
					new PeopleIKnowEmailAddressCollectionFolderProperty(XSOFactory.Default, NullTracer.Instance, 0).Publish(mailboxIdentityMailboxSession);
				}
			}
			return result;
		}

		private IdAndSession parentFolderIdAndSession;

		protected ItemId personaId;
	}
}
