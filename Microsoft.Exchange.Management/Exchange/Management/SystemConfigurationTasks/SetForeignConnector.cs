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
	[Cmdlet("Set", "ForeignConnector", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetForeignConnector : SetSystemConfigurationObjectTask<ForeignConnectorIdParameter, ForeignConnector>
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
				return Strings.ConfirmationMessageSetForeignConnector(this.Identity.ToString());
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
			ForeignConnector foreignConnector = (ForeignConnector)base.PrepareDataObject();
			if (!MultiValuedPropertyBase.IsNullOrEmpty(this.SourceTransportServers))
			{
				foreignConnector.SourceTransportServers = base.ResolveIdParameterCollection<ServerIdParameter, Server, ADObjectId>(this.SourceTransportServers, base.DataSession, this.RootId, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotFound), new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotUnique), null, null);
				if (this.SourceTransportServers.Count > 0)
				{
					ManageSendConnectors.SetConnectorHomeMta(foreignConnector, (IConfigurationSession)base.DataSession);
				}
			}
			return foreignConnector;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			try
			{
				ForeignConnectorTaskUtil.CheckTopology();
				ForeignConnectorTaskUtil.ValidateObject(this.DataObject, (IConfigurationSession)base.DataSession, this);
			}
			catch (MultiSiteSourceServersException ex)
			{
				base.WriteWarning(ex.Message);
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, this.DataObject);
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			ForeignConnector foreignConnector = dataObject as ForeignConnector;
			this.currentlyEnabled = foreignConnector.Enabled;
			foreignConnector.IsScopedConnector = foreignConnector.GetScopedConnector();
			foreignConnector.ResetChangeTracking();
			base.StampChangesOn(dataObject);
		}

		protected override void InternalProcessRecord()
		{
			ForeignConnector dataObject = this.DataObject;
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
