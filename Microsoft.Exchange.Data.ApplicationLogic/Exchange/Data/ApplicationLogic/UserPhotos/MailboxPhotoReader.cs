using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxPhotoReader : IMailboxPhotoReader
	{
		public MailboxPhotoReader(ITracer upstreamTracer) : this(new XSOFactory(), upstreamTracer)
		{
		}

		internal MailboxPhotoReader(IXSOFactory xsoFactory, ITracer upstreamTracer)
		{
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
			this.xsoFactory = xsoFactory;
		}

		public PhotoMetadata Read(IMailboxSession session, UserPhotoSize size, bool preview, Stream output, IPerformanceDataLogger perfLogger)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (perfLogger == null)
			{
				throw new ArgumentNullException("perfLogger");
			}
			if (preview)
			{
				return this.ReadPreview(session, size, output);
			}
			return this.ReadActual(session, size, output, perfLogger);
		}

		public int ReadThumbprint(IMailboxSession session, bool preview, bool forceReloadThumbprint)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return this.ReadThumbprintInternal(session, preview, forceReloadThumbprint);
		}

		public int ReadThumbprint(IMailboxSession session, bool preview)
		{
			return this.ReadThumbprint(session, preview, true);
		}

		public int ReadAllPreviewSizes(IMailboxSession session, IDictionary<UserPhotoSize, byte[]> output)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			this.ReadAllSizesPreview(session, output);
			return this.ReadThumbprintInternal(session, true, true);
		}

		public bool HasPhotoBeenDeleted(Exception e)
		{
			ArgumentValidator.ThrowIfNull("e", e);
			return e.Data != null && e.Data.Contains("Photos.PhotoHasBeenDeletedKey");
		}

		internal static PropertyDefinition GetPropertyDefinitionForSize(UserPhotoSize size)
		{
			switch (size)
			{
			case UserPhotoSize.HR48x48:
				return UserPhotoSchema.UserPhotoHR48x48;
			case UserPhotoSize.HR64x64:
				return UserPhotoSchema.UserPhotoHR64x64;
			case UserPhotoSize.HR96x96:
				return UserPhotoSchema.UserPhotoHR96x96;
			case UserPhotoSize.HR120x120:
				return UserPhotoSchema.UserPhotoHR120x120;
			case UserPhotoSize.HR240x240:
				return UserPhotoSchema.UserPhotoHR240x240;
			case UserPhotoSize.HR360x360:
				return UserPhotoSchema.UserPhotoHR360x360;
			case UserPhotoSize.HR432x432:
				return UserPhotoSchema.UserPhotoHR432x432;
			case UserPhotoSize.HR504x504:
				return UserPhotoSchema.UserPhotoHR504x504;
			case UserPhotoSize.HR648x648:
				return UserPhotoSchema.UserPhotoHR648x648;
			default:
				throw new ArgumentNullException("size");
			}
		}

		private static UserPhotoSize NextSmallerSize(UserPhotoSize size)
		{
			switch (size)
			{
			case UserPhotoSize.HR64x64:
				return UserPhotoSize.HR48x48;
			case UserPhotoSize.HR96x96:
				return UserPhotoSize.HR64x64;
			case UserPhotoSize.HR120x120:
				return UserPhotoSize.HR96x96;
			case UserPhotoSize.HR240x240:
				return UserPhotoSize.HR120x120;
			case UserPhotoSize.HR360x360:
				return UserPhotoSize.HR240x240;
			case UserPhotoSize.HR432x432:
				return UserPhotoSize.HR360x360;
			case UserPhotoSize.HR504x504:
				return UserPhotoSize.HR432x432;
			case UserPhotoSize.HR648x648:
				return UserPhotoSize.HR504x504;
			}
			throw new ObjectNotFoundException(Strings.UserPhotoNotFound);
		}

		private PhotoMetadata ReadPreview(IMailboxSession session, UserPhotoSize size, Stream output)
		{
			StoreId storeId = this.FindPreviewItem(session);
			PhotoMetadata result;
			using (IItem item = Item.Bind((MailboxSession)session, storeId))
			{
				result = this.ReadPhotoOfSizeOrBestMatch(item, size, output);
			}
			return result;
		}

		private PhotoMetadata ReadActual(IMailboxSession session, UserPhotoSize size, Stream output, IPerformanceDataLogger perfLogger)
		{
			StoreId photoId = this.FindActualItem(session, perfLogger);
			PhotoMetadata result;
			using (IItem item = MailboxPhotoReader.BindAndTrackPerformance(session, photoId, "MailboxPhotoReaderBindPhotoItem", perfLogger))
			{
				using (new StopwatchPerformanceTracker("MailboxPhotoReaderReadStream", perfLogger))
				{
					using (new StorePerformanceTracker("MailboxPhotoReaderReadStream", perfLogger))
					{
						result = this.ReadPhotoOfSizeOrBestMatch(item, size, output);
					}
				}
			}
			return result;
		}

		private void ReadAllSizesPreview(IMailboxSession session, IDictionary<UserPhotoSize, byte[]> outputBuffers)
		{
			using (IItem item = Item.Bind((MailboxSession)session, this.FindPreviewItem(session)))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo reader: reading all sizes in preview item.");
				foreach (UserPhotoSize userPhotoSize in MailboxPhotoReader.AllPhotoSizes)
				{
					if (outputBuffers.ContainsKey(userPhotoSize))
					{
						this.tracer.TraceDebug<UserPhotoSize>((long)this.GetHashCode(), "Mailbox photo reader: skipping size {0} of preview because output buffer already contains a photo that size.", userPhotoSize);
					}
					else
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							this.ReadPhotoOfSizeOrBestMatch(item, userPhotoSize, memoryStream);
							outputBuffers[userPhotoSize] = memoryStream.ToArray();
						}
					}
				}
			}
		}

		private VersionedId FindPreviewItem(IMailboxSession session)
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo reader: searching for preview photo in store.");
			return this.FindPhotoItem(session, "IPM.UserPhoto.Preview");
		}

		private VersionedId FindActualItem(IMailboxSession session, IPerformanceDataLogger perfLogger)
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo reader: searching for photo in store.");
			VersionedId result;
			using (new StopwatchPerformanceTracker("MailboxPhotoReaderFindPhoto", perfLogger))
			{
				using (new StorePerformanceTracker("MailboxPhotoReaderFindPhoto", perfLogger))
				{
					result = this.FindPhotoItem(session, "IPM.UserPhoto");
				}
			}
			return result;
		}

		private VersionedId FindPhotoItem(IMailboxSession session, string itemClass)
		{
			List<VersionedId> list = (from photo in new UserPhotoEnumerator(session, this.GetPhotoStoreId(session), itemClass, this.xsoFactory, this.upstreamTracer)
			select photo.GetValueOrDefault<VersionedId>(ItemSchema.Id, null) into id
			where id != null
			select id).ToList<VersionedId>();
			if (list.Count == 0)
			{
				this.tracer.TraceDebug<string>((long)this.GetHashCode(), "Mailbox photo reader: no photo item with class {0} exists.", itemClass);
				throw new ObjectNotFoundException(Strings.UserPhotoWithClassNotFound(itemClass));
			}
			if (list.Count > 1)
			{
				this.tracer.TraceError<string>((long)this.GetHashCode(), "Mailbox photo reader: too many photo items with class {0} exist.", itemClass);
				throw new ObjectNotFoundException(Strings.UserPhotoTooManyItems(itemClass));
			}
			return list[0];
		}

		private StoreObjectId GetPhotoStoreId(IMailboxSession session)
		{
			StoreObjectId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.Configuration);
			if (defaultFolderId == null)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo reader: photo store doesn't exist.");
				throw new ObjectNotFoundException(Strings.UserPhotoStoreNotFound);
			}
			return defaultFolderId;
		}

		private int ReadThumbprintInternal(IMailboxSession session, bool preview, bool forceReloadThumbprint)
		{
			Mailbox mailbox = ((MailboxSession)session).Mailbox;
			if (forceReloadThumbprint)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo reader: reloading thumbprint properties");
				mailbox.ForceReload(MailboxPhotoReader.ThumbprintProperties);
			}
			object obj = mailbox.TryGetProperty(preview ? MailboxPhotoReader.UserPhotoPreviewCacheIdProperty : MailboxPhotoReader.UserPhotoCacheIdProperty);
			if (!(obj is int))
			{
				this.tracer.TraceDebug<bool>((long)this.GetHashCode(), "Mailbox photo reader: no thumbprint.  Preview? {0}", preview);
				throw new ObjectNotFoundException(Strings.UserPhotoThumbprintNotFound(preview));
			}
			if (0.Equals(obj))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo reader: thumbprint indicates photo has been deleted.");
				throw MailboxPhotoReader.CreateExceptionIndicatingPhotoHasBeenDeleted();
			}
			this.tracer.TraceDebug<int, bool>((long)this.GetHashCode(), "Mailbox photo reader: read thumbprint: {0:X8}.  Preview? {1}", (int)obj, preview);
			return (int)obj;
		}

		private PhotoMetadata ReadPhotoOfSizeOrBestMatch(IItem photo, UserPhotoSize size, Stream output)
		{
			UserPhotoSize userPhotoSize = size;
			PhotoMetadata result;
			try
			{
				IL_02:
				using (Stream stream = photo.OpenPropertyStream(MailboxPhotoReader.GetPropertyDefinitionForSize(userPhotoSize), PropertyOpenMode.ReadOnly))
				{
					stream.CopyTo(output);
					result = new PhotoMetadata
					{
						Length = stream.Length,
						ContentType = "image/jpeg"
					};
				}
			}
			catch (ObjectNotFoundException)
			{
				this.tracer.TraceDebug<UserPhotoSize>((long)this.GetHashCode(), "Mailbox photo reader: photo of size {0} not found.", userPhotoSize);
				userPhotoSize = MailboxPhotoReader.NextSmallerSize(userPhotoSize);
				goto IL_02;
			}
			return result;
		}

		private static IItem BindAndTrackPerformance(IMailboxSession session, StoreId photoId, string marker, IPerformanceDataLogger perfLogger)
		{
			IItem result;
			using (new StopwatchPerformanceTracker(marker, perfLogger))
			{
				using (new StorePerformanceTracker(marker, perfLogger))
				{
					result = Item.Bind((MailboxSession)session, photoId);
				}
			}
			return result;
		}

		private static Exception CreateExceptionIndicatingPhotoHasBeenDeleted()
		{
			ObjectNotFoundException ex = new ObjectNotFoundException(Strings.PhotoHasBeenDeleted);
			ex.Data["Photos.PhotoHasBeenDeletedKey"] = true;
			return ex;
		}

		private const string PhotoHasBeenDeletedKey = "Photos.PhotoHasBeenDeletedKey";

		internal const string PhotoContentType = "image/jpeg";

		internal const int ThumbprintThatIndicatesPhotoHasBeenDeleted = 0;

		internal static readonly UserPhotoSize[] AllPhotoSizes = new UserPhotoSize[]
		{
			UserPhotoSize.HR48x48,
			UserPhotoSize.HR64x64,
			UserPhotoSize.HR96x96,
			UserPhotoSize.HR120x120,
			UserPhotoSize.HR240x240,
			UserPhotoSize.HR360x360,
			UserPhotoSize.HR432x432,
			UserPhotoSize.HR504x504,
			UserPhotoSize.HR648x648
		};

		private static readonly PropertyDefinition UserPhotoCacheIdProperty = MailboxSchema.UserPhotoCacheId;

		private static readonly PropertyDefinition UserPhotoPreviewCacheIdProperty = MailboxSchema.UserPhotoPreviewCacheId;

		private static readonly PropertyDefinition[] ThumbprintProperties = new PropertyDefinition[]
		{
			MailboxPhotoReader.UserPhotoCacheIdProperty,
			MailboxPhotoReader.UserPhotoPreviewCacheIdProperty
		};

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly ITracer upstreamTracer;

		private readonly IXSOFactory xsoFactory;
	}
}
