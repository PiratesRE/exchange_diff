using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetNamesFromIDs : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetNamesFromIDs;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetNamesFromIDs();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, PropertyId[] propertyIds)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.propertyIds = propertyIds;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteCountedPropertyIds(this.propertyIds);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetNamesFromIDsResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetNamesFromIDs.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.propertyIds = reader.ReadSizeAndPropertyIdArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetNamesFromIDs(serverObject, this.propertyIds, RopGetNamesFromIDs.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			if (this.propertyIds != null)
			{
				stringBuilder.Append(" IDs=[");
				Util.AppendToString<PropertyId>(stringBuilder, this.propertyIds);
				stringBuilder.Append("]");
			}
		}

		private const RopId RopType = RopId.GetNamesFromIDs;

		private static GetNamesFromIDsResultFactory resultFactory = new GetNamesFromIDsResultFactory();

		private PropertyId[] propertyIds;
	}
}
