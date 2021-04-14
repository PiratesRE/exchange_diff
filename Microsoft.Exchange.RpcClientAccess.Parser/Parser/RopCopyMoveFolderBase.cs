using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopCopyMoveFolderBase : DualInputRop
	{
		protected bool ReportProgress
		{
			get
			{
				return this.reportProgress;
			}
		}

		protected bool Recurse
		{
			get
			{
				return this.recurse;
			}
		}

		protected StoreId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		protected String8 FolderName
		{
			get
			{
				return this.folderName;
			}
		}

		protected virtual bool IsCopyFolder
		{
			get
			{
				return false;
			}
		}

		internal void SetInput(byte logonIndex, byte sourceHandleTableIndex, byte destinationHandleTableIndex, bool reportProgress, bool recurse, bool useUnicode, StoreId folderId, string folderName)
		{
			base.SetCommonInput(logonIndex, sourceHandleTableIndex, destinationHandleTableIndex);
			this.reportProgress = reportProgress;
			this.recurse = recurse;
			this.useUnicode = useUnicode;
			this.folderId = folderId;
			this.folderName = String8.Create(folderName);
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteBool(this.reportProgress);
			if (this.IsCopyFolder)
			{
				writer.WriteBool(this.recurse);
			}
			writer.WriteBool(this.useUnicode);
			this.folderId.Serialize(writer);
			if (this.useUnicode)
			{
				writer.WriteUnicodeString(this.folderName.StringValue, StringFlags.IncludeNull);
				return;
			}
			writer.WriteString8(this.folderName.StringValue, string8Encoding, StringFlags.IncludeNull);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.reportProgress = reader.ReadBool();
			if (this.IsCopyFolder)
			{
				this.recurse = reader.ReadBool();
			}
			this.useUnicode = reader.ReadBool();
			this.folderId = StoreId.Parse(reader);
			this.folderName = String8.Parse(reader, this.useUnicode, StringFlags.IncludeNull);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			this.folderName.ResolveString8Values(string8Encoding);
		}

		private bool reportProgress;

		private bool recurse;

		private bool useUnicode;

		private StoreId folderId;

		private String8 folderName;
	}
}
