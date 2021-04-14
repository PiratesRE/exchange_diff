using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferMessageCopyTo : FastTransferCopyTo
	{
		public FastTransferMessageCopyTo(bool isShallowCopy, IMessage message, bool isTopLevel) : base(isShallowCopy, message.PropertyBag, isTopLevel)
		{
			Util.ThrowOnNullArgument(message, "message");
			this.message = message;
		}

		protected override IPropertyFilter GetDownloadPropertiesFilter(FastTransferDownloadContext context)
		{
			return context.PropertyFilterFactory.GetMessageCopyToFilter(base.IsTopLevel);
		}

		protected override IEnumerator<FastTransferStateMachine?> DownloadContents(FastTransferDownloadContext context)
		{
			if (!base.IsShallowCopy)
			{
				IMessagePropertyFilter messageCopyToFilter = context.PropertyFilterFactory.GetMessageCopyToFilter(base.IsTopLevel);
				return FastTransferMessageChange.SerializeRecipientsAndAttachments(context, this.message, messageCopyToFilter.IncludeRecipients, messageCopyToFilter.IncludeAttachments);
			}
			return null;
		}

		protected override IEnumerator<FastTransferStateMachine?> UploadContents(FastTransferUploadContext context)
		{
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(FastTransferMessageChange.ParseRecipientsAndAttachments(context, this.message)));
			yield break;
		}

		protected override void InternalDispose()
		{
			this.message.Dispose();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferMessageCopyTo>(this);
		}

		private readonly IMessage message;
	}
}
