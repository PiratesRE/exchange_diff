using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Security.Cryptography
{
	[SuppressUnmanagedCodeSecurity]
	[ComVisible(false)]
	internal static class CngNativeMethods
	{
		[DllImport("Ncrypt.dll", CharSet = CharSet.Unicode)]
		public static extern int NCryptOpenStorageProvider(out SafeNCryptHandle provider, string providerName, uint reserved);

		[DllImport("Ncrypt.dll")]
		public static extern int NCryptFreeObject(IntPtr handle);

		[DllImport("Ncrypt.dll", CharSet = CharSet.Unicode)]
		public static extern int NCryptOpenKey(SafeNCryptHandle provider, out SafeNCryptHandle key, string keyName, uint legacyKeySpecifier, CngNativeMethods.KeyOptions options);

		[DllImport("Ncrypt.dll", CharSet = CharSet.Unicode)]
		public static extern int NCryptGetProperty(SafeNCryptHandle owner, string property, out uint value, uint valueSize, out uint bytes, CngNativeMethods.PropertyOptions options);

		[DllImport("Ncrypt.dll", CharSet = CharSet.Unicode)]
		public static extern int NCryptGetProperty(SafeNCryptHandle owner, string property, [Out] byte[] value, uint valueSize, out uint bytes, CngNativeMethods.PropertyOptions options);

		[DllImport("Ncrypt.dll", CharSet = CharSet.Unicode)]
		public static extern int NCryptSetProperty(SafeNCryptHandle owner, string property, [In] byte[] value, uint valueSize, CngNativeMethods.PropertyOptions options);

		private const string NCRYPT = "Ncrypt.dll";

		public const string ImplementationTypeProperty = "Impl Type";

		public const string LengthProperty = "Length";

		public const string SecurityDescriptorProperty = "Security Descr";

		public const string ExportPolicyProperty = "Export Policy";

		public enum SecurityStatus : uint
		{
			BadKeyset = 2148073494U,
			SilentContext = 2148073506U
		}

		[Flags]
		public enum KeyOptions : uint
		{
			MachineKeyset = 32U,
			Silent = 64U
		}

		[Flags]
		public enum ImplemenationType : uint
		{
			Hardware = 1U,
			Software = 2U,
			Removable = 8U,
			HardwareRandomNumberGenerator = 16U
		}

		public enum PropertyOptions : uint
		{
			OwnerSecurityInformation = 1U,
			GroupSecurityInformation,
			DACLSecurityInformation = 4U,
			SACLSecurityInformation = 8U
		}

		[Flags]
		public enum AllowExportPolicy
		{
			Exportable = 1,
			PlaintextExportable = 2,
			Archivable = 4,
			PlaintextArchivable = 8
		}
	}
}
