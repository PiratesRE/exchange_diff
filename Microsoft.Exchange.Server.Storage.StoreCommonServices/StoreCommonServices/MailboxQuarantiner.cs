using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class MailboxQuarantiner
	{
		public MailboxQuarantiner(StoreDatabase database)
		{
			this.database = database;
		}

		internal StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		internal List<Guid> QuarantinedMailboxes
		{
			get
			{
				return this.quarantinedMailboxes.ToList<Guid>();
			}
		}

		public void SetMailboxQuarantineStatus(Context context, Guid mailboxGuid, bool quarantined, TimeSpan quarantineDuration, string quarantineReason)
		{
			bool flag = false;
			using (context.AssociateWithDatabase(this.Database))
			{
				TimeSpan lockTimeout = TimeSpan.FromMinutes(30.0);
				bool flag2;
				MailboxState mailboxState;
				if (!MailboxStateCache.TryGetByGuidLocked(context, mailboxGuid, MailboxCreation.DontAllow, false, false, (MailboxState state) => lockTimeout, context.Diagnostics, out flag2, out mailboxState))
				{
					if (flag2)
					{
						Globals.AssertRetail(false, "Mailbox lock contention on attempt to quarantine mailbox.");
					}
					if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Mailbox \"" + mailboxGuid.ToString() + "\" does not exist");
					}
				}
				else
				{
					try
					{
						mailboxState.AddReference();
						mailboxState.Quarantined = quarantined;
						if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "State of mailbox \"" + mailboxGuid.ToString() + "\" has been set to " + (quarantined ? "quarantined" : "unquarantined"));
						}
						MailboxTaskQueue.GetMailboxTaskQueue(context, mailboxState).ScheduleWorkerTaskIfNeeded();
						flag = true;
					}
					finally
					{
						mailboxState.ReleaseReference();
						mailboxState.ReleaseMailboxLock(false);
					}
					if (flag)
					{
						if (quarantined)
						{
							if (!this.quarantinedMailboxes.Contains(mailboxGuid))
							{
								this.quarantinedMailboxes.Add(mailboxGuid);
								if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Mailbox \"" + mailboxGuid.ToString() + "\" has been put into quarantined list");
								}
								Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_MailboxQuarantined, new object[]
								{
									mailboxGuid.ToString(),
									quarantineDuration,
									quarantineReason
								});
							}
							FaultInjection.InjectFault(MailboxQuarantiner.mailboxQuarantinedTestHook);
						}
						else
						{
							if (this.quarantinedMailboxes.Contains(mailboxGuid))
							{
								this.quarantinedMailboxes.Remove(mailboxGuid);
								if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Mailbox \"" + mailboxGuid.ToString() + "\" has been removed from the quarantined list");
								}
								Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_MailboxUnquarantined, new object[]
								{
									mailboxGuid.ToString()
								});
							}
							FaultInjection.InjectFault(MailboxQuarantiner.mailboxUnquarantinedTestHook);
						}
						StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(this.Database);
						databaseInstance.QuarantinedMailboxCount.RawValue = (long)this.QuarantinedMailboxes.Count;
					}
				}
			}
		}

		internal static IDisposable SetMailboxQuarantinedTestHook(Action action)
		{
			return MailboxQuarantiner.mailboxQuarantinedTestHook.SetTestHook(action);
		}

		internal static IDisposable SetMailboxUnquarantinedTestHook(Action action)
		{
			return MailboxQuarantiner.mailboxUnquarantinedTestHook.SetTestHook(action);
		}

		internal static void EnumeratePreQuarantinedMailboxes(Context context, MailboxQuarantiner quarantiner, Func<bool> shouldCallbackContinue)
		{
			if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Start enumerating pre-quarantined mailbox on database \"" + quarantiner.Database.MdbName + "\"");
			}
			HashSet<Guid> hashSet = new HashSet<Guid>(quarantiner.QuarantinedMailboxes);
			List<PrequarantinedMailbox> preQuarantinedMailboxes = MailboxQuarantineProvider.Instance.GetPreQuarantinedMailboxes(quarantiner.Database.MdbGuid);
			foreach (PrequarantinedMailbox prequarantinedMailbox in preQuarantinedMailboxes)
			{
				if (hashSet.Contains(prequarantinedMailbox.MailboxGuid))
				{
					hashSet.Remove(prequarantinedMailbox.MailboxGuid);
				}
				TimeSpan t = DateTime.UtcNow - prequarantinedMailbox.LastCrashTime;
				if (t < prequarantinedMailbox.QuarantineDuration && prequarantinedMailbox.CrashCount >= 3)
				{
					if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Mailbox \"" + prequarantinedMailbox.MailboxGuid.ToString() + "\" will be quarantined");
					}
					quarantiner.SetMailboxQuarantineStatus(context, prequarantinedMailbox.MailboxGuid, true, prequarantinedMailbox.QuarantineDuration, prequarantinedMailbox.QuarantineReason);
				}
				else
				{
					if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Mailbox \"" + prequarantinedMailbox.MailboxGuid.ToString() + "\" will be unquarantined as the last crash time exceeded the duration");
					}
					quarantiner.SetMailboxQuarantineStatus(context, prequarantinedMailbox.MailboxGuid, false, prequarantinedMailbox.QuarantineDuration, prequarantinedMailbox.QuarantineReason);
					if (t > MailboxQuarantiner.quarantineWindow)
					{
						MailboxQuarantineProvider.Instance.UnquarantineMailbox(quarantiner.Database.MdbGuid, prequarantinedMailbox.MailboxGuid);
					}
				}
			}
			if (hashSet.Count > 0)
			{
				foreach (Guid mailboxGuid in hashSet)
				{
					if (ExTraceGlobals.MailboxQuarantineTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.MailboxQuarantineTracer.TraceDebug(0L, "Mailbox \"" + mailboxGuid.ToString() + "\" will be automaticaly unquarantined as the key has been deleted");
					}
					quarantiner.SetMailboxQuarantineStatus(context, mailboxGuid, false, TimeSpan.Zero, string.Empty);
				}
			}
		}

		internal static void MountEventHandler(StoreDatabase database)
		{
			MailboxQuarantiner context = new MailboxQuarantiner(database);
			Task<MailboxQuarantiner>.TaskCallback taskCallback = TaskExecutionWrapper<MailboxQuarantiner>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.EnumeratePreQuarantinedMailboxes, ClientType.System, database.MdbGuid), new TaskExecutionWrapper<MailboxQuarantiner>.TaskCallback<Context>(MailboxQuarantiner.EnumeratePreQuarantinedMailboxes));
			taskCallback(new TaskExecutionDiagnosticsProxy(), context, () => true);
			RecurringTask<MailboxQuarantiner> task = new RecurringTask<MailboxQuarantiner>(taskCallback, context, MailboxQuarantiner.interval, false);
			database.TaskList.Add(task, true);
		}

		internal const int CrashCountThreshold = 3;

		private static TimeSpan quarantineWindow = TimeSpan.FromHours(2.0);

		private static TimeSpan interval = TimeSpan.FromSeconds(30.0);

		private static Hookable<Action> mailboxQuarantinedTestHook = Hookable<Action>.Create(true, null);

		private static Hookable<Action> mailboxUnquarantinedTestHook = Hookable<Action>.Create(true, null);

		private readonly StoreDatabase database;

		private HashSet<Guid> quarantinedMailboxes = new HashSet<Guid>();
	}
}
