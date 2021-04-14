using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulPublicLogonResult : SuccessfulLogonResult
	{
		internal SuccessfulPublicLogonResult(IServerObject logonObject, LogonFlags logonFlags, StoreId[] folderIds, LogonExtendedResponseFlags extendedFlags, LocaleInfo? localeInfo, ReplId databaseReplId, Guid databaseGuid, Guid perUserReadGuid) : base(logonObject, logonFlags, folderIds, extendedFlags, localeInfo)
		{
			if (base.IsLogonFlagSet(LogonFlags.Private))
			{
				throw new ArgumentException("Public logon result requires private logon flag to be unset", "logonFlags");
			}
			this.databaseReplId = databaseReplId;
			this.databaseGuid = databaseGuid;
			this.perUserReadGuid = perUserReadGuid;
		}

		internal SuccessfulPublicLogonResult(Reader reader) : base(reader)
		{
			this.databaseReplId = ReplId.Parse(reader);
			this.databaseGuid = reader.ReadGuid();
			this.perUserReadGuid = reader.ReadGuid();
		}

		public ReplId DatabaseReplId
		{
			get
			{
				return this.databaseReplId;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public Guid PerUserReadGuid
		{
			get
			{
				return this.perUserReadGuid;
			}
		}

		public override string ToString()
		{
			return "SuccessfulPublicLogonResult: " + this.ToBareString();
		}

		public new string ToBareString()
		{
			return string.Format("{0} DatabaseId[{1}] Database[{2}] PerUserRead[{3}]", new object[]
			{
				base.ToBareString(),
				this.databaseReplId.ToBareString(),
				this.databaseGuid,
				this.perUserReadGuid
			});
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.databaseReplId.Serialize(writer);
			writer.WriteGuid(this.databaseGuid);
			writer.WriteGuid(this.perUserReadGuid);
		}

		private readonly ReplId databaseReplId;

		private readonly Guid databaseGuid;

		private readonly Guid perUserReadGuid;
	}
}
