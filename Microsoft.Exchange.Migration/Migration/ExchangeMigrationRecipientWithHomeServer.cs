using System;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal abstract class ExchangeMigrationRecipientWithHomeServer : ExchangeMigrationRecipient, IMigrationSerializable
	{
		public ExchangeMigrationRecipientWithHomeServer(MigrationUserRecipientType recepientType) : base(recepientType)
		{
		}

		public string MsExchHomeServerName { get; protected set; }

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			if (!string.IsNullOrEmpty(this.MsExchHomeServerName))
			{
				message[MigrationBatchMessageSchema.MigrationJobItemExchangeMsExchHomeServerName] = this.MsExchHomeServerName;
			}
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			if (!base.ReadFromMessageItem(message))
			{
				return false;
			}
			this.MsExchHomeServerName = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemExchangeMsExchHomeServerName, null);
			return true;
		}
	}
}
