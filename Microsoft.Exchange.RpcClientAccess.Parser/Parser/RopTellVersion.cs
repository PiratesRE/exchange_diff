using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopTellVersion : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.TellVersion;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopTellVersion();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ushort productMajorVersion, ushort buildMajorVersion, ushort buildMinorVersion)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.productMajorVersion = productMajorVersion;
			this.buildMajorVersion = buildMajorVersion;
			this.buildMinorVersion = buildMinorVersion;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16(this.productMajorVersion);
			writer.WriteUInt16(this.buildMajorVersion);
			writer.WriteUInt16(this.buildMinorVersion);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulTellVersionResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopTellVersion.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.productMajorVersion = reader.ReadUInt16();
			this.buildMajorVersion = reader.ReadUInt16();
			this.buildMinorVersion = reader.ReadUInt16();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.TellVersion(serverObject, this.productMajorVersion, this.buildMajorVersion, this.buildMinorVersion, RopTellVersion.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Product=").Append(this.productMajorVersion);
			stringBuilder.Append(" BuildMajor=").Append(this.buildMajorVersion);
			stringBuilder.Append(" BuildMinor=").Append(this.buildMinorVersion);
		}

		private const RopId RopType = RopId.TellVersion;

		private static TellVersionResultFactory resultFactory = new TellVersionResultFactory();

		private ushort productMajorVersion;

		private ushort buildMajorVersion;

		private ushort buildMinorVersion;
	}
}
