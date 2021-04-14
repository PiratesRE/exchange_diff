using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class DeletePersonaCommand : ServiceCommand<ServiceResultNone>
	{
		public DeletePersonaCommand(CallContext callContext, ItemId personaId, BaseFolderId deleteInFolder) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(personaId, "personaId", "DeletePersonaCommand::ctor");
			this.personaId = personaId;
			this.deleteInFolder = deleteInFolder;
		}

		protected override ServiceResultNone InternalExecute()
		{
			StoreSession storeSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			StoreId storeId = null;
			if (IdConverter.EwsIdIsActiveDirectoryObject(this.personaId.GetId()))
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)3142918589U);
			}
			PersonId personId = IdConverter.EwsIdToPersonId(this.personaId.Id);
			if (this.deleteInFolder != null)
			{
				IdAndSession idAndSession = base.GetIdAndSession(this.deleteInFolder);
				if (idAndSession.Session.IsPublicFolderSession)
				{
					storeSession = idAndSession.Session;
					storeId = idAndSession.Id;
				}
			}
			Persona.DeletePersona(storeSession, personId, this.personaId, storeId);
			return new ServiceResultNone();
		}

		private ItemId personaId;

		private BaseFolderId deleteInFolder;
	}
}
