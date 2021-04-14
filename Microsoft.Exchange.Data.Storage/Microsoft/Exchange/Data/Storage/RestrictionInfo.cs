using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RestrictionInfo
	{
		public RestrictionInfo(ContentRight usageRights, ExDateTime expiryTime, string owner)
		{
			EnumValidator.ThrowIfInvalid<ContentRight>(usageRights, "usageRights");
			this.usageRights = usageRights;
			this.expiryTime = expiryTime;
			this.conversationOwner = owner;
		}

		public ContentRight UsageRights
		{
			get
			{
				return this.usageRights;
			}
		}

		public ExDateTime ExpiryTime
		{
			get
			{
				return this.expiryTime;
			}
		}

		public string ConversationOwner
		{
			get
			{
				return this.conversationOwner;
			}
		}

		private ContentRight usageRights;

		private ExDateTime expiryTime;

		private string conversationOwner;
	}
}
