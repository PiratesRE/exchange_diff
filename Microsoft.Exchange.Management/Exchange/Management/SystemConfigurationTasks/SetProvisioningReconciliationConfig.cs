using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ProvisioningReconciliationConfig", SupportsShouldProcess = true)]
	public sealed class SetProvisioningReconciliationConfig : SetSingletonSystemConfigurationObjectTask<ProvisioningReconciliationConfig>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmSetProvisioningReconciliationConfig;
			}
		}

		internal new Fqdn DomainController { get; set; }

		[Parameter(Mandatory = true)]
		public MultiValuedProperty<ReconciliationCookie> ReconciliationCookies
		{
			get
			{
				return (MultiValuedProperty<ReconciliationCookie>)base.Fields["ReconciliationCookies"];
			}
			set
			{
				base.Fields["ReconciliationCookies"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ProvisioningReconciliationConfig dataObject = this.DataObject;
			if (dataObject != null && this.ReconciliationCookies != null && this.ReconciliationCookies.Count > 0)
			{
				dataObject.ReconciliationCookies = this.ReconciliationCookies;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
