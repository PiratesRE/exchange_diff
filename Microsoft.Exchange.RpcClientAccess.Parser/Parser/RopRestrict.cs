using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopRestrict : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.Restrict;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopRestrict();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, RestrictFlags flags, Restriction restriction)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.flags = flags;
			this.restriction = restriction;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.flags);
			writer.WriteSizedRestriction(this.restriction, string8Encoding, WireFormatStyle.Rop);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulRestrictResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopRestrict.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.flags = (RestrictFlags)reader.ReadByte();
			this.restriction = reader.ReadSizeAndRestriction(WireFormatStyle.Rop);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			if (this.restriction != null)
			{
				this.restriction.ResolveString8Values(string8Encoding);
			}
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.Restrict(serverObject, this.flags, this.restriction, RopRestrict.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" Restriction=[");
			if (this.restriction != null)
			{
				this.restriction.AppendToString(stringBuilder);
			}
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.Restrict;

		private static RestrictResultFactory resultFactory = new RestrictResultFactory();

		private RestrictFlags flags;

		private Restriction restriction;
	}
}
