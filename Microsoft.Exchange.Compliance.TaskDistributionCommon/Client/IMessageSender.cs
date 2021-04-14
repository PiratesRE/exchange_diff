using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Client
{
	public interface IMessageSender
	{
		Task<bool[]> SendMessageAsync(IEnumerable<ComplianceMessage> messages);
	}
}
