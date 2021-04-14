using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Set", "SharedConfigDC")]
	public sealed class SetSharedConfigDC : Task
	{
		[LocDescription(Strings.IDs.DomainControllerName)]
		[Parameter(Mandatory = true)]
		public string DomainController
		{
			get
			{
				return (string)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			Exception ex = null;
			try
			{
				ADSession.SetSharedConfigDC((base.CurrentOrganizationId.PartitionId != null) ? base.CurrentOrganizationId.PartitionId.ForestFQDN : TopologyProvider.LocalForestFqdn, this.DomainController, 389);
			}
			catch (DataSourceOperationException ex2)
			{
				ex = ex2;
			}
			catch (DataSourceTransientException ex3)
			{
				ex = ex3;
			}
			catch (DataValidationException ex4)
			{
				ex = ex4;
			}
			finally
			{
				if (ex != null)
				{
					base.WriteError(ex, ErrorCategory.InvalidOperation, null);
				}
				TaskLogger.LogExit();
			}
		}
	}
}
