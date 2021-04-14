using System;
using System.IO;
using System.Management.Automation;
using System.Net.Sockets;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.OAB;
using Microsoft.Mapi;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class OfflineAddressBookTaskUtility
	{
		internal static void ValidatePublicFolderInfrastructure(IConfigDataProvider session, Task.TaskErrorLoggingDelegate errorHandler, Task.TaskWarningLoggingDelegate warningHandler, bool publicFolderDistributionEnabled)
		{
			IConfigurable[] array = session.Find<PublicFolderDatabase>(null, null, true, null);
			if (array.Length == 0)
			{
				if (publicFolderDistributionEnabled)
				{
					errorHandler(new InvalidOperationException(Strings.ErrorPublicFolderFree), ErrorCategory.InvalidOperation, null);
					return;
				}
				warningHandler(Strings.WarningPublicFolderFree);
			}
		}

		internal static PublicFolderDatabase FindPublicFolderDatabase(IConfigDataProvider session, ADObjectId server, Task.TaskErrorLoggingDelegate errorHandler)
		{
			Server server2 = (Server)session.Read<Server>(server);
			PublicFolderDatabase[] publicFolderDatabases = server2.GetPublicFolderDatabases();
			PublicFolderDatabase result;
			if (publicFolderDatabases.Length == 0)
			{
				ADObjectId identity = PublicFolderDatabase.FindClosestPublicFolderDatabase(session, server);
				result = (PublicFolderDatabase)session.Read<PublicFolderDatabase>(identity);
			}
			else
			{
				result = publicFolderDatabases[0];
			}
			return result;
		}

		internal static OfflineAddressBook ResetOldDefaultOab(IConfigDataProvider session, Task.TaskErrorLoggingDelegate errorHandler)
		{
			OfflineAddressBook offlineAddressBook = null;
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, OfflineAddressBookSchema.IsDefault, true);
			IConfigurable[] array = session.Find<OfflineAddressBook>(filter, null, true, null);
			if (array.Length > 0)
			{
				offlineAddressBook = (OfflineAddressBook)array[0];
				offlineAddressBook.IsDefault = false;
				session.Save(offlineAddressBook);
			}
			return offlineAddressBook;
		}

		internal static MultiValuedProperty<ADObjectId> ValidateAddressBook(IConfigDataProvider session, AddressBookBaseIdParameter[] addressBooks, OfflineAddressBookTaskUtility.GetUniqueObject getAddressBook, OfflineAddressBook target, Task.TaskErrorLoggingDelegate writeError)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>(false, OfflineAddressBookSchema.AddressLists, new object[0]);
			if (addressBooks != null)
			{
				foreach (AddressBookBaseIdParameter addressBookBaseIdParameter in addressBooks)
				{
					if (addressBookBaseIdParameter != null)
					{
						IConfigurable configurable = getAddressBook(addressBookBaseIdParameter, session, null, new LocalizedString?(Strings.ErrorAddressListOrGlobalAddressListNotFound(addressBookBaseIdParameter.ToString())), new LocalizedString?(Strings.ErrorAddressListOrGlobalAddressListNotUnique(addressBookBaseIdParameter.ToString())));
						if (configurable != null)
						{
							if (multiValuedProperty.Contains((ADObjectId)configurable.Identity))
							{
								writeError(new InvalidOperationException(Strings.ErrorOabALAlreadyAssigned((target.Identity != null) ? target.Identity.ToString() : target.Name, configurable.Identity.ToString())), ErrorCategory.InvalidOperation, target.Identity);
							}
							else
							{
								multiValuedProperty.Add((ADObjectId)configurable.Identity);
							}
						}
					}
				}
			}
			return multiValuedProperty;
		}

		internal static MultiValuedProperty<ADObjectId> ValidateVirtualDirectory(IConfigDataProvider session, VirtualDirectoryIdParameter[] virtualDirectories, OfflineAddressBookTaskUtility.GetUniqueObject getOabVirtualDirectory, OfflineAddressBook target, Task.TaskErrorLoggingDelegate writeError)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>(false, OfflineAddressBookSchema.VirtualDirectories, new object[0]);
			foreach (VirtualDirectoryIdParameter virtualDirectoryIdParameter in virtualDirectories)
			{
				if (virtualDirectoryIdParameter != null)
				{
					IConfigurable configurable = getOabVirtualDirectory(virtualDirectoryIdParameter, session, null, new LocalizedString?(Strings.OabVirtualDirectoryNotExisting(virtualDirectoryIdParameter.ToString())), new LocalizedString?(Strings.OabVirtualDirectoryAmbiguous(virtualDirectoryIdParameter.ToString())));
					if (configurable != null)
					{
						if (multiValuedProperty.Contains((ADObjectId)configurable.Identity))
						{
							writeError(new InvalidOperationException(Strings.ErrorOabVDirAlreadyAssigned((target.Identity != null) ? target.Identity.ToString() : target.Name, configurable.Identity.ToString())), ErrorCategory.InvalidOperation, target.Identity);
						}
						else
						{
							multiValuedProperty.Add((ADObjectId)configurable.Identity);
						}
					}
				}
			}
			return multiValuedProperty;
		}

		internal static ADObjectId ValidateGeneratingMailbox(IConfigDataProvider session, MailboxIdParameter generatingMailboxId, OfflineAddressBookTaskUtility.GetUniqueObject getAdUser, OfflineAddressBook target, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError)
		{
			ADObjectId result = null;
			if (OABVariantConfigurationSettings.IsLinkedOABGenMailboxesEnabled)
			{
				if (generatingMailboxId == null)
				{
					writeWarning(Strings.WarningGeneratingMailboxIsNullOABWillNotBeGenerated);
				}
				else
				{
					ADUser aduser = (ADUser)getAdUser(generatingMailboxId, session, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(generatingMailboxId.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotUnique(generatingMailboxId.ToString())));
					if (aduser.RecipientTypeDetails != RecipientTypeDetails.ArbitrationMailbox || aduser.PersistedCapabilities == null || !aduser.PersistedCapabilities.Contains(Capability.OrganizationCapabilityOABGen))
					{
						writeError(new InvalidOperationException(Strings.ErrorGeneratingMailboxInvalid(aduser.Name)), ErrorCategory.InvalidOperation, target.Identity);
					}
					result = aduser.Id;
				}
			}
			else if (generatingMailboxId != null)
			{
				writeError(new InvalidOperationException(Strings.ErrorLinkedMailboxesAreNotSupported), ErrorCategory.InvalidOperation, target.Identity);
			}
			return result;
		}

		internal static bool IsKB922817InstalledOnServer(string computerName, Task.TaskErrorLoggingDelegate errorHandler)
		{
			Exception ex = null;
			bool result = false;
			try
			{
				using (RegistryKey registryKey = RegistryUtil.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\KB922817"))
					{
						result = (registryKey2 != null);
					}
				}
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				errorHandler(new InvalidOperationException(Strings.ErrorAccessingRegistryRaisesException(computerName, ex.Message)), ErrorCategory.ReadError, null);
			}
			return result;
		}

		internal static void WarnForNoDistribution(OfflineAddressBook dataObject, Task.TaskWarningLoggingDelegate warningHandler)
		{
			if (!dataObject.PublicFolderDistributionEnabled && !dataObject.WebDistributionEnabled)
			{
				warningHandler(Strings.WarningOABWithoutDistribution);
			}
		}

		internal static void DoMaintenanceTask(PublicFolderDatabase publicStore, string domainController, Task.TaskWarningLoggingDelegate warningHandler)
		{
			Server server = publicStore.GetServer();
			if (!server.IsExchange2007OrLater)
			{
				warningHandler(Strings.WarningSiteFolderCheckTaskNotAvailableOnTiServer(server.Name));
				return;
			}
			string a = domainController;
			try
			{
				a = SystemConfigurationTasksHelper.GetConfigurationDomainControllerFqdn(domainController);
			}
			catch (SocketException ex)
			{
				warningHandler(Strings.ErrorResolveFqdnForDomainController(domainController, ex.Message));
			}
			string configDC = ADSession.GetConfigDC(TopologyProvider.LocalForestFqdn, server.Name);
			if (string.Equals(a, configDC, StringComparison.InvariantCultureIgnoreCase))
			{
				try
				{
					using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", server.Fqdn, null, null, null))
					{
						exRpcAdmin.DoMaintenanceTask(publicStore.Guid, MaintenanceTask.SiteFolderCheck);
					}
					return;
				}
				catch (MapiPermanentException ex2)
				{
					TaskLogger.Trace("Set/New-OfflineAddressBook.InternalProcessRecord raises exception while running site folder check task: {0}", new object[]
					{
						ex2.Message
					});
					warningHandler(Strings.ErrorFailedToRunSiteFolderCheckTask(server.Name, ex2.Message));
					return;
				}
				catch (MapiRetryableException ex3)
				{
					TaskLogger.Trace("Set/New-OfflineAddressBook.InternalProcessRecord raises exception while running site folder check task: {0}", new object[]
					{
						ex3.Message
					});
					warningHandler(Strings.ErrorFailedToRunSiteFolderCheckTask(server.Name, ex3.Message));
					return;
				}
			}
			warningHandler(Strings.WarningOabSiteFolderCheckNotRun(server.Name));
		}

		internal delegate IConfigurable GetUniqueObject(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError);
	}
}
