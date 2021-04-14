using System;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PSTAttachment : IAttachment, IDisposable
	{
		public PSTAttachment(PSTMessage parentMessage, IAttachment iPstAttachment)
		{
			this.parentMessage = parentMessage;
			this.iPstAttachment = iPstAttachment;
			this.propertyBag = new PSTPropertyBag(parentMessage.PstMailbox, iPstAttachment.PropertyBag);
			PropertyValue property = this.propertyBag.GetProperty(PropertyTag.AttachmentNumber);
			if (property.IsError)
			{
				property = new PropertyValue(PropertyTag.AttachmentNumber, (int)iPstAttachment.AttachmentNumber);
			}
			this.propertyBag.SetProperty(property);
			this.attachmentNumber = (int)property.Value;
			if (iPstAttachment.Message != null)
			{
				this.embeddedMessage = new PSTMessage(parentMessage.PstMailbox, iPstAttachment.Message, true);
			}
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public bool IsEmbeddedMessage
		{
			get
			{
				return this.embeddedMessage != null;
			}
		}

		public int AttachmentNumber
		{
			get
			{
				PropertyValue propertyValue = this.propertyBag.GetAnnotatedProperty(PropertyTag.AttachmentNumber).PropertyValue;
				if (propertyValue.IsError)
				{
					return this.attachmentNumber;
				}
				return (int)propertyValue.Value;
			}
		}

		public IMessage GetEmbeddedMessage()
		{
			if (this.embeddedMessage == null)
			{
				IMessage iPstMessage = this.iPstAttachment.AddMessageAttachment();
				this.embeddedMessage = new PSTMessage(this.parentMessage.PstMailbox, iPstMessage, true);
			}
			return this.embeddedMessage;
		}

		public void Save()
		{
			try
			{
				this.iPstAttachment.Save();
			}
			catch (PSTExceptionBase innerException)
			{
				throw new UnableToCreatePSTMessagePermanentException(this.parentMessage.PstMailbox.IPst.FileName, innerException);
			}
		}

		public void Dispose()
		{
		}

		private PSTMessage parentMessage;

		private int attachmentNumber;

		private PSTPropertyBag propertyBag;

		private PSTMessage embeddedMessage;

		private IAttachment iPstAttachment;
	}
}
