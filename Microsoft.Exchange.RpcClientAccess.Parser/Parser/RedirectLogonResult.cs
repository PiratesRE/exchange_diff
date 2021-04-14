using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RedirectLogonResult : RopResult
	{
		internal RedirectLogonResult(Reader reader) : base(reader)
		{
			this.logonFlags = (LogonFlags)reader.ReadByte();
			this.serverLegacyDn = reader.ReadAsciiString(StringFlags.IncludeNull | StringFlags.Sized);
		}

		internal RedirectLogonResult(string serverLegacyDn, LogonFlags logonFlags) : base(RopId.Logon, ErrorCode.WrongServer, null)
		{
			this.logonFlags = logonFlags;
			this.serverLegacyDn = serverLegacyDn;
		}

		internal string ServerLegacyDn
		{
			get
			{
				return this.serverLegacyDn;
			}
		}

		internal LogonFlags LogonFlags
		{
			get
			{
				return this.logonFlags;
			}
		}

		public override string ToString()
		{
			return string.Format("RedirectLogonResult: {0} {1}", this.ServerLegacyDn, this.LogonFlags);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte((byte)this.logonFlags);
			writer.WriteAsciiString(this.ServerLegacyDn, StringFlags.IncludeNull | StringFlags.Sized);
		}

		private readonly string serverLegacyDn;

		private readonly LogonFlags logonFlags;
	}
}
