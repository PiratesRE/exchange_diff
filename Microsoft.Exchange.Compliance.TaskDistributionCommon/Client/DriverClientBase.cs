using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Client
{
	public abstract class DriverClientBase : IMessageSender
	{
		public virtual async Task<bool[]> SendMessageAsync(IEnumerable<ComplianceMessage> messages)
		{
			List<Task<ComplianceMessage>> tasks = new List<Task<ComplianceMessage>>();
			List<bool> sentMessages = new List<bool>();
			foreach (ComplianceMessage message in messages)
			{
				tasks.Add(this.GetResponseAsync(message));
			}
			foreach (Task<ComplianceMessage> t in tasks)
			{
				try
				{
					await t;
					sentMessages.Add(true);
				}
				catch (Exception)
				{
					sentMessages.Add(false);
				}
			}
			return sentMessages.ToArray();
		}

		public abstract Task<ComplianceMessage> GetResponseAsync(ComplianceMessage message);
	}
}
