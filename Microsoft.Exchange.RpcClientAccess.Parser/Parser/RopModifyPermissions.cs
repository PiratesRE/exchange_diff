using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopModifyPermissions : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ModifyPermissions;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopModifyPermissions();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(string.Format(" ModifyPermissionsFlags={0}", this.modifyPermissionsFlags));
			stringBuilder.Append(" Permissions={");
			Util.AppendToString<ModifyTableRow>(stringBuilder, this.permissions);
			stringBuilder.Append("}");
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ModifyPermissionsFlags modifyPermissionsFlags, ModifyTableRow[] permissions)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.modifyPermissionsFlags = modifyPermissionsFlags;
			this.permissions = permissions;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.modifyPermissionsFlags);
			writer.WriteSizedModifyTableRows(this.permissions, string8Encoding);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopModifyPermissions.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.modifyPermissionsFlags = (ModifyPermissionsFlags)reader.ReadByte();
			this.permissions = reader.ReadSizeAndModifyTableRowArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			for (int i = 0; i < this.permissions.Length; i++)
			{
				this.permissions[i].ResolveString8Values(string8Encoding);
			}
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.ModifyPermissions(serverObject, this.modifyPermissionsFlags, this.permissions, RopModifyPermissions.resultFactory);
		}

		private const RopId RopType = RopId.ModifyPermissions;

		private static ModifyPermissionsResultFactory resultFactory = new ModifyPermissionsResultFactory();

		private ModifyPermissionsFlags modifyPermissionsFlags;

		private ModifyTableRow[] permissions;
	}
}
