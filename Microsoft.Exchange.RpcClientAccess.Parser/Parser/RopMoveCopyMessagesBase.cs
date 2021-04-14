using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopMoveCopyMessagesBase : DualInputRop
	{
		protected bool ReportProgress
		{
			get
			{
				return this.reportProgress;
			}
		}

		protected bool IsCopy
		{
			get
			{
				return this.isCopy;
			}
		}

		protected StoreId[] MessageIds
		{
			get
			{
				return this.messageIds;
			}
		}

		internal void SetInput(byte logonIndex, byte sourceHandleTableIndex, byte destinationHandleTableIndex, StoreId[] messageIds, bool reportProgress, bool isCopy)
		{
			base.SetCommonInput(logonIndex, sourceHandleTableIndex, destinationHandleTableIndex);
			this.messageIds = messageIds;
			this.reportProgress = reportProgress;
			this.isCopy = isCopy;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteCountedStoreIds(this.messageIds);
			writer.WriteBool(this.reportProgress);
			writer.WriteBool(this.isCopy);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.messageIds = reader.ReadSizeAndStoreIdArray();
			this.reportProgress = reader.ReadBool();
			this.isCopy = reader.ReadBool();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" IsCopy=").Append(this.IsCopy);
			stringBuilder.Append(" Progress=").Append(this.ReportProgress);
			stringBuilder.Append(" Mids=[");
			Util.AppendToString<StoreId>(stringBuilder, this.messageIds);
			stringBuilder.Append("]");
		}

		private StoreId[] messageIds;

		private bool reportProgress;

		private bool isCopy;
	}
}
