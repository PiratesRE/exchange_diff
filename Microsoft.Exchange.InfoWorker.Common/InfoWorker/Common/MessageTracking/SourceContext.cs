using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class SourceContext
	{
		public const string TransportRules = "Transport Rule Agent";

		public const string EncryptionAgent = "Federated Delivery Encryption Agent";

		public const string ApprovalAgent = "Approval Processing Agent";

		public const string ContentFilterAgent = "Content Filter Agent";

		public const string MailboxRulesAgent = "Mailbox Rules Agent";

		public const string MailboxRulesAgentDelegateAccessRule = "Mailbox Rules Agent.DelegateAccess";
	}
}
