using System;
using System.IO;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Trigger;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Move", "OfflineAddressBook", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class MoveOfflineAddressBook : SystemConfigurationObjectActionTask<OfflineAddressBookIdParameter, OfflineAddressBook>
	{
		[Parameter(Mandatory = false)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return this.confirmationMessage;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			OfflineAddressBook offlineAddressBook = (OfflineAddressBook)base.PrepareDataObject();
			if (this.provisionedCurrentServer)
			{
				this.confirmationMessage = LocalizedString.Empty;
				TaskLogger.LogExit();
				return offlineAddressBook;
			}
			if (this.targetServer == null)
			{
				this.targetServer = (Server)base.GetDataObject<Server>(this.Server, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			}
			if (offlineAddressBook.IsE15OrLater())
			{
				this.WriteError(new InvalidOperationException(Strings.CannotMoveE15OrLaterOab), ErrorCategory.InvalidOperation, offlineAddressBook.Identity, true);
			}
			if (this.targetServer.IsE15OrLater)
			{
				this.WriteError(new InvalidOperationException(Strings.CannotMoveOabToE15OrLaterServer), ErrorCategory.InvalidOperation, offlineAddressBook.Identity, true);
			}
			bool flag = false;
			try
			{
				this.sourceServer = (Server)base.GetDataObject<Server>(new ServerIdParameter(offlineAddressBook.Server), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(offlineAddressBook.Server.Name.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(offlineAddressBook.Server.Name.ToString())));
			}
			catch (ArgumentNullException)
			{
				flag = true;
			}
			catch (ManagementObjectNotFoundException)
			{
				flag = true;
			}
			if (flag)
			{
				this.WriteWarning(Strings.WarningOABSourceServerDoesNotExist(offlineAddressBook.Server.Name));
			}
			if (this.sourceServer != null && this.targetServer.Guid == this.sourceServer.Guid)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorMovingOabAlreadyThere(this.Identity.ToString(), this.targetServer.Name.ToString())), ErrorCategory.InvalidOperation, offlineAddressBook.Identity);
			}
			if (this.targetServer.IsExchange2007OrLater)
			{
				if (!this.targetServer.IsMailboxServer)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorMoveOabToNoMailboxServer(this.Server.ToString())), ErrorCategory.InvalidOperation, offlineAddressBook.Identity);
				}
				if (offlineAddressBook.Versions.Contains(OfflineAddressBookVersion.Version1))
				{
					MultiValuedProperty<OfflineAddressBookVersion> multiValuedProperty = new MultiValuedProperty<OfflineAddressBookVersion>();
					foreach (OfflineAddressBookVersion offlineAddressBookVersion in offlineAddressBook.Versions)
					{
						if (offlineAddressBookVersion != OfflineAddressBookVersion.Version1)
						{
							multiValuedProperty.Add(offlineAddressBookVersion);
						}
					}
					offlineAddressBook.Versions = multiValuedProperty;
				}
				if (this.sourceServer == null)
				{
					if (this.targetServer.IsE14OrLater)
					{
						offlineAddressBook.ExchangeVersion = ExchangeObjectVersion.Exchange2010;
						if (offlineAddressBook.Versions.Contains(OfflineAddressBookVersion.Version4))
						{
							offlineAddressBook.ConfiguredAttributes = OfflineAddressBookMapiProperty.DefaultOABPropertyList;
						}
					}
					else
					{
						this.ClearNewAttributesForOldExchange(offlineAddressBook, ExchangeObjectVersion.Exchange2007);
						offlineAddressBook.ExchangeVersion = ExchangeObjectVersion.Exchange2007;
					}
					offlineAddressBook.MinAdminVersion = new int?(offlineAddressBook.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32());
					offlineAddressBook.DiffRetentionPeriod = new Unlimited<int>?(OfflineAddressBook.DefaultDiffRetentionPeriod);
				}
				else if (this.targetServer.IsE14OrLater)
				{
					if (!this.sourceServer.IsE14OrLater)
					{
						if (this.sourceServer.IsExchange2007OrLater)
						{
							offlineAddressBook.ExchangeVersion = ExchangeObjectVersion.Exchange2010;
							offlineAddressBook.MinAdminVersion = new int?(offlineAddressBook.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32());
							if (offlineAddressBook.Versions.Contains(OfflineAddressBookVersion.Version4))
							{
								offlineAddressBook.ConfiguredAttributes = OfflineAddressBookMapiProperty.DefaultOABPropertyList;
							}
						}
						else
						{
							offlineAddressBook.ExchangeVersion = ExchangeObjectVersion.Exchange2010;
							offlineAddressBook.MinAdminVersion = new int?(offlineAddressBook.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32());
							offlineAddressBook.DiffRetentionPeriod = new Unlimited<int>?(OfflineAddressBook.DefaultDiffRetentionPeriod);
							if (offlineAddressBook.Versions.Contains(OfflineAddressBookVersion.Version4))
							{
								offlineAddressBook.ConfiguredAttributes = OfflineAddressBookMapiProperty.DefaultOABPropertyList;
							}
						}
					}
				}
				else if (this.sourceServer.IsE14OrLater)
				{
					this.ClearNewAttributesForOldExchange(offlineAddressBook, ExchangeObjectVersion.Exchange2007);
					offlineAddressBook.ExchangeVersion = ExchangeObjectVersion.Exchange2007;
					offlineAddressBook.MinAdminVersion = new int?(offlineAddressBook.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32());
				}
				else if (!this.sourceServer.IsExchange2007OrLater)
				{
					offlineAddressBook.ExchangeVersion = ExchangeObjectVersion.Exchange2007;
					offlineAddressBook.MinAdminVersion = new int?(offlineAddressBook.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32());
					offlineAddressBook.DiffRetentionPeriod = new Unlimited<int>?(OfflineAddressBook.DefaultDiffRetentionPeriod);
				}
			}
			else
			{
				if (this.targetServer.IsPreE12FrontEnd)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorMoveOabToTiFrondEndServer), ErrorCategory.InvalidOperation, offlineAddressBook.Identity);
				}
				if (this.sourceServer != null && !this.sourceServer.IsExchange2007OrLater)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorMoveOabBetweenTwoTiServers), ErrorCategory.InvalidOperation, offlineAddressBook.Identity);
				}
				if (offlineAddressBook.Versions.Contains(OfflineAddressBookVersion.Version4) && !this.targetServer.IsExchange2003Sp3OrLater && !OfflineAddressBookTaskUtility.IsKB922817InstalledOnServer(this.targetServer.Fqdn, new Task.TaskErrorLoggingDelegate(base.WriteError)))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorMoveVersion4OabToTiServerWithoutSP3(offlineAddressBook.Identity.ToString())), ErrorCategory.InvalidOperation, offlineAddressBook.Identity);
				}
				if (offlineAddressBook.WebDistributionEnabled)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorMoveWebDistributionEnabledOabToTiServer), ErrorCategory.InvalidOperation, offlineAddressBook.Identity);
				}
				if (!offlineAddressBook.PublicFolderDistributionEnabled)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorMovePublicFolderDistributionDisabledOabToTiServer(this.Identity.ToString())), ErrorCategory.InvalidOperation, offlineAddressBook.Identity);
				}
				this.ClearNewAttributesForOldExchange(offlineAddressBook, ExchangeObjectVersion.Exchange2003);
				offlineAddressBook.DiffRetentionPeriod = new Unlimited<int>?(Unlimited<int>.UnlimitedValue);
				offlineAddressBook.MinAdminVersion = null;
				offlineAddressBook.ExchangeVersion = ExchangeObjectVersion.Exchange2003;
			}
			offlineAddressBook.Server = (ADObjectId)this.targetServer.Identity;
			if (this.targetServer.IsE14OrLater)
			{
				offlineAddressBook.LastTouchedTime = new DateTime?((DateTime)ExDateTime.Now);
			}
			if (!this.targetServer.IsE14OrLater && this.sourceServer != null && this.sourceServer.IsE14OrLater)
			{
				this.confirmationMessage = Strings.ConfirmationMessageMoveOfflineAddressBookE14ToLowerVersion(this.Identity.ToString(), this.Server.ToString());
			}
			else
			{
				this.confirmationMessage = Strings.ConfirmationMessageMoveOfflineAddressBook(this.Identity.ToString(), this.Server.ToString());
			}
			if (this.DataObject.IsModified(OfflineAddressBookSchema.ConfiguredAttributes))
			{
				this.DataObject.UpdateRawMapiAttributes(!this.targetServer.IsE14OrLater);
			}
			TaskLogger.LogExit();
			return offlineAddressBook;
		}

		protected override void InternalProcessRecord()
		{
			if (this.provisionedCurrentServer)
			{
				return;
			}
			TaskLogger.LogEnter();
			int num = 0;
			string text = null;
			if (this.sourceServer != null && this.sourceServer.IsExchange2007OrLater && this.targetServer.IsExchange2007OrLater && this.DataObject.Versions.Contains(OfflineAddressBookVersion.Version4))
			{
				num = this.CopyOabFiles(out text);
			}
			base.InternalProcessRecord();
			if (text != null && num > 0)
			{
				this.WriteWarning(Strings.OabFilesLeft(this.Identity.ToString(), text));
			}
			if (this.targetServer.IsExchange2007OrLater && this.sourceServer != null && !this.sourceServer.IsExchange2007OrLater && this.DataObject.Versions.Contains(OfflineAddressBookVersion.Version4))
			{
				this.WriteWarning(Strings.TiE12Warning(this.Identity.ToString(), this.Server.ToString()));
			}
			TaskLogger.LogExit();
		}

		private int CopyOabFiles(out string sourceLocation)
		{
			int num = 0;
			sourceLocation = null;
			string text = Path.Combine("\\\\" + this.sourceServer.Name, "ExchangeOAB");
			Exception innerException = null;
			LocalizedString value;
			try
			{
				if (!Directory.Exists(text))
				{
					return num;
				}
				sourceLocation = Path.Combine(text, this.DataObject.Guid.ToString());
				DirectoryInfo directoryInfo = new DirectoryInfo(sourceLocation);
				if (!directoryInfo.Exists)
				{
					return num;
				}
				string text2 = Path.Combine("\\\\" + this.targetServer.Name, "ExchangeOAB");
				if (!Directory.Exists(text2))
				{
					try
					{
						TriggerPrivateRpcClient triggerPrivateRpcClient = new TriggerPrivateRpcClient(this.targetServer.Name);
						triggerPrivateRpcClient.CreateExchangeOabFolder();
					}
					catch (RpcException ex)
					{
						base.WriteError(new LocalizedException(Strings.ErrorFailedToCreateExchangeOabFolder(this.targetServer.Name, ex.Message), ex), ErrorCategory.InvalidResult, this.Identity);
						return num;
					}
					catch (COMException ex2)
					{
						base.WriteError(new LocalizedException(Strings.ErrorFailedToCreateExchangeOabFolder(this.targetServer.Name, ex2.Message), ex2), ErrorCategory.InvalidResult, this.Identity);
						return num;
					}
				}
				string text3 = Path.Combine(text2, this.DataObject.Guid.ToString());
				DirectoryInfo directoryInfo2 = new DirectoryInfo(text3);
				if (!directoryInfo2.Exists)
				{
					directoryInfo2.Create();
				}
				FileInfo[] files = directoryInfo.GetFiles();
				if (files.Length == 0)
				{
					return num;
				}
				long num2 = 0L;
				long num3 = 0L;
				foreach (FileInfo fileInfo in files)
				{
					num2 += fileInfo.Length;
				}
				if (num2 == 0L)
				{
					return num;
				}
				LocalizedString activity = Strings.CopyingOABFiles(this.DataObject.Name, this.targetServer.Name);
				foreach (FileInfo fileInfo2 in files)
				{
					LocalizedString statusDescription = Strings.CopyingFile(fileInfo2.Name);
					int percentCompleted = (int)(num3 * 100L / num2);
					base.WriteProgress(activity, statusDescription, percentCompleted);
					fileInfo2.CopyTo(Path.Combine(text3, fileInfo2.Name), true);
					num++;
					num3 += fileInfo2.Length;
				}
				return num;
			}
			catch (PathTooLongException ex3)
			{
				innerException = ex3;
				value = Strings.ErrorPathTooLong;
			}
			catch (SecurityException ex4)
			{
				innerException = ex4;
				value = Strings.ErrorNotEnoughPermissions;
			}
			catch (UnauthorizedAccessException ex5)
			{
				innerException = ex5;
				value = Strings.ErrorNotEnoughPermissions;
			}
			catch (DirectoryNotFoundException ex6)
			{
				innerException = ex6;
				value = Strings.ErrorDirectoryNotFound;
			}
			catch (IOException ex7)
			{
				innerException = ex7;
				value = Strings.ErrorDirectoryNotEmpty;
			}
			base.WriteError(new InvalidOperationException(value, innerException), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			return 0;
		}

		protected override void InternalBeginProcessing()
		{
			if (this.Server == null)
			{
				if (!base.IsProvisioningLayerAvailable)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorAutomaticProvisioningFailedToFindDatabase("Server")), ErrorCategory.InvalidData, null);
				}
				else
				{
					this.isServerRequired = true;
				}
			}
			base.InternalBeginProcessing();
		}

		private void ClearNewAttributesForOldExchange(OfflineAddressBook dataObject, ExchangeObjectVersion version)
		{
			foreach (PropertyDefinition propertyDefinition in dataObject.Schema.AllProperties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (((version.IsOlderThan(adpropertyDefinition.VersionAdded) && !adpropertyDefinition.IsCalculated) || (version == ExchangeObjectVersion.Exchange2007 && adpropertyDefinition == OfflineAddressBookSchema.ANRProperties) || (version == ExchangeObjectVersion.Exchange2007 && adpropertyDefinition == OfflineAddressBookSchema.DetailsProperties) || (version == ExchangeObjectVersion.Exchange2007 && adpropertyDefinition == OfflineAddressBookSchema.TruncatedProperties)) && !dataObject.ExchangeVersion.IsOlderThan(adpropertyDefinition.VersionAdded))
				{
					if (adpropertyDefinition.IsMultivalued)
					{
						((MultiValuedPropertyBase)dataObject[adpropertyDefinition]).Clear();
					}
					else
					{
						dataObject[adpropertyDefinition] = null;
					}
				}
			}
		}

		protected override void ProvisionDefaultValues(IConfigurable temporaryObject, IConfigurable dataObject)
		{
			this.DataObject = (OfflineAddressBook)dataObject;
			OfflineAddressBook dataObject2 = new OfflineAddressBook();
			ProvisioningLayer.ProvisionDefaultProperties(this, this.ConvertDataObjectToPresentationObject(temporaryObject), this.ConvertDataObjectToPresentationObject(dataObject2), false);
			this.ValidateProvisionedProperties(dataObject2);
		}

		protected override void ValidateProvisionedProperties(IConfigurable dataObject)
		{
			if (this.isServerRequired)
			{
				OfflineAddressBook offlineAddressBook = dataObject as OfflineAddressBook;
				if (offlineAddressBook != null && offlineAddressBook.IsChanged(OfflineAddressBookSchema.Server))
				{
					if (offlineAddressBook.Server.Equals(this.DataObject.Server))
					{
						this.provisionedCurrentServer = true;
						this.WriteWarning(Strings.WarningProvisionedServerAlreadyAssigned(offlineAddressBook.Server.ToString()));
						return;
					}
					this.Server = new ServerIdParameter(offlineAddressBook.Server);
					return;
				}
				else
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorParameterRequiredButNotProvisioned("Server")), ErrorCategory.InvalidData, null);
				}
			}
		}

		private Server sourceServer;

		private Server targetServer;

		private bool isServerRequired;

		private LocalizedString confirmationMessage = LocalizedString.Empty;

		private bool provisionedCurrentServer;
	}
}
