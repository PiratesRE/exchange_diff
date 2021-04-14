using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulCreateAttachmentResult : RopResult
	{
		internal uint AttachmentNumber
		{
			get
			{
				return this.attachmentNumber;
			}
		}

		internal SuccessfulCreateAttachmentResult(IServerObject serverObject, uint attachmentNumber) : base(RopId.CreateAttachment, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
			this.attachmentNumber = attachmentNumber;
		}

		internal SuccessfulCreateAttachmentResult(Reader reader) : base(reader)
		{
			this.attachmentNumber = reader.ReadUInt32();
		}

		internal static SuccessfulCreateAttachmentResult Parse(Reader reader)
		{
			return new SuccessfulCreateAttachmentResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.attachmentNumber);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Number=").Append(this.AttachmentNumber);
		}

		private readonly uint attachmentNumber;
	}
}
