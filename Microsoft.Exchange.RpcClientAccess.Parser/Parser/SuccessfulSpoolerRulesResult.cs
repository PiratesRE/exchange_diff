using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSpoolerRulesResult : RopResult
	{
		internal SuccessfulSpoolerRulesResult(StoreId? folderId) : base(RopId.SpoolerRules, ErrorCode.None, null)
		{
			this.folderId = folderId;
		}

		internal SuccessfulSpoolerRulesResult(Reader reader) : base(reader)
		{
			if (reader.ReadBool())
			{
				this.folderId = new StoreId?(StoreId.Parse(reader));
			}
		}

		public StoreId? FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		internal static SuccessfulSpoolerRulesResult Parse(Reader reader)
		{
			return new SuccessfulSpoolerRulesResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.folderId != null, 1);
			if (this.folderId != null)
			{
				this.folderId.Value.Serialize(writer);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Trigger=").Append(this.folderId != null);
			if (this.folderId != null)
			{
				stringBuilder.Append(" FID=").Append(this.folderId.Value.ToString());
			}
		}

		private readonly StoreId? folderId;
	}
}
