using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MessagePropertyRestriction : PropertyRestriction
	{
		public MessagePropertyRestriction()
		{
			this.BlockBeforeLink.Add(MessageItemSchema.LinkedId);
			this.BlockBeforeLink.Add(MessageItemSchema.LinkedUrl);
			this.BlockBeforeLink.Add(MessageItemSchema.LinkedObjectVersion);
			this.BlockBeforeLink.Add(MessageItemSchema.LinkedSiteUrl);
			this.BlockBeforeLink.Add(MessageItemSchema.LinkedDocumentSize);
			this.BlockAfterLink.Add(MessageItemSchema.LinkedId);
			this.BlockAfterLink.Add(MessageItemSchema.LinkedUrl);
			this.BlockAfterLink.Add(MessageItemSchema.LinkedObjectVersion);
			this.BlockAfterLink.Add(MessageItemSchema.LinkedSiteUrl);
			this.BlockAfterLink.Add(MessageItemSchema.LinkedDocumentSize);
			this.BlockAfterLink.Add(StoreObjectSchema.DisplayName);
			this.BlockAfterLink.Add(StoreObjectSchema.ItemClass);
			this.BlockAfterLink.Add(InternalSchema.MapiSubject);
			this.BlockAfterLink.Add(InternalSchema.NormalizedSubjectInternal);
			this.BlockAfterLink.Add(InternalSchema.SubjectPrefixInternal);
			this.BlockAfterLink.Add(ItemSchema.NormalizedSubject);
			this.BlockAfterLink.Add(ItemSchema.SubjectPrefix);
			this.BlockAfterLink.Add(ItemSchema.LastModifiedBy);
			this.BlockAfterLink.Add(ItemSchema.ReceivedTime);
		}

		public static MessagePropertyRestriction Instance = new MessagePropertyRestriction();
	}
}
