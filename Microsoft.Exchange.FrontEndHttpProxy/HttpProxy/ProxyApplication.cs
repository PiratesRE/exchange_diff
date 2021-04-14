using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.EventLogs;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	public class ProxyApplication : HttpApplication
	{
		public static string ApplicationVersion
		{
			get
			{
				return HttpProxyGlobals.ApplicationVersion;
			}
		}

		private static void ConfigureServicePointManager()
		{
			ServicePointManager.DefaultConnectionLimit = HttpProxySettings.ServicePointConnectionLimit.Value;
			ServicePointManager.UseNagleAlgorithm = false;
			ProxyApplication.ConfigureSecureProtocols();
		}

		private static void ConfigureSecureProtocols()
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.EnableTls11.Enabled)
			{
				ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11;
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.EnableTls12.Enabled)
			{
				ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
			}
		}

		private void Application_Start(object sender, EventArgs e)
		{
			Diagnostics.InitializeWatsonReporting();
			if (Globals.InstanceType == InstanceType.NotInitialized)
			{
				string text = HttpProxyGlobals.ProtocolType.ToString();
				text = "FE_" + text;
				Globals.InitializeMultiPerfCounterInstance(text);
			}
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				Task.Factory.StartNew(delegate()
				{
					SettingOverrideSync.Instance.Start(true);
				});
				CertificateValidationManager.RegisterCallback(Constants.CertificateValidationComponentId, ProxyApplication.RemoteCertificateValidationCallback);
				ProxyApplication.ConfigureServicePointManager();
				if (DownLevelServerManager.IsApplicable)
				{
					DownLevelServerManager.Instance.Initialize();
				}
			});
			PerfCounters.UpdateHttpProxyPerArrayCounters();
			Diagnostics.Logger.LogEvent(FrontEndHttpProxyEventLogConstants.Tuple_ApplicationStart, null, new object[]
			{
				HttpProxyGlobals.ProtocolType
			});
		}

		private void Application_End(object sender, EventArgs e)
		{
			Diagnostics.Logger.LogEvent(FrontEndHttpProxyEventLogConstants.Tuple_ApplicationShutdown, null, new object[]
			{
				HttpProxyGlobals.ProtocolType
			});
			RequestDetailsLogger.FlushQueuedFileWrites();
			SettingOverrideSync.Instance.Stop();
		}

		private void Application_Error(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			Exception lastError = httpApplication.Server.GetLastError();
			Diagnostics.ReportException(lastError, FrontEndHttpProxyEventLogConstants.Tuple_InternalServerError, null, "Exception from Application_Error event: {0}");
		}

		internal static readonly RemoteCertificateValidationCallback RemoteCertificateValidationCallback = (object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => HttpProxyRegistry.OwaAllowInternalUntrustedCerts.Member || errors == SslPolicyErrors.None;
	}
}
