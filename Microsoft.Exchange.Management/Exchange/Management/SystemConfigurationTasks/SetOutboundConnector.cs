using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "OutboundConnector", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class SetOutboundConnector : SetSystemConfigurationObjectTask<OutboundConnectorIdParameter, TenantOutboundConnector>
	{
		private string CurrentName { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetOutboundConnector(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public bool BypassValidation
		{
			get
			{
				return base.Fields.Contains("BypassValidation") && (bool)base.Fields["BypassValidation"];
			}
			set
			{
				base.Fields["BypassValidation"] = value;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADObject adobject = (ADObject)base.ResolveDataObject();
			this.CurrentName = adobject.Name;
			return adobject;
		}

		protected override void InternalProcessRecord()
		{
			ManageTenantOutboundConnectors.ClearSmartHostsListIfNecessary(this.DataObject);
			base.InternalProcessRecord();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			LocalizedException ex = ManageTenantOutboundConnectors.ValidateConnectorNameReferences(this.DataObject, this.CurrentName, base.DataSession);
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidArgument, null);
			}
			ManageTenantOutboundConnectors.ValidateOutboundConnectorDataObject(this.DataObject, this, base.DataSession, this.BypassValidation);
			ManageTenantOutboundConnectors.ValidateIfAcceptedDomainsCanBeRoutedWithConnectors(this.DataObject, base.DataSession, this, false);
			TaskLogger.LogExit();
		}
	}
}
