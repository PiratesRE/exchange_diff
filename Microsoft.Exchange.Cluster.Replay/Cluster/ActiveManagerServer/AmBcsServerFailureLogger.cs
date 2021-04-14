using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmBcsServerFailureLogger : IAmBcsErrorLogger
	{
		public AmBcsServerFailureLogger(Guid dbGuid, string dbName, bool fLogEvents)
		{
			this.m_dbGuid = dbGuid;
			this.m_dbName = dbName;
			this.m_failureTable = new Dictionary<AmServerName, OrderedDictionary>(5);
			this.IsEventLoggingEnabled = fLogEvents;
		}

		public bool IsEventLoggingEnabled { get; private set; }

		public bool IsFailedForServer(AmServerName server)
		{
			return this.m_failureTable.ContainsKey(server);
		}

		public void ReportCopyStatusFailure(AmServerName server, string statusCheckThatFailed, string checksRun, string errorMessage)
		{
			this.ReportCopyStatusFailure(server, statusCheckThatFailed, checksRun, errorMessage, ReplayCrimsonEvents.BcsDbNodeChecksFailed, new object[]
			{
				this.m_dbName,
				this.m_dbGuid,
				server,
				checksRun,
				errorMessage
			});
		}

		public void ReportCopyStatusFailure(AmServerName server, string statusCheckThatFailed, string checksRun, string errorMessage, ReplayCrimsonEvent evt, params object[] evtArgs)
		{
			AmBcsServerFailureLogger.AmBcsCheckInfo checkInfo = new AmBcsServerFailureLogger.AmBcsCheckInfo(AmBcsServerFailureLogger.AmBcsCheckType.CopyStatus, statusCheckThatFailed);
			this.ReportFailureInternal(server, checkInfo, errorMessage, true, evt, evtArgs);
		}

		public void ReportServerFailure(AmServerName server, string serverCheckThatFailed, string errorMessage)
		{
			this.ReportServerFailure(server, serverCheckThatFailed, errorMessage, true);
		}

		public void ReportServerFailure(AmServerName server, string serverCheckThatFailed, string errorMessage, bool overwriteAllowed)
		{
			AmBcsServerFailureLogger.AmBcsCheckInfo checkInfo = new AmBcsServerFailureLogger.AmBcsCheckInfo(AmBcsServerFailureLogger.AmBcsCheckType.ServerLevel, serverCheckThatFailed);
			this.ReportFailureInternal(server, checkInfo, errorMessage, overwriteAllowed, ReplayCrimsonEvents.BcsDbNodeActivationBlocked, new object[]
			{
				this.m_dbName,
				this.m_dbGuid,
				server,
				errorMessage
			});
		}

		public void ReportServerFailure(AmServerName server, string serverCheckThatFailed, string errorMessage, ReplayCrimsonEvent evt, params object[] evtArgs)
		{
			AmBcsServerFailureLogger.AmBcsCheckInfo checkInfo = new AmBcsServerFailureLogger.AmBcsCheckInfo(AmBcsServerFailureLogger.AmBcsCheckType.ServerLevel, serverCheckThatFailed);
			this.ReportFailureInternal(server, checkInfo, errorMessage, true, evt, evtArgs);
		}

		public string[] GetAllExceptions()
		{
			if (this.m_failureTable.Count == 0)
			{
				return null;
			}
			return this.m_failureTable.Values.SelectMany((OrderedDictionary ordered) => ordered.Values.Cast<string>()).ToArray<string>();
		}

		public Exception GetLastException()
		{
			string concatenatedErrorString = this.GetConcatenatedErrorString();
			if (concatenatedErrorString != null)
			{
				return new AmDbNotMountedMultipleServersException(this.m_dbName, this.GetConcatenatedErrorString());
			}
			return null;
		}

		public string GetConcatenatedErrorString()
		{
			if (this.m_failureTable.Count == 0)
			{
				return null;
			}
			IEnumerable<KeyValuePair<AmServerName, string>> enumerable = from kvp in this.m_failureTable
			where kvp.Value != null
			select new KeyValuePair<AmServerName, string>(kvp.Key, (string)kvp.Value[kvp.Value.Count - 1]);
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.AppendLine();
			foreach (KeyValuePair<AmServerName, string> keyValuePair in enumerable)
			{
				stringBuilder.AppendFormat("\r\n\r\n        {0}:\r\n        {1}\r\n        ", keyValuePair.Key.NetbiosName, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		private void ReportFailureInternal(AmServerName server, AmBcsServerFailureLogger.AmBcsCheckInfo checkInfo, string errorMessage, bool overwriteAllowed, ReplayCrimsonEvent evt, params object[] evtArgs)
		{
			OrderedDictionary orderedDictionary;
			if (this.m_failureTable.ContainsKey(server))
			{
				orderedDictionary = this.m_failureTable[server];
			}
			else
			{
				orderedDictionary = new OrderedDictionary(10);
				this.m_failureTable.Add(server, orderedDictionary);
			}
			string str;
			bool flag = !this.TryGetErrorFromCheckTable(orderedDictionary, checkInfo, out str) || (overwriteAllowed && !SharedHelper.StringIEquals(str, errorMessage));
			if (flag)
			{
				this.AddErrorIntoCheckTable(orderedDictionary, checkInfo, errorMessage);
				if (this.IsEventLoggingEnabled)
				{
					evt.LogGeneric(evtArgs);
					return;
				}
			}
			else
			{
				AmTrace.Debug("BCS: Failure for server '{0}' and check [{1}] has already been recorded. Suppressing raising another event.", new object[]
				{
					server,
					checkInfo
				});
			}
		}

		private bool TryGetErrorFromCheckTable(OrderedDictionary checkTable, AmBcsServerFailureLogger.AmBcsCheckInfo checkInfo, out string errorMessage)
		{
			errorMessage = null;
			bool flag = checkTable.Contains(checkInfo);
			if (flag)
			{
				errorMessage = (string)checkTable[checkInfo];
				return true;
			}
			return false;
		}

		private void AddErrorIntoCheckTable(OrderedDictionary checkTable, AmBcsServerFailureLogger.AmBcsCheckInfo checkInfo, string errorMessage)
		{
			checkTable[checkInfo] = errorMessage;
		}

		public const string ServerErrorFormatStr = "\r\n\r\n        {0}:\r\n        {1}\r\n        ";

		private Dictionary<AmServerName, OrderedDictionary> m_failureTable;

		private Guid m_dbGuid;

		private string m_dbName;

		private enum AmBcsCheckType
		{
			CopyStatus,
			ServerLevel
		}

		private class AmBcsCheckInfo : IEquatable<AmBcsServerFailureLogger.AmBcsCheckInfo>
		{
			public AmBcsCheckInfo(AmBcsServerFailureLogger.AmBcsCheckType type, string checkName)
			{
				this.Type = type;
				this.CheckName = checkName;
			}

			public AmBcsServerFailureLogger.AmBcsCheckType Type { get; private set; }

			public string CheckName { get; private set; }

			public static bool IsEqual(object a, object b)
			{
				if (object.ReferenceEquals(a, b))
				{
					return true;
				}
				if (a == null || b == null)
				{
					return false;
				}
				if (a is AmBcsServerFailureLogger.AmBcsCheckInfo && b is AmBcsServerFailureLogger.AmBcsCheckInfo)
				{
					return ((AmBcsServerFailureLogger.AmBcsCheckInfo)a).Equals((AmBcsServerFailureLogger.AmBcsCheckInfo)b);
				}
				return a.Equals(b);
			}

			public bool Equals(AmBcsServerFailureLogger.AmBcsCheckInfo other)
			{
				return this.Type == other.Type && SharedHelper.StringIEquals(this.CheckName, other.CheckName);
			}

			public override string ToString()
			{
				if (this.m_toString == null)
				{
					this.m_toString = string.Format(AmBcsServerFailureLogger.AmBcsCheckInfo.s_toStringFormat, this.Type, this.CheckName);
				}
				return this.m_toString;
			}

			public override bool Equals(object obj)
			{
				return AmBcsServerFailureLogger.AmBcsCheckInfo.IsEqual(this, obj);
			}

			public override int GetHashCode()
			{
				return SharedHelper.GetStringIHashCode(this.ToString());
			}

			private static string s_toStringFormat = "Type:{0}, Name:{1}";

			private string m_toString;
		}
	}
}
