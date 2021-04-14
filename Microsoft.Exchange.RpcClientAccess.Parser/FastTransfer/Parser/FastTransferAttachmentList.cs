using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferAttachmentList : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public FastTransferAttachmentList(IMessage message) : base(false)
		{
			Util.ThrowOnNullArgument(message, "message");
			this.message = message;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			foreach (IAttachmentHandle handle in this.message.GetAttachments())
			{
				FastTransferAttachment fastTransferAttachment = this.CreateDownloadFastTransferAttachment(handle);
				yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferAttachment));
			}
			yield break;
		}

		private FastTransferAttachment CreateDownloadFastTransferAttachment(IAttachmentHandle handle)
		{
			FastTransferAttachment result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IAttachment attachment = handle.GetAttachment();
				disposeGuard.Add<IAttachment>(attachment);
				FastTransferAttachment fastTransferAttachment = new FastTransferAttachment(attachment, false);
				disposeGuard.Add<FastTransferAttachment>(fastTransferAttachment);
				disposeGuard.Success();
				result = fastTransferAttachment;
			}
			return result;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			PropertyTag marker = default(PropertyTag);
			while (!context.NoMoreData && context.DataInterface.TryPeekMarker(out marker) && marker == PropertyTag.NewAttach)
			{
				FastTransferAttachment fastTransferAttachment = this.CreateUploadFastTransferAttachment();
				yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferAttachment));
			}
			yield break;
		}

		private FastTransferAttachment CreateUploadFastTransferAttachment()
		{
			FastTransferAttachment result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IAttachment attachment = this.message.CreateAttachment();
				disposeGuard.Add<IAttachment>(attachment);
				FastTransferAttachment fastTransferAttachment = new FastTransferAttachment(attachment, false);
				disposeGuard.Add<FastTransferAttachment>(fastTransferAttachment);
				disposeGuard.Success();
				result = fastTransferAttachment;
			}
			return result;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferAttachmentList>(this);
		}

		private readonly IMessage message;
	}
}
