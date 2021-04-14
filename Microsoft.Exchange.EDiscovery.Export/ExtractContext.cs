using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.PST;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class ExtractContext
	{
		public ExtractContext(IPST pstSession, ItemInformation item)
		{
			this.containerStack = new Stack<ExtractContext.PropertyBagContainer>();
			this.Item = item;
			this.PstSession = pstSession;
		}

		public ItemInformation Item { get; private set; }

		public IPST PstSession { get; private set; }

		public IPropertyBag CurrentPropertyBag { get; private set; }

		public void EnterAttachmentContext()
		{
			if (this.containerStack.Count == 0)
			{
				throw new ExportException(ExportErrorType.MessageDataCorrupted);
			}
			this.EnterContext(new ExtractContext.AttachmentWrapper(this.containerStack.Peek().AddAttachment()));
		}

		public void EnterMessageContext(IMessage message)
		{
			if (message == null)
			{
				if (this.containerStack.Count == 0)
				{
					throw new ExportException(ExportErrorType.MessageDataCorrupted);
				}
				message = this.containerStack.Peek().AddEmbeddedMessage();
			}
			this.EnterContext(new ExtractContext.MessageWrapper(message));
		}

		public void EnterRecipientContext()
		{
			if (this.containerStack.Count == 0)
			{
				throw new ExportException(ExportErrorType.MessageDataCorrupted);
			}
			this.EnterContext(new ExtractContext.RecipientWrapper(this.containerStack.Peek().AddRecipient()));
		}

		public void ExitAttachmentContext()
		{
			this.ExitContext<ExtractContext.AttachmentWrapper>();
		}

		public void ExitMessageContext()
		{
			this.ExitContext<ExtractContext.MessageWrapper>();
		}

		public void ExitRecipientContext()
		{
			this.ExitContext<ExtractContext.RecipientWrapper>();
		}

		private void EnterContext(ExtractContext.PropertyBagContainer container)
		{
			this.containerStack.Push(container);
			this.CurrentPropertyBag = container.PropertyBag;
		}

		private void ExitContext<T>() where T : ExtractContext.PropertyBagContainer
		{
			if (this.containerStack.Count > 0)
			{
				ExtractContext.PropertyBagContainer propertyBagContainer = this.containerStack.Pop();
				if (propertyBagContainer is T)
				{
					propertyBagContainer.Save();
					if (this.containerStack.Count > 0)
					{
						this.CurrentPropertyBag = this.containerStack.Peek().PropertyBag;
						return;
					}
					this.CurrentPropertyBag = null;
					return;
				}
			}
			throw new ExportException(ExportErrorType.MessageDataCorrupted);
		}

		private Stack<ExtractContext.PropertyBagContainer> containerStack;

		private abstract class PropertyBagContainer
		{
			public abstract IPropertyBag PropertyBag { get; }

			public virtual void Save()
			{
			}

			public virtual IMessage AddEmbeddedMessage()
			{
				throw new ExportException(ExportErrorType.MessageDataCorrupted);
			}

			public virtual IPropertyBag AddRecipient()
			{
				throw new ExportException(ExportErrorType.MessageDataCorrupted);
			}

			public virtual IAttachment AddAttachment()
			{
				throw new ExportException(ExportErrorType.MessageDataCorrupted);
			}
		}

		private class AttachmentWrapper : ExtractContext.PropertyBagContainer
		{
			public AttachmentWrapper(IAttachment attachment)
			{
				this.attachment = attachment;
			}

			public override IPropertyBag PropertyBag
			{
				get
				{
					return this.attachment.PropertyBag;
				}
			}

			public override void Save()
			{
				this.attachment.Save();
			}

			public override IMessage AddEmbeddedMessage()
			{
				IMessage message = this.attachment.AddMessageAttachment();
				IProperty property = this.attachment.PropertyBag.AddProperty(922812429U);
				IPropertyWriter propertyWriter = property.OpenStreamWriter();
				propertyWriter.Write(BitConverter.GetBytes((ulong)message.Id));
				propertyWriter.Close();
				return message;
			}

			private IAttachment attachment;
		}

		private class MessageWrapper : ExtractContext.PropertyBagContainer
		{
			public MessageWrapper(IMessage message)
			{
				this.message = message;
				this.recipients = new List<IPropertyBag>();
			}

			public override IPropertyBag PropertyBag
			{
				get
				{
					return this.message.PropertyBag;
				}
			}

			public override void Save()
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				StringBuilder stringBuilder3 = new StringBuilder();
				foreach (IPropertyBag propertyBag in this.recipients)
				{
					IProperty property = null;
					IProperty property2 = null;
					if (propertyBag.Properties.TryGetValue(PropertyTag.RecipientType.Id, out property) && property != null && propertyBag.Properties.TryGetValue(PropertyTag.DisplayName.Id, out property2) && property2 != null)
					{
						IPropertyReader propertyReader = property.OpenStreamReader();
						byte[] value = propertyReader.Read();
						uint num = BitConverter.ToUInt32(value, 0);
						propertyReader.Close();
						IPropertyReader propertyReader2 = property2.OpenStreamReader();
						byte[] bytes = propertyReader2.Read();
						string @string = Encoding.Unicode.GetString(bytes);
						propertyReader2.Close();
						switch (num)
						{
						case 1U:
							stringBuilder.Append(@string);
							stringBuilder.Append(';');
							break;
						case 2U:
							stringBuilder2.Append(@string);
							stringBuilder2.Append(';');
							break;
						case 3U:
							stringBuilder3.Append(@string);
							stringBuilder3.Append(';');
							break;
						}
					}
				}
				this.AddRecipientDisplayProperty(PropertyTag.DisplayTo, stringBuilder);
				this.AddRecipientDisplayProperty(PropertyTag.DisplayCc, stringBuilder2);
				this.AddRecipientDisplayProperty(PropertyTag.DisplayBcc, stringBuilder3);
				this.recipients.Clear();
				this.message.Save();
			}

			public override IAttachment AddAttachment()
			{
				return this.message.AddAttachment();
			}

			public override IPropertyBag AddRecipient()
			{
				IPropertyBag propertyBag = this.message.AddRecipient();
				this.recipients.Add(propertyBag);
				return propertyBag;
			}

			private void AddRecipientDisplayProperty(PropertyTag propertyTag, StringBuilder displayString)
			{
				if (displayString.Length > 0)
				{
					IProperty property = this.PropertyBag.AddProperty(propertyTag.NormalizedValueForPst);
					IPropertyWriter propertyWriter = property.OpenStreamWriter();
					propertyWriter.Write(Encoding.Unicode.GetBytes(displayString.ToString(0, displayString.Length - 1)));
					propertyWriter.Close();
				}
			}

			private IMessage message;

			private List<IPropertyBag> recipients;
		}

		private class RecipientWrapper : ExtractContext.PropertyBagContainer
		{
			public RecipientWrapper(IPropertyBag recipient)
			{
				this.recipient = recipient;
			}

			public override IPropertyBag PropertyBag
			{
				get
				{
					return this.recipient;
				}
			}

			private IPropertyBag recipient;
		}
	}
}
