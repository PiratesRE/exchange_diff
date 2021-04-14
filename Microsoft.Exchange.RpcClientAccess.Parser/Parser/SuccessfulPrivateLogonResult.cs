using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulPrivateLogonResult : SuccessfulLogonResult
	{
		internal SuccessfulPrivateLogonResult(IServerObject logonObject, LogonFlags logonFlags, StoreId[] folderIds, LogonExtendedResponseFlags extendedFlags, LocaleInfo? localeInfo, LogonResponseFlags logonResponseFlags, Guid mailboxInstanceGuid, ReplId databaseReplId, Guid databaseGuid, ExDateTime serverTime, ulong routingConfigurationTimestamp, StoreState storeState) : base(logonObject, logonFlags, folderIds, extendedFlags, localeInfo)
		{
			if (!base.IsLogonFlagSet(LogonFlags.Private))
			{
				throw new ArgumentException("Private logon result requires private logon flag to be set", "logonFlags");
			}
			this.logonResponseFlags = logonResponseFlags;
			this.mailboxInstanceGuid = mailboxInstanceGuid;
			this.databaseReplId = databaseReplId;
			this.databaseGuid = databaseGuid;
			this.serverTime = serverTime;
			this.routingConfigurationTimestamp = routingConfigurationTimestamp;
			this.storeState = storeState;
		}

		internal SuccessfulPrivateLogonResult(Reader reader) : base(reader)
		{
			this.logonResponseFlags = (LogonResponseFlags)reader.ReadByte();
			this.mailboxInstanceGuid = reader.ReadGuid();
			this.databaseReplId = ReplId.Parse(reader);
			this.databaseGuid = reader.ReadGuid();
			int second = (int)reader.ReadByte();
			int minute = (int)reader.ReadByte();
			int hour = (int)reader.ReadByte();
			reader.ReadByte();
			int day = (int)reader.ReadByte();
			int month = (int)reader.ReadByte();
			int year = (int)reader.ReadUInt16();
			this.serverTime = new ExDateTime(ExTimeZone.UtcTimeZone, year, month, day, hour, minute, second);
			this.routingConfigurationTimestamp = reader.ReadUInt64();
			this.storeState = (StoreState)reader.ReadUInt32();
		}

		internal LogonResponseFlags LogonResponseFlags
		{
			get
			{
				return this.logonResponseFlags;
			}
		}

		internal Guid MailboxInstanceGuid
		{
			get
			{
				return this.mailboxInstanceGuid;
			}
		}

		internal ReplId DatabaseReplId
		{
			get
			{
				return this.databaseReplId;
			}
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		internal ulong RoutingConfigurationTimestamp
		{
			get
			{
				return this.routingConfigurationTimestamp;
			}
		}

		public override string ToString()
		{
			return "SuccessfulPrivateLogonResult: " + this.ToBareString();
		}

		public new string ToBareString()
		{
			return string.Format("{0} LogonResponse[{1}] MailboxInstance[{2}] DatabaseId[{3}] Database[{4}] Time[{5}] Gwart[{6:X}] State[{7}]", new object[]
			{
				base.ToBareString(),
				this.logonResponseFlags,
				this.mailboxInstanceGuid,
				this.databaseReplId.ToBareString(),
				this.databaseGuid,
				this.serverTime,
				this.routingConfigurationTimestamp,
				this.storeState
			});
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte((byte)this.logonResponseFlags);
			writer.WriteGuid(this.mailboxInstanceGuid);
			this.databaseReplId.Serialize(writer);
			writer.WriteGuid(this.databaseGuid);
			writer.WriteByte((byte)this.serverTime.Second);
			writer.WriteByte((byte)this.serverTime.Minute);
			writer.WriteByte((byte)this.serverTime.Hour);
			writer.WriteByte((byte)this.serverTime.DayOfWeek);
			writer.WriteByte((byte)this.serverTime.Day);
			writer.WriteByte((byte)this.serverTime.Month);
			writer.WriteUInt16((ushort)this.serverTime.Year);
			writer.WriteUInt64(this.routingConfigurationTimestamp);
			writer.WriteUInt32((uint)this.storeState);
		}

		private readonly LogonResponseFlags logonResponseFlags;

		private readonly Guid mailboxInstanceGuid;

		private readonly ReplId databaseReplId;

		private readonly Guid databaseGuid;

		private readonly ExDateTime serverTime;

		private readonly ulong routingConfigurationTimestamp;

		private readonly StoreState storeState;
	}
}
