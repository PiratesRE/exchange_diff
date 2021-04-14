using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopEmptyFolderBase : InputRop
	{
		protected bool ReportProgress
		{
			get
			{
				return this.reportProgress;
			}
		}

		protected EmptyFolderFlags EmptyFolderFlags
		{
			get
			{
				return this.emptyFolderFlags;
			}
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, bool reportProgress, EmptyFolderFlags emptyFolderFlags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.reportProgress = reportProgress;
			this.emptyFolderFlags = emptyFolderFlags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteBool(this.reportProgress);
			writer.WriteByte((byte)this.emptyFolderFlags);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.reportProgress = reader.ReadBool();
			this.emptyFolderFlags = (EmptyFolderFlags)reader.ReadByte();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.EmptyFolderFlags);
			stringBuilder.Append(" Progress=").Append(this.ReportProgress);
		}

		private bool reportProgress;

		private EmptyFolderFlags emptyFolderFlags;
	}
}
