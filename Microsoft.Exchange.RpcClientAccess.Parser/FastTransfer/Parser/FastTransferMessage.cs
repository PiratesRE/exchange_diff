using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferMessage : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public FastTransferMessage(IMessage message, FastTransferMessage.MessageType messageType, bool isTopLevel) : base(isTopLevel)
		{
			Util.ThrowOnNullArgument(message, "message");
			this.message = message;
			this.messageType = messageType;
		}

		private PropertyTag StartMarker
		{
			get
			{
				switch (this.messageType)
				{
				case FastTransferMessage.MessageType.Normal:
					return PropertyTag.StartMessage;
				case FastTransferMessage.MessageType.Associated:
					return PropertyTag.StartFAIMsg;
				case FastTransferMessage.MessageType.Embedded:
					return PropertyTag.StartEmbed;
				default:
					throw new InvalidOperationException(string.Format("Invalid MessageType {0}.", this.messageType));
				}
			}
		}

		private PropertyTag EndMarker
		{
			get
			{
				if (this.messageType != FastTransferMessage.MessageType.Embedded)
				{
					return PropertyTag.EndMessage;
				}
				return PropertyTag.EndEmbed;
			}
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			context.DataInterface.PutMarker(this.StartMarker);
			if (context.IsMovingMailbox)
			{
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, this.message.PropertyBag.GetAnnotatedProperty(PropertyTag.LongTermId).PropertyValue));
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, this.message.PropertyBag.GetAnnotatedProperty(PropertyTag.InstanceIdBin).PropertyValue));
			}
			else if (this.messageType == FastTransferMessage.MessageType.Embedded)
			{
				Feature.Stubbed(86205, "Need to serialize actual MID of embedded message.");
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, FastTransferMessage.EmptyMidPropertyValue));
			}
			else
			{
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, this.message.PropertyBag.GetAnnotatedProperty(PropertyTag.Mid).PropertyValue));
			}
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.message.PropertyBag, context.PropertyFilterFactory.GetMessageFilter(base.IsTopLevel))));
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferRecipientList(this.message)));
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferAttachmentList(this.message)));
			context.DataInterface.PutMarker(this.EndMarker);
			if (this.messageType != FastTransferMessage.MessageType.Embedded)
			{
				context.IncrementProgress();
			}
			yield break;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			context.DataInterface.ReadMarker(this.StartMarker);
			yield return null;
			if (context.IsMovingMailbox)
			{
				SingleMemberPropertyBag longTermIdPropertyBag = new SingleMemberPropertyBag(PropertyTag.LongTermId);
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, longTermIdPropertyBag));
				this.message.SetLongTermId(StoreLongTermId.Parse((byte[])longTermIdPropertyBag.PropertyValue.Value, true));
				SingleMemberPropertyBag instanceIdPropertyBag = new SingleMemberPropertyBag(PropertyTag.LongTermId);
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, instanceIdPropertyBag));
			}
			else if (this.messageType == FastTransferMessage.MessageType.Embedded)
			{
				Feature.Stubbed(86205, "Need to read and process actual MID if found to be needed.");
				yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, new SingleMemberPropertyBag(PropertyTag.Mid)));
			}
			PropertyTag propertyTag;
			for (;;)
			{
				if (context.DataInterface.TryPeekMarker(out propertyTag))
				{
					if (propertyTag == PropertyTag.NewAttach)
					{
						yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferAttachmentList(this.message)));
					}
					else
					{
						if (!(propertyTag == PropertyTag.StartRecip))
						{
							break;
						}
						yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferRecipientList(this.message)));
					}
				}
				else
				{
					if (FastTransferPropList.MetaProperties.Contains(propertyTag))
					{
						goto Block_6;
					}
					yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.message.PropertyBag)));
				}
			}
			context.DataInterface.ReadMarker(this.EndMarker);
			this.message.Save();
			yield break;
			Block_6:
			throw new RopExecutionException(string.Format("Unexpected meta-marker found: {0}.", propertyTag), ErrorCode.FxUnexpectedMarker);
		}

		protected override void InternalDispose()
		{
			this.message.Dispose();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferMessage>(this);
		}

		private static readonly PropertyValue EmptyMidPropertyValue = new PropertyValue(PropertyTag.Mid, 0L);

		private readonly IMessage message;

		private readonly FastTransferMessage.MessageType messageType;

		public enum MessageType
		{
			Normal,
			Associated,
			Embedded
		}
	}
}
