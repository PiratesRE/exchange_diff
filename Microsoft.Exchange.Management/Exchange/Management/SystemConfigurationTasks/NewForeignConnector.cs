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
	[Cmdlet("New", "ForeignConnector", SupportsShouldProcess = true, DefaultParameterSetName = "AddressSpaces")]
	public sealed class NewForeignConnector : NewSystemConfigurationObjectTask<ForeignConnector>
	{
		[Parameter(ParameterSetName = "AddressSpaces", Mandatory = true)]
		public MultiValuedProperty<AddressSpace> AddressSpaces
		{
			get
			{
				return this.DataObject.AddressSpaces;
			}
			set
			{
				this.DataObject.AddressSpaces = value;
			}
		}

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

		[Parameter]
		public bool IsScopedConnector
		{
			get
			{
				return this.DataObject.IsScopedConnector;
			}
			set
			{
				this.DataObject.IsScopedConnector = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewForeignConnectorAddressSpaces(base.Name, base.FormatMultiValuedProperty(this.AddressSpaces));
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ForeignConnector foreignConnector = (ForeignConnector)base.PrepareDataObject();
			if (!MultiValuedPropertyBase.IsNullOrEmpty(this.SourceTransportServers))
			{
				foreignConnector.SourceTransportServers = base.ResolveIdParameterCollection<ServerIdParameter, Server, ADObjectId>(this.SourceTransportServers, base.DataSession, this.RootId, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotFound), new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotUnique), null, null);
			}
			else
			{
				Server server = null;
				try
				{
					server = ((ITopologyConfigurationSession)base.DataSession).ReadLocalServer();
				}
				catch (TransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ResourceUnavailable, this.DataObject);
				}
				if (ForeignConnectorTaskUtil.IsHubServer(server))
				{
					foreignConnector.SourceTransportServers = new MultiValuedProperty<ADObjectId>(false, SendConnectorSchema.SourceTransportServers, new ADObjectId[]
					{
						server.Id
					});
				}
			}
			if (!MultiValuedPropertyBase.IsNullOrEmpty(foreignConnector.SourceTransportServers))
			{
				ManageSendConnectors.SetConnectorHomeMta(foreignConnector, (IConfigurationSession)base.DataSession);
			}
			if (string.IsNullOrEmpty(foreignConnector.DropDirectory))
			{
				foreignConnector.DropDirectory = foreignConnector.Name;
			}
			ManageSendConnectors.SetConnectorId(foreignConnector, ((ITopologyConfigurationSession)base.DataSession).GetRoutingGroupId());
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

		protected override void InternalProcessRecord()
		{
			ForeignConnector dataObject = this.DataObject;
			if (dataObject.IsScopedConnector)
			{
				ManageSendConnectors.AdjustAddressSpaces(dataObject);
			}
			base.InternalProcessRecord();
			ManageSendConnectors.UpdateGwartLastModified((ITopologyConfigurationSession)base.DataSession, this.DataObject.SourceRoutingGroup, new ManageSendConnectors.ThrowTerminatingErrorDelegate(base.WriteError));
		}
	}
}
