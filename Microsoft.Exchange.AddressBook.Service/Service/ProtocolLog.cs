using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal static class ProtocolLog
	{
		internal static bool Enabled { get; private set; }

		internal static string DefaultLogFilePath
		{
			get
			{
				return ProtocolLog.defaultLogFilePath;
			}
		}

		internal static LogSchema Schema
		{
			get
			{
				if (ProtocolLog.schema == null)
				{
					ProtocolLog.schema = new LogSchema("Microsoft Exchange", "15.00.1497.015", ProtocolLog.defaultLogTypeName, ProtocolLog.GetColumnArray());
				}
				return ProtocolLog.schema;
			}
		}

		internal static void Initialize(ExDateTime serviceStartTime, string logFilePath, TimeSpan maxRetentionPeriond, ByteQuantifiedSize directorySizeQuota, ByteQuantifiedSize perFileSizeQuota, bool applyHourPrecision)
		{
			ProtocolLog.log = new Log(ProtocolLog.defaultLogFilePrefix, new LogHeaderFormatter(ProtocolLog.Schema), ProtocolLog.defaultLogComponent);
			ProtocolLog.log.Configure(logFilePath, maxRetentionPeriond, (long)directorySizeQuota.ToBytes(), (long)perFileSizeQuota.ToBytes(), applyHourPrecision);
			ProtocolLog.Enabled = true;
		}

		internal static void SetDefaults(string logFilePath, string logTypeName, string logFilePrefix, string logComponent)
		{
			ProtocolLog.defaultLogFilePath = logFilePath;
			ProtocolLog.defaultLogTypeName = logTypeName;
			ProtocolLog.defaultLogFilePrefix = logFilePrefix;
			ProtocolLog.defaultLogComponent = logComponent;
		}

		internal static void Shutdown()
		{
			if (ProtocolLog.log != null)
			{
				ProtocolLog.log.Close();
			}
		}

		internal static ProtocolLogSession CreateSession(int sessionId, string clientAddress, string serverAddress, string protocolSequence)
		{
			ProtocolLogSession protocolLogSession = new ProtocolLogSession(new LogRowFormatter(ProtocolLog.Schema));
			protocolLogSession[ProtocolLog.Field.SessionId] = sessionId;
			if (!string.IsNullOrEmpty(clientAddress))
			{
				protocolLogSession[ProtocolLog.Field.ClientIp] = clientAddress;
			}
			if (!string.IsNullOrEmpty(serverAddress))
			{
				protocolLogSession[ProtocolLog.Field.ServerIp] = string.Intern(serverAddress);
			}
			if (!string.IsNullOrEmpty(protocolSequence))
			{
				protocolLogSession[ProtocolLog.Field.Protocol] = string.Intern(protocolSequence);
			}
			return protocolLogSession;
		}

		internal static void Append(LogRowFormatter row)
		{
			if (ProtocolLog.Enabled)
			{
				ProtocolLog.log.Append(row, 0);
			}
		}

		internal static void LogProtocolFailure(string operation, IList<string> requestIds, IList<string> cookies, string message, string userName, string protocolSequence, string clientAddress, string organization, Exception exception)
		{
			ProtocolLogSession protocolLogSession = ProtocolLog.CreateSession(0, clientAddress, null, protocolSequence);
			if (!string.IsNullOrEmpty(organization))
			{
				protocolLogSession[ProtocolLog.Field.OrganizationInfo] = organization;
			}
			string text = string.Empty;
			if (cookies != null && cookies.Count > 0)
			{
				text = string.Join("|", cookies);
			}
			string text2 = string.Empty;
			if (requestIds != null && requestIds.Count > 0)
			{
				text2 = string.Join("|", requestIds);
			}
			string arg = string.Empty;
			if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2))
			{
				arg = string.Format(" [{0}{1}{2}]", text, string.IsNullOrEmpty(text) ? string.Empty : ";", text2);
			}
			string arg2 = string.Empty;
			if (!string.IsNullOrEmpty(userName))
			{
				arg2 = string.Format(" [{0}]", userName);
			}
			protocolLogSession.AppendProtocolFailure(operation, string.Format("{0}{1}{2}", message, arg2, arg), exception.LogMessage(true));
		}

		internal static string LogMessage(this Exception exception, bool wantDetails = true)
		{
			if (exception == null)
			{
				return string.Empty;
			}
			if (wantDetails)
			{
				return exception.ToString();
			}
			return exception.GetType().ToString() + ": " + exception.Message;
		}

		private static string[] GetColumnArray()
		{
			string[] array = new string[ProtocolLog.Fields.Length];
			for (int i = 0; i < ProtocolLog.Fields.Length; i++)
			{
				array[i] = ProtocolLog.Fields[i].ColumnName;
			}
			return array;
		}

		private const string LogTypeName = "AddressBook Protocol Logs";

		private const string LogFilePrefix = "AddressBook_";

		private const string LogComponent = "AddressBookProtocolLogs";

		internal static readonly ProtocolLog.FieldInfo[] Fields = new ProtocolLog.FieldInfo[]
		{
			new ProtocolLog.FieldInfo(ProtocolLog.Field.DateTime, "date-time"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.SessionId, "session-id"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.SequenceNumber, "seq-number"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ClientName, "client-name"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.OrganizationInfo, "organization-info"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ClientIp, "client-ip"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ServerIp, "server-ip"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.Protocol, "protocol"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.Operation, "operation"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.RpcStatus, "rpc-status"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ProcessingTime, "processing-time"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.OperationSpecific, "operation-specific"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.Failures, "failures"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.Authentication, "authentication"),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.Delay, "delay")
		};

		private static LogSchema schema;

		private static Log log;

		private static string defaultLogFilePath = string.Format("{0}Logging\\AddressBook Service\\", ExchangeSetupContext.InstallPath);

		private static string defaultLogTypeName = "AddressBook Protocol Logs";

		private static string defaultLogFilePrefix = "AddressBook_";

		private static string defaultLogComponent = "AddressBookProtocolLogs";

		internal enum Field
		{
			DateTime,
			SessionId,
			SequenceNumber,
			ClientName,
			OrganizationInfo,
			ClientIp,
			ServerIp,
			Protocol,
			Operation,
			RpcStatus,
			ProcessingTime,
			OperationSpecific,
			Failures,
			Authentication,
			Delay
		}

		internal struct FieldInfo
		{
			public FieldInfo(ProtocolLog.Field field, string columnName)
			{
				this.Field = field;
				this.ColumnName = columnName;
			}

			internal readonly ProtocolLog.Field Field;

			internal readonly string ColumnName;
		}
	}
}
