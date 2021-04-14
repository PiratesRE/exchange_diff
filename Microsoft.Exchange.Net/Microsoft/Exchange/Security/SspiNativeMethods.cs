using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	internal static class SspiNativeMethods
	{
		[DllImport("secur32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal unsafe static extern SecurityStatus AcceptSecurityContext(ref SspiHandle credentialHandle, void* inContextPtr, SecurityBufferDescriptor inputBuffer, ContextFlags inFlags, Endianness endianness, ref SspiHandle outContextPtr, SecurityBufferDescriptor outputBuffer, ref ContextFlags attributes, out long timeStamp);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "AcquireCredentialsHandleW")]
		internal unsafe static extern SecurityStatus AcquireCredentialsHandle(string principalName, string packageName, CredentialUse usage, void* logonID, ref SchannelCredential credential, void* keyCallback, void* keyArgument, ref SspiHandle handlePtr, out long timeStamp);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "AcquireCredentialsHandleW")]
		internal unsafe static extern SecurityStatus AcquireCredentialsHandle(string principalName, string packageName, CredentialUse usage, void* logonID, ref AuthIdentity credential, void* keyCallback, void* keyArgument, ref SspiHandle handlePtr, out long timeStamp);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "AcquireCredentialsHandleW")]
		internal unsafe static extern SecurityStatus AcquireCredentialsHandle(string principalName, string packageName, CredentialUse usage, void* logonID, IntPtr zero, void* keyCallback, void* keyArgument, ref SspiHandle handlePtr, out long timeStamp);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "InitializeSecurityContextW")]
		internal unsafe static extern SecurityStatus InitializeSecurityContext(ref SspiHandle credentialHandle, void* inContextPtr, string targetName, ContextFlags inFlags, int reservedI, Endianness endianness, SecurityBufferDescriptor inputBuffer, int reservedII, ref SspiHandle outContextPtr, SecurityBufferDescriptor outputBuffer, ref ContextFlags attributes, out long timeStamp);

		[DllImport("secur32.dll", ExactSpelling = true)]
		internal static extern SecurityStatus FreeContextBuffer(IntPtr contextBuffer);

		[DllImport("secur32.dll", ExactSpelling = true)]
		internal static extern SecurityStatus FreeCredentialsHandle(ref SspiHandle handlePtr);

		[DllImport("secur32.dll", ExactSpelling = true)]
		internal static extern SecurityStatus DeleteSecurityContext(ref SspiHandle handlePtr);

		[DllImport("secur32.dll", EntryPoint = "QueryContextAttributesW")]
		internal static extern SecurityStatus QueryContextAttributes(ref SspiHandle contextHandle, ContextAttribute attribute, byte[] buffer);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "QuerySecurityPackageInfoW")]
		internal static extern SecurityStatus QuerySecurityPackageInfo(string packageName, out SafeContextBuffer secPkgInfo);

		[DllImport("secur32.dll", ExactSpelling = true)]
		internal static extern SecurityStatus DecryptMessage(ref SspiHandle handlePtr, SecurityBufferDescriptor inOut, uint sequenceNumber, out QualityOfProtection qualityOfProtection);

		[DllImport("secur32.dll", ExactSpelling = true)]
		internal static extern SecurityStatus EncryptMessage(ref SspiHandle handlePtr, QualityOfProtection qualityOfProtection, SecurityBufferDescriptor inOut, uint sequenceNumber);

		[DllImport("crypt32.dll", ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CertFreeCertificateContext(IntPtr certContext);

		[DllImport("secur32.dll")]
		internal static extern SecurityStatus QuerySecurityContextToken(ref SspiHandle contextHandle, out SafeTokenHandle token);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "QueryCredentialsAttributesW")]
		internal static extern SecurityStatus QueryCredentialsAttributes(ref SspiHandle handlePtr, CredentialsAttribute credentialAttribute, byte[] buffer);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle(IntPtr handle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "LogonUserW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool LogonUser(string username, string domain, IntPtr password, LogonType logonType, LogonProvider logonProvider, ref SafeTokenHandle token);

		[DllImport("advapi32.dll", EntryPoint = "LogonUserA", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool LogonUser(byte[] username, byte[] domain, byte[] password, LogonType logonType, LogonProvider logonProvider, ref SafeTokenHandle token);

		public const string UnifiedProviderName = "Microsoft Unified Security Protocol Provider";

		private const string SECUR32 = "secur32.dll";

		private const string CRYPT32 = "crypt32.dll";

		private const string ADVAPI32 = "advapi32.dll";

		private const string KERNEL32 = "kernel32.dll";
	}
}
