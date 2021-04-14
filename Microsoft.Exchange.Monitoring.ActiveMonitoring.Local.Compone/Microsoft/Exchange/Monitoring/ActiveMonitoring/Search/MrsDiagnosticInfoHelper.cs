using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.ApplicationLogic;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search
{
	public class MrsDiagnosticInfoHelper
	{
		static MrsDiagnosticInfoHelper()
		{
			Array values = Enum.GetValues(typeof(MrsDiagnosticInfoHelper.Argument));
			foreach (object obj in values)
			{
				int num = (int)obj;
				MrsDiagnosticInfoHelper.Argument key = (MrsDiagnosticInfoHelper.Argument)num;
				MrsDiagnosticInfoHelper.diagnosticInfoCacheLocks.Add(key, new object());
				MrsDiagnosticInfoHelper.diagnosticInfoCacheTimeoutTime.Add(key, DateTime.MinValue);
			}
			MrsDiagnosticInfoHelper.argumentComponentDictionary.Add(MrsDiagnosticInfoHelper.Argument.Job, "MailboxReplicationService");
			MrsDiagnosticInfoHelper.argumentComponentDictionary.Add(MrsDiagnosticInfoHelper.Argument.Resource, "SystemWorkLoadManager");
			MrsDiagnosticInfoHelper.argumentComponentDictionary.Add(MrsDiagnosticInfoHelper.Argument.History, "SystemWorkLoadManager");
		}

		internal static XmlDocument GetDiagnosticInfoWithCaching(MrsDiagnosticInfoHelper.Argument argument)
		{
			if (!MrsDiagnosticInfoHelper.diagnosticInfoCache.ContainsKey(argument) || DateTime.UtcNow > MrsDiagnosticInfoHelper.diagnosticInfoCacheTimeoutTime[argument])
			{
				lock (MrsDiagnosticInfoHelper.diagnosticInfoCacheLocks[argument])
				{
					if (!MrsDiagnosticInfoHelper.diagnosticInfoCache.ContainsKey(argument) || DateTime.UtcNow > MrsDiagnosticInfoHelper.diagnosticInfoCacheTimeoutTime[argument])
					{
						string diagnosticInfoXml = null;
						Exception ex = null;
						Action delegateGetDiagnosticInfo = delegate()
						{
							try
							{
								string componentName = null;
								MrsDiagnosticInfoHelper.argumentComponentDictionary.TryGetValue(argument, out componentName);
								diagnosticInfoXml = ProcessAccessManager.ClientRunProcessCommand(null, "msexchangemailboxreplication", componentName, argument.ToString(), false, true, null);
							}
							catch (Exception ex)
							{
								ex = ex;
							}
						};
						IAsyncResult asyncResult = delegateGetDiagnosticInfo.BeginInvoke(delegate(IAsyncResult r)
						{
							delegateGetDiagnosticInfo.EndInvoke(r);
						}, null);
						if (!asyncResult.AsyncWaitHandle.WaitOne(MrsDiagnosticInfoHelper.GetDiagnosticInfoCallTimeout))
						{
							ex = new TimeoutException();
						}
						if (ex != null)
						{
							MrsDiagnosticInfoHelper.diagnosticInfoCache[argument] = null;
							MrsDiagnosticInfoHelper.diagnosticInfoCacheTimeoutTime[argument] = DateTime.UtcNow + MrsDiagnosticInfoHelper.DiagnosticInfoCacheTimeout;
							throw ex;
						}
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(diagnosticInfoXml);
						MrsDiagnosticInfoHelper.diagnosticInfoCache[argument] = xmlDocument;
						MrsDiagnosticInfoHelper.diagnosticInfoCacheTimeoutTime[argument] = DateTime.UtcNow + MrsDiagnosticInfoHelper.DiagnosticInfoCacheTimeout;
					}
				}
			}
			return MrsDiagnosticInfoHelper.diagnosticInfoCache[argument];
		}

		internal static XmlNodeList GetMoveJobs()
		{
			XmlDocument diagnosticInfoWithCaching = MrsDiagnosticInfoHelper.GetDiagnosticInfoWithCaching(MrsDiagnosticInfoHelper.Argument.Job);
			if (diagnosticInfoWithCaching == null)
			{
				return null;
			}
			return diagnosticInfoWithCaching.SelectNodes("/Diagnostics/Components/MailboxReplicationService/Jobs/Move");
		}

		internal static XmlNode GetResource(string resourceKey, string classification)
		{
			XmlDocument diagnosticInfoWithCaching = MrsDiagnosticInfoHelper.GetDiagnosticInfoWithCaching(MrsDiagnosticInfoHelper.Argument.Resource);
			if (diagnosticInfoWithCaching == null)
			{
				return null;
			}
			string xpath = string.Format("/Diagnostics/Components/SystemWorkloadManager/Resources/Resource[ResourceKey='{0}' and Classification='{1}']", resourceKey, classification);
			return diagnosticInfoWithCaching.SelectSingleNode(xpath);
		}

		internal static XmlNodeList GetHistory(string resourceKey, string classification)
		{
			XmlDocument diagnosticInfoWithCaching = MrsDiagnosticInfoHelper.GetDiagnosticInfoWithCaching(MrsDiagnosticInfoHelper.Argument.History);
			if (diagnosticInfoWithCaching == null)
			{
				return null;
			}
			string xpath = string.Format("/Diagnostics/Components/SystemWorkloadManager/History/Entry[Type='Monitor' and ResourceKey='{0}' and Classification='{1}']", resourceKey, classification);
			return diagnosticInfoWithCaching.SelectNodes(xpath);
		}

		private const string MailboxReplicationServiceProcess = "msexchangemailboxreplication";

		internal static readonly TimeSpan GetDiagnosticInfoCallTimeout = TimeSpan.FromMinutes(2.0);

		private static readonly TimeSpan DiagnosticInfoCacheTimeout = TimeSpan.FromMinutes(5.0);

		private static Dictionary<MrsDiagnosticInfoHelper.Argument, XmlDocument> diagnosticInfoCache = new Dictionary<MrsDiagnosticInfoHelper.Argument, XmlDocument>();

		private static Dictionary<MrsDiagnosticInfoHelper.Argument, DateTime> diagnosticInfoCacheTimeoutTime = new Dictionary<MrsDiagnosticInfoHelper.Argument, DateTime>();

		private static Dictionary<MrsDiagnosticInfoHelper.Argument, object> diagnosticInfoCacheLocks = new Dictionary<MrsDiagnosticInfoHelper.Argument, object>();

		private static Dictionary<MrsDiagnosticInfoHelper.Argument, string> argumentComponentDictionary = new Dictionary<MrsDiagnosticInfoHelper.Argument, string>();

		internal enum Argument
		{
			Resource,
			Job,
			History
		}
	}
}
