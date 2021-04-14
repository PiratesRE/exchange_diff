using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OutlookService.Probes
{
	public class OutlookServiceLocalPingProbe : OutlookServicePingProbe
	{
		static OutlookServiceLocalPingProbe()
		{
			OutlookServiceLocalPingProbe.TrustAllCerts();
		}

		private static void TrustAllCerts()
		{
			ServicePointManager.ServerCertificateValidationCallback = ((object param0, X509Certificate param1, X509Chain param2, SslPolicyErrors param3) => true);
		}

		public override void PopulateDefinition<TDefinition>(TDefinition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			if (probeDefinition == null)
			{
				throw new ArgumentException("definition must be a ProbeDefinition");
			}
			probeDefinition.Endpoint = "https://localhost:444/outlookservice";
			probeDefinition.TimeoutSeconds = OutlookServiceLocalPingProbe.Timeout;
			if (propertyBag.ContainsKey("Endpoint"))
			{
				probeDefinition.Endpoint = propertyBag["Endpoint"];
			}
			int timeoutSeconds;
			if (propertyBag.ContainsKey("TimeOutSeconds") && int.TryParse(propertyBag["TimeOutSeconds"], out timeoutSeconds))
			{
				probeDefinition.TimeoutSeconds = timeoutSeconds;
			}
		}

		public static ProbeDefinition CreateDefinition(string assemblyPath, string probeName, string endpoint)
		{
			return new ProbeDefinition
			{
				AssemblyPath = assemblyPath,
				TypeName = typeof(OutlookServiceLocalPingProbe).FullName,
				Name = probeName,
				ServiceName = ExchangeComponent.HxServiceMail.Name,
				RecurrenceIntervalSeconds = OutlookServiceLocalPingProbe.PingProbeRecurrenceIntervalSeconds,
				TimeoutSeconds = OutlookServiceLocalPingProbe.Timeout,
				MaxRetryAttempts = 3,
				Endpoint = endpoint,
				Account = string.Empty,
				AccountPassword = string.Empty,
				Enabled = true
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			MailboxDatabaseInfo localMonitoringAccount = this.GetLocalMonitoringAccount();
			if (localMonitoringAccount != null)
			{
				base.Definition.Account = localMonitoringAccount.MonitoringAccountUserPrincipalName;
				base.Definition.AccountPassword = localMonitoringAccount.MonitoringAccountPassword;
				WTFDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "OutlookServiceLocalPingProbe.DoWork : Endpoint={0}, Account={1}, Password={2}", base.Definition.Endpoint, base.Definition.Account, base.Definition.AccountPassword, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceLocalPingProbe.cs", 152);
			}
			else
			{
				base.Definition.Account = string.Empty;
				base.Definition.AccountPassword = string.Empty;
				WTFDiagnostics.TraceDebug(ExTraceGlobals.HTTPTracer, base.TraceContext, "OutlookServiceLocalPingProbe.DoWork : Monitoring user does not exist. Resetting Account to String.Empty", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceLocalPingProbe.cs", 165);
			}
			base.DoWork(cancellationToken);
		}

		protected override void ExecuteRequest(SocketClient client)
		{
			if (string.IsNullOrEmpty(base.Definition.Account) || string.IsNullOrEmpty(base.Definition.AccountPassword))
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("Execution will succeed because monitoring account not yet loaded", new object[0]), null, "ExecuteRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceLocalPingProbe.cs", 182);
				base.Result.ResultType = ResultType.Succeeded;
				return;
			}
			base.ExecuteRequest(client);
		}

		private MailboxDatabaseInfo GetLocalMonitoringAccount()
		{
			TracingContext context = new TracingContext();
			string typeName = base.Definition.TypeName;
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, context, string.Format("{0}::GetMonitoringAccount:: Entering GetMonitoringAccount", typeName), null, "GetLocalMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceLocalPingProbe.cs", 197);
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, context, string.Format("{0}::GetMonitoringAccount:: Checking if MailboxDatabaseEndpointExists", typeName), null, "GetLocalMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceLocalPingProbe.cs", 200);
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, context, string.Format("{0}::GetMonitoringAccount:: No Mailbox found on this server", typeName), null, "GetLocalMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceLocalPingProbe.cs", 205);
				return null;
			}
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, context, string.Format("{0}::GetMonitoringAccount:: Looking for BackendCredentials", typeName), null, "GetLocalMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceLocalPingProbe.cs", 210);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				if (!string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccountPassword))
				{
					WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, context, string.Format("{0}::GetMonitoringAccount:: Backend Credentials Found Account = {1} Password = {2}", typeName, mailboxDatabaseInfo.MonitoringAccount, mailboxDatabaseInfo.MonitoringAccountPassword), null, "GetLocalMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceLocalPingProbe.cs", 219);
					return mailboxDatabaseInfo;
				}
			}
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, context, string.Format("OutlookServiceLocalPingProbe::GetMonitoringAccount:: No credentials found returning null", new object[0]), null, "GetLocalMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceLocalPingProbe.cs", 224);
			return null;
		}

		protected const string DefaultEndpoint = "https://localhost:444/outlookservice";

		private const string PropertyBagEndpoint = "Endpoint";

		private const string PropertyBagTimeOutSeconds = "TimeOutSeconds";

		private const int MaxRetryAttempts = 3;

		public static readonly int PingProbeRecurrenceIntervalSeconds = 60;

		public new static readonly int Timeout = 50;

		public static readonly string ProbeName = "OutlookServiceLocalPingProbe";
	}
}
