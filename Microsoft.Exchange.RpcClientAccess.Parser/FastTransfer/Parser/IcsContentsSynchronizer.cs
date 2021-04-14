using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class IcsContentsSynchronizer : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public IcsContentsSynchronizer(IContentsSynchronizer contentsSynchronizer, IcsContentsSynchronizer.Options options) : this(contentsSynchronizer, null, options, null)
		{
			Util.ThrowOnNullArgument(contentsSynchronizer, "contentsSynchronizer");
		}

		public IcsContentsSynchronizer(IContentsSynchronizer contentsSynchronizer, IcsContentsSynchronizer.Options options, IEnumerable<PropertyTag> additionalHeaderProperties) : this(contentsSynchronizer, null, options, additionalHeaderProperties)
		{
			Util.ThrowOnNullArgument(contentsSynchronizer, "contentsSynchronizer");
		}

		internal IcsContentsSynchronizer(IContentsSynchronizerClient contentsSynchronizerClient, IcsContentsSynchronizer.Options options) : this(null, contentsSynchronizerClient, options, null)
		{
			Util.ThrowOnNullArgument(contentsSynchronizerClient, "contentsSynchronizerClient");
		}

		private IcsContentsSynchronizer(IContentsSynchronizer contentsSynchronizer, IContentsSynchronizerClient contentsSynchronizerClient, IcsContentsSynchronizer.Options options, IEnumerable<PropertyTag> additionalHeaderProperties) : base(true)
		{
			this.contentsSynchronizer = contentsSynchronizer;
			this.contentsSynchronizerClient = contentsSynchronizerClient;
			this.options = options;
			this.additionalHeaderProperties = additionalHeaderProperties;
			if ((this.contentsSynchronizer != null && (this.options & IcsContentsSynchronizer.Options.PartialItem) == IcsContentsSynchronizer.Options.PartialItem && (this.options & IcsContentsSynchronizer.Options.ManifestMode) == IcsContentsSynchronizer.Options.None) || this.contentsSynchronizerClient != null)
			{
				this.partialItemState = new IcsPartialItemState();
			}
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.SerializeProgressInfo(context)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.SerializeMessageChanges(context)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(IcsContentsSynchronizer.SerializeFixedPropertyBag(context, this.contentsSynchronizer.GetDeletions(), PropertyTag.IncrSyncDel, IcsContentsSynchronizer.deletionsPropertyTags)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(IcsContentsSynchronizer.SerializeFixedPropertyBag(context, this.contentsSynchronizer.GetReadUnreadStateChanges(), PropertyTag.IncrSyncRead, IcsContentsSynchronizer.readUnreadStateChangesPropertyTags)));
			FastTransferIcsState state = this.CreateDownloadFinalState();
			yield return new FastTransferStateMachine?(context.CreateStateMachine(state));
			context.DataInterface.PutMarker(PropertyTag.IncrSyncEnd);
			yield break;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.ParseProgressInformation(context)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.ParseMessageChanges(context)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(IcsContentsSynchronizer.ParsePropertyBag(context, this.contentsSynchronizerClient.LoadDeletionPropertyBag(), PropertyTag.IncrSyncDel)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(IcsContentsSynchronizer.ParsePropertyBag(context, this.contentsSynchronizerClient.LoadReadUnreadPropertyBag(), PropertyTag.IncrSyncRead)));
			FastTransferIcsState state = this.CreateUploadFinalState();
			yield return new FastTransferStateMachine?(context.CreateStateMachine(state));
			context.DataInterface.ReadMarker(PropertyTag.IncrSyncEnd);
			yield break;
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.contentsSynchronizer);
			Util.DisposeIfPresent(this.contentsSynchronizerClient);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IcsContentsSynchronizer>(this);
		}

		private static IEnumerator<FastTransferStateMachine?> SerializeFixedPropertyBag(FastTransferDownloadContext context, IPropertyBag propertyBag, PropertyTag beginMarker, IList<PropertyTag> propertyTagsSuperset)
		{
			if (!propertyBag.GetAnnotatedProperties().IsEmpty<AnnotatedPropertyValue>())
			{
				context.DataInterface.PutMarker(beginMarker);
				yield return null;
				yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(propertyBag, propertyTagsSuperset)
				{
					SkipPropertyError = true
				}));
			}
			yield break;
		}

		private static IEnumerator<FastTransferStateMachine?> ParsePropertyBag(FastTransferUploadContext context, IPropertyBag propertyBag, PropertyTag beginMarker)
		{
			PropertyTag marker = default(PropertyTag);
			context.DataInterface.TryPeekMarker(out marker);
			if (marker == beginMarker)
			{
				context.DataInterface.ReadMarker(beginMarker);
				yield return null;
				yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(propertyBag)));
			}
			yield break;
		}

		private FastTransferIcsState CreateDownloadFinalState()
		{
			FastTransferIcsState result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IIcsState finalState = this.contentsSynchronizer.GetFinalState();
				disposeGuard.Add<IIcsState>(finalState);
				FastTransferIcsState fastTransferIcsState = new FastTransferIcsState(finalState);
				disposeGuard.Add<FastTransferIcsState>(fastTransferIcsState);
				disposeGuard.Success();
				result = fastTransferIcsState;
			}
			return result;
		}

		private IEnumerator<FastTransferStateMachine?> SerializeProgressInfo(FastTransferDownloadContext context)
		{
			if ((this.options & IcsContentsSynchronizer.Options.ProgressMode) == IcsContentsSynchronizer.Options.ProgressMode)
			{
				context.DataInterface.PutMarker(PropertyTag.IncrSyncProgressMode);
				yield return null;
				byte[] buffer = new byte[32];
				PropertyValue progressInformation;
				using (Writer writer = new BufferWriter(buffer))
				{
					this.contentsSynchronizer.ProgressInformation.Serialize(writer);
					progressInformation = new PropertyValue(IcsContentsSynchronizer.progressInformationPropertyTag, buffer);
				}
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, progressInformation));
			}
			yield break;
		}

		private IEnumerator<FastTransferStateMachine?> SerializeMessageChanges(FastTransferDownloadContext context)
		{
			using (IEnumerator<IMessageChange> messageChanges = this.contentsSynchronizer.GetChanges())
			{
				while (messageChanges.MoveNext())
				{
					IMessageChange messageChange = messageChanges.Current;
					FastTransferMessageChange fastTransferMessageChange = this.CreateDownloadFastTransferMessageChange(messageChange);
					yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferMessageChange));
				}
			}
			yield break;
		}

		private FastTransferMessageChange CreateDownloadFastTransferMessageChange(IMessageChange messageChange)
		{
			FastTransferMessageChange result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<IMessageChange>(messageChange);
				FastTransferMessageChange fastTransferMessageChange = new FastTransferMessageChange(messageChange, this.options, this.additionalHeaderProperties, this.partialItemState, base.IsTopLevel);
				disposeGuard.Add<FastTransferMessageChange>(fastTransferMessageChange);
				disposeGuard.Success();
				result = fastTransferMessageChange;
			}
			return result;
		}

		private FastTransferIcsState CreateUploadFinalState()
		{
			FastTransferIcsState result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IIcsState icsState = this.contentsSynchronizerClient.LoadFinalState();
				disposeGuard.Add<IIcsState>(icsState);
				FastTransferIcsState fastTransferIcsState = new FastTransferIcsState(icsState);
				disposeGuard.Add<FastTransferIcsState>(fastTransferIcsState);
				disposeGuard.Success();
				result = fastTransferIcsState;
			}
			return result;
		}

		private IEnumerator<FastTransferStateMachine?> ParseProgressInformation(FastTransferUploadContext context)
		{
			PropertyTag marker = default(PropertyTag);
			context.DataInterface.TryPeekMarker(out marker);
			if (marker == PropertyTag.IncrSyncProgressMode)
			{
				context.DataInterface.ReadMarker(PropertyTag.IncrSyncProgressMode);
				yield return null;
				SingleMemberPropertyBag singleMemberPropertyBag = new SingleMemberPropertyBag(IcsContentsSynchronizer.progressInformationPropertyTag);
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, singleMemberPropertyBag));
				using (Reader reader = Reader.CreateBufferReader(singleMemberPropertyBag.PropertyValue.GetValue<byte[]>()))
				{
					this.contentsSynchronizerClient.SetProgressInformation(ProgressInformation.Parse(reader));
					yield break;
				}
			}
			yield break;
		}

		private IEnumerator<FastTransferStateMachine?> ParseMessageChanges(FastTransferUploadContext context)
		{
			PropertyTag marker = default(PropertyTag);
			while (context.DataInterface.TryPeekMarker(out marker) && FastTransferMessageChange.IsMessageBeginMarker(marker))
			{
				FastTransferMessageChange fastTransferMessageChange = this.CreateUploadFastTransferMessageChange();
				yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferMessageChange));
			}
			yield break;
		}

		private FastTransferMessageChange CreateUploadFastTransferMessageChange()
		{
			FastTransferMessageChange result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IMessageChangeClient messageChangeClient = this.contentsSynchronizerClient.UploadMessageChange();
				disposeGuard.Add<IMessageChangeClient>(messageChangeClient);
				FastTransferMessageChange fastTransferMessageChange = new FastTransferMessageChange(messageChangeClient, this.options, this.partialItemState, base.IsTopLevel);
				disposeGuard.Add<FastTransferMessageChange>(fastTransferMessageChange);
				disposeGuard.Success();
				result = fastTransferMessageChange;
			}
			return result;
		}

		private static readonly PropertyTag progressInformationPropertyTag = new PropertyTag(PropertyId.Null, PropertyType.Binary);

		private static readonly PropertyTag[] deletionsPropertyTags = new PropertyTag[]
		{
			PropertyTag.IdsetDeleted,
			PropertyTag.IdsetSoftDeleted,
			PropertyTag.IdsetExpired
		};

		private static readonly PropertyTag[] readUnreadStateChangesPropertyTags = new PropertyTag[]
		{
			PropertyTag.IdsetRead,
			PropertyTag.IdsetUnread
		};

		private readonly IContentsSynchronizer contentsSynchronizer;

		private readonly IContentsSynchronizerClient contentsSynchronizerClient;

		private readonly IcsContentsSynchronizer.Options options;

		private readonly IEnumerable<PropertyTag> additionalHeaderProperties;

		private readonly IcsPartialItemState partialItemState;

		[Flags]
		internal enum Options
		{
			None = 0,
			ProgressMode = 1,
			IncludeMid = 2,
			IncludeMessageSize = 4,
			IncludeChangeNumber = 8,
			ManifestMode = 16,
			PartialItem = 32,
			Conversations = 64,
			IncludeReadChangeNumber = 128
		}
	}
}
