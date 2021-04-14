using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopWritePerUserInformation : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.WritePerUserInformation;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopWritePerUserInformation();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" LTID=[").Append(this.longTermId).Append("]");
			stringBuilder.Append(" HasFinished=").Append(this.hasFinished);
			stringBuilder.Append(" DataOffset=").Append(this.dataOffset);
			if (this.data != null)
			{
				stringBuilder.Append(" Data=[");
				Util.AppendToString(stringBuilder, this.data);
				stringBuilder.Append("]");
			}
			if (this.replicaGuid != null)
			{
				stringBuilder.Append(" ReplicaGuid=").Append(this.replicaGuid.Value);
			}
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreLongTermId longTermId, bool hasFinished, uint dataOffset, byte[] data, Guid? replicaGuid)
		{
			Util.ThrowOnNullArgument(data, "data");
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.longTermId = longTermId;
			this.hasFinished = hasFinished;
			this.dataOffset = dataOffset;
			this.data = data;
			this.replicaGuid = replicaGuid;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.longTermId.Serialize(writer);
			writer.WriteBool(this.hasFinished);
			writer.WriteUInt32(this.dataOffset);
			writer.WriteSizedBytes(this.data);
			if (this.replicaGuid != null)
			{
				writer.WriteGuid(this.replicaGuid.Value);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopWritePerUserInformation.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
		{
			base.InternalParseInput(reader, serverObjectHandleTable, logonTracker);
			this.longTermId = StoreLongTermId.Parse(reader);
			this.hasFinished = reader.ReadBool();
			this.dataOffset = reader.ReadUInt32();
			this.data = reader.ReadSizeAndByteArray();
			if (this.dataOffset == 0U && !logonTracker.ParseIsPublicLogon(base.LogonIndex))
			{
				this.replicaGuid = new Guid?(reader.ReadGuid());
				return;
			}
			this.replicaGuid = null;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.WritePerUserInformation(serverObject, this.longTermId, this.hasFinished, this.dataOffset, this.data, this.replicaGuid, RopWritePerUserInformation.resultFactory);
		}

		private const RopId RopType = RopId.WritePerUserInformation;

		private static WritePerUserInformationResultFactory resultFactory = new WritePerUserInformationResultFactory();

		private StoreLongTermId longTermId;

		private bool hasFinished;

		private uint dataOffset;

		private byte[] data;

		private Guid? replicaGuid;
	}
}
