using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SetModernGroupPinState : ServiceCommand<SetModernGroupPinStateJsonResponse>
	{
		public SetModernGroupPinState(CallContext context, string groupSmtpAddress, bool isPinned) : base(context)
		{
			if (!SmtpAddress.IsValidSmtpAddress(groupSmtpAddress))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(CoreResources.IDs.ErrorInvalidSmtpAddress), FaultParty.Sender);
			}
			this.groupSmtpAddress = groupSmtpAddress;
			this.isPinned = isPinned;
		}

		protected override SetModernGroupPinStateJsonResponse InternalExecute()
		{
			ExTraceGlobals.ModernGroupsTracer.TraceDebug<bool, SmtpAddress>((long)this.GetHashCode(), "SetModernGroupPinState.InternalExecute: Set pin to {0} for user {1}.", this.isPinned, base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress);
			IRecipientSession adSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			try
			{
				GroupMailboxAccessLayer.Execute("SetModernGroupPinState", adSession, base.MailboxIdentityMailboxSession, delegate(GroupMailboxAccessLayer accessLayer)
				{
					UserMailboxLocator user = UserMailboxLocator.Instantiate(adSession, this.CallContext.AccessingADUser);
					ProxyAddress proxyAddress = new SmtpProxyAddress(this.groupSmtpAddress, true);
					GroupMailboxLocator group = GroupMailboxLocator.Instantiate(adSession, proxyAddress);
					bool isModernGroupsNewArchitecture = this.CallContext.FeaturesManager != null && this.CallContext.FeaturesManager.IsFeatureSupported("ModernGroupsNewArchitecture");
					accessLayer.SetGroupPinState(user, group, this.isPinned, isModernGroupsNewArchitecture);
				});
			}
			catch (NotAMemberException innerException)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)2923349632U, innerException);
			}
			catch (LocalizedException innerException2)
			{
				throw new InternalServerErrorException(innerException2);
			}
			return new SetModernGroupPinStateJsonResponse();
		}

		private readonly string groupSmtpAddress;

		private readonly bool isPinned;
	}
}
