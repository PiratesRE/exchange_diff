using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class SuccessfulLogonResult : LogonResult
	{
		protected SuccessfulLogonResult(IServerObject logonObject, LogonFlags logonFlags, StoreId[] folderIds, LogonExtendedResponseFlags extendedFlags, LocaleInfo? localeInfo) : base(ErrorCode.None, logonObject)
		{
			this.logonFlags = logonFlags;
			this.folderIds = folderIds;
			this.extendedFlags = extendedFlags;
			this.localeInfo = localeInfo;
			if ((long)folderIds.Length != 13L)
			{
				throw new ArgumentException("Must set " + 13U + " folder ids", "folderIds");
			}
			if (extendedFlags != LogonExtendedResponseFlags.None && !this.IsLogonFlagSet(LogonFlags.Extended))
			{
				throw new ArgumentException("Extended response flags are specified but LogonFlags.Extended is not set", "logonFlags");
			}
			if (localeInfo != null && !this.IsExtendedFlagSet(LogonExtendedResponseFlags.LocaleInfo))
			{
				throw new ArgumentException("Locale is specified but LogonExtendedResponseFlags.LocaleInfo is not set", "extendedFlags");
			}
		}

		protected SuccessfulLogonResult(Reader reader) : base(reader)
		{
			this.logonFlags = (LogonFlags)reader.ReadByte();
			this.folderIds = new StoreId[13];
			int num = 0;
			while ((long)num < 13L)
			{
				this.folderIds[num] = StoreId.Parse(reader);
				num++;
			}
			this.extendedFlags = LogonExtendedResponseFlags.None;
			this.localeInfo = null;
			if (this.IsLogonFlagSet(LogonFlags.Extended))
			{
				this.extendedFlags = (LogonExtendedResponseFlags)reader.ReadUInt32();
				if (this.IsExtendedFlagSet(LogonExtendedResponseFlags.LocaleInfo))
				{
					this.localeInfo = new LocaleInfo?(Microsoft.Exchange.RpcClientAccess.Parser.LocaleInfo.Parse(reader));
				}
			}
		}

		internal LogonFlags LogonFlags
		{
			get
			{
				return this.logonFlags;
			}
		}

		internal StoreId[] FolderIds
		{
			get
			{
				return this.folderIds;
			}
		}

		internal LogonExtendedResponseFlags LogonExtendedResponseFlags
		{
			get
			{
				return this.extendedFlags;
			}
		}

		internal LocaleInfo? LocaleInfo
		{
			get
			{
				return this.localeInfo;
			}
		}

		public string ToBareString()
		{
			return string.Format("LogonFlags[{0}] FolderIds[{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}] Extended[{14}] Locale[{15}]", new object[]
			{
				this.logonFlags,
				this.folderIds[0],
				this.folderIds[1],
				this.folderIds[2],
				this.folderIds[3],
				this.folderIds[4],
				this.folderIds[5],
				this.folderIds[6],
				this.folderIds[7],
				this.folderIds[8],
				this.folderIds[9],
				this.folderIds[10],
				this.folderIds[11],
				this.folderIds[12],
				this.extendedFlags,
				(this.localeInfo != null) ? this.localeInfo.Value.ToBareString() : "null"
			});
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte((byte)this.logonFlags);
			foreach (StoreId storeId in this.folderIds)
			{
				storeId.Serialize(writer);
			}
			if (this.IsLogonFlagSet(LogonFlags.Extended))
			{
				writer.WriteUInt32((uint)this.extendedFlags);
				if (this.IsExtendedFlagSet(LogonExtendedResponseFlags.LocaleInfo))
				{
					this.localeInfo.Value.Serialize(writer);
				}
			}
		}

		protected bool IsLogonFlagSet(LogonFlags flagToTest)
		{
			return (this.logonFlags & flagToTest) == flagToTest;
		}

		protected bool IsExtendedFlagSet(LogonExtendedResponseFlags flagToTest)
		{
			return this.IsLogonFlagSet(LogonFlags.Extended) && (this.extendedFlags & flagToTest) == flagToTest;
		}

		private const uint FolderIdsCount = 13U;

		private readonly LogonFlags logonFlags;

		private readonly StoreId[] folderIds;

		private readonly LogonExtendedResponseFlags extendedFlags;

		private readonly LocaleInfo? localeInfo;
	}
}
