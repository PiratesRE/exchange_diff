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
	[Cmdlet("Set", "DeliveryAgentConnector", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetDeliveryAgentConnector : SetTopologySystemConfigurationObjectTask<DeliveryAgentConnectorIdParameter, DeliveryAgentConnector>
	{
		[Parameter]
		public MultiValuedProperty<ServerIdParameter> SourceTransportServers
		{
			get
			{
				return (MultiValuedProperty<ServerIdParameter>)base.Fields["SourceTransportServers"];
			}
			set
			{
				base.Fields["SourceTransportServers"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetDeliveryAgentConnector(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			internal get
			{
				return this.force;
			}
			set
			{
				this.force = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			DeliveryAgentConnector deliveryAgentConnector = (DeliveryAgentConnector)base.PrepareDataObject();
			if (base.Fields.IsModified("SourceTransportServers"))
			{
				if (this.SourceTransportServers != null)
				{
					deliveryAgentConnector.SourceTransportServers = base.ResolveIdParameterCollection<ServerIdParameter, Server, ADObjectId>(this.SourceTransportServers, base.DataSession, this.RootId, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotFound), new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotUnique), null, null);
					if (deliveryAgentConnector.SourceTransportServers.Count > 0)
					{
						ManageSendConnectors.SetConnectorHomeMta(deliveryAgentConnector, (IConfigurationSession)base.DataSession);
					}
				}
				else
				{
					deliveryAgentConnector.SourceTransportServers = null;
				}
			}
			return deliveryAgentConnector;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			ADObjectId sourceRoutingGroup = this.DataObject.SourceRoutingGroup;
			bool flag;
			bool flag2;
			LocalizedException ex = ManageSendConnectors.ValidateTransportServers((IConfigurationSession)base.DataSession, this.DataObject, ref sourceRoutingGroup, true, true, this, out flag, out flag2);
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidOperation, this.DataObject);
				return;
			}
			if (flag2)
			{
				this.WriteWarning(Strings.WarningMultiSiteSourceServers);
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			DeliveryAgentConnector deliveryAgentConnector = (DeliveryAgentConnector)dataObject;
			this.currentlyEnabled = deliveryAgentConnector.Enabled;
			deliveryAgentConnector.IsScopedConnector = deliveryAgentConnector.GetScopedConnector();
			deliveryAgentConnector.ResetChangeTracking();
			base.StampChangesOn(dataObject);
		}

		protected override void InternalProcessRecord()
		{
			DeliveryAgentConnector dataObject = this.DataObject;
			if (this.currentlyEnabled && !dataObject.Enabled && !this.force && !base.ShouldContinue(Strings.ConfirmationMessageDisableSendConnector))
			{
				return;
			}
			ManageSendConnectors.AdjustAddressSpaces(dataObject);
			base.InternalProcessRecord();
			ManageSendConnectors.UpdateGwartLastModified((ITopologyConfigurationSession)base.DataSession, this.DataObject.SourceRoutingGroup, new ManageSendConnectors.ThrowTerminatingErrorDelegate(base.WriteError));
		}

		private SwitchParameter force;

		private bool currentlyEnabled;
	}
}
