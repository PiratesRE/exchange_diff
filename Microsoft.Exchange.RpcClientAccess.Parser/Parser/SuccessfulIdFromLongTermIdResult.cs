using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulIdFromLongTermIdResult : RopResult
	{
		internal SuccessfulIdFromLongTermIdResult(StoreId storeId) : base(RopId.IdFromLongTermId, ErrorCode.None, null)
		{
			this.storeId = storeId;
		}

		internal SuccessfulIdFromLongTermIdResult(Reader reader) : base(reader)
		{
			this.storeId = StoreId.Parse(reader);
		}

		public StoreId StoreId
		{
			get
			{
				return this.storeId;
			}
		}

		internal static SuccessfulIdFromLongTermIdResult Parse(Reader reader)
		{
			return new SuccessfulIdFromLongTermIdResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.storeId.Serialize(writer);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" ID=").Append(this.storeId.ToString());
		}

		private readonly StoreId storeId;
	}
}
