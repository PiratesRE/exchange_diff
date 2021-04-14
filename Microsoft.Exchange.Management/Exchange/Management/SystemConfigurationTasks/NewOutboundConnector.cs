using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "OutboundConnector", SupportsShouldProcess = true)]
	public class NewOutboundConnector : NewMultitenancySystemConfigurationObjectTask<TenantOutboundConnector>
	{
		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return this.DataObject.Enabled;
			}
			set
			{
				this.DataObject.Enabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseMXRecord
		{
			get
			{
				return this.DataObject.UseMXRecord;
			}
			set
			{
				this.DataObject.UseMXRecord = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TenantConnectorType ConnectorType
		{
			get
			{
				return this.DataObject.ConnectorType;
			}
			set
			{
				this.DataObject.ConnectorType = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TenantConnectorSource ConnectorSource
		{
			get
			{
				return this.DataObject.ConnectorSource;
			}
			set
			{
				this.DataObject.ConnectorSource = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return this.DataObject.Comment;
			}
			set
			{
				this.DataObject.Comment = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpDomainWithSubdomains> RecipientDomains
		{
			get
			{
				return this.DataObject.RecipientDomains;
			}
			set
			{
				this.DataObject.RecipientDomains = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmartHost> SmartHosts
		{
			get
			{
				return this.DataObject.SmartHosts;
			}
			set
			{
				this.DataObject.SmartHosts = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomainWithSubdomains TlsDomain
		{
			get
			{
				return this.DataObject.TlsDomain;
			}
			set
			{
				this.DataObject.TlsDomain = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TlsAuthLevel? TlsSettings
		{
			get
			{
				return this.DataObject.TlsSettings;
			}
			set
			{
				this.DataObject.TlsSettings = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsTransportRuleScoped
		{
			get
			{
				return this.DataObject.IsTransportRuleScoped;
			}
			set
			{
				this.DataObject.IsTransportRuleScoped = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RouteAllMessagesViaOnPremises
		{
			get
			{
				return this.DataObject.RouteAllMessagesViaOnPremises;
			}
			set
			{
				this.DataObject.RouteAllMessagesViaOnPremises = value;
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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			ManageTenantOutboundConnectors.ValidateOutboundConnectorDataObject(this.DataObject, this, base.DataSession, this.BypassValidation);
			IEnumerable<TenantOutboundConnector> enumerable = base.DataSession.FindPaged<TenantOutboundConnector>(null, ((IConfigurationSession)base.DataSession).GetOrgContainerId().GetDescendantId(this.DataObject.ParentPath), false, null, ADGenericPagedReader<TenantOutboundConnector>.DefaultPageSize);
			foreach (TenantOutboundConnector tenantOutboundConnector in enumerable)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(this.DataObject.Name, tenantOutboundConnector.Name))
				{
					base.WriteError(new ErrorOutboundConnectorAlreadyExistsException(tenantOutboundConnector.Name), ErrorCategory.InvalidOperation, null);
					break;
				}
			}
			ManageTenantOutboundConnectors.ValidateIfAcceptedDomainsCanBeRoutedWithConnectors(this.DataObject, base.DataSession, this, false);
			TaskLogger.LogExit();
		}

		[Parameter(Mandatory = false)]
		public bool CloudServicesMailEnabled
		{
			get
			{
				return this.DataObject.CloudServicesMailEnabled;
			}
			set
			{
				this.DataObject.CloudServicesMailEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllAcceptedDomains
		{
			get
			{
				return this.DataObject.AllAcceptedDomains;
			}
			set
			{
				this.DataObject.AllAcceptedDomains = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			ManageTenantOutboundConnectors.ClearSmartHostsListIfNecessary(this.DataObject);
			base.InternalProcessRecord();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewOutboundConnector(base.Name);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TenantOutboundConnector tenantOutboundConnector = (TenantOutboundConnector)base.PrepareDataObject();
			tenantOutboundConnector.SetId(base.DataSession as IConfigurationSession, base.Name);
			return tenantOutboundConnector;
		}
	}
}
