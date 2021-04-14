using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulCreateMessageResult : RopResult
	{
		internal SuccessfulCreateMessageResult(IServerObject serverObject, StoreId? messageId) : base(RopId.CreateMessage, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
			this.messageId = messageId;
		}

		internal SuccessfulCreateMessageResult(Reader reader) : base(reader)
		{
			bool flag = reader.ReadBool();
			if (flag)
			{
				this.messageId = new StoreId?(StoreId.Parse(reader));
			}
		}

		internal StoreId? MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		internal static RopResult Parse(Reader reader)
		{
			return new SuccessfulCreateMessageResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.messageId != null);
			if (this.messageId != null)
			{
				this.messageId.Value.Serialize(writer);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" MID=").Append((this.messageId != null) ? this.messageId.ToString() : "null");
		}

		private readonly StoreId? messageId;
	}
}
