using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SharePointSignalStore;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class OfficeGraphLog
	{
		public static void Start()
		{
			OfficeGraphLog.officeGraphLogSchema = new LogSchema("Microsoft Exchange Server", "15.00.1497.012", "Office Graph Log", OfficeGraphLog.Fields);
			OfficeGraphLog.log = new Log(OfficeGraphLogSchema.LogPrefix, new LogHeaderFormatter(OfficeGraphLog.officeGraphLogSchema), "OfficeGraph");
			OfficeGraphLog.log.Configure("D:\\OfficeGraph", TimeSpan.FromDays(1.0), 104857600L, 5242880L);
		}

		public static void LogSignal(OfficeGraphSignalType signalType, string signal, string organizationId, string sharePointUrl)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(OfficeGraphLog.officeGraphLogSchema);
			logRowFormatter[1] = signalType.ToString();
			logRowFormatter[2] = signal;
			logRowFormatter[3] = organizationId;
			logRowFormatter[4] = sharePointUrl;
			OfficeGraphLog.Append(logRowFormatter);
		}

		public static void Stop()
		{
			if (OfficeGraphLog.log != null)
			{
				OfficeGraphLog.log.Close();
				OfficeGraphLog.log = null;
			}
		}

		private static string[] InitializeFields()
		{
			string[] array = new string[Enum.GetValues(typeof(OfficeGraphLog.OfficeGraphLogField)).Length];
			array[0] = "TimeStamp";
			array[1] = "SignalType";
			array[2] = "Signal";
			array[3] = "OrganizationId";
			array[4] = "SharePointUrl";
			return array;
		}

		private static void Append(LogRowFormatter row)
		{
			OfficeGraphLog.log.Append(row, 0, DateTime.UtcNow);
		}

		private const string LogComponentName = "OfficeGraph";

		private const string LogPath = "D:\\OfficeGraph";

		private static readonly string[] Fields = OfficeGraphLog.InitializeFields();

		private static LogSchema officeGraphLogSchema;

		private static Log log;

		internal enum OfficeGraphLogField
		{
			TimeStamp,
			SignalType,
			Signal,
			OrganizationId,
			SharePointUrl
		}
	}
}
