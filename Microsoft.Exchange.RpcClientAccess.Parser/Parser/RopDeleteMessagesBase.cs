using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopDeleteMessagesBase : InputRop
	{
		protected bool ReportProgress
		{
			get
			{
				return this.reportProgress;
			}
		}

		protected bool IsOkToSendNonReadNotification
		{
			get
			{
				return this.isOkToSendNonReadNotification;
			}
		}

		protected StoreId[] MessageIds
		{
			get
			{
				return this.messageIds;
			}
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, bool reportProgress, bool isOkToSendNonReadNotification, StoreId[] messageIds)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.reportProgress = reportProgress;
			this.isOkToSendNonReadNotification = isOkToSendNonReadNotification;
			this.messageIds = messageIds;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteBool(this.reportProgress);
			writer.WriteBool(this.isOkToSendNonReadNotification);
			writer.WriteCountedStoreIds(this.messageIds);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.reportProgress = reader.ReadBool();
			this.isOkToSendNonReadNotification = reader.ReadBool();
			this.messageIds = reader.ReadSizeAndStoreIdArray();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Progress=").Append(this.ReportProgress);
			stringBuilder.Append(" SendNRN=").Append(this.IsOkToSendNonReadNotification);
			stringBuilder.Append(" MIDs=[");
			Util.AppendToString<StoreId>(stringBuilder, this.MessageIds);
			stringBuilder.Append("]");
		}

		private bool reportProgress;

		private bool isOkToSendNonReadNotification;

		private StoreId[] messageIds;
	}
}
