using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Nspi.Client;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class DoMTConnectivityProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestDoMTConnectivity, delegate
			{
				bool flag = true;
				StringBuilder stringBuilder = new StringBuilder();
				string text = new ServerIdParameter().ToString();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting DoMT check against server {0}", text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\DoMTConnectivityProbe.cs", 54);
				string text2;
				string text3;
				string text4;
				if (!DirectoryUtils.GetCredentials(out text2, out text3, out text4, this))
				{
					base.Result.StateAttribute1 = "No Monitoring users";
				}
				else
				{
					base.Result.StateAttribute3 = text2;
					if (!string.IsNullOrEmpty(text2) && SmtpAddress.IsValidSmtpAddress(text2))
					{
						NetworkCredential nc = new NetworkCredential(text2, text3);
						stringBuilder.Append(string.Format("Creating NSPI Client with local server {0} using user {1}, password {2};", text, text2, text3));
						using (NspiClient nspiClient = new NspiClient(text, nc))
						{
							Stopwatch stopwatch = Stopwatch.StartNew();
							try
							{
								stringBuilder.Append("Step 1 Binding;");
								NspiStatus nspiStatus = nspiClient.Bind(NspiBindFlags.None);
								stringBuilder.Append(string.Format("Binding result: {0} (0x{1:X8});", nspiStatus, (int)nspiStatus));
								if (nspiStatus != NspiStatus.Success)
								{
									throw new Exception("NSPI Client Binding did not return success status.");
								}
								uint num = 0U;
								stringBuilder.Append("Step 2 GetHierarchyInfo;");
								PropRowSet propRowSet;
								nspiStatus = nspiClient.GetHierarchyInfo(NspiGetHierarchyInfoFlags.None, ref num, out propRowSet);
								stringBuilder.Append(string.Format("GetHierarchyInfo result: {0} (0x{1:X8});", nspiStatus, (int)nspiStatus));
								if (nspiStatus != NspiStatus.Success)
								{
									throw new Exception("NSPI Client GetHierarchyInfo did not return success status.");
								}
								stringBuilder.Append("Step 3 Unbinding;");
								int num2 = nspiClient.Unbind();
								stringBuilder.Append(string.Format("UnBinding result: {0});", num2));
								if (num2 != 1)
								{
									throw new Exception("NSPI Client Unbinding did not return success status.");
								}
								stringBuilder.Append("Test Pass.");
								stopwatch.Stop();
								base.Result.SampleValue = (double)stopwatch.ElapsedMilliseconds;
							}
							catch (Exception ex)
							{
								stopwatch.Stop();
								flag = false;
								base.Result.Error = ex.ToString();
								base.Result.SampleValue = (double)stopwatch.ElapsedMilliseconds;
							}
						}
						base.Result.StateAttribute1 = stringBuilder.ToString();
					}
					else
					{
						base.Result.StateAttribute1 = "Empty or invalid Monitoring user, check StateAttribute3 for username";
					}
				}
				WTFDiagnostics.TraceInformation<bool, double, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", flag, base.Result.SampleValue, base.Result.StateAttribute1, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\DoMTConnectivityProbe.cs", 142);
				if (!flag)
				{
					throw new Exception(base.Result.Error);
				}
			});
		}
	}
}
