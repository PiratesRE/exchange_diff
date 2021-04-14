using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OAuth
{
	public class OAuthPartnerProbe : ProbeWorkItem
	{
		public OAuthPartnerProbe()
		{
			this.Tracer = ExTraceGlobals.EWSTracer;
		}

		private protected bool Verbose { protected get; private set; }

		private protected bool TrustAnySslCertificate { protected get; private set; }

		private protected int ProbeTimeLimit { protected get; private set; }

		private protected int HttpRequestTimeout { protected get; private set; }

		protected Trace Tracer { get; set; }

		protected OAuthPartnerProbe.ProbeState State { get; set; }

		private protected DateTime TimeStarted { protected get; private set; }

		protected DateTime TimeCompleted { get; set; }

		protected string DiagnosticMessage { get; set; }

		protected ADUser MonitoringUser { get; set; }

		protected Exception Error { get; set; }

		protected string Breadcrumbs
		{
			get
			{
				if (this.breadcrumbs != null)
				{
					return this.breadcrumbs.ToString();
				}
				return string.Empty;
			}
		}

		protected static ProbeDefinition CreateDefinition(string monitoringUser, string probeName, string targetResourceName, Uri targetEndpoint, string typeName)
		{
			return new ProbeDefinition
			{
				AssemblyPath = OAuthDiscovery.AssemblyPath,
				TypeName = typeName,
				Name = probeName,
				ServiceName = ExchangeComponent.Ews.Name,
				TargetResource = targetResourceName,
				Endpoint = ((targetEndpoint != null) ? targetEndpoint.AbsoluteUri : string.Empty),
				RecurrenceIntervalSeconds = 900,
				TimeoutSeconds = 60,
				MaxRetryAttempts = 0,
				Account = monitoringUser,
				AccountPassword = string.Empty,
				AccountDisplayName = monitoringUser
			};
		}

		protected static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
		{
			return true;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.TimeStarted = DateTime.UtcNow;
			this.State = OAuthPartnerProbe.ProbeState.PreparingRequest;
			WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, "configuring probe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 190);
			this.Configure();
			if (this.TrustAnySslCertificate)
			{
				CertificateValidationManager.RegisterCallback("OauthPartnerProbe:", new RemoteCertificateValidationCallback(OAuthPartnerProbe.ValidateRemoteCertificate));
			}
			string text = string.Format("accessing endpoint {0} with user {1} from probe {2}", string.IsNullOrEmpty(base.Definition.Endpoint) ? "(none)" : base.Definition.Endpoint, base.Definition.Account, base.Definition.Name);
			if (this.Verbose)
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += text;
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 210);
			}
			try
			{
				text = string.Format("getting user {0} from AD", base.Definition.Account);
				this.DropBreadcrumb(text);
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 217);
				this.MonitoringUser = this.GetADUser();
				if (this.MonitoringUser == null)
				{
					throw new ApplicationException(string.Format("OAuthPartnerProbe FAILED: unable to retrieve monitoring user{0} from AD", base.Definition.Account));
				}
				text = "starting OAuth request";
				this.DropBreadcrumb(text);
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 230);
				this.State = OAuthPartnerProbe.ProbeState.RunningRequest;
				this.State = this.RunOAuthPartnerProbe();
				text = "request completed, result is " + this.State.ToString();
				this.DropBreadcrumb(text);
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 238);
				this.TimeCompleted = DateTime.UtcNow;
			}
			catch (Exception ex)
			{
				this.State = OAuthPartnerProbe.ProbeState.FailedRequest;
				this.Error = ex;
				this.TimeCompleted = DateTime.UtcNow;
				this.DropBreadcrumb("request failed: " + OAuthPartnerProbe.Flatten(ex));
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += OAuthPartnerProbe.Flatten(ex);
				text = string.Format("OAuthPartnerProbe FAILED: uncaught exception thrown {0}", ex.Message);
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 252);
				throw;
			}
			finally
			{
				int num = (int)(this.TimeCompleted - this.TimeStarted).TotalMilliseconds;
				WTFDiagnostics.TraceInformation<int>(this.Tracer, base.TraceContext, "probe completed in {0} ms", num, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 260);
				this.ReportMachineReadableProbeDetails();
				this.ReportHumanReadableProbeDetails();
				base.Result.SampleValue = (double)num;
			}
			OAuthPartnerProbe.ProbeState state = this.State;
			if (state == OAuthPartnerProbe.ProbeState.Passed)
			{
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, "OAuthPartnerProbe PASSED", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 278);
				return;
			}
			WTFDiagnostics.TraceInformation<OAuthPartnerProbe.ProbeState>(this.Tracer, base.TraceContext, "OAuthPartnerProbe FAILED: {0}", this.State, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 285);
			throw new ApplicationException(string.Format("OAuthPartnerProbe FAILED: {0}", this.State));
		}

		protected virtual OAuthPartnerProbe.ProbeState RunOAuthPartnerProbe()
		{
			this.DiagnosticMessage = string.Empty;
			return OAuthPartnerProbe.ProbeState.Passed;
		}

		private static string Flatten(Exception e)
		{
			return e.ToString().Replace("\r\n", "+");
		}

		private void Configure()
		{
			this.Verbose = this.ReadAttribute("Verbose", true);
			this.TrustAnySslCertificate = this.ReadAttribute("TrustAnySslCertificate", false);
			this.ProbeTimeLimit = Math.Max(base.Definition.TimeoutSeconds * 1000, 60000);
			this.HttpRequestTimeout = (int)this.ReadAttribute("HttpRequestTimeout", TimeSpan.FromMilliseconds((double)(this.ProbeTimeLimit - 1000))).TotalMilliseconds;
			if (this.Verbose)
			{
				string text = string.Format("probe defined timeout={0}ms, actual timeout={1}ms, http request timeout={2}ms\r\n", base.Definition.TimeoutSeconds * 1000, this.ProbeTimeLimit, this.HttpRequestTimeout);
				ProbeResult result = base.Result;
				result.ExecutionContext += text;
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "Configure", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 345);
			}
		}

		private ADUser GetADUser()
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.GetADUser: Getting AD information for user {0}", base.Definition.Account, null, "GetADUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 359);
			SmtpAddress smtpAddress = SmtpAddress.Empty;
			try
			{
				smtpAddress = SmtpAddress.Parse(base.Definition.Account);
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.GetADUser: Failed to parse SMTP address for user {0} Exception: {1}", base.Definition.Account, ex.ToString(), null, "GetADUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 371);
				return null;
			}
			ADSessionSettings sessionSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(smtpAddress.Domain);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 383, "GetADUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs");
			QueryFilter filter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.UserPrincipalName, base.Definition.Account),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.PrimarySmtpAddress, base.Definition.Account)
			});
			ADRecipient[] array = tenantOrRootOrgRecipientSession.Find(null, QueryScope.SubTree, filter, null, 1);
			if (array != null && array.Length > 0)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.GetADUser: Successfully retrieved AD information for user {0}", base.Definition.Account, null, "GetADUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 408);
				return array[0] as ADUser;
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.EWSTracer, base.TraceContext, "OAuthDiscovery.GetADUser: Unable to get AD information for user {0}", base.Definition.Account, null, "GetADUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPartnerProbe.cs", 417);
			return null;
		}

		private void DropBreadcrumb(string s)
		{
			if (this.breadcrumbs == null)
			{
				this.breadcrumbs = new StringBuilder(512);
			}
			int num = (int)(DateTime.UtcNow - this.TimeStarted).TotalMilliseconds;
			this.breadcrumbs.AppendFormat("[{0:0000}] {1}\r\n", num, s);
		}

		private void ReportMachineReadableProbeDetails()
		{
			switch (this.State)
			{
			case OAuthPartnerProbe.ProbeState.PreparingRequest:
			case OAuthPartnerProbe.ProbeState.RunningRequest:
			case OAuthPartnerProbe.ProbeState.Passed:
				break;
			case OAuthPartnerProbe.ProbeState.WaitingResponse:
			case OAuthPartnerProbe.ProbeState.FailedRequest:
			case OAuthPartnerProbe.ProbeState.FailedResponse:
			case OAuthPartnerProbe.ProbeState.TimedOut:
			{
				List<string> list = this.SplitDiagnosticMessage(this.DiagnosticMessage);
				string[] array = list.ToArray();
				base.Result.StateAttribute21 = base.Definition.Endpoint;
				base.Result.StateAttribute22 = base.Definition.Account;
				int num = 0;
				if (num < array.Length)
				{
					base.Result.FailureContext = array[num++];
				}
				if (num < array.Length)
				{
					base.Result.StateAttribute1 = array[num++];
				}
				if (num < array.Length)
				{
					base.Result.StateAttribute2 = array[num++];
				}
				if (num < array.Length)
				{
					base.Result.StateAttribute3 = array[num++];
				}
				if (num < array.Length)
				{
					base.Result.StateAttribute4 = array[num++];
				}
				if (num < array.Length)
				{
					base.Result.StateAttribute5 = array[num++];
				}
				if (num < array.Length)
				{
					base.Result.StateAttribute11 = array[num++];
				}
				if (num < array.Length)
				{
					base.Result.StateAttribute12 = array[num++];
				}
				if (num < array.Length)
				{
					base.Result.StateAttribute13 = array[num++];
				}
				if (num < array.Length)
				{
					base.Result.StateAttribute14 = array[num++];
				}
				if (num < array.Length)
				{
					base.Result.StateAttribute15 = array[num++];
				}
				break;
			}
			default:
				return;
			}
		}

		private List<string> SplitDiagnosticMessage(string diagnosticMessage)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < diagnosticMessage.Length; i += 1000)
			{
				string item = diagnosticMessage.Substring(i, (i + 1000 < diagnosticMessage.Length) ? 1000 : (diagnosticMessage.Length - i));
				list.Add(item);
			}
			return list;
		}

		private void ReportHumanReadableProbeDetails()
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			int num = (int)(this.TimeCompleted - this.TimeStarted).TotalMilliseconds;
			string text = string.IsNullOrEmpty(base.Definition.Endpoint) ? "(none)" : base.Definition.Endpoint;
			switch (this.State)
			{
			case OAuthPartnerProbe.ProbeState.PreparingRequest:
				stringBuilder.AppendFormat("Probe to partner endpoint {0} {1} with user {2} after {3} milliseconds.", new object[]
				{
					text,
					this.State,
					base.Definition.Account,
					num
				});
				goto IL_1A0;
			case OAuthPartnerProbe.ProbeState.WaitingResponse:
			case OAuthPartnerProbe.ProbeState.Passed:
			case OAuthPartnerProbe.ProbeState.TimedOut:
				stringBuilder.AppendFormat("Probe to partner endpoint {0} {1} with user {2} after {3} milliseconds.", new object[]
				{
					text,
					this.State,
					base.Definition.Account,
					num
				});
				goto IL_1A0;
			case OAuthPartnerProbe.ProbeState.FailedRequest:
			case OAuthPartnerProbe.ProbeState.FailedResponse:
				stringBuilder.AppendFormat("Probe to partner endpoint {0} {1} with user {2} after {3} milliseconds.", new object[]
				{
					text,
					this.State,
					base.Definition.Account,
					num
				});
				if (this.Error != null && this.Error.Message != null)
				{
					stringBuilder.Append(this.Error.Message.Replace("+", "plus"));
					goto IL_1A0;
				}
				goto IL_1A0;
			}
			stringBuilder.AppendFormat("new state {0} - update ReportHumanReadableProbeDetails!", this.State);
			IL_1A0:
			stringBuilder.Append("+");
			if (this.Verbose)
			{
				stringBuilder.Append(this.Breadcrumbs);
			}
			ProbeResult result = base.Result;
			result.ExecutionContext += stringBuilder.ToString();
		}

		private const int MinimumTimeLimit = 60000;

		private static readonly string ProbeTypeName = typeof(OAuthPartnerProbe).FullName;

		private StringBuilder breadcrumbs;

		protected enum ProbeState
		{
			PreparingRequest,
			RunningRequest,
			WaitingResponse,
			Passed,
			FailedRequest,
			FailedResponse,
			TimedOut
		}
	}
}
