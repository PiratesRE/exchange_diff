using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxPhotoWriter : IMailboxPhotoWriter
	{
		public MailboxPhotoWriter(IMailboxSession session, ITracer upstreamTracer) : this(session, new XSOFactory(), upstreamTracer)
		{
		}

		internal MailboxPhotoWriter(IMailboxSession session, IXSOFactory xsoFactory, ITracer upstreamTracer)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.upstreamTracer = upstreamTracer;
			this.session = session;
			this.xsoFactory = xsoFactory;
		}

		public void UploadPreview(int thumbprint, IDictionary<UserPhotoSize, byte[]> photos)
		{
			if (photos == null || photos.Count == 0)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "Mailbox photo writer: uploaded photo is empty.");
				return;
			}
			this.DeleteAllPreviewItems();
			this.CreatePreviewItem(photos);
			this.StorePreviewThumbprint(thumbprint);
		}

		public void Save()
		{
			VersionedId previewId = this.FindPreviewItem();
			int previewThumbprint = this.ReadPreviewThumbprint();
			this.DeleteAllActualPhotoItems();
			this.PromotePreviewToActualPhoto(previewId, previewThumbprint);
		}

		public void Clear()
		{
			this.DeleteAllActualPhotoItems();
			this.StoreThumbprint(MailboxPhotoWriter.UserPhotoCacheIdProperty, 0);
			this.EnsureDeletedNotificationItem();
			this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo writer: photo has been cleared.");
		}

		public void ClearPreview()
		{
			this.DeleteAllPreviewItems();
			this.DeletePropertyFromMailboxTableAndSaveChanges(MailboxPhotoWriter.UserPhotoPreviewCacheIdProperty);
			this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo writer: PREVIEW photo has been cleared.");
		}

		private void PromotePreviewToActualPhoto(VersionedId previewId, int previewThumbprint)
		{
			this.tracer.TraceDebug<VersionedId, int>((long)this.GetHashCode(), "Mailbox photo writer: promoting preview photo to actual photo.  Preview photo has id = {0} and thumbprint = {1:X8}", previewId, previewThumbprint);
			using (Item item = Item.Bind((MailboxSession)this.session, previewId, MailboxPhotoWriter.AllPhotoSizeProperties))
			{
				using (IItem item2 = Item.CloneItem((MailboxSession)this.session, this.GetPhotoStoreId(), item, false, false, MailboxPhotoWriter.AllPhotoSizeProperties))
				{
					item2.ClassName = "IPM.UserPhoto";
					item2.Save(SaveMode.ResolveConflicts);
					this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo writer: preview photo item has been promoted to actual photo item.");
				}
			}
			this.session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
			{
				previewId
			});
			this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo writer: deleted preview item.");
			this.StoreThumbprint(MailboxPhotoWriter.UserPhotoCacheIdProperty, previewThumbprint);
			this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo writer: thumbprint of preview photo has been promoted to thumbprint of actual photo.");
		}

		private VersionedId FindPreviewItem()
		{
			List<VersionedId> list = (from photo in new UserPhotoEnumerator(this.session, this.GetPhotoStoreId(), "IPM.UserPhoto.Preview", this.xsoFactory, this.upstreamTracer)
			select photo.GetValueOrDefault<VersionedId>(ItemSchema.Id, null) into id
			where id != null
			select id).ToList<VersionedId>();
			if (list.Count == 0)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "Mailbox photo writer: no preview photo item.");
				throw new ObjectNotFoundException(Strings.UserPhotoNotFound);
			}
			if (list.Count > 1)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "Mailbox photo writer: too many preview photo items.");
				throw new ObjectNotFoundException(Strings.UserPhotoTooManyItems("IPM.UserPhoto.Preview"));
			}
			return list[0];
		}

		private void DeleteAllActualPhotoItems()
		{
			this.DeleteAllPhotoItems("IPM.UserPhoto");
		}

		private void DeleteAllPreviewItems()
		{
			this.DeleteAllPhotoItems("IPM.UserPhoto.Preview");
		}

		private void EnsureDeletedNotificationItem()
		{
			this.DeleteAllPhotoItems("IPM.UserPhoto.DeletedNotification");
			this.CreateDeletedNotificationItem();
		}

		private void DeleteAllPhotoItems(string itemClass)
		{
			VersionedId[] array = (from photo in new UserPhotoEnumerator(this.session, this.GetPhotoStoreId(), itemClass, this.xsoFactory, this.upstreamTracer)
			select photo.GetValueOrDefault<VersionedId>(ItemSchema.Id, null) into id
			where id != null
			select id).ToArray<VersionedId>();
			if (array.Length == 0)
			{
				return;
			}
			this.session.Delete(DeleteItemFlags.HardDelete, array);
		}

		private void CreateDeletedNotificationItem()
		{
			using (IItem item = Item.Create((MailboxSession)this.session, "IPM.UserPhoto.DeletedNotification", this.GetPhotoStoreId()))
			{
				item.Save(SaveMode.ResolveConflicts);
				this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo writer: dummy deleted notification item created successfully.");
			}
		}

		private void CreatePreviewItem(IDictionary<UserPhotoSize, byte[]> photos)
		{
			using (IItem item = Item.Create((MailboxSession)this.session, "IPM.UserPhoto.Preview", this.GetPhotoStoreId()))
			{
				foreach (UserPhotoSize userPhotoSize in MailboxPhotoWriter.AllPhotoSizes)
				{
					byte[] buffer;
					if (!photos.TryGetValue(userPhotoSize, out buffer))
					{
						this.tracer.TraceDebug<UserPhotoSize>((long)this.GetHashCode(), "Mailbox photo writer: photo of size {0} not available in preview (input).  Skipped.", userPhotoSize);
					}
					else
					{
						this.tracer.TraceDebug<UserPhotoSize>((long)this.GetHashCode(), "Mailbox photo writer: storing photo of size {0} onto preview item.", userPhotoSize);
						using (MemoryStream memoryStream = new MemoryStream(buffer))
						{
							this.StorePhotoOfSpecificSize(item, userPhotoSize, MailboxPhotoReader.GetPropertyDefinitionForSize(userPhotoSize), memoryStream);
						}
					}
				}
				item.Save(SaveMode.ResolveConflicts);
				this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo writer: preview item created successfully.");
			}
		}

		private void StorePhotoOfSpecificSize(IItem photoItem, UserPhotoSize size, PropertyDefinition specificSizeProperty, MemoryStream photo)
		{
			if (photo == null || photo.Length == 0L)
			{
				this.tracer.TraceDebug<UserPhotoSize>((long)this.GetHashCode(), "Mailbox photo writer: photo of size {0} not available and will not be stored onto preview item.", size);
				return;
			}
			using (Stream stream = photoItem.OpenPropertyStream(specificSizeProperty, PropertyOpenMode.Create))
			{
				photo.Seek(0L, SeekOrigin.Begin);
				photo.CopyTo(stream);
			}
		}

		private void StorePreviewThumbprint(int thumbprint)
		{
			this.StoreThumbprint(MailboxPhotoWriter.UserPhotoPreviewCacheIdProperty, thumbprint);
			this.tracer.TraceDebug<int>((long)this.GetHashCode(), "Mailbox photo writer: stored preview thumbprint = {0:X8}", thumbprint);
		}

		private int ReadPreviewThumbprint()
		{
			Mailbox mailbox = ((MailboxSession)this.session).Mailbox;
			object obj = mailbox.TryGetProperty(MailboxPhotoWriter.UserPhotoPreviewCacheIdProperty);
			if (obj is int)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo writer: read thumbprint of preview photo: {0:X8}", new object[]
				{
					obj
				});
				return (int)obj;
			}
			this.tracer.TraceError((long)this.GetHashCode(), "Mailbox photo reader: thumbprint of preview photo doesn't exist.");
			throw new ObjectNotFoundException(Strings.UserPhotoThumbprintNotFound(true));
		}

		private StoreObjectId GetPhotoStoreId()
		{
			return this.session.GetDefaultFolderId(DefaultFolderType.Configuration);
		}

		private void DeletePropertyFromMailboxTableAndSaveChanges(PropertyDefinition property)
		{
			Mailbox mailbox = ((MailboxSession)this.session).Mailbox;
			mailbox.Delete(property);
			mailbox.Save();
			mailbox.Load();
		}

		private void StoreThumbprint(PropertyDefinition thumbprintProperty, int thumbprint)
		{
			Mailbox mailbox = ((MailboxSession)this.session).Mailbox;
			mailbox[thumbprintProperty] = thumbprint;
			mailbox.Save();
			mailbox.Load();
		}

		private static readonly PropertyDefinition UserPhotoCacheIdProperty = MailboxSchema.UserPhotoCacheId;

		private static readonly PropertyDefinition UserPhotoPreviewCacheIdProperty = MailboxSchema.UserPhotoPreviewCacheId;

		private static readonly UserPhotoSize[] AllPhotoSizes = MailboxPhotoReader.AllPhotoSizes;

		private static readonly ICollection<PropertyDefinition> AllPhotoSizeProperties = new PropertyDefinition[]
		{
			UserPhotoSchema.UserPhotoHR648x648,
			UserPhotoSchema.UserPhotoHR504x504,
			UserPhotoSchema.UserPhotoHR432x432,
			UserPhotoSchema.UserPhotoHR360x360,
			UserPhotoSchema.UserPhotoHR240x240,
			UserPhotoSchema.UserPhotoHR120x120,
			UserPhotoSchema.UserPhotoHR96x96,
			UserPhotoSchema.UserPhotoHR64x64,
			UserPhotoSchema.UserPhotoHR48x48
		};

		private readonly IMailboxSession session;

		private readonly IXSOFactory xsoFactory;

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly ITracer upstreamTracer;
	}
}
