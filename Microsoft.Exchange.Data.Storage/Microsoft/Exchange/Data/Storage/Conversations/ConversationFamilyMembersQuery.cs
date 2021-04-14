using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationFamilyMembersQuery : ConversationMembersQueryBase
	{
		public ConversationFamilyMembersQuery(IXSOFactory xsoFactory, IMailboxSession session) : base(xsoFactory, session)
		{
		}

		protected override ComparisonFilter CreateConversationFilter(ConversationId conversationFamilyId)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.MapiConversationFamilyId, conversationFamilyId.GetBytes());
		}

		protected override IFolder GetSearchFolder(ICollection<PropertyDefinition> headerPropertyDefinition)
		{
			return base.XsoFactory.BindToFolder(base.Session, DefaultFolderType.AllItems, headerPropertyDefinition.ToArray<PropertyDefinition>());
		}

		protected override IQueryResult GetQueryResult(IFolder rootFolder, ComparisonFilter conversationFilter, ICollection<PropertyDefinition> headerPropertyDefinition)
		{
			return rootFolder.IItemQuery(ItemQueryType.None, conversationFilter, ConversationFamilyMembersQuery.SortColumns, headerPropertyDefinition);
		}

		private static readonly SortBy[] SortColumns = new SortBy[]
		{
			new SortBy(InternalSchema.MapiConversationFamilyId, SortOrder.Ascending),
			new SortBy(ItemSchema.ConversationIndex, SortOrder.Ascending)
		};
	}
}
