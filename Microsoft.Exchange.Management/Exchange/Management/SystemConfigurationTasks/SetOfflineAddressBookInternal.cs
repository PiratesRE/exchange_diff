using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class SetOfflineAddressBookInternal : SetSystemConfigurationObjectTask<OfflineAddressBookIdParameter, OfflineAddressBook>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetOfflineAddressBook(this.Identity.ToString());
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

		[ValidateNotNullOrEmpty]
		[Parameter]
		public AddressBookBaseIdParameter[] AddressLists
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

		[Parameter]
		public SwitchParameter ApplyMandatoryProperties
		{
			get
			{
				return (SwitchParameter)(base.Fields["ApplyMandatoryProperties"] ?? false);
			}
			set
			{
				base.Fields["ApplyMandatoryProperties"] = value;
			}
		}

		[Parameter]
		public SwitchParameter UseDefaultAttributes
		{
			get
			{
				return (SwitchParameter)(base.Fields["UseDefaultAttributes"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UseDefaultAttributes"] = value;
			}
		}

		protected override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			return this.ApplyMandatoryProperties.IsPresent;
		}

		protected override void UpgradeExchangeVersion(ADObject adObject)
		{
			if (!this.DataObject.IsE15OrLater())
			{
				return;
			}
			OfflineAddressBook offlineAddressBook = (OfflineAddressBook)adObject;
			Server server = (Server)base.GetDataObject<Server>(new ServerIdParameter(offlineAddressBook.Server), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(offlineAddressBook.Server.Name.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(offlineAddressBook.Server.Name.ToString())));
			if (server.IsE14OrLater)
			{
				offlineAddressBook.ExchangeVersion = ExchangeObjectVersion.Exchange2010;
				return;
			}
			if (server.IsExchange2007OrLater)
			{
				offlineAddressBook.ExchangeVersion = ExchangeObjectVersion.Exchange2007;
				return;
			}
			offlineAddressBook.ExchangeVersion = ExchangeObjectVersion.Exchange2003;
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			bool flag = ((OfflineAddressBook)dataObject).Versions.Contains(OfflineAddressBookVersion.Version1);
			this.isOriginalPfDistributionEnabled = ((OfflineAddressBook)dataObject).PublicFolderDistributionEnabled;
			base.StampChangesOn(dataObject);
			bool flag2 = ((OfflineAddressBook)dataObject).Versions.Contains(OfflineAddressBookVersion.Version1);
			this.isPresentPfDistributionEnabled = ((OfflineAddressBook)dataObject).PublicFolderDistributionEnabled;
			OfflineAddressBook offlineAddressBook = (OfflineAddressBook)dataObject;
			for (int i = 0; i < SetOfflineAddressBookInternal.propertiesCannotBeSet.GetLength(0); i++)
			{
				if (offlineAddressBook.IsModified(SetOfflineAddressBookInternal.propertiesCannotBeSet[i, 0]))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorSpecifiedPropertyCannotBeSet((SetOfflineAddressBookInternal.propertiesCannotBeSet[i, 1] ?? SetOfflineAddressBookInternal.propertiesCannotBeSet[i, 0]).ToString())), ErrorCategory.InvalidOperation, this.Identity);
				}
			}
			if (!flag && flag2)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorSetVersion1OfExchange12OAB), ErrorCategory.InvalidOperation, offlineAddressBook.Identity);
			}
			if (this.GeneratingMailbox != null)
			{
				if (offlineAddressBook.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorLegacyOABDoesNotSupportLinkedMailbox), ErrorCategory.InvalidOperation, this.Identity);
				}
				offlineAddressBook.GeneratingMailbox = OfflineAddressBookTaskUtility.ValidateGeneratingMailbox(base.TenantGlobalCatalogSession, this.GeneratingMailbox, new OfflineAddressBookTaskUtility.GetUniqueObject(base.GetDataObject<ADUser>), offlineAddressBook, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (this.AddressLists != null && this.AddressLists.Length > 0)
			{
				offlineAddressBook.AddressLists = OfflineAddressBookTaskUtility.ValidateAddressBook(base.DataSession, this.AddressLists, new OfflineAddressBookTaskUtility.GetUniqueObject(base.GetDataObject<AddressBookBase>), offlineAddressBook, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			else if (offlineAddressBook.IsModified(OfflineAddressBookSchema.AddressLists))
			{
				List<AddressBookBaseIdParameter> list = new List<AddressBookBaseIdParameter>();
				if (offlineAddressBook.AddressLists != null && offlineAddressBook.AddressLists.Added.Length != 0)
				{
					foreach (ADObjectId adObjectId in offlineAddressBook.AddressLists.Added)
					{
						list.Add(new AddressBookBaseIdParameter(adObjectId));
					}
					OfflineAddressBookTaskUtility.ValidateAddressBook(base.DataSession, list.ToArray(), new OfflineAddressBookTaskUtility.GetUniqueObject(base.GetDataObject<AddressBookBase>), offlineAddressBook, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
			if (base.Fields.IsModified("VirtualDirectories"))
			{
				if (this.VirtualDirectories != null && this.VirtualDirectories.Length > 0)
				{
					offlineAddressBook.VirtualDirectories = OfflineAddressBookTaskUtility.ValidateVirtualDirectory(base.GlobalConfigSession, this.VirtualDirectories, new OfflineAddressBookTaskUtility.GetUniqueObject(base.GetDataObject<ADOabVirtualDirectory>), offlineAddressBook, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				else
				{
					offlineAddressBook.VirtualDirectories = null;
				}
			}
			else if (offlineAddressBook.IsModified(OfflineAddressBookSchema.VirtualDirectories))
			{
				if (offlineAddressBook.VirtualDirectories != null && offlineAddressBook.VirtualDirectories.Added.Length > 0)
				{
					List<VirtualDirectoryIdParameter> list2 = new List<VirtualDirectoryIdParameter>();
					foreach (ADObjectId adObjectId2 in offlineAddressBook.VirtualDirectories.Added)
					{
						list2.Add(new VirtualDirectoryIdParameter(adObjectId2));
					}
					OfflineAddressBookTaskUtility.ValidateVirtualDirectory(base.GlobalConfigSession, list2.ToArray(), new OfflineAddressBookTaskUtility.GetUniqueObject(base.GetDataObject<ADOabVirtualDirectory>), offlineAddressBook, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				else
				{
					offlineAddressBook.VirtualDirectories = null;
				}
			}
			bool flag3 = false;
			if (this.isOriginalPfDistributionEnabled ^ this.isPresentPfDistributionEnabled)
			{
				if (offlineAddressBook.PublicFolderDatabase != null)
				{
					IEnumerable<PublicFolderDatabase> objects = new DatabaseIdParameter(offlineAddressBook.PublicFolderDatabase)
					{
						AllowLegacy = true
					}.GetObjects<PublicFolderDatabase>(null, base.GlobalConfigSession);
					using (IEnumerator<PublicFolderDatabase> enumerator = objects.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							this.publicFolderDatabase = enumerator.Current;
							if (enumerator.MoveNext())
							{
								this.publicFolderDatabase = null;
							}
							else
							{
								flag3 = true;
							}
						}
					}
				}
				if (!flag3)
				{
					OfflineAddressBookTaskUtility.ValidatePublicFolderInfrastructure(base.GlobalConfigSession, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), this.isPresentPfDistributionEnabled);
				}
			}
			if (this.UseDefaultAttributes.IsPresent)
			{
				if (offlineAddressBook.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
				{
					base.WriteError(new InvalidOperationException(Strings.CannotUseDefaultAttributesForExchange2007OAB), ErrorCategory.InvalidOperation, this.Identity);
				}
				offlineAddressBook.ConfiguredAttributes = OfflineAddressBookMapiProperty.DefaultOABPropertyList;
			}
			if (!this.isOriginalPfDistributionEnabled && this.isPresentPfDistributionEnabled && !flag3)
			{
				this.publicFolderDatabase = OfflineAddressBookTaskUtility.FindPublicFolderDatabase(base.GlobalConfigSession, offlineAddressBook.Server, new Task.TaskErrorLoggingDelegate(base.WriteError));
				offlineAddressBook.PublicFolderDatabase = (ADObjectId)this.publicFolderDatabase.Identity;
			}
			if (offlineAddressBook.IsChanged(OfflineAddressBookSchema.VirtualDirectories) || this.isOriginalPfDistributionEnabled != this.isPresentPfDistributionEnabled)
			{
				OfflineAddressBookTaskUtility.WarnForNoDistribution(offlineAddressBook, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.DataObject.IsChanged(OfflineAddressBookSchema.IsDefault) && !this.DataObject.IsDefault)
			{
				base.WriteError(new InvalidOperationException(Strings.CannotResetDefaultOAB), ErrorCategory.InvalidOperation, this.Identity);
			}
			if (this.DataObject.IsModified(OfflineAddressBookSchema.ConfiguredAttributes))
			{
				try
				{
					this.DataObject.UpdateRawMapiAttributes(false);
				}
				catch (ArgumentException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, this.DataObject);
				}
			}
			this.ThrowErrorIfUnsupportedParameterIsSpecified();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			OfflineAddressBook offlineAddressBook = null;
			if (this.DataObject.IsChanged(OfflineAddressBookSchema.IsDefault) && this.DataObject.IsDefault)
			{
				offlineAddressBook = OfflineAddressBookTaskUtility.ResetOldDefaultOab(base.DataSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			bool flag = false;
			try
			{
				base.InternalProcessRecord();
				flag = true;
				if (!this.isOriginalPfDistributionEnabled && this.isPresentPfDistributionEnabled)
				{
					this.WriteWarning(Strings.DoNotMoveImmediately(this.DataObject.Name));
					OfflineAddressBookTaskUtility.DoMaintenanceTask(this.publicFolderDatabase, base.DomainController, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				}
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

		private void ThrowErrorIfUnsupportedParameterIsSpecified()
		{
			if (!this.DataObject.IsE15OrLater())
			{
				return;
			}
			if (this.ApplyMandatoryProperties.IsPresent)
			{
				this.WriteError(new InvalidOperationException(Strings.CannotSpecifyApplyMandatoryPropertiesParameterWithE15OrLaterOab), ErrorCategory.InvalidOperation, this.Identity, true);
			}
			foreach (ADPropertyDefinition adpropertyDefinition in SetOfflineAddressBookInternal.ParametersUnsupportedForOAB15OrLater)
			{
				if (this.DataObject.IsModified(adpropertyDefinition))
				{
					this.WriteError(new InvalidOperationException(Strings.CannotSpecifyParameterWithE15OrLaterOab(adpropertyDefinition.Name)), ErrorCategory.InvalidOperation, this.Identity, true);
				}
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static SetOfflineAddressBookInternal()
		{
			ADPropertyDefinition[,] array = new ADPropertyDefinition[3, 2];
			array[0, 0] = ADConfigurationObjectSchema.AdminDisplayName;
			array[1, 0] = OfflineAddressBookSchema.Server;
			array[2, 0] = OfflineAddressBookSchema.PublicFolderDatabase;
			SetOfflineAddressBookInternal.propertiesCannotBeSet = array;
		}

		private static readonly ADPropertyDefinition[] ParametersUnsupportedForOAB15OrLater = new ADPropertyDefinition[]
		{
			OfflineAddressBookSchema.Schedule,
			OfflineAddressBookSchema.Versions
		};

		private static readonly ADPropertyDefinition[,] propertiesCannotBeSet;

		private bool isOriginalPfDistributionEnabled;

		private bool isPresentPfDistributionEnabled;

		private PublicFolderDatabase publicFolderDatabase;
	}
}
