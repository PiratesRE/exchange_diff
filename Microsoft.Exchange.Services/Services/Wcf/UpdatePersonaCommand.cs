using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class UpdatePersonaCommand : UpdateCreatePersonaCommandBase
	{
		public UpdatePersonaCommand(CallContext callContext, UpdatePersonaRequest request) : base(callContext, request)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "UpdatePersonaRequest", "UpdatePersonaCommand::UpdatePersonaCommand");
			WcfServiceCommandBase.ThrowIfNull(request.PersonaId, "UpdatePersonaRequest.PersonaId", "UpdatePersonaCommand::UpdatePersonaCommand");
			this.personaId = request.PersonaId;
			if (request.ParentFolderId != null)
			{
				this.idAndSession = base.GetIdAndSession(request.ParentFolderId.BaseFolderId);
			}
		}

		protected override Persona InternalExecute()
		{
			StoreSession storeSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			StoreId folderId = null;
			if (this.idAndSession != null && this.idAndSession.Session.IsPublicFolderSession)
			{
				storeSession = this.idAndSession.Session;
				folderId = this.idAndSession.Id;
			}
			PersonId personId = IdConverter.EwsIdToPersonId(this.personaId.GetId());
			return Persona.UpdatePersona(storeSession, personId, this.personaId, this.propertyChanges, this.personType, folderId);
		}

		protected ItemId personaId;

		private IdAndSession idAndSession;
	}
}
