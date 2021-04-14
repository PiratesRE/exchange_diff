using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Set", "LocalPermissions")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class SetLocalPermissions : Task
	{
		public SetLocalPermissions()
		{
			using (Stream permissionsFileStream = SetLocalPermissions.GetPermissionsFileStream())
			{
				this.permissionsXmlDocument = new SafeXmlDocument();
				this.permissionsXmlDocument.Load(permissionsFileStream);
			}
			this.fileSystemRightsDictionary.Add("genericall", FileSystemRights.FullControl);
			this.fileSystemRightsDictionary.Add("genericread", FileSystemRights.ReadExtendedAttributes | FileSystemRights.ReadAttributes | FileSystemRights.ReadPermissions);
			this.fileSystemRightsDictionary.Add("read", FileSystemRights.ReadData);
			this.fileSystemRightsDictionary.Add("traverse", FileSystemRights.ExecuteFile);
			this.fileSystemRightsDictionary.Add("genericwrite", FileSystemRights.Write);
			this.fileSystemRightsDictionary.Add("readextendedattributes", FileSystemRights.ReadExtendedAttributes);
			this.fileSystemRightsDictionary.Add("readpermission", FileSystemRights.ReadPermissions);
			this.fileSystemRightsDictionary.Add("readattributes", FileSystemRights.ReadAttributes);
			this.fileSystemRightsDictionary.Add("deletechild", FileSystemRights.DeleteSubdirectoriesAndFiles);
			this.registryRightsDictionary.Add("genericall", RegistryRights.FullControl);
			this.registryRightsDictionary.Add("read", RegistryRights.ExecuteKey);
			this.wellKnowSecurityIdentitiesDictionary.Add("Administrators", this.GetSecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid));
			this.wellKnowSecurityIdentitiesDictionary.Add("NetworkService", this.GetSecurityIdentifier(WellKnownSidType.NetworkServiceSid));
			this.wellKnowSecurityIdentitiesDictionary.Add("LocalService", this.GetSecurityIdentifier(WellKnownSidType.LocalServiceSid));
			this.wellKnowSecurityIdentitiesDictionary.Add("AuthenticatedUser", this.GetSecurityIdentifier(WellKnownSidType.AuthenticatedUserSid));
			this.wellKnowSecurityIdentitiesDictionary.Add("System", this.GetSecurityIdentifier(WellKnownSidType.LocalSystemSid));
			this.rootRegistryKeysDictionary.Add("HKEY_CLASSES_ROOT", Registry.ClassesRoot);
			this.rootRegistryKeysDictionary.Add("HKEY_CURRENT_CONFIG", Registry.CurrentConfig);
			this.rootRegistryKeysDictionary.Add("HKEY_CURRENT_USER", Registry.CurrentUser);
			this.rootRegistryKeysDictionary.Add("HKEY_LOCAL_MACHINE", Registry.LocalMachine);
			this.rootRegistryKeysDictionary.Add("HKEY_USERS", Registry.Users);
			this.rootRegistryKeysDictionary.Add("HKEY_PERFORMANCE_DATA", Registry.PerformanceData);
			this.environmentVariablesDictionary.Add("Version", ConfigurationContext.Setup.InstalledVersion.ToString());
			this.installedPath = ConfigurationContext.Setup.InstallPath;
		}

		private SecurityIdentifier GetSecurityIdentifier(WellKnownSidType wellKnownSidType)
		{
			return new SecurityIdentifier(wellKnownSidType, null);
		}

		private static Stream GetPermissionsFileStream()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			return executingAssembly.GetManifestResourceStream("LocalPermissions.xml");
		}

		[Parameter(Mandatory = false)]
		public string Feature
		{
			get
			{
				return (string)base.Fields["Feature"];
			}
			set
			{
				base.Fields["Feature"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			try
			{
				if (string.IsNullOrEmpty(this.Feature))
				{
					this.SetCommonPermissions();
				}
				else
				{
					this.SetFeaturePermissions(this.Feature);
				}
			}
			catch (ArgumentException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (DirectoryNotFoundException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
			}
			catch (UnauthorizedAccessException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidOperation, null);
			}
			catch (IOException exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidOperation, null);
			}
			catch (SystemException exception5)
			{
				base.WriteError(exception5, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		private string GetTargetFolder(XmlNode targetNode)
		{
			string text = targetNode.Attributes["Path"].Value;
			text = this.ReplaceEnvironmentVariables(text);
			text = Path.Combine(this.installedPath, text ?? string.Empty);
			if (!Directory.Exists(text))
			{
				throw new ArgumentException(Strings.DirectoryDoesNotExist(text), null);
			}
			return text;
		}

		private RegistryKey GetTargetRegistryKey(XmlNode targetNode)
		{
			RegistryKey registryKey = this.rootRegistryKeysDictionary[targetNode.Attributes["Root"].Value];
			string text = targetNode.Attributes["Key"].Value;
			text = this.ReplaceEnvironmentVariables(text);
			RegistryKey registryKey2 = registryKey.OpenSubKey(text, true);
			if (registryKey2 == null)
			{
				throw new ArgumentException(Strings.RegistryKeyDoesNotExist(text, registryKey.Name), null);
			}
			return registryKey2;
		}

		private string ReplaceEnvironmentVariables(string str)
		{
			StringBuilder stringBuilder = new StringBuilder(str);
			foreach (string text in this.environmentVariablesDictionary.Keys)
			{
				stringBuilder.Replace("$" + text, this.environmentVariablesDictionary[text]);
			}
			return stringBuilder.ToString();
		}

		private DirectorySecurity GetOrginalDirectorySecurity(string path)
		{
			if (!Directory.Exists(path))
			{
				throw new ArgumentException(Strings.DirectoryDoesNotExist(path), null);
			}
			return Directory.GetAccessControl(path);
		}

		private RegistrySecurity GetOrginalRegistrySecurity(RegistryKey key)
		{
			return key.GetAccessControl();
		}

		private void SetDirectorySecurity(string path, DirectorySecurity directorySecurity)
		{
			if (!Directory.Exists(path))
			{
				throw new ArgumentException(Strings.DirectoryDoesNotExist(path), null);
			}
			Directory.SetAccessControl(path, directorySecurity);
		}

		private void SetRegistrySecurity(RegistryKey key, RegistrySecurity registrySecurity)
		{
			key.SetAccessControl(registrySecurity);
		}

		private RegistryAccessRule CreateRegistryAccessRule(IdentityReference identity, RegistryRights rights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType accessControlType)
		{
			return new RegistryAccessRule(identity, rights, inheritanceFlags, propagationFlags, accessControlType);
		}

		private FileSystemAccessRule CreateFileSystemAccessRule(IdentityReference identity, FileSystemRights rights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType accessControlType)
		{
			return new FileSystemAccessRule(identity, rights, inheritanceFlags, propagationFlags, accessControlType);
		}

		private void AddFileSystemAccessRule(DirectorySecurity permissions, FileSystemAccessRule accessRule)
		{
			permissions.AddAccessRule(accessRule);
		}

		private void AddRegistryAccessRule(RegistrySecurity permissions, RegistryAccessRule accessRule)
		{
			permissions.AddAccessRule(accessRule);
		}

		private void RemoveFileSystemAccessRuleAll(DirectorySecurity permissions, SecurityIdentifier securityIdentifier)
		{
			permissions.RemoveAccessRuleAll(new FileSystemAccessRule(securityIdentifier, FileSystemRights.FullControl, AccessControlType.Allow));
		}

		private void RemoveRegistryAccessRuleAll(RegistrySecurity permissions, SecurityIdentifier securityIdentifier)
		{
			permissions.RemoveAccessRuleAll(new RegistryAccessRule(securityIdentifier, RegistryRights.FullControl, AccessControlType.Allow));
		}

		private void SetCommonPermissions()
		{
			XmlNode permissionsOnCurrentLevel = this.permissionsXmlDocument.SelectSingleNode(string.Format("{0}/{1}", "Permissions", "CommonPermissionSet"));
			this.SetPermissionsOnCurrentLevel(permissionsOnCurrentLevel);
		}

		private void SetFeaturePermissions(string feature)
		{
			TaskLogger.LogEnter();
			XmlNode xmlNode = this.permissionsXmlDocument.SelectSingleNode(string.Format("{0}/{1}[@{2}='{3}']", new object[]
			{
				"Permissions",
				"FeaturePermissionSet",
				"Name",
				feature
			}));
			if (xmlNode != null)
			{
				this.SetPermissionsOnCurrentLevel(xmlNode);
				using (XmlNodeList xmlNodeList = xmlNode.SelectNodes("SharedPermissionSet"))
				{
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode2 = (XmlNode)obj;
						string value = xmlNode2.Attributes["Name"].Value;
						XmlNode permissionsOnCurrentLevel = this.permissionsXmlDocument.SelectSingleNode(string.Format("{0}/{1}[@{2}='{3}']", new object[]
						{
							"Permissions",
							"SharedPermissionSet",
							"Name",
							value
						}));
						this.SetPermissionsOnCurrentLevel(permissionsOnCurrentLevel);
					}
				}
			}
			TaskLogger.LogExit();
		}

		private void SetPermissionsOnCurrentLevel(XmlNode permissionSetNode)
		{
			this.SetPermissionsOnCurrentLevel<string, DirectorySecurity, FileSystemAccessRule, FileSystemRights>(permissionSetNode, "Folder", this.fileSystemRightsDictionary, new SetLocalPermissions.GetTarget<string>(this.GetTargetFolder), new SetLocalPermissions.GetOrginalPermissionsOnTarget<DirectorySecurity, string>(this.GetOrginalDirectorySecurity), new SetLocalPermissions.SetPermissionsOnTarget<DirectorySecurity, string>(this.SetDirectorySecurity), new SetLocalPermissions.CreateAccessRule<FileSystemAccessRule, FileSystemRights>(this.CreateFileSystemAccessRule), new SetLocalPermissions.AddAccessRule<DirectorySecurity, FileSystemAccessRule>(this.AddFileSystemAccessRule), new SetLocalPermissions.RemoveAccessRuleAll<DirectorySecurity>(this.RemoveFileSystemAccessRuleAll));
			this.SetPermissionsOnCurrentLevel<RegistryKey, RegistrySecurity, RegistryAccessRule, RegistryRights>(permissionSetNode, "Registry", this.registryRightsDictionary, new SetLocalPermissions.GetTarget<RegistryKey>(this.GetTargetRegistryKey), new SetLocalPermissions.GetOrginalPermissionsOnTarget<RegistrySecurity, RegistryKey>(this.GetOrginalRegistrySecurity), new SetLocalPermissions.SetPermissionsOnTarget<RegistrySecurity, RegistryKey>(this.SetRegistrySecurity), new SetLocalPermissions.CreateAccessRule<RegistryAccessRule, RegistryRights>(this.CreateRegistryAccessRule), new SetLocalPermissions.AddAccessRule<RegistrySecurity, RegistryAccessRule>(this.AddRegistryAccessRule), new SetLocalPermissions.RemoveAccessRuleAll<RegistrySecurity>(this.RemoveRegistryAccessRuleAll));
		}

		private void SetPermissionsOnCurrentLevel<TTarget, TSecurity, TAccessRule, TRights>(XmlNode permissionSetNode, string targetType, Dictionary<string, TRights> rightsDictionary, SetLocalPermissions.GetTarget<TTarget> getTarget, SetLocalPermissions.GetOrginalPermissionsOnTarget<TSecurity, TTarget> getOrginalPermissionsOnTarget, SetLocalPermissions.SetPermissionsOnTarget<TSecurity, TTarget> setPermissionsOnTarget, SetLocalPermissions.CreateAccessRule<TAccessRule, TRights> createAccessRule, SetLocalPermissions.AddAccessRule<TSecurity, TAccessRule> addAccessRule, SetLocalPermissions.RemoveAccessRuleAll<TSecurity> removeAccessRuleAll) where TTarget : class where TSecurity : NativeObjectSecurity, new() where TAccessRule : AccessRule
		{
			TaskLogger.LogEnter();
			using (XmlNodeList xmlNodeList = permissionSetNode.SelectNodes(targetType))
			{
				foreach (object obj in xmlNodeList)
				{
					XmlNode targetNode = (XmlNode)obj;
					this.ChangePermissions<TTarget, TSecurity, TAccessRule, TRights>(targetNode, rightsDictionary, getTarget, getOrginalPermissionsOnTarget, setPermissionsOnTarget, createAccessRule, addAccessRule, removeAccessRuleAll);
				}
			}
			TaskLogger.LogExit();
		}

		private void ChangePermissions<TTarget, TSecurity, TAccessRule, TRights>(XmlNode targetNode, Dictionary<string, TRights> rightsDictionary, SetLocalPermissions.GetTarget<TTarget> getTarget, SetLocalPermissions.GetOrginalPermissionsOnTarget<TSecurity, TTarget> getOrginalPermissionsOnTarget, SetLocalPermissions.SetPermissionsOnTarget<TSecurity, TTarget> setPermissionsOnTarget, SetLocalPermissions.CreateAccessRule<TAccessRule, TRights> createAccessRule, SetLocalPermissions.AddAccessRule<TSecurity, TAccessRule> addAccessRule, SetLocalPermissions.RemoveAccessRuleAll<TSecurity> removeAccessRuleAll) where TTarget : class where TSecurity : NativeObjectSecurity, new() where TAccessRule : AccessRule
		{
			TaskLogger.LogEnter();
			TTarget target = getTarget(targetNode);
			TSecurity tsecurity = default(TSecurity);
			if (targetNode.Attributes["Sddl"] != null)
			{
				string value = targetNode.Attributes["Sddl"].Value;
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException(null, "Sddl");
				}
				if (targetNode.Attributes.Count > 2)
				{
					foreach (object obj in targetNode.Attributes)
					{
						XmlNode xmlNode = (XmlNode)obj;
						if (xmlNode.Name != "Sddl" && xmlNode.Name != "Path")
						{
							throw new ArgumentException(null, xmlNode.Name);
						}
					}
				}
				if (targetNode.ChildNodes.Count > 0)
				{
					throw new ArgumentException(null, targetNode.ChildNodes[0].Name);
				}
				tsecurity = Activator.CreateInstance<TSecurity>();
				tsecurity.SetSecurityDescriptorSddlForm(value);
			}
			else
			{
				tsecurity = getOrginalPermissionsOnTarget(target);
				if (tsecurity.AreAccessRulesCanonical)
				{
					tsecurity.SetAccessRuleProtection(this.IsProtected(targetNode), this.PreserveInheritance(targetNode));
				}
				else
				{
					tsecurity = Activator.CreateInstance<TSecurity>();
				}
				using (XmlNodeList xmlNodeList = targetNode.SelectNodes("Permission"))
				{
					foreach (object obj2 in xmlNodeList)
					{
						XmlNode permissionNode = (XmlNode)obj2;
						SecurityIdentifier securityIdentifier = this.GetSecurityIdentifier(permissionNode);
						InheritanceFlags inheritanceFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
						PropagationFlags propagationFlags = PropagationFlags.None;
						if (this.IsProtected(targetNode) && this.PreserveInheritance(targetNode) && !this.IsExtended(permissionNode))
						{
							removeAccessRuleAll(tsecurity, securityIdentifier);
						}
						List<SetLocalPermissions.RightsWithAccessControlType<TRights>> rights = this.GetRights<TRights>(permissionNode, rightsDictionary);
						foreach (SetLocalPermissions.RightsWithAccessControlType<TRights> rightsWithAccessControlType in rights)
						{
							addAccessRule(tsecurity, createAccessRule(securityIdentifier, rightsWithAccessControlType.Rights, inheritanceFlags, propagationFlags, rightsWithAccessControlType.AccessControlType));
						}
					}
				}
			}
			setPermissionsOnTarget(target, tsecurity);
			TaskLogger.LogExit();
		}

		private bool IsProtected(XmlNode targetNode)
		{
			return targetNode.Attributes["Protected"] != null && string.Compare(targetNode.Attributes["Protected"].Value, "yes", true) == 0;
		}

		private bool PreserveInheritance(XmlNode targetNode)
		{
			return targetNode.Attributes["PreserveInheritance"] == null || string.Compare(targetNode.Attributes["PreserveInheritance"].Value, "yes", true) == 0;
		}

		private SecurityIdentifier GetSecurityIdentifier(XmlNode permissionNode)
		{
			string value = permissionNode.Attributes["User"].Value;
			return this.wellKnowSecurityIdentitiesDictionary[value];
		}

		private bool IsExtended(XmlNode permissionNode)
		{
			return permissionNode.Attributes["Extended"] != null && string.Compare(permissionNode.Attributes["Extended"].Value, "yes", true) == 0;
		}

		private List<SetLocalPermissions.RightsWithAccessControlType<TRights>> GetRights<TRights>(XmlNode permissionNode, Dictionary<string, TRights> rightsDictionary)
		{
			TaskLogger.LogEnter();
			List<SetLocalPermissions.RightsWithAccessControlType<TRights>> list = new List<SetLocalPermissions.RightsWithAccessControlType<TRights>>();
			foreach (object obj in permissionNode.Attributes)
			{
				XmlAttribute permissionAttribute = (XmlAttribute)obj;
				SetLocalPermissions.RightsWithAccessControlType<TRights> rights = this.GetRights<TRights>(permissionAttribute, rightsDictionary);
				if (rights != null)
				{
					list.Add(rights);
				}
			}
			TaskLogger.LogExit();
			return list;
		}

		private SetLocalPermissions.RightsWithAccessControlType<TRights> GetRights<TRights>(XmlAttribute permissionAttribute, Dictionary<string, TRights> rightsDictionary)
		{
			TaskLogger.LogEnter();
			SetLocalPermissions.RightsWithAccessControlType<TRights> rightsWithAccessControlType = null;
			if (permissionAttribute.Name != "User" && permissionAttribute.Name != "Extended")
			{
				rightsWithAccessControlType = new SetLocalPermissions.RightsWithAccessControlType<TRights>();
				rightsWithAccessControlType.Rights = rightsDictionary[permissionAttribute.Name.ToLower()];
				if (string.Compare(permissionAttribute.Value, "yes", true) == 0)
				{
					rightsWithAccessControlType.AccessControlType = AccessControlType.Allow;
				}
				else
				{
					rightsWithAccessControlType.AccessControlType = AccessControlType.Deny;
				}
			}
			TaskLogger.LogExit();
			return rightsWithAccessControlType;
		}

		private const string PermissionsFileName = "LocalPermissions.xml";

		private const string RootNode = "Permissions";

		private const string FeaturePermissionSetNode = "FeaturePermissionSet";

		private const string CommonPermissionSetNode = "CommonPermissionSet";

		private const string SharedPermissionSetNode = "SharedPermissionSet";

		private const string PermissionSetNodeNameAttribute = "Name";

		private const string FolderNode = "Folder";

		private const string FolderNodePathAttribute = "Path";

		private const string PermissionNode = "Permission";

		private const string PermissionNodeUserAttribute = "User";

		private const string PermissionNodeExtendedAttribute = "Extended";

		private const string RegistryNode = "Registry";

		private const string RegistryNodeRootAttribute = "Root";

		private const string RegistryNodeKeyAttribute = "Key";

		private const string TargetNodeProtectedAttribute = "Protected";

		private const string TargetNodePreserveInheritanceAttribute = "PreserveInheritance";

		private const string SddlAttribute = "Sddl";

		private const string ClassRootBaseKey = "HKEY_CLASSES_ROOT";

		private const string CurrentConfigBaseKey = "HKEY_CURRENT_CONFIG";

		private const string CurrentUserBaseKey = "HKEY_CURRENT_USER";

		private const string LocalMachineBaseKey = "HKEY_LOCAL_MACHINE";

		private const string UsersBaseKey = "HKEY_USERS";

		private const string DynDataBaseKey = "HKEY_DYN_DATA";

		private const string PerformanceDataBaseKey = "HKEY_PERFORMANCE_DATA";

		private const string AdministratorsName = "Administrators";

		private const string NetworkServiceName = "NetworkService";

		private const string LocalServiceName = "LocalService";

		private const string AuthenticatedUserName = "AuthenticatedUser";

		private const string SystemName = "System";

		private const string PrefixOfEnvironmentVariable = "$";

		private const string Version = "Version";

		private SafeXmlDocument permissionsXmlDocument;

		private Dictionary<string, FileSystemRights> fileSystemRightsDictionary = new Dictionary<string, FileSystemRights>();

		private Dictionary<string, RegistryRights> registryRightsDictionary = new Dictionary<string, RegistryRights>();

		private Dictionary<string, SecurityIdentifier> wellKnowSecurityIdentitiesDictionary = new Dictionary<string, SecurityIdentifier>();

		private Dictionary<string, RegistryKey> rootRegistryKeysDictionary = new Dictionary<string, RegistryKey>();

		private Dictionary<string, string> environmentVariablesDictionary = new Dictionary<string, string>();

		private readonly string installedPath;

		private delegate TTarget GetTarget<TTarget>(XmlNode targetNode) where TTarget : class;

		private delegate TSecurity GetOrginalPermissionsOnTarget<TSecurity, TTarget>(TTarget target) where TSecurity : NativeObjectSecurity where TTarget : class;

		private delegate void SetPermissionsOnTarget<TSecurity, TTarget>(TTarget target, TSecurity security) where TSecurity : NativeObjectSecurity where TTarget : class;

		private delegate TAccessRule CreateAccessRule<TAccessRule, TRights>(IdentityReference identity, TRights rights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType accessControlType) where TAccessRule : AccessRule;

		private delegate void AddAccessRule<TSecurity, TAccessRule>(TSecurity permissions, TAccessRule accessRule) where TSecurity : NativeObjectSecurity where TAccessRule : AccessRule;

		private delegate void RemoveAccessRuleAll<TSecurity>(TSecurity permissions, SecurityIdentifier securityIdentifier) where TSecurity : NativeObjectSecurity;

		private class RightsWithAccessControlType<TRights>
		{
			public TRights Rights;

			public AccessControlType AccessControlType;
		}
	}
}
