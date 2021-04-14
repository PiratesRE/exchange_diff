using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FastTransferAttachmentContent : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public FastTransferAttachmentContent(IAttachment attachment, bool isTopLevel) : base(isTopLevel)
		{
			this.attachment = attachment;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			base.CheckDisposed();
			IPropertyFilter handlerFilter = context.PropertyFilterFactory.GetAttachmentCopyToFilter(base.IsTopLevel);
			IPropertyFilter filter = handlerFilter;
			if (this.attachment.IsEmbeddedMessage)
			{
				filter = new AndPropertyFilter(new IPropertyFilter[]
				{
					handlerFilter,
					FastTransferAttachmentContent.ExcludeAttachmentDataObjectFilter
				});
			}
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.attachment.PropertyBag, filter)));
			if (this.attachment.IsEmbeddedMessage && handlerFilter.IncludeProperty(PropertyTag.AttachmentDataObject))
			{
				using (IMessage embeddedMessage = this.attachment.GetEmbeddedMessage())
				{
					yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferMessage(embeddedMessage, FastTransferMessage.MessageType.Embedded, false)));
				}
			}
			yield break;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			base.CheckDisposed();
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.attachment.PropertyBag)));
			PropertyTag marker;
			if (!context.NoMoreData && context.DataInterface.TryPeekMarker(out marker) && marker == PropertyTag.StartEmbed)
			{
				using (IMessage embeddedMessage = this.attachment.GetEmbeddedMessage())
				{
					yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferMessage(embeddedMessage, FastTransferMessage.MessageType.Embedded, false)));
				}
			}
			yield break;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferAttachmentContent>(this);
		}

		protected override void InternalDispose()
		{
			this.attachment.Dispose();
			base.InternalDispose();
		}

		private static readonly IPropertyFilter ExcludeAttachmentDataObjectFilter = new ExcludingPropertyFilter(new PropertyTag[]
		{
			PropertyTag.AttachmentDataObject
		});

		private readonly IAttachment attachment;
	}
}
