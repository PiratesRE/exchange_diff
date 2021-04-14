using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetModernGroupUnseenItems : ServiceCommand<GetModernGroupUnseenItemsResponse>
	{
		public GetModernGroupUnseenItems(CallContext context) : base(context)
		{
			this.session = context.SessionCache.GetMailboxIdentityMailboxSession();
		}

		public static bool RequestShouldUseSharedContext(string methodName)
		{
			return methodName == "GetModernGroupUnseenItems";
		}

		protected override GetModernGroupUnseenItemsResponse InternalExecute()
		{
			ExTraceGlobals.ModernGroupsTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "GetModernGroupUnseenItems.InternalExecute: Getting unseen items for user {0}.", base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress);
			GetModernGroupUnseenItemsResponse result;
			try
			{
				IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
				UserMailboxLocator userMailboxLocator = UserMailboxLocator.Instantiate(adrecipientSession, base.CallContext.AccessingADUser);
				UnseenDataType unseenData;
				using (IUnseenItemsReader unseenItemsReader = UnseenItemsReader.Create(this.session))
				{
					ModernGroupNotificationLocator modernGroupNotificationLocator = new ModernGroupNotificationLocator(adrecipientSession);
					IMemberSubscriptionItem memberSubscription = modernGroupNotificationLocator.GetMemberSubscription(this.session, userMailboxLocator);
					unseenItemsReader.LoadLastNItemReceiveDates(this.session);
					ExDateTime lastUpdateTimeUTC = memberSubscription.LastUpdateTimeUTC;
					int unseenItemCount = unseenItemsReader.GetUnseenItemCount(lastUpdateTimeUTC);
					unseenData = new UnseenDataType(unseenItemCount, ExDateTimeConverter.ToUtcXsdDateTime(lastUpdateTimeUTC));
				}
				result = new GetModernGroupUnseenItemsResponse
				{
					UnseenData = unseenData
				};
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
