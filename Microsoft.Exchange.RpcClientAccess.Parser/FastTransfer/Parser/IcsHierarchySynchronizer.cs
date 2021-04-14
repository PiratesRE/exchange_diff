using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	internal sealed class IcsHierarchySynchronizer : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public IcsHierarchySynchronizer(IHierarchySynchronizer hierarchySynchronizer, IHierarchySynchronizerClient hierarchySynchronizerClient, IcsHierarchySynchronizer.Options options) : base(true)
		{
			this.hierarchySynchronizer = hierarchySynchronizer;
			this.hierarchySynchronizerClient = hierarchySynchronizerClient;
			this.options = options;
		}

		public IcsHierarchySynchronizer(IHierarchySynchronizer hierarchySynchronizer, IcsHierarchySynchronizer.Options options) : this(hierarchySynchronizer, null, options)
		{
			Util.ThrowOnNullArgument(hierarchySynchronizer, "hierarchySynchronizer");
		}

		internal IcsHierarchySynchronizer(IHierarchySynchronizerClient hierarchySynchronizerClient, IcsHierarchySynchronizer.Options options) : this(null, hierarchySynchronizerClient, options)
		{
			Util.ThrowOnNullArgument(hierarchySynchronizerClient, "hierarchySynchronizerClient");
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			using (IEnumerator<IFolderChange> changes = this.hierarchySynchronizer.GetChanges())
			{
				bool includeFid = (this.options & IcsHierarchySynchronizer.Options.IncludeFid) == IcsHierarchySynchronizer.Options.IncludeFid;
				bool includeChangeNumber = (this.options & IcsHierarchySynchronizer.Options.IncludeChangeNumber) == IcsHierarchySynchronizer.Options.IncludeChangeNumber;
				IPropertyFilter propertyFilter = context.PropertyFilterFactory.GetIcsHierarchyFilter(includeFid, includeChangeNumber);
				while (changes.MoveNext())
				{
					using (IFolderChange currentFolderChange = changes.Current)
					{
						context.DataInterface.PutMarker(PropertyTag.IncrSyncChg);
						yield return null;
						using (IEnumerator<PropertyTag> enumerator = currentFolderChange.FolderPropertyBag.WithNoValue(this.FilterPropertyList(IcsHierarchySynchronizer.RequiredFolderChangeProperties, propertyFilter)).GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								PropertyTag propertyTag = enumerator.Current;
								throw new RopExecutionException(string.Format("Required folder change property {0} is missing", propertyTag), (ErrorCode)2147942487U);
							}
						}
						yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(currentFolderChange.FolderPropertyBag, propertyFilter)));
					}
				}
			}
			IPropertyBag deletions = this.hierarchySynchronizer.GetDeletions();
			AnnotatedPropertyValue idSetDeletedValue = deletions.GetAnnotatedProperty(PropertyTag.IdsetDeleted);
			if (!idSetDeletedValue.PropertyValue.IsNotFound)
			{
				context.DataInterface.PutMarker(PropertyTag.IncrSyncDel);
				yield return null;
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, deletions, idSetDeletedValue));
			}
			FastTransferIcsState state = this.CreateDownloadFinalState();
			yield return new FastTransferStateMachine?(context.CreateStateMachine(state));
			context.DataInterface.PutMarker(PropertyTag.IncrSyncEnd);
			yield break;
		}

		private FastTransferIcsState CreateDownloadFinalState()
		{
			FastTransferIcsState result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IIcsState finalState = this.hierarchySynchronizer.GetFinalState();
				disposeGuard.Add<IIcsState>(finalState);
				FastTransferIcsState fastTransferIcsState = new FastTransferIcsState(finalState);
				disposeGuard.Add<FastTransferIcsState>(fastTransferIcsState);
				disposeGuard.Success();
				result = fastTransferIcsState;
			}
			return result;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			PropertyTag propertyTag;
			context.DataInterface.TryPeekMarker(out propertyTag);
			while (propertyTag == PropertyTag.IncrSyncChg)
			{
				context.DataInterface.ReadMarker(PropertyTag.IncrSyncChg);
				yield return null;
				IPropertyBag folderChanges = this.hierarchySynchronizerClient.LoadFolderChanges();
				do
				{
					yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, folderChanges));
				}
				while (!context.DataInterface.TryPeekMarker(out propertyTag));
			}
			IPropertyBag folderDeletion = this.hierarchySynchronizerClient.LoadFolderDeletion();
			if (propertyTag == PropertyTag.IncrSyncDel)
			{
				context.DataInterface.ReadMarker(PropertyTag.IncrSyncDel);
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, folderDeletion));
			}
			FastTransferIcsState state = this.CreateUploadFinalState();
			yield return new FastTransferStateMachine?(context.CreateStateMachine(state));
			context.DataInterface.ReadMarker(PropertyTag.IncrSyncEnd);
			yield break;
		}

		private FastTransferIcsState CreateUploadFinalState()
		{
			FastTransferIcsState result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IIcsState icsState = this.hierarchySynchronizerClient.LoadFinalState();
				disposeGuard.Add<IIcsState>(icsState);
				FastTransferIcsState fastTransferIcsState = new FastTransferIcsState(icsState);
				disposeGuard.Add<FastTransferIcsState>(fastTransferIcsState);
				disposeGuard.Success();
				result = fastTransferIcsState;
			}
			return result;
		}

		private IEnumerable<PropertyTag> FilterPropertyList(IEnumerable<PropertyTag> propertyTags, IPropertyFilter propertyFilter)
		{
			return from propertyTag in propertyTags
			where propertyFilter.IncludeProperty(propertyTag)
			select propertyTag;
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.hierarchySynchronizer);
			Util.DisposeIfPresent(this.hierarchySynchronizerClient);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IcsHierarchySynchronizer>(this);
		}

		private readonly IHierarchySynchronizer hierarchySynchronizer;

		private readonly IHierarchySynchronizerClient hierarchySynchronizerClient;

		private readonly IcsHierarchySynchronizer.Options options;

		public static readonly ReadOnlyCollection<PropertyTag> RequiredFolderChangeProperties = new ReadOnlyCollection<PropertyTag>(new PropertyTag[]
		{
			PropertyTag.ExternalParentFid,
			PropertyTag.ExternalFid,
			PropertyTag.LastModificationTime,
			PropertyTag.ExternalChangeNumber,
			PropertyTag.ExternalPredecessorChangeList,
			PropertyTag.DisplayName,
			PropertyTag.Fid
		});

		[Flags]
		internal enum Options
		{
			None = 0,
			IncludeFid = 1,
			IncludeChangeNumber = 2
		}
	}
}
