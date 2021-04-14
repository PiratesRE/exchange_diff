using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetPersona : SingleStepServiceCommand<GetPersonaRequest, GetPersonaResponseMessage>
	{
		public GetPersona(CallContext callContext, GetPersonaRequest request) : base(callContext, request)
		{
			this.personaId = request.PersonaId;
			this.emailAddress = request.EmailAddress;
			this.parentFolderId = request.ParentFolderId;
			if (this.personaId != null && this.personaId.Id != null)
			{
				this.hashCode = this.personaId.Id.GetHashCode();
				return;
			}
			if (this.emailAddress != null && !string.IsNullOrEmpty(this.emailAddress.EmailAddress))
			{
				this.hashCode = this.emailAddress.EmailAddress.GetHashCode();
				return;
			}
			this.hashCode = 0;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetPersonaResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetPersonaResponseMessage> Execute()
		{
			GetPersona.Tracer.TraceDebug<string, string>((long)this.hashCode, "GetPersona.Execute: Entering, with PersonaId {0}, EmailAddress: {1}.", (this.personaId == null) ? "(null)" : this.personaId.Id, (this.emailAddress == null) ? "(null)" : this.emailAddress.EmailAddress);
			GetPersonaResponseMessage getPersonaResponseMessage = new GetPersonaResponseMessage();
			MailboxSession mailboxIdentityMailboxSession = base.GetMailboxIdentityMailboxSession();
			if ((this.personaId == null || string.IsNullOrEmpty(this.personaId.Id)) && (this.emailAddress == null || string.IsNullOrEmpty(this.emailAddress.EmailAddress)))
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)3784063568U);
			}
			Persona persona;
			if (this.parentFolderId != null && this.parentFolderId.BaseFolderId != null)
			{
				IdAndSession idAndSession = new IdConverter(base.CallContext).ConvertFolderIdToIdAndSession(this.parentFolderId.BaseFolderId, IdConverter.ConvertOption.IgnoreChangeKey);
				persona = Persona.LoadFromPersonaId((PublicFolderSession)idAndSession.Session, null, this.personaId, Persona.FullPersonaShape, null, idAndSession.Id);
			}
			else if (this.emailAddress != null)
			{
				persona = Persona.LoadFromEmailAddressWithGalAggregation(mailboxIdentityMailboxSession, base.CallContext.ADRecipientSessionContext.GetGALScopedADRecipientSession(base.CallContext.EffectiveCaller.ClientSecurityContext), this.emailAddress, Persona.FullPersonaShape);
			}
			else
			{
				persona = Persona.LoadFromPersonaIdWithGalAggregation(mailboxIdentityMailboxSession, base.CallContext.ADRecipientSessionContext.GetGALScopedADRecipientSession(base.CallContext.EffectiveCaller.ClientSecurityContext), this.personaId, Persona.FullPersonaShape, null);
			}
			if (persona == null)
			{
				GetPersona.Tracer.TraceDebug((long)this.hashCode, "GetPersona.Execute: No Persona found for the given identity, throwing object not found error.");
				throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
			}
			getPersonaResponseMessage.Persona = persona;
			GetPersona.Tracer.TraceDebug((long)this.hashCode, "GetPersona.Execute: Exiting, Persona found for the given identity.");
			return new ServiceResult<GetPersonaResponseMessage>(getPersonaResponseMessage);
		}

		private static readonly Trace Tracer = ExTraceGlobals.GetPersonaCallTracer;

		private readonly ItemId personaId;

		private readonly EmailAddressWrapper emailAddress;

		private TargetFolderId parentFolderId;

		private readonly int hashCode;
	}
}
