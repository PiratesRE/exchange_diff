using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Set", "UMCallRouterSettings", SupportsShouldProcess = true)]
	public sealed class SetUMCallRouterSettings : SetSingletonSystemConfigurationObjectTask<SIPFEServerConfiguration>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetUmCallRouterSettings;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<UMDialPlanIdParameter> DialPlans
		{
			get
			{
				return (MultiValuedProperty<UMDialPlanIdParameter>)base.Fields["DialPlans"];
			}
			set
			{
				base.Fields["DialPlans"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				ServerIdParameter serverIdParameter = this.Server ?? ServerIdParameter.Parse(Environment.MachineName);
				Server server = (Server)base.GetDataObject<Server>(serverIdParameter, base.DataSession as IConfigurationSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
				this.serverName = server.Name;
				return SIPFEServerConfiguration.GetRootId(server);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (this.DialPlans != null)
				{
					this.DataObject.DialPlans = base.ResolveIdParameterCollection<UMDialPlanIdParameter, UMDialPlan, ADObjectId>(this.DialPlans, base.DataSession, null, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.NonExistantDialPlan), new Func<IIdentityParameter, LocalizedString>(Strings.MultipleDialplansWithSameId), null, new Func<IConfigurable, IConfigurable>(this.ValidateDialPlan));
				}
				else if (base.Fields.IsModified("DialPlans") && (this.DataObject.DialPlans != null || this.DataObject.DialPlans.Count > 0))
				{
					this.DataObject.DialPlans.Clear();
				}
				if (this.DataObject.IsChanged(SIPFEServerConfigurationSchema.ExternalHostFqdn))
				{
					if (this.DataObject.ExternalHostFqdn != null && this.DataObject.ExternalHostFqdn.IsIPAddress)
					{
						base.WriteError(new LocalizedException(Strings.InvalidExternalHostFqdn), ErrorCategory.InvalidArgument, this.DataObject);
					}
					else
					{
						this.WriteWarning(Strings.ChangesTakeEffectAfterRestartingUmCallRouterService(Strings.ExternalHostFqdnChanges, this.serverName, string.Empty));
					}
				}
				if (this.DataObject.IsChanged(SIPFEServerConfigurationSchema.UMStartupMode))
				{
					switch (this.DataObject.UMStartupMode)
					{
					case UMStartupMode.TCP:
						this.WriteWarning(Strings.ChangesTakeEffectAfterRestartingUmCallRouterService(Strings.UMStartupModeChanges, this.serverName, string.Empty));
						if (!string.IsNullOrEmpty(this.DataObject.UMCertificateThumbprint))
						{
							this.DataObject.UMCertificateThumbprint = null;
							this.WriteWarning(Strings.CallRouterTransferFromTLStoTCPModeWarning);
						}
						break;
					case UMStartupMode.TLS:
					case UMStartupMode.Dual:
						this.WriteWarning(Strings.ChangesTakeEffectAfterRestartingUmCallRouterService(Strings.UMStartupModeChanges, this.serverName, Strings.ValidCertRequiredForUMCallRouter));
						if (((SIPFEServerConfiguration)this.DataObject.GetOriginalObject()).UMStartupMode == UMStartupMode.TCP)
						{
							this.WriteWarning(Strings.TransferFromTCPtoTLSModeWarning);
						}
						break;
					default:
						throw new InvalidOperationException("Unknown value of UMStartupMode");
					}
				}
				if (this.DataObject.IsChanged(SIPFEServerConfigurationSchema.SipTcpListeningPort) || this.DataObject.IsChanged(SIPFEServerConfigurationSchema.SipTlsListeningPort))
				{
					if (this.DataObject.SipTcpListeningPort == this.DataObject.SipTlsListeningPort)
					{
						base.WriteError(new TcpAndTlsPortsCannotBeSameException(), ErrorCategory.InvalidArgument, this.DataObject);
					}
					this.WriteWarning(Strings.ChangesTakeEffectAfterRestartingUmCallRouterService(Strings.PortChanges, this.serverName, Strings.FirewallCorrectlyConfigured(this.serverName)));
				}
			}
			TaskLogger.LogExit();
		}

		private IConfigurable ValidateDialPlan(IConfigurable configObject)
		{
			UMDialPlan umdialPlan = (UMDialPlan)configObject;
			if (umdialPlan.URIType != UMUriType.SipName)
			{
				this.WriteError(new CannotAddNonSipNameDialplanToCallRouterException(umdialPlan.ToString()), ErrorCategory.InvalidData, umdialPlan, false);
			}
			return umdialPlan;
		}

		private string serverName = string.Empty;
	}
}
