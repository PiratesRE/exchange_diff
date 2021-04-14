using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GroupMembershipCompositeReader : IGroupMembershipReader<GroupMailbox>
	{
		public GroupMembershipCompositeReader(IGroupMembershipReader<string> aadReader, IGroupMembershipReader<GroupMailbox> mailboxReader, IQueuedGroupJoinInvoker joinInvoker, IGroupsLogger logger, IGroupMailboxCollectionBuilder groupMailboxCollectionBuilder) : this(aadReader, mailboxReader, logger, (IEnumerable<string> aadGroups, IEnumerable<GroupMailbox> mailboxGroups) => new GroupMembershipEnumerator(aadGroups, mailboxGroups, joinInvoker, groupMailboxCollectionBuilder, logger))
		{
		}

		public GroupMembershipCompositeReader(IGroupMembershipReader<string> aadReader, IGroupMembershipReader<GroupMailbox> mailboxReader, IGroupsLogger logger, Func<IEnumerable<string>, IEnumerable<GroupMailbox>, IEnumerable<GroupMailbox>> groupMembershipEnumeratorCreator)
		{
			ArgumentValidator.ThrowIfNull("aadReader", aadReader);
			ArgumentValidator.ThrowIfNull("mailboxReader", mailboxReader);
			ArgumentValidator.ThrowIfNull("logger", logger);
			ArgumentValidator.ThrowIfNull("groupMembershipEnumeratorCreator", groupMembershipEnumeratorCreator);
			this.aadReader = aadReader;
			this.mailboxReader = mailboxReader;
			this.logger = logger;
			this.groupMembershipEnumeratorCreator = groupMembershipEnumeratorCreator;
		}

		public IEnumerable<GroupMailbox> GetJoinedGroups()
		{
			this.logger.CurrentAction = GroupMembershipAction.EnumerateGroups;
			IEnumerable<string> enumerable = null;
			Task<IEnumerable<string>> task = Task.Run<IEnumerable<string>>(() => this.aadReader.GetJoinedGroups());
			IEnumerable<GroupMailbox> joinedGroups = this.mailboxReader.GetJoinedGroups();
			try
			{
				if (task.Wait(GroupMembershipCompositeReader.MaxWaitTimeForAADQuery))
				{
					enumerable = task.Result;
				}
			}
			catch (AggregateException exception)
			{
				this.logger.LogException(exception, "GroupMembershipCompositeReader.GetJoinedGroups - Unable to retrieve group membership from AAD.", new object[0]);
			}
			if (enumerable == null)
			{
				GroupMembershipCompositeReader.Tracer.TraceDebug((long)this.GetHashCode(), "GroupMembershipCompositeReader.GetJoinedGroups - AAD lookup failed. Returning unfiltered list from mailbox.");
				return joinedGroups;
			}
			GroupMembershipCompositeReader.Tracer.TraceDebug((long)this.GetHashCode(), "GroupMembershipCompositeReader.GetJoinedGroups - Found groups in AAD. Building filtered / augmented list.");
			return this.groupMembershipEnumeratorCreator(enumerable, joinedGroups);
		}

		private static readonly Trace Tracer = ExTraceGlobals.ModernGroupsTracer;

		private static readonly TimeSpan MaxWaitTimeForAADQuery = TimeSpan.FromSeconds(20.0);

		private readonly IGroupMembershipReader<string> aadReader;

		private readonly IGroupMembershipReader<GroupMailbox> mailboxReader;

		private readonly IGroupsLogger logger;

		private readonly Func<IEnumerable<string>, IEnumerable<GroupMailbox>, IEnumerable<GroupMailbox>> groupMembershipEnumeratorCreator;
	}
}
