using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission.Agents
{
	internal class ParkedItemSubmitterAgentFactory : StoreDriverAgentFactory
	{
		public override StoreDriverAgent CreateAgent(SmtpServer server)
		{
			return new ParkedItemSubmitterAgent();
		}
	}
}
