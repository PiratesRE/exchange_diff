using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopCopyTo : DualInputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.CopyTo;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopCopyTo();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = CopyToResultFactory.Parse(reader);
		}

		internal void SetInput(byte logonIndex, byte sourceHandleTableIndex, byte destinationHandleTableIndex, bool reportProgress, bool copySubObjects, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] excludePropertyTags)
		{
			base.SetCommonInput(logonIndex, sourceHandleTableIndex, destinationHandleTableIndex);
			this.reportProgress = reportProgress;
			this.copySubObjects = copySubObjects;
			this.copyPropertiesFlags = copyPropertiesFlags;
			this.excludePropertyTags = excludePropertyTags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteBool(this.reportProgress);
			writer.WriteBool(this.copySubObjects);
			writer.WriteByte((byte)this.copyPropertiesFlags);
			writer.WriteCountAndPropertyTagArray(this.excludePropertyTags, FieldLength.WordSize);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new CopyToResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.reportProgress = reader.ReadBool();
			this.copySubObjects = reader.ReadBool();
			this.copyPropertiesFlags = (CopyPropertiesFlags)reader.ReadByte();
			this.excludePropertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IServerObject destinationServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			CopyToResultFactory resultFactory = new CopyToResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
			this.result = ropHandler.CopyTo(sourceServerObject, destinationServerObject, this.reportProgress, this.copySubObjects, this.copyPropertiesFlags, this.excludePropertyTags, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.copyPropertiesFlags);
			stringBuilder.Append(" CopySubObjects=").Append(this.copySubObjects);
			stringBuilder.Append(" Progress=").Append(this.reportProgress);
			stringBuilder.Append(" ExcludeTags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.excludePropertyTags);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.CopyTo;

		private bool reportProgress;

		private bool copySubObjects;

		private CopyPropertiesFlags copyPropertiesFlags;

		private PropertyTag[] excludePropertyTags;
	}
}
