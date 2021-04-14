using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopCopyProperties : DualInputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.CopyProperties;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopCopyProperties();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = CopyPropertiesResultFactory.Parse(reader);
		}

		internal void SetInput(byte logonIndex, byte sourceHandleTableIndex, byte destinationHandleTableIndex, bool reportProgress, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] propertyTags)
		{
			base.SetCommonInput(logonIndex, sourceHandleTableIndex, destinationHandleTableIndex);
			this.reportProgress = reportProgress;
			this.copyPropertiesFlags = copyPropertiesFlags;
			this.propertyTags = propertyTags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteBool(this.reportProgress);
			writer.WriteByte((byte)this.copyPropertiesFlags);
			writer.WriteCountAndPropertyTagArray(this.propertyTags, FieldLength.WordSize);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new CopyPropertiesResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
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
			this.copyPropertiesFlags = (CopyPropertiesFlags)reader.ReadByte();
			this.propertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IServerObject destinationServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			CopyPropertiesResultFactory resultFactory = new CopyPropertiesResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
			this.result = ropHandler.CopyProperties(sourceServerObject, destinationServerObject, this.reportProgress, this.copyPropertiesFlags, this.propertyTags, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.copyPropertiesFlags);
			stringBuilder.Append(" Progress=").Append(this.reportProgress);
			stringBuilder.Append(" Tags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.propertyTags);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.CopyProperties;

		private bool reportProgress;

		private CopyPropertiesFlags copyPropertiesFlags;

		private PropertyTag[] propertyTags;
	}
}
