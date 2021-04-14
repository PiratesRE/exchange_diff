using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Security.Cryptography;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SmtpAuth : DisposeTrackableBase
	{
		internal SmtpAuth(ISmtpClientDebugOutput smtpClientDebugOutput)
		{
			this.smtpClientDebugOutput = smtpClientDebugOutput;
		}

		internal static bool VerifyResponse(byte[] certPubKey, byte[] tlsKey, byte[] clientKerbSessionKey, byte[] serverKerbSessionKey, string response)
		{
			byte[] array = Convert.FromBase64String(response);
			byte[] array2 = new byte[certPubKey.Length + clientKerbSessionKey.Length + tlsKey.Length + serverKerbSessionKey.Length];
			Buffer.BlockCopy(certPubKey, 0, array2, 0, certPubKey.Length);
			Buffer.BlockCopy(clientKerbSessionKey, 0, array2, certPubKey.Length, clientKerbSessionKey.Length);
			Buffer.BlockCopy(tlsKey, 0, array2, clientKerbSessionKey.Length + certPubKey.Length, tlsKey.Length);
			Buffer.BlockCopy(serverKerbSessionKey, 0, array2, certPubKey.Length + clientKerbSessionKey.Length + tlsKey.Length, serverKerbSessionKey.Length);
			byte[] array3 = null;
			using (SHA256CryptoServiceProvider sha256CryptoServiceProvider = new SHA256CryptoServiceProvider())
			{
				array3 = sha256CryptoServiceProvider.ComputeHash(array2);
			}
			if (array.Length != array3.Length)
			{
				return false;
			}
			for (int i = 0; i < array3.Length; i++)
			{
				if (array[i] != array3[i])
				{
					return false;
				}
			}
			return true;
		}

		internal static string GenerateChallenge(byte[] certPubKey, byte[] gssKey, byte[] tlsKey)
		{
			byte[] array = new byte[certPubKey.Length + gssKey.Length + tlsKey.Length];
			Buffer.BlockCopy(certPubKey, 0, array, 0, certPubKey.Length);
			Buffer.BlockCopy(gssKey, 0, array, certPubKey.Length, gssKey.Length);
			Buffer.BlockCopy(tlsKey, 0, array, certPubKey.Length + gssKey.Length, tlsKey.Length);
			string result;
			using (SHA256CryptoServiceProvider sha256CryptoServiceProvider = new SHA256CryptoServiceProvider())
			{
				result = Convert.ToBase64String(sha256CryptoServiceProvider.ComputeHash(array));
			}
			return result;
		}

		internal string HandleOutboundAuth(string mutualBlob, string targetSPN, byte[] sslCertificatePublicKey, byte[] sslSessionKey, bool firstTime)
		{
			base.CheckDisposed();
			string text = null;
			string text2 = null;
			if (firstTime)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "Creating Authentication Provider Outbound", new object[0]);
				this.outboundAuthProvider = new SmtpAuth.AuthenticationProvider(SmtpSspiMechanism.Kerberos, null, null, null, targetSPN, 2, this.smtpClientDebugOutput);
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "Creating Authentication Provider Inbound", new object[0]);
				this.inboundAuthProvider = new SmtpAuth.AuthenticationProvider(SmtpSspiMechanism.Kerberos, null, null, null, null, 1, this.smtpClientDebugOutput);
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "GetAuthenticationString", new object[0]);
				text = this.outboundAuthProvider.GetAuthenticationString(null, true);
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "GenerateChallenge", new object[0]);
				string challengeRepsonse = SmtpAuth.GenerateChallenge(sslCertificatePublicKey, this.outboundAuthProvider.GetSessionKey(), sslSessionKey);
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "MutualString", new object[0]);
				return SmtpAuth.ConstructAuthString(text, challengeRepsonse);
			}
			this.ParseAuthString(mutualBlob, ref text, ref text2);
			if (text == null || text2 == null)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "We havent received both the ticket and response", new object[0]);
				throw new AuthFailureException();
			}
			string authenticationString = this.inboundAuthProvider.GetAuthenticationString(text, true);
			if (!SmtpAuth.VerifyResponse(sslCertificatePublicKey, sslSessionKey, this.outboundAuthProvider.GetSessionKey(), this.inboundAuthProvider.GetSessionKey(), text2))
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "Failed to Verify Mutual GSSAPI Response", new object[0]);
				throw new AuthFailureException();
			}
			if (this.inboundAuthProvider.State != AuthenticationState.Secured || authenticationString != null)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "Kerberose ticket verification failed", new object[0]);
				throw new AuthFailureException();
			}
			return string.Empty;
		}

		public string GetInitialBlob(NetworkCredential creds, SmtpSspiMechanism saslMechanism)
		{
			if (saslMechanism == SmtpSspiMechanism.Login)
			{
				string s = (!string.IsNullOrEmpty(creds.Domain)) ? (creds.Domain + "\\" + creds.UserName) : creds.UserName;
				return Convert.ToBase64String(Encoding.ASCII.GetBytes(s));
			}
			this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), string.Format("{0} is not an expected SMTP authentication mechanism", saslMechanism), new object[0]);
			throw new UnsupportedAuthMechanismException(saslMechanism.ToString());
		}

		public string GetNextBlob(byte[] serverChallenge, NetworkCredential creds, SmtpSspiMechanism saslMechanism)
		{
			if (saslMechanism == SmtpSspiMechanism.Login)
			{
				return Convert.ToBase64String(Encoding.ASCII.GetBytes(creds.Password));
			}
			this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), string.Format("{0} is not an expected SMTP authentication mechanism", saslMechanism), new object[0]);
			throw new UnsupportedAuthMechanismException(saslMechanism.ToString());
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "SmtpAuth::Dispose()", new object[0]);
				if (this.inboundAuthProvider != null)
				{
					this.inboundAuthProvider.Dispose();
					this.inboundAuthProvider = null;
				}
				if (this.outboundAuthProvider != null)
				{
					this.outboundAuthProvider.Dispose();
					this.outboundAuthProvider = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SmtpAuth>(this);
		}

		private static string ConstructAuthString(string securityBlob, string challengeRepsonse)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (securityBlob != null)
			{
				string value = string.Format(CultureInfo.InvariantCulture, "{0:X}", new object[]
				{
					securityBlob.Length
				}).PadLeft(8, '0');
				stringBuilder.Append(value);
				stringBuilder.Append(securityBlob);
			}
			else
			{
				stringBuilder.Append("00000000");
			}
			if (challengeRepsonse != null)
			{
				string value = string.Format(CultureInfo.InvariantCulture, "{0:X}", new object[]
				{
					challengeRepsonse.Length
				}).PadLeft(8, '0');
				stringBuilder.Append(value);
				stringBuilder.Append(challengeRepsonse);
			}
			else
			{
				stringBuilder.Append("00000000");
			}
			return stringBuilder.ToString();
		}

		private void ExtractOneField(string input, ref int offset, ref string output)
		{
			if (offset + 8 >= input.Length)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "size blob corruption", new object[0]);
				throw new AuthFailureException();
			}
			int num = int.Parse(input.Substring(offset, 8), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			offset += 8;
			if (num < 0 || offset + num > input.Length)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "size blob corruption", new object[0]);
				throw new AuthFailureException();
			}
			if (num != 0)
			{
				output = input.Substring(offset, num);
				offset += num;
			}
		}

		private void ParseAuthString(string authBlob, ref string securityBlob, ref string challengeRespose)
		{
			int num = 0;
			this.ExtractOneField(authBlob, ref num, ref securityBlob);
			this.ExtractOneField(authBlob, ref num, ref challengeRespose);
			if (num != authBlob.Length)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "Auth blob parsing doesn't match exact length", new object[0]);
				throw new AuthFailureException();
			}
		}

		private SmtpAuth.AuthenticationProvider inboundAuthProvider;

		private SmtpAuth.AuthenticationProvider outboundAuthProvider;

		private ISmtpClientDebugOutput smtpClientDebugOutput;

		internal class AuthenticationProvider : DisposeTrackableBase
		{
			internal AuthenticationProvider(SmtpSspiMechanism sspiMechanism, string userName, string domainName, string password, string targetName, int authenticationDirection, ISmtpClientDebugOutput smtpClientDebugOutput)
			{
				this.mechanism = sspiMechanism;
				this.authUserName = userName;
				this.authDomainName = domainName;
				this.authPassword = password;
				this.authTargetName = targetName;
				this.authDirection = authenticationDirection;
				this.smtpClientDebugOutput = smtpClientDebugOutput;
				SmtpSspiMechanism smtpSspiMechanism = this.mechanism;
				if (smtpSspiMechanism != SmtpSspiMechanism.Kerberos)
				{
					this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), string.Format("{0} is not an expected SMTP authentication mechanism", this.mechanism), new object[0]);
					throw new UnsupportedAuthMechanismException(this.mechanism.ToString());
				}
				this.InitializeSSPI();
			}

			internal AuthenticationState State
			{
				get
				{
					base.CheckDisposed();
					return this.state;
				}
				set
				{
					base.CheckDisposed();
					this.state = value;
				}
			}

			internal string GetAuthenticationString(string responseBlob, bool is64bitEncode)
			{
				base.CheckDisposed();
				byte[] array = null;
				byte[] array2 = null;
				if (responseBlob != null && responseBlob.Length != 0)
				{
					array = (is64bitEncode ? Convert.FromBase64String(responseBlob) : Encoding.ASCII.GetBytes(responseBlob));
				}
				if (this.mechanism == SmtpSspiMechanism.Kerberos)
				{
					array2 = ((this.authDirection == 2) ? this.GetSSPIClientResponse(array) : this.GetSSPIServerResponse(array));
				}
				if (array2 == null)
				{
					return null;
				}
				if (!is64bitEncode)
				{
					return Encoding.ASCII.GetString(array2);
				}
				return Convert.ToBase64String(array2);
			}

			internal byte[] GetSSPIClientResponse(byte[] serverBlob)
			{
				base.CheckDisposed();
				IntPtr intPtr = IntPtr.Zero;
				IntPtr intPtr2 = IntPtr.Zero;
				IntPtr intPtr3 = IntPtr.Zero;
				IntPtr intPtr4 = IntPtr.Zero;
				NativeMethods.SEC_STATUS secStatus = 0;
				bool isZero = this.ctxHandle.IsZero;
				byte[] result;
				try
				{
					NativeMethods.SecBuffer secBuffer = default(NativeMethods.SecBuffer);
					secBuffer.cbBuffer = this.maxToken;
					secBuffer.BufferType = 2;
					intPtr2 = Marshal.AllocHGlobal(this.maxToken);
					secBuffer.buffer = intPtr2;
					NativeMethods.SecBufferDesc secBufferDesc = new NativeMethods.SecBufferDesc();
					secBufferDesc.ulVersion = 0;
					secBufferDesc.cBuffers = 1;
					intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(secBuffer));
					Marshal.StructureToPtr(secBuffer, intPtr, false);
					secBufferDesc.pBuffer = intPtr;
					long num = 0L;
					int num2 = 0;
					SmtpSspiMechanism smtpSspiMechanism = this.mechanism;
					if (smtpSspiMechanism != SmtpSspiMechanism.Kerberos)
					{
						this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), string.Format("{0} is not an expected SMTP authentication mechanism", this.mechanism), new object[0]);
						throw new UnsupportedAuthMechanismException(this.mechanism.ToString());
					}
					int fContextReq = 16400;
					if (isZero)
					{
						secStatus = NativeMethods.InitializeSecurityContext(ref this.credHandle, IntPtr.Zero, this.authTargetName, fContextReq, 0, NativeMethods.Endianness.Network, IntPtr.Zero, 0, ref this.ctxHandle, secBufferDesc, ref num2, out num);
					}
					else
					{
						int num3 = serverBlob.Length;
						NativeMethods.SecBuffer secBuffer2 = default(NativeMethods.SecBuffer);
						secBuffer2.cbBuffer = num3;
						secBuffer2.BufferType = 2;
						intPtr4 = Marshal.AllocHGlobal(num3);
						secBuffer2.buffer = intPtr4;
						Marshal.Copy(serverBlob, 0, intPtr4, serverBlob.Length);
						NativeMethods.SecBufferDesc secBufferDesc2 = new NativeMethods.SecBufferDesc();
						secBufferDesc2.ulVersion = 0;
						secBufferDesc2.cBuffers = 1;
						intPtr3 = Marshal.AllocHGlobal(Marshal.SizeOf(secBuffer2));
						Marshal.StructureToPtr(secBuffer2, intPtr3, false);
						secBufferDesc2.pBuffer = intPtr3;
						secStatus = NativeMethods.InitializeSecurityContext_NextSslBlob(ref this.credHandle, ref this.ctxHandle, this.authTargetName, fContextReq, 0, NativeMethods.Endianness.Network, secBufferDesc2, 0, ref this.ctxHandle, secBufferDesc, ref num2, out num);
					}
					int num4 = secStatus;
					byte[] array;
					if (num4 != 0)
					{
						if (num4 != 590610)
						{
							this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "InitializeSecurityContext inside GetSPPIClientResponse Failed with Error Code = " + secStatus.ToString(), new object[0]);
							throw new AuthApiFailureException(new Win32Exception(secStatus).Message);
						}
						secBuffer = (NativeMethods.SecBuffer)Marshal.PtrToStructure(intPtr, typeof(NativeMethods.SecBuffer));
						if (secBuffer.cbBuffer == 0)
						{
							this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "outputBuffer.cbBuffer == 0", new object[0]);
							throw new AuthFailureException();
						}
						array = new byte[secBuffer.cbBuffer];
						Marshal.Copy(intPtr2, array, 0, secBuffer.cbBuffer);
						this.State = AuthenticationState.Negotiating;
					}
					else
					{
						secBuffer = (NativeMethods.SecBuffer)Marshal.PtrToStructure(intPtr, typeof(NativeMethods.SecBuffer));
						if (secBuffer.cbBuffer == 0)
						{
							this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "outputBuffer.cbBuffer == 0", new object[0]);
							throw new AuthFailureException();
						}
						array = new byte[secBuffer.cbBuffer];
						Marshal.Copy(intPtr2, array, 0, secBuffer.cbBuffer);
						this.State = AuthenticationState.Secured;
					}
					result = array;
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(intPtr);
					}
					if (intPtr2 != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(intPtr2);
					}
					if (intPtr3 != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(intPtr3);
					}
					if (intPtr4 != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(intPtr4);
					}
				}
				return result;
			}

			internal byte[] GetSSPIServerResponse(byte[] clientBlob)
			{
				base.CheckDisposed();
				bool isZero = this.ctxHandle.IsZero;
				byte[] array = null;
				IntPtr intPtr = IntPtr.Zero;
				IntPtr intPtr2 = IntPtr.Zero;
				IntPtr intPtr3 = IntPtr.Zero;
				NativeMethods.SecBufferDesc secBufferDesc = new NativeMethods.SecBufferDesc();
				NativeMethods.SecBufferDesc secBufferDesc2 = new NativeMethods.SecBufferDesc();
				GCHandle gchandle = default(GCHandle);
				try
				{
					if (clientBlob != null)
					{
						intPtr = Marshal.AllocHGlobal(clientBlob.Length);
						Marshal.Copy(clientBlob, 0, intPtr, clientBlob.Length);
						NativeMethods.SecBuffer[] array2 = new NativeMethods.SecBuffer[2];
						array2[0].cbBuffer = clientBlob.Length;
						array2[0].BufferType = 2;
						array2[0].buffer = intPtr;
						array2[1].cbBuffer = 0;
						array2[1].BufferType = 0;
						array2[1].buffer = IntPtr.Zero;
						gchandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
						secBufferDesc.ulVersion = 0;
						secBufferDesc.cBuffers = 2;
						secBufferDesc.pBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(array2, 0);
					}
					NativeMethods.SecBuffer secBuffer = default(NativeMethods.SecBuffer);
					secBuffer.cbBuffer = this.maxToken;
					secBuffer.BufferType = 2;
					intPtr3 = Marshal.AllocHGlobal(secBuffer.cbBuffer);
					secBuffer.buffer = intPtr3;
					secBufferDesc2 = new NativeMethods.SecBufferDesc();
					secBufferDesc2.ulVersion = 0;
					secBufferDesc2.cBuffers = 1;
					intPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf(secBuffer));
					Marshal.StructureToPtr(secBuffer, intPtr2, false);
					secBufferDesc2.pBuffer = intPtr2;
					NativeMethods.SEC_STATUS secStatus = 0;
					NativeMethods.ContextFlags contextFlags = NativeMethods.ContextFlags.ReplayDetect | NativeMethods.ContextFlags.SequenceDetect | NativeMethods.ContextFlags.Confidentiality | NativeMethods.ContextFlags.AcceptExtendedError | NativeMethods.ContextFlags.AcceptStream;
					long num = 0L;
					if (isZero)
					{
						this.ctxHandle = default(NativeMethods.CredHandle);
						secStatus = NativeMethods.AcceptSecurityContext(ref this.credHandle, IntPtr.Zero, (intPtr != IntPtr.Zero) ? secBufferDesc : null, NativeMethods.ContextFlags.Zero, NativeMethods.Endianness.Network, ref this.ctxHandle, secBufferDesc2, ref contextFlags, out num);
					}
					else
					{
						secStatus = NativeMethods.AcceptSecurityContext(ref this.credHandle, ref this.ctxHandle, (intPtr != IntPtr.Zero) ? secBufferDesc : null, NativeMethods.ContextFlags.Zero, NativeMethods.Endianness.Network, ref this.ctxHandle, secBufferDesc2, ref contextFlags, out num);
					}
					int num2 = secStatus;
					if (num2 != 0)
					{
						if (num2 != 590610)
						{
							this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "AcceptSecurityContext failed with " + secStatus.ToString(), new object[0]);
							throw new AuthApiFailureException(new Win32Exception(secStatus).Message);
						}
						this.State = AuthenticationState.Negotiating;
					}
					else
					{
						this.State = AuthenticationState.Secured;
					}
					secBuffer = default(NativeMethods.SecBuffer);
					secBuffer = (NativeMethods.SecBuffer)Marshal.PtrToStructure(intPtr2, typeof(NativeMethods.SecBuffer));
					if (secBuffer.cbBuffer != 0)
					{
						array = new byte[secBuffer.cbBuffer];
						Marshal.Copy(intPtr3, array, 0, secBuffer.cbBuffer);
					}
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(intPtr);
						intPtr = IntPtr.Zero;
					}
					if (intPtr3 != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(intPtr3);
						intPtr3 = IntPtr.Zero;
					}
					if (intPtr2 != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(intPtr2);
						intPtr2 = IntPtr.Zero;
					}
					if (gchandle.IsAllocated)
					{
						gchandle.Free();
					}
				}
				return array;
			}

			internal byte[] GetSessionKey()
			{
				base.CheckDisposed();
				if (this.mechanism != SmtpSspiMechanism.Kerberos)
				{
					this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "Currently not supported other than Kerberoes key", new object[0]);
					throw new AuthFailureException();
				}
				NativeMethods.SessionKey sessionKey = default(NativeMethods.SessionKey);
				NativeMethods.SEC_STATUS secStatus = NativeMethods.QueryContextAttributes(ref this.ctxHandle, NativeMethods.ContextAttribute.SECPKG_ATTR_SESSION_KEY, ref sessionKey);
				if (secStatus == 0)
				{
					byte[] array = new byte[sessionKey.SessionKeyLength];
					Marshal.Copy(sessionKey.SessionKeyValuePtr, array, 0, sessionKey.SessionKeyLength);
					return array;
				}
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "QueryContext Attribute for SessionKey failed with error " + secStatus.ToString(), new object[0]);
				throw new AuthApiFailureException(new Win32Exception(secStatus).Message);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "AuthenticationProvider::Dispose()", new object[0]);
					if (!this.ctxHandle.IsZero)
					{
						int num = NativeMethods.DeleteSecurityContext(ref this.ctxHandle);
						this.ctxHandle.SetToInvalid();
						this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "AuthenticationProvider::Dispose() DeleteSecurityContext returned {0:x}", new object[]
						{
							num
						});
					}
					if (!this.credHandle.IsZero)
					{
						int num = NativeMethods.FreeCredentialsHandle(ref this.credHandle);
						this.credHandle.SetToInvalid();
						this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "AuthenticationProvider::Dispose() FreeCredentialsHandle returned {0:x}", new object[]
						{
							num
						});
					}
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<SmtpAuth.AuthenticationProvider>(this);
			}

			private void InitializeSSPI()
			{
				IntPtr zero = IntPtr.Zero;
				IntPtr zero2 = IntPtr.Zero;
				string empty = string.Empty;
				NativeMethods.AuthData authData = default(NativeMethods.AuthData);
				long num = 0L;
				NativeMethods.SEC_STATUS secStatus = 0;
				if (this.authUserName != null && this.authUserName != string.Empty)
				{
					authData.Domain = this.authDomainName;
					authData.DomainLength = this.authDomainName.Length;
					authData.Flags = 2;
					authData.Password = this.authPassword;
					authData.PasswordLength = this.authPassword.Length;
					authData.User = this.authUserName;
					authData.UserLength = this.authUserName.Length;
					secStatus = NativeMethods.AcquireCredentialsHandle(null, "Kerberos", this.authDirection, IntPtr.Zero, ref authData, IntPtr.Zero, IntPtr.Zero, ref this.credHandle, out num);
				}
				else
				{
					secStatus = NativeMethods.AcquireCredentialsHandle(null, "Kerberos", this.authDirection, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, ref this.credHandle, out num);
				}
				if (secStatus != 0 || this.credHandle.IsZero)
				{
					this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "Error getting the user's credential handle " + secStatus.ToString(), new object[0]);
					throw new AuthApiFailureException(new Win32Exception(secStatus).Message);
				}
				zero = IntPtr.Zero;
				secStatus = NativeMethods.QuerySecurityPackageInfo("Kerberos", ref zero);
				if (secStatus != 0)
				{
					this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "QuerySecurityPackageInfo failed " + secStatus.ToString(), new object[0]);
					throw new AuthApiFailureException(new Win32Exception(secStatus).Message);
				}
				NativeMethods.SecurityPackageInfo securityPackageInfo = new NativeMethods.SecurityPackageInfo(zero);
				this.maxToken = securityPackageInfo.cbMaxToken;
				if (zero != IntPtr.Zero)
				{
					NativeMethods.FreeContextBuffer(zero);
					zero = IntPtr.Zero;
				}
				this.ctxHandle = default(NativeMethods.CredHandle);
				this.ctxHandle.SetToInvalid();
				this.State = AuthenticationState.Initialized;
			}

			private const string AuthPackageName = "Kerberos";

			private SmtpSspiMechanism mechanism;

			private string authUserName;

			private string authPassword;

			private string authDomainName;

			private string authTargetName;

			private int authDirection;

			private int maxToken;

			private AuthenticationState state;

			private NativeMethods.CredHandle credHandle = default(NativeMethods.CredHandle);

			private NativeMethods.CredHandle ctxHandle = default(NativeMethods.CredHandle);

			private ISmtpClientDebugOutput smtpClientDebugOutput;
		}
	}
}
