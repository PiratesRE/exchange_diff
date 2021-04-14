using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OutlookService
{
	public class OutlookServiceSelfTestProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<TDefinition>(TDefinition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			if (probeDefinition == null)
			{
				throw new ArgumentException("definition must be a ProbeDefinition");
			}
			probeDefinition.Endpoint = "https://localhost:444/outlookservice/exhealth.check";
			probeDefinition.TimeoutSeconds = (int)OutlookServiceSelfTestProbe.Timeout.TotalSeconds;
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
			string account = "No Account Available";
			string accountPassword = "No Password Available";
			MailboxDatabaseInfo monitoringAccount = OutlookServiceSelfTestProbe.GetMonitoringAccount();
			if (monitoringAccount != null)
			{
				account = monitoringAccount.MonitoringAccount + "@" + monitoringAccount.MonitoringAccountDomain;
				accountPassword = monitoringAccount.MonitoringAccountPassword;
			}
			return new ProbeDefinition
			{
				AssemblyPath = assemblyPath,
				TypeName = typeof(OutlookServiceSelfTestProbe).FullName,
				Name = probeName,
				ServiceName = ExchangeComponent.HxServiceMail.Name,
				RecurrenceIntervalSeconds = OutlookServiceSelfTestProbe.PingProbeRecurrenceInterval,
				TimeoutSeconds = OutlookServiceSelfTestProbe.PingProbeTimeout,
				MaxRetryAttempts = 3,
				Endpoint = endpoint,
				Account = account,
				AccountPassword = accountPassword,
				TimeoutSeconds = (int)OutlookServiceSelfTestProbe.Timeout.TotalSeconds,
				Enabled = true
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			using (HttpClient httpClient = new HttpClient())
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Entering OutlookServiceSelfTestProbe DoWork().", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceSelfTestProbe.cs", 136);
				httpClient.Timeout = TimeSpan.FromSeconds((double)base.Definition.TimeoutSeconds);
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, base.Definition.Endpoint);
				try
				{
					WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("OutlookServiceSelfTestProbe::DoWork:: Trusting all certificates", new object[0]), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceSelfTestProbe.cs", 146);
					ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
					WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("OutlookServiceSelfTestProbe::DoWork:: Sending the request and wait for the response, sending a cancellation token in case the probe timesout", base.GetType().FullName), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceSelfTestProbe.cs", 149);
					HttpResponseMessage result = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).Result;
					WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("OutlookServiceSelfTestProbe::DoWork:: Populate probe result information", new object[0]), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceSelfTestProbe.cs", 152);
					base.Result.StateAttribute2 = result.Content.ReadAsStringAsync().Result;
					base.Result.StateAttribute6 = (double)result.StatusCode;
					base.Result.StateAttribute23 = base.Definition.Account;
					base.Result.StateAttribute22 = base.Definition.AccountPassword;
					WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("OutlookServiceSelfTestProbe::DoWork:: Makeing sure that the response was Success, otherwise throw", new object[0]), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceSelfTestProbe.cs", 158);
					result.EnsureSuccessStatusCode();
				}
				catch (Exception ex)
				{
					WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("OutlookServiceSelfTestProbe::DoWork:: Report Failure exception {0}", ex), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceSelfTestProbe.cs", 163);
					base.Result.StateAttribute4 = string.Format("Exception: {0}", ex.Message);
					throw ex;
				}
			}
		}

		private static MailboxDatabaseInfo GetMonitoringAccount()
		{
			TracingContext context = new TracingContext();
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, context, string.Format("OutlookServiceSelfTestProbe::GetMonitoringAccount:: Entering GetMonitoringAccount", new object[0]), null, "GetMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceSelfTestProbe.cs", 177);
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, context, string.Format("OutlookServiceSelfTestProbe::GetMonitoringAccount:: Looking for BackendCredentials", new object[0]), null, "GetMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceSelfTestProbe.cs", 181);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe)
			{
				if (!string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccountPassword))
				{
					return mailboxDatabaseInfo;
				}
			}
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, context, string.Format("OutlookServiceSelfTestProbe::GetMonitoringAccount:: No Backend credentials found, looking for CAFE credentials", new object[0]), null, "GetMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceSelfTestProbe.cs", 193);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo2 in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				if (!string.IsNullOrWhiteSpace(mailboxDatabaseInfo2.MonitoringAccountPassword))
				{
					return mailboxDatabaseInfo2;
				}
			}
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, context, string.Format("OutlookServiceSelfTestProbe::GetMonitoringAccount:: No credentials found returning null", new object[0]), null, "GetMonitoringAccount", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\OutlookServiceSelfTestProbe.cs", 206);
			return null;
		}

		protected const string DefaultEndpoint = "https://localhost:444/outlookservice/exhealth.check";

		private const int MaxRetryAttempts = 3;

		public static readonly int PingProbeRecurrenceInterval = 60;

		public static readonly int PingProbeTimeout = OutlookServiceSelfTestProbe.PingProbeRecurrenceInterval - 2;

		public static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(55000.0);

		public static readonly string ProbeName = "OutlookServiceSelfTestProbe";
	}
}
