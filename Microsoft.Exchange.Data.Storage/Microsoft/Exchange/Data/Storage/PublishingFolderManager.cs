using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublishingFolderManager : SharingFolderManagerBase<PublishingSubscriptionData>
	{
		public PublishingFolderManager(MailboxSession mailboxSession) : base(mailboxSession)
		{
		}

		protected override ExtendedFolderFlags SharingFolderFlags
		{
			get
			{
				return ExtendedFolderFlags.ReadOnly | ExtendedFolderFlags.WebCalFolder | ExtendedFolderFlags.SharedIn | ExtendedFolderFlags.PersonalShare | ExtendedFolderFlags.ExclusivelyBound;
			}
		}

		protected override LocalizedString CreateLocalFolderName(PublishingSubscriptionData subscriptionData)
		{
			return new LocalizedString(subscriptionData.RemoteFolderName);
		}

		protected override void CreateOrUpdateSharingBinding(PublishingSubscriptionData subscriptionData, string localFolderName, StoreObjectId folderId)
		{
			if (this.InternetCalendarBindingExists(folderId))
			{
				return;
			}
			this.CreateInternetCalendarBinding(subscriptionData, folderId);
		}

		private bool InternetCalendarBindingExists(StoreObjectId folderId)
		{
			bool result;
			using (Folder folder = Folder.Bind(base.MailboxSession, folderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, null, PublishingFolderManager.QueryBindingColumns))
				{
					result = queryResult.SeekToCondition(SeekReference.OriginBeginning, PublishingFolderManager.SharingProviderGuidFilter);
				}
			}
			return result;
		}

		private void CreateInternetCalendarBinding(PublishingSubscriptionData subscriptionData, StoreObjectId folderId)
		{
			using (Item item = MessageItem.CreateAssociated(base.MailboxSession, folderId))
			{
				item[BindingItemSchema.SharingProviderGuid] = PublishingFolderManager.InternetCalendarProviderGuid;
				item[StoreObjectSchema.ItemClass] = "IPM.Sharing.Binding.In";
				item[BindingItemSchema.SharingLocalType] = subscriptionData.DataType.ContainerClass;
				item.Save(SaveMode.NoConflictResolution);
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, StoreObjectId>((long)this.GetHashCode(), "{0}: PublishingFolderManager.CreateInternetCalendarBinding saved binding message for folder id: {1}", base.MailboxSession.MailboxOwner, subscriptionData.LocalFolderId);
			}
		}

		private const ExtendedFolderFlags ICalSharingFolderFlags = ExtendedFolderFlags.ReadOnly | ExtendedFolderFlags.WebCalFolder | ExtendedFolderFlags.SharedIn | ExtendedFolderFlags.PersonalShare | ExtendedFolderFlags.ExclusivelyBound;

		private static readonly PropertyDefinition[] QueryBindingColumns = new PropertyDefinition[]
		{
			BindingItemSchema.SharingProviderGuid,
			StoreObjectSchema.ItemClass
		};

		public static readonly Guid InternetCalendarProviderGuid = new Guid("{FB98726D-69AE-491e-B7D8-8F0E026E0D5F}");

		private static readonly ComparisonFilter SharingProviderGuidFilter = new ComparisonFilter(ComparisonOperator.Equal, BindingItemSchema.SharingProviderGuid, PublishingFolderManager.InternetCalendarProviderGuid);

		private enum QueryBindingColumnsIndex
		{
			SharingProviderGuid,
			ItemClass
		}
	}
}
