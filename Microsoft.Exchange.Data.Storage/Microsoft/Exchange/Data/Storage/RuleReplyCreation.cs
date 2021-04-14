using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RuleReplyCreation : ReplyCreation
	{
		internal RuleReplyCreation(MessageItem originalItem, MessageItem newItem, MessageItem template, ReplyForwardConfiguration configuration) : this(originalItem, newItem, template, configuration, false)
		{
		}

		internal RuleReplyCreation(MessageItem originalItem, MessageItem newItem, MessageItem template, ReplyForwardConfiguration configuration, bool shouldUseSender) : base(originalItem, newItem, configuration, false, shouldUseSender, false)
		{
			this.template = template;
		}

		protected override void UpdateNewItemProperties()
		{
			base.UpdateNewItemProperties();
			MessageItem messageItem = (MessageItem)this.newItem;
			if (this.template.ReplyTo.Count != 0 && messageItem.ReplyTo.Count == 0)
			{
				messageItem.PropertyBag[InternalSchema.MapiReplyToNames] = this.template.PropertyBag.GetValueOrDefault<string>(InternalSchema.MapiReplyToNames);
				messageItem.PropertyBag[InternalSchema.MapiReplyToBlob] = this.template.PropertyBag.GetValueOrDefault<byte[]>(InternalSchema.MapiReplyToBlob);
			}
			this.newItem.ClassName = this.template.ClassName;
		}

		protected override void BuildBody(BodyConversionCallbacks callbacks)
		{
			BodyReadConfiguration configuration = new BodyReadConfiguration(this.template.Body.RawFormat, this.template.Body.RawCharset.Name);
			BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(this.template.Body.RawFormat, this.template.Body.RawCharset.Name);
			bodyWriteConfiguration.SetTargetFormat(this.template.Body.Format, this.template.Body.Charset);
			using (Stream stream = this.template.Body.OpenReadStream(configuration))
			{
				using (Stream stream2 = this.newItem.Body.OpenWriteStream(bodyWriteConfiguration))
				{
					Util.StreamHandler.CopyStreamData(stream, stream2);
				}
			}
		}

		protected override void BuildAttachments(BodyConversionCallbacks callbacks, InboundConversionOptions optionsForSmime)
		{
			base.CopyAttachments(callbacks, this.template.AttachmentCollection, this.newItem.AttachmentCollection, false, this.template.Body.Format == BodyFormat.TextPlain, optionsForSmime);
		}

		private readonly MessageItem template;
	}
}
