using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.FederatedDirectory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class WarmupGroupManagementDependency
	{
		internal static bool HasWarmUpAttempted
		{
			get
			{
				return WarmupGroupManagementDependency.hasWarmUpAttempted;
			}
		}

		internal static Action<object, string> OnAttemptCompletionCallBack { get; set; }

		internal static void Reset()
		{
			lock (WarmupGroupManagementDependency.syncObject)
			{
				WarmupGroupManagementDependency.hasWarmUpAttempted = false;
			}
		}

		internal static void WarmUpAsyncIfRequired(ExchangePrincipal currentUser)
		{
			if (WarmupGroupManagementDependency.HasWarmUpAttempted)
			{
				return;
			}
			lock (WarmupGroupManagementDependency.syncObject)
			{
				if (!WarmupGroupManagementDependency.HasWarmUpAttempted)
				{
					WarmupGroupManagementDependency.hasWarmUpAttempted = true;
					WarmupGroupManagementDependency.LogEntry("Scheduling Group management warmup call.");
					System.Threading.Tasks.Task.Factory.StartNew(delegate()
					{
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();
						using (PSLocalTask<NewGroupMailbox, GroupMailbox> pslocalTask = CmdletTaskFactory.Instance.CreateNewGroupMailboxTask(currentUser))
						{
							pslocalTask.Task.Name = "WarmUpRequest";
							pslocalTask.Task.ExecutingUser = new RecipientIdParameter(currentUser.MailboxInfo.PrimarySmtpAddress.ToString());
							pslocalTask.WhatIfMode = true;
							WarmupGroupManagementDependency.LogEntry("Execute warm up call");
							pslocalTask.Task.Execute();
							WarmupGroupManagementDependency.LogEntry(string.Format("Executed new group mailbox warm call. output = {0}, error = {1}, total seconds = {2}", (pslocalTask.Result == null) ? "null" : pslocalTask.Result.ToString(), pslocalTask.Error, stopwatch.Elapsed.TotalSeconds.ToString("n2")));
							if (WarmupGroupManagementDependency.OnAttemptCompletionCallBack != null)
							{
								WarmupGroupManagementDependency.OnAttemptCompletionCallBack(pslocalTask.Result, pslocalTask.ErrorMessage);
							}
						}
					}).ContinueWith(delegate(System.Threading.Tasks.Task t)
					{
						WarmupGroupManagementDependency.LogEntry("UnExpected exception. error =" + t.Exception);
						if (WarmupGroupManagementDependency.OnAttemptCompletionCallBack != null)
						{
							WarmupGroupManagementDependency.OnAttemptCompletionCallBack(null, t.Exception.ToString());
						}
					}, TaskContinuationOptions.OnlyOnFaulted);
				}
			}
		}

		private static void LogEntry(string message)
		{
			FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.TraceTag>
			{
				{
					FederatedDirectoryLogSchema.TraceTag.TaskName,
					"WarmupGroupManagementDependency"
				},
				{
					FederatedDirectoryLogSchema.TraceTag.ActivityId,
					"3ca7a0ab-9404-497f-b691-000000000000"
				},
				{
					FederatedDirectoryLogSchema.TraceTag.CurrentAction,
					"WarmupGroupManagementDependency"
				},
				{
					FederatedDirectoryLogSchema.TraceTag.Message,
					message
				}
			});
		}

		private const string ActivityId = "3ca7a0ab-9404-497f-b691-000000000000";

		private static readonly object syncObject = new object();

		private static volatile bool hasWarmUpAttempted;
	}
}
