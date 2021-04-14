using System;
using System.Configuration;
using System.Management.Automation;
using System.Reflection;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.PswsClient;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal static class EOPRecipient
	{
		public static string GetPsWsHostServerName()
		{
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
			string result = string.Empty;
			KeyValueConfigurationElement keyValueConfigurationElement = configuration.AppSettings.Settings[EOPRecipient.PsWsHostServerKey];
			if (keyValueConfigurationElement != null && !string.IsNullOrEmpty(keyValueConfigurationElement.Value))
			{
				result = keyValueConfigurationElement.Value;
			}
			return result;
		}

		public static void SetProperty(PswsCmdlet cmdlet, string propertyName, string propertyValue)
		{
			ArgumentValidator.ThrowIfNull("cmdlet", cmdlet);
			if (!string.IsNullOrEmpty(propertyValue))
			{
				cmdlet.Parameters[propertyName] = propertyValue;
			}
		}

		public static void SetProperty(PswsCmdlet cmdlet, string propertyName, string[] propertyValue)
		{
			ArgumentValidator.ThrowIfNull("cmdlet", cmdlet);
			if (propertyValue != null)
			{
				cmdlet.Parameters[propertyName] = propertyValue;
			}
		}

		public static void SetProperty(PswsCmdlet cmdlet, string propertyName, ProxyAddress propertyValue)
		{
			ArgumentValidator.ThrowIfNull("cmdlet", cmdlet);
			if (propertyValue != null)
			{
				cmdlet.Parameters[propertyName] = propertyValue.ProxyAddressString;
			}
		}

		public static void SetProperty(PswsCmdlet cmdlet, string propertyName, WindowsLiveId propertyValue)
		{
			if (propertyValue != null)
			{
				cmdlet.Parameters[propertyName] = propertyValue.ToString();
			}
		}

		public static void SetProperty(PswsCmdlet cmdlet, string propertyName, OrganizationIdParameter propertyValue)
		{
			if (propertyValue != null)
			{
				cmdlet.Parameters[propertyName] = propertyValue.ToString();
			}
		}

		public static void SetProperty(PswsCmdlet cmdlet, string propertyName, SmtpAddress propertyValue)
		{
			if (propertyValue.Length > 0)
			{
				cmdlet.Parameters[propertyName] = propertyValue.ToString();
			}
		}

		public static void SetProperty(PswsCmdlet cmdlet, string propertyName, SecureString propertyValue)
		{
			ArgumentValidator.ThrowIfNull("cmdlet", cmdlet);
			if (propertyValue != null)
			{
				cmdlet.Parameters[propertyName] = propertyValue;
			}
		}

		public static void SetProperty(PswsCmdlet cmdlet, string propertyName, CountryInfo propertyValue)
		{
			ArgumentValidator.ThrowIfNull("cmdlet", cmdlet);
			if (propertyValue != null)
			{
				cmdlet.Parameters[propertyName] = propertyValue;
			}
		}

		public static void SetProperty(PswsCmdlet cmdlet, string propertyName, ProxyAddressCollection propertyValue)
		{
			ArgumentValidator.ThrowIfNull("cmdlet", cmdlet);
			if (propertyValue != null)
			{
				cmdlet.Parameters[propertyName] = propertyValue.ToStringArray();
			}
		}

		public static void CheckForError(Task task, PswsCmdlet cmdlet)
		{
			ArgumentValidator.ThrowIfNull("task", task);
			ArgumentValidator.ThrowIfNull("cmdlet", cmdlet);
			if (!string.IsNullOrEmpty(cmdlet.Error))
			{
				string errMsg = cmdlet.Error.ToString();
				EOPRecipient.PublishErrorEvent(errMsg);
				string text = "<NULL>";
				Authenticator authenticator = cmdlet.Authenticator as Authenticator;
				if (authenticator != null)
				{
					text = text.ToString();
				}
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_FfoReportingRecipientTaskFailure, new string[]
				{
					cmdlet.Organization ?? "<NULL>",
					cmdlet.HostServerName ?? "<NULL>",
					cmdlet.ToString() ?? "<NULL>",
					cmdlet.AdditionalHeaders.ToString() ?? "<NULL>",
					(cmdlet.RequestTimeout != null) ? cmdlet.RequestTimeout.Value.ToString() : "<NULL>",
					cmdlet.Exception.ToString() ?? "<NULL>"
				});
				task.WriteError(cmdlet.Exception, ErrorCategory.InvalidOperation, null);
			}
		}

		private static void PublishErrorEvent(string errMsg)
		{
			if (!errMsg.Contains("ProtocolError"))
			{
				EventNotificationItem.Publish(ExchangeComponent.FfoUmc.Name, EOPRecipient.PswsFailureMonitor, null, errMsg, ResultSeverityLevel.Error, false);
			}
		}

		private static string PsWsHostServerKey = "PsWsHostServerName";

		internal static readonly string PswsFailureMonitor = "FFO-UMC-EOPRecipient-PSWS-Failure";
	}
}
