using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal sealed class ReconciliationAccountPerTenantSettings : TenantConfigurationCacheableItem<JournalingReconciliationAccount>
	{
		public ReconciliationAccountPerTenantSettings()
		{
		}

		public ReconciliationAccountPerTenantSettings(List<JournalingReconciliationAccount> reconciliationAccounts) : base(true)
		{
			this.SetInternalData(reconciliationAccounts.ToArray());
		}

		public override void ReadData(IConfigurationSession session)
		{
			JournalingReconciliationAccount[] internalData = (JournalingReconciliationAccount[])session.Find<JournalingReconciliationAccount>(null, null, true, null);
			this.SetInternalData(internalData);
		}

		public ReconciliationAccountPerTenantSettings.ReconciliationAccountData[] AccountDataArray
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.accountDataArray;
			}
		}

		public override long ItemSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				if (this.accountDataArray == null)
				{
					return 0L;
				}
				long num = 0L;
				foreach (ReconciliationAccountPerTenantSettings.ReconciliationAccountData reconciliationAccountData in this.accountDataArray)
				{
					num += reconciliationAccountData.ItemSize;
				}
				return num + 18L;
			}
		}

		private void SetInternalData(ICollection<JournalingReconciliationAccount> reconciliationAccounts)
		{
			if (reconciliationAccounts == null)
			{
				this.accountDataArray = null;
				return;
			}
			this.accountDataArray = new ReconciliationAccountPerTenantSettings.ReconciliationAccountData[reconciliationAccounts.Count];
			int num = 0;
			foreach (JournalingReconciliationAccount adObject in reconciliationAccounts)
			{
				this.accountDataArray[num++] = new ReconciliationAccountPerTenantSettings.ReconciliationAccountData(adObject);
			}
		}

		private ReconciliationAccountPerTenantSettings.ReconciliationAccountData[] accountDataArray;

		public sealed class ReconciliationAccountData
		{
			public ReconciliationAccountData(JournalingReconciliationAccount adObject)
			{
				if (adObject.ArchiveUri != null)
				{
					this.archiveUri = adObject.ArchiveUri.ToString();
				}
				if (adObject.Mailboxes != null)
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(adObject.OrganizationId), 86, ".ctor", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Configuration\\ReconciliationAccountPerTenantSettings.cs");
					this.mailboxes = new string[adObject.Mailboxes.Count];
					for (int i = 0; i < adObject.Mailboxes.Count; i++)
					{
						ADRecipient adrecipient = tenantOrRootOrgRecipientSession.Read(adObject.Mailboxes[i]);
						SmtpAddress primarySmtpAddress = adrecipient.PrimarySmtpAddress;
						this.mailboxes[i] = adrecipient.PrimarySmtpAddress.ToString();
					}
				}
				this.objectGuid = adObject.Guid;
			}

			public Uri ArchiveUri
			{
				get
				{
					return new Uri(this.archiveUri);
				}
			}

			public string[] Mailboxes
			{
				get
				{
					return this.mailboxes;
				}
			}

			public Guid Guid
			{
				get
				{
					return this.objectGuid;
				}
			}

			public long ItemSize
			{
				get
				{
					int num = ReconciliationAccountPerTenantSettings.ReconciliationAccountData.GetStringLength(this.archiveUri);
					num += 18;
					foreach (string str in this.mailboxes)
					{
						num += ReconciliationAccountPerTenantSettings.ReconciliationAccountData.GetStringLength(str);
					}
					num += 18;
					num += 16;
					return (long)num;
				}
			}

			private static int GetStringLength(string str)
			{
				return str.Length * 2 + 18;
			}

			private const int FixedObjectOverhead = 18;

			private string archiveUri;

			private string[] mailboxes;

			private Guid objectGuid;
		}
	}
}
