using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class IMAPJobItemSubscriptionSettings : JobItemSubscriptionSettingsBase
	{
		internal IMAPJobItemSubscriptionSettings()
		{
		}

		public string Username { get; private set; }

		public string EncryptedPassword { get; private set; }

		public string UserRootFolder { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return IMAPJobItemSubscriptionSettings.ImapJobItemSubscriptionSettingsPropertyDefinitions;
			}
		}

		protected override bool IsEmpty
		{
			get
			{
				return base.IsEmpty && string.IsNullOrEmpty(this.Username) && string.IsNullOrEmpty(this.EncryptedPassword) && string.IsNullOrEmpty(this.UserRootFolder);
			}
		}

		public override JobItemSubscriptionSettingsBase Clone()
		{
			return new IMAPJobItemSubscriptionSettings
			{
				Username = this.Username,
				EncryptedPassword = this.EncryptedPassword,
				UserRootFolder = this.UserRootFolder,
				LastModifiedTime = base.LastModifiedTime
			};
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			message[MigrationBatchMessageSchema.MigrationJobItemIncomingUsername] = this.Username;
			message[MigrationBatchMessageSchema.MigrationJobItemEncryptedIncomingPassword] = this.EncryptedPassword;
			if (!string.IsNullOrEmpty(this.UserRootFolder))
			{
				message[MigrationBatchMessageSchema.MigrationUserRootFolder] = this.UserRootFolder;
				return;
			}
			message.Delete(MigrationBatchMessageSchema.MigrationUserRootFolder);
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.Username = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemIncomingUsername, null);
			this.EncryptedPassword = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemEncryptedIncomingPassword, null);
			this.UserRootFolder = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationUserRootFolder, null);
			return base.ReadFromMessageItem(message);
		}

		public override void UpdateFromDataRow(IMigrationDataRow request)
		{
			bool flag = false;
			IMAPMigrationDataRow imapmigrationDataRow = request as IMAPMigrationDataRow;
			if (imapmigrationDataRow == null)
			{
				throw new ArgumentException("expected an IMAPMigrationDataRow", "request");
			}
			if (!object.Equals(this.Username, imapmigrationDataRow.ImapUserId))
			{
				this.Username = imapmigrationDataRow.ImapUserId;
				flag = true;
			}
			if (!object.Equals(this.EncryptedPassword, imapmigrationDataRow.EncryptedPassword))
			{
				this.EncryptedPassword = imapmigrationDataRow.EncryptedPassword;
				flag = true;
			}
			if (!object.Equals(this.UserRootFolder, imapmigrationDataRow.MigrationUserRootFolder))
			{
				this.UserRootFolder = imapmigrationDataRow.MigrationUserRootFolder;
				flag = true;
			}
			if (flag || base.LastModifiedTime == ExDateTime.MinValue)
			{
				base.LastModifiedTime = ExDateTime.UtcNow;
			}
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			parent.Add(new object[]
			{
				new XElement("Username", this.Username),
				new XElement("EncryptedPassword", this.EncryptedPassword),
				new XElement("UserRootFolder", this.UserRootFolder)
			});
		}

		public static readonly PropertyDefinition[] ImapJobItemSubscriptionSettingsPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new StorePropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobItemIncomingUsername,
				MigrationBatchMessageSchema.MigrationJobItemEncryptedIncomingPassword,
				MigrationBatchMessageSchema.MigrationUserRootFolder
			},
			SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions
		});
	}
}
