using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.FederatedDirectory;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class QueuedGroupJoinInvoker : IQueuedGroupJoinInvoker
	{
		public QueuedGroupJoinInvoker()
		{
			this.pendingGroups = new Queue<GroupMailbox>(3);
		}

		public int QueueSize
		{
			get
			{
				return this.pendingGroups.Count;
			}
		}

		public void Enqueue(GroupMailbox group)
		{
			ArgumentValidator.ThrowIfNull("group", group);
			QueuedGroupJoinInvoker.Tracer.TraceDebug<string>((long)this.GetHashCode(), "QueuedGroupJoinInvoker.Enqueue. Enqueuing group to join. ExternalId={0}", group.Locator.ExternalId);
			this.pendingGroups.Enqueue(group);
		}

		public bool ProcessQueue(UserMailboxLocator userMailbox, Guid parentActivityId)
		{
			ArgumentValidator.ThrowIfNull("userMailbox", userMailbox);
			if (this.QueueSize == 0)
			{
				return true;
			}
			if (QueuedGroupJoinInvoker.UserMailboxesInProgress.TryAdd(userMailbox.ExternalId, true))
			{
				System.Threading.Tasks.Task.Run(delegate()
				{
					this.ProcessQueueImplementation(userMailbox, parentActivityId);
				});
				return true;
			}
			return false;
		}

		private void ProcessQueueImplementation(UserMailboxLocator userMailbox, Guid parentActivityId)
		{
			Guid activityId = Guid.NewGuid();
			string text = activityId.ToString();
			try
			{
				GroupsLogger groupsLogger = new GroupsLogger(GroupTaskName.JoinAADGroups, activityId);
				groupsLogger.LogTrace("QueuedGroupJoinInvoker.ProcessQueueImplementation. ParentActivityId={0}. User={1}. GroupCount={2}", new object[]
				{
					parentActivityId,
					userMailbox.ExternalId,
					this.QueueSize
				});
				ADUser user = userMailbox.FindAdUser();
				ExchangePrincipal user2 = ExchangePrincipal.FromADUser(user, null);
				while (this.pendingGroups.Count > 0)
				{
					GroupMailbox group = this.pendingGroups.Dequeue();
					this.InvokeJoinGroup(text, user2, group, groupsLogger);
				}
				groupsLogger.LogTrace("QueuedGroupJoinInvoker.ProcessQueueImplementation. Finished Processing Queue", new object[0]);
			}
			finally
			{
				bool flag;
				if (!QueuedGroupJoinInvoker.UserMailboxesInProgress.TryRemove(userMailbox.ExternalId, out flag))
				{
					QueuedGroupJoinInvoker.Tracer.TraceWarning<string, string>((long)this.GetHashCode(), "ActivityId={0}. QueuedGroupJoinInvoker.ProcessQueueImplementation - Unable to remove user external id from the dictionary. Key={1}", text, userMailbox.ExternalId);
				}
			}
		}

		private void InvokeJoinGroup(string activityId, ExchangePrincipal user, GroupMailbox group, IGroupsLogger logger)
		{
			try
			{
				using (PSLocalTask<SetGroupMailbox, object> pslocalTask = CmdletTaskFactory.Instance.CreateSetGroupMailboxTask(user))
				{
					pslocalTask.CaptureAdditionalIO = true;
					pslocalTask.Task.Identity = new RecipientIdParameter(group.Locator.ExternalId);
					pslocalTask.Task.AddedMembers = new RecipientIdParameter[]
					{
						new RecipientIdParameter(user.ExternalDirectoryObjectId)
					};
					logger.LogTrace("QueuedGroupJoinInvoker.InvokeJoinGroup - Executing Set-GroupMailbox: {0}.", new object[]
					{
						new PSLocalTaskLogging.SetGroupMailboxToString(pslocalTask.Task)
					});
					pslocalTask.Task.Execute();
					QueuedGroupJoinInvoker.Tracer.TraceDebug<string, PSLocalTaskLogging.TaskOutputToString>((long)this.GetHashCode(), "ActivityId={0}. QueuedGroupJoinInvoker.InvokeJoinGroup - TaskOutput: {1}", activityId, new PSLocalTaskLogging.TaskOutputToString(pslocalTask.AdditionalIO));
					if (pslocalTask.Error != null)
					{
						logger.LogTrace("QueuedGroupJoinInvoker.InvokeJoinGroup - Set-GroupMailbox failed. Group={0}. Error={1}. Output={2}", new object[]
						{
							group.Locator.ExternalId,
							pslocalTask.ErrorMessage,
							new PSLocalTaskLogging.TaskOutputToString(pslocalTask.AdditionalIO)
						});
					}
				}
			}
			catch (Exception ex)
			{
				if (!GrayException.IsGrayException(ex))
				{
					throw;
				}
				logger.LogException(ex, "QueuedGroupJoinInvoker.InvokeJoinGroup - Caught exception while executing Set-GroupMailbox. Group={0}.", new object[]
				{
					group.Locator.ExternalId
				});
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.ModernGroupsTracer;

		private static readonly ConcurrentDictionary<string, bool> UserMailboxesInProgress = new ConcurrentDictionary<string, bool>();

		private readonly Queue<GroupMailbox> pendingGroups;
	}
}
