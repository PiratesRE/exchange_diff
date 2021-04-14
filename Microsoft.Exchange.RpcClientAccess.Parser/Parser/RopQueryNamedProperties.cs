using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopQueryNamedProperties : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.QueryNamedProperties;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopQueryNamedProperties();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, QueryNamedPropertyFlags queryFlags, Guid? propertyGuid)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.queryFlags = queryFlags;
			this.propertyGuid = propertyGuid;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.queryFlags);
			writer.WriteBool(this.propertyGuid != null, 1);
			if (this.propertyGuid != null)
			{
				writer.WriteGuid(this.propertyGuid.Value);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulQueryNamedPropertiesResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopQueryNamedProperties.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.queryFlags = (QueryNamedPropertyFlags)reader.ReadByte();
			if (reader.ReadBool())
			{
				this.propertyGuid = new Guid?(reader.ReadGuid());
			}
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.QueryNamedProperties(serverObject, this.queryFlags, this.propertyGuid, RopQueryNamedProperties.resultFactory);
		}

		private const RopId RopType = RopId.QueryNamedProperties;

		private static QueryNamedPropertiesResultFactory resultFactory = new QueryNamedPropertiesResultFactory();

		private QueryNamedPropertyFlags queryFlags;

		private Guid? propertyGuid;
	}
}
