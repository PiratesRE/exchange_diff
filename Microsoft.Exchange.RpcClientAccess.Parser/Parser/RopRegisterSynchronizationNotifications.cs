using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopRegisterSynchronizationNotifications : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.RegisterSynchronizationNotifications;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopRegisterSynchronizationNotifications();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId[] folderIds, uint[] folderChangeNumbers)
		{
			Util.ThrowOnNullArgument(folderIds, "folderIds");
			Util.ThrowOnNullArgument(folderChangeNumbers, "folderChangeNumbers");
			if (folderIds.Length != folderChangeNumbers.Length)
			{
				throw new ArgumentException("Number of elements in list of Folder Ids and list of change numbers do not match");
			}
			if (folderIds.Length > 65535)
			{
				throw new ArgumentOutOfRangeException("folderIds.Length", folderIds.Length, "Number of elements in list of Folder Ids exceeds the maximum allowed.");
			}
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.folderIds = folderIds;
			this.folderChangeNumbers = folderChangeNumbers;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16((ushort)this.folderIds.Length);
			for (int i = 0; i < this.folderIds.Length; i++)
			{
				this.folderIds[i].Serialize(writer);
			}
			for (int j = 0; j < this.folderChangeNumbers.Length; j++)
			{
				writer.WriteUInt32(this.folderChangeNumbers[j]);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopRegisterSynchronizationNotifications.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			ushort num = reader.ReadUInt16();
			if (num == 0)
			{
				this.folderIds = Array<StoreId>.Empty;
				this.folderChangeNumbers = Array<uint>.Empty;
				return;
			}
			reader.CheckBoundary((uint)num, 8U);
			this.folderIds = new StoreId[(int)num];
			for (int i = 0; i < (int)num; i++)
			{
				this.folderIds[i] = StoreId.Parse(reader);
			}
			reader.CheckBoundary((uint)num, 4U);
			this.folderChangeNumbers = new uint[(int)num];
			for (int j = 0; j < (int)num; j++)
			{
				this.folderChangeNumbers[j] = reader.ReadUInt32();
			}
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.RegisterSynchronizationNotifications(serverObject, this.folderIds, this.folderChangeNumbers, RopRegisterSynchronizationNotifications.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			if (this.folderIds != null)
			{
				stringBuilder.Append(" folderIds=[");
				Util.AppendToString<StoreId>(stringBuilder, this.folderIds);
				stringBuilder.Append("]");
			}
			if (this.folderChangeNumbers != null)
			{
				stringBuilder.Append(" folderChangeNumbers=[");
				Util.AppendToString<uint>(stringBuilder, this.folderChangeNumbers);
				stringBuilder.Append("]");
			}
		}

		private const RopId RopType = RopId.RegisterSynchronizationNotifications;

		private static RegisterSynchronizationNotificationsResultFactory resultFactory = new RegisterSynchronizationNotificationsResultFactory();

		private StoreId[] folderIds;

		private uint[] folderChangeNumbers;
	}
}
