using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc
{
	internal class RpcClientBase : IDisposable
	{
		protected unsafe void* BindingHandle
		{
			get
			{
				return this.m_bindingHandle;
			}
		}

		protected unsafe RpcClientBase(RpcBindingInfo bindingInfo, void* interfaceHandle)
		{
			this.Initialize(bindingInfo, interfaceHandle);
		}

		protected RpcClientBase(RpcBindingInfo bindingInfo)
		{
			this.Initialize(bindingInfo, null);
		}

		protected unsafe RpcClientBase(string machineName, string proxyServer, string protocolSequence, string servicePrincipalName, RpcClientFlags flags, void* interfaceHandle, ValueType clientObjectGuid, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService)
		{
			this.Initialize3(machineName, proxyServer, "<none>", protocolSequence, servicePrincipalName, nc, flags, httpAuthenticationScheme, authenticationService, interfaceHandle, clientObjectGuid, string.Empty);
		}

		protected unsafe RpcClientBase(string machineName, string proxyServer, string protocolSequence, string servicePrincipalName, void* interfaceHandle, ValueType clientObjectGuid, [MarshalAs(UnmanagedType.U1)] bool useEncryptedConnection, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate, [MarshalAs(UnmanagedType.U1)] bool uniqueBinding)
		{
			RpcClientFlags rpcClientFlags = allowImpersonation ? RpcClientFlags.AllowImpersonation : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags2 = useEncryptedConnection ? RpcClientFlags.UseEncryptedConnection : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags3 = ignoreInvalidServerCertificate ? RpcClientFlags.IgnoreInvalidServerCertificate : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags4 = uniqueBinding ? RpcClientFlags.UniqueBinding : RpcClientFlags.None;
			RpcClientFlags flags = rpcClientFlags4 | rpcClientFlags3 | rpcClientFlags2 | rpcClientFlags | RpcClientFlags.UseSsl;
			this.Initialize3(machineName, proxyServer, "<none>", protocolSequence, servicePrincipalName, nc, flags, httpAuthenticationScheme, authenticationService, interfaceHandle, clientObjectGuid, string.Empty);
		}

		protected unsafe RpcClientBase(string machineName, string proxyServer, string protocolSequence, string servicePrincipalName, void* interfaceHandle, ValueType clientObjectGuid, [MarshalAs(UnmanagedType.U1)] bool useEncryptedConnection, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate)
		{
			RpcClientFlags rpcClientFlags = allowImpersonation ? RpcClientFlags.AllowImpersonation : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags2 = useEncryptedConnection ? RpcClientFlags.UseEncryptedConnection : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags3 = ignoreInvalidServerCertificate ? RpcClientFlags.IgnoreInvalidServerCertificate : RpcClientFlags.None;
			RpcClientFlags flags = rpcClientFlags3 | rpcClientFlags2 | rpcClientFlags | RpcClientFlags.UseSsl;
			this.Initialize3(machineName, proxyServer, "<none>", protocolSequence, servicePrincipalName, nc, flags, httpAuthenticationScheme, authenticationService, interfaceHandle, clientObjectGuid, string.Empty);
		}

		protected RpcClientBase(string machineName, string proxyServer, string protocolSequence, string servicePrincipalName, [MarshalAs(UnmanagedType.U1)] bool useEncryptedConnection, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate, string certificateSubjectName)
		{
			RpcClientFlags rpcClientFlags = allowImpersonation ? RpcClientFlags.AllowImpersonation : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags2 = useEncryptedConnection ? RpcClientFlags.UseEncryptedConnection : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags3 = ignoreInvalidServerCertificate ? RpcClientFlags.IgnoreInvalidServerCertificate : RpcClientFlags.None;
			this.Initialize3(machineName, proxyServer, "<none>", protocolSequence, servicePrincipalName, nc, rpcClientFlags3 | rpcClientFlags2 | rpcClientFlags | RpcClientFlags.UseSsl, httpAuthenticationScheme, authenticationService, null, null, certificateSubjectName);
		}

		protected RpcClientBase(string machineName, string proxyServer, string protocolSequence, string servicePrincipalName, [MarshalAs(UnmanagedType.U1)] bool useEncryptedConnection, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate, [MarshalAs(UnmanagedType.U1)] bool uniqueBinding)
		{
			RpcClientFlags rpcClientFlags = allowImpersonation ? RpcClientFlags.AllowImpersonation : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags2 = useEncryptedConnection ? RpcClientFlags.UseEncryptedConnection : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags3 = ignoreInvalidServerCertificate ? RpcClientFlags.IgnoreInvalidServerCertificate : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags4 = uniqueBinding ? RpcClientFlags.UniqueBinding : RpcClientFlags.None;
			RpcClientFlags flags = rpcClientFlags4 | rpcClientFlags3 | rpcClientFlags2 | rpcClientFlags | RpcClientFlags.UseSsl;
			this.Initialize3(machineName, proxyServer, "<none>", protocolSequence, servicePrincipalName, nc, flags, httpAuthenticationScheme, authenticationService, null, null, string.Empty);
		}

		protected RpcClientBase(string machineName, string proxyServer, string protocolSequence, string servicePrincipalName, [MarshalAs(UnmanagedType.U1)] bool useEncryptedConnection, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation)
		{
			this.Initialize1(machineName, proxyServer, protocolSequence, servicePrincipalName, nc, httpAuthenticationScheme, authenticationService, null, null, allowImpersonation, useEncryptedConnection);
		}

		protected RpcClientBase(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, [MarshalAs(UnmanagedType.U1)] bool useEncryptedConnection, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation)
		{
			this.Initialize1(machineName, proxyServer, protocolSequence, null, nc, httpAuthenticationScheme, authenticationService, null, null, allowImpersonation, useEncryptedConnection);
		}

		protected RpcClientBase(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation)
		{
			this.Initialize1(machineName, proxyServer, protocolSequence, null, nc, httpAuthenticationScheme, authenticationService, null, null, allowImpersonation, true);
		}

		protected RpcClientBase(string machineName, string proxyServer, string protocolSequence, [MarshalAs(UnmanagedType.U1)] bool useEncryptedConnection, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate, [MarshalAs(UnmanagedType.U1)] bool uniqueBinding)
		{
			RpcClientFlags rpcClientFlags = allowImpersonation ? RpcClientFlags.AllowImpersonation : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags2 = useEncryptedConnection ? RpcClientFlags.UseEncryptedConnection : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags3 = ignoreInvalidServerCertificate ? RpcClientFlags.IgnoreInvalidServerCertificate : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags4 = uniqueBinding ? RpcClientFlags.UniqueBinding : RpcClientFlags.None;
			RpcClientFlags flags = rpcClientFlags4 | rpcClientFlags3 | rpcClientFlags2 | rpcClientFlags | RpcClientFlags.UseSsl;
			this.Initialize3(machineName, proxyServer, "<none>", protocolSequence, null, nc, flags, httpAuthenticationScheme, authenticationService, null, null, string.Empty);
		}

		protected unsafe RpcClientBase(string machineName, string protocolSequence, NetworkCredential nc, AuthenticationService authenticationService, void* interfaceHandle, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation)
		{
			this.Initialize1(machineName, null, protocolSequence, null, nc, HttpAuthenticationScheme.Basic, authenticationService, interfaceHandle, null, allowImpersonation, true);
		}

		protected unsafe RpcClientBase(string machineName, NetworkCredential nc, void* interfaceHandle)
		{
			this.Initialize1(machineName, null, null, null, nc, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate, interfaceHandle, null, false, true);
		}

		protected RpcClientBase(string machineName, NetworkCredential nc, AuthenticationService authenticationService)
		{
			this.Initialize1(machineName, null, null, null, nc, HttpAuthenticationScheme.Basic, authenticationService, null, null, false, true);
		}

		protected RpcClientBase(string machineName, NetworkCredential nc)
		{
			this.Initialize1(machineName, null, null, null, nc, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate, null, null, false, true);
		}

		protected RpcClientBase(string machineName, ValueType clientObjectGuid)
		{
			this.Initialize1(machineName, null, null, null, null, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate, null, clientObjectGuid, false, true);
		}

		protected RpcClientBase(string machineName)
		{
			this.Initialize1(machineName, null, null, null, null, HttpAuthenticationScheme.Basic, AuthenticationService.Negotiate, null, null, false, true);
		}

		protected unsafe RpcClientBase(string machineName, string proxyServer, string protocolSequence, NetworkCredential nc, AuthenticationService authenticationService, void* interfaceHandle, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation)
		{
			this.Initialize1(machineName, proxyServer, protocolSequence, null, nc, HttpAuthenticationScheme.Basic, authenticationService, interfaceHandle, null, allowImpersonation, true);
		}

		protected RpcClientBase(string machineName, string protocolSequence, NetworkCredential nc, AuthenticationService authenticationService)
		{
			this.Initialize1(machineName, null, protocolSequence, null, nc, HttpAuthenticationScheme.Basic, authenticationService, null, null, false, true);
		}

		protected void ResetRetryCounter()
		{
			this.accessDeniedRetryCount = 0;
			this.busyRetryCount = 0;
			this.unavailableRetryCount = 0;
			this.callCancelledRetryCount = 0;
		}

		protected int RetryRpcCall(int rpc_status, RpcRetryType flags)
		{
			int result = 0;
			if (rpc_status != 5)
			{
				if (rpc_status != 1722)
				{
					if (rpc_status != 1723 && rpc_status != 1727)
					{
						if (rpc_status == 1818 && (flags & RpcRetryType.CallCancelled) != RpcRetryType.None && this.callCancelledRetryCount < 1)
						{
							<Module>.Sleep(3000);
							this.callCancelledRetryCount++;
							result = 1;
						}
					}
					else if ((flags & RpcRetryType.ServerBusy) != RpcRetryType.None && this.busyRetryCount < 6)
					{
						<Module>.Sleep(1000);
						this.busyRetryCount++;
						result = 1;
					}
				}
				else if ((flags & RpcRetryType.ServerUnavailable) != RpcRetryType.None && this.unavailableRetryCount < 1)
				{
					<Module>.Sleep(3000);
					this.unavailableRetryCount++;
					result = 1;
				}
			}
			else if ((flags & RpcRetryType.AccessDenied) != RpcRetryType.None)
			{
				int num = this.accessDeniedRetryCount;
				if (num < 1)
				{
					this.accessDeniedRetryCount = num + 1;
					result = 1;
				}
			}
			return result;
		}

		private void ~RpcClientBase()
		{
			this.!RpcClientBase();
		}

		private unsafe void !RpcClientBase()
		{
			void* bindingHandle = this.m_bindingHandle;
			if (bindingHandle != null)
			{
				void* ptr = bindingHandle;
				<Module>.RpcBindingFree(&ptr);
				this.m_bindingHandle = null;
			}
		}

		internal static void IfFailed_ThrowRpcExceptionWithEEInfo(int rpcStatus, string routineName)
		{
			if (rpcStatus != 0)
			{
				<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(rpcStatus, routineName);
			}
		}

		internal static void ThrowRpcException(int rpcStatus, string routineName)
		{
			string message = string.Format("Error 0x{0:x} ({2}) from {1}", rpcStatus, routineName, new Win32Exception(rpcStatus).Message);
			throw <Module>.GetRpcException(rpcStatus, message);
		}

		public void SetTimeOut(int value)
		{
			int num = <Module>.RpcBindingSetOption(this.m_bindingHandle, 12, (ulong)value);
			if (num != null)
			{
				RpcClientBase.ThrowRpcException(num, "RpcBindingSetOption");
			}
		}

		public unsafe int Ping()
		{
			RPC_STATS_VECTOR* ptr = null;
			int result;
			try
			{
				result = <Module>.RpcMgmtInqStats(this.m_bindingHandle, &ptr);
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.RpcMgmtStatsVectorFree(&ptr);
				}
			}
			return result;
		}

		public unsafe void SetCookie(string name, string value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			string text = string.Format("{0}={1}", name, value);
			SafeMarshalHGlobalHandle safeMarshalHGlobalHandle = new SafeMarshalHGlobalHandle(Marshal.StringToHGlobalAnsi(text));
			IntPtr intPtr = safeMarshalHGlobalHandle.DangerousGetHandle();
			_RPC_C_OPT_COOKIE_AUTH_DESCRIPTOR rpc_C_OPT_COOKIE_AUTH_DESCRIPTOR;
			*(ref rpc_C_OPT_COOKIE_AUTH_DESCRIPTOR + 8) = intPtr.ToPointer();
			rpc_C_OPT_COOKIE_AUTH_DESCRIPTOR = text.Length + 1;
			try
			{
				int num = <Module>.RpcBindingSetOption(this.m_bindingHandle, 7, ref rpc_C_OPT_COOKIE_AUTH_DESCRIPTOR);
				if (num != null)
				{
					RpcClientBase.ThrowRpcException(num, "RpcBindingSetOption(RPC_C_OPT_COOKIE_AUTH)");
				}
			}
			finally
			{
				if (safeMarshalHGlobalHandle != null)
				{
					((IDisposable)safeMarshalHGlobalHandle).Dispose();
				}
			}
		}

		public unsafe string GetBindingString()
		{
			ushort* value = null;
			string result;
			try
			{
				void* bindingHandle = this.m_bindingHandle;
				string routineName = "RpcBindingToStringBinding";
				int num = <Module>.RpcBindingToStringBindingW(bindingHandle, &value);
				if (num != 0)
				{
					throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num, routineName);
				}
				result = Marshal.PtrToStringUni((IntPtr)((void*)value));
			}
			finally
			{
				<Module>.RpcStringFreeW(&value);
			}
			return result;
		}

		private unsafe void Initialize1(string machineName, string proxyServer, string protocolSequence, string servicePrincipalName, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, void* interfaceHandle, ValueType clientObjectGuid, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation, [MarshalAs(UnmanagedType.U1)] bool useEncryptedConnection)
		{
			RpcClientFlags rpcClientFlags = allowImpersonation ? RpcClientFlags.AllowImpersonation : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags2 = useEncryptedConnection ? RpcClientFlags.UseEncryptedConnection : RpcClientFlags.None;
			RpcClientFlags flags = rpcClientFlags2 | rpcClientFlags | RpcClientFlags.UseSsl;
			this.Initialize3(machineName, proxyServer, "<none>", protocolSequence, servicePrincipalName, nc, flags, httpAuthenticationScheme, authenticationService, interfaceHandle, clientObjectGuid, string.Empty);
		}

		private unsafe void Initialize2(string machineName, string proxyServer, string protocolSequence, string servicePrincipalName, NetworkCredential nc, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, void* interfaceHandle, ValueType clientObjectGuid, [MarshalAs(UnmanagedType.U1)] bool allowImpersonation, [MarshalAs(UnmanagedType.U1)] bool useEncryptedConnection, [MarshalAs(UnmanagedType.U1)] bool ignoreInvalidServerCertificate)
		{
			RpcClientFlags rpcClientFlags = allowImpersonation ? RpcClientFlags.AllowImpersonation : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags2 = useEncryptedConnection ? RpcClientFlags.UseEncryptedConnection : RpcClientFlags.None;
			RpcClientFlags rpcClientFlags3 = ignoreInvalidServerCertificate ? RpcClientFlags.IgnoreInvalidServerCertificate : RpcClientFlags.None;
			RpcClientFlags flags = rpcClientFlags3 | rpcClientFlags2 | rpcClientFlags | RpcClientFlags.UseSsl;
			this.Initialize3(machineName, proxyServer, "<none>", protocolSequence, servicePrincipalName, nc, flags, httpAuthenticationScheme, authenticationService, interfaceHandle, clientObjectGuid, string.Empty);
		}

		private unsafe void Initialize3(string machineName, string proxyServer, string webProxyServer, string protocolSequence, string servicePrincipalName, NetworkCredential nc, RpcClientFlags flags, HttpAuthenticationScheme httpAuthenticationScheme, AuthenticationService authenticationService, void* interfaceHandle, ValueType clientObjectGuid, string certificateSubjectName)
		{
			RpcBindingInfo rpcBindingInfo = new RpcBindingInfo();
			rpcBindingInfo.RpcServer = machineName;
			rpcBindingInfo.RpcProxyServer = proxyServer;
			rpcBindingInfo.UseProtocolSequenceWithOptionalRpcPortSpecification(protocolSequence);
			rpcBindingInfo.ServicePrincipalName = servicePrincipalName;
			rpcBindingInfo.Credential = nc;
			rpcBindingInfo.RpcProxyAuthentication = httpAuthenticationScheme;
			rpcBindingInfo.RpcAuthentication = authenticationService;
			byte allowImpersonation = (byte)flags & 1;
			rpcBindingInfo.AllowImpersonation = (allowImpersonation != 0);
			byte ignoreInvalidRpcProxyServerCertificateSubject = (byte)(flags >> 2 & RpcClientFlags.AllowImpersonation);
			rpcBindingInfo.IgnoreInvalidRpcProxyServerCertificateSubject = (ignoreInvalidRpcProxyServerCertificateSubject != 0);
			byte useUniqueBinding = (byte)(flags >> 3 & RpcClientFlags.AllowImpersonation);
			rpcBindingInfo.UseUniqueBinding = (useUniqueBinding != 0);
			byte useExplicitEndpointLookup = (byte)(flags >> 4 & RpcClientFlags.AllowImpersonation);
			rpcBindingInfo.UseExplicitEndpointLookup = (useExplicitEndpointLookup != 0);
			byte useSsl = (byte)(flags >> 5 & RpcClientFlags.AllowImpersonation);
			rpcBindingInfo.UseSsl = (useSsl != 0);
			rpcBindingInfo.ClientCertificateSubjectName = certificateSubjectName;
			byte useRpcEncryption;
			if (authenticationService != AuthenticationService.None && (flags & RpcClientFlags.UseEncryptedConnection) != RpcClientFlags.None)
			{
				useRpcEncryption = 1;
			}
			else
			{
				useRpcEncryption = 0;
			}
			rpcBindingInfo.UseRpcEncryption = (useRpcEncryption != 0);
			if (clientObjectGuid != null)
			{
				rpcBindingInfo.ClientObjectGuid = (Guid)clientObjectGuid;
			}
			this.Initialize(rpcBindingInfo, interfaceHandle);
		}

		private unsafe void Initialize(RpcBindingInfo bindingInfo, void* interfaceHandle)
		{
			MarshalledString marshalledString = null;
			MarshalledString marshalledString2 = null;
			MarshalledString marshalledString3 = null;
			MarshalledString marshalledString4 = null;
			string value = null;
			string name = null;
			MarshalledString marshalledString5 = null;
			_CERT_CONTEXT* ptr = null;
			ushort* ptr2 = null;
			bool flag = false;
			try
			{
				bindingInfo.DefaultOmittedProperties();
				bool flag2 = bindingInfo.ProtocolSequence == "ncalrpc";
				byte b = (!string.IsNullOrEmpty(bindingInfo.ClientCertificateSubjectName)) ? 1 : 0;
				bool flag3 = b;
				if (bindingInfo.UseRpcEncryption && bindingInfo.RpcAuthentication == AuthenticationService.None)
				{
					throw new RpcException("Cannot specify encryption for anonymous connections", 1746);
				}
				if (flag2 != null)
				{
					if (<Module>.Microsoft.Exchange.Rpc.IsLocalSystem() != null && bindingInfo.Credential != null)
					{
						throw new InvalidOperationException("Cannot use LRPC with explicit credentials while running as LocalSystem");
					}
					if (flag3 != null)
					{
						throw new InvalidOperationException("Cannot use LRPC with certificate authentication");
					}
				}
				else if (flag3 != null)
				{
					bindingInfo.RpcAuthentication = AuthenticationService.SecureChannel;
					bindingInfo.RpcProxyAuthentication = HttpAuthenticationScheme.Certificate;
				}
				if (string.IsNullOrEmpty(bindingInfo.ServicePrincipalName))
				{
					if (bindingInfo.RpcAuthentication != AuthenticationService.Kerberos && bindingInfo.RpcAuthentication != AuthenticationService.Negotiate)
					{
						if (bindingInfo.RpcAuthentication == AuthenticationService.SecureChannel)
						{
							bindingInfo.ServicePrincipalName = bindingInfo.ExpectedRpcProxyServerCertificateSubject;
						}
					}
					else
					{
						bindingInfo.UseKerberosSpn(null, null);
					}
				}
				this.m_bindingHandle = RpcClientBase.CreateBinding(bindingInfo, interfaceHandle);
				this.SetTimeOut((int)bindingInfo.Timeout.TotalMilliseconds);
				if (flag2 == null)
				{
					string routineName = "RpcMgmtSetComTimeout";
					int num = <Module>.RpcMgmtSetComTimeout(this.m_bindingHandle, 0U);
					if (num != 0)
					{
						throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num, routineName);
					}
				}
				if (bindingInfo.UseUniqueBinding)
				{
					string routineName2 = "RpcBindingSetOption(RPC_C_OPT_UNIQUE_BINDING)";
					int num2 = <Module>.RpcBindingSetOption(this.m_bindingHandle, 11, 1UL);
					if (num2 != 0)
					{
						throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num2, routineName2);
					}
				}
				if (interfaceHandle != null)
				{
					string routineName3 = "RpcEpResolveBinding";
					int num3 = <Module>.RpcEpResolveBinding(this.m_bindingHandle, interfaceHandle);
					if (num3 != 0)
					{
						throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num3, routineName3);
					}
				}
				uint rpcProxyAuthentication = bindingInfo.RpcProxyAuthentication;
				MarshalledString marshalledString6 = new MarshalledString(bindingInfo.ExpectedRpcProxyServerCertificateSubject);
				try
				{
					marshalledString = marshalledString6;
					string stringToMarshal;
					if (bindingInfo.Credential != null && !string.IsNullOrEmpty(bindingInfo.Credential.Domain))
					{
						stringToMarshal = bindingInfo.Credential.Domain;
					}
					else
					{
						stringToMarshal = null;
					}
					MarshalledString marshalledString7 = new MarshalledString(stringToMarshal);
					try
					{
						marshalledString2 = marshalledString7;
						MarshalledString marshalledString8 = new MarshalledString((bindingInfo.Credential == null) ? null : bindingInfo.Credential.UserName);
						try
						{
							marshalledString3 = marshalledString8;
							string stringToMarshal2;
							if (bindingInfo.Credential != null && !string.IsNullOrEmpty(bindingInfo.Credential.Password))
							{
								stringToMarshal2 = bindingInfo.Credential.Password;
							}
							else
							{
								stringToMarshal2 = null;
							}
							MarshalledString marshalledString9 = new MarshalledString(stringToMarshal2);
							try
							{
								marshalledString4 = marshalledString9;
								_SCHANNEL_CRED schannel_CRED = 0;
								initblk(ref schannel_CRED + 4, 0, 76L);
								_SEC_WINNT_AUTH_IDENTITY_W sec_WINNT_AUTH_IDENTITY_W = 0L;
								initblk(ref sec_WINNT_AUTH_IDENTITY_W + 8, 0, 40L);
								_SEC_WINNT_AUTH_IDENTITY_W* ptr3 = null;
								_SEC_WINNT_AUTH_IDENTITY_W sec_WINNT_AUTH_IDENTITY_W2 = 0L;
								initblk(ref sec_WINNT_AUTH_IDENTITY_W2 + 8, 0, 40L);
								_RPC_HTTP_TRANSPORT_CREDENTIALS_W rpc_HTTP_TRANSPORT_CREDENTIALS_W = 0L;
								initblk(ref rpc_HTTP_TRANSPORT_CREDENTIALS_W + 8, 0, 32L);
								_RPC_SECURITY_QOS_V2_W rpc_SECURITY_QOS_V2_W = 0;
								initblk(ref rpc_SECURITY_QOS_V2_W + 4, 0, 28L);
								rpc_SECURITY_QOS_V2_W = 2;
								*(ref rpc_SECURITY_QOS_V2_W + 8) = 0;
								if (bindingInfo.Credential != null)
								{
									*(ref sec_WINNT_AUTH_IDENTITY_W + 44) = 2;
									*(ref sec_WINNT_AUTH_IDENTITY_W + 16) = marshalledString2.op_Implicit();
									*(ref sec_WINNT_AUTH_IDENTITY_W + 24) = marshalledString2.Length;
									sec_WINNT_AUTH_IDENTITY_W = marshalledString3.op_Implicit();
									*(ref sec_WINNT_AUTH_IDENTITY_W + 8) = marshalledString3.Length;
									*(ref sec_WINNT_AUTH_IDENTITY_W + 32) = marshalledString4.op_Implicit();
									*(ref sec_WINNT_AUTH_IDENTITY_W + 40) = marshalledString4.Length;
									ptr3 = &sec_WINNT_AUTH_IDENTITY_W;
								}
								uint num4;
								if (bindingInfo.RpcAuthentication == AuthenticationService.None)
								{
									num4 = 1;
									*(ref rpc_SECURITY_QOS_V2_W + 12) = 1;
								}
								else
								{
									num4 = (bindingInfo.UseRpcEncryption ? 6 : 5);
									*(ref rpc_SECURITY_QOS_V2_W + 12) = (bindingInfo.AllowImpersonation ? 3 : 2);
								}
								int num5;
								if (bindingInfo.RpcAuthentication != AuthenticationService.Negotiate && bindingInfo.RpcAuthentication != AuthenticationService.Kerberos && bindingInfo.RpcAuthentication != AuthenticationService.SecureChannel)
								{
									num5 = 0;
								}
								else
								{
									num5 = 1;
								}
								*(ref rpc_SECURITY_QOS_V2_W + 4) = num5;
								if (flag3 != null)
								{
									ptr = RpcClientBase.GetMyCertificate(bindingInfo.ClientCertificateSubjectName);
									schannel_CRED = 4;
									*(ref schannel_CRED + 4) = 1;
									*(ref schannel_CRED + 8) = ref ptr;
									*(ref schannel_CRED + 72) = 1048;
								}
								if (bindingInfo.ProtocolSequence == "ncacn_http")
								{
									*(ref rpc_HTTP_TRANSPORT_CREDENTIALS_W + 16) = 1;
									*(ref rpc_HTTP_TRANSPORT_CREDENTIALS_W + 24) = ref rpcProxyAuthentication;
									*(ref rpc_HTTP_TRANSPORT_CREDENTIALS_W + 8) = 2;
									*(ref rpc_HTTP_TRANSPORT_CREDENTIALS_W + 12) = 1;
									*(ref rpc_HTTP_TRANSPORT_CREDENTIALS_W + 32) = marshalledString.op_Implicit();
									if (bindingInfo.UseSsl)
									{
										*(ref rpc_HTTP_TRANSPORT_CREDENTIALS_W + 8) = (*(ref rpc_HTTP_TRANSPORT_CREDENTIALS_W + 8) | 1);
									}
									if (bindingInfo.IgnoreInvalidRpcProxyServerCertificateSubject)
									{
										*(ref rpc_HTTP_TRANSPORT_CREDENTIALS_W + 8) = (*(ref rpc_HTTP_TRANSPORT_CREDENTIALS_W + 8) | 8);
									}
									if (flag3 != null)
									{
										ptr2 = RpcClientBase.GetAuthIdentityForClientCert(&sec_WINNT_AUTH_IDENTITY_W2, ptr, *(ref sec_WINNT_AUTH_IDENTITY_W + 32));
										rpc_HTTP_TRANSPORT_CREDENTIALS_W = ref sec_WINNT_AUTH_IDENTITY_W2;
										*(ref schannel_CRED + 56) = 128;
									}
									else
									{
										rpc_HTTP_TRANSPORT_CREDENTIALS_W = ptr3;
									}
									*(ref rpc_SECURITY_QOS_V2_W + 16) = 1;
									*(ref rpc_SECURITY_QOS_V2_W + 24) = ref rpc_HTTP_TRANSPORT_CREDENTIALS_W;
									if (bindingInfo.PackHeadersAndCookiesIntoRpcCookie(out name, out value))
									{
										this.SetCookie(name, value);
									}
								}
								MarshalledString marshalledString10 = new MarshalledString(bindingInfo.ServicePrincipalName);
								try
								{
									marshalledString5 = marshalledString10;
									_SEC_WINNT_AUTH_IDENTITY_W* ptr4 = ref (flag3 != 0) ? ref schannel_CRED : ref *(_SCHANNEL_CRED*)ptr3;
									string routineName4 = "RpcBindingSetAuthInfoExW";
									int num6 = <Module>.RpcBindingSetAuthInfoExW(this.m_bindingHandle, marshalledString5.op_Implicit(), num4, bindingInfo.RpcAuthentication, (void*)ptr4, 0, (_RPC_SECURITY_QOS*)(&rpc_SECURITY_QOS_V2_W));
									if (num6 != 0)
									{
										throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num6, routineName4);
									}
									flag = true;
								}
								catch
								{
									((IDisposable)marshalledString5).Dispose();
									throw;
								}
								((IDisposable)marshalledString5).Dispose();
							}
							catch
							{
								((IDisposable)marshalledString4).Dispose();
								throw;
							}
							((IDisposable)marshalledString4).Dispose();
						}
						catch
						{
							((IDisposable)marshalledString3).Dispose();
							throw;
						}
						((IDisposable)marshalledString3).Dispose();
					}
					catch
					{
						((IDisposable)marshalledString2).Dispose();
						throw;
					}
					((IDisposable)marshalledString2).Dispose();
				}
				catch
				{
					((IDisposable)marshalledString).Dispose();
					throw;
				}
				((IDisposable)marshalledString).Dispose();
			}
			finally
			{
				if (!flag)
				{
					((IDisposable)this).Dispose();
				}
				<Module>.CertFreeCertificateContext(ptr);
				<Module>.CredFree((void*)ptr2);
			}
		}

		private unsafe static void* CreateBinding(RpcBindingInfo bindingInfo, void* interfaceHandle)
		{
			MarshalledString marshalledString = null;
			MarshalledString marshalledString2 = null;
			MarshalledString marshalledString3 = null;
			MarshalledString marshalledString4 = null;
			ushort* ptr = null;
			ushort* ptr2 = null;
			void* result;
			try
			{
				if (bindingInfo.UseExplicitEndpointLookup)
				{
					result = RpcClientBase.FindObjectBinding(bindingInfo, interfaceHandle);
				}
				else
				{
					_GUID guid = 0;
					initblk(ref guid + 4, 0, 12L);
					if (bindingInfo.ClientObjectGuid != Guid.Empty)
					{
						_GUID guid2 = <Module>.Microsoft.Exchange.Rpc.?A0x31f30f38.GUIDFromGuid(bindingInfo.ClientObjectGuid);
						guid = guid2;
					}
					else if (interfaceHandle != null)
					{
						string routineName = "RpcIfInqId";
						_RPC_IF_ID rpc_IF_ID;
						int num = <Module>.RpcIfInqId(interfaceHandle, &rpc_IF_ID);
						if (num != 0)
						{
							throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num, routineName);
						}
						cpblk(ref guid, ref rpc_IF_ID, 16);
					}
					string routineName2 = "UuidToString";
					int num2 = <Module>.UuidToStringW((_GUID*)(&guid), &ptr2);
					if (num2 != 0)
					{
						throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num2, routineName2);
					}
					MarshalledString marshalledString5 = new MarshalledString(bindingInfo.RpcServer);
					void* ptr4;
					try
					{
						marshalledString = marshalledString5;
						MarshalledString marshalledString6 = new MarshalledString(bindingInfo.ProtocolSequence);
						try
						{
							marshalledString2 = marshalledString6;
							MarshalledString marshalledString7 = new MarshalledString(bindingInfo.RpcPort.ToString());
							try
							{
								marshalledString3 = marshalledString7;
								MarshalledString marshalledString8 = new MarshalledString(RpcClientBase.GetBindingOptions(bindingInfo));
								try
								{
									marshalledString4 = marshalledString8;
									string routineName3 = "RpcStringBindingCompose";
									int num3 = <Module>.RpcStringBindingComposeW(ptr2, marshalledString2.op_Implicit(), marshalledString.op_Implicit(), marshalledString3.op_Implicit(), marshalledString4.op_Implicit(), &ptr);
									if (num3 != 0)
									{
										throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num3, routineName3);
									}
									string routineName4 = "RpcBindingFromStringBinding";
									void* ptr3;
									int num4 = <Module>.RpcBindingFromStringBindingW(ptr, &ptr3);
									if (num4 != 0)
									{
										throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num4, routineName4);
									}
									ptr4 = ptr3;
								}
								catch
								{
									((IDisposable)marshalledString4).Dispose();
									throw;
								}
								((IDisposable)marshalledString4).Dispose();
							}
							catch
							{
								((IDisposable)marshalledString3).Dispose();
								throw;
							}
							((IDisposable)marshalledString3).Dispose();
						}
						catch
						{
							((IDisposable)marshalledString2).Dispose();
							throw;
						}
						((IDisposable)marshalledString2).Dispose();
					}
					catch
					{
						((IDisposable)marshalledString).Dispose();
						throw;
					}
					((IDisposable)marshalledString).Dispose();
					result = ptr4;
				}
			}
			finally
			{
				<Module>.RpcStringFreeW(&ptr2);
				<Module>.RpcStringFreeW(&ptr);
			}
			return result;
		}

		private static string GetBindingOptions(RpcBindingInfo bindingInfo)
		{
			string text = null;
			if (bindingInfo.ProtocolSequence == "ncacn_http")
			{
				text += string.Format("RpcProxy={0}:{1}", bindingInfo.RpcProxyServer, (int)bindingInfo.RpcProxyPort);
				if (!string.IsNullOrEmpty(bindingInfo.WebProxyServer))
				{
					text += string.Format(", HttpProxy={0}", bindingInfo.WebProxyServer);
				}
			}
			return text;
		}

		private unsafe static void* FindObjectBinding(RpcBindingInfo bindingInfo, void* interfaceHandle)
		{
			MarshalledString marshalledString = null;
			MarshalledString marshalledString2 = null;
			MarshalledString marshalledString3 = null;
			ushort* ptr = null;
			void* ptr2 = null;
			void** ptr3 = null;
			void* result;
			try
			{
				if (bindingInfo.ProtocolSequence == "ncacn_ip_tcp")
				{
					MarshalledString marshalledString4 = new MarshalledString("ncacn_ip_tcp");
					try
					{
						marshalledString = marshalledString4;
						MarshalledString marshalledString5 = new MarshalledString(bindingInfo.RpcServer);
						try
						{
							marshalledString2 = marshalledString5;
							MarshalledString marshalledString6 = new MarshalledString(RpcClientBase.GetBindingOptions(bindingInfo));
							try
							{
								marshalledString3 = marshalledString6;
								string routineName = "RpcStringBindingCompose(EPM)";
								int num = <Module>.RpcStringBindingComposeW(null, marshalledString.op_Implicit(), marshalledString2.op_Implicit(), null, marshalledString3.op_Implicit(), &ptr);
								if (num != 0)
								{
									throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num, routineName);
								}
								string routineName2 = "RpcBindingFromStringBinding(EPM)";
								int num2 = <Module>.RpcBindingFromStringBindingW(ptr, &ptr2);
								if (num2 != 0)
								{
									throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num2, routineName2);
								}
							}
							catch
							{
								((IDisposable)marshalledString3).Dispose();
								throw;
							}
							((IDisposable)marshalledString3).Dispose();
						}
						catch
						{
							((IDisposable)marshalledString2).Dispose();
							throw;
						}
						((IDisposable)marshalledString2).Dispose();
					}
					catch
					{
						((IDisposable)marshalledString).Dispose();
						throw;
					}
					((IDisposable)marshalledString).Dispose();
				}
				else if (bindingInfo.ProtocolSequence != "ncalrpc")
				{
					throw new ArgumentException("Explicit endpoint lookup requires protocol to be ncalrpc or ncacn_tcp.");
				}
				if (interfaceHandle == null)
				{
					throw new ArgumentException("Explicit endpoint lookup requires an interface to be specified.");
				}
				string routineName3 = "RpcIfInqId";
				_RPC_IF_ID rpc_IF_ID;
				int num3 = <Module>.RpcIfInqId(interfaceHandle, &rpc_IF_ID);
				if (num3 != 0)
				{
					throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num3, routineName3);
				}
				_GUID guid = <Module>.Microsoft.Exchange.Rpc.?A0x31f30f38.GUIDFromGuid(bindingInfo.ClientObjectGuid);
				_GUID guid2 = guid;
				string routineName4 = "RpcMgmtEpEltInqBegin";
				int num4 = <Module>.RpcMgmtEpEltInqBegin(ptr2, 3, &rpc_IF_ID, 1, &guid2, &ptr3);
				if (num4 != 0)
				{
					throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num4, routineName4);
				}
				for (;;)
				{
					void* ptr4 = null;
					try
					{
						string routineName5 = "RpcMgmtEpEltInqNext";
						_RPC_IF_ID rpc_IF_ID2;
						_GUID guid3;
						int num5 = <Module>.RpcMgmtEpEltInqNextW(ptr3, &rpc_IF_ID2, &ptr4, &guid3, null);
						if (num5 != 0)
						{
							throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num5, routineName5);
						}
						ulong num6 = 16UL;
						_GUID* ptr5 = &guid2;
						_GUID* ptr6 = &guid3;
						int num7 = 0;
						for (;;)
						{
							byte b = *(byte*)ptr6;
							byte b2 = *(byte*)ptr5;
							if (b < b2 || b > b2)
							{
								goto IL_1FD;
							}
							if (num6 == 1UL)
							{
								break;
							}
							num6 -= 1UL;
							ptr6 += 1L / (long)sizeof(_GUID);
							ptr5 += 1L / (long)sizeof(_GUID);
						}
						if (0 == num7)
						{
							string routineName6 = "RpcBindingSetObject";
							int num8 = <Module>.RpcBindingSetObject(ptr4, &guid2);
							if (num8 != 0)
							{
								throw <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num8, routineName6);
							}
							void* ptr7 = ptr4;
							ptr4 = null;
							result = ptr7;
							break;
						}
						IL_1FD:;
					}
					finally
					{
						if (ptr4 != null)
						{
							<Module>.RpcBindingFree(&ptr4);
						}
					}
				}
			}
			finally
			{
				<Module>.RpcStringFreeW(&ptr);
				if (ptr2 != null)
				{
					<Module>.RpcBindingFree(&ptr2);
				}
				if (ptr3 != null)
				{
					<Module>.RpcMgmtEpEltInqDone(&ptr3);
				}
			}
			return result;
		}

		private unsafe static ushort* GetAuthIdentityForClientCert(_SEC_WINNT_AUTH_IDENTITY_W* certificateCredentials, _CERT_CONTEXT* pDesiredCert, ushort* wszPin)
		{
			ulong num = 0UL;
			ulong num2 = 0UL;
			ushort* ptr = null;
			ushort* result;
			try
			{
				if (<Module>.CryptAcquireContextW(&num, null, (ushort*)(&<Module>.??_C@_1FG@GAFGNOGK@?$AAM?$AAi?$AAc?$AAr?$AAo?$AAs?$AAo?$AAf?$AAt?$AA?5?$AAB?$AAa?$AAs?$AAe?$AA?5?$AAC?$AAr?$AAy?$AAp?$AAt?$AAo?$AAg?$AAr?$AAa?$AAp?$AAh?$AAi?$AAc?$AA?5?$AAP?$AAr?$AAo@), 1, -268435456) == null)
				{
					RpcClientBase.ThrowRpcException(<Module>.GetLastError(), "CryptAcquireContext failed");
				}
				if (<Module>.CryptCreateHash(num, 32772U, 0UL, 0, &num2) == null)
				{
					RpcClientBase.ThrowRpcException(<Module>.GetLastError(), "CryptCreateHash failed");
				}
				if (<Module>.CryptHashData(num2, *(long*)(pDesiredCert + 8L / (long)sizeof(_CERT_CONTEXT)), *(int*)(pDesiredCert + 16L / (long)sizeof(_CERT_CONTEXT)), 0) == null)
				{
					RpcClientBase.ThrowRpcException(<Module>.GetLastError(), "CryptHashData failed");
				}
				uint num3 = 20;
				_CERT_CREDENTIAL_INFO cert_CREDENTIAL_INFO = 0;
				initblk(ref cert_CREDENTIAL_INFO + 4, 0, 20L);
				cert_CREDENTIAL_INFO = 24;
				if (<Module>.CryptGetHashParam(num2, 2, ref cert_CREDENTIAL_INFO + 4, &num3, 0) == null)
				{
					RpcClientBase.ThrowRpcException(<Module>.GetLastError(), "CryptGetHashParam failed");
				}
				if (<Module>.CredMarshalCredentialW((_CRED_MARSHAL_TYPE)1, (void*)(&cert_CREDENTIAL_INFO), &ptr) == null)
				{
					RpcClientBase.ThrowRpcException(<Module>.GetLastError(), "CredMarshalCredentialW failed");
				}
				*(long*)certificateCredentials = ptr;
				ushort* ptr2 = ptr;
				while (*(short*)ptr2 != 0)
				{
					ptr2 += 2L / 2L;
				}
				ulong num4 = (ulong)(ptr2 - ptr >> 1);
				*(int*)(certificateCredentials + 8L / (long)sizeof(_SEC_WINNT_AUTH_IDENTITY_W)) = (int)((uint)num4);
				*(long*)(certificateCredentials + 32L / (long)sizeof(_SEC_WINNT_AUTH_IDENTITY_W)) = wszPin;
				ulong num5;
				if (wszPin != null)
				{
					ushort* ptr3 = wszPin;
					while (*(short*)ptr3 != 0)
					{
						ptr3 += 2L / 2L;
					}
					num5 = (ulong)(ptr3 - wszPin >> 1);
				}
				else
				{
					num5 = 0UL;
				}
				*(int*)(certificateCredentials + 40L / (long)sizeof(_SEC_WINNT_AUTH_IDENTITY_W)) = (int)((uint)num5);
				*(long*)(certificateCredentials + 16L / (long)sizeof(_SEC_WINNT_AUTH_IDENTITY_W)) = 0L;
				*(int*)(certificateCredentials + 24L / (long)sizeof(_SEC_WINNT_AUTH_IDENTITY_W)) = 0;
				*(int*)(certificateCredentials + 44L / (long)sizeof(_SEC_WINNT_AUTH_IDENTITY_W)) = 2;
				result = ptr;
			}
			finally
			{
				if (num2 != 0UL)
				{
					<Module>.CryptDestroyHash(num2);
				}
				if (num != 0UL)
				{
					<Module>.CryptReleaseContext(num, 0);
				}
			}
			return result;
		}

		private unsafe static _CERT_CONTEXT* GetMyCertificate(string certificateSubjectName)
		{
			MarshalledString marshalledString = null;
			MarshalledString marshalledString2 = new MarshalledString(certificateSubjectName);
			_CERT_CONTEXT* result;
			try
			{
				marshalledString = marshalledString2;
				void* ptr = null;
				try
				{
					ptr = <Module>.CertOpenSystemStoreW(0UL, (ushort*)(&<Module>.??_C@_15ELPOAHMG@?$AAM?$AAY?$AA?$AA@));
					if (ptr != null)
					{
						_CERT_CONTEXT* ptr2 = <Module>.CertFindCertificateInStore(ptr, 65537, 0, 524295, (void*)marshalledString.op_Implicit(), null);
						if (ptr2 != null)
						{
							result = ptr2;
							goto IL_83;
						}
						RpcClientBase.ThrowRpcException(<Module>.GetLastError(), "Certificate not found");
					}
					else
					{
						RpcClientBase.ThrowRpcException(<Module>.GetLastError(), "CertOpenSystemStore");
					}
				}
				finally
				{
					if (ptr != null)
					{
						<Module>.CertCloseStore(ptr, 0);
					}
				}
			}
			catch
			{
				((IDisposable)marshalledString).Dispose();
				throw;
			}
			((IDisposable)marshalledString).Dispose();
			return null;
			IL_83:
			((IDisposable)marshalledString).Dispose();
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				this.~RpcClientBase();
			}
			else
			{
				try
				{
					this.!RpcClientBase();
				}
				finally
				{
					base.Finalize();
				}
			}
		}

		public sealed void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected override void Finalize()
		{
			this.Dispose(false);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static RpcClientBase()
		{
			RpcClientBase.unavailableRetryMax = 1;
			RpcClientBase.busyRetryMax = 6;
			RpcClientBase.accessDeniedRetryMax = 1;
		}

		private unsafe void* m_bindingHandle;

		private static int accessDeniedRetryMax = 1;

		private static int busyRetryMax = 6;

		private static int unavailableRetryMax = 1;

		private static int callCancelledRetryMax = 1;

		private int accessDeniedRetryCount;

		private int busyRetryCount;

		private int unavailableRetryCount;

		private int callCancelledRetryCount;
	}
}
