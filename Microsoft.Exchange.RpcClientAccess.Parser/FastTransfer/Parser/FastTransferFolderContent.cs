using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FastTransferFolderContent : FastTransferFolderContentBase, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public FastTransferFolderContent(IFolder folder, FastTransferFolderContentBase.IncludeSubObject includeSubObject, bool isTopLevel) : base(folder, includeSubObject, isTopLevel)
		{
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferFolderContent>(this);
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			base.CheckDisposed();
			if (!base.IsTopLevel)
			{
				yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.DownloadRequiredProperties(context)));
			}
			IPropertyFilter filter = base.IsTopLevel ? context.PropertyFilterFactory.GetCopyFolderFilter() : context.PropertyFilterFactory.GetCopySubfolderFilter();
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(base.DownloadProperties(context, filter)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(base.DownloadSubObjects(context, false)));
			context.IncrementProgress();
			yield break;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			base.CheckDisposed();
			if (!base.IsTopLevel)
			{
				yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.UploadRequiredProperties(context)));
			}
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(base.UploadProperties(context)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(base.UploadMessages(context)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(base.UploadSubfolders(context)));
			yield break;
		}

		private IEnumerator<FastTransferStateMachine?> DownloadRequiredProperties(FastTransferDownloadContext context)
		{
			yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, base.Folder.PropertyBag, base.Folder.PropertyBag.GetAnnotatedProperty(PropertyTag.Fid)));
			Feature.Stubbed(15505, "Support code page and string conversion.");
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(base.Folder.PropertyBag, new PropertyTag[]
			{
				PropertyTag.DisplayName,
				PropertyTag.Comment
			})));
			yield break;
		}

		private IEnumerator<FastTransferStateMachine?> UploadRequiredProperties(FastTransferUploadContext context)
		{
			yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, base.Folder.PropertyBag));
			PropertyValue fid = base.Folder.PropertyBag.GetAnnotatedProperty(PropertyTag.Fid).PropertyValue;
			if (fid.IsError)
			{
				throw new RopExecutionException(string.Format("The FID should have been downloaded in the first place. FID = {0}.", fid), (ErrorCode)2147942487U);
			}
			yield break;
		}
	}
}
