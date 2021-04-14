using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OutlookService
{
	internal class ExtraDetailsAlertResponder : EscalateResponder
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubjectUnhealthy, string escalationMessageUnhealthy, int probeIntervalSeconds, string logFolderRelativePath, string appPoolName, bool enabled = true, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Urgent, int minimumSecondsBetweenEscalates = 14400, string dailySchedulePattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59")
		{
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, escalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, enabled, notificationServiceClass, minimumSecondsBetweenEscalates, string.Empty, false);
			responderDefinition.AssemblyPath = ExtraDetailsAlertResponder.AssemblyPath;
			responderDefinition.TypeName = ExtraDetailsAlertResponder.TypeName;
			responderDefinition.Attributes[ExtraDetailsAlertResponder.ProbeIntervalSecondsKey] = probeIntervalSeconds.ToString();
			responderDefinition.Attributes[ExtraDetailsAlertResponder.LogFolderKey] = logFolderRelativePath;
			responderDefinition.Attributes[ExtraDetailsAlertResponder.AppPoolNameKey] = appPoolName;
			return responderDefinition;
		}

		internal override void GetEscalationSubjectAndMessage(MonitorResult monitorResult, out string escalationSubject, out string escalationMessage, bool rethrow = false, Action<ResponseMessageReader> textGeneratorModifier = null)
		{
			Action<ResponseMessageReader> action = null;
			bool flag = false;
			string escalationMessage2 = base.Definition.EscalationMessage;
			if (escalationMessage2.Contains(ExtraDetailsAlertResponder.OncallHelpHtmlKeyword))
			{
				base.Definition.EscalationMessage = escalationMessage2.Replace(ExtraDetailsAlertResponder.OncallHelpHtmlKeyword, this.GetEscalationMessageHtmlTemplate());
				flag = true;
			}
			if (flag && monitorResult.LastFailedProbeResultId > 0)
			{
				ProbeResult result = base.Broker.GetProbeResult(monitorResult.LastFailedProbeId, monitorResult.LastFailedProbeResultId).ExecuteAsync(base.LocalCancellationToken, base.TraceContext).Result;
				if (result != null)
				{
					try
					{
						ExtraDetailsAlertResponder.ExtraStrings extra = this.BuildExtraStrings(result, monitorResult);
						action = delegate(ResponseMessageReader reader)
						{
							if (textGeneratorModifier != null)
							{
								textGeneratorModifier(reader);
							}
							reader.AddObjectResolver<ExtraDetailsAlertResponder.ExtraStrings>("Extra", () => extra);
						};
					}
					catch (Exception ex)
					{
						base.Result.StateAttribute5 = ex.ToString();
						WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "[ExtraDetailsAlertResponder.GetEscalationSubjectAndMessage] Exception hit while generating alert contents: {0}", ex.ToString(), null, "GetEscalationSubjectAndMessage", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\Responders\\ExtraDetailsAlertResponder.cs", 200);
					}
				}
			}
			base.GetEscalationSubjectAndMessage(monitorResult, out escalationSubject, out escalationMessage, rethrow, action ?? textGeneratorModifier);
		}

		private ExtraDetailsAlertResponder.ExtraStrings BuildExtraStrings(ProbeResult probeResult, MonitorResult monitorResult)
		{
			string impactedServerFqdn = this.GetImpactedServerFqdn(probeResult, monitorResult);
			TimeSpan t = TimeSpan.FromSeconds(double.Parse(base.Definition.Attributes[ExtraDetailsAlertResponder.ProbeIntervalSecondsKey]) * 5.0);
			DateTime dateTime;
			if (monitorResult.FirstAlertObservedTime != null)
			{
				dateTime = monitorResult.FirstAlertObservedTime.Value - t;
			}
			else
			{
				dateTime = probeResult.ExecutionStartTime - TimeSpan.FromMinutes(2.0);
			}
			string ospurl;
			string debugUrl;
			if (impactedServerFqdn.ToLower().Contains("namsdf01"))
			{
				ospurl = ExtraDetailsAlertResponder.OSPUrlSDF;
				debugUrl = ExtraDetailsAlertResponder.DebugUrlSDF;
			}
			else
			{
				ospurl = ExtraDetailsAlertResponder.OSPUrlProd;
				debugUrl = ExtraDetailsAlertResponder.DebugUrlProd;
			}
			return new ExtraDetailsAlertResponder.ExtraStrings
			{
				ServerName = ExtraDetailsAlertResponder.GetServerNameFromFqdn(impactedServerFqdn),
				ServerFQDN = impactedServerFqdn,
				Username = this.GetUsername(probeResult, monitorResult),
				Password = this.GetPassword(probeResult, monitorResult),
				LogFolder = base.Definition.Attributes[ExtraDetailsAlertResponder.LogFolderKey],
				LogStartTime = dateTime.ToString("s"),
				LogEndTime = probeResult.ExecutionEndTime.ToString("s"),
				RequestId = this.GetRequestId(probeResult, monitorResult),
				AppPoolName = base.Definition.Attributes[ExtraDetailsAlertResponder.AppPoolNameKey],
				OSPUrl = ospurl,
				DebugUrl = debugUrl,
				ProbeFullName = this.GetFullProbeName(probeResult)
			};
		}

		private static string GetServerNameFromFqdn(string fqdn)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			if (fqdn.Contains('.'))
			{
				return fqdn.Split(new char[]
				{
					'.'
				})[0];
			}
			return string.Empty;
		}

		private string GetImpactedServerFqdn(ProbeResult probeResult, MonitorResult monitorResult)
		{
			return DirectoryGeneralUtils.GetLocalFQDN();
		}

		private string GetUsername(ProbeResult probeResult, MonitorResult monitorResult)
		{
			if (probeResult.StateAttribute23 == null)
			{
				return "No Account available";
			}
			return probeResult.StateAttribute23;
		}

		private string GetPassword(ProbeResult probeResult, MonitorResult monitorResult)
		{
			if (probeResult.StateAttribute22 == null)
			{
				return "No Password available";
			}
			return probeResult.StateAttribute22;
		}

		private string GetRequestId(ProbeResult probeResult, MonitorResult monitorResult)
		{
			return probeResult.StateAttribute1;
		}

		private string GetFullProbeName(ProbeResult proberesult)
		{
			return proberesult.HealthSetName + "\\" + proberesult.ResultName;
		}

		private string GetEscalationMessageHtmlTemplate()
		{
			string result = "";
			try
			{
				using (Stream manifestResourceStream = typeof(ExtraDetailsAlertResponder).Assembly.GetManifestResourceStream(ExtraDetailsAlertResponder.OutlookServiceProbeEscalationHtml))
				{
					if (manifestResourceStream != null)
					{
						using (StreamReader streamReader = new StreamReader(manifestResourceStream))
						{
							result = streamReader.ReadToEnd();
							goto IL_87;
						}
					}
					base.Result.StateAttribute5 = string.Format("Problem reading resource file {0}", ExtraDetailsAlertResponder.OutlookServiceProbeEscalationHtml);
					WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "[ExtraDetailsAlertResponder.GetEscalationSubjectAndMessage] {0}", base.Result.StateAttribute5, null, "GetEscalationMessageHtmlTemplate", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\Responders\\ExtraDetailsAlertResponder.cs", 387);
					IL_87:;
				}
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute5 = ex.ToString();
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "[ExtraDetailsAlertResponder.GetEscalationSubjectAndMessage] Exception hit while getting EscalationMessageHtmlTemplate: {0}", ex.ToString(), null, "GetEscalationMessageHtmlTemplate", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OutlookService\\Responders\\ExtraDetailsAlertResponder.cs", 400);
			}
			return result;
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ExtraDetailsAlertResponder).FullName;

		internal static readonly string ProbeIntervalSecondsKey = "ProbeIntervalSecondsKey";

		internal static readonly string LogFolderKey = "LogFolderKey";

		internal static readonly string AppPoolNameKey = "AppPoolNameKey";

		internal static readonly string ProbeMonitorResultParserTypeKey = "ProbeMonitorResultParserTypeKey";

		private static readonly string OncallHelpHtmlKeyword = "{OnCallHelpHtml}";

		private static readonly string OutlookServiceProbeEscalationHtml = "EscalationMessage.OutlookServiceProbe.html";

		private static readonly string OSPUrlProd = "https://osp.outlook.com";

		private static readonly string DebugUrlProd = "https://osp.outlook.com/ecp/osp/Change/remote.rdp?machine=BL2PR03LG202&forest=namprd03.prod.outlook.com&service=Exchange&loc=BL2";

		private static readonly string OSPUrlSDF = "https://ospbeta.outlook.com";

		private static readonly string DebugUrlSDF = "https://ospbeta.outlook.com/ecp/osp/Change/remote.rdp?machine=SN2SDF0110LG001&forest=namsdf01.sdf.exchangelabs.com&service=Exchange&loc=SN2";

		internal class ExtraStrings
		{
			public string ServerName;

			public string ServerFQDN;

			public string Username;

			public string Password;

			public string LogFolder;

			public string LogStartTime;

			public string LogEndTime;

			public string RequestId;

			public string AppPoolName;

			public string OSPUrl;

			public string DebugUrl;

			public string ProbeFullName;
		}
	}
}
