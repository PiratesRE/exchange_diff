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
	[Cmdlet("New", "DeliveryAgentConnector", SupportsShouldProcess = true, DefaultParameterSetName = "AddressSpaces")]
	public sealed class NewDeliveryAgentConnector : NewSystemConfigurationObjectTask<DeliveryAgentConnector>
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

		[Parameter(Mandatory = true)]
		public string DeliveryProtocol
		{
			get
			{
				return this.DataObject.DeliveryProtocol;
			}
			set
			{
				this.DataObject.DeliveryProtocol = value;
			}
		}

		[Parameter]
		public int MaxMessagesPerConnection
		{
			get
			{
				return this.DataObject.MaxMessagesPerConnection;
			}
			set
			{
				this.DataObject.MaxMessagesPerConnection = value;
			}
		}

		[Parameter]
		public int MaxConcurrentConnections
		{
			get
			{
				return this.DataObject.MaxConcurrentConnections;
			}
			set
			{
				this.DataObject.MaxConcurrentConnections = value;
			}
		}

		[Parameter]
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

		[Parameter]
		public Unlimited<ByteQuantifiedSize> MaxMessageSize
		{
			get
			{
				return this.DataObject.MaxMessageSize;
			}
			set
			{
				this.DataObject.MaxMessageSize = value;
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewDeliveryAgentConnector(base.Name, base.FormatMultiValuedProperty(this.AddressSpaces));
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			DeliveryAgentConnector deliveryAgentConnector = (DeliveryAgentConnector)base.PrepareDataObject();
			if (!MultiValuedPropertyBase.IsNullOrEmpty(this.SourceTransportServers))
			{
				deliveryAgentConnector.SourceTransportServers = base.ResolveIdParameterCollection<ServerIdParameter, Server, ADObjectId>(this.SourceTransportServers, base.DataSession, this.RootId, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotFound), new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotUnique), null, null);
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
				if (server != null && server.IsHubTransportServer && server.IsExchange2007OrLater)
				{
					deliveryAgentConnector.SourceTransportServers = new MultiValuedProperty<ADObjectId>(false, SendConnectorSchema.SourceTransportServers, new ADObjectId[]
					{
						server.Id
					});
				}
			}
			if (!MultiValuedPropertyBase.IsNullOrEmpty(deliveryAgentConnector.SourceTransportServers))
			{
				ManageSendConnectors.SetConnectorHomeMta(deliveryAgentConnector, (IConfigurationSession)base.DataSession);
			}
			ManageSendConnectors.SetConnectorId(deliveryAgentConnector, ((ITopologyConfigurationSession)base.DataSession).GetRoutingGroupId());
			return deliveryAgentConnector;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (TopologyProvider.IsAdamTopology())
			{
				base.WriteError(new LocalizedException(Strings.CannotRunDeliveryAgentConnectorTaskOnEdge), ErrorCategory.InvalidOperation, null);
			}
			ADObjectId sourceRoutingGroup = this.DataObject.SourceRoutingGroup;
			bool flag;
			bool flag2;
			LocalizedException ex = ManageSendConnectors.ValidateTransportServers((IConfigurationSession)base.DataSession, this.DataObject, ref sourceRoutingGroup, false, true, this, out flag, out flag2);
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

		protected override void InternalProcessRecord()
		{
			DeliveryAgentConnector dataObject = this.DataObject;
			if (dataObject.IsScopedConnector)
			{
				ManageSendConnectors.AdjustAddressSpaces(dataObject);
			}
			base.InternalProcessRecord();
			ManageSendConnectors.UpdateGwartLastModified((ITopologyConfigurationSession)base.DataSession, this.DataObject.SourceRoutingGroup, new ManageSendConnectors.ThrowTerminatingErrorDelegate(base.WriteError));
		}
	}
}
