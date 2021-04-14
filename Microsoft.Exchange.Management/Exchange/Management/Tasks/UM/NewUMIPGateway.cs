using System;
using System.Management.Automation;
using System.Net.Sockets;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("New", "UMIPGateway", SupportsShouldProcess = true)]
	public sealed class NewUMIPGateway : NewMultitenancySystemConfigurationObjectTask<UMIPGateway>
	{
		[Parameter(Mandatory = true)]
		public UMSmartHost Address
		{
			get
			{
				return this.DataObject.Address;
			}
			set
			{
				this.DataObject.Address = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddressFamily IPAddressFamily
		{
			get
			{
				return this.DataObject.IPAddressFamily;
			}
			set
			{
				this.DataObject.IPAddressFamily = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMDialPlanIdParameter UMDialPlan
		{
			get
			{
				return (UMDialPlanIdParameter)base.Fields["UMDialPlan"];
			}
			set
			{
				base.Fields["UMDialPlan"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMGlobalCallRoutingScheme GlobalCallRoutingScheme
		{
			get
			{
				return this.DataObject.GlobalCallRoutingScheme;
			}
			set
			{
				this.DataObject.GlobalCallRoutingScheme = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewUMIPGateway(base.Name.ToString(), this.Address.ToString());
			}
		}

		internal static LocalizedException ValidateFQDNInTenantAcceptedDomain(UMIPGateway gateway, IConfigurationSession session)
		{
			if (!CommonConstants.UseDataCenterCallRouting)
			{
				return null;
			}
			if (gateway == null || gateway.Address == null)
			{
				throw new ArgumentNullException("gateway");
			}
			if (gateway.GlobalCallRoutingScheme == UMGlobalCallRoutingScheme.E164)
			{
				return null;
			}
			string text = gateway.Address.ToString();
			QueryFilter filter = new NotFilter(new BitMaskAndFilter(AcceptedDomainSchema.AcceptedDomainFlags, 1UL));
			ADPagedReader<AcceptedDomain> adpagedReader = session.FindPaged<AcceptedDomain>(session.GetOrgContainerId(), QueryScope.SubTree, filter, null, 0);
			AcceptedDomain[] array = adpagedReader.ReadAllPages();
			bool flag = false;
			foreach (AcceptedDomain acceptedDomain in array)
			{
				string domain = acceptedDomain.DomainName.Domain;
				if (text.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
				{
					string text2 = text.Substring(0, text.Length - domain.Length);
					if (text2.Length == 0)
					{
						flag = true;
						break;
					}
					int num = text2.IndexOf('.');
					if (num == text2.Length - 1)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return new GatewayFqdnNotInAcceptedDomainException();
			}
			return null;
		}

		internal static LocalizedException ValidateIPAddressFamily(UMIPGateway gateway)
		{
			if (gateway.Address.IsIPAddress)
			{
				if (gateway.IsModified(UMIPGatewaySchema.IPAddressFamily) || gateway.IsModified(UMIPGatewaySchema.IPAddressFamily))
				{
					if ((gateway.Address.Address.AddressFamily == AddressFamily.InterNetwork && gateway.IPAddressFamily != IPAddressFamily.IPv4Only) || (gateway.Address.Address.AddressFamily == AddressFamily.InterNetworkV6 && gateway.IPAddressFamily != IPAddressFamily.IPv6Only))
					{
						return new GatewayIPAddressFamilyInconsistentException();
					}
				}
				else
				{
					IPAddressFamily ipaddressFamily = (gateway.Address.Address.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddressFamily.IPv6Only : IPAddressFamily.IPv4Only;
					gateway.IPAddressFamily = ipaddressFamily;
				}
			}
			return null;
		}

		protected override IConfigurable PrepareDataObject()
		{
			UMIPGateway umipgateway = (UMIPGateway)base.PrepareDataObject();
			umipgateway.SetId((IConfigurationSession)base.DataSession, base.Name);
			return umipgateway;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			TaskLogger.LogEnter();
			if (!base.HasErrors)
			{
				if (CommonConstants.UseDataCenterCallRouting && this.DataObject.Address.IsIPAddress && this.DataObject.GlobalCallRoutingScheme != UMGlobalCallRoutingScheme.E164)
				{
					base.WriteError(new GatewayAddressRequiresFqdnException(), ErrorCategory.InvalidOperation, this.DataObject);
				}
				LocalizedException ex = NewUMIPGateway.ValidateFQDNInTenantAcceptedDomain(this.DataObject, this.ConfigurationSession);
				if (ex != null)
				{
					base.WriteError(ex, ErrorCategory.InvalidOperation, this.DataObject);
				}
				string text = this.DataObject.Address.ToString();
				this.CheckAndWriteError(new IPGatewayAlreadyExistsException(text), text);
				if (this.UMDialPlan != null)
				{
					IConfigurationSession session = (IConfigurationSession)base.DataSession;
					this.dialPlan = (UMDialPlan)base.GetDataObject<UMDialPlan>(this.UMDialPlan, session, null, new LocalizedString?(Strings.NonExistantDialPlan(this.UMDialPlan.ToString())), new LocalizedString?(Strings.MultipleDialplansWithSameId(this.UMDialPlan.ToString())));
					if (this.dialPlan.URIType == UMUriType.SipName && !VariantConfiguration.InvariantNoFlightingSnapshot.UM.HuntGroupCreationForSipDialplans.Enabled)
					{
						base.WriteError(new CannotCreateGatewayForHostedSipDialPlanException(), ErrorCategory.InvalidOperation, this.DataObject);
					}
				}
				if (!this.DataObject.IsModified(UMIPGatewaySchema.GlobalCallRoutingScheme))
				{
					if (CommonConstants.UseDataCenterCallRouting)
					{
						this.GlobalCallRoutingScheme = UMGlobalCallRoutingScheme.GatewayGuid;
					}
					else
					{
						this.GlobalCallRoutingScheme = UMGlobalCallRoutingScheme.None;
					}
				}
				LocalizedException ex2 = NewUMIPGateway.ValidateIPAddressFamily(this.DataObject);
				if (ex2 != null)
				{
					base.WriteError(ex2, ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.CreateParentContainerIfNeeded(this.DataObject);
			base.WriteVerbose(Strings.AttemptingToCreateIPGateway);
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				if (this.dialPlan != null)
				{
					this.CreateHuntgroup(this.dialPlan);
				}
				base.WriteResult();
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_NewIPGatewayCreated, null, new object[]
				{
					base.Name,
					this.DataObject.Address.ToString()
				});
				if (this.GlobalCallRoutingScheme == UMGlobalCallRoutingScheme.GatewayGuid)
				{
					this.WriteWarning(Strings.ConfigureGatewayToForwardCallsMsg);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult()
		{
		}

		private void CreateHuntgroup(UMDialPlan dp)
		{
			UMHuntGroup umhuntGroup = new UMHuntGroup();
			umhuntGroup.UMDialPlan = dp.Id;
			AdName adName = new AdName("CN", Strings.DefaultUMHuntGroupName.ToString());
			ADObjectId descendantId = this.DataObject.Id.GetDescendantId(new ADObjectId(adName.ToString(), Guid.Empty));
			base.WriteVerbose(Strings.AttemptingToCreateHuntgroup);
			umhuntGroup.SetId(descendantId);
			if (base.CurrentOrganizationId != null)
			{
				umhuntGroup.OrganizationId = base.CurrentOrganizationId;
			}
			else
			{
				umhuntGroup.OrganizationId = base.ExecutingUserOrganizationId;
			}
			base.DataSession.Save(umhuntGroup);
		}

		private void CheckAndWriteError(LocalizedException ex, string addr)
		{
			UMIPGateway[] array = Utility.FindGatewayByIPAddress(addr, this.ConfigurationSession);
			if (array != null && array.Length > 0)
			{
				base.WriteError(ex, ErrorCategory.InvalidOperation, null);
			}
		}

		private UMDialPlan dialPlan;
	}
}
