using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopOpenStream : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.OpenStream;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopOpenStream();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndexInput, PropertyTag propertyTagInput, OpenMode openModeInput)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndexInput);
			this.propertyTag = propertyTagInput;
			this.openMode = openModeInput;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WritePropertyTag(this.propertyTag);
			writer.WriteByte((byte)this.openMode);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulOpenStreamResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopOpenStream.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.propertyTag = reader.ReadPropertyTag();
			this.openMode = (OpenMode)reader.ReadByte();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override int? MinimumResponseSizeRequired
		{
			get
			{
				return new int?(10);
			}
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.OpenStream(serverObject, this.propertyTag, this.openMode, RopOpenStream.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" OpenMode=").Append(this.openMode);
			stringBuilder.Append(" Tag=").Append(this.propertyTag);
		}

		private const RopId RopType = RopId.OpenStream;

		private static OpenStreamResultFactory resultFactory = new OpenStreamResultFactory();

		private PropertyTag propertyTag;

		private OpenMode openMode;
	}
}
