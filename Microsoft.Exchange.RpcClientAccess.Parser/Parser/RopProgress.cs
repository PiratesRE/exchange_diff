using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopProgress : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.Progress;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopProgress();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" WantCancel=").Append(this.wantCancel);
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, bool wantCancel)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.wantCancel = wantCancel;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteBool(this.wantCancel, 1);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = ProgressResultFactory.Parse(reader);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopProgress.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.wantCancel = reader.ReadBool();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.Progress(serverObject, this.wantCancel, RopProgress.resultFactory);
		}

		private const RopId RopType = RopId.Progress;

		private static ProgressResultFactory resultFactory = new ProgressResultFactory();

		private bool wantCancel;
	}
}
