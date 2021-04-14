using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GroupMembershipEnumerator : IEnumerable<GroupMailbox>, IEnumerable
	{
		public GroupMembershipEnumerator(IEnumerable<string> aadGroups, IEnumerable<GroupMailbox> mailboxGroups, IQueuedGroupJoinInvoker joinInvoker, IGroupMailboxCollectionBuilder groupMailboxCollectionBuilder, IGroupsLogger logger)
		{
			ArgumentValidator.ThrowIfNull("aadGroups", aadGroups);
			ArgumentValidator.ThrowIfNull("mailboxGroups", mailboxGroups);
			ArgumentValidator.ThrowIfNull("joinInvoker", joinInvoker);
			ArgumentValidator.ThrowIfNull("groupMailboxCollectionBuilder", groupMailboxCollectionBuilder);
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.aadGroups = aadGroups;
			this.mailboxGroups = mailboxGroups;
			this.joinInvoker = joinInvoker;
			this.groupMailboxCollectionBuilder = groupMailboxCollectionBuilder;
			this.logger = logger;
		}

		public IEnumerator<GroupMailbox> GetEnumerator()
		{
			this.logger.CurrentAction = GroupMembershipAction.EnumerateGroups;
			HashSet<string> aadExternalIds = new HashSet<string>(this.aadGroups);
			GroupMembershipEnumerator.Tracer.TraceDebug<int>((long)this.GetHashCode(), "GroupMembershipEnumerator.GetEnumerator - AAD Group list contains {0} groups.", aadExternalIds.Count);
			foreach (GroupMailbox group in this.mailboxGroups)
			{
				if (aadExternalIds.Contains(group.Locator.ExternalId))
				{
					GroupMembershipEnumerator.Tracer.TraceInformation<string>(this.GetHashCode(), 0L, "GroupMembershipEnumerator.GetEnumerator - Found group in AAD and Mailbox. ExternalId={0}", group.Locator.ExternalId);
					aadExternalIds.Remove(group.Locator.ExternalId);
					yield return group;
				}
				else
				{
					this.logger.LogTrace("GroupMembershipEnumerator.GetEnumerator: Found group in mailbox but not AAD. Omitting from results. ExternalId={0}", new object[]
					{
						group.Locator.ExternalId
					});
				}
			}
			if (aadExternalIds.Count > 0)
			{
				foreach (GroupMailbox group2 in this.groupMailboxCollectionBuilder.BuildGroupMailboxes(aadExternalIds.ToArray<string>()))
				{
					this.logger.LogTrace("GroupMembershipEnumerator.GetEnumerator: Found group in AAD but not in Mailbox. Including in results. ExternalId={0}", new object[]
					{
						group2.Locator.ExternalId
					});
					this.joinInvoker.Enqueue(group2);
					yield return group2;
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generics interface of GetEnumerator.");
		}

		private static readonly Trace Tracer = ExTraceGlobals.ModernGroupsTracer;

		private readonly IEnumerable<string> aadGroups;

		private readonly IEnumerable<GroupMailbox> mailboxGroups;

		private readonly IQueuedGroupJoinInvoker joinInvoker;

		private readonly IGroupMailboxCollectionBuilder groupMailboxCollectionBuilder;

		private readonly IGroupsLogger logger;
	}
}
