using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.TypeConversion.Converters
{
	internal class IdConverter
	{
		public static IdConverter Instance
		{
			get
			{
				return IdConverter.SingleTonInstance;
			}
		}

		public static IList<AttachmentId> GetAttachmentIds(string id)
		{
			byte[] buffer = Convert.FromBase64String(id);
			IList<AttachmentId> result;
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					List<AttachmentId> list = new List<AttachmentId>();
					ServiceIdConverter.ReadAttachmentIds(binaryReader, list);
					result = list;
				}
			}
			return result;
		}

		public static string GetHierarchicalAttachmentStringId(IList<AttachmentId> attachmentIds)
		{
			int requiredByteCountForAttachmentIds = ServiceIdConverter.GetRequiredByteCountForAttachmentIds(attachmentIds);
			byte[] array = new byte[requiredByteCountForAttachmentIds];
			ServiceIdConverter.WriteAttachmentIds(attachmentIds, array, 0);
			return Convert.ToBase64String(array);
		}

		public virtual TEntity CreateBasicEntity<TEntity>(VersionedId objectId, IStoreSession session) where TEntity : IStorageEntity, new()
		{
			string changeKey;
			string id = this.ToStringId(objectId, session, out changeKey);
			TEntity result = (default(TEntity) == null) ? Activator.CreateInstance<TEntity>() : default(TEntity);
			result.Id = id;
			result.ChangeKey = changeKey;
			return result;
		}

		public virtual StoreId GetStoreId(IStorageEntity entity)
		{
			return this.ToStoreId(entity.Id, entity.ChangeKey);
		}

		public virtual StoreId ToStoreId(string entityId, string changeKey)
		{
			StoreObjectId storeObjectId = this.ToStoreObjectId(entityId);
			if (string.IsNullOrEmpty(changeKey))
			{
				return storeObjectId;
			}
			byte[] changeKey2 = Convert.FromBase64String(changeKey);
			return new VersionedId(storeObjectId, changeKey2);
		}

		public virtual StoreObjectId ToStoreObjectId(string id)
		{
			return StoreId.EwsIdToStoreObjectId(id);
		}

		public virtual string ToStringId(StoreId storeId, IStoreSession session, out string changeKey)
		{
			VersionedId versionedId = storeId as VersionedId;
			changeKey = ((versionedId == null) ? null : versionedId.ChangeKeyAsBase64String());
			return this.ToStringId(storeId, session);
		}

		public virtual string ToStringId(StoreId storeId, IStoreSession session)
		{
			if (session is IPublicFolderSession)
			{
				StoreObjectId parentFolderId = session.GetParentFolderId(StoreId.GetStoreObjectId(storeId));
				return StoreId.PublicFolderStoreIdToEwsId(storeId, parentFolderId);
			}
			return StoreId.StoreIdToEwsId(session.MailboxGuid, storeId);
		}

		public virtual string ToStringId(AttachmentId attachementId)
		{
			return IdConverter.GetHierarchicalAttachmentStringId(new List<AttachmentId>
			{
				attachementId
			});
		}

		private static readonly IdConverter SingleTonInstance = new IdConverter();
	}
}
