using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MessageCategoryDataProvider : XsoMailboxDataProviderBase
	{
		internal ADUser ADMailboxOwner
		{
			get
			{
				return this.adMailboxOwner;
			}
		}

		public MessageCategoryDataProvider(ADSessionSettings adSessionSettings, ADUser mailboxOwner, string action) : base(adSessionSettings, mailboxOwner, action)
		{
			this.adMailboxOwner = mailboxOwner;
		}

		internal MessageCategoryDataProvider()
		{
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			MessageCategoryId messageCategoryId = rootId as MessageCategoryId;
			if (sortBy != null)
			{
				throw new NotSupportedException("sortBy");
			}
			if (rootId != null && messageCategoryId == null)
			{
				throw new NotSupportedException("rootId");
			}
			MasterCategoryList categoryList = base.MailboxSession.GetMasterCategoryList();
			if (messageCategoryId == null || (messageCategoryId.Name == null && messageCategoryId.CategoryId == null))
			{
				foreach (Category category in categoryList)
				{
					yield return (T)((object)this.ConvertCategoryToPresentationObject(category));
				}
			}
			else if (messageCategoryId.CategoryId != null)
			{
				Category category2 = categoryList[messageCategoryId.CategoryId.Value];
				yield return (T)((object)this.ConvertCategoryToPresentationObject(category2));
			}
			else
			{
				Category category3 = categoryList[messageCategoryId.Name];
				if (category3 != null)
				{
					yield return (T)((object)this.ConvertCategoryToPresentationObject(category3));
				}
			}
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			throw new NotImplementedException();
		}

		protected override void InternalDelete(ConfigurableObject instance)
		{
			throw new NotImplementedException();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MessageCategoryDataProvider>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		private MessageCategory ConvertCategoryToPresentationObject(Category category)
		{
			return new MessageCategory
			{
				MailboxOwnerId = base.MailboxSession.MailboxOwner.ObjectId,
				Name = category.Name,
				Color = category.Color,
				Guid = category.Guid
			};
		}

		private ADUser adMailboxOwner;
	}
}
