using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class MailboxQuarantineProvider : IMailboxQuarantineProvider
	{
		private MailboxQuarantineProvider()
		{
		}

		public static IMailboxQuarantineProvider Instance
		{
			get
			{
				return MailboxQuarantineProvider.hookableInstance.Value;
			}
		}

		internal static IDisposable SetTestHook(IMailboxQuarantineProvider testHook)
		{
			return MailboxQuarantineProvider.hookableInstance.SetTestHook(testHook);
		}

		public bool IsMigrationAccessAllowed(Guid databaseGuid, Guid mailboxGuid)
		{
			string subkeyName = string.Format("SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\Private-{1}\\QuarantinedMailboxes\\{2}", Environment.MachineName, databaseGuid, mailboxGuid);
			bool value = RegistryReader.Instance.GetValue<bool>(Registry.LocalMachine, subkeyName, "AllowMigrationOfQuarantinedMailbox", false);
			if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxQuarantineTracer.TraceDebug<Guid, Guid, bool>(0L, "IsMigrationAccessAllowed check (database={0}, mailbox={1}, result={2}", databaseGuid, mailboxGuid, value);
			}
			return value;
		}

		public void PrequarantineMailbox(Guid databaseGuid, Guid mailboxGuid, string reason)
		{
			try
			{
				string subkeyName = string.Format("SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\Private-{1}\\QuarantinedMailboxes\\{2}", Environment.MachineName, databaseGuid, mailboxGuid);
				RegistryWriter.Instance.CreateSubKey(Registry.LocalMachine, subkeyName);
				int value = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, subkeyName, "CrashCount", 0);
				RegistryWriter.Instance.SetValue(Registry.LocalMachine, subkeyName, "CrashCount", value + 1, RegistryValueKind.DWord);
				RegistryWriter.Instance.SetValue(Registry.LocalMachine, subkeyName, "LastCrashTime", DateTime.UtcNow.ToFileTime(), RegistryValueKind.QWord);
				if (string.IsNullOrEmpty(reason))
				{
					RegistryWriter.Instance.DeleteValue(Registry.LocalMachine, subkeyName, "MailboxQuarantineDescription");
				}
				else
				{
					RegistryWriter.Instance.SetValue(Registry.LocalMachine, subkeyName, "MailboxQuarantineDescription", PrequarantinedMailbox.TruncateQuarantineReason(reason), RegistryValueKind.String);
				}
				if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Pre-quarantining mailbox \"" + mailboxGuid.ToString() + "\"");
				}
			}
			catch (ArgumentException ex)
			{
				if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.MailboxQuarantineTracer.TraceError(0L, "Unexpected error druing pre-quarantine a mailbox " + ex.ToString());
				}
			}
		}

		public void UnquarantineMailbox(Guid databaseGuid, Guid mailboxGuid)
		{
			string subkeyName = string.Format("SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\Private-{1}\\QuarantinedMailboxes\\{2}", Environment.MachineName, databaseGuid, mailboxGuid);
			try
			{
				RegistryWriter.Instance.DeleteSubKeyTree(Registry.LocalMachine, subkeyName);
				if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Un-quarantining mailbox \"" + mailboxGuid.ToString() + "\"");
				}
			}
			catch (ArgumentException)
			{
				if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Quarantine key already removed");
				}
			}
		}

		public List<PrequarantinedMailbox> GetPreQuarantinedMailboxes(Guid databaseGuid)
		{
			IRegistryReader instance = RegistryReader.Instance;
			IRegistryWriter instance2 = RegistryWriter.Instance;
			string text = string.Format("SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\Private-{1}\\QuarantinedMailboxes", Environment.MachineName, databaseGuid);
			string[] array = null;
			try
			{
				array = instance.GetSubKeyNames(Registry.LocalMachine, text);
			}
			catch (ArgumentException)
			{
				if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Parent key deleted, un-quarantining all mailboxes");
				}
				array = null;
			}
			if (array == null)
			{
				return new List<PrequarantinedMailbox>(0);
			}
			List<PrequarantinedMailbox> list = new List<PrequarantinedMailbox>(array.Length);
			string[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				string text2 = array2[i];
				Guid guid = Guid.Empty;
				string text3 = Path.Combine(text, text2);
				try
				{
					guid = new Guid(text2);
				}
				catch (FormatException)
				{
					if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.MailboxQuarantineTracer.TraceError(0L, "Unrecognized key \"" + text2 + "\" deleted");
					}
					try
					{
						instance2.DeleteSubKeyTree(Registry.LocalMachine, text3);
					}
					catch (ArgumentException)
					{
						if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Key \"" + text3 + "\" has been deleted");
						}
					}
					goto IL_28D;
				}
				goto IL_104;
				IL_28D:
				i++;
				continue;
				IL_104:
				int num = Convert.ToInt32(MailboxQuarantineProvider.defaultQuarantineDuration.TotalSeconds);
				try
				{
					num = instance.GetValue<int>(Registry.LocalMachine, text3, "MailboxQuarantineDurationInSeconds", num);
				}
				catch (OverflowException)
				{
					num = Convert.ToInt32(MailboxQuarantineProvider.defaultQuarantineDuration.TotalSeconds);
				}
				string text4 = string.Empty;
				text4 = instance.GetValue<string>(Registry.LocalMachine, text3, "MailboxQuarantineDescription", text4);
				text4 = PrequarantinedMailbox.TruncateQuarantineReason(text4);
				int value = instance.GetValue<int>(Registry.LocalMachine, text3, "CrashCount", -1);
				long value2 = instance.GetValue<long>(Registry.LocalMachine, text3, "LastCrashTime", (long)((ulong)-1));
				if (value2 != (long)((ulong)-1) && value != -1)
				{
					try
					{
						DateTime dateTime = DateTime.FromFileTimeUtc(value2);
						list.Add(new PrequarantinedMailbox(guid, value, dateTime, TimeSpan.FromSeconds((double)num), text4));
						if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Mailbox {0} quarantined: crash count={1}, lastCrashtime={2}, duration={3}, reason = {4}", new object[]
							{
								guid,
								value,
								dateTime,
								num,
								text4
							});
						}
					}
					catch (ArgumentOutOfRangeException)
					{
						if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Invalid last crash time, key \"" + text2 + "\" deleted");
						}
						try
						{
							instance2.DeleteSubKeyTree(Registry.LocalMachine, text3);
						}
						catch (ArgumentException)
						{
							if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Key \"" + text3 + "\" has been deleted");
							}
						}
					}
					goto IL_28D;
				}
				goto IL_28D;
			}
			return list;
		}

		private const string ValueNameCrashCount = "CrashCount";

		private const string ValueNameLastCrashTime = "LastCrashTime";

		private const string ValueNameAllowMigration = "AllowMigrationOfQuarantinedMailbox";

		private const string ValueQuarantineDuration = "MailboxQuarantineDurationInSeconds";

		private const string ValueQuarantineReason = "MailboxQuarantineDescription";

		private const string KeyNameFormatQuarantinedMailboxRoot = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\Private-{1}\\QuarantinedMailboxes";

		private const string KeyNameFormatQuarantinedMailbox = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\Private-{1}\\QuarantinedMailboxes\\{2}";

		private static TimeSpan defaultQuarantineDuration = TimeSpan.FromHours(24.0);

		private static Hookable<IMailboxQuarantineProvider> hookableInstance = Hookable<IMailboxQuarantineProvider>.Create(false, new MailboxQuarantineProvider());
	}
}
