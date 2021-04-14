using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopCreateFolder : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.CreateFolder;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopCreateFolder();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, FolderType folderType, bool useUnicode, CreateFolderFlags flags, string displayName, string folderComment, StoreLongTermId? longTermId)
		{
			Util.ThrowOnNullArgument(displayName, "displayName");
			Util.ThrowOnNullArgument(folderComment, "folderComment");
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.folderType = folderType;
			this.useUnicode = useUnicode;
			this.flags = flags;
			this.displayName = String8.Create(displayName);
			this.folderComment = String8.Create(folderComment);
			this.longTermId = longTermId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.folderType);
			writer.WriteBool(this.useUnicode);
			writer.WriteByte((byte)this.flags);
			writer.WriteBool(this.longTermId != null);
			if (this.useUnicode)
			{
				writer.WriteUnicodeString(this.displayName.StringValue, StringFlags.IncludeNull);
				writer.WriteUnicodeString(this.folderComment.StringValue, StringFlags.IncludeNull);
			}
			else
			{
				writer.WriteString8(this.displayName.StringValue, string8Encoding, StringFlags.IncludeNull);
				writer.WriteString8(this.folderComment.StringValue, string8Encoding, StringFlags.IncludeNull);
			}
			if (this.longTermId != null)
			{
				this.longTermId.Value.Serialize(writer);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulCreateFolderResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopCreateFolder.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.folderType = (FolderType)reader.ReadByte();
			this.useUnicode = reader.ReadBool();
			CreateFolderFlags createFolderFlags = (CreateFolderFlags)reader.ReadByte();
			bool flag = reader.ReadBool();
			this.displayName = String8.Parse(reader, this.useUnicode, StringFlags.IncludeNull);
			this.folderComment = String8.Parse(reader, this.useUnicode, StringFlags.IncludeNull);
			if (flag)
			{
				this.longTermId = new StoreLongTermId?(StoreLongTermId.Parse(reader));
			}
			if ((createFolderFlags & CreateFolderFlags.ReservedForLegacySupport) != CreateFolderFlags.None)
			{
				this.flags = CreateFolderFlags.OpenIfExists;
				return;
			}
			this.flags = createFolderFlags;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			this.displayName.ResolveString8Values(string8Encoding);
			this.folderComment.ResolveString8Values(string8Encoding);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.CreateFolder(serverObject, this.folderType, this.flags, this.displayName.StringValue, this.folderComment.StringValue, this.longTermId, RopCreateFolder.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Type=").Append(this.folderType);
			stringBuilder.Append(" Unicode=").Append(this.useUnicode);
			stringBuilder.Append(" Flags=[").Append(this.flags).Append("]");
			stringBuilder.Append(" Display Name=[").Append(this.displayName).Append("]");
			stringBuilder.Append(" Comment=[").Append(this.folderComment).Append("]");
			stringBuilder.Append(" LTID=[").Append(this.longTermId).Append("]");
		}

		private const RopId RopType = RopId.CreateFolder;

		private static CreateFolderResultFactory resultFactory = new CreateFolderResultFactory();

		private FolderType folderType;

		private bool useUnicode;

		private CreateFolderFlags flags;

		private String8 displayName;

		private String8 folderComment;

		private StoreLongTermId? longTermId;
	}
}
