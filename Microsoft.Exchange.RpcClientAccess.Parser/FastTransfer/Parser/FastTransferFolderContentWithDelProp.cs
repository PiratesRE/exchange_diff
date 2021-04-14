using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FastTransferFolderContentWithDelProp : FastTransferFolderContentBase, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public FastTransferFolderContentWithDelProp(IFolder folder, FastTransferFolderContentBase.IncludeSubObject includeSubObject) : base(folder, includeSubObject, true)
		{
		}

		public FastTransferFolderContentWithDelProp(IFolder folder) : base(folder, FastTransferFolderContentBase.IncludeSubObject.All, true)
		{
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferFolderContentWithDelProp>(this);
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			base.CheckDisposed();
			IPropertyFilter filter = context.PropertyFilterFactory.GetFolderCopyToFilter();
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(base.DownloadProperties(context, filter)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(base.DownloadSubObjects(context, true)));
			context.IncrementProgress();
			yield break;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			base.CheckDisposed();
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(base.UploadProperties(context)));
			while (!context.NoMoreData)
			{
				PropertyTag markerOrMetaProperty;
				if (context.DataInterface.TryPeekMarker(out markerOrMetaProperty))
				{
					if (markerOrMetaProperty == PropertyTag.StartMessage || markerOrMetaProperty == PropertyTag.StartFAIMsg)
					{
						yield return new FastTransferStateMachine?(new FastTransferStateMachine(base.UploadMessages(context)));
					}
					else
					{
						if (!(markerOrMetaProperty == PropertyTag.StartSubFld))
						{
							break;
						}
						yield return new FastTransferStateMachine?(new FastTransferStateMachine(base.UploadSubfolders(context)));
					}
				}
				else
				{
					if (!(markerOrMetaProperty == PropertyTag.FXDelProp) && !(markerOrMetaProperty == PropertyTag.DNPrefix) && !(markerOrMetaProperty == PropertyTag.NewFXFolder))
					{
						throw new RopExecutionException(string.Format("Property {0} was not expected", markerOrMetaProperty), ErrorCode.FxUnexpectedMarker);
					}
					yield return new FastTransferStateMachine?(FastTransferPropertyValue.SkipPropertyIfExists(context, markerOrMetaProperty));
				}
			}
			yield break;
		}
	}
}
