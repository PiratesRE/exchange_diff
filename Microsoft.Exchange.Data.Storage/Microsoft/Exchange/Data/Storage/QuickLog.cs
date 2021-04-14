using System;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class QuickLog
	{
		private static bool IsDisabled { get; set; }

		private int MaxLogEntries { get; set; }

		public QuickLog() : this(40)
		{
		}

		public QuickLog(int maxLogEntries)
		{
			this.LogStore = new SingleInstanceItemHandler(this.LogMessageClass, DefaultFolderType.Configuration);
			this.MaxLogEntries = maxLogEntries;
		}

		private static void CheckLoggingDisabled(object ignored)
		{
			int num = 0;
			try
			{
				num = (int)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15", "DisableQuickLog", 0);
			}
			catch
			{
			}
			QuickLog.IsDisabled = (num == 1);
		}

		protected void AppendFormatLogEntry(MailboxSession session, string entry, params object[] args)
		{
			this.AppendFormatLogEntry(session, null, false, entry, args);
		}

		protected void AppendFormatLogEntry(MailboxSession session, Exception e, bool logWatsonReport, string entry, params object[] args)
		{
			if (QuickLog.IsDisabled)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(1024);
			try
			{
				stringBuilder.AppendFormat("{0}, Mailbox: '{1}', Entry ", DateTime.UtcNow.ToString(), session.DisplayName);
				stringBuilder.AppendFormat(entry, args);
				stringBuilder.AppendLine();
				if (e != null)
				{
					stringBuilder.Append(e);
				}
				else
				{
					logWatsonReport = false;
				}
			}
			catch
			{
				stringBuilder.AppendFormat("Failed to format string: {0}", entry);
				logWatsonReport = false;
			}
			try
			{
				this.WriteLogEntry(session, stringBuilder.ToString());
			}
			catch (LocalizedException ex)
			{
				if (ex.InnerException == null || (!(ex.InnerException is QuotaExceededException) && !(ex.InnerException is StorageTransientException)))
				{
					throw;
				}
				logWatsonReport = false;
			}
			if (logWatsonReport)
			{
				ExWatson.SendReport(e, ReportOptions.DoNotCollectDumps | ReportOptions.DoNotLogProcessAndThreadIds | ReportOptions.DoNotFreezeThreads, stringBuilder.ToString());
			}
		}

		protected abstract string LogMessageClass { get; }

		protected void WriteUniqueLogEntry(MailboxSession mailboxSession, string entry, string uniqueKey)
		{
			this.WriteLogEntry(mailboxSession, entry, uniqueKey);
		}

		protected void WriteLogEntry(MailboxSession session, string entry)
		{
			this.WriteLogEntry(session, entry, null);
		}

		private void WriteLogEntry(MailboxSession session, string entry, string uniqueKey)
		{
			if (QuickLog.IsDisabled)
			{
				return;
			}
			int i = 0;
			while (i < 2)
			{
				i++;
				try
				{
					string itemContent = this.LogStore.GetItemContent(session);
					if (uniqueKey != null && !string.IsNullOrEmpty(itemContent))
					{
						int num = itemContent.IndexOf('\n');
						if (num == -1)
						{
							num = itemContent.Length - 1;
						}
						if (itemContent.IndexOf(uniqueKey, 0, num) != -1)
						{
							break;
						}
					}
					StringBuilder stringBuilder = new StringBuilder(6000);
					stringBuilder.AppendLine(entry);
					if (!string.IsNullOrEmpty(itemContent))
					{
						int num2 = 1;
						int j;
						for (j = 0; j < entry.Length; j++)
						{
							if (entry[j] == '\n')
							{
								num2++;
							}
						}
						for (j = 0; j < itemContent.Length; j++)
						{
							if (itemContent[j] == '\n')
							{
								num2++;
								if (num2 >= this.MaxLogEntries)
								{
									break;
								}
							}
						}
						j++;
						if (j >= itemContent.Length)
						{
							j = itemContent.Length;
						}
						stringBuilder.Append(itemContent, 0, j);
					}
					this.LogStore.SetItemContent(session, stringBuilder.ToString());
					break;
				}
				catch (ObjectNotFoundException)
				{
					if (i == 2)
					{
						break;
					}
				}
				catch (VirusException)
				{
					this.LogStore.Delete(session);
					if (i == 2)
					{
						break;
					}
				}
			}
		}

		private string[] GetContent(MailboxSession session)
		{
			string itemContent = this.LogStore.GetItemContent(session);
			string[] result = null;
			if (!string.IsNullOrEmpty(itemContent))
			{
				char[] separator = new char[]
				{
					'\n'
				};
				result = itemContent.Split(separator, this.MaxLogEntries, StringSplitOptions.RemoveEmptyEntries);
			}
			return result;
		}

		private const int DefaultMaxLogEntries = 40;

		private const int MaxRetry = 2;

		private const ReportOptions WatsonReportOptions = ReportOptions.DoNotCollectDumps | ReportOptions.DoNotLogProcessAndThreadIds | ReportOptions.DoNotFreezeThreads;

		private const string DisableLoggingKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		private const string DisableQuickLogValue = "DisableQuickLog";

		private static readonly TimeSpan RegistryTimerFrequency = TimeSpan.FromMinutes(5.0);

		private static Timer registryTimer = new Timer(new TimerCallback(QuickLog.CheckLoggingDisabled), null, TimeSpan.Zero, QuickLog.RegistryTimerFrequency);

		private readonly SingleInstanceItemHandler LogStore;
	}
}
