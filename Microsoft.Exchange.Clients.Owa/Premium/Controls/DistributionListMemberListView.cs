using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class DistributionListMemberListView : ListView
	{
		internal DistributionListMemberListView(UserContext userContext, DistributionList distributionList, ColumnId sortedColumn, SortOrder order) : this(userContext, DistributionListMemberListView.GetRecipientsFromDistributionList(distributionList), sortedColumn, order)
		{
		}

		internal DistributionListMemberListView(UserContext userContext, RecipientInfo[] recipients, int count, ColumnId sortedColumn, SortOrder order) : this(userContext, DistributionListMemberListView.GetRecipientsFromRecipientInfo(recipients, count), sortedColumn, order)
		{
		}

		private DistributionListMemberListView(UserContext userContext, List<Participant> participants, ColumnId sortedColumn, SortOrder order) : base(userContext, sortedColumn, order, false)
		{
			this.participants = participants;
			base.Initialize();
		}

		protected override string EventNamespace
		{
			get
			{
				return "EditPDL";
			}
		}

		protected override ViewType ViewTypeId
		{
			get
			{
				return ViewType.ContactModule;
			}
		}

		protected override bool IsSortable
		{
			get
			{
				return true;
			}
		}

		private static List<Participant> GetRecipientsFromDistributionList(DistributionList distributionList)
		{
			List<Participant> list = new List<Participant>();
			if (distributionList == null)
			{
				return list;
			}
			foreach (DistributionListMember distributionListMember in distributionList)
			{
				if (!(distributionListMember.Participant == null))
				{
					if (distributionListMember.Participant.Origin is OneOffParticipantOrigin && distributionListMember.MainEntryId is ADParticipantEntryId)
					{
						list.Add(distributionListMember.Participant.ChangeOrigin(new DirectoryParticipantOrigin()));
					}
					else
					{
						list.Add(distributionListMember.Participant);
					}
				}
			}
			return list;
		}

		private static List<Participant> GetRecipientsFromRecipientInfo(RecipientInfo[] recipients, int count)
		{
			List<Participant> list = new List<Participant>();
			for (int i = 0; i < Math.Min(recipients.Length, count); i++)
			{
				Participant item;
				recipients[i].ToParticipant(out item);
				list.Add(item);
			}
			return list;
		}

		public override SortBy[] GetSortByProperties()
		{
			return null;
		}

		protected override void InitializeListViewContents()
		{
			base.ViewDescriptor = new ViewDescriptor(ColumnId.MemberDisplayName, false, new ColumnId[]
			{
				ColumnId.MemberIcon,
				ColumnId.MemberDisplayName,
				ColumnId.MemberEmail
			});
			base.Contents = new DistributionListContents(base.UserContext, base.ViewDescriptor);
			base.Contents.DataSource = new DistributionListDataSource(base.UserContext, base.Contents.Properties, this.participants, base.SortedColumn, base.SortOrder);
		}

		protected override bool IsValidDataSource(IListViewDataSource dataSource)
		{
			return dataSource is DistributionListDataSource;
		}

		private List<Participant> participants;
	}
}
