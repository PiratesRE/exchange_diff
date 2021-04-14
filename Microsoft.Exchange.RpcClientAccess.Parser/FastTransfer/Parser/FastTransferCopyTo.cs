using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class FastTransferCopyTo : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		protected FastTransferCopyTo(bool isShallowCopy, IPropertyBag propertyBag, bool isTopLevel) : base(isTopLevel)
		{
			this.isShallowCopy = isShallowCopy;
			this.propertyBag = propertyBag;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.DownloadProperties(context)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.DownloadContents(context)));
			yield break;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.UploadProperties(context)));
			if (!context.NoMoreData)
			{
				yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.UploadContents(context)));
			}
			yield break;
		}

		protected abstract IPropertyFilter GetDownloadPropertiesFilter(FastTransferDownloadContext context);

		protected virtual IEnumerator<FastTransferStateMachine?> DownloadProperties(FastTransferDownloadContext context)
		{
			IPropertyFilter filter = this.GetDownloadPropertiesFilter(context);
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.propertyBag, filter)));
			yield break;
		}

		protected abstract IEnumerator<FastTransferStateMachine?> DownloadContents(FastTransferDownloadContext context);

		protected bool IsShallowCopy
		{
			get
			{
				return this.isShallowCopy;
			}
		}

		protected virtual IEnumerator<FastTransferStateMachine?> UploadProperties(FastTransferUploadContext context)
		{
			yield return new FastTransferStateMachine?(FastTransferPropertyValue.SkipPropertyIfExists(context, PropertyTag.DNPrefix));
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.propertyBag)));
			yield break;
		}

		protected abstract IEnumerator<FastTransferStateMachine?> UploadContents(FastTransferUploadContext context);

		private readonly IPropertyBag propertyBag;

		private readonly bool isShallowCopy;
	}
}
