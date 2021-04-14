using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Hybrid;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Update", "HybridConfiguration", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
	public sealed class UpdateHybridConfiguration : SingletonSystemConfigurationObjectActionTask<HybridConfiguration>, IUserInterface
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return HybridStrings.ConfirmationMessageUpdateHybridConfiguration;
			}
		}

		[Parameter(Mandatory = true)]
		public PSCredential OnPremisesCredentials
		{
			get
			{
				return (PSCredential)base.Fields["OnPremCredentials"];
			}
			set
			{
				base.Fields["OnPremCredentials"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public PSCredential TenantCredentials
		{
			get
			{
				return (PSCredential)base.Fields["TenantCredentials"];
			}
			set
			{
				base.Fields["TenantCredentials"] = value;
			}
		}

		[Parameter]
		public SwitchParameter ForceUpgrade
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceUpgrade"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForceUpgrade"] = value;
			}
		}

		[Parameter]
		public SwitchParameter SuppressOAuthWarning
		{
			get
			{
				return (SwitchParameter)(base.Fields["SuppressOAuthWarning"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SuppressOAuthWarning"] = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.WriteWarning(HybridStrings.WarningRedirectCU10HybridStandaloneConfiguration);
			base.InternalProcessRecord();
			Action<LocalizedString> writeVerbose = base.IsVerboseOn ? new Action<LocalizedString>(this.WriteVerbose) : null;
			using (ILogger logger = Logger.Create(writeVerbose))
			{
				IList<Tuple<string, string>> hybridConfigurationObjectValues = this.GetHybridConfigurationObjectValues();
				int maxNameLength = hybridConfigurationObjectValues.Max((Tuple<string, string> v) => v.Item1.Length);
				logger.LogInformation(string.Format("{0}\r\n{1}", "Hybrid Configuration Object", string.Join("\r\n", from v in hybridConfigurationObjectValues
				select string.Format("{0}{1} : {2}", v.Item1, new string(' ', maxNameLength - v.Item1.Length), v.Item2))));
				Func<IOnPremisesSession> createOnPremisesSessionFunc = () => new PowerShellOnPremisesSession(logger, Dns.GetHostName(), this.OnPremisesCredentials);
				Func<ITenantSession> createTenantSessionFunc = () => new PowerShellTenantSession(logger, Configuration.PowerShellEndpoint(this.DataObject.ServiceInstance), this.TenantCredentials);
				UpdateHybridConfigurationLogic.ProcessRecord(logger, this, this.DataObject, createOnPremisesSessionFunc, createTenantSessionFunc, new Action<Exception, ErrorCategory, object>(base.WriteError), this.ForceUpgrade, this.SuppressOAuthWarning);
				TaskLogger.LogExit();
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			SetHybridConfigurationLogic.Validate(this.DataObject, base.HasErrors, new Task.TaskErrorLoggingDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		public new void WriteVerbose(LocalizedString text)
		{
			base.WriteVerbose(text);
		}

		public new void WriteWarning(LocalizedString text)
		{
			base.WriteWarning(text);
		}

		public void WriteProgessIndicator(LocalizedString activity, LocalizedString statusDescription, int percentCompleted)
		{
			base.WriteProgress(activity, statusDescription, percentCompleted);
		}

		public new bool ShouldContinue(LocalizedString message)
		{
			return base.ShouldContinue(message);
		}

		private IList<Tuple<string, string>> GetHybridConfigurationObjectValues()
		{
			HybridConfiguration dataObject = this.DataObject;
			return new List<Tuple<string, string>>
			{
				new Tuple<string, string>("Features", UpdateHybridConfiguration.ToString<HybridFeature>(dataObject.Features)),
				new Tuple<string, string>("Domains", UpdateHybridConfiguration.ToString<AutoDiscoverSmtpDomain>(dataObject.Domains)),
				new Tuple<string, string>("OnPremisesSmartHost", TaskCommon.ToStringOrNull(dataObject.OnPremisesSmartHost)),
				new Tuple<string, string>("ClientAccessServers", UpdateHybridConfiguration.ToString<ADObjectId>(dataObject.ClientAccessServers)),
				new Tuple<string, string>("EdgeTransportServers", UpdateHybridConfiguration.ToString<ADObjectId>(dataObject.EdgeTransportServers)),
				new Tuple<string, string>("ReceivingTransportServers", UpdateHybridConfiguration.ToString<ADObjectId>(dataObject.ReceivingTransportServers)),
				new Tuple<string, string>("SendingTransportServers", UpdateHybridConfiguration.ToString<ADObjectId>(dataObject.SendingTransportServers)),
				new Tuple<string, string>("TlsCertificateName", TaskCommon.ToStringOrNull(dataObject.TlsCertificateName)),
				new Tuple<string, string>("ServiceInstance", TaskCommon.ToStringOrNull(dataObject.ServiceInstance))
			};
		}

		private static string ToString<T>(MultiValuedProperty<T> value)
		{
			Func<T, string> func = null;
			if (value == null)
			{
				return string.Empty;
			}
			string separator = ", ";
			if (func == null)
			{
				func = ((T v) => TaskCommon.ToStringOrNull(v));
			}
			return string.Join(separator, value.Select(func));
		}

		private const string HybridConfigurationObject = "Hybrid Configuration Object";

		private const string PropertyFeatures = "Features";

		private const string PropertyDomains = "Domains";

		private const string PropertyOnPremisesSmartHost = "OnPremisesSmartHost";

		private const string PropertyClientAccessServers = "ClientAccessServers";

		private const string PropertyEdgeTransportServers = "EdgeTransportServers";

		private const string PropertyReceivingTransportServers = "ReceivingTransportServers";

		private const string PropertySendingTransportServers = "SendingTransportServers";

		private const string PropertyTlsCertificateName = "TlsCertificateName";

		private const string PropertyServiceInstance = "ServiceInstance";
	}
}
