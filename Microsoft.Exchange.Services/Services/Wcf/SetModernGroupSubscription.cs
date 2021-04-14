using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SetModernGroupSubscription : ServiceCommand<bool>
	{
		public SetModernGroupSubscription(CallContext context) : base(context)
		{
			this.session = context.SessionCache.GetMailboxIdentityMailboxSession();
		}

		protected override bool InternalExecute()
		{
			ExTraceGlobals.ModernGroupsTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "SetModernGroupSubscription.InternalExecute: Setting subscription for user {0}.", base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress);
			bool result;
			try
			{
				IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
				UserMailboxLocator userMailboxLocator = UserMailboxLocator.Instantiate(adrecipientSession, base.CallContext.AccessingADUser);
				ModernGroupNotificationLocator modernGroupNotificationLocator = new ModernGroupNotificationLocator(adrecipientSession);
				modernGroupNotificationLocator.UpdateMemberSubscription(this.session, userMailboxLocator);
				result = true;
			}
			catch (TransientException arg)
			{
				ExTraceGlobals.ModernGroupsTracer.TraceError<TransientException>((long)this.GetHashCode(), "GetModernGroupUnseenItems.InternalExecute: TransientException while querying for unseen conversation items. {0}.", arg);
				throw;
			}
			catch (DataSourceOperationException arg2)
			{
				ExTraceGlobals.ModernGroupsTracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "GetModernGroupUnseenItems.InternalExecute: DataSourceOperationException while querying for unseen conversation items. {0}.", arg2);
				throw;
			}
			return result;
		}

		private readonly MailboxSession session;
	}
}
