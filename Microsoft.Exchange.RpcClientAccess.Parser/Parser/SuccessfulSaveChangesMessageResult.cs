using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSaveChangesMessageResult : RopResult
	{
		internal SuccessfulSaveChangesMessageResult(byte realHandleTableIndex, StoreId messageId) : base(RopId.SaveChangesMessage, ErrorCode.None, null)
		{
			this.realHandleTableIndex = realHandleTableIndex;
			this.messageId = messageId;
		}

		internal SuccessfulSaveChangesMessageResult(Reader reader) : base(reader)
		{
			this.realHandleTableIndex = reader.ReadByte();
			this.messageId = StoreId.Parse(reader);
		}

		public StoreId MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		internal static SuccessfulSaveChangesMessageResult Parse(Reader reader)
		{
			return new SuccessfulSaveChangesMessageResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte(this.realHandleTableIndex);
			this.messageId.Serialize(writer);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" MID=").Append(this.messageId.ToString());
		}

		private readonly StoreId messageId;

		private readonly byte realHandleTableIndex;
	}
}
