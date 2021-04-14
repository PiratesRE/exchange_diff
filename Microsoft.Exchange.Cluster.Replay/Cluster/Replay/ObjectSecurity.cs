using System;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class ObjectSecurity
	{
		public static DirectorySecurity ExchangeFolderSecurity
		{
			get
			{
				string text = ObjectSecurity.ExchangeServersUsgSid.ToString();
				string sddlForm = string.Format(CultureInfo.InvariantCulture, "O:BAG:SYD:P(A;OICI;FA;;;SY)(A;OICI;FA;;;BA)(A;OICI;GR;;;{0})", new object[]
				{
					text
				});
				DirectorySecurity directorySecurity = new DirectorySecurity();
				directorySecurity.SetSecurityDescriptorSddlForm(sddlForm, AccessControlSections.Access);
				return directorySecurity;
			}
		}

		public static void AddRulesToSharedDirectory(string directory)
		{
			DirectorySecurity accessControl = Directory.GetAccessControl(directory);
			foreach (FileSystemAccessRule rule in ObjectSecurity.SharedDirectoryRules)
			{
				accessControl.AddAccessRule(rule);
			}
			Directory.SetAccessControl(directory, accessControl);
		}

		public static FileSystemSecurity NetShareSecurity
		{
			get
			{
				string text = ObjectSecurity.ExchangeServersUsgSid.ToString();
				string sddlForm = string.Format(CultureInfo.InvariantCulture, "D:P(A;;{1};;;{0})", new object[]
				{
					text,
					"0x120089"
				});
				FileSystemSecurity fileSystemSecurity = new DirectorySecurity();
				fileSystemSecurity.SetSecurityDescriptorSddlForm(sddlForm, AccessControlSections.Access);
				return fileSystemSecurity;
			}
		}

		public static byte[] NetShareSecurityBinaryForm
		{
			get
			{
				return ObjectSecurity.NetShareSecurity.GetSecurityDescriptorBinaryForm();
			}
		}

		public static FileSecurity TemporaryFileSecurity
		{
			get
			{
				string text = null;
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					text = current.User.Value;
				}
				string sddlForm = string.Format(CultureInfo.InvariantCulture, "D:P(A;;FRFWSD;;;{0})(A;;SD;;;BA)", new object[]
				{
					text
				});
				FileSecurity fileSecurity = new FileSecurity();
				fileSecurity.SetSecurityDescriptorSddlForm(sddlForm, AccessControlSections.Access);
				return fileSecurity;
			}
		}

		private static FileSecurity CreateBaseRpcSecurityObject()
		{
			FileSecurity fileSecurity = new FileSecurity();
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
			FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.SetOwner(securityIdentifier);
			fileSecurity.SetAccessRule(fileSystemAccessRule);
			SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
			fileSystemAccessRule = new FileSystemAccessRule(identity, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.AddAccessRule(fileSystemAccessRule);
			SecurityIdentifier identity2 = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
			fileSystemAccessRule = new FileSystemAccessRule(identity2, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.AddAccessRule(fileSystemAccessRule);
			fileSystemAccessRule = new FileSystemAccessRule(ObjectSecurity.ExchangeServersUsgSid, FileSystemRights.ReadData, AccessControlType.Allow);
			fileSecurity.AddAccessRule(fileSystemAccessRule);
			return fileSecurity;
		}

		public static FileSecurity BaseRpcSecurity
		{
			get
			{
				return ObjectSecurity.CreateBaseRpcSecurityObject();
			}
		}

		public static FileSecurity ActiveManagerRpcSecurity
		{
			get
			{
				return ObjectSecurity.BaseRpcSecurity;
			}
		}

		public static FileSecurity ReplayRpcSecurity
		{
			get
			{
				return ObjectSecurity.BaseRpcSecurity;
			}
		}

		private static FileSystemAccessRule[] SharedDirectoryRules
		{
			get
			{
				return new FileSystemAccessRule[]
				{
					new FileSystemAccessRule(ObjectSecurity.ExchangeServersUsgSid, FileSystemRights.Read, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow)
				};
			}
		}

		public static SecurityIdentifier ExchangeServersUsgSid
		{
			get
			{
				IADRootOrganizationRecipientSession iadrootOrganizationRecipientSession = ADSessionFactory.CreateIgnoreInvalidRootOrgRecipientSession();
				return iadrootOrganizationRecipientSession.GetExchangeServersUsgSid();
			}
		}

		public const int RetrySleepDurationMilliSecs = 100;

		private static readonly TimeSpan ADRetryDelay = new TimeSpan(0, 0, 3);
	}
}
