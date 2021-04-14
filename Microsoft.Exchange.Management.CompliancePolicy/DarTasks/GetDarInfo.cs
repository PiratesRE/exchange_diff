using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.CompliancePolicy.LocStrings;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	[Cmdlet("Get", "DarInfo")]
	public sealed class GetDarInfo : Task
	{
		public GetDarInfo()
		{
			this.ExecutionUnit = new ServerIdParameter();
		}

		[Parameter(Mandatory = false)]
		public OrganizationIdParameter TenantId { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter ExecutionUnit { get; set; }

		protected override void InternalProcessRecord()
		{
			if (this.TenantId != null)
			{
				try
				{
					base.CurrentOrganizationId = GetDarTask.ResolveOrganizationId(this.TenantId);
				}
				catch (ArgumentException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, this.TenantId);
					return;
				}
				base.WriteObject(string.Format(Strings.ResolvedOrg, base.CurrentOrganizationId));
				this.ExecutionUnit = GetDarTask.ResolveServerId(base.CurrentOrganizationId);
				base.WriteObject(string.Format(Strings.ResolvedServer, this.ExecutionUnit));
			}
			try
			{
				using (HostRpcClient hostRpcClient = new HostRpcClient(this.ExecutionUnit.Fqdn))
				{
					string darInfo = hostRpcClient.GetDarInfo();
					if (!string.IsNullOrEmpty(darInfo))
					{
						foreach (string sendToPipeline in darInfo.Split(new char[]
						{
							'\n'
						}))
						{
							base.WriteObject(sendToPipeline);
						}
					}
				}
			}
			catch (ServerUnavailableException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ConnectionError, this.ExecutionUnit.Fqdn);
			}
		}
	}
}
