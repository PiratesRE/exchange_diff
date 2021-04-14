using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps
{
	public class RpsEscalateResponder : EscalateResponder
	{
		private static FailureCategoryList FailureCategoryList
		{
			get
			{
				if (RpsEscalateResponder.failureCategoryList == null)
				{
					RpsEscalateResponder.failureCategoryList = RpsEscalateResponder.LoadFailureCategory();
				}
				return RpsEscalateResponder.failureCategoryList;
			}
		}

		public new static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationService, string escalationTeam, string escalationSubjectUnhealthy, string escalationMessageUnhealthy, bool enabled = true, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Urgent, int minimumSecondsBetweenEscalates = 14400, string dailySchedulePattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", bool loadEscalationMessageUnhealthyFromResource = false)
		{
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, escalationService, escalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, enabled, notificationServiceClass, minimumSecondsBetweenEscalates, dailySchedulePattern, loadEscalationMessageUnhealthyFromResource);
			responderDefinition.AssemblyPath = typeof(RpsEscalateResponder).Assembly.Location;
			responderDefinition.TypeName = typeof(RpsEscalateResponder).FullName;
			return responderDefinition;
		}

		internal override void GetEscalationSubjectAndMessage(MonitorResult monitorResult, out string escalationSubject, out string escalationMessage, bool rethrow = false, Action<ResponseMessageReader> textGeneratorModifier = null)
		{
			escalationMessage = null;
			escalationSubject = null;
			StringBuilder stringBuilder = new StringBuilder();
			try
			{
				base.GetEscalationSubjectAndMessage(monitorResult, out escalationSubject, out escalationMessage, false, null);
				MonitorDefinition monitorDefinition = this.GetMonitorDefinition(monitorResult.WorkItemId);
				if (monitorDefinition == null)
				{
					escalationMessage += "<br/>No monitor definition is found.<br/>";
					return;
				}
				DateTime startTime = (monitorResult.FirstAlertObservedTime != null) ? monitorResult.FirstAlertObservedTime.Value.AddSeconds(-monitorDefinition.SecondaryMonitoringThreshold) : monitorResult.ExecutionStartTime.AddSeconds(-monitorDefinition.SecondaryMonitoringThreshold);
				if (monitorResult.ExecutionStartTime > startTime.AddHours(4.0))
				{
					startTime = monitorResult.ExecutionStartTime.AddHours(-4.0);
				}
				int num = 0;
				List<ProbeResult> allFailedProbeResults = this.GetAllFailedProbeResults(monitorDefinition.SampleMask, startTime, monitorResult.ExecutionStartTime, out num);
				stringBuilder.AppendFormat("Total {0} probe results were found in last {1} seconds, {2} failed (Pass rate {3}%)<br/>", new object[]
				{
					num,
					monitorDefinition.SecondaryMonitoringThreshold,
					allFailedProbeResults.Count,
					(num - allFailedProbeResults.Count) * 100 / num
				});
				if (allFailedProbeResults.Count <= 0)
				{
					escalationMessage += "<br/>Found no failed probe resutls.<br/>";
					return;
				}
				if (!base.Broker.IsLocal())
				{
					IGrouping<string, ProbeResult>[] groupingResults = (from pr in allFailedProbeResults
					group pr by pr.StateAttribute15 into g
					orderby g.Count<ProbeResult>() descending
					select g).ToArray<IGrouping<string, ProbeResult>>();
					stringBuilder.Append(this.GetGroupingInformation("Failed probe results grouped by backend servers :", groupingResults, false, false));
				}
				IGrouping<string, ProbeResult>[] array = (from pr in allFailedProbeResults
				group pr by pr.StateAttribute12 into g
				orderby g.Count<ProbeResult>() descending
				select g).ToArray<IGrouping<string, ProbeResult>>();
				stringBuilder.Append(RpsEscalateResponder.GetFailureCategoryInfomation(array));
				stringBuilder.Append(this.GetGroupingInformation("Failed probe results grouped by failure category :", array, true, true));
				string key = array[0].Key;
				if (base.Broker.IsLocal())
				{
					stringBuilder.Append(this.GetRelatedResponderStatus(startTime));
				}
				else
				{
					if (base.Definition.Attributes.ContainsKey("OSPPageLinks"))
					{
						stringBuilder.AppendFormat(base.Definition.Attributes["OSPPageLinks"], HttpUtility.UrlEncode(startTime.ToString("o")), HttpUtility.UrlEncode(startTime.AddSeconds(monitorDefinition.SecondaryMonitoringThreshold).ToString("o")));
					}
					stringBuilder.Append(RpsEscalateResponder.GenerateImageLink(key));
				}
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute5 = ex.ToString();
			}
			base.Result.StateAttribute4 = stringBuilder.ToString();
			if (!string.IsNullOrEmpty(escalationMessage) && base.Broker.IsLocal())
			{
				escalationMessage = stringBuilder.ToString() + escalationMessage;
				return;
			}
			escalationMessage = stringBuilder.ToString();
		}

		private MonitorDefinition GetMonitorDefinition(int id)
		{
			IDataAccessQuery<MonitorDefinition> monitorDefinitions = base.Broker.GetMonitorDefinitions(DateTime.MaxValue);
			MonitorDefinition lastMonitorDefinition = null;
			monitorDefinitions.ExecuteAsync(delegate(MonitorDefinition monitorDef)
			{
				if (monitorDef.Id == id)
				{
					lastMonitorDefinition = monitorDef;
				}
			}, default(CancellationToken), base.TraceContext).Wait();
			return lastMonitorDefinition;
		}

		private List<ProbeResult> GetAllFailedProbeResults(string sampleMask, DateTime startTime, DateTime endTime, out int totalProbeResultCount)
		{
			IDataAccessQuery<ProbeResult> probeResults = base.Broker.GetProbeResults(sampleMask, startTime, endTime);
			List<ProbeResult> allFailedProbeResults = new List<ProbeResult>();
			int count = 0;
			probeResults.ExecuteAsync(delegate(ProbeResult pr)
			{
				count++;
				if (pr.ResultType != ResultType.Succeeded)
				{
					this.UpdateFailedProbeResult(pr);
					allFailedProbeResults.Add(pr);
				}
			}, default(CancellationToken), base.TraceContext).Wait();
			totalProbeResultCount = count;
			return allFailedProbeResults;
		}

		private void UpdateFailedProbeResult(ProbeResult pr)
		{
			if (string.IsNullOrEmpty(pr.StateAttribute12))
			{
				if (pr.ResultType != ResultType.Failed && pr.ResultType != ResultType.Succeeded)
				{
					pr.StateAttribute12 = pr.Error;
				}
				else
				{
					pr.StateAttribute12 = "Unknown";
				}
			}
			else if (pr.StateAttribute12.Contains(";"))
			{
				pr.StateAttribute12 = pr.StateAttribute12.Remove(pr.StateAttribute12.IndexOf(';'));
			}
			if (string.IsNullOrEmpty(pr.StateAttribute15))
			{
				pr.StateAttribute15 = "Unknown";
			}
		}

		private string GetGroupingInformation(string description, IGrouping<string, ProbeResult>[] groupingResults, bool appendErrorSample = false, bool isGroupedByFailureCategory = false)
		{
			List<string[]> list = new List<string[]>();
			if (groupingResults.Count<IGrouping<string, ProbeResult>>() > 0)
			{
				string item = isGroupedByFailureCategory ? "Failure Category" : "BackEnd Server";
				string item2 = isGroupedByFailureCategory ? "BackEnd Server" : "Failure Category";
				List<string> list2 = new List<string>();
				list2.Add(item);
				list2.Add("Count");
				if (appendErrorSample)
				{
					list2.Add("SampleError");
				}
				if (!base.Broker.IsLocal())
				{
					list2.Add(item2);
				}
				list.Add(list2.ToArray());
				foreach (IGrouping<string, ProbeResult> grouping in groupingResults)
				{
					list2.Clear();
					list2.Add(grouping.Key);
					list2.Add(grouping.Count<ProbeResult>().ToString());
					if (appendErrorSample)
					{
						list2.Add(grouping.First<ProbeResult>().Error);
					}
					if (grouping.Count<ProbeResult>() > 0 && !base.Broker.IsLocal())
					{
						StringBuilder stringBuilder = new StringBuilder();
						IGrouping<string, ProbeResult>[] array = grouping.GroupBy(delegate(ProbeResult pr)
						{
							if (!isGroupedByFailureCategory)
							{
								return pr.StateAttribute12;
							}
							return pr.StateAttribute15;
						}).ToArray<IGrouping<string, ProbeResult>>();
						foreach (IGrouping<string, ProbeResult> grouping2 in array)
						{
							stringBuilder.AppendFormat("{0}={1}", grouping2.Key, grouping2.Count<ProbeResult>());
							stringBuilder.AppendLine();
						}
						list2.Add(stringBuilder.ToString());
					}
					list.Add(list2.ToArray());
				}
				return this.ConvertToHtmlTable(list.ToArray());
			}
			return string.Format("{0} : No grouping information<br/>", description);
		}

		private string ConvertToHtmlTable(string[][] tableData)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<br/>");
			stringBuilder.Append("<table border=\"1\" style=\"padding: 6px;border-collapse:collapse;font-size:13px;font-family:consolas;\">");
			for (int i = 0; i < tableData.Length; i++)
			{
				if (i == 0)
				{
					stringBuilder.Append("<tr style=\"background: #D6EDFF;\">");
				}
				else
				{
					stringBuilder.Append("<tr>");
				}
				foreach (string input in tableData[i])
				{
					if (i == 0)
					{
						stringBuilder.AppendFormat("<th>{0}</th>", AntiXssEncoder.HtmlEncode(input, false));
					}
					else
					{
						stringBuilder.AppendFormat("<td>{0}</td>", AntiXssEncoder.HtmlEncode(input, false));
					}
				}
				stringBuilder.Append("</tr>");
			}
			stringBuilder.Append("</table>");
			return stringBuilder.ToString();
		}

		private string GetRelatedResponderStatus(DateTime startTime)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ResponderDefinition[] relatedResponderDefinition = this.GetRelatedResponderDefinition();
			if (relatedResponderDefinition.Length <= 0)
			{
				stringBuilder.Append("Related Responder Status : None <br/>");
				return stringBuilder.ToString();
			}
			stringBuilder.Append("Related Responder Status :");
			stringBuilder.Append("<br/>");
			stringBuilder.Append("<table border=\"1\" style=\"padding: 6px;border-collapse:collapse;font-size:13px;font-family:consolas;\">");
			stringBuilder.Append("<tr style=\"background: #D6EDFF;\"><th>Responder Name</th><th>Status</th></tr>");
			ResponderDefinition[] array = relatedResponderDefinition;
			for (int i = 0; i < array.Length; i++)
			{
				ResponderDefinition responderDefinition = array[i];
				IDataAccessQuery<ResponderResult> responderResults = base.Broker.GetResponderResults(responderDefinition, startTime);
				ResponderResult passedResult = null;
				ResponderResult failedResult = null;
				responderResults.ExecuteAsync(delegate(ResponderResult rr)
				{
					if (rr.IsRecoveryAttempted)
					{
						if (rr.RecoveryResult == ServiceRecoveryResult.Succeeded && passedResult == null)
						{
							passedResult = rr;
						}
						if (rr.RecoveryResult == ServiceRecoveryResult.Failed && failedResult == null)
						{
							failedResult = rr;
						}
					}
				}, default(CancellationToken), base.TraceContext).Wait();
				if (passedResult != null)
				{
					stringBuilder.AppendFormat("<tr style=\"background: #00FF00;\"><td>{0}</td><td>Passed at {1}</td></tr>", responderDefinition.Name, passedResult.ExecutionStartTime);
				}
				else if (failedResult != null)
				{
					stringBuilder.AppendFormat("<tr style=\"background: #FF0000;\"><td>{0}</td><td>Failed. Last failure : {1}</td></tr>", responderDefinition.Name, failedResult.Error);
				}
				else
				{
					stringBuilder.AppendFormat("<tr style=\"background: #E3E3E3;\"><td>{0}</td><td>{1}</td></tr>", responderDefinition.Name, "No passed or failed recovery result is found");
				}
			}
			stringBuilder.Append("</table>");
			return stringBuilder.ToString();
		}

		private DataAccess CreateDataAccessObject()
		{
			Type type = base.Broker.GetType();
			if (type.IsGenericType)
			{
				Type[] genericArguments = type.GetGenericArguments();
				if (genericArguments.Length > 0 && typeof(DataAccess).IsAssignableFrom(genericArguments[0]))
				{
					ConstructorInfo constructor = genericArguments[0].GetConstructor(new Type[0]);
					return (DataAccess)constructor.Invoke(new object[0]);
				}
			}
			return null;
		}

		private ResponderDefinition[] GetRelatedResponderDefinition()
		{
			List<ResponderDefinition> relatedDefinitions = new List<ResponderDefinition>();
			DataAccess dataAccess = this.CreateDataAccessObject();
			if (dataAccess != null)
			{
				IEnumerable<ResponderDefinition> query = from d in dataAccess.GetTable<ResponderDefinition, DateTime>(WorkDefinitionIndex<ResponderDefinition>.StartTime(DateTime.MaxValue))
				where d.DeploymentId == Settings.DeploymentId
				select d;
				dataAccess.AsDataAccessQuery<ResponderDefinition>(query).ExecuteAsync(delegate(ResponderDefinition responderDefinition)
				{
					if (this.Definition.AlertMask == responderDefinition.AlertMask && this.Definition.Name != responderDefinition.Name)
					{
						relatedDefinitions.Add(responderDefinition);
					}
				}, default(CancellationToken), base.TraceContext).Wait();
			}
			return relatedDefinitions.ToArray();
		}

		private static FailureCategoryList LoadFailureCategory()
		{
			try
			{
				string filePath = Path.Combine(Path.GetDirectoryName(typeof(RpsEscalateResponder).Assembly.Location), "RpsFailureCategory.xml");
				return FailureCategoryList.LoadFrom(filePath);
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceError<Exception>(ExTraceGlobals.RPSTracer, new TracingContext(), "Failed to load FailureCategoryList, Error={0}", arg, null, "LoadFailureCategory", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSEscalateResponder.cs", 539);
			}
			return null;
		}

		private static string GetFailureCategoryInfomation(IGrouping<string, ProbeResult>[] groupingResults)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (RpsEscalateResponder.FailureCategoryList == null)
			{
				stringBuilder.Append("Failed to load failure category information. <br/>");
				return stringBuilder.ToString();
			}
			if (groupingResults.Count<IGrouping<string, ProbeResult>>() <= 0)
			{
				return string.Empty;
			}
			stringBuilder.Append("Load failure category information.<br/>");
			stringBuilder.Append("<table border=\"1\" style=\"padding: 6px;border-collapse:collapse;font-size:13px;font-family:consolas;\">");
			for (int i = 0; i < groupingResults.Length; i++)
			{
				IGrouping<string, ProbeResult> groupingResult = groupingResults[i];
				stringBuilder.AppendFormat("<tr><td colspan=2 style=\"background: #D6EDFF;\">{0} ({1})</td></tr>", groupingResult.Key, groupingResult.Count<ProbeResult>());
				FailureCategory failureCategory = (from fc in RpsEscalateResponder.FailureCategoryList.FailureCategories
				where fc.IsMatch(groupingResult.Key)
				select fc).FirstOrDefault<FailureCategory>();
				if (failureCategory != null)
				{
					stringBuilder.AppendFormat("<tr><td/><td>{0}</td></tr>", failureCategory.Description);
					int num = 1;
					foreach (FailureInstance failureInstance in failureCategory.Instances)
					{
						stringBuilder.AppendFormat("<tr style=\"background: #e7e7e7;\"><td style=\"column-width:40px;\">{0}</td><td>Cause: {1}</td>", num++, failureInstance.Cause);
						stringBuilder.AppendFormat("<tr><td/><td>Resolution: {0}</td>", failureInstance.Resolution);
					}
				}
				else
				{
					stringBuilder.Append("<tr><td/><td>cannot find current failure category in RPSFailureCategory.xml</td></tr>");
				}
			}
			stringBuilder.Append("</table>");
			stringBuilder.Append("You can find latest RPS failure category information at [Utah]\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSFailureCategory.xml<br/>");
			return stringBuilder.ToString();
		}

		private static string GenerateImageLink(string failureCategory)
		{
			if (string.IsNullOrEmpty(failureCategory))
			{
				return string.Empty;
			}
			string arg;
			if (failureCategory.Equals("DatabaseServerRoutingError"))
			{
				arg = "DatabaseServerRoutingError";
			}
			else if (Regex.IsMatch(failureCategory, "ADServerSettingsChangedException|ADTransientException|ADExternalException|DirectoryOperationException"))
			{
				arg = "AD";
			}
			else if (failureCategory.Contains('-'))
			{
				arg = failureCategory.Remove(failureCategory.IndexOf('-'));
			}
			else
			{
				arg = "Default";
			}
			return string.Format("<img src=\"\\\\modcfs01\\CSM\\Users\\AlertChart\\{0}.png\" alt=\"{0}.png FailureCategory:{1}\" />", arg, failureCategory);
		}

		private static FailureCategoryList failureCategoryList;
	}
}
