using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopImportHierarchyChange : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ImportHierarchyChange;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopImportHierarchyChange();
		}

		internal void SetInput(byte logonIndex, byte handleIndex, PropertyValue[] hierarchyPropertyValues, PropertyValue[] folderPropertyValues)
		{
			base.SetCommonInput(logonIndex, handleIndex);
			this.hierarchyPropertyValues = hierarchyPropertyValues;
			this.folderPropertyValues = folderPropertyValues;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteCountAndPropertyValueList(this.hierarchyPropertyValues, string8Encoding, WireFormatStyle.Rop);
			writer.WriteCountAndPropertyValueList(this.folderPropertyValues, string8Encoding, WireFormatStyle.Rop);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulImportHierarchyChangeResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.ImportHierarchyChange(serverObject, this.hierarchyPropertyValues, this.folderPropertyValues, RopImportHierarchyChange.resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopImportHierarchyChange.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.hierarchyPropertyValues = reader.ReadCountAndPropertyValueList(WireFormatStyle.Rop);
			this.folderPropertyValues = reader.ReadCountAndPropertyValueList(WireFormatStyle.Rop);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			foreach (PropertyValue propertyValue in this.hierarchyPropertyValues)
			{
				propertyValue.ResolveString8Values(string8Encoding);
			}
			foreach (PropertyValue propertyValue2 in this.folderPropertyValues)
			{
				propertyValue2.ResolveString8Values(string8Encoding);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" HeaderProps=[");
			Util.AppendToString<PropertyValue>(stringBuilder, this.hierarchyPropertyValues);
			stringBuilder.Append("]");
			stringBuilder.Append(" FolderProps=[");
			Util.AppendToString<PropertyValue>(stringBuilder, this.folderPropertyValues);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.ImportHierarchyChange;

		private static ImportHierarchyChangeResultFactory resultFactory = new ImportHierarchyChangeResultFactory();

		private PropertyValue[] hierarchyPropertyValues;

		private PropertyValue[] folderPropertyValues;
	}
}
