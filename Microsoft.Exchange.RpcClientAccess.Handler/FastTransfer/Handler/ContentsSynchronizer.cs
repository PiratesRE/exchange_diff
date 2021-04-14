using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContentsSynchronizer : SynchronizerBase, IContentsSynchronizer, IDisposable
	{
		internal ContentsSynchronizer(ReferenceCount<CoreFolder> referenceCoreFolder, SyncFlag syncFlags, Restriction restriction, SyncExtraFlag extraFlags, IcsState icsState, Encoding string8Encoding, bool wantUnicode, params PropertyTag[] referencePropertyTags) : base(referenceCoreFolder, syncFlags, extraFlags, icsState)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.string8Encoding = string8Encoding;
				this.wantUnicode = wantUnicode;
				ManifestConfigFlags manifestFlags = ContentsSynchronizer.GetManifestFlags(syncFlags, extraFlags);
				QueryFilter filter = (restriction != null) ? new FilterRestrictionTranslator(referenceCoreFolder.ReferencedObject.Session).Translate(restriction) : null;
				PropertyDefinition[] mustRequestPropertyDefinitions = ContentsSynchronizer.MustRequestPropertyDefinitions;
				IPropertyBag propertyBag = new MemoryPropertyBag(this.SessionAdaptor);
				icsState.Checkpoint(propertyBag);
				IcsStateStream icsStateStream = new IcsStateStream(propertyBag);
				CoreFolder referencedObject = this.SyncRootFolder.ReferencedObject;
				this.manifestProvider = referencedObject.GetContentManifest(manifestFlags, filter, icsStateStream.ToXsoState(), mustRequestPropertyDefinitions);
				disposeGuard.Success();
			}
		}

		public ProgressInformation ProgressInformation
		{
			get
			{
				return new ProgressInformation(0, 10, 10, 1048576UL, 1048576UL);
			}
		}

		public static void CheckFlags(SyncFlag syncFlag, SyncExtraFlag extraFlags)
		{
			ContentsSynchronizer.GetManifestFlags(syncFlag, extraFlags);
		}

		public IEnumerator<IMessageChange> GetChanges()
		{
			base.CheckDisposed();
			ManifestItemChange change;
			while (this.manifestProvider.TryGetNextChange(out change))
			{
				IPropertyBag messageHeaderPropertyBag = new MemoryPropertyBag(this.SessionAdaptor);
				SynchronizerBase.SetPropertyValuesFromServer(messageHeaderPropertyBag, this.SyncRootFolder.ReferencedObject.Session, change.PropertyValues);
				StoreObjectId itemId = StoreObjectId.FromProviderSpecificId(messageHeaderPropertyBag.GetAnnotatedProperty(PropertyTag.EntryId).PropertyValue.GetValue<byte[]>());
				this.CheckAndFixMessageChanges(messageHeaderPropertyBag);
				IMessageChange messageChange = null;
				if (this.TryGetMessageChange(itemId, messageHeaderPropertyBag, out messageChange))
				{
					yield return messageChange;
				}
			}
			yield break;
		}

		public IPropertyBag GetDeletions()
		{
			base.CheckDisposed();
			IPropertyBag propertyBag = new MemoryPropertyBag(this.SessionAdaptor);
			ManifestItemDeletion manifestItemDeletion;
			while (this.manifestProvider.TryGetDeletion(out manifestItemDeletion))
			{
				PropertyTag propertyTag;
				if (manifestItemDeletion.IsExpired)
				{
					propertyTag = PropertyTag.IdsetExpired;
				}
				else if (manifestItemDeletion.IsSoftDeleted)
				{
					propertyTag = PropertyTag.IdsetSoftDeleted;
				}
				else
				{
					propertyTag = PropertyTag.IdsetDeleted;
				}
				propertyBag.SetProperty(new PropertyValue(propertyTag, manifestItemDeletion.IdsetDeleted));
			}
			return propertyBag;
		}

		public IPropertyBag GetReadUnreadStateChanges()
		{
			base.CheckDisposed();
			IPropertyBag propertyBag = new MemoryPropertyBag(this.SessionAdaptor);
			ManifestItemReadUnread manifestItemReadUnread;
			while (this.manifestProvider.TryGetReadUnread(out manifestItemReadUnread))
			{
				PropertyTag propertyTag = manifestItemReadUnread.IsRead ? PropertyTag.IdsetRead : PropertyTag.IdsetUnread;
				propertyBag.SetProperty(new PropertyValue(propertyTag, manifestItemReadUnread.IdsetReadUnread));
			}
			return propertyBag;
		}

		public IIcsState GetFinalState()
		{
			base.CheckDisposed();
			IPropertyBag propertyBag = new MemoryPropertyBag(this.SessionAdaptor);
			IcsStateStream icsStateStream = new IcsStateStream(propertyBag);
			StorageIcsState state = icsStateStream.ToXsoState();
			this.manifestProvider.GetFinalState(ref state);
			icsStateStream.FromXsoState(state);
			this.IcsState.Load(IcsStateOrigin.ServerFinal, propertyBag);
			return new IcsStateAdaptor(this.IcsState, this.SyncRootFolder);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.manifestProvider);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ContentsSynchronizer>(this);
		}

		private static ManifestConfigFlags GetManifestFlags(SyncFlag syncFlag, SyncExtraFlag extraFlags)
		{
			ManifestConfigFlags manifestConfigFlags = ManifestConfigFlags.None;
			if ((ushort)(syncFlag & (SyncFlag.NoConflicts | SyncFlag.Conversations | SyncFlag.MessageSelective)) != 0)
			{
				throw new RopExecutionException(string.Format("Unsupported SyncFlag present: {0}", syncFlag), (ErrorCode)2147746050U);
			}
			ushort num = (ushort)(syncFlag & SyncFlag.CatchUp);
			if ((ushort)(syncFlag & (SyncFlag.Associated | SyncFlag.Normal)) == 0)
			{
				throw new RopExecutionException(string.Format("One or both Normal or Associated should be specified: {0}", syncFlag), (ErrorCode)2147942487U);
			}
			SynchronizerBase.TranslateFlag(SyncFlag.NoDeletions, ManifestConfigFlags.NoDeletions, syncFlag, ref manifestConfigFlags);
			SynchronizerBase.TranslateFlag(SyncFlag.NoSoftDeletions, ManifestConfigFlags.NoSoftDeletions, syncFlag, ref manifestConfigFlags);
			SynchronizerBase.TranslateFlag(SyncFlag.ReadState, ManifestConfigFlags.ReadState, syncFlag, ref manifestConfigFlags);
			SynchronizerBase.TranslateFlag(SyncFlag.Associated, ManifestConfigFlags.Associated, syncFlag, ref manifestConfigFlags);
			SynchronizerBase.TranslateFlag(SyncFlag.Normal, ManifestConfigFlags.Normal, syncFlag, ref manifestConfigFlags);
			SynchronizerBase.TranslateFlag(SyncFlag.CatchUp, ManifestConfigFlags.Catchup, syncFlag, ref manifestConfigFlags);
			if ((extraFlags & (SyncExtraFlag.ManifestMode | SyncExtraFlag.CatchUpFull)) != SyncExtraFlag.None)
			{
				throw new RopExecutionException(string.Format("Unsupported SyncExtraFlag present: {0}", extraFlags), (ErrorCode)2147746050U);
			}
			if ((extraFlags & SyncExtraFlag.OrderByDeliveryTime) != SyncExtraFlag.None)
			{
				manifestConfigFlags |= ManifestConfigFlags.OrderByDeliveryTime;
			}
			if ((ushort)(syncFlag & SyncFlag.OnlySpecifiedProps) != 0)
			{
				Feature.Stubbed(65991, "return only specified properties");
			}
			if ((ushort)(syncFlag & SyncFlag.Unicode) == 0)
			{
				throw Feature.NotImplemented(212753, "codepage conversion");
			}
			if ((ushort)(syncFlag & SyncFlag.LimitedIMessage) != 0)
			{
				throw Feature.NotImplemented(62197, "don't return RTF bodies");
			}
			return manifestConfigFlags;
		}

		private void HonorOptionNoForeignKeys(IPropertyBag propertyBag)
		{
			if ((ushort)(this.SyncFlags & SyncFlag.NoForeignKeys) == 256)
			{
				propertyBag.SetProperty(new PropertyValue(PropertyTag.ExternalMid, base.ConvertIdToLongTermId(propertyBag, PropertyTag.Mid)));
			}
		}

		private void CheckAndFixMessageChanges(IPropertyBag propertyBag)
		{
			SynchronizerBase.CheckRequiredProperties(propertyBag, ContentsSynchronizer.RequiredInputProperties);
			this.HonorOptionNoForeignKeys(propertyBag);
		}

		private bool TryGetMessageChange(StoreObjectId itemId, IPropertyBag messageHeaderPropertyBag, out IMessageChange messageChange)
		{
			messageChange = null;
			bool result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreItem referencedObject;
				try
				{
					referencedObject = disposeGuard.Add<CoreItem>(CoreItem.Bind(this.SyncRootFolder.ReferencedObject.Session, itemId, CoreObjectSchema.AllPropertiesOnStore));
				}
				catch (ObjectNotFoundException)
				{
					return false;
				}
				DownloadBodyOption downloadBodyOption = ((ushort)(this.SyncFlags & SyncFlag.BestBody) == 8192) ? DownloadBodyOption.BestBodyOnly : DownloadBodyOption.RtfOnly;
				ReferenceCount<CoreItem> referenceCount = new ReferenceCount<CoreItem>(referencedObject);
				try
				{
					MessageAdaptor message = disposeGuard.Add<MessageAdaptor>(new MessageAdaptor(referenceCount, new MessageAdaptor.Options
					{
						IsReadOnly = true,
						IsEmbedded = false,
						DownloadBodyOption = downloadBodyOption
					}, this.string8Encoding, this.wantUnicode, null));
					MessageChangeAdaptor messageChangeAdaptor = new MessageChangeAdaptor(messageHeaderPropertyBag, message);
					disposeGuard.Add<MessageChangeAdaptor>(messageChangeAdaptor);
					disposeGuard.Success();
					messageChange = messageChangeAdaptor;
					result = true;
				}
				finally
				{
					referenceCount.Release();
				}
			}
			return result;
		}

		private static readonly PropertyTag[] MustRequestProperties = new PropertyTag[]
		{
			PropertyTag.ChangeNumber,
			PropertyTag.MessageSize
		};

		private static readonly PropertyDefinition[] MustRequestPropertyDefinitions = new PropertyDefinition[]
		{
			PropertyTagPropertyDefinition.CreateCustom("ChangeNumber", PropertyTag.ChangeNumber),
			CoreItemSchema.Size
		};

		private static readonly PropertyTag[] RequiredInputProperties = Util.MergeArrays<PropertyTag>(new ICollection<PropertyTag>[]
		{
			FastTransferMessageChange.AllMessageHeaderProperties,
			ContentsSynchronizer.MustRequestProperties,
			new PropertyTag[]
			{
				PropertyTag.Mid,
				PropertyTag.EntryId
			}
		});

		private readonly ContentManifestProvider manifestProvider;

		private readonly Encoding string8Encoding;

		private readonly bool wantUnicode;
	}
}
