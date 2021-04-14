using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetIdsFromNames : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetIdsFromNames;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetIdsFromNames();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, GetIdsFromNamesFlags flags, NamedProperty[] namedProperties)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.flags = flags;
			this.namedProperties = namedProperties;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.flags);
			writer.WriteCountedNamedProperties(this.namedProperties);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetIdsFromNamesResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetIdsFromNames.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.flags = (GetIdsFromNamesFlags)reader.ReadByte();
			this.namedProperties = reader.ReadSizeAndNamedPropertyArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetIdsFromNames(serverObject, this.flags, this.namedProperties, RopGetIdsFromNames.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" Names=[");
			Util.AppendToString<NamedProperty>(stringBuilder, this.namedProperties);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.GetIdsFromNames;

		private static GetIdsFromNamesResultFactory resultFactory = new GetIdsFromNamesResultFactory();

		private GetIdsFromNamesFlags flags;

		private NamedProperty[] namedProperties;
	}
}
