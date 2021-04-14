using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage.MailboxRules;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.MailboxRules
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class StoreDriverLimitChecker : LimitChecker
	{
		public StoreDriverLimitChecker(IRuleEvaluationContext context) : base(context)
		{
		}

		protected override void MessageTrackThrottle<C, L>(string limitType, C count, L limit)
		{
			if (base.ServerIPAddress == null)
			{
				base.ServerIPAddress = this.context.LocalServerNetworkAddress;
			}
			RuleEvaluationContext ruleEvaluationContext = this.context as RuleEvaluationContext;
			MessageTrackingLog.TrackThrottle(MessageTrackingSource.MAILBOXRULE, ruleEvaluationContext.MbxTransportMailItem, base.ServerIPAddress, this.context.CurrentFolderDisplayName, string.Format(CultureInfo.InvariantCulture, "{0}:{1}/{2}", new object[]
			{
				limitType,
				count,
				limit
			}), this.context.Recipient, this.context.CurrentRule.Name);
		}
	}
}
