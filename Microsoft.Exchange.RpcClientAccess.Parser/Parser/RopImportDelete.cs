using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopImportDelete : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ImportDelete;
			}
		}

		internal ImportDeleteFlags ImportDeleteFlags
		{
			get
			{
				return this.importDeleteFlags;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopImportDelete();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ImportDeleteFlags importDeleteFlags, PropertyValue[] deleteChanges)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.importDeleteFlags = importDeleteFlags;
			this.deleteChanges = deleteChanges;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.importDeleteFlags);
			writer.WriteCountAndPropertyValueList(this.deleteChanges, string8Encoding, WireFormatStyle.Rop);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.ImportDelete(sourceServerObject, this.importDeleteFlags, this.deleteChanges, RopImportDelete.resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopImportDelete.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.importDeleteFlags = (ImportDeleteFlags)reader.ReadByte();
			this.deleteChanges = reader.ReadCountAndPropertyValueList(WireFormatStyle.Rop);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			foreach (PropertyValue propertyValue in this.deleteChanges)
			{
				propertyValue.ResolveString8Values(string8Encoding);
			}
		}

		private const RopId RopType = RopId.ImportDelete;

		private static ImportDeleteResultFactory resultFactory = new ImportDeleteResultFactory();

		private ImportDeleteFlags importDeleteFlags;

		private PropertyValue[] deleteChanges;
	}
}
