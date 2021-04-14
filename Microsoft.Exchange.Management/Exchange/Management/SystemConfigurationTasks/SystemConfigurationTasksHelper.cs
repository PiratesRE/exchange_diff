using System;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class SystemConfigurationTasksHelper
	{
		public static bool IsKnownWmiException(Exception e)
		{
			return typeof(WmiException).IsInstanceOfType(e);
		}

		public static bool IsKnownMapiDotNETException(Exception e)
		{
			return typeof(MapiPermanentException).IsInstanceOfType(e) || typeof(MapiRetryableException).IsInstanceOfType(e);
		}

		public static bool IsKnownClusterUpdateDatabaseResourceException(Exception e)
		{
			return typeof(ClusterException).IsInstanceOfType(e);
		}

		internal static string GetConfigurationDomainControllerFqdn(string domainController)
		{
			if (string.IsNullOrEmpty(domainController))
			{
				return ADSession.GetCurrentConfigDCForLocalForest();
			}
			Fqdn fqdn;
			if (!Fqdn.TryParse(domainController, out fqdn) || 0 >= domainController.IndexOf('.'))
			{
				domainController = Dns.GetHostEntry(domainController).HostName;
			}
			return domainController;
		}

		internal static void PrepareDomainControllerRecipientSessionForUpdate(IRecipientSession domainControllerSession, ADObjectId recipientId, string domainController, string domainControllerDomainName)
		{
			if (domainControllerSession == null)
			{
				throw new ArgumentNullException("domainControllerSession");
			}
			if (recipientId == null)
			{
				throw new ArgumentNullException("recipientId");
			}
			if (string.IsNullOrEmpty(recipientId.DistinguishedName))
			{
				throw new ArgumentNullException("recipientId.DistinguishedName");
			}
			if (string.Equals(recipientId.DescendantDN(1).Rdn.UnescapedName, "Configuration", StringComparison.OrdinalIgnoreCase))
			{
				domainControllerSession.UseConfigNC = true;
				domainControllerSession.DomainController = domainController;
				return;
			}
			if (!string.IsNullOrEmpty(domainControllerDomainName) && string.Equals(domainControllerDomainName, DNConvertor.FqdnFromDomainDistinguishedName(recipientId.DomainId.DistinguishedName), StringComparison.InvariantCultureIgnoreCase))
			{
				domainControllerSession.UseConfigNC = false;
				domainControllerSession.DomainController = domainController;
				return;
			}
			domainControllerSession.UseConfigNC = false;
			domainControllerSession.DomainController = null;
		}

		public static bool TryCreateDirectory(string serverFQDN, string directoryPath, FileSystemAccessRule[] directoryPermissions, Task.TaskVerboseLoggingDelegate verboseDelegate, Task.TaskWarningLoggingDelegate warningDelegate)
		{
			bool flag = false;
			if (string.Compare(serverFQDN, NativeHelpers.GetLocalComputerFqdn(false), StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				try
				{
					verboseDelegate(Strings.VerboseCheckDirectoryExistenceCondition(serverFQDN, directoryPath));
					if (Directory.Exists(directoryPath))
					{
						if (directoryPermissions != null)
						{
							DirectorySecurity directorySecurity = SystemConfigurationTasksHelper.CreateDirectorySecurityHelper(directoryPermissions);
							directorySecurity.SetAccessRuleProtection(true, false);
							verboseDelegate(Strings.VerboseSetAccessControl(serverFQDN, directoryPath));
							TaskLogger.Trace("Directory \"" + directoryPath + "\" already exists on the local machine, just set the access control.", new object[0]);
							Directory.SetAccessControl(directoryPath, directorySecurity);
							flag = true;
						}
						else
						{
							verboseDelegate(Strings.VerboseNoAccessControlSpecified(serverFQDN, directoryPath));
							TaskLogger.Trace("No access control specified to be set on directory \"" + directoryPath + "\".", new object[0]);
							flag = true;
						}
					}
					else if (directoryPermissions != null)
					{
						DirectorySecurity directorySecurity = SystemConfigurationTasksHelper.CreateDirectorySecurityHelper(directoryPermissions);
						directorySecurity.SetAccessRuleProtection(true, false);
						verboseDelegate(Strings.VerboseCreateDirectory(serverFQDN, directoryPath));
						TaskLogger.Trace("Directory \"" + directoryPath + "\" does not exist.  Create the directory on the local machine and set the access control.", new object[0]);
						Directory.CreateDirectory(directoryPath, directorySecurity);
						flag = true;
					}
					else
					{
						verboseDelegate(Strings.VerboseCreateDirectory(serverFQDN, directoryPath));
						verboseDelegate(Strings.VerboseNoAccessControlSpecified(serverFQDN, directoryPath));
						TaskLogger.Trace("Directory \"" + directoryPath + "\" does not exist.  Create the directory on the local machine.  No access controls specified to set on the directory.", new object[0]);
						Directory.CreateDirectory(directoryPath);
						flag = true;
					}
					goto IL_43C;
				}
				catch (NotSupportedException ex)
				{
					verboseDelegate(Strings.VerboseFailedCreateDirectory(serverFQDN, directoryPath, ex.Message));
					TaskLogger.Trace("this.TryCreateDirectory() raised exception: {0}", new object[]
					{
						ex.ToString()
					});
					goto IL_43C;
				}
				catch (ArgumentNullException ex2)
				{
					verboseDelegate(Strings.VerboseFailedCreateDirectory(serverFQDN, directoryPath, ex2.Message));
					TaskLogger.Trace("this.TryCreateDirectory() raised exception: {0}", new object[]
					{
						ex2.ToString()
					});
					goto IL_43C;
				}
				catch (UnauthorizedAccessException ex3)
				{
					verboseDelegate(Strings.VerboseFailedCreateDirectory(serverFQDN, directoryPath, ex3.Message));
					TaskLogger.Trace("this.TryCreateDirectory() raised exception: {0}", new object[]
					{
						ex3.ToString()
					});
					goto IL_43C;
				}
				catch (ArgumentException ex4)
				{
					verboseDelegate(Strings.VerboseFailedCreateDirectory(serverFQDN, directoryPath, ex4.Message));
					TaskLogger.Trace("this.TryCreateDirectory() raised exception: {0}", new object[]
					{
						ex4.ToString()
					});
					goto IL_43C;
				}
				catch (IOException ex5)
				{
					verboseDelegate(Strings.VerboseFailedCreateDirectory(serverFQDN, directoryPath, ex5.Message));
					TaskLogger.Trace("this.TryCreateDirectory() raised exception: {0}", new object[]
					{
						ex5.ToString()
					});
					goto IL_43C;
				}
			}
			try
			{
				verboseDelegate(Strings.VerboseCheckDirectoryExistenceCondition(serverFQDN, directoryPath));
				uint num;
				if (!WmiWrapper.IsDirectoryExisting(serverFQDN, directoryPath))
				{
					verboseDelegate(Strings.VerboseCreateDirectory(serverFQDN, directoryPath));
					TaskLogger.Trace(string.Concat(new string[]
					{
						"Create the directory \"",
						directoryPath,
						"\" on the remote machine \"",
						serverFQDN,
						"\"."
					}), new object[0]);
					num = WmiWrapper.CreateDirectory(serverFQDN, directoryPath);
				}
				else
				{
					num = 0U;
				}
				if (num == 0U)
				{
					if (directoryPermissions != null)
					{
						verboseDelegate(Strings.VerboseSetAccessControl(serverFQDN, directoryPath));
						TaskLogger.Trace(string.Concat(new string[]
						{
							"Set the access control for directory \"",
							directoryPath,
							"\" on the remote machine \"",
							serverFQDN,
							"\"."
						}), new object[0]);
						num = WmiWrapper.ChangeSecurityPermissions(serverFQDN, directoryPath, directoryPermissions);
						if (num == 0U)
						{
							flag = true;
						}
						else
						{
							verboseDelegate(Strings.VerboseFailedSetAccessControl(serverFQDN, directoryPath, "WMI Status: " + num.ToString()));
							TaskLogger.Trace("this.TryCreateDirectory() failed to set the access control on the directory - WMI Status: {0}", new object[]
							{
								num.ToString()
							});
						}
					}
					else
					{
						verboseDelegate(Strings.VerboseNoAccessControlSpecified(serverFQDN, directoryPath));
						TaskLogger.Trace("No access control specified to be set on directory \"" + directoryPath + "\".", new object[0]);
						flag = true;
					}
				}
				else
				{
					verboseDelegate(Strings.VerboseFailedCreateDirectory(serverFQDN, directoryPath, "WMI Status: " + num.ToString()));
					TaskLogger.Trace("this.TryCreateDirectory() failed to create the directory - WMI Status: {0}", new object[]
					{
						num.ToString()
					});
				}
			}
			catch (WmiException ex6)
			{
				verboseDelegate(Strings.VerboseFailedCreateDirectory(serverFQDN, directoryPath, ex6.Message));
				TaskLogger.Trace("this.TryCreateDirectory() raised exception: {0}", new object[]
				{
					ex6.ToString()
				});
			}
			catch (UnauthorizedAccessException ex7)
			{
				verboseDelegate(Strings.ErrorFailedToConnectToServer(serverFQDN, ex7.Message));
				TaskLogger.Trace("this.TryCreateDirectory() raised exception: {0}", new object[]
				{
					ex7.ToString()
				});
			}
			IL_43C:
			if (!flag)
			{
				warningDelegate(Strings.WarningFailedCreateDirectory(serverFQDN, directoryPath));
			}
			return flag;
		}

		private static DirectorySecurity CreateDirectorySecurityHelper(FileSystemAccessRule[] directoryPermissions)
		{
			DirectorySecurity directorySecurity = new DirectorySecurity();
			for (int i = 0; i < directoryPermissions.Length; i++)
			{
				directorySecurity.AddAccessRule(directoryPermissions[i]);
			}
			return directorySecurity;
		}

		internal static int GenerateVersionNumber(Version version)
		{
			return 1879080960 | (version.Major & 63) << 22 | (version.Minor & 63) << 16 | (version.Build & 32767);
		}

		internal static void DoubleWrite<TDataObject>(ObjectId objectId, TDataObject scratchPad, string ownerServerName, Fqdn domainController, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskWarningLoggingDelegate writeWarning) where TDataObject : ADConfigurationObject, new()
		{
			string configDC;
			try
			{
				writeVerbose(Strings.VerboseGetStoreConfigDCName(ownerServerName));
				configDC = ADSession.GetConfigDC(TopologyProvider.LocalForestFqdn, ownerServerName);
			}
			catch (ADTransientException ex)
			{
				writeWarning(Strings.VerboseFailedDoubleWriteOperation(ex.Message));
				TaskLogger.Trace("Failed to get the domain controller which store is using: {0}.", new object[]
				{
					ex.Message
				});
				return;
			}
			if (domainController == null || !domainController.ToString().Equals(configDC, StringComparison.InvariantCultureIgnoreCase))
			{
				IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(configDC, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 416, "DoubleWrite", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\common\\SystemConfigurationCommon.cs");
				writeVerbose(TaskVerboseStringHelper.GetReadObjectVerboseString(objectId, configurationSession, typeof(TDataObject)));
				try
				{
					TDataObject tdataObject = configurationSession.Read<TDataObject>((ADObjectId)objectId);
					writeVerbose(TaskVerboseStringHelper.GetSourceVerboseString(configurationSession));
					if (tdataObject == null)
					{
						writeVerbose(Strings.VerboseFailedReadObjectFromDC(objectId.ToString(), typeof(TDataObject).Name, configDC));
						TaskLogger.Trace("Failed to read the data object from config DC: \"{0}\"", new object[]
						{
							configDC
						});
						TaskLogger.LogExit();
						return;
					}
					tdataObject.CopyChangesFrom(scratchPad);
					writeVerbose(Strings.VerboseDoubleWriteADChangeOnDC(objectId.ToString(), configDC));
					configurationSession.Save(tdataObject);
				}
				catch (DataValidationException ex2)
				{
					writeWarning(Strings.VerboseFailedDoubleWriteOperation(ex2.Message));
					TaskLogger.Trace("Double write failed on DC '{0}': {1}", new object[]
					{
						configDC,
						ex2.ToString()
					});
				}
				catch (ADOperationException ex3)
				{
					writeWarning(Strings.VerboseFailedDoubleWriteOperation(ex3.Message));
					TaskLogger.Trace("Double write failed on DC '{0}': {1}", new object[]
					{
						configDC,
						ex3.ToString()
					});
				}
				catch (DataSourceTransientException ex4)
				{
					writeWarning(Strings.VerboseFailedDoubleWriteOperation(ex4.Message));
					TaskLogger.Trace("Double write failed on DC '{0}': {1}", new object[]
					{
						configDC,
						ex4.ToString()
					});
				}
				finally
				{
					writeVerbose(TaskVerboseStringHelper.GetSourceVerboseString(configurationSession));
				}
			}
			TaskLogger.LogExit();
		}

		internal static string GetCmdletName(Type type)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(CmdletAttribute), false);
			if (customAttributes.Length == 0)
			{
				return string.Empty;
			}
			CmdletAttribute cmdletAttribute = customAttributes[0] as CmdletAttribute;
			StringBuilder stringBuilder = new StringBuilder(cmdletAttribute.VerbName);
			stringBuilder.Append("-");
			stringBuilder.Append(cmdletAttribute.NounName);
			return stringBuilder.ToString();
		}

		internal static ADOperationResult GetExternalDirectoryOrganizationId(IConfigDataProvider dataSession, OrganizationId organizationId, out Guid externalDirectoryOrganizationId)
		{
			Guid tempGuid = Guid.Empty;
			ADOperationResult result = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ExchangeConfigurationUnit exchangeConfigurationUnit = (ExchangeConfigurationUnit)dataSession.Read<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit);
				if (exchangeConfigurationUnit != null)
				{
					Guid.TryParse(exchangeConfigurationUnit.ExternalDirectoryOrganizationId, out tempGuid);
				}
			});
			externalDirectoryOrganizationId = tempGuid;
			return result;
		}
	}
}
