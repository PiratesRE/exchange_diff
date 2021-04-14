using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferMessageIterator : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public FastTransferMessageIterator(IMessageIterator messageIterator, FastTransferCopyMessagesFlag options, bool isTopLevel) : base(isTopLevel)
		{
			Util.ThrowOnNullArgument(messageIterator, "messageIterator");
			this.messageIterator = messageIterator;
		}

		public FastTransferMessageIterator(IMessageIteratorClient messageIteratorClient, bool isTopLevel) : base(isTopLevel)
		{
			Util.ThrowOnNullArgument(messageIteratorClient, "messageIteratorClient");
			this.messageIteratorClient = messageIteratorClient;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			yield return new FastTransferStateMachine?(FastTransferPropertyValue.SkipPropertyIfExists(context, PropertyTag.DNPrefix));
			yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.ParseMessages(context)));
			yield break;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			return this.SerializeMessages(context);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.messageIterator);
			Util.DisposeIfPresent(this.messageIteratorClient);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferMessageIterator>(this);
		}

		private IEnumerator<FastTransferStateMachine?> ParseMessages(FastTransferUploadContext context)
		{
			while (!context.NoMoreData)
			{
				PropertyTag propertyTag;
				if (context.DataInterface.TryPeekMarker(out propertyTag))
				{
					if (!(propertyTag == PropertyTag.StartMessage) && !(propertyTag == PropertyTag.StartFAIMsg))
					{
						break;
					}
					bool isAssociatedMessage = propertyTag == PropertyTag.StartFAIMsg;
					FastTransferMessage.MessageType messageType = (!isAssociatedMessage) ? FastTransferMessage.MessageType.Normal : FastTransferMessage.MessageType.Associated;
					using (IMessage message = this.messageIteratorClient.UploadMessage(isAssociatedMessage))
					{
						yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferMessage(message, messageType, base.IsTopLevel)));
					}
				}
				else
				{
					if (!propertyTag.IsMetaProperty)
					{
						break;
					}
					yield return new FastTransferStateMachine?(FastTransferPropertyValue.SkipPropertyIfExists(context, propertyTag));
				}
			}
			yield break;
		}

		private IEnumerator<FastTransferStateMachine?> SerializeMessages(FastTransferDownloadContext context)
		{
			using (IEnumerator<IMessage> messages = this.messageIterator.GetMessages())
			{
				while (messages.MoveNext())
				{
					IMessage message = messages.Current;
					if (message != null)
					{
						FastTransferMessage fastTransferMessage = this.CreateFastTransferMessage(message);
						yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferMessage));
					}
					else
					{
						yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, FastTransferMessageIterator.partialCompletionWarningPropertyValue));
					}
				}
			}
			yield break;
		}

		private FastTransferMessage CreateFastTransferMessage(IMessage message)
		{
			FastTransferMessage result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<IMessage>(message);
				FastTransferMessage.MessageType messageType = message.IsAssociated ? FastTransferMessage.MessageType.Associated : FastTransferMessage.MessageType.Normal;
				FastTransferMessage fastTransferMessage = new FastTransferMessage(message, messageType, base.IsTopLevel);
				disposeGuard.Add<FastTransferMessage>(fastTransferMessage);
				disposeGuard.Success();
				result = fastTransferMessage;
			}
			return result;
		}

		private static readonly PropertyValue partialCompletionWarningPropertyValue = new PropertyValue(PropertyTag.EcWarning, 263808);

		private readonly IMessageIterator messageIterator;

		private readonly IMessageIteratorClient messageIteratorClient;
	}
}
