using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopImportMessageMove : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ImportMessageMove;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopImportMessageMove();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte[] sourceFolder, byte[] sourceMessage, byte[] predecessorChangeList, byte[] destinationMessage, byte[] destinationChangeNumber)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.sourceFolder = sourceFolder;
			this.sourceMessage = sourceMessage;
			this.predecessorChangeList = predecessorChangeList;
			this.destinationMessage = destinationMessage;
			this.destinationChangeNumber = destinationChangeNumber;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytes(this.sourceFolder, FieldLength.DWordSize);
			writer.WriteSizedBytes(this.sourceMessage, FieldLength.DWordSize);
			writer.WriteSizedBytes(this.predecessorChangeList, FieldLength.DWordSize);
			writer.WriteSizedBytes(this.destinationMessage, FieldLength.DWordSize);
			writer.WriteSizedBytes(this.destinationChangeNumber, FieldLength.DWordSize);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulImportMessageMoveResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopImportMessageMove.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.sourceFolder = reader.ReadSizeAndByteArray(FieldLength.DWordSize);
			this.sourceMessage = reader.ReadSizeAndByteArray(FieldLength.DWordSize);
			this.predecessorChangeList = reader.ReadSizeAndByteArray(FieldLength.DWordSize);
			this.destinationMessage = reader.ReadSizeAndByteArray(FieldLength.DWordSize);
			this.destinationChangeNumber = reader.ReadSizeAndByteArray(FieldLength.DWordSize);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.ImportMessageMove(serverObject, this.sourceFolder, this.sourceMessage, this.predecessorChangeList, this.destinationMessage, this.destinationChangeNumber, RopImportMessageMove.resultFactory);
		}

		private const RopId RopType = RopId.ImportMessageMove;

		private static ImportMessageMoveResultFactory resultFactory = new ImportMessageMoveResultFactory();

		private byte[] sourceFolder;

		private byte[] sourceMessage;

		private byte[] predecessorChangeList;

		private byte[] destinationMessage;

		private byte[] destinationChangeNumber;
	}
}
