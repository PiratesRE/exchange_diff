using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text;
using System.Threading;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class RusEnginePublishingFailureProbe : RusPublishingPipelineBase
	{
		private TimeSpan AcceptedDelayForPublishingToAkamai { get; set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.TraceDebug("RusEnginePublishingFailureProbe started.");
			base.RusEngineName = base.GetExtensionAttributeStringFromXml(base.Definition.ExtensionAttributes, "//RusEnginePublishingFailureProbeParam", "EngineName", true);
			base.Platforms = base.GetExtensionAttributeStringFromXml(base.Definition.ExtensionAttributes, "//RusEnginePublishingFailureProbeParam", "Platforms", true).Split(new char[]
			{
				','
			});
			this.AcceptedDelayForPublishingToAkamai = base.GetTimeSpanExtensionAttributeFromXml(base.Definition.ExtensionAttributes, "//RusEnginePublishingFailureProbeParam", "AcceptedDelayForPublishingToAkamai", RusEnginePublishingFailureProbe.defaultAllowedPublishingDelay, RusEnginePublishingFailureProbe.minimumAllowedPublishingDelay, RusEnginePublishingFailureProbe.maximumAllowedPublishingDelay);
			base.TraceDebug(string.Format("[AcceptedDelayForPublishingToAkamai: {0}, EngineName: {1}, Platforms: {2}]", this.AcceptedDelayForPublishingToAkamai, base.RusEngineName, string.Join(",", base.Platforms)));
			bool flag = false;
			List<string> list = new List<string>();
			foreach (string text in base.Platforms)
			{
				base.RusEngine = new RusEngine(base.RusEngineName, text);
				DateTime engineFilesDownloadedTimeFromQds = base.RusEngine.GetEngineFilesDownloadedTimeFromQds(base.RusPrimaryFileShareRootPath, base.RusAlternateFileShareRootPath, true);
				if (DateTime.UtcNow - engineFilesDownloadedTimeFromQds < this.AcceptedDelayForPublishingToAkamai)
				{
					engineFilesDownloadedTimeFromQds = base.RusEngine.GetEngineFilesDownloadedTimeFromQds(base.RusPrimaryFileShareRootPath, base.RusAlternateFileShareRootPath, false);
				}
				base.TraceDebug(string.Format("[{0} platform most recent downloaded time: {1}]", text, engineFilesDownloadedTimeFromQds));
				DateTime forefrontdlManifestCreatedTimeInUtc = base.RusEngine.ForefrontdlManifestCreatedTimeInUtc;
				base.TraceDebug(string.Format("[{0} platform manifest published time to Forefrontdl: {1}]", text, forefrontdlManifestCreatedTimeInUtc));
				if (forefrontdlManifestCreatedTimeInUtc < engineFilesDownloadedTimeFromQds)
				{
					flag = true;
					list.Add(text);
				}
			}
			base.TraceDebug(string.Format("[Is any Engine platform OutOfSync: {0}]", flag));
			if (flag)
			{
				string value = string.Format("{0} engines are out of sync in Akamai for the following platforms: {1} \n \n", base.RusEngineName, string.Join(",", list.ToArray()));
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(value);
				foreach (string text2 in list)
				{
					List<string> asyncQueueRequestResults = this.GetAsyncQueueRequestResults(3, base.RusEngineName, text2);
					string value2 = string.Format("Following are list of AsyncQueueRequests for {0}:{1} engine in last {2} hours. \n", base.RusEngineName, text2, 3);
					stringBuilder.Append(value2);
					foreach (string text3 in asyncQueueRequestResults)
					{
						stringBuilder.Append(text3 + "\n");
						string requestId = text3.Substring(text3.IndexOf("RequestId:") + 10, 36);
						Dictionary<string, StringBuilder> asyncQueueStepResults = this.GetAsyncQueueStepResults(requestId);
						if (asyncQueueStepResults != null && asyncQueueStepResults.Count > 0)
						{
							foreach (string text4 in asyncQueueStepResults.Keys)
							{
								stringBuilder.Append(text4 + "\n");
								stringBuilder.Append(asyncQueueStepResults[text4].ToString() + "\n");
							}
						}
					}
				}
				base.LogTraceErrorAndThrowApplicationException(stringBuilder.ToString());
			}
			base.TraceDebug("RusEnginePublishingFailureProbe finished with success.");
		}

		private Dictionary<string, StringBuilder> GetAsyncQueueStepResults(string requestId)
		{
			Dictionary<string, StringBuilder> dictionary = null;
			string psScript = string.Format("({0} -Owner {1} | where {2}$_.RequestId -eq '{3}'{4}).Steps", new object[]
			{
				"Get-AsyncQueueRequest",
				"RusPipelineJob",
				"{",
				requestId,
				"}"
			});
			Collection<PSObject> collection = base.ExecuteForeFrontManagementShellScript(psScript, false);
			if (collection != null && collection.Count > 0)
			{
				dictionary = new Dictionary<string, StringBuilder>();
				foreach (PSObject psobject in collection)
				{
					string text = Convert.ToString(psobject.Properties["RequestStepId"].Value);
					string text2 = Convert.ToString(psobject.Properties["OrganizationalUnitRoot"].Value);
					string text3 = Convert.ToString(psobject.Properties["StepStatus"].Value);
					string text4 = Convert.ToString(psobject.Properties["StepName"].Value);
					string text5 = Convert.ToString(psobject.Properties["StepNumber"].Value);
					string text6 = Convert.ToString(psobject.Properties["ProcessInstanceName"].Value);
					string key = string.Format("StepNumber:{0}, StepName:{1}, StepStatus:{2}, ProcessInstanceName:{3}", new object[]
					{
						text5,
						text4,
						text3,
						text6
					});
					StringBuilder stringBuilder = new StringBuilder();
					if (!text3.Equals("Completed", StringComparison.OrdinalIgnoreCase))
					{
						psScript = string.Format("{0} -RequestId {1} -OrganizationalUnitRoot {2} -RequestStepId {3}", new object[]
						{
							"Get-AsyncQueueLog",
							requestId,
							text2,
							text
						});
						Collection<PSObject> collection2 = base.ExecuteForeFrontManagementShellScript(psScript, false);
						if (collection2 != null && collection2.Count > 0)
						{
							foreach (PSObject psobject2 in collection2)
							{
								string arg = Convert.ToString(psobject2.Properties["LogTime"].Value);
								string arg2 = Convert.ToString(psobject2.Properties["LogType"].Value);
								string arg3 = Convert.ToString(psobject2.Properties["LogData"].Value);
								string str = string.Format("LogTime:{0}, LogType:{1}, LogData:{2}", arg, arg2, arg3);
								stringBuilder.Append(str + "\n");
							}
						}
					}
					dictionary.Add(key, stringBuilder);
				}
			}
			return dictionary;
		}

		private List<string> GetAsyncQueueRequestResults(int lookBackTimeInHours, string engineName, string platfrom)
		{
			List<string> list = null;
			string psScript = string.Format("{0} -Owner {1} | where {2}$_.Cookie -like 'Engine:{3},Platform:{4}*' -and $_.CreationTime -gt [DateTime]::UtcNow.AddHours(-{5}){6}", new object[]
			{
				"Get-AsyncQueueRequest",
				"RusPipelineJob",
				"{",
				engineName,
				platfrom,
				lookBackTimeInHours,
				"}"
			});
			Collection<PSObject> collection = base.ExecuteForeFrontManagementShellScript(psScript, false);
			if (collection != null && collection.Count > 0)
			{
				list = new List<string>();
				using (IEnumerator<PSObject> enumerator = collection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PSObject psobject = enumerator.Current;
						string item = string.Format("RequestId:{0}, Cookie:{1}, CreationTime:{2}, LastModifiedTime:{3}, RequestStatus:{4}", new object[]
						{
							Convert.ToString(psobject.Properties["RequestId"].Value),
							Convert.ToString(psobject.Properties["Cookie"].Value),
							Convert.ToString(psobject.Properties["CreationTime"].Value),
							Convert.ToString(psobject.Properties["LastModifiedTime"].Value),
							Convert.ToString(psobject.Properties["RequestStatus"].Value)
						});
						list.Add(item);
					}
					return list;
				}
			}
			string errorMsg = string.Format("'{0}' cmdlet for AsyncQueueRequest with owner: {1} returned null or empty results in last {2} hours", "Get-AsyncQueueRequest", "RusPipelineJob", lookBackTimeInHours);
			base.LogTraceErrorAndThrowApplicationException(errorMsg);
			return list;
		}

		private const string ProbeParamXmlNode = "//RusEnginePublishingFailureProbeParam";

		private const string AcceptedDelayForPublishingToAkamaiXmlAttribute = "AcceptedDelayForPublishingToAkamai";

		private const int DefaultLookBackHours = 3;

		private static TimeSpan defaultAllowedPublishingDelay = TimeSpan.FromMinutes(45.0);

		private static TimeSpan minimumAllowedPublishingDelay = TimeSpan.FromMinutes(5.0);

		private static TimeSpan maximumAllowedPublishingDelay = TimeSpan.FromHours(3.0);
	}
}
