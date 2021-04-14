using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetReceiveFolderResult : RopResult
	{
		internal StoreId ReceiveFolderId
		{
			get
			{
				return this.receiveFolderId;
			}
		}

		internal string MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		internal SuccessfulGetReceiveFolderResult(StoreId receiveFolderId, string messageClass) : base(RopId.GetReceiveFolder, ErrorCode.None, null)
		{
			this.receiveFolderId = receiveFolderId;
			this.messageClass = messageClass;
		}

		internal SuccessfulGetReceiveFolderResult(Reader reader) : base(reader)
		{
			this.receiveFolderId = StoreId.Parse(reader);
			this.messageClass = reader.ReadAsciiString(StringFlags.IncludeNull);
		}

		internal static SuccessfulGetReceiveFolderResult Parse(Reader reader)
		{
			return new SuccessfulGetReceiveFolderResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.receiveFolderId.Serialize(writer);
			writer.WriteAsciiString(this.messageClass, StringFlags.IncludeNull);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Folder=").Append(this.receiveFolderId.ToString());
			stringBuilder.Append(" Message Class=[").Append(this.messageClass).Append("]");
		}

		private readonly StoreId receiveFolderId;

		private readonly string messageClass;
	}
}
