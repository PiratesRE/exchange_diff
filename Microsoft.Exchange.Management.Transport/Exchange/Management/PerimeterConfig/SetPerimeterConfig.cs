using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.PerimeterConfig
{
	[Cmdlet("Set", "PerimeterConfig", SupportsShouldProcess = true)]
	public sealed class SetPerimeterConfig : SetMultitenancySingletonSystemConfigurationObjectTask<PerimeterConfig>
	{
		[Parameter(Mandatory = false)]
		public MailFlowPartnerIdParameter MailFlowPartner
		{
			get
			{
				return (MailFlowPartnerIdParameter)base.Fields[PerimeterConfigSchema.MailFlowPartner];
			}
			set
			{
				base.Fields[PerimeterConfigSchema.MailFlowPartner] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetPerimeterConfig;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			PerimeterConfig perimeterConfig = (PerimeterConfig)this.GetDynamicParameters();
			if (base.Fields.IsModified(PerimeterConfigSchema.MailFlowPartner))
			{
				MailFlowPartnerIdParameter mailFlowPartner = this.MailFlowPartner;
				if (mailFlowPartner != null)
				{
					IConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 81, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\PerimeterConfig\\SetPerimeterConfig.cs");
					MailFlowPartner mailFlowPartner2 = (MailFlowPartner)base.GetDataObject<MailFlowPartner>(mailFlowPartner, session, this.RootId, new LocalizedString?(Strings.MailFlowPartnerNotExists(mailFlowPartner)), new LocalizedString?(Strings.MailFlowPartnerNotUnique(mailFlowPartner)), ExchangeErrorCategory.Client);
					perimeterConfig.MailFlowPartner = (ADObjectId)mailFlowPartner2.Identity;
					return;
				}
				perimeterConfig.MailFlowPartner = null;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			PerimeterConfig dataObject = this.DataObject;
			this.CheckForDuplicates(dataObject);
			IPAddress ipaddress;
			if (dataObject.IsChanged(PerimeterConfigSchema.InternalServerIPAddresses) && this.CheckIPv6AddressIsPresent(dataObject.InternalServerIPAddresses, out ipaddress))
			{
				base.WriteError(new IPv6AddressesAreNotAllowedInInternalServerIPAddressesException(ipaddress.ToString()), ErrorCategory.InvalidOperation, null);
			}
			if (dataObject.IsChanged(PerimeterConfigSchema.GatewayIPAddresses))
			{
				SetPerimeterConfig.IPAddressType ipaddressType;
				if (this.CheckForInvalidIPAddress(dataObject.GatewayIPAddresses, out ipaddressType, out ipaddress))
				{
					switch (ipaddressType)
					{
					case SetPerimeterConfig.IPAddressType.IPv6:
						base.WriteError(new IPv6AddressesAreNotAllowedInGatewayIPAddressesException(ipaddress.ToString()), ErrorCategory.InvalidOperation, null);
						break;
					case SetPerimeterConfig.IPAddressType.InvalidIPv4:
						base.WriteError(new InvalidIPv4AddressesAreNotAllowedInGatewayIPAddressesException(ipaddress.ToString()), ErrorCategory.InvalidOperation, null);
						break;
					case SetPerimeterConfig.IPAddressType.ReservedIPv4:
						base.WriteError(new ReservedIPv4AddressesAreNotAllowedInGatewayIPAddressesException(ipaddress.ToString()), ErrorCategory.InvalidOperation, null);
						break;
					}
				}
				if (dataObject.GatewayIPAddresses.Count > 40)
				{
					base.WriteError(new MaximumAllowedNumberOfGatewayIPAddressesExceededException(40), ErrorCategory.InvalidOperation, null);
				}
			}
			if (!dataObject.IPSafelistingSyncEnabled && dataObject.EhfConfigSyncEnabled && (dataObject.IsChanged(PerimeterConfigSchema.GatewayIPAddresses) || dataObject.IsChanged(PerimeterConfigSchema.InternalServerIPAddresses)))
			{
				base.WriteError(new CannotAddIPSafelistsIfIPSafelistingSyncDisabledException(), ErrorCategory.InvalidOperation, null);
			}
			if (dataObject.IsChanged(PerimeterConfigSchema.PerimeterOrgId) && !string.IsNullOrEmpty(dataObject.PerimeterOrgId) && !dataObject.EhfConfigSyncEnabled)
			{
				base.WriteError(new CannotSetPerimeterOrgIdIfEhfConfigSyncIsDisabledException(), ErrorCategory.InvalidOperation, null);
			}
			if ((dataObject.IsChanged(PerimeterConfigSchema.RouteOutboundViaFfoFrontendEnabled) || dataObject.IsChanged(PerimeterConfigSchema.RouteOutboundViaEhfEnabled)) && dataObject.RouteOutboundViaFfoFrontendEnabled == dataObject.RouteOutboundViaEhfEnabled)
			{
				base.WriteError(new CannotSetBothEhfAndFfoRoutingException(), ErrorCategory.InvalidOperation, null);
			}
		}

		private bool CheckIPv6AddressIsPresent(MultiValuedProperty<IPAddress> ipList, out IPAddress ipv6Address)
		{
			ipv6Address = null;
			foreach (IPAddress ipaddress in ipList)
			{
				if (ipaddress.AddressFamily == AddressFamily.InterNetworkV6)
				{
					ipv6Address = ipaddress;
					return true;
				}
			}
			return false;
		}

		private bool CheckForInvalidIPAddress(MultiValuedProperty<IPAddress> ipList, out SetPerimeterConfig.IPAddressType ipType, out IPAddress ipAddress)
		{
			ipType = SetPerimeterConfig.IPAddressType.IPv4;
			ipAddress = null;
			foreach (IPAddress ipaddress in ipList)
			{
				if (ipaddress.AddressFamily == AddressFamily.InterNetworkV6)
				{
					ipAddress = ipaddress;
					ipType = SetPerimeterConfig.IPAddressType.IPv6;
					return true;
				}
				if (!IPAddressValidation.IsValidIPv4Address(ipaddress.ToString()))
				{
					ipAddress = ipaddress;
					ipType = SetPerimeterConfig.IPAddressType.InvalidIPv4;
					return true;
				}
				if (IPAddressValidation.IsReservedIPv4Address(ipaddress.ToString()))
				{
					ipAddress = ipaddress;
					ipType = SetPerimeterConfig.IPAddressType.ReservedIPv4;
					return true;
				}
			}
			return false;
		}

		private void CheckForDuplicates(PerimeterConfig config)
		{
			if (config.GatewayIPAddresses == null || config.InternalServerIPAddresses == null || config.GatewayIPAddresses.Count == 0 || config.InternalServerIPAddresses.Count == 0)
			{
				return;
			}
			HashSet<IPAddress> hashSet = new HashSet<IPAddress>(config.GatewayIPAddresses);
			foreach (IPAddress ipaddress in config.InternalServerIPAddresses)
			{
				if (hashSet.Contains(ipaddress))
				{
					base.WriteError(new DuplicateItemInGatewayIpAddressListException(ipaddress.ToString()), ErrorCategory.InvalidOperation, ipaddress);
				}
			}
		}

		private const int maxGatewayIPAddressCount = 40;

		private enum IPAddressType
		{
			IPv4,
			IPv6,
			InvalidIPv4,
			ReservedIPv4
		}
	}
}
