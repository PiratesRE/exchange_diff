using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	internal class FileShareWitness
	{
		public FileShareWitness(ITopologyConfigurationSession adSession, string dagName)
		{
			this.m_adSession = adSession;
			this.m_dagName = dagName;
			this.m_clusterReference = new WeakReference<IAmCluster>(null);
			this.m_shareRemark = string.Format(FileShareWitness.s_shareRemarkFormat, this.m_dagName);
		}

		public FileShareWitness(ITopologyConfigurationSession adSession, string dagName, FileShareWitnessServerName witnessServer, NonRootLocalLongFullPath witnessDirectory) : this(adSession, dagName)
		{
			this.m_witnessServer = witnessServer;
			this.m_witnessDirectory = witnessDirectory;
		}

		public FileShareWitness(ITopologyConfigurationSession adSession, string dagName, FileShareWitnessServerName witnessServer, NonRootLocalLongFullPath witnessDirectory, IAmCluster cluster) : this(adSession, dagName, witnessServer, witnessDirectory)
		{
			this.m_clusterReference.SetTarget(cluster);
		}

		private List<FileSystemAccessRule> AccessRules
		{
			get
			{
				return this.m_accessRules;
			}
		}

		private void AssignWitnessServerFqdn()
		{
			if (this.WitnessServer.isFqdn)
			{
				this.m_witnessServerFqdn = this.WitnessServer.Fqdn;
				return;
			}
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = this.CreateRootSession();
				ADComputer adcomputer = topologyConfigurationSession.FindComputerByHostName(this.WitnessServer.HostName);
				if (adcomputer == null || string.IsNullOrEmpty(adcomputer.DnsHostName))
				{
					throw new ArgumentException(Strings.ErrorUnableToFindFqdnForHost(this.WitnessServer.HostName), "FileShareWitnessServerName");
				}
				this.m_witnessServerFqdn = adcomputer.DnsHostName;
			}
			catch (ADTransientException ex)
			{
				throw new ErrorUnableToFindFqdnForHostADErrorException(this.WitnessServer.HostName, ex.Message);
			}
			catch (ADExternalException ex2)
			{
				throw new ErrorUnableToFindFqdnForHostADErrorException(this.WitnessServer.HostName, ex2.Message);
			}
			catch (ADOperationException ex3)
			{
				throw new ErrorUnableToFindFqdnForHostADErrorException(this.WitnessServer.HostName, ex3.Message);
			}
		}

		private void RecalculateAccessRules()
		{
			if (this.m_accessRules == null)
			{
				this.m_accessRules = new List<FileSystemAccessRule>();
			}
			string text = NativeHelpers.GetDomainName();
			int num = text.IndexOf('.');
			if (num > -1)
			{
				text = text.Substring(0, num);
			}
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DatabaseAvailabilityGroupSchema.Name, this.m_dagName);
			DatabaseAvailabilityGroup[] array = this.m_adSession.Find<DatabaseAvailabilityGroup>(null, QueryScope.SubTree, filter, null, 1);
			if (array == null || array.Length == 0)
			{
				this.m_accessRules.Clear();
				TaskLogger.Trace("Account {0}$ for witness share \\\\{1}\\{2} does not exist yet, assuming we are working with empty dag, fsw will be created with current user {3} permissions and removed.", new object[]
				{
					this.m_dagName,
					this.WitnessServer,
					this.ShareName,
					WindowsIdentity.GetCurrent().User
				});
				FileSystemAccessRule item = new FileSystemAccessRule(WindowsIdentity.GetCurrent().User.Translate(typeof(NTAccount)) as NTAccount, FileSystemRights.FullControl, AccessControlType.Allow);
				this.m_accessRules.Add(item);
				return;
			}
			MultiValuedProperty<IPAddress> databaseAvailabilityGroupIpAddresses = array[0].DatabaseAvailabilityGroupIpAddresses;
			bool flag = databaseAvailabilityGroupIpAddresses == null || databaseAvailabilityGroupIpAddresses.Count != 1 || !IPAddress.None.Equals(databaseAvailabilityGroupIpAddresses[0]);
			if (flag)
			{
				bool flag2 = true;
				if (this.m_accessRules.Count == 1)
				{
					if (this.m_accessRules[0].IdentityReference.Value.Contains(this.m_dagName))
					{
						flag2 = false;
					}
					else
					{
						this.m_accessRules.Clear();
					}
				}
				if (flag2)
				{
					ITopologyConfigurationSession topologyConfigurationSession = this.CreateRootSession();
					DateTime t = DateTime.UtcNow.AddMinutes(6.0);
					ADComputer adcomputer;
					for (adcomputer = topologyConfigurationSession.FindComputerByHostName(this.m_dagName); adcomputer == null; adcomputer = topologyConfigurationSession.FindComputerByHostName(this.m_dagName))
					{
						if (DateTime.UtcNow > t)
						{
							throw new DagUnableToFindCnoException(this.m_dagName);
						}
						Thread.Sleep(TimeSpan.FromSeconds(20.0));
					}
					IdentityReference identity = adcomputer.Sid.Translate(typeof(NTAccount));
					FileSystemAccessRule item2 = new FileSystemAccessRule(identity, FileSystemRights.FullControl, AccessControlType.Allow);
					this.m_accessRules.Add(item2);
					return;
				}
			}
			else
			{
				bool flag3 = false;
				IAmCluster amCluster = null;
				DatabaseAvailabilityGroup databaseAvailabilityGroup = array[0];
				List<AmServerName> list = new List<AmServerName>();
				try
				{
					if (!this.m_clusterReference.TryGetTarget(out amCluster) || (amCluster is AmCluster && ((AmCluster)amCluster).IsDisposed))
					{
						amCluster = null;
						foreach (ADObjectId adobjectId in databaseAvailabilityGroup.Servers)
						{
							AmServerName amServerName = new AmServerName(adobjectId);
							if (!databaseAvailabilityGroup.StoppedMailboxServers.Contains(amServerName.Fqdn) && (databaseAvailabilityGroup.ServersInMaintenance == null || !databaseAvailabilityGroup.ServersInMaintenance.Contains(adobjectId)))
							{
								amCluster = ClusterFactory.Instance.OpenByName(amServerName);
								if (amCluster != null)
								{
									flag3 = true;
									break;
								}
							}
						}
					}
					if (amCluster != null)
					{
						using (IEnumerator<IAmClusterNode> enumerator2 = amCluster.EnumerateNodes().GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								IAmClusterNode amClusterNode = enumerator2.Current;
								list.Add(amClusterNode.Name);
							}
							goto IL_352;
						}
					}
					foreach (ADObjectId serverId in databaseAvailabilityGroup.Servers)
					{
						list.Add(new AmServerName(serverId));
					}
					IL_352:;
				}
				finally
				{
					if (flag3 && amCluster != null)
					{
						amCluster.Dispose();
					}
				}
				this.m_accessRules.Clear();
				ITopologyConfigurationSession topologyConfigurationSession2 = this.CreateRootSession();
				foreach (AmServerName amServerName2 in list)
				{
					ADComputer adcomputer2 = topologyConfigurationSession2.FindComputerByHostName(amServerName2.NetbiosName);
					IdentityReference identity2 = adcomputer2.Sid.Translate(typeof(NTAccount));
					FileSystemAccessRule item3 = new FileSystemAccessRule(identity2, FileSystemRights.FullControl, AccessControlType.Allow);
					this.m_accessRules.Add(item3);
				}
			}
		}

		private ITopologyConfigurationSession CreateRootSession()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.m_adSession.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 493, "CreateRootSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\FileShareWitness.cs");
			topologyConfigurationSession.UseConfigNC = false;
			topologyConfigurationSession.UseGlobalCatalog = true;
			return topologyConfigurationSession;
		}

		private bool ServerHasMailboxRole(string serverName)
		{
			MiniServer miniServer = null;
			try
			{
				miniServer = this.m_adSession.FindMiniServerByFqdn(SharedHelper.GetFqdnNameFromNode(serverName), FileShareWitness.s_miniServerProperties);
			}
			catch (ADTransientException ex)
			{
				TaskLogger.Trace("Attempt to check mailbox role for server {0} failed. Specific error: {1}", new object[]
				{
					serverName,
					ex
				});
			}
			catch (ADExternalException ex2)
			{
				TaskLogger.Trace("Attempt to check mailbox role for server {0} failed. Specific error: {1}", new object[]
				{
					serverName,
					ex2
				});
			}
			catch (ADOperationException ex3)
			{
				TaskLogger.Trace("Attempt to check mailbox role for server {0} failed. Specific error: {1}", new object[]
				{
					serverName,
					ex3
				});
			}
			return miniServer != null && miniServer.IsMailboxServer;
		}

		public bool Initialize()
		{
			if (this.m_witnessServer == null)
			{
				List<string> frontendTransportServersInLocalSite = ReplayConfiguration.GetFrontendTransportServersInLocalSite();
				if (frontendTransportServersInLocalSite != null && frontendTransportServersInLocalSite.Count > 0)
				{
					foreach (string text in frontendTransportServersInLocalSite)
					{
						if (!this.ServerHasMailboxRole(text))
						{
							if (!FileShareWitnessServerName.TryParse(text, out this.m_witnessServer))
							{
								TaskLogger.Trace("Attempt to parse FrontendTransport server name {0} to its file share witness name failed.", new object[]
								{
									text
								});
								break;
							}
							break;
						}
					}
				}
				if (this.m_witnessServer == null)
				{
					this.m_initializationException = new DagFswUnableToDetermineFrontendTransportServerException();
					throw this.m_initializationException;
				}
			}
			try
			{
				this.AssignWitnessServerFqdn();
			}
			catch (ArgumentException ex)
			{
				this.m_initializationException = new DagFswUnableToParseWitnessServerNameException(ex);
				throw this.m_initializationException;
			}
			catch (ErrorUnableToFindFqdnForHostADErrorException ex2)
			{
				this.m_initializationException = new DagFswUnableToParseWitnessServerNameException(ex2);
				throw this.m_initializationException;
			}
			string text2 = this.WitnessServer.DomainName;
			if (string.IsNullOrEmpty(text2))
			{
				int num = this.WitnessServerFqdn.IndexOf('.');
				if (num > -1)
				{
					text2 = this.WitnessServerFqdn.Substring(num + 1);
				}
			}
			string arg = string.Format(FileShareWitness.s_dagShareNameFormat, this.m_dagName, text2);
			UncFileSharePath.TryParse(string.Format(FileShareWitness.s_uncPathFormat, this.WitnessServerFqdn, arg), out this.m_fileShareWitnessShare);
			if (this.m_witnessDirectory == null)
			{
				string text3 = WmiWrapper.TryGetSystemDrive(this.WitnessServerFqdn);
				if (text3 == null)
				{
					this.m_initializationException = new DagFswUnableToBindWitnessDirectoryException(this.WitnessServerFqdn);
					throw this.m_initializationException;
				}
				this.m_witnessDirectory = NonRootLocalLongFullPath.Parse(string.Format(FileShareWitness.s_defaultDirectoryFormat, text3, arg));
			}
			this.m_initialized = true;
			return this.m_initialized;
		}

		public bool Exists()
		{
			if (!this.m_initialized)
			{
				string ex = Strings.DagFswInternalErrorFswObjectNotInitialized.ToString();
				if (this.m_initializationException != null)
				{
					ex = this.m_initializationException.Message;
				}
				throw new DagFswNotInitializedException(ex);
			}
			if (this.m_exists == null)
			{
				try
				{
					string text;
					byte[] securityDescriptorBinaryForm;
					NetShare.GetShareInfoWithSecurity(this.WitnessServerFqdn, this.ShareName, out text, out this.m_existingShareRemark, out this.m_shareType, out this.m_sharePermissions, out this.m_shareMaxUses, out securityDescriptorBinaryForm);
					this.m_existingWitnessDirectory = NonRootLocalLongFullPath.Parse(text);
					TaskLogger.Trace("Found existing witness share \\\\{0}\\{1} pointing to {2} (expecting {3})", new object[]
					{
						this.WitnessServer,
						this.ShareName,
						text,
						this.m_witnessDirectory.ToString()
					});
					if (this.m_existingWitnessDirectory != this.WitnessDirectory)
					{
						throw new DagFswSharePointsToWrongDirectoryException(this.ShareName, this.WitnessServerFqdn, this.m_existingWitnessDirectory.ToString(), this.WitnessDirectory.ToString());
					}
					this.m_existingSecurity = new DirectorySecurity();
					this.m_existingSecurity.SetSecurityDescriptorBinaryForm(securityDescriptorBinaryForm);
					this.m_exists = new bool?(true);
				}
				catch (Win32Exception ex2)
				{
					if ((long)ex2.NativeErrorCode == 2310L)
					{
						this.m_exists = new bool?(false);
					}
					else
					{
						if (ex2.NativeErrorCode == 5)
						{
							throw new DagFswInsufficientPermissionsToAccessFswException(this.WitnessServerFqdn, ex2);
						}
						throw new DagFswServerNotAccessibleException(this.WitnessServerFqdn, ex2);
					}
				}
			}
			return this.m_exists.Value;
		}

		public FileShareWitnessCheckResult Check()
		{
			this.RecalculateAccessRules();
			if (!this.Exists())
			{
				return FileShareWitnessCheckResult.FswDoesNotExist;
			}
			if (this.m_existingWitnessDirectory != this.WitnessDirectory)
			{
				return FileShareWitnessCheckResult.FswWrongDirectory;
			}
			AuthorizationRuleCollection accessRules = this.m_existingSecurity.GetAccessRules(true, true, typeof(NTAccount));
			if (accessRules == null)
			{
				return FileShareWitnessCheckResult.FswWrongPermissions;
			}
			List<FileSystemAccessRule> list = new List<FileSystemAccessRule>(this.AccessRules);
			foreach (object obj in accessRules)
			{
				AuthorizationRule authorizationRule = (AuthorizationRule)obj;
				FileSystemAccessRule fsRule = (FileSystemAccessRule)authorizationRule;
				FileSystemAccessRule fileSystemAccessRule = list.Find((FileSystemAccessRule ar) => fsRule.IdentityReference == ar.IdentityReference && fsRule.FileSystemRights == ar.FileSystemRights);
				if (fileSystemAccessRule != null)
				{
					list.Remove(fileSystemAccessRule);
				}
			}
			if (list.Count == 0)
			{
				return FileShareWitnessCheckResult.FswOK;
			}
			return FileShareWitnessCheckResult.FswWrongPermissions;
		}

		public void Create()
		{
			FileShareWitnessCheckResult fileShareWitnessCheckResult = this.Check();
			if (fileShareWitnessCheckResult != FileShareWitnessCheckResult.FswOK)
			{
				FileSystemSecurity fileSystemSecurity = new DirectorySecurity();
				foreach (FileSystemAccessRule rule in this.AccessRules)
				{
					fileSystemSecurity.AddAccessRule(rule);
				}
				switch (fileShareWitnessCheckResult)
				{
				case FileShareWitnessCheckResult.FswDoesNotExist:
					try
					{
						TaskLogger.Trace("Attempting to create new witness share \\\\{0}\\{1}.", new object[]
						{
							this.WitnessServer,
							this.ShareName
						});
						WmiWrapper.CreateDirectory(this.WitnessServerFqdn, this.WitnessDirectory.ToString());
						int num;
						NetShare.AddShare(string.Format(FileShareWitness.s_hostNameFormat, this.WitnessServerFqdn), null, this.ShareName, this.WitnessDirectory.ToString(), this.m_shareRemark, fileSystemSecurity.GetSecurityDescriptorBinaryForm(), out num);
						this.m_existingShareRemark = this.m_shareRemark;
						this.m_shareType = 0U;
						this.m_sharePermissions = 1;
						this.m_shareMaxUses = -1;
						this.m_existingWitnessDirectory = this.WitnessDirectory;
						this.m_existingSecurity = fileSystemSecurity;
						this.m_exists = new bool?(true);
						this.m_isJustCreated = true;
						TaskLogger.Trace("Attempt to create new witness share \\\\{0}\\{1} succeeded.", new object[]
						{
							this.WitnessServer,
							this.ShareName
						});
						return;
					}
					catch (WmiException ex)
					{
						throw new DagFswUnableToCreateWitnessDirectoryException(this.WitnessServerFqdn, this.WitnessDirectory.ToString(), ex);
					}
					catch (Win32Exception ex2)
					{
						throw new DagFswUnableToCreateWitnessShareException(this.WitnessServerFqdn, this.FileShareWitnessShare.ToString(), ex2);
					}
					break;
				case FileShareWitnessCheckResult.FswWrongDirectory:
					goto IL_1C1;
				case FileShareWitnessCheckResult.FswWrongPermissions:
					break;
				default:
					return;
				}
				fileSystemSecurity = this.m_existingSecurity;
				foreach (FileSystemAccessRule rule2 in this.AccessRules)
				{
					fileSystemSecurity.AddAccessRule(rule2);
				}
				try
				{
					IL_1C1:
					TaskLogger.Trace("Attempting to modify permissions on witness share \\\\{0}\\{1}.", new object[]
					{
						this.WitnessServer,
						this.ShareName
					});
					NetShare.SetShareInfo(this.WitnessServerFqdn, this.ShareName, this.WitnessDirectory.ToString(), this.m_shareRemark, this.m_shareType, this.m_sharePermissions, -1, fileSystemSecurity.GetSecurityDescriptorBinaryForm());
					this.m_existingWitnessDirectory = this.WitnessDirectory;
					this.m_existingSecurity = fileSystemSecurity;
					this.m_shareMaxUses = -1;
					this.m_exists = new bool?(true);
					TaskLogger.Trace("Attempt to modify permissions on witness share \\\\{0}\\{1} succeeded.", new object[]
					{
						this.WitnessServer,
						this.ShareName
					});
				}
				catch (Win32Exception ex3)
				{
					throw new DagFswUnableToUpdateWitnessShareException(this.WitnessServerFqdn, this.FileShareWitnessShare.ToString(), ex3);
				}
			}
		}

		public void Delete()
		{
			this.Delete(false);
		}

		public void Delete(bool forceDelete)
		{
			if (this.Exists())
			{
				if (!this.m_isJustCreated)
				{
					if (!forceDelete)
					{
						return;
					}
				}
				try
				{
					TaskLogger.Trace("Attempting to delete witness share \\\\{0}\\{1}. Just created: {2}, forced: {3}", new object[]
					{
						this.WitnessServer,
						this.ShareName,
						this.m_isJustCreated,
						forceDelete
					});
					NetShare.DeleteShare(this.WitnessServerFqdn, this.ShareName);
					WmiWrapper.RemoveDirectory(this.WitnessServerFqdn, this.WitnessDirectory.ToString());
					this.m_exists = new bool?(false);
					TaskLogger.Trace("Attempt to delete witness share \\\\{0}\\{1} succeeded.", new object[]
					{
						this.WitnessServer,
						this.ShareName
					});
				}
				catch (WmiException ex)
				{
					throw new DagFswUnableToRemoveWitnessDirectoryException(this.WitnessServerFqdn, this.WitnessDirectory.ToString(), ex);
				}
				catch (Win32Exception ex2)
				{
					if ((long)ex2.NativeErrorCode == 2310L)
					{
						this.m_exists = new bool?(false);
					}
					else
					{
						if (ex2.NativeErrorCode == 5)
						{
							throw new DagFswInsufficientPermissionsToDeleteFswException(this.WitnessServerFqdn, this.FileShareWitnessShare.ToString(), ex2);
						}
						throw new DagFswServerNotAccessibleToDeleteFswException(this.WitnessServerFqdn, this.FileShareWitnessShare.ToString(), ex2);
					}
				}
			}
		}

		public FileShareWitnessExceptionType GetExceptionType(LocalizedException ex)
		{
			FileShareWitnessExceptionType result = FileShareWitnessExceptionType.FswUnknownError;
			if (ex is DagFswUnableToDetermineFrontendTransportServerException || ex is DagFswUnableToBindWitnessDirectoryException || ex is DagFswNotInitializedException || ex is DagFswInsufficientPermissionsToAccessFswException || ex is DagFswServerNotAccessibleException || ex is DagFswSharePointsToWrongDirectoryException || ex is DagFswUnableToParseWitnessServerNameException)
			{
				result = FileShareWitnessExceptionType.FswInitializationError;
			}
			else if (ex is DagFswUnableToCreateWitnessDirectoryException || ex is DagFswUnableToCreateWitnessShareException || ex is DagFswUnableToUpdateWitnessShareException)
			{
				result = FileShareWitnessExceptionType.FswCreateError;
			}
			else if (ex is DagFswInsufficientPermissionsToDeleteFswException || ex is DagFswServerNotAccessibleToDeleteFswException || ex is DagFswUnableToRemoveWitnessDirectoryException)
			{
				result = FileShareWitnessExceptionType.FswDeleteError;
			}
			return result;
		}

		public bool Equals(FileShareWitness fsw)
		{
			return fsw != null && (fsw.WitnessServer != this.WitnessServer && fsw.WitnessDirectory != this.WitnessDirectory);
		}

		public string ShareName
		{
			get
			{
				return this.m_fileShareWitnessShare.ShareName;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this.m_initialized;
			}
		}

		public bool IsJustCreated
		{
			get
			{
				return this.m_isJustCreated;
			}
		}

		public FileShareWitnessServerName WitnessServer
		{
			get
			{
				return this.m_witnessServer;
			}
		}

		public string WitnessServerFqdn
		{
			get
			{
				return this.m_witnessServerFqdn;
			}
		}

		public NonRootLocalLongFullPath WitnessDirectory
		{
			get
			{
				return this.m_witnessDirectory;
			}
		}

		public UncFileSharePath FileShareWitnessShare
		{
			get
			{
				return this.m_fileShareWitnessShare;
			}
		}

		private bool m_initialized;

		private Exception m_initializationException;

		private ITopologyConfigurationSession m_adSession;

		private readonly string m_dagName;

		private FileShareWitnessServerName m_witnessServer;

		private string m_witnessServerFqdn;

		private NonRootLocalLongFullPath m_witnessDirectory;

		private UncFileSharePath m_fileShareWitnessShare;

		private List<FileSystemAccessRule> m_accessRules;

		private readonly string m_shareRemark;

		private bool? m_exists = null;

		private bool m_isJustCreated;

		private NonRootLocalLongFullPath m_existingWitnessDirectory;

		private string m_existingShareRemark;

		private uint m_shareType;

		private int m_sharePermissions;

		private int m_shareMaxUses;

		private FileSystemSecurity m_existingSecurity;

		private WeakReference<IAmCluster> m_clusterReference;

		private static readonly string s_dagShareNameFormat = "{0}.{1}";

		private static readonly string s_uncPathFormat = "\\\\{0}\\{1}";

		private static readonly string s_hostNameFormat = "\\\\{0}";

		private static readonly string s_defaultDirectoryFormat = "{0}\\DAGFileShareWitnesses\\{1}";

		private static readonly string s_shareRemarkFormat = "File share witness created for microsoft exchange database availability group {0}.";

		private static readonly PropertyDefinition[] s_miniServerProperties = new PropertyDefinition[]
		{
			ServerSchema.IsMailboxServer
		};
	}
}
