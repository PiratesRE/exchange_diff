using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Wac
{
	public class OAuthWacProbe : ProbeWorkItem
	{
		public OAuthWacProbe()
		{
			this.Tracer = ExTraceGlobals.OWATracer;
		}

		private Trace Tracer { get; set; }

		private OAuthWacProbe.ProbeState State { get; set; }

		private DateTime TimeStarted { get; set; }

		private DateTime TimeCompleted { get; set; }

		private string DiagnosticMessage { get; set; }

		private ADUser MonitoringUser { get; set; }

		internal static ProbeDefinition CreateDefinition(string monitoringUser, string probeName, string targetEndpoint, string secondaryEndpoint)
		{
			return new ProbeDefinition
			{
				TypeName = OAuthWacProbe.ProbeTypeName,
				Name = probeName,
				ServiceName = ExchangeComponent.OwaDependency.Name,
				Endpoint = targetEndpoint,
				SecondaryEndpoint = secondaryEndpoint,
				Account = monitoringUser,
				AccountPassword = string.Empty,
				AccountDisplayName = monitoringUser
			};
		}

		private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
		{
			return true;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.TimeStarted = DateTime.UtcNow;
			this.State = OAuthWacProbe.ProbeState.PreparingRequest;
			WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, "configuring probe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 136);
			CertificateValidationManager.RegisterCallback("OAuthWacProbe:", new RemoteCertificateValidationCallback(OAuthWacProbe.ValidateRemoteCertificate));
			string text = string.Format("accessing wac endpoint {0} and wopi endpoint {1} with user {2} from probe {3}", new object[]
			{
				string.IsNullOrEmpty(base.Definition.Endpoint) ? "(none)" : base.Definition.Endpoint,
				base.Definition.SecondaryEndpoint,
				base.Definition.Account,
				base.Definition.Name
			});
			base.Result.StateAttribute1 = text;
			WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 151);
			try
			{
				text = string.Format("getting user {0} from AD", base.Definition.Account);
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 156);
				this.MonitoringUser = this.GetADUser();
				if (this.MonitoringUser == null)
				{
					throw new ApplicationException(string.Format("OAuthWacProbe FAILED: unable to retrieve monitoring user{0} from AD", base.Definition.Account));
				}
				text = "starting OAuth request";
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 168);
				this.State = OAuthWacProbe.ProbeState.RunningRequest;
				this.State = this.RunOAuthWacProbe();
				text = "request completed, result is " + this.State.ToString();
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 175);
				this.TimeCompleted = DateTime.UtcNow;
			}
			catch (Exception ex)
			{
				this.State = OAuthWacProbe.ProbeState.FailedRequest;
				this.TimeCompleted = DateTime.UtcNow;
				base.Result.Error = OAuthWacProbe.Flatten(ex);
				text = string.Format("OAuthWacProbe FAILED: uncaught exception thrown {0}", ex.Message);
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 187);
				throw;
			}
			finally
			{
				base.Result.ExecutionContext = this.DiagnosticMessage;
				int num = (int)(this.TimeCompleted - this.TimeStarted).TotalMilliseconds;
				WTFDiagnostics.TraceInformation<int>(this.Tracer, base.TraceContext, "probe completed in {0} ms", num, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 196);
				base.Result.SampleValue = (double)num;
			}
			OAuthWacProbe.ProbeState state = this.State;
			if (state == OAuthWacProbe.ProbeState.Passed)
			{
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, "OAuthWacProbe PASSED", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 211);
				return;
			}
			string message = string.Format("OAuthWacProbe FAILED: {0} when {1} and the following execution context: {2}. Also here is the fatal exception information occured during the probe execution: {3}", new object[]
			{
				this.State,
				base.Result.StateAttribute1,
				base.Result.ExecutionContext,
				string.IsNullOrEmpty(base.Result.Error) ? "(none)" : base.Result.Error
			});
			WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 225);
			throw new ApplicationException(message);
		}

		private ADUser GetADUser()
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "OAuthDiscovery.GetADUser: Getting AD information for user {0}", base.Definition.Account, null, "GetADUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 240);
			SmtpAddress smtpAddress = SmtpAddress.Empty;
			try
			{
				smtpAddress = SmtpAddress.Parse(base.Definition.Account);
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.OWATracer, base.TraceContext, "OAuthDiscovery.GetADUser: Failed to parse SMTP address for user {0} Exception: {1}", base.Definition.Account, ex.ToString(), null, "GetADUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 252);
				return null;
			}
			ADSessionSettings sessionSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(smtpAddress.Domain);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 264, "GetADUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs");
			QueryFilter filter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.UserPrincipalName, base.Definition.Account),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.PrimarySmtpAddress, base.Definition.Account)
			});
			ADRecipient[] array = tenantOrRootOrgRecipientSession.Find(null, QueryScope.SubTree, filter, null, 1);
			if (array != null && array.Length > 0)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "OAuthDiscovery.GetADUser: Successfully retrieved AD information for user {0}", base.Definition.Account, null, "GetADUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 289);
				return array[0] as ADUser;
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "OAuthDiscovery.GetADUser: Unable to get AD information for user {0}", base.Definition.Account, null, "GetADUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\OAuthWacProbe.cs", 298);
			return null;
		}

		private static string Flatten(Exception e)
		{
			return e.ToString().Replace("\r\n", "+");
		}

		private OAuthWacProbe.ProbeState RunOAuthWacProbe()
		{
			string empty = string.Empty;
			Microsoft.Exchange.Security.OAuth.ResultType resultType = TestOAuthWacConnectivityHelper.SendWacOAuthRequest(base.Definition.SecondaryEndpoint, base.Definition.Endpoint, this.MonitoringUser, out empty);
			this.DiagnosticMessage = empty;
			if (resultType != Microsoft.Exchange.Security.OAuth.ResultType.Error)
			{
				return OAuthWacProbe.ProbeState.Passed;
			}
			return OAuthWacProbe.ProbeState.FailedRequest;
		}

		private static readonly string ProbeTypeName = typeof(OAuthWacProbe).FullName;

		private enum ProbeState
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
