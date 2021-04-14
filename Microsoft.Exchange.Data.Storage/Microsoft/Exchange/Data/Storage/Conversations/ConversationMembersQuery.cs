using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationMembersQuery : ConversationMembersQueryBase
	{
		public ConversationMembersQuery(IXSOFactory xsoFactory, IMailboxSession session) : base(xsoFactory, session)
		{
		}

		protected override ComparisonFilter CreateConversationFilter(ConversationId conversationId)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.MapiConversationId, conversationId.GetBytes());
		}

		protected override IFolder GetSearchFolder(ICollection<PropertyDefinition> headerPropertyDefinition)
		{
			return base.XsoFactory.BindToFolder(base.Session, DefaultFolderType.Configuration, headerPropertyDefinition.ToArray<PropertyDefinition>());
		}

		protected override IQueryResult GetQueryResult(IFolder rootFolder, ComparisonFilter conversationFilter, ICollection<PropertyDefinition> headerPropertyDefinition)
		{
			return rootFolder.IConversationMembersQuery(conversationFilter, null, headerPropertyDefinition);
		}
	}
}
