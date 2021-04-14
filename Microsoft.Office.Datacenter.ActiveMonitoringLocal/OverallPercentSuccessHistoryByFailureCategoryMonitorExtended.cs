using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class OverallPercentSuccessHistoryByFailureCategoryMonitorExtendedOutput : OverallPercentSuccessHistoryByFailureCategoryMonitor
	{
		public new static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, double availabilityPercentage, TimeSpan monitoringInterval, TimeSpan recurrenceInterval, TimeSpan secondaryMonitoringInterval, int failureCategoryMask = -1, bool enabled = true)
		{
			MonitorDefinition monitorDefinition = OverallPercentSuccessHistoryByFailureCategoryMonitor.CreateDefinition(name, sampleMask, serviceName, component, availabilityPercentage, monitoringInterval, recurrenceInterval, secondaryMonitoringInterval, failureCategoryMask, enabled);
			monitorDefinition.AssemblyPath = OverallPercentSuccessHistoryByFailureCategoryMonitorExtendedOutput.AssemblyPath;
			monitorDefinition.TypeName = OverallPercentSuccessHistoryByFailureCategoryMonitorExtendedOutput.TypeName;
			return monitorDefinition;
		}

		internal override Task SetProbeFailureCategoryNumbers(CancellationToken cancellationToken)
		{
			Task<Dictionary<int, int>> failureCategoryCountsForAllFailedProbeResults = base.GetFailureCategoryCountsForAllFailedProbeResults(base.Definition.SampleMask, cancellationToken);
			return failureCategoryCountsForAllFailedProbeResults.Continue(delegate(Dictionary<int, int> failureCategoryCounts)
			{
				int totalFailureCategoryValue;
				int totalFailureCategoryCount;
				double totalFailureCategoryPercent;
				base.GetProbeFailureCategoryStatistics(failureCategoryCounts, out totalFailureCategoryValue, out totalFailureCategoryCount, out totalFailureCategoryPercent);
				base.Result.TotalFailureCategoryValue = totalFailureCategoryValue;
				base.Result.TotalFailureCategoryCount = totalFailureCategoryCount;
				base.Result.TotalFailureCategoryPercent = totalFailureCategoryPercent;
				WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallPercentSuccessHistoryByFailureCategoryMonitor: Processed total result(s), total max caluculated attribute is {0}.", base.Result.TotalFailureCategoryValue, null, "SetProbeFailureCategoryNumbers", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessHistoryByFailureCategoryMonitorExtendedOutput.cs", 165);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		protected override Task SetPercentSuccessNumbers(CancellationToken cancellationToken)
		{
			Task<Dictionary<ResultType, int>> resultTypeCountsForAllProbeResults = base.GetResultTypeCountsForAllProbeResults(base.Definition.SampleMask, false, cancellationToken);
			return resultTypeCountsForAllProbeResults.Continue(delegate(Dictionary<ResultType, int> resultTypeCounts)
			{
				int totalSampleCount;
				int totalFailedCount;
				double totalValue;
				this.GetResultStatistics(resultTypeCounts, out totalSampleCount, out totalFailedCount, out totalValue);
				base.Result.TotalSampleCount = totalSampleCount;
				base.Result.TotalFailedCount = totalFailedCount;
				base.Result.TotalValue = totalValue;
				WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallPercentSuccessMonitor: Processed {0} total result(s).", base.Result.TotalSampleCount, null, "SetPercentSuccessNumbers", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessHistoryByFailureCategoryMonitorExtendedOutput.cs", 201);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		internal override Task<bool> AlertBasedOnResultHistory(CancellationToken cancellationToken)
		{
			bool isAlert = true;
			int monitorCounter = 0;
			DateTime startTime = base.Result.ExecutionStartTime.AddSeconds(-base.Definition.SecondaryMonitoringThreshold);
			base.Result.StateAttribute5 = startTime.ToString();
			this.CalculateFinalFailureResult(base.Result.StateAttribute1);
			IOrderedEnumerable<MonitorResult> query = from r in base.Broker.GetSuccessfulMonitorResults(base.Definition, startTime)
			where r.ExecutionStartTime < base.Result.ExecutionStartTime
			orderby r.ExecutionEndTime descending
			select r;
			DateTime firstMonitorTime = DateTime.MaxValue;
			Task<int> task = base.Broker.AsDataAccessQuery<MonitorResult>(query).ExecuteAsync(delegate(MonitorResult result)
			{
				if (result.ExecutionStartTime.AddTicks(-(result.ExecutionStartTime.Ticks % 10000000L)) > startTime)
				{
					monitorCounter++;
					this.CalculateFinalFailureResult(result.StateAttribute1);
					if (result.TotalSampleCount > 0)
					{
						if (result.TotalValue >= this.Definition.MonitoringThreshold)
						{
							this.Result.StateAttribute6 = 1.0;
							isAlert = false;
						}
						else if (this.Definition.FailureCategoryMask >= 0 && result.TotalFailureCategoryValue != this.Definition.FailureCategoryMask)
						{
							this.Result.StateAttribute6 = 4.0;
							isAlert = false;
						}
						if (result.ExecutionStartTime < firstMonitorTime)
						{
							firstMonitorTime = result.ExecutionStartTime;
						}
					}
				}
			}, cancellationToken, base.TraceContext);
			return task.Continue(delegate(Task<int> t)
			{
				if (isAlert && ((double)monitorCounter < this.Definition.SecondaryMonitoringThreshold / (double)this.Definition.RecurrenceIntervalSeconds - 1.0 || (this.Result.ExecutionStartTime - firstMonitorTime).TotalSeconds < this.Definition.SecondaryMonitoringThreshold - (double)this.Definition.RecurrenceIntervalSeconds))
				{
					this.Result.StateAttribute6 = 2.0;
					isAlert = false;
				}
				return isAlert;
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			Task task = Task.Factory.StartNew(delegate()
			{
				Task task2 = Task.Factory.StartNew(delegate()
				{
					if (this.errorOutput == null)
					{
						this.allProbeResults = new List<ProbeResult>();
						this.errorOutput = new XmlDocument();
						XmlNode newChild = this.errorOutput.CreateElement("Failures");
						this.errorOutput.AppendChild(newChild);
						this.QueryForAllProbeResultsInWindow(cancellationToken);
					}
				}, cancellationToken);
				Task task3 = task2.ContinueWith(delegate(Task result)
				{
					this.SetPercentSuccessNumbers(cancellationToken);
					if (this.Definition.FailureCategoryMask >= 0)
					{
						this.SetProbeFailureCategoryNumbers(cancellationToken);
					}
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
				task3.ContinueWith(delegate(Task result)
				{
					if (this.errorOutput != null)
					{
						if (this.failureReasonsNode != null)
						{
							XmlNode xmlNode = this.errorOutput.SelectSingleNode("//Failures");
							xmlNode.AppendChild(this.failureReasonsNode);
						}
						using (StringWriter stringWriter = new StringWriter())
						{
							using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
							{
								this.errorOutput.WriteTo(xmlWriter);
								xmlWriter.Close();
								this.Result.StateAttribute1 = stringWriter.GetStringBuilder().ToString();
							}
						}
					}
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "OverallPercentSuccessHistoryByFailureCategoryMonitorExtendedOutputMonitor: Finished collecting result data.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessHistoryByFailureCategoryMonitorExtendedOutput.cs", 378);
			}, cancellationToken, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
			task.ContinueWith(delegate(Task t)
			{
				if (this.Result.TotalValue < this.Definition.MonitoringThreshold)
				{
					Task<bool> task2 = this.AlertBasedOnResultHistory(cancellationToken);
					task2.Continue(delegate(bool alertTask)
					{
						if (alertTask)
						{
							if (this.Definition.FailureCategoryMask >= 0)
							{
								if (this.Result.TotalFailureCategoryValue == this.Definition.FailureCategoryMask)
								{
									this.Result.IsAlert = true;
									this.Result.StateAttribute2 = this.OutputFailuresInHtmlFormat();
								}
								else
								{
									this.Result.IsAlert = false;
								}
								if (!this.Result.IsAlert)
								{
									this.Result.StateAttribute6 = 3.0;
									return;
								}
							}
							else
							{
								this.Result.IsAlert = true;
								this.Result.StateAttribute2 = this.OutputFailuresInHtmlFormat();
							}
						}
					}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
				}
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "OverallPercentSuccessHistoryByFailureCategoryMonitorExtendedOutputMonitor: Finished analyzing probe results.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessHistoryByFailureCategoryMonitorExtendedOutput.cs", 430);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
		}

		internal Task<int> QueryForAllProbeResultsInWindow(CancellationToken cancellationToken)
		{
			DateTime startTime = base.Result.ExecutionStartTime - TimeSpan.FromSeconds((double)base.Definition.MonitoringIntervalSeconds);
			IDataAccessQuery<ProbeResult> probeResults = base.Broker.GetProbeResults(base.Definition.SampleMask, startTime, base.Result.ExecutionStartTime);
			IEnumerable<ProbeResult> query = from r in probeResults
			select r;
			return base.Broker.AsDataAccessQuery<ProbeResult>(query).ExecuteAsync(delegate(ProbeResult result)
			{
				this.allProbeResults.Add(result);
			}, cancellationToken, base.TraceContext);
		}

		internal override Task<Dictionary<ResultType, int>> GetResultTypeCountsForProbeResults(string sampleMask, DateTime startTime, DateTime endTime, bool consolidateFailureResults, CancellationToken cancellationToken)
		{
			Dictionary<ResultType, int> resultTypeCounts = new Dictionary<ResultType, int>();
			Dictionary<string, int> failureComponentsCount = new Dictionary<string, int>();
			Task<object> task = Task.Factory.StartNew<object>(delegate(object result)
			{
				XmlNode xmlNode = this.errorOutput.SelectSingleNode("//Failures");
				XmlNode xmlNode2 = this.errorOutput.CreateElement("TotalProbes");
				XmlAttribute xmlAttribute = this.errorOutput.CreateAttribute("count");
				xmlAttribute.Value = this.allProbeResults.Count.ToString();
				xmlNode2.Attributes.Append(xmlAttribute);
				xmlNode.AppendChild(xmlNode2);
				foreach (ProbeResult probeResult in this.allProbeResults)
				{
					Dictionary<ResultType, int> resultTypeCounts;
					if (consolidateFailureResults && MonitorWorkItem.ShouldConsiderFailed(probeResult.ResultType))
					{
						if (resultTypeCounts.ContainsKey(ResultType.Failed))
						{
							(resultTypeCounts = resultTypeCounts)[ResultType.Failed] = resultTypeCounts[ResultType.Failed] + 1;
						}
						else
						{
							resultTypeCounts.Add(ResultType.Failed, 1);
						}
					}
					else if (resultTypeCounts.ContainsKey(probeResult.ResultType))
					{
						Dictionary<ResultType, int> resultTypeCounts2;
						ResultType resultType;
						(resultTypeCounts2 = resultTypeCounts)[resultType = probeResult.ResultType] = resultTypeCounts2[resultType] + 1;
					}
					else
					{
						resultTypeCounts.Add(probeResult.ResultType, 1);
					}
					if (probeResult.FailureCategory != this.Definition.FailureCategoryMask && probeResult.ResultType == ResultType.Failed)
					{
						Dictionary<string, int> failureComponentsCount;
						if (failureComponentsCount.ContainsKey(probeResult.StateAttribute1))
						{
							string stateAttribute;
							(failureComponentsCount = failureComponentsCount)[stateAttribute = probeResult.StateAttribute1] = failureComponentsCount[stateAttribute] + 1;
						}
						else
						{
							failureComponentsCount.Add(probeResult.StateAttribute1, 1);
						}
					}
				}
				return result;
			}, cancellationToken);
			return task.Continue(delegate(Task<object> totalResultCount)
			{
				XmlNode xmlNode = this.errorOutput.SelectSingleNode("//Failures");
				XmlNode xmlNode2 = this.GenerateProbeCountOutput(failureComponentsCount, false);
				if (xmlNode2 != null)
				{
					xmlNode.AppendChild(xmlNode2);
				}
				return resultTypeCounts;
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		internal override Task<Dictionary<int, int>> GetFailureCategoryCountsForFailedProbeResults(string sampleMask, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
		{
			Dictionary<int, int> attributeCounts = new Dictionary<int, int>();
			Dictionary<string, int> failureReasonCounts = new Dictionary<string, int>();
			Task<object> task = Task.Factory.StartNew<object>(delegate(object result)
			{
				foreach (ProbeResult probeResult in this.allProbeResults)
				{
					if (probeResult.ResultType == ResultType.Failed)
					{
						if (probeResult.FailureCategory == this.Definition.FailureCategoryMask)
						{
							Dictionary<string, int> failureReasonCounts;
							if (failureReasonCounts.ContainsKey(probeResult.StateAttribute2))
							{
								string stateAttribute;
								(failureReasonCounts = failureReasonCounts)[stateAttribute = probeResult.StateAttribute2] = failureReasonCounts[stateAttribute] + 1;
							}
							else
							{
								failureReasonCounts.Add(probeResult.StateAttribute2, 1);
							}
						}
						Dictionary<int, int> attributeCounts;
						if (attributeCounts.ContainsKey(probeResult.FailureCategory))
						{
							int failureCategory;
							(attributeCounts = attributeCounts)[failureCategory = probeResult.FailureCategory] = attributeCounts[failureCategory] + 1;
						}
						else
						{
							attributeCounts.Add(probeResult.FailureCategory, 1);
						}
					}
				}
				return result;
			}, cancellationToken);
			return task.Continue(delegate(Task<object> totalAttributeCount)
			{
				this.failureReasonsNode = this.GenerateProbeCountOutput(failureReasonCounts, true);
				return attributeCounts;
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		internal XmlNode GenerateProbeCountOutput(Dictionary<string, int> failureResults, bool isFailureReasons)
		{
			IEnumerable<KeyValuePair<string, int>> enumerable = (from pair in failureResults
			orderby pair.Value descending
			select pair).Take(5);
			XmlNode xmlNode;
			if (isFailureReasons)
			{
				xmlNode = this.errorOutput.CreateElement("FailureReasons");
				using (IEnumerator<KeyValuePair<string, int>> enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, int> keyValuePair = enumerator.Current;
						XmlNode xmlNode2 = this.errorOutput.CreateElement(keyValuePair.Key);
						XmlAttribute xmlAttribute = this.errorOutput.CreateAttribute("count");
						xmlAttribute.Value = keyValuePair.Value.ToString();
						xmlNode2.Attributes.Append(xmlAttribute);
						xmlNode.AppendChild(xmlNode2);
					}
					return xmlNode;
				}
			}
			xmlNode = this.errorOutput.CreateElement("FailureComponents");
			foreach (KeyValuePair<string, int> keyValuePair2 in enumerable)
			{
				XmlNode xmlNode3 = this.errorOutput.CreateElement(keyValuePair2.Key);
				XmlAttribute xmlAttribute2 = this.errorOutput.CreateAttribute("count");
				xmlAttribute2.Value = keyValuePair2.Value.ToString();
				xmlNode3.Attributes.Append(xmlAttribute2);
				xmlNode.AppendChild(xmlNode3);
			}
			return xmlNode;
		}

		internal void CalculateFinalFailureResult(string xmlDocString)
		{
			if (this.finalFailureReasons == null)
			{
				this.finalFailureReasons = new Dictionary<string, int>();
			}
			if (this.finalFailureComponents == null)
			{
				this.finalFailureComponents = new Dictionary<string, int>();
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlDocString);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//Failures/TotalProbes");
			this.totalProbeExecutions += int.Parse(xmlNode.Attributes.GetNamedItem("count").Value);
			XmlNode xmlNode2 = xmlDocument.SelectSingleNode(string.Format("//Failures/{0}", "FailureReasons"));
			if (xmlNode2 != null)
			{
				XmlNodeList childNodes = xmlNode2.ChildNodes;
				foreach (object obj in childNodes)
				{
					XmlNode xmlNode3 = (XmlNode)obj;
					if (this.finalFailureReasons.ContainsKey(xmlNode3.Name))
					{
						Dictionary<string, int> dictionary;
						string name;
						(dictionary = this.finalFailureReasons)[name = xmlNode3.Name] = dictionary[name] + int.Parse(xmlNode3.Attributes.GetNamedItem("count").Value);
					}
					else
					{
						this.finalFailureReasons.Add(xmlNode3.Name, int.Parse(xmlNode3.Attributes.GetNamedItem("count").Value));
					}
				}
			}
			XmlNode xmlNode4 = xmlDocument.SelectSingleNode(string.Format("//Failures/{0}", "FailureComponents"));
			if (xmlNode4 != null)
			{
				XmlNodeList childNodes2 = xmlNode4.ChildNodes;
				foreach (object obj2 in childNodes2)
				{
					XmlNode xmlNode5 = (XmlNode)obj2;
					if (this.finalFailureComponents.ContainsKey(xmlNode5.Name))
					{
						Dictionary<string, int> dictionary2;
						string name2;
						(dictionary2 = this.finalFailureComponents)[name2 = xmlNode5.Name] = dictionary2[name2] + int.Parse(xmlNode5.Attributes.GetNamedItem("count").Value);
					}
					else
					{
						this.finalFailureComponents.Add(xmlNode5.Name, int.Parse(xmlNode5.Attributes.GetNamedItem("count").Value));
					}
				}
			}
		}

		private string OutputFailuresInHtmlFormat()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.totalProbeExecutions > 0)
			{
				stringBuilder.Append(string.Format("Total probes: {0}<br><br>", this.totalProbeExecutions.ToString()));
			}
			if (this.finalFailureReasons.Count > 0)
			{
				stringBuilder.Append("Failure Reasons");
				IEnumerable<KeyValuePair<string, int>> enumerable = (from pair in this.finalFailureReasons
				orderby pair.Value descending
				select pair).Take(5);
				foreach (KeyValuePair<string, int> keyValuePair in enumerable)
				{
					stringBuilder.Append(string.Format("<li>{0}&nbsp{1}", keyValuePair.Key, keyValuePair.Value.ToString()));
				}
				stringBuilder.Append("<br><br>");
			}
			if (this.finalFailureComponents.Count > 0)
			{
				stringBuilder.Append("Other Component Failures");
				IEnumerable<KeyValuePair<string, int>> enumerable2 = (from pair in this.finalFailureComponents
				orderby pair.Value descending
				select pair).Take(5);
				foreach (KeyValuePair<string, int> keyValuePair2 in enumerable2)
				{
					stringBuilder.Append(string.Format("<li>{0}&nbsp{1}", keyValuePair2.Key, keyValuePair2.Value.ToString()));
				}
			}
			return stringBuilder.ToString();
		}

		private const string FailureReasons = "FailureReasons";

		private const string FailureComponents = "FailureComponents";

		private const string TotalProbeExecutions = "TotalProbes";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OverallPercentSuccessHistoryByFailureCategoryMonitorExtendedOutput).FullName;

		private XmlDocument errorOutput;

		private List<ProbeResult> allProbeResults;

		private Dictionary<string, int> finalFailureReasons;

		private Dictionary<string, int> finalFailureComponents;

		private XmlNode failureReasonsNode;

		private int totalProbeExecutions;
	}
}
