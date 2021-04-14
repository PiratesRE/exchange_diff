using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopSaveChanges : InputRop
	{
		internal override byte InputHandleTableIndex
		{
			get
			{
				return this.realHandleTableIndex;
			}
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte realHandleTableIndex, SaveChangesMode saveChangesMode)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.saveChangesMode = saveChangesMode;
			this.realHandleTableIndex = realHandleTableIndex;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte(this.realHandleTableIndex);
			writer.WriteByte((byte)this.saveChangesMode);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.realHandleTableIndex = reader.ReadByte();
			this.saveChangesMode = (SaveChangesMode)reader.ReadByte();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" SaveMode=").Append(this.saveChangesMode);
		}

		protected byte realHandleTableIndex;

		protected SaveChangesMode saveChangesMode;
	}
}
