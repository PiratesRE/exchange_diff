using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Client;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Resolver;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	[OutputType(new Type[]
	{
		typeof(bool)
	})]
	[Cmdlet("Send", "ComplianceMessage")]
	public sealed class SendComplianceMessage : Microsoft.Exchange.Configuration.Tasks.Task
	{
		[Parameter(Mandatory = true)]
		public byte[] SerializedComplianceMessage
		{
			get
			{
				return ComplianceSerializer.Serialize<ComplianceMessage>(ComplianceMessage.Description, this.message);
			}
			set
			{
				this.message = ComplianceSerializer.DeSerialize<ComplianceMessage>(ComplianceMessage.Description, value);
			}
		}

		public SendComplianceMessage()
		{
			this.message = new ComplianceMessage();
		}

		protected override void InternalProcessRecord()
		{
			if (this.message != null && this.message.WorkDefinitionType == WorkDefinitionType.Test && this.message.MessageId.Equals("RpsProxyClientTestMessage", StringComparison.InvariantCultureIgnoreCase))
			{
				base.WriteObject(true);
				return;
			}
			this.message.TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8);
			WorkloadClientBase workloadClientBase = new IntraExchangeWorkloadClient();
			ActiveDirectoryTargetResolver activeDirectoryTargetResolver = new ActiveDirectoryTargetResolver();
			IEnumerable<ComplianceMessage> sources = new List<ComplianceMessage>
			{
				this.message
			};
			IEnumerable<ComplianceMessage> messages = activeDirectoryTargetResolver.Resolve(sources);
			Task<bool[]> task = workloadClientBase.SendMessageAsync(messages);
			task.Wait();
			bool flag = task.Result[0];
			base.WriteObject(flag);
		}

		private const string RpsProxyClientTestMessage = "RpsProxyClientTestMessage";

		private ComplianceMessage message;
	}
}
