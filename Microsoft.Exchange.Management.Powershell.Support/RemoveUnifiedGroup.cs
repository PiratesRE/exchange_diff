using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Remove", "UnifiedGroup")]
	public sealed class RemoveUnifiedGroup : UnifiedGroupTask
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public Guid Identity { get; set; }

		protected override void InternalProcessRecord()
		{
			AADClient aadclient = AADClientFactory.Create(base.OrganizationId, GraphProxyVersions.Version14);
			if (aadclient == null)
			{
				base.WriteError(new TaskException(Strings.ErrorUnableToSessionWithAAD), ExchangeErrorCategory.Client, null);
			}
			try
			{
				base.WriteVerbose("Calling DeleteGroup", new object[0]);
				aadclient.DeleteGroup(this.Identity.ToString());
				base.WriteVerbose("DeleteGroup succeeded", new object[0]);
			}
			catch (AADException ex)
			{
				base.WriteVerbose("DeleteGroup failed with exception: {0}", new object[]
				{
					ex
				});
				base.WriteError(new TaskException(Strings.ErrorUnableToRemove(this.Identity.ToString())), base.GetErrorCategory(ex), null);
			}
		}
	}
}
