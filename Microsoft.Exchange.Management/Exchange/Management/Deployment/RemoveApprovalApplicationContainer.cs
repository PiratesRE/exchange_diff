using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "ApprovalApplicationContainer")]
	public sealed class RemoveApprovalApplicationContainer : RemoveSystemConfigurationObjectTask<ApprovalApplicationContainerIdParameter, ApprovalApplicationContainer>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override ApprovalApplicationContainerIdParameter Identity
		{
			get
			{
				return (ApprovalApplicationContainerIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.Identity == null)
				{
					this.Identity = ApprovalApplicationContainerIdParameter.Parse(ApprovalApplicationContainer.DefaultName);
				}
				base.InternalValidate();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}
	}
}
