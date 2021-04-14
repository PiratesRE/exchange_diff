using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class LiveIdAuthenticationProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (propertyBag.ContainsKey("LiveIdProbeLatencyThreshold"))
			{
				pDef.Attributes["LiveIdProbeLatencyThreshold"] = propertyBag["LiveIdProbeLatencyThreshold"].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value forLiveIdProbeLatencyThreshold");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestLiveIdAuthentication, delegate
			{
				if (!base.Definition.Attributes.ContainsKey("LiveIdProbeLatencyThreshold"))
				{
					throw new ArgumentException("LiveIdProbeLatencyThreshold");
				}
				int num = int.Parse(base.Definition.Attributes["LiveIdProbeLatencyThreshold"]);
				Random random = new Random();
				int num2 = random.Next(0, 3);
				string arg = new ServerIdParameter().ToString();
				WTFDiagnostics.TraceInformation<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting LiveId availability check against server {0}", arg, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\LiveIdAuthenticationProbe.cs", 86);
				string text = string.Empty;
				bool flag = false;
				base.Result.StateAttribute1 = string.Empty;
				string text2;
				string text3;
				string text4;
				if (!DirectoryUtils.GetLiveIdProbeCredentials(out text2, out text3, out text4, this) && string.IsNullOrEmpty(text2) && !SmtpAddress.IsValidSmtpAddress(text2))
				{
					text2 = "fakeuser_123456789@faketenant_123456789.onmicrosoft.com";
					text3 = "fakePassword";
					base.Result.StateAttribute1 = string.Format("No Monitoring users. Will use fake user name: {0} and fake password 'fakePassword', expecting auth result as InvalidCreds.  ", text2);
				}
				if (string.IsNullOrEmpty(text3))
				{
					text2 = "fakeuser_123456789@faketenant_123456789.onmicrosoft.com";
					text3 = "fakePassword";
					ProbeResult result = base.Result;
					result.StateAttribute1 += "Monitoring service could not set correct password for the monitoring account.  Hence will use fake user and password.  Expecting auth result as InvalidCreds.  ";
				}
				base.Result.StateAttribute3 = string.Format("User {0} password {1}", string.IsNullOrEmpty(text2) ? "null" : text2, string.IsNullOrEmpty(text3) ? "null" : text3);
				LiveIdBasicAuthentication liveIdBasicAuthentication = new LiveIdBasicAuthentication();
				liveIdBasicAuthentication.ApplicationName = "LiveIDAuthenticationProbeSTx";
				liveIdBasicAuthentication.UserIpAddress = null;
				liveIdBasicAuthentication.UserAgent = "LiveIDAuthenticationProbeSTx";
				liveIdBasicAuthentication.SyncAD = true;
				liveIdBasicAuthentication.SyncADBackEndOnly = true;
				liveIdBasicAuthentication.BypassPositiveLogonCache = (num2 != 2);
				try
				{
					Stopwatch stopwatch = Stopwatch.StartNew();
					IAsyncResult asyncResult = liveIdBasicAuthentication.BeginGetCommonAccessToken(Encoding.Default.GetBytes(text2), Encoding.Default.GetBytes(text3), null, Guid.Empty, null, null);
					LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
					if (lazyAsyncResult != null)
					{
						lazyAsyncResult.InternalWaitForCompletion();
					}
					else
					{
						asyncResult.AsyncWaitHandle.WaitOne();
						asyncResult.AsyncWaitHandle.Close();
					}
					string text5;
					LiveIdAuthResult liveIdAuthResult = liveIdBasicAuthentication.EndGetCommonAccessToken(asyncResult, out text5);
					stopwatch.Stop();
					if (liveIdAuthResult == LiveIdAuthResult.Success)
					{
						flag = true;
					}
					else if (liveIdAuthResult == LiveIdAuthResult.ExpiredCreds || liveIdAuthResult == LiveIdAuthResult.InvalidCreds)
					{
						flag = true;
					}
					else
					{
						flag = false;
						text = liveIdAuthResult.ToString();
						base.Result.StateAttribute2 = DirectoryUtils.ExceptionType.ProtectedServiceHostIssue.ToString();
					}
					if (flag && stopwatch.ElapsedMilliseconds > (long)num)
					{
						text = string.Format("High Latency in msecs Actual {0}  Expected {1}", stopwatch.ElapsedMilliseconds, num);
						base.Result.StateAttribute2 = DirectoryUtils.ExceptionType.AuthenticationFailureNotServiceIssue.ToString();
						flag = false;
					}
					string lastRequestErrorMessage = liveIdBasicAuthentication.LastRequestErrorMessage;
					ProbeResult result2 = base.Result;
					result2.StateAttribute1 += (string.IsNullOrEmpty(lastRequestErrorMessage) ? null : lastRequestErrorMessage);
					base.Result.Error = (flag ? null : text);
					base.Result.SampleValue = (double)stopwatch.ElapsedMilliseconds;
				}
				catch (CommunicationObjectFaultedException ex)
				{
					flag = false;
					text = string.Format("{0} occured when FederatedAuthService AuthServiceClient due to CommunicationObjectFaultedException {1}", DirectoryUtils.ExceptionType.ProtectedServiceHostIssue.ToString(), ex.ToString());
					base.Result.StateAttribute2 = DirectoryUtils.ExceptionType.ProtectedServiceHostIssue.ToString();
					base.Result.Error = text;
				}
				catch (TimeoutException ex2)
				{
					flag = false;
					text = string.Format("{0} occured when FederatedAuthService AuthServiceClient due to Timeout Exception {1}", DirectoryUtils.ExceptionType.ProtectedServiceHostIssue.ToString(), ex2.ToString());
					base.Result.StateAttribute2 = DirectoryUtils.ExceptionType.ProtectedServiceHostIssue.ToString();
					base.Result.Error = text;
				}
				WTFDiagnostics.TraceInformation<bool, double, string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", flag, base.Result.SampleValue, base.Result.StateAttribute1, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\LiveIdAuthenticationProbe.cs", 207);
				if (!flag)
				{
					throw new Exception(base.Result.Error);
				}
			});
		}
	}
}
