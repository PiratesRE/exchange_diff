using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailMessageDataProvider : XsoMailboxDataProviderBase
	{
		public MailMessageDataProvider(ADSessionSettings adSessionSettings, ADUser mailboxOwner, ISecurityAccessToken userToken, string action) : base(adSessionSettings, mailboxOwner, userToken, action)
		{
		}

		public MailMessageDataProvider(ADSessionSettings adSessionSettings, ADUser mailboxOwner, string action) : base(adSessionSettings, mailboxOwner, action)
		{
		}

		internal MailMessageDataProvider()
		{
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			if (filter != null && !(filter is FalseFilter))
			{
				throw new NotSupportedException("filter");
			}
			if (sortBy != null)
			{
				throw new NotSupportedException("sortBy");
			}
			if (rootId != null && !(rootId is MailboxStoreObjectId))
			{
				throw new NotSupportedException("rootId: " + rootId.GetType().FullName);
			}
			if (!typeof(MailMessage).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
			MailboxStoreObjectId messageId = rootId as MailboxStoreObjectId;
			if (messageId != null)
			{
				MailMessage mailMessage = new MailMessage();
				try
				{
					using (MessageItem messageItem = MessageItem.Bind(base.MailboxSession, messageId.StoreObjectId, mailMessage.Schema.AllDependentXsoProperties))
					{
						mailMessage.LoadDataFromXso(messageId.MailboxOwnerId, messageItem);
						mailMessage.SetRecipients(messageItem.Recipients);
					}
				}
				catch (ObjectNotFoundException)
				{
					yield break;
				}
				yield return (T)((object)mailMessage);
			}
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			throw new NotImplementedException();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailMessageDataProvider>(this);
		}
	}
}
