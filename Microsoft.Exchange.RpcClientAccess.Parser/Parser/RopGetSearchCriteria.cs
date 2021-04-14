using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetSearchCriteria : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetSearchCriteria;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetSearchCriteria();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, GetSearchCriteriaFlags flags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.flags = flags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteBool((byte)(this.flags & GetSearchCriteriaFlags.Unicode) != 0, 1);
			writer.WriteBool((byte)(this.flags & GetSearchCriteriaFlags.Restriction) != 0, 1);
			writer.WriteBool((byte)(this.flags & GetSearchCriteriaFlags.FolderIds) != 0, 1);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => SuccessfulGetSearchCriteriaResult.Parse(readerParameter, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new GetSearchCriteriaResultFactory(base.LogonIndex);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.flags = GetSearchCriteriaFlags.None;
			if (reader.ReadBool())
			{
				this.flags |= GetSearchCriteriaFlags.Unicode;
			}
			if (reader.ReadBool())
			{
				this.flags |= GetSearchCriteriaFlags.Restriction;
			}
			if (reader.ReadBool())
			{
				this.flags |= GetSearchCriteriaFlags.FolderIds;
			}
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			GetSearchCriteriaResultFactory resultFactory = new GetSearchCriteriaResultFactory(base.LogonIndex);
			this.result = ropHandler.GetSearchCriteria(serverObject, this.flags, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
		}

		private const RopId RopType = RopId.GetSearchCriteria;

		private GetSearchCriteriaFlags flags;
	}
}
