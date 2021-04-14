using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RopGetPerUserGuid : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetPerUserGuid;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetPerUserGuid();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreLongTermId publicFolderLongTermId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.publicFolderLongTermId = publicFolderLongTermId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.publicFolderLongTermId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetPerUserGuidResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetPerUserGuid.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.publicFolderLongTermId = StoreLongTermId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetPerUserGuid(serverObject, this.publicFolderLongTermId, RopGetPerUserGuid.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" PublicFolder=").Append(this.publicFolderLongTermId);
		}

		private const RopId RopType = RopId.GetPerUserGuid;

		private static readonly GetPerUserGuidResultFactory resultFactory = new GetPerUserGuidResultFactory();

		private StoreLongTermId publicFolderLongTermId;
	}
}
