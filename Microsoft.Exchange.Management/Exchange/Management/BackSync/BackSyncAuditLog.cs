using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.BackSync
{
	internal class BackSyncAuditLog
	{
		public static bool IsEnabled
		{
			get
			{
				bool result;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(BackSyncAuditLog.AuditLogRegistryKey))
				{
					int num = (registryKey != null) ? ((int)registryKey.GetValue("Enable", 1)) : 1;
					result = (num > 0);
				}
				return result;
			}
		}

		public static BackSyncAuditLog Instance
		{
			get
			{
				if (BackSyncAuditLog.instance == null)
				{
					lock (BackSyncAuditLog.syncRoot)
					{
						if (BackSyncAuditLog.instance == null)
						{
							BackSyncAuditLog.instance = new BackSyncAuditLog();
						}
					}
				}
				return BackSyncAuditLog.instance;
			}
		}

		static BackSyncAuditLog()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(BackSyncAuditLog.AuditLogRegistryKey))
			{
				if (registryKey != null)
				{
					BackSyncAuditLog.LogDirectory = (string)registryKey.GetValue("LogDirectory", BackSyncAuditLog.LogDirectory);
					BackSyncAuditLog.LogFileMaxAge = (int)registryKey.GetValue("LogFileMaxAge", BackSyncAuditLog.LogFileMaxAge);
					BackSyncAuditLog.LogDirectoryMaxSize = (int)registryKey.GetValue("LogDirectoryMaxSize", BackSyncAuditLog.LogDirectoryMaxSize);
					BackSyncAuditLog.LogFileMaxSize = (int)registryKey.GetValue("LogFileMaxSize", BackSyncAuditLog.LogFileMaxSize);
					BackSyncAuditLog.LogCacheSize = (int)registryKey.GetValue("LogCacheSize", BackSyncAuditLog.LogCacheSize);
					BackSyncAuditLog.LogFlushInterval = (int)registryKey.GetValue("LogFlushInterval", BackSyncAuditLog.LogFlushInterval);
				}
			}
		}

		private BackSyncAuditLog()
		{
			this.auditLogger = new Log("BackSyncAudit", new LogHeaderFormatter(BackSyncAuditLog.AuditLogSchema, true), "MSExchange Back Sync");
			this.auditLogger.Configure(BackSyncAuditLog.LogDirectory, TimeSpan.FromDays((double)BackSyncAuditLog.LogFileMaxAge), (long)BackSyncAuditLog.LogDirectoryMaxSize * 1024L * 1024L, (long)BackSyncAuditLog.LogFileMaxSize * 1024L * 1024L, BackSyncAuditLog.LogCacheSize * 1024 * 1024, TimeSpan.FromSeconds((double)BackSyncAuditLog.LogFlushInterval), true);
		}

		public void Append(string executingUser, byte[] cookie, NameValueCollection parameters, object response, Dictionary<SyncObject, Exception> errors)
		{
			string responseIdentity = Guid.NewGuid().ToString("N");
			this.Append(BackSyncAuditLog.CreateLogRowForFirstLine(executingUser, cookie, parameters, responseIdentity));
			if (response is GetChangesResponse)
			{
				DirectoryChanges getChangesResult = ((GetChangesResponse)response).GetChangesResult;
				this.Append<DirectoryObject>(responseIdentity, getChangesResult.Objects);
				this.Append<DirectoryLink>(responseIdentity, getChangesResult.Links);
			}
			else
			{
				if (!(response is GetDirectoryObjectsResponse))
				{
					throw new NotImplementedException("Need implement writing audit log for response type: " + response.GetType().Name);
				}
				DirectoryObjectsAndLinks getDirectoryObjectsResult = ((GetDirectoryObjectsResponse)response).GetDirectoryObjectsResult;
				this.Append<DirectoryObject>(responseIdentity, getDirectoryObjectsResult.Objects);
				this.Append<DirectoryLink>(responseIdentity, getDirectoryObjectsResult.Links);
				this.Append<DirectoryObjectError>(responseIdentity, getDirectoryObjectsResult.Errors);
			}
			this.Append(responseIdentity, errors);
		}

		private void Append<T>(string responseIdentity, T[] objects)
		{
			if (objects == null || objects.Length == 0)
			{
				return;
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = false
			};
			for (int i = 0; i < objects.Length; i++)
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
				{
					xmlSerializer.Serialize(xmlWriter, objects[i]);
					this.Append(BackSyncAuditLog.CreateLogRowForSyncObject(responseIdentity, objects[i], stringBuilder.ToString()));
					stringBuilder.Length = 0;
				}
			}
		}

		private void Append(string responseIdentity, Dictionary<SyncObject, Exception> errors)
		{
			foreach (KeyValuePair<SyncObject, Exception> keyValuePair in errors)
			{
				this.Append(BackSyncAuditLog.CreateLogRowForError(responseIdentity, keyValuePair.Key, keyValuePair.Value));
			}
		}

		private void Append(LogRowFormatter row)
		{
			this.auditLogger.Append(row, 0);
		}

		private static LogRowFormatter CreateLogRowForSyncObject(string responseIdentity, object syncObject, string objectInXml)
		{
			string objectId = null;
			string sourceId = null;
			string targetId = null;
			string contextId;
			if (syncObject is DirectoryObject)
			{
				DirectoryObject directoryObject = (DirectoryObject)syncObject;
				objectId = directoryObject.ObjectId;
				contextId = directoryObject.ContextId;
			}
			else if (syncObject is DirectoryObjectError)
			{
				DirectoryObjectError directoryObjectError = (DirectoryObjectError)syncObject;
				objectId = directoryObjectError.ObjectId;
				contextId = directoryObjectError.ContextId;
			}
			else
			{
				if (!(syncObject is DirectoryLink))
				{
					throw new NotSupportedException("Don't know how to extract IDs for new type: " + syncObject.GetType().Name);
				}
				DirectoryLink directoryLink = (DirectoryLink)syncObject;
				contextId = directoryLink.ContextId;
				sourceId = directoryLink.SourceId;
				targetId = directoryLink.TargetId;
			}
			return BackSyncAuditLog.CreateLogRow(null, null, null, responseIdentity, objectId, contextId, sourceId, targetId, objectInXml, null);
		}

		private static LogRowFormatter CreateLogRowForFirstLine(string executingUser, byte[] cookie, NameValueCollection parameters, string responseIdentity)
		{
			string cookieInBase = (cookie == null) ? null : Convert.ToBase64String(cookie);
			string parameters2 = (parameters == null) ? null : BackSyncAuditLog.ToParametersString(parameters);
			return BackSyncAuditLog.CreateLogRow(executingUser, cookieInBase, parameters2, responseIdentity, null, null, null, null, null, null);
		}

		private static LogRowFormatter CreateLogRowForError(string responseIdentity, SyncObject syncObject, Exception e)
		{
			return BackSyncAuditLog.CreateLogRow(null, null, null, responseIdentity, syncObject.ObjectId, syncObject.ContextId, null, null, null, e.Message);
		}

		private static LogRowFormatter CreateLogRow(string executingUser, string cookieInBase64, string parameters, string responseIdentity, string objectId, string contextId, string sourceId, string targetId, string objectInXml, string error)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(BackSyncAuditLog.AuditLogSchema);
			logRowFormatter[0] = null;
			logRowFormatter[1] = executingUser;
			logRowFormatter[2] = cookieInBase64;
			logRowFormatter[3] = parameters;
			logRowFormatter[4] = responseIdentity;
			logRowFormatter[5] = objectId;
			logRowFormatter[6] = contextId;
			logRowFormatter[7] = sourceId;
			logRowFormatter[8] = targetId;
			logRowFormatter[9] = ((objectInXml == null) ? null : objectInXml.Replace('\r', ' ').Replace('\n', ' '));
			logRowFormatter[10] = ((error == null) ? null : error.Replace('\r', ' ').Replace('\n', ' '));
			return logRowFormatter;
		}

		private static string ToParametersString(NameValueCollection parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in parameters)
			{
				string text = (string)obj;
				stringBuilder.AppendFormat("{0}: {1};", text, parameters[text]);
			}
			return stringBuilder.ToString();
		}

		private const string ParameterStringFormat = "{0}: {1};";

		private const string ComponentName = "MSExchange Back Sync";

		private const string LogFileNamePrefix = "BackSyncAudit";

		private const string LogType = "Audit Log";

		private const string LogEnableValueName = "Enable";

		private const string LogDirectoryValueName = "LogDirectory";

		private const string LogFileMaxAgeValueName = "LogFileMaxAge";

		private const string LogDirectoryMaxSizeValueName = "LogDirectoryMaxSize";

		private const string LogFileMaxSizeValueName = "LogFileMaxSize";

		private const string LogCacheSizeName = "LogCacheSize";

		private const string LogFlushIntervalName = "LogFlushInterval";

		private const int LogEnableDefaultValue = 1;

		private static readonly string[] LogFields = new string[]
		{
			"Timestamp",
			"ExecutingUser",
			"Cookie",
			"Parameters",
			"ResponseIdentity",
			"ObjectID",
			"ContextID",
			"SourceID",
			"TargetID",
			"Object in XML",
			"Error"
		};

		private static readonly string LogVersion = ConfigurationContext.Setup.GetExecutingVersion().ToString();

		private static readonly string AuditLogRegistryKey = Path.Combine("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\BackSync", "AuditLog");

		private static readonly string LogDirectory = Path.Combine(ConfigurationContext.Setup.InstallPath, "Logging\\BackSyncLogs\\");

		private static readonly int LogFileMaxSize = 100;

		private static readonly int LogFileMaxAge = 30;

		private static readonly int LogDirectoryMaxSize = 30720;

		private static readonly int LogCacheSize = 2;

		private static readonly int LogFlushInterval = 60;

		private static readonly LogSchema AuditLogSchema = new LogSchema("MSExchange Back Sync", BackSyncAuditLog.LogVersion, "Audit Log", BackSyncAuditLog.LogFields);

		private static object syncRoot = new object();

		private static BackSyncAuditLog instance;

		private Log auditLogger;
	}
}
