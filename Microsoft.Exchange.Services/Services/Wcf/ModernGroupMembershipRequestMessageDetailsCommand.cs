using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class ModernGroupMembershipRequestMessageDetailsCommand : ServiceCommand<ModernGroupMembershipRequestMessageDetailsResponse>
	{
		public ModernGroupMembershipRequestMessageDetailsCommand(CallContext callContext, ModernGroupMembershipRequestMessageDetailsRequest request) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "request", "ModernGroupMembershipRequestMessageDetailsCommand::ModernGroupMembershipRequestMessageDetailsCommand");
			this.request = request;
			this.request.Validate();
		}

		protected override ModernGroupMembershipRequestMessageDetailsResponse InternalExecute()
		{
			ModernGroupMembershipRequestMessageDetailsResponse result = null;
			MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			using (GroupMailboxJoinRequestMessageItem groupMailboxJoinRequestMessageItem = GroupMailboxJoinRequestMessageItem.Bind(mailboxIdentityMailboxSession, this.request.MessageStoreId))
			{
				ADUser aduser = adrecipientSession.FindByProxyAddress(new SmtpProxyAddress(groupMailboxJoinRequestMessageItem.GroupSmtpAddress, true)) as ADUser;
				ModernGroupMembershipRequestMessageDetailsResponse modernGroupMembershipRequestMessageDetailsResponse = new ModernGroupMembershipRequestMessageDetailsResponse();
				modernGroupMembershipRequestMessageDetailsResponse.GroupPersona = this.GetGroupPersona(aduser);
				modernGroupMembershipRequestMessageDetailsResponse.IsOwner = aduser.Owners.Any((ADObjectId owner) => owner.Equals(base.CallContext.AccessingADUser.ObjectId));
				result = modernGroupMembershipRequestMessageDetailsResponse;
			}
			return result;
		}

		private Persona GetGroupPersona(ADUser groupADUser)
		{
			return new Persona
			{
				PersonaId = IdConverter.PersonaIdFromADObjectId(groupADUser.ObjectId.ObjectGuid),
				ADObjectId = groupADUser.ObjectId.ObjectGuid,
				DisplayName = groupADUser.DisplayName,
				Alias = groupADUser.Alias,
				PersonaType = PersonaTypeConverter.ToString(PersonType.ModernGroup),
				EmailAddress = new EmailAddressWrapper
				{
					Name = (groupADUser.DisplayName ?? string.Empty),
					EmailAddress = groupADUser.PrimarySmtpAddress.ToString(),
					RoutingType = "SMTP",
					MailboxType = MailboxHelper.MailboxTypeType.GroupMailbox.ToString()
				}
			};
		}

		private ModernGroupMembershipRequestMessageDetailsRequest request;
	}
}
