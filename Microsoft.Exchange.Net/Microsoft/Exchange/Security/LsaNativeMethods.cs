using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security
{
	internal static class LsaNativeMethods
	{
		[DllImport("secur32.dll")]
		internal static extern int LsaConnectUntrusted(out SafeLsaUntrustedHandle LsaHandle);

		[DllImport("secur32.dll")]
		internal static extern int LsaLookupAuthenticationPackage(SafeLsaUntrustedHandle LsaHandle, LsaNativeMethods.LsaAnsiString PackageName, out int AuthenticationPackage);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("secur32.dll")]
		internal static extern int LsaDeregisterLogonProcess(IntPtr LsaHandle);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("secur32.dll")]
		internal static extern int LsaCallAuthenticationPackage(SafeLsaUntrustedHandle LsaHandle, int packageId, [In] ref LsaNativeMethods.KerberosPurgeTicketCacheRequest request, int requestLength, [In] IntPtr returnBuffer, [In] [Out] ref int returnBufferLength, out int ProtocolStatus);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("secur32.dll", SetLastError = true)]
		internal static extern int LsaCallAuthenticationPackage([In] SafeLsaUntrustedHandle lsaHandle, [In] int authenticationPackage, [In] ref LsaNativeMethods.LiveQueryUserInfoRequest protocolSubmitBuffer, [In] int submitBufferLength, out IntPtr protocolReturnBuffer, out int returnBufferLength, out int protocolStatus);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("secur32.dll")]
		internal static extern int LsaCallAuthenticationPackage(SafeLsaUntrustedHandle LsaHandle, int packageId, IntPtr request, int requestLength, [In] IntPtr returnBuffer, [In] [Out] ref int returnBufferLength, out int ProtocolStatus);

		[DllImport("secur32.dll")]
		internal static extern int LsaFreeReturnBuffer(IntPtr buffer);

		[DllImport("advapi32.dll")]
		internal static extern int LsaOpenPolicy(LsaNativeMethods.LsaUnicodeString target, LsaNativeMethods.LsaObjectAttributes objectAttributes, LsaNativeMethods.PolicyAccess access, out SafeLsaPolicyHandle handle);

		[DllImport("advapi32.dll")]
		internal static extern int LsaQueryInformationPolicy(SafeLsaPolicyHandle handle, LsaNativeMethods.PolicyInformationClass infoClass, out SafeLsaMemoryHandle buffer);

		[DllImport("advapi32.dll")]
		internal static extern int LsaNtStatusToWinError(int ntstatus);

		[DllImport("advapi32.dll")]
		internal static extern int LsaStorePrivateData(SafeLsaPolicyHandle policyHandle, LsaNativeMethods.LsaUnicodeString keyName, LsaNativeMethods.LsaUnicodeString privateData);

		[DllImport("advapi32.dll")]
		internal static extern int LsaAddAccountRights(SafeLsaPolicyHandle policyHandle, [MarshalAs(UnmanagedType.LPArray)] byte[] accountSid, LsaNativeMethods.LsaUnicodeStringStruct[] userRights, int countOfRights);

		[DllImport("advapi32.dll")]
		internal static extern int LsaEnumerateAccountRights(SafeLsaPolicyHandle policyHandle, [MarshalAs(UnmanagedType.LPArray)] byte[] accountSid, out SafeLsaMemoryHandle userRightsPtr, out int countOfRights);

		[DllImport("advapi32.dll")]
		internal static extern int LsaRemoveAccountRights(SafeLsaPolicyHandle policyHandle, [MarshalAs(UnmanagedType.LPArray)] byte[] accountSid, [MarshalAs(UnmanagedType.Bool)] bool allRights, LsaNativeMethods.LsaUnicodeStringStruct[] userRights, int countOfRights);

		private const string ADVAPI32 = "advapi32.dll";

		private const string SECUR32 = "secur32.dll";

		[StructLayout(LayoutKind.Sequential)]
		internal class LsaObjectAttributes
		{
			internal LsaObjectAttributes()
			{
				this.Length = Marshal.SizeOf(typeof(LsaNativeMethods.LsaObjectAttributes));
			}

			private int Length;

			private IntPtr RootDirectory;

			private LsaNativeMethods.LsaUnicodeString ObjectName;

			private int Attributes;

			private IntPtr SecurityDescriptor;

			private IntPtr SecurityQualityOfService;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class LsaUnicodeString
		{
			internal LsaUnicodeString()
			{
			}

			internal LsaUnicodeString(ushort length, ushort maxLength, IntPtr buffer)
			{
				this.length = length;
				this.maxLength = maxLength;
				this.buffer = buffer;
			}

			internal string Value
			{
				get
				{
					if (this.length > 0)
					{
						return Marshal.PtrToStringUni(this.buffer, (int)(this.length / 2));
					}
					return null;
				}
			}

			internal ushort length;

			internal ushort maxLength;

			internal IntPtr buffer;
		}

		internal struct LsaUnicodeStringStruct
		{
			internal LsaUnicodeStringStruct(LsaNativeMethods.LsaUnicodeString value)
			{
				this.length = value.length;
				this.maxLength = value.maxLength;
				this.buffer = value.buffer;
			}

			internal string Value
			{
				get
				{
					if (this.length > 0)
					{
						return Marshal.PtrToStringUni(this.buffer, (int)(this.length / 2));
					}
					return null;
				}
			}

			internal ushort Length
			{
				get
				{
					return this.length;
				}
				set
				{
					this.length = value;
				}
			}

			internal ushort MaxLength
			{
				get
				{
					return this.maxLength;
				}
				set
				{
					this.maxLength = value;
				}
			}

			internal IntPtr Buffer
			{
				get
				{
					return this.buffer;
				}
				set
				{
					this.buffer = value;
				}
			}

			private ushort length;

			private ushort maxLength;

			private IntPtr buffer;
		}

		internal class SafeLsaUnicodeString : LsaNativeMethods.LsaUnicodeString, IDisposable
		{
			internal SafeLsaUnicodeString(string value)
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.gch = GCHandle.Alloc(value, GCHandleType.Pinned);
					this.buffer = this.gch.AddrOfPinnedObject();
					this.length = (ushort)(value.Length * 2);
					this.maxLength = this.length;
				}
			}

			public void Dispose()
			{
				if (this.gch.IsAllocated)
				{
					this.gch.Free();
				}
			}

			private GCHandle gch;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class LsaAnsiString
		{
			internal string Value
			{
				get
				{
					if (this.length > 0)
					{
						return Marshal.PtrToStringAnsi(this.buffer, (int)this.length);
					}
					return null;
				}
			}

			protected ushort length;

			protected ushort maxLength;

			protected IntPtr buffer;
		}

		internal class SafeLsaAnsiString : LsaNativeMethods.LsaAnsiString, IDisposable
		{
			internal SafeLsaAnsiString(string value)
			{
				this.buffer = IntPtr.Zero;
				if (!string.IsNullOrEmpty(value))
				{
					this.buffer = Marshal.StringToHGlobalAnsi(value);
					this.length = (ushort)value.Length;
					this.maxLength = this.length;
				}
			}

			public void Dispose()
			{
				if (this.buffer != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(this.buffer);
					this.buffer = IntPtr.Zero;
				}
			}
		}

		internal struct LUID
		{
			internal LUID(int lowPart, int highPart)
			{
				this.LowPart = lowPart;
				this.HighPart = highPart;
			}

			public int LowPart;

			public int HighPart;
		}

		internal enum KerberosProtocolMessage
		{
			DebugRequest,
			QueryTicketCache,
			ChangeMachinePassword,
			VerifyPac,
			RetrieveTicket,
			UpdateAddresses,
			PurgeTicketCache,
			ChangePassword,
			RetrieveEncodedTicket,
			DecryptData,
			AddBindingCacheEntry,
			SetPassword,
			SetPasswordEx,
			VerifyCredentials,
			QueryTicketCacheEx,
			PurgeTicketCacheEx,
			RefreshSmartcardCredentials,
			AddExtraCredentials,
			QuerySupplementalCredentials,
			TransferCredentials,
			QueryTicketCacheEx2
		}

		internal struct KerberosPurgeTicketCacheRequest
		{
			public KerberosPurgeTicketCacheRequest(int dummy)
			{
				this.messageType = LsaNativeMethods.KerberosProtocolMessage.PurgeTicketCache;
				this.logonId = default(LsaNativeMethods.LUID);
				this.serverName = default(LsaNativeMethods.LsaUnicodeStringStruct);
				this.realmName = default(LsaNativeMethods.LsaUnicodeStringStruct);
			}

			private LsaNativeMethods.KerberosProtocolMessage messageType;

			private LsaNativeMethods.LUID logonId;

			private LsaNativeMethods.LsaUnicodeStringStruct serverName;

			private LsaNativeMethods.LsaUnicodeStringStruct realmName;
		}

		internal struct KerberosAddCredentialsRequestStruct
		{
			public static readonly int SizeOf = Marshal.SizeOf(typeof(LsaNativeMethods.KerberosAddCredentialsRequestStruct));

			public LsaNativeMethods.KerberosProtocolMessage MessageType;

			public LsaNativeMethods.LsaUnicodeStringStruct Username;

			public LsaNativeMethods.LsaUnicodeStringStruct Domain;

			public LsaNativeMethods.LsaUnicodeStringStruct Password;

			public LsaNativeMethods.LUID LogonId;

			public LsaNativeMethods.KerbRequestCredentialFlags Flags;
		}

		internal static class KerberosAddCredentialsRequest
		{
			public unsafe static SafeSecureHGlobalHandle MarshalToNative(string username, string domain, SecureString password, LsaNativeMethods.KerbRequestCredentialFlags flags, LsaNativeMethods.LUID luid)
			{
				int num = LsaNativeMethods.KerberosAddCredentialsRequest.ByteLength(username);
				int num2 = LsaNativeMethods.KerberosAddCredentialsRequest.ByteLength(domain);
				int num3 = LsaNativeMethods.KerberosAddCredentialsRequest.ByteLength(password);
				int size = LsaNativeMethods.KerberosAddCredentialsRequestStruct.SizeOf + num + num2 + num3;
				SafeSecureHGlobalHandle result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					SafeSecureHGlobalHandle safeSecureHGlobalHandle = SafeSecureHGlobalHandle.AllocHGlobal(size);
					disposeGuard.Add<SafeSecureHGlobalHandle>(safeSecureHGlobalHandle);
					LsaNativeMethods.KerberosAddCredentialsRequestStruct* ptr = (LsaNativeMethods.KerberosAddCredentialsRequestStruct*)safeSecureHGlobalHandle.DangerousGetHandle().ToPointer();
					char* ptr2 = (char*)(ptr + 1);
					ptr->MessageType = LsaNativeMethods.KerberosProtocolMessage.AddExtraCredentials;
					LsaNativeMethods.KerberosAddCredentialsRequest.MarshalUnicodeString(username, ref ptr->Username, ref ptr2);
					LsaNativeMethods.KerberosAddCredentialsRequest.MarshalUnicodeString(domain, ref ptr->Domain, ref ptr2);
					LsaNativeMethods.KerberosAddCredentialsRequest.MarshalUnicodeString(password, ref ptr->Password, ref ptr2);
					ptr->LogonId = luid;
					ptr->Flags = flags;
					disposeGuard.Success();
					result = safeSecureHGlobalHandle;
				}
				return result;
			}

			private unsafe static void MarshalUnicodeString(string source, ref LsaNativeMethods.LsaUnicodeStringStruct destination, ref char* extraData)
			{
				destination.Length = (destination.MaxLength = (ushort)LsaNativeMethods.KerberosAddCredentialsRequest.ByteLength(source));
				if (destination.Length == 0)
				{
					destination.Buffer = IntPtr.Zero;
					return;
				}
				fixed (char* ptr = source)
				{
					destination.Buffer = (IntPtr)extraData;
					char* ptr2 = ptr;
					for (int i = 0; i < source.Length; i++)
					{
						char* ptr3;
						extraData = (ptr3 = extraData) + (IntPtr)2;
						*ptr3 = *(ptr2++);
					}
				}
			}

			private unsafe static void MarshalUnicodeString(SecureString source, ref LsaNativeMethods.LsaUnicodeStringStruct destination, ref char* extraData)
			{
				destination.Length = (destination.MaxLength = (ushort)LsaNativeMethods.KerberosAddCredentialsRequest.ByteLength(source));
				if (destination.Length == 0)
				{
					destination.Buffer = IntPtr.Zero;
					return;
				}
				using (SafeSecureHGlobalHandle safeSecureHGlobalHandle = SafeSecureHGlobalHandle.DecryptAndAllocHGlobal(source))
				{
					destination.Buffer = (IntPtr)extraData;
					char* ptr = (char*)safeSecureHGlobalHandle.DangerousGetHandle().ToPointer();
					for (int i = 0; i < source.Length; i++)
					{
						char* ptr2;
						extraData = (ptr2 = extraData) + (IntPtr)2;
						*ptr2 = *(ptr++);
					}
				}
			}

			private static int ByteLength(string str)
			{
				if (!string.IsNullOrEmpty(str))
				{
					return str.Length * 2;
				}
				return 0;
			}

			private static int ByteLength(SecureString str)
			{
				if (str != null)
				{
					return str.Length * 2;
				}
				return 0;
			}
		}

		[Flags]
		internal enum KerbRequestCredentialFlags
		{
			Add = 1,
			Replace = 2,
			Remove = 4
		}

		internal enum LiveProtocolMessageType
		{
			QueryUserInfoMessage = 256
		}

		internal struct LiveQueryUserInfoRequest
		{
			public LiveQueryUserInfoRequest(int dummy)
			{
				this.messageType = LsaNativeMethods.LiveProtocolMessageType.QueryUserInfoMessage;
				this.headerLength = (ushort)Marshal.SizeOf(typeof(LsaNativeMethods.LiveQueryUserInfoRequest));
				this.logonId = default(LsaNativeMethods.LUID);
			}

			private LsaNativeMethods.LiveProtocolMessageType messageType;

			private ushort headerLength;

			private LsaNativeMethods.LUID logonId;
		}

		internal struct LiveQueryUserInfoResponse
		{
			public ulong Cid
			{
				get
				{
					return this.cid;
				}
			}

			public string GetEmailAddress(IntPtr basePointer, int bufferLength)
			{
				if ((long)bufferLength - (long)((ulong)this.emailAddressOffset) < 0L || (long)bufferLength - (long)((ulong)this.emailAddressOffset) - (long)((ulong)this.emailAddressLengthInChars) < 0L)
				{
					throw new Win32Exception("Email Address is pointing outside of the buffer");
				}
				IntPtr ptr = new IntPtr(basePointer.ToInt64() + (long)((ulong)this.emailAddressOffset));
				return Marshal.PtrToStringUni(ptr, (int)this.emailAddressLengthInChars);
			}

			public string GetTicket(IntPtr basePointer, int bufferLength)
			{
				if ((long)bufferLength - (long)((ulong)this.ticketOffset) < 0L || (long)bufferLength - (long)((ulong)this.ticketOffset) - (long)((ulong)this.ticketLengthInChars) < 0L)
				{
					throw new Win32Exception("Ticket is pointing outside of the buffer");
				}
				IntPtr ptr = new IntPtr(basePointer.ToInt64() + (long)((ulong)this.ticketOffset));
				return Marshal.PtrToStringUni(ptr, (int)this.ticketLengthInChars);
			}

			public string GetSiteName(IntPtr basePointer, int bufferLength)
			{
				if ((long)bufferLength - (long)((ulong)this.siteNameOffset) < 0L || (long)bufferLength - (long)((ulong)this.siteNameOffset) - (long)((ulong)this.siteNameLengthInChars) < 0L)
				{
					throw new Win32Exception("Site name is pointing outside of the buffer");
				}
				IntPtr ptr = new IntPtr(basePointer.ToInt64() + (long)((ulong)this.siteNameOffset));
				return Marshal.PtrToStringUni(ptr, (int)this.siteNameLengthInChars);
			}

			private LsaNativeMethods.LiveProtocolMessageType messageType;

			private ushort headerLength;

			private ulong cid;

			private uint emailAddressOffset;

			private ushort emailAddressLengthInChars;

			private uint ticketOffset;

			private ushort ticketLengthInChars;

			private uint siteNameOffset;

			private ushort siteNameLengthInChars;
		}

		internal class PolicyDnsDomainInfo
		{
			internal PolicyDnsDomainInfo(SafeLsaMemoryHandle memory)
			{
				LsaNativeMethods.PolicyDnsDomainInfo.PolicyDnsDomainInfoStruct policyDnsDomainInfoStruct = (LsaNativeMethods.PolicyDnsDomainInfo.PolicyDnsDomainInfoStruct)Marshal.PtrToStructure(memory.DangerousGetHandle(), typeof(LsaNativeMethods.PolicyDnsDomainInfo.PolicyDnsDomainInfoStruct));
				this.Name = policyDnsDomainInfoStruct.Name.Value;
				this.DnsDomainName = policyDnsDomainInfoStruct.DnsDomainName.Value;
				this.DnsForestName = policyDnsDomainInfoStruct.DnsForestName.Value;
				this.DomainGuid = policyDnsDomainInfoStruct.DomainGuid;
				if (policyDnsDomainInfoStruct.Sid != IntPtr.Zero)
				{
					this.Sid = new SecurityIdentifier(policyDnsDomainInfoStruct.Sid);
				}
			}

			internal bool IsDomainMember
			{
				get
				{
					return this.Sid != null;
				}
			}

			internal string Name;

			internal string DnsDomainName;

			internal string DnsForestName;

			internal Guid DomainGuid;

			internal SecurityIdentifier Sid;

			private struct PolicyDnsDomainInfoStruct
			{
				internal LsaNativeMethods.LsaUnicodeStringStruct Name;

				internal LsaNativeMethods.LsaUnicodeStringStruct DnsDomainName;

				internal LsaNativeMethods.LsaUnicodeStringStruct DnsForestName;

				internal Guid DomainGuid;

				internal IntPtr Sid;
			}
		}

		[Flags]
		internal enum PolicyAccess
		{
			ViewLocalInformation = 1,
			ViewAuditInformation = 2,
			GetPrivateInformation = 4,
			TrustAdmin = 8,
			CreateAccount = 16,
			CreateSecret = 32,
			CreatePrivilege = 64,
			SetDefaultQuotaLimits = 128,
			SetAuditRequirements = 256,
			AuditLogAdmin = 512,
			ServerAdmin = 1024,
			LookupNames = 2048,
			Notification = 4096,
			AllAccess = 8191
		}

		internal enum PolicyInformationClass
		{
			AuditLogInformation = 1,
			AuditEventsInformation,
			PrimaryDomainInformation,
			PdAccountInformation,
			AccountDomainInformation,
			LsaServerRoleInformation,
			ReplicaSourceInformation,
			DefaultQuotaInformation,
			ModificationInformation,
			AuditFullSetInformation,
			AuditFullQueryInformation,
			DnsDomainInformation
		}
	}
}
