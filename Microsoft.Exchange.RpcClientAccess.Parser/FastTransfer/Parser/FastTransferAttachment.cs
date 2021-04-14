using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferAttachment : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public FastTransferAttachment(IAttachment attachment, bool isTopLevel) : base(isTopLevel)
		{
			Util.ThrowOnNullArgument(attachment, "attachment");
			this.attachment = attachment;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			context.DataInterface.PutMarker(PropertyTag.NewAttach);
			PropertyValue attachmentNumber = this.attachment.PropertyBag.GetAnnotatedProperty(PropertyTag.AttachmentNumber).PropertyValue;
			if (attachmentNumber.IsError)
			{
				throw new NotSupportedException("Found an attachment without an attachment number.");
			}
			yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, attachmentNumber));
			if (context.IsMovingMailbox)
			{
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, this.attachment.PropertyBag.GetAnnotatedProperty(PropertyTag.InstanceIdBin).PropertyValue));
			}
			IPropertyFilter filter;
			if (this.attachment.IsEmbeddedMessage)
			{
				filter = context.PropertyFilterFactory.GetEmbeddedMessageFilter(false);
			}
			else
			{
				filter = context.PropertyFilterFactory.GetAttachmentFilter(base.IsTopLevel);
			}
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.attachment.PropertyBag, filter)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.SerializeEmbeddedMessage(context)));
			context.DataInterface.PutMarker(PropertyTag.EndAttach);
			yield break;
		}

		private IEnumerator<FastTransferStateMachine?> SerializeEmbeddedMessage(FastTransferDownloadContext context)
		{
			if (this.attachment.IsEmbeddedMessage)
			{
				FastTransferMessage fastTransferMessage = this.CreateDownloadFastTransferEmbeddedMessage();
				yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferMessage));
			}
			yield break;
		}

		private FastTransferMessage CreateDownloadFastTransferEmbeddedMessage()
		{
			FastTransferMessage result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IMessage embeddedMessage = this.attachment.GetEmbeddedMessage();
				disposeGuard.Add<IMessage>(embeddedMessage);
				FastTransferMessage fastTransferMessage = new FastTransferMessage(embeddedMessage, FastTransferMessage.MessageType.Embedded, false);
				disposeGuard.Add<FastTransferMessage>(fastTransferMessage);
				disposeGuard.Success();
				result = fastTransferMessage;
			}
			return result;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			context.DataInterface.ReadMarker(PropertyTag.NewAttach);
			SingleMemberPropertyBag singleMemberPropertyBag = new SingleMemberPropertyBag(PropertyTag.AttachmentNumber);
			yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, singleMemberPropertyBag));
			int attachmentNumber = this.attachment.AttachmentNumber;
			singleMemberPropertyBag.PropertyValue.GetValue<int>();
			this.attachment.PropertyBag.SetProperty(singleMemberPropertyBag.PropertyValue);
			if (context.IsMovingMailbox)
			{
				SingleMemberPropertyBag longTermIdPropertyBag = new SingleMemberPropertyBag(PropertyTag.LongTermId);
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, longTermIdPropertyBag));
			}
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.attachment.PropertyBag)));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.ParseEmbeddedMessage(context)));
			context.DataInterface.ReadMarker(PropertyTag.EndAttach);
			this.attachment.Save();
			yield break;
		}

		private IEnumerator<FastTransferStateMachine?> ParseEmbeddedMessage(FastTransferUploadContext context)
		{
			PropertyTag marker;
			if (context.DataInterface.TryPeekMarker(out marker) && marker == PropertyTag.StartEmbed)
			{
				FastTransferMessage fastTransferMessage = this.CreateUploadFastTransferEmbeddedMessage();
				yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferMessage));
			}
			yield break;
		}

		private FastTransferMessage CreateUploadFastTransferEmbeddedMessage()
		{
			FastTransferMessage result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IMessage embeddedMessage = this.attachment.GetEmbeddedMessage();
				disposeGuard.Add<IMessage>(embeddedMessage);
				FastTransferMessage fastTransferMessage = new FastTransferMessage(embeddedMessage, FastTransferMessage.MessageType.Embedded, false);
				disposeGuard.Add<FastTransferMessage>(fastTransferMessage);
				disposeGuard.Success();
				result = fastTransferMessage;
			}
			return result;
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.attachment);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferAttachment>(this);
		}

		private readonly IAttachment attachment;
	}
}
