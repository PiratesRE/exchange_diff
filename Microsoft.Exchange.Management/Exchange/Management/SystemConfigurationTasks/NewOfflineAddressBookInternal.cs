using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class NewOfflineAddressBookInternal : NewMultitenancySystemConfigurationObjectTask<OfflineAddressBook>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewOfflineAddressBook(base.Name.ToString(), base.FormatMultiValuedProperty(this.AddressLists));
			}
		}

		private int MaxOfflineAddressBooks
		{
			get
			{
				if (!Datacenter.IsMicrosoftHostedOnly(true))
				{
					return int.MaxValue;
				}
				int? maxOfflineAddressBooks = this.ConfigurationSession.GetOrgContainer().MaxOfflineAddressBooks;
				if (maxOfflineAddressBooks == null)
				{
					return 250;
				}
				return maxOfflineAddressBooks.GetValueOrDefault();
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(OrgContainerNotFoundException).IsInstanceOfType(exception) || typeof(TenantOrgContainerNotFoundException).IsInstanceOfType(exception);
		}

		public virtual AddressBookBaseIdParameter[] AddressLists
		{
			get
			{
				return (AddressBookBaseIdParameter[])base.Fields["AddressLists"];
			}
			set
			{
				base.Fields["AddressLists"] = value;
			}
		}

		[Parameter]
		public bool IsDefault
		{
			get
			{
				return this.DataObject.IsDefault;
			}
			set
			{
				this.DataObject.IsDefault = value;
			}
		}

		[Parameter]
		public MailboxIdParameter GeneratingMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["GeneratingMailbox"];
			}
			set
			{
				base.Fields["GeneratingMailbox"] = value;
			}
		}

		[Parameter]
		public bool GlobalWebDistributionEnabled
		{
			get
			{
				return (bool)(base.Fields["GlobalWebDistributionEnabled"] ?? false);
			}
			set
			{
				base.Fields["GlobalWebDistributionEnabled"] = value;
			}
		}

		[Parameter]
		public bool ShadowMailboxDistributionEnabled
		{
			get
			{
				return (bool)(base.Fields["ShadowMailboxDistributionEnabled"] ?? false);
			}
			set
			{
				base.Fields["ShadowMailboxDistributionEnabled"] = value;
			}
		}

		[Parameter]
		public Unlimited<int>? DiffRetentionPeriod
		{
			get
			{
				return this.DataObject.DiffRetentionPeriod;
			}
			set
			{
				this.DataObject.DiffRetentionPeriod = value;
			}
		}

		[Parameter]
		public VirtualDirectoryIdParameter[] VirtualDirectories
		{
			get
			{
				return (VirtualDirectoryIdParameter[])base.Fields["VirtualDirectories"];
			}
			set
			{
				base.Fields["VirtualDirectories"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.DataObject = (OfflineAddressBook)base.PrepareDataObject();
			MultiValuedProperty<OfflineAddressBookVersion> multiValuedProperty = new MultiValuedProperty<OfflineAddressBookVersion>();
			multiValuedProperty.Add(OfflineAddressBookVersion.Version4);
			this.DataObject.Versions = multiValuedProperty;
			this.DataObject.ConfiguredAttributes = OfflineAddressBookMapiProperty.DefaultOABPropertyList;
			this.DataObject.UpdateRawMapiAttributes(false);
			if (base.Fields.IsModified("GeneratingMailbox"))
			{
				this.DataObject.GeneratingMailbox = OfflineAddressBookTaskUtility.ValidateGeneratingMailbox(base.TenantGlobalCatalogSession, this.GeneratingMailbox, new OfflineAddressBookTaskUtility.GetUniqueObject(base.GetDataObject<ADUser>), this.DataObject, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			this.DataObject.AddressLists = OfflineAddressBookTaskUtility.ValidateAddressBook(base.DataSession, this.AddressLists, new OfflineAddressBookTaskUtility.GetUniqueObject(base.GetDataObject<AddressBookBase>), this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			this.DataObject.PublicFolderDistributionEnabled = false;
			this.DataObject.PublicFolderDatabase = null;
			if (this.VirtualDirectories != null && this.VirtualDirectories.Length != 0)
			{
				this.DataObject.VirtualDirectories = OfflineAddressBookTaskUtility.ValidateVirtualDirectory(base.GlobalConfigSession, this.VirtualDirectories, new OfflineAddressBookTaskUtility.GetUniqueObject(base.GetDataObject<ADOabVirtualDirectory>), this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.Fields.IsModified("GlobalWebDistributionEnabled"))
			{
				this.DataObject.GlobalWebDistributionEnabled = this.GlobalWebDistributionEnabled;
			}
			if (base.Fields.IsModified("ShadowMailboxDistributionEnabled"))
			{
				this.DataObject.ShadowMailboxDistributionEnabled = this.ShadowMailboxDistributionEnabled;
			}
			this.DataObject.Server = ((IConfigurationSession)base.DataSession).GetOrgContainerId().GetDescendantId(this.DataObject.ParentPath);
			if (!this.DataObject.IsModified(OfflineAddressBookSchema.IsDefault) && ((IConfigurationSession)base.DataSession).Find<OfflineAddressBook>(null, QueryScope.SubTree, null, null, 1).Length == 0)
			{
				this.DataObject.IsDefault = true;
			}
			this.DataObject.SetId((IConfigurationSession)base.DataSession, base.Name);
			string parentLegacyDN = string.Format(CultureInfo.InvariantCulture, "{0}/cn=addrlists/cn=oabs", new object[]
			{
				((IConfigurationSession)base.DataSession).GetOrgContainer().LegacyExchangeDN
			});
			this.DataObject[OfflineAddressBookSchema.ExchangeLegacyDN] = LegacyDN.GenerateLegacyDN(parentLegacyDN, this.DataObject, true, new LegacyDN.LegacyDNIsUnique(this.LegacyDNIsUnique));
			OfflineAddressBookTaskUtility.WarnForNoDistribution(this.DataObject, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			return this.DataObject;
		}

		protected override void InternalValidate()
		{
			int maxOfflineAddressBooks = this.MaxOfflineAddressBooks;
			if (maxOfflineAddressBooks < 2147483647)
			{
				IEnumerable<OfflineAddressBook> enumerable = base.DataSession.FindPaged<OfflineAddressBook>(null, ((IConfigurationSession)base.DataSession).GetOrgContainerId().GetDescendantId(this.DataObject.ParentPath), false, null, ADGenericPagedReader<OfflineAddressBook>.DefaultPageSize);
				int num = 0;
				foreach (OfflineAddressBook offlineAddressBook in enumerable)
				{
					num++;
					if (num >= maxOfflineAddressBooks)
					{
						base.WriteError(new ManagementObjectAlreadyExistsException(Strings.ErrorTooManyItems(maxOfflineAddressBooks)), ErrorCategory.LimitsExceeded, base.Name);
						break;
					}
				}
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			OfflineAddressBook offlineAddressBook = null;
			if (this.DataObject.IsDefault)
			{
				offlineAddressBook = OfflineAddressBookTaskUtility.ResetOldDefaultOab(base.DataSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			bool flag = false;
			try
			{
				base.InternalProcessRecord();
				flag = true;
			}
			finally
			{
				if (!flag && offlineAddressBook != null)
				{
					offlineAddressBook.IsDefault = true;
					try
					{
						base.DataSession.Save(offlineAddressBook);
					}
					catch (DataSourceTransientException exception)
					{
						this.WriteError(exception, ErrorCategory.WriteError, null, false);
					}
				}
			}
		}

		private bool LegacyDNIsUnique(string legacyDN)
		{
			bool result = false;
			OfflineAddressBook[] array = ((IConfigurationSession)base.DataSession).Find<OfflineAddressBook>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, OfflineAddressBookSchema.ExchangeLegacyDN, legacyDN), null, 2);
			if (array.Length <= 0 || (array.Length == 1 && array[0].DistinguishedName.Equals(((ADObjectId)this.DataObject[ADObjectSchema.Id]).DistinguishedName)))
			{
				result = true;
			}
			return result;
		}

		protected override void ValidateProvisionedProperties(IConfigurable dataObject)
		{
			OfflineAddressBook offlineAddressBook = dataObject as OfflineAddressBook;
			if (offlineAddressBook != null && offlineAddressBook.IsChanged(OfflineAddressBookSchema.VirtualDirectories) && (this.GlobalWebDistributionEnabled || (base.Fields.IsModified("VirtualDirectories") && base.Fields["VirtualDirectories"] == null)))
			{
				offlineAddressBook.VirtualDirectories = null;
			}
		}
	}
}
