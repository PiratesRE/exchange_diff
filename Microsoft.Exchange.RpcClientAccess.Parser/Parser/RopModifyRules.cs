using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopModifyRules : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ModifyRules;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopModifyRules();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" ModifyRulesFlags=").Append(this.modifyRulesFlags);
			stringBuilder.Append(" RulesData={");
			Util.AppendToString<ModifyTableRow>(stringBuilder, this.rulesData);
			stringBuilder.Append("}");
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ModifyRulesFlags modifyRulesFlags, ModifyTableRow[] rulesData)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.modifyRulesFlags = modifyRulesFlags;
			this.rulesData = rulesData;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.modifyRulesFlags);
			writer.WriteSizedModifyTableRows(this.rulesData, string8Encoding);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopModifyRules.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.modifyRulesFlags = (ModifyRulesFlags)reader.ReadByte();
			this.rulesData = reader.ReadSizeAndModifyTableRowArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			for (int i = 0; i < this.rulesData.Length; i++)
			{
				this.rulesData[i].ResolveString8Values(string8Encoding);
			}
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.ModifyRules(serverObject, this.modifyRulesFlags, this.rulesData, RopModifyRules.resultFactory);
		}

		private const RopId RopType = RopId.ModifyRules;

		private static ModifyRulesResultFactory resultFactory = new ModifyRulesResultFactory();

		private ModifyRulesFlags modifyRulesFlags;

		private ModifyTableRow[] rulesData;
	}
}
