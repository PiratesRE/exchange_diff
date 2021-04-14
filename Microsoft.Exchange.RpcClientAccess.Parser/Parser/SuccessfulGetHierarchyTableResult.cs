using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetHierarchyTableResult : RopResult
	{
		internal SuccessfulGetHierarchyTableResult(IServerObject serverObject, int rowCount) : base(RopId.GetHierarchyTable, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
			this.rowCount = rowCount;
		}

		internal SuccessfulGetHierarchyTableResult(Reader reader) : base(reader)
		{
			this.rowCount = reader.ReadInt32();
		}

		internal int RowCount
		{
			get
			{
				return this.rowCount;
			}
		}

		public static SuccessfulGetHierarchyTableResult Parse(Reader reader)
		{
			return new SuccessfulGetHierarchyTableResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteInt32(this.rowCount);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" RowCount=").Append(this.rowCount);
		}

		private readonly int rowCount;
	}
}
