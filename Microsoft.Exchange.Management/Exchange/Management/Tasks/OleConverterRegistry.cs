using System;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	internal static class OleConverterRegistry
	{
		private static byte[] GetBinaryDescriptor(string sddlDescriptor)
		{
			RawSecurityDescriptor rawSecurityDescriptor = new RawSecurityDescriptor(sddlDescriptor);
			byte[] array = new byte[rawSecurityDescriptor.BinaryLength];
			rawSecurityDescriptor.GetBinaryForm(array, 0);
			return array;
		}

		private static void SetPermissionsForAppId(string appId)
		{
			string subkey = "AppID\\" + appId;
			using (RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(subkey))
			{
				byte[] binaryDescriptor = OleConverterRegistry.GetBinaryDescriptor("O:BAG:BAD:(A;;CCDC;;;BA)(A;;CCDC;;;SY)(A;;CCDC;;;NS)(A;;CCDC;;;LS)(A;;CCDC;;;IU)");
				registryKey.SetValue("AccessPermission", binaryDescriptor);
				byte[] binaryDescriptor2 = OleConverterRegistry.GetBinaryDescriptor("O:BAG:BAD:(A;;CCDCSW;;;BA)(A;;CCDCSW;;;SY)(A;;CCDCSW;;;NS)(A;;CCDCSW;;;LS)(A;;CCDCSW;;;IU)");
				registryKey.SetValue("LaunchPermission", binaryDescriptor2);
			}
		}

		private static void RemovePermissionsForAppId(string appId)
		{
			string name = "AppID\\" + appId;
			using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(name, true))
			{
				if (registryKey != null)
				{
					registryKey.DeleteValue("AccessPermission", false);
					registryKey.DeleteValue("LaunchPermission", false);
				}
			}
		}

		public static void RegisterOleConverter()
		{
			OleConverterRegistry.SetPermissionsForAppId("{13147291-05DE-4577-B1AF-E0BB444B3B27}");
			OleConverterRegistry.SetPermissionsForAppId("{131473D0-EC52-4001-A295-E2DD73A7B115}");
			ComRunAsPwdUtil.SetRunAsPassword("{131473D0-EC52-4001-A295-E2DD73A7B115}", string.Empty);
		}

		public static void UnregisterOleConverter()
		{
			ComRunAsPwdUtil.SetRunAsPassword("{131473D0-EC52-4001-A295-E2DD73A7B115}", null);
			OleConverterRegistry.RemovePermissionsForAppId("{13147291-05DE-4577-B1AF-E0BB444B3B27}");
			OleConverterRegistry.RemovePermissionsForAppId("{131473D0-EC52-4001-A295-E2DD73A7B115}");
		}

		private const string AccessPermission = "O:BAG:BAD:(A;;CCDC;;;BA)(A;;CCDC;;;SY)(A;;CCDC;;;NS)(A;;CCDC;;;LS)(A;;CCDC;;;IU)";

		private const string LaunchPermission = "O:BAG:BAD:(A;;CCDCSW;;;BA)(A;;CCDCSW;;;SY)(A;;CCDCSW;;;NS)(A;;CCDCSW;;;LS)(A;;CCDCSW;;;IU)";

		private const string OleConverterAppIdLocalService = "{131473D0-EC52-4001-A295-E2DD73A7B115}";

		private const string OleConverterAppIdUser = "{13147291-05DE-4577-B1AF-E0BB444B3B27}";
	}
}
