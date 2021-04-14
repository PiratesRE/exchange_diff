using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "SendConnector", DefaultParameterSetName = "Identity")]
	public sealed class GetSendConnector : GetSystemConfigurationObjectTask<SendConnectorIdParameter, SmtpSendConnectorConfig>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				return configurationSession.GetOrgContainerId().GetChildId("Administrative Groups");
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			SmtpSendConnectorConfig smtpSendConnectorConfig = dataObject as SmtpSendConnectorConfig;
			if (smtpSendConnectorConfig != null && !smtpSendConnectorConfig.IsReadOnly)
			{
				smtpSendConnectorConfig.DNSRoutingEnabled = string.IsNullOrEmpty(smtpSendConnectorConfig.SmartHostsString);
				smtpSendConnectorConfig.IsScopedConnector = smtpSendConnectorConfig.GetScopedConnector();
				smtpSendConnectorConfig.ResetChangeTracking();
			}
			base.WriteResult(dataObject);
		}

		public const string rootLocation = "Administrative Groups";
	}
}
