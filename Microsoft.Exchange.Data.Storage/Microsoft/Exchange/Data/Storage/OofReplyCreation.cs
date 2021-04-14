using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OofReplyCreation : ReplyCreation
	{
		internal OofReplyCreation(MessageItem originalItem, MessageItem newItem, MessageItem template, ReplyForwardConfiguration configuration) : base(originalItem, newItem, configuration, false, true, true)
		{
			this.template = template;
		}

		protected override void BuildSubject()
		{
			this.newItem[InternalSchema.SubjectPrefix] = ClientStrings.OofReply.ToString(base.Culture);
			this.newItem[InternalSchema.NormalizedSubject] = this.originalItem.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal, string.Empty);
		}

		protected override void UpdateNewItemProperties()
		{
			base.UpdateNewItemProperties();
			this.newItem[InternalSchema.OofReplyType] = this.template.GetValueOrDefault<int>(InternalSchema.OofReplyType, 2);
			this.newItem[InternalSchema.ItemClass] = this.template.GetValueOrDefault<string>(InternalSchema.ItemClass, "IPM.Note");
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

		private readonly MessageItem template;

		internal enum OofReplyType
		{
			Legacy,
			Single,
			Internal,
			External
		}
	}
}
