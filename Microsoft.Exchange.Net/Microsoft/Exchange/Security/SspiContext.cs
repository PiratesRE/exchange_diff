using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security
{
	internal class SspiContext : DisposeTrackableBase
	{
		public SspiContext()
		{
			this.ServerTlsProtocols = SspiContext.DefaultServerTlsProtocols;
			this.ClientTlsProtocols = SspiContext.DefaultClientTlsProtocols;
		}

		public SchannelProtocols ServerTlsProtocols { get; set; }

		public static SchannelProtocols DefaultServerTlsProtocols
		{
			get
			{
				return SchannelProtocols.Zero;
			}
		}

		public SchannelProtocols ClientTlsProtocols { get; set; }

		public static SchannelProtocols DefaultClientTlsProtocols
		{
			get
			{
				return SchannelProtocols.Zero;
			}
		}

		public ContextState State
		{
			get
			{
				return this.state;
			}
		}

		public ContextFlags RequestedContextFlags
		{
			get
			{
				return this.requestedContextFlags;
			}
		}

		public ContextFlags ReturnedContextFlags
		{
			get
			{
				return this.returnedContextFlags;
			}
		}

		public int HeaderSize
		{
			get
			{
				return this.headerSize;
			}
		}

		public int MaxMessageSize
		{
			get
			{
				return this.maxMessageSize;
			}
		}

		public int MaxStreamSize
		{
			get
			{
				return this.headerSize + this.maxMessageSize + this.trailerSize;
			}
		}

		public int MaxTokenSize
		{
			get
			{
				return this.maxTokenSize;
			}
		}

		public int TrailerSize
		{
			get
			{
				return this.trailerSize;
			}
		}

		public CredentialUse CredentialUse
		{
			get
			{
				return this.credentialUse;
			}
		}

		public static IntPtr IntPtrAdd(IntPtr a, int b)
		{
			return (IntPtr)((long)a + (long)b);
		}

		public static int IntPtrDiff(IntPtr a, IntPtr b)
		{
			return (int)((long)a - (long)b);
		}

		public virtual SecurityStatus InitializeForInboundAuthentication(string packageName, ExtendedProtectionConfig config, ChannelBindingToken token)
		{
			if (this.credHandle != null)
			{
				throw new InvalidOperationException();
			}
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			this.requestedContextFlags = (ContextFlags.AcceptExtendedError | ContextFlags.AcceptAllowNonUserLogons);
			if (config.PolicySetting != ExtendedProtectionPolicySetting.None)
			{
				if (config.PolicySetting == ExtendedProtectionPolicySetting.Allow)
				{
					this.requestedContextFlags |= ContextFlags.AllowMissingBindings;
				}
				if (config.ExtendedProtectionTlsTerminatedAtProxyScenario)
				{
					this.requestedContextFlags |= ContextFlags.ProxyBindings;
					token = null;
				}
				this.extendedProtectionConfig = config;
			}
			else
			{
				token = null;
			}
			this.credentialUse = CredentialUse.Inbound;
			return this.InitializeAuthenticationCommon(packageName, null, AuthIdentity.Default, token);
		}

		public SecurityStatus InitializeForOutboundAuthentication(string packageName, string target, AuthIdentity authIdentity, bool requestMutualAuth, ChannelBindingToken token)
		{
			if (this.credHandle != null)
			{
				throw new InvalidOperationException();
			}
			if (requestMutualAuth)
			{
				this.requestedContextFlags = (ContextFlags.MutualAuth | ContextFlags.InitExtendedError);
			}
			else
			{
				this.requestedContextFlags = ContextFlags.InitExtendedError;
			}
			this.credentialUse = CredentialUse.Outbound;
			return this.InitializeAuthenticationCommon(packageName, target, authIdentity, token);
		}

		public SecurityStatus InitializeAuthenticationCommon(string packageName, string target, AuthIdentity authIdentity, ChannelBindingToken token)
		{
			this.targetName = target;
			this.channelBindingToken = token;
			SecurityStatus securityStatus = this.DetermineMaxToken(packageName);
			if (securityStatus != SecurityStatus.OK)
			{
				return securityStatus;
			}
			this.credHandle = new SafeCredentialsHandle();
			long num;
			securityStatus = SspiNativeMethods.AcquireCredentialsHandle(null, packageName, this.credentialUse, null, ref authIdentity, null, null, ref this.credHandle.SspiHandle, out num);
			if (securityStatus == SecurityStatus.OK)
			{
				this.state = ContextState.Initialized;
			}
			return securityStatus;
		}

		public unsafe SecurityStatus InitializeForTls(CredentialUse use, bool requestClientCertificate, X509Certificate cert, string target)
		{
			if (this.credHandle != null)
			{
				throw new InvalidOperationException();
			}
			this.isTls = true;
			SchannelProtocols schannelProtocols;
			SchannelCredential.Flags flags;
			switch (use)
			{
			case CredentialUse.Inbound:
				schannelProtocols = this.ServerTlsProtocols;
				this.requestedContextFlags = (ContextFlags.ReplayDetect | ContextFlags.SequenceDetect | ContextFlags.Confidentiality | ContextFlags.AcceptExtendedError | ContextFlags.AcceptStream);
				flags = SchannelCredential.Flags.Zero;
				if (requestClientCertificate)
				{
					this.requestedContextFlags |= ContextFlags.MutualAuth;
				}
				SspiContext.tlsInboundCredHandleCache.TryGetValue(Tuple.Create<SchannelProtocols, X509Certificate>(schannelProtocols, cert), out this.credHandle);
				break;
			case CredentialUse.Outbound:
				schannelProtocols = this.ClientTlsProtocols;
				this.requestedContextFlags = (ContextFlags.ReplayDetect | ContextFlags.SequenceDetect | ContextFlags.Confidentiality | ContextFlags.InitExtendedError | ContextFlags.AcceptExtendedError | ContextFlags.InitManualCredValidation | ContextFlags.InitUseSuppliedCreds);
				flags = (SchannelCredential.Flags.ValidateManual | SchannelCredential.Flags.NoDefaultCred);
				this.credHandle = TlsCredentialCache.Find(cert, target);
				break;
			default:
				throw new ArgumentOutOfRangeException("use");
			}
			this.credentialUse = use;
			this.certificate = cert;
			this.targetName = target;
			SecurityStatus securityStatus = this.DetermineMaxToken("Microsoft Unified Security Protocol Provider");
			if (securityStatus != SecurityStatus.OK)
			{
				return securityStatus;
			}
			if (this.credHandle == null)
			{
				SchannelCredential schannelCredential = new SchannelCredential(4, cert, flags, schannelProtocols);
				IntPtr certContextArray = schannelCredential.certContextArray;
				IntPtr certContextArray2 = new IntPtr((void*)(&certContextArray));
				if (certContextArray != IntPtr.Zero)
				{
					schannelCredential.certContextArray = certContextArray2;
				}
				this.credHandle = new SafeCredentialsHandle();
				long num;
				securityStatus = SspiNativeMethods.AcquireCredentialsHandle(null, "Microsoft Unified Security Protocol Provider", use, null, ref schannelCredential, null, null, ref this.credHandle.SspiHandle, out num);
			}
			if (securityStatus == SecurityStatus.OK)
			{
				this.state = ContextState.Initialized;
			}
			if (use == CredentialUse.Inbound && SspiContext.tlsInboundCredHandleCache != null)
			{
				SspiContext.tlsInboundCredHandleCache.AddOrUpdate(Tuple.Create<SchannelProtocols, X509Certificate>(schannelProtocols, cert), this.credHandle, (Tuple<SchannelProtocols, X509Certificate> key, SafeCredentialsHandle oldValue) => this.credHandle);
			}
			return securityStatus;
		}

		public SecurityStatus NegotiateSecurityContext(NetworkBuffer inputBuffer, NetworkBuffer outputBuffer)
		{
			outputBuffer.EmptyBuffer();
			int num;
			int num2;
			SecurityStatus result;
			if (inputBuffer != null)
			{
				result = this.NegotiateSecurityContext(inputBuffer.Buffer, inputBuffer.DataStartOffset, inputBuffer.Remaining, outputBuffer.Buffer, outputBuffer.BufferStartOffset, outputBuffer.Length, out num, out num2);
			}
			else
			{
				result = this.NegotiateSecurityContext(null, 0, 0, outputBuffer.Buffer, outputBuffer.BufferStartOffset, outputBuffer.Length, out num, out num2);
			}
			if (num != 0 && inputBuffer != null)
			{
				inputBuffer.ConsumeData(num);
			}
			if (num2 != 0)
			{
				outputBuffer.ReportBytesFilled(num2);
			}
			return result;
		}

		public SecurityStatus NegotiateSecurityContext(byte[] inputBuffer, int inputOffset, int inputLength, byte[] outputBuffer, int outputOffset, int outputLength, out int inputConsumed, out int outputFilled)
		{
			if (this.state == ContextState.Initialized)
			{
				this.state = ContextState.Negotiating;
			}
			else if (this.state != ContextState.Negotiating)
			{
				throw new InvalidOperationException();
			}
			if (this.contextHandle == null)
			{
				this.contextHandle = new SafeContextHandle();
			}
			SecurityStatus securityStatus = this.NegotiateSecurityContextInternal(inputBuffer, inputOffset, inputLength, outputBuffer, outputOffset, outputLength, out inputConsumed, out outputFilled);
			if (securityStatus == SecurityStatus.OK)
			{
				this.state = ContextState.NegotiationComplete;
				StreamSizes streamSizes;
				if (this.QueryStreamSizes(out streamSizes) == SecurityStatus.OK)
				{
					this.headerSize = streamSizes.Header;
					this.maxMessageSize = streamSizes.MaxMessage;
					this.trailerSize = streamSizes.Trailer;
				}
				SecSizes secSizes;
				if (this.QuerySizes(out secSizes) == SecurityStatus.OK)
				{
					this.maxTokenSize = secSizes.MaxToken;
					this.blockSize = secSizes.BlockSize;
					this.securityTrailerSize = secSizes.SecurityTrailer;
				}
			}
			return securityStatus;
		}

		protected unsafe virtual SecurityStatus NegotiateSecurityContextInternal(byte[] inputBuffer, int inputOffset, int inputLength, byte[] outputBuffer, int outputOffset, int outputLength, out int inputConsumed, out int outputFilled)
		{
			GCHandle gchandle = default(GCHandle);
			GCHandle gchandle2 = default(GCHandle);
			GCHandle gchandle3 = default(GCHandle);
			int num = 0;
			if (inputBuffer != null)
			{
				num += 2;
			}
			if (this.channelBindingToken != null)
			{
				num++;
			}
			SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(num);
			SecurityBufferDescriptor securityBufferDescriptor2 = new SecurityBufferDescriptor(1);
			SecurityBuffer[] array = (num > 0) ? new SecurityBuffer[num] : null;
			SecurityBuffer[] array2 = new SecurityBuffer[1];
			inputConsumed = 0;
			outputFilled = 0;
			SecurityStatus securityStatus;
			try
			{
				num = 0;
				if (inputBuffer != null && array != null)
				{
					gchandle = GCHandle.Alloc(inputBuffer, GCHandleType.Pinned);
					array[num].count = inputLength;
					array[num].type = BufferType.Token;
					array[num].token = Marshal.UnsafeAddrOfPinnedArrayElement(inputBuffer, inputOffset);
					num++;
					array[num].count = 0;
					array[num].type = BufferType.Empty;
					array[num].token = IntPtr.Zero;
					num++;
				}
				if (this.channelBindingToken != null && array != null)
				{
					gchandle2 = GCHandle.Alloc(this.channelBindingToken.Buffer, GCHandleType.Pinned);
					array[num].count = this.channelBindingToken.Buffer.Length;
					array[num].type = BufferType.ChannelBindings;
					array[num].token = Marshal.UnsafeAddrOfPinnedArrayElement(this.channelBindingToken.Buffer, 0);
				}
				gchandle3 = GCHandle.Alloc(outputBuffer, GCHandleType.Pinned);
				array2[0].count = outputLength;
				array2[0].type = BufferType.Token;
				array2[0].token = Marshal.UnsafeAddrOfPinnedArrayElement(outputBuffer, outputOffset);
				try
				{
					fixed (IntPtr* ptr = array)
					{
						securityBufferDescriptor.UnmanagedPointer = (void*)ptr;
						try
						{
							fixed (IntPtr* ptr2 = array2)
							{
								securityBufferDescriptor2.UnmanagedPointer = (void*)ptr2;
								SspiHandle sspiHandle = this.contextHandle.SspiHandle;
								if (this.credentialUse == CredentialUse.Outbound)
								{
									long num2;
									securityStatus = SspiNativeMethods.InitializeSecurityContext(ref this.credHandle.SspiHandle, sspiHandle.IsZero ? null : ((void*)(&sspiHandle)), this.targetName, this.requestedContextFlags, 0, Endianness.Network, securityBufferDescriptor, 0, ref this.contextHandle.SspiHandle, securityBufferDescriptor2, ref this.returnedContextFlags, out num2);
								}
								else
								{
									long num2;
									securityStatus = SspiNativeMethods.AcceptSecurityContext(ref this.credHandle.SspiHandle, sspiHandle.IsZero ? null : ((void*)(&sspiHandle)), securityBufferDescriptor, this.requestedContextFlags, Endianness.Network, ref this.contextHandle.SspiHandle, securityBufferDescriptor2, ref this.returnedContextFlags, out num2);
								}
								outputFilled = ((!SspiContext.IsSecurityStatusFailure(securityStatus)) ? array2[0].count : 0);
								if (inputBuffer != null && securityStatus != SecurityStatus.IncompleteMessage && array != null)
								{
									inputConsumed = array[0].count - array[1].count;
								}
							}
						}
						finally
						{
							IntPtr* ptr2 = null;
						}
					}
				}
				finally
				{
					IntPtr* ptr = null;
				}
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
				if (gchandle2.IsAllocated)
				{
					gchandle2.Free();
				}
				if (gchandle3.IsAllocated)
				{
					gchandle3.Free();
				}
			}
			return securityStatus;
		}

		public unsafe SecurityStatus EncryptMessage(byte[] buffer, int offset, int size, NetworkBuffer outBuffer)
		{
			if (this.state != ContextState.NegotiationComplete)
			{
				throw new InvalidOperationException();
			}
			if (size < 0 || size > this.maxMessageSize)
			{
				throw new ArgumentException("size");
			}
			GCHandle gchandle = default(GCHandle);
			SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(4);
			SecurityBuffer[] array = new SecurityBuffer[4];
			outBuffer.EmptyBuffer();
			Buffer.BlockCopy(buffer, offset, outBuffer.Buffer, outBuffer.BufferStartOffset + this.headerSize, size);
			SecurityStatus result;
			try
			{
				gchandle = GCHandle.Alloc(outBuffer.Buffer, GCHandleType.Pinned);
				array[0].count = this.headerSize;
				array[0].type = BufferType.Header;
				array[0].token = Marshal.UnsafeAddrOfPinnedArrayElement(outBuffer.Buffer, outBuffer.BufferStartOffset);
				array[1].count = size;
				array[1].type = BufferType.Data;
				array[1].token = SspiContext.IntPtrAdd(array[0].token, array[0].count);
				array[2].count = this.trailerSize;
				array[2].type = BufferType.Trailer;
				array[2].token = SspiContext.IntPtrAdd(array[1].token, array[1].count);
				array[3].count = 0;
				array[3].type = BufferType.Empty;
				array[3].token = IntPtr.Zero;
				try
				{
					fixed (IntPtr* ptr = array)
					{
						securityBufferDescriptor.UnmanagedPointer = (void*)ptr;
						result = SspiNativeMethods.EncryptMessage(ref this.contextHandle.SspiHandle, QualityOfProtection.None, securityBufferDescriptor, 0U);
					}
				}
				finally
				{
					IntPtr* ptr = null;
				}
				outBuffer.Filled = array[0].count + array[1].count + array[2].count;
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
			return result;
		}

		public unsafe SecurityStatus EncryptMessage(NetworkBuffer outBuffer)
		{
			if (this.state != ContextState.NegotiationComplete)
			{
				throw new InvalidOperationException();
			}
			if (this.headerSize > outBuffer.Consumed)
			{
				throw new InvalidOperationException();
			}
			if (this.trailerSize > outBuffer.Unused)
			{
				throw new InvalidOperationException();
			}
			GCHandle gchandle = default(GCHandle);
			SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(4);
			SecurityBuffer[] array = new SecurityBuffer[4];
			SecurityStatus result;
			try
			{
				gchandle = GCHandle.Alloc(outBuffer.Buffer, GCHandleType.Pinned);
				array[0].count = this.headerSize;
				array[0].type = BufferType.Header;
				array[0].token = Marshal.UnsafeAddrOfPinnedArrayElement(outBuffer.Buffer, outBuffer.DataStartOffset - this.headerSize);
				array[1].count = outBuffer.Remaining;
				array[1].type = BufferType.Data;
				array[1].token = SspiContext.IntPtrAdd(array[0].token, array[0].count);
				array[2].count = this.trailerSize;
				array[2].type = BufferType.Trailer;
				array[2].token = SspiContext.IntPtrAdd(array[1].token, array[1].count);
				array[3].count = 0;
				array[3].type = BufferType.Empty;
				array[3].token = IntPtr.Zero;
				try
				{
					fixed (IntPtr* ptr = array)
					{
						securityBufferDescriptor.UnmanagedPointer = (void*)ptr;
						result = SspiNativeMethods.EncryptMessage(ref this.contextHandle.SspiHandle, QualityOfProtection.None, securityBufferDescriptor, 0U);
					}
				}
				finally
				{
					IntPtr* ptr = null;
				}
				outBuffer.PutBackUnconsumedData(this.headerSize);
				outBuffer.Filled = outBuffer.Consumed + array[0].count + array[1].count + array[2].count;
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
			return result;
		}

		public unsafe SecurityStatus DecryptMessage(NetworkBuffer outBuffer)
		{
			if (this.state != ContextState.NegotiationComplete)
			{
				throw new InvalidOperationException();
			}
			GCHandle gchandle = default(GCHandle);
			SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(4);
			SecurityBuffer[] array = new SecurityBuffer[4];
			bool flag = false;
			SecurityStatus result;
			try
			{
				gchandle = GCHandle.Alloc(outBuffer.Buffer, GCHandleType.Pinned);
				IntPtr intPtr = Marshal.UnsafeAddrOfPinnedArrayElement(outBuffer.Buffer, outBuffer.BufferStartOffset);
				SecurityStatus securityStatus;
				for (;;)
				{
					array[0].count = outBuffer.EncryptedDataLength;
					array[0].type = BufferType.Data;
					array[0].token = SspiContext.IntPtrAdd(intPtr, outBuffer.EncryptedDataOffset);
					try
					{
						fixed (IntPtr* ptr = array)
						{
							securityBufferDescriptor.UnmanagedPointer = (void*)ptr;
							QualityOfProtection qualityOfProtection;
							securityStatus = SspiNativeMethods.DecryptMessage(ref this.contextHandle.SspiHandle, securityBufferDescriptor, 0U, out qualityOfProtection);
						}
					}
					finally
					{
						IntPtr* ptr = null;
					}
					if (securityStatus == SecurityStatus.IncompleteMessage && flag)
					{
						break;
					}
					if (securityStatus != SecurityStatus.OK)
					{
						goto Block_7;
					}
					flag = true;
					outBuffer.EncryptedDataLength = 0;
					outBuffer.EncryptedDataOffset = 0;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].type == BufferType.Data)
						{
							int count = array[i].count;
							if (count != 0)
							{
								int num = SspiContext.IntPtrDiff(array[i].token, intPtr);
								Buffer.BlockCopy(outBuffer.Buffer, outBuffer.BufferStartOffset + num, outBuffer.Buffer, outBuffer.BufferStartOffset + outBuffer.Filled, count);
								outBuffer.Filled += count;
							}
						}
						else if (array[i].type == BufferType.Extra)
						{
							outBuffer.EncryptedDataLength = array[i].count;
							outBuffer.EncryptedDataOffset = SspiContext.IntPtrDiff(array[i].token, intPtr);
						}
					}
					if (outBuffer.EncryptedDataLength == 0)
					{
						goto Block_12;
					}
					for (int j = 1; j < array.Length; j++)
					{
						array[j].count = 0;
						array[j].type = BufferType.Empty;
						array[j].token = IntPtr.Zero;
					}
				}
				return SecurityStatus.OK;
				Block_7:
				return securityStatus;
				Block_12:
				result = SecurityStatus.OK;
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
			return result;
		}

		public SecurityStatus WrapMessage(byte[] inputBuffer, bool encryptMessage, out byte[] outputBuffer)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			return this.WrapMessage(inputBuffer, 0, inputBuffer.Length, encryptMessage, out outputBuffer);
		}

		public unsafe SecurityStatus WrapMessage(byte[] buffer, int offset, int size, bool encryptMessage, out byte[] outputBuffer)
		{
			if (this.state != ContextState.NegotiationComplete)
			{
				throw new InvalidOperationException();
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (size < 0 || size > this.maxTokenSize)
			{
				throw new ArgumentException("size");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentException("offset");
			}
			SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(3);
			SecurityBuffer[] array = new SecurityBuffer[3];
			outputBuffer = null;
			int cb = this.securityTrailerSize + size + this.blockSize;
			SafeHGlobalHandle safeHGlobalHandle = new SafeHGlobalHandle(Marshal.AllocHGlobal(cb));
			array[0].count = this.securityTrailerSize;
			array[0].type = BufferType.Token;
			array[0].token = safeHGlobalHandle.DangerousGetHandle();
			array[1].count = size;
			array[1].type = BufferType.Data;
			array[1].token = SspiContext.IntPtrAdd(array[0].token, array[0].count);
			Marshal.Copy(buffer, offset, array[1].token, size);
			array[2].count = this.blockSize;
			array[2].type = BufferType.Padding;
			array[2].token = SspiContext.IntPtrAdd(array[1].token, array[1].count);
			fixed (IntPtr* ptr = array)
			{
				securityBufferDescriptor.UnmanagedPointer = (void*)ptr;
				SecurityStatus securityStatus = this.SspiEncryptMessage(ref this.contextHandle.SspiHandle, encryptMessage ? QualityOfProtection.None : QualityOfProtection.WrapNoEncrypt, securityBufferDescriptor, 0U);
				if (securityStatus != SecurityStatus.OK)
				{
					safeHGlobalHandle.Dispose();
					return securityStatus;
				}
			}
			int num = array[0].count + array[1].count + array[2].count;
			outputBuffer = new byte[num];
			Marshal.Copy(array[0].token, outputBuffer, 0, array[0].count);
			Marshal.Copy(array[1].token, outputBuffer, array[0].count, array[1].count);
			Marshal.Copy(array[2].token, outputBuffer, array[0].count + array[1].count, array[2].count);
			safeHGlobalHandle.Dispose();
			return SecurityStatus.OK;
		}

		protected virtual SecurityStatus SspiEncryptMessage(ref SspiHandle handlePtr, QualityOfProtection qualityOfProtection, SecurityBufferDescriptor inOut, uint sequenceNumber)
		{
			return SspiNativeMethods.EncryptMessage(ref handlePtr, qualityOfProtection, inOut, sequenceNumber);
		}

		public SecurityStatus UnwrapMessage(byte[] inputBuffer, out byte[] outputBuffer)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			return this.UnwrapMessage(inputBuffer, 0, inputBuffer.Length, out outputBuffer);
		}

		public unsafe SecurityStatus UnwrapMessage(byte[] inputBuffer, int offset, int size, out byte[] outputBuffer)
		{
			outputBuffer = null;
			if (this.state != ContextState.NegotiationComplete)
			{
				throw new InvalidOperationException();
			}
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (size < 0 || size > this.maxTokenSize)
			{
				throw new ArgumentException("size");
			}
			if (offset < 0 || offset > inputBuffer.Length)
			{
				throw new ArgumentException("offset");
			}
			SecurityBufferDescriptor securityBufferDescriptor = new SecurityBufferDescriptor(2);
			SecurityBuffer[] array = new SecurityBuffer[2];
			SecurityStatus securityStatus;
			using (SafeHGlobalHandle safeHGlobalHandle = new SafeHGlobalHandle(Marshal.AllocHGlobal(size)))
			{
				Marshal.Copy(inputBuffer, offset, safeHGlobalHandle.DangerousGetHandle(), size);
				array[0].count = size;
				array[0].type = BufferType.Stream;
				array[0].token = safeHGlobalHandle.DangerousGetHandle();
				array[1].count = 0;
				array[1].type = BufferType.Data;
				array[1].token = IntPtr.Zero;
				try
				{
					fixed (IntPtr* ptr = array)
					{
						securityBufferDescriptor.UnmanagedPointer = (void*)ptr;
						QualityOfProtection qualityOfProtection;
						securityStatus = SspiNativeMethods.DecryptMessage(ref this.contextHandle.SspiHandle, securityBufferDescriptor, 0U, out qualityOfProtection);
						if (securityStatus == SecurityStatus.OK)
						{
							outputBuffer = new byte[array[1].count];
							Marshal.Copy(array[1].token, outputBuffer, 0, array[1].count);
						}
					}
				}
				finally
				{
					IntPtr* ptr = null;
				}
			}
			return securityStatus;
		}

		public SecurityStatus DetermineMaxToken(string packageName)
		{
			SafeContextBuffer safeContextBuffer;
			SecurityStatus securityStatus = SspiNativeMethods.QuerySecurityPackageInfo(packageName, out safeContextBuffer);
			if (securityStatus == SecurityStatus.OK)
			{
				SecurityPackageInfo securityPackageInfo = new SecurityPackageInfo(safeContextBuffer.DangerousGetHandle());
				this.maxTokenSize = securityPackageInfo.MaxToken;
			}
			safeContextBuffer.Close();
			return securityStatus;
		}

		public virtual SecurityStatus QuerySecurityContextToken(out SafeTokenHandle token)
		{
			return SspiNativeMethods.QuerySecurityContextToken(ref this.contextHandle.SspiHandle, out token);
		}

		public SecurityStatus QueryStreamSizes(out StreamSizes sizes)
		{
			object obj;
			SecurityStatus result = this.QueryContextAttributes(ContextAttribute.StreamSizes, out obj);
			sizes = (StreamSizes)obj;
			return result;
		}

		public SecurityStatus QueryEapKeyBlock(out EapKeyBlock block)
		{
			object obj;
			SecurityStatus result = this.QueryContextAttributes(ContextAttribute.EapKeyBlock, out obj);
			block = (EapKeyBlock)obj;
			return result;
		}

		public virtual SecurityStatus QuerySessionKey(out SessionKey block)
		{
			object obj;
			SecurityStatus result = this.QueryContextAttributes(ContextAttribute.SessionKey, out obj);
			block = (SessionKey)obj;
			return result;
		}

		public SecurityStatus QueryConnectionInfo(out ConnectionInfo connectionInfo)
		{
			object obj;
			SecurityStatus result = this.QueryContextAttributes(ContextAttribute.ConnectionInfo, out obj);
			connectionInfo = (ConnectionInfo)obj;
			return result;
		}

		public SecurityStatus QuerySizes(out SecSizes sizes)
		{
			object obj;
			SecurityStatus result = this.QueryContextAttributes(ContextAttribute.Sizes, out obj);
			sizes = (SecSizes)obj;
			return result;
		}

		public SecurityStatus QueryRemoteCertificate(out X509Certificate2 cert)
		{
			object obj;
			SecurityStatus result = this.QueryContextAttributes(ContextAttribute.RemoteCertificate, out obj);
			cert = (X509Certificate2)obj;
			return result;
		}

		public SecurityStatus QueryLocalCertificate(out X509Certificate2 cert)
		{
			object obj;
			SecurityStatus result = this.QueryContextAttributes(ContextAttribute.LocalCertificate, out obj);
			cert = (X509Certificate2)obj;
			return result;
		}

		public SecurityStatus QueryCredentialsNames(out string userName)
		{
			object obj;
			SecurityStatus result = this.QueryCredentialsAttributes(CredentialsAttribute.Names, out obj);
			userName = ((CredentialsNames)obj).UserName;
			return result;
		}

		public SecurityStatus QueryPackageInfo(out SecurityPackageInfo info)
		{
			object obj;
			SecurityStatus result = this.QueryContextAttributes(ContextAttribute.PackageInfo, out obj);
			info = (SecurityPackageInfo)obj;
			return result;
		}

		public SecurityStatus QueryContextAttributes(ContextAttribute attribute, out object returnedAttributes)
		{
			returnedAttributes = null;
			int size;
			if (attribute <= ContextAttribute.PackageInfo)
			{
				if (attribute == ContextAttribute.Sizes)
				{
					size = SecSizes.Size;
					goto IL_B7;
				}
				if (attribute == ContextAttribute.StreamSizes)
				{
					size = StreamSizes.Size;
					goto IL_B7;
				}
				switch (attribute)
				{
				case ContextAttribute.SessionKey:
					size = SessionKey.Size;
					goto IL_B7;
				case ContextAttribute.PackageInfo:
					size = IntPtr.Size;
					goto IL_B7;
				}
			}
			else
			{
				switch (attribute)
				{
				case ContextAttribute.UniqueBindings:
				case ContextAttribute.EndpointBindings:
					size = ChannelBindingToken.Size;
					goto IL_B7;
				case ContextAttribute.ClientSpecifiedTarget:
					size = IntPtr.Size;
					goto IL_B7;
				default:
					switch (attribute)
					{
					case ContextAttribute.RemoteCertificate:
					case ContextAttribute.LocalCertificate:
						size = IntPtr.Size;
						goto IL_B7;
					default:
						switch (attribute)
						{
						case ContextAttribute.ConnectionInfo:
							size = ConnectionInfo.Size;
							goto IL_B7;
						case ContextAttribute.EapKeyBlock:
							size = EapKeyBlock.Size;
							goto IL_B7;
						}
						break;
					}
					break;
				}
			}
			return SecurityStatus.Unsupported;
			IL_B7:
			byte[] array = new byte[size];
			SecurityStatus securityStatus = SspiNativeMethods.QueryContextAttributes(ref this.contextHandle.SspiHandle, attribute, array);
			if (securityStatus != SecurityStatus.OK)
			{
				return securityStatus;
			}
			if (attribute <= ContextAttribute.PackageInfo)
			{
				if (attribute == ContextAttribute.Sizes)
				{
					returnedAttributes = new SecSizes(array);
					return securityStatus;
				}
				if (attribute == ContextAttribute.StreamSizes)
				{
					returnedAttributes = new StreamSizes(array);
					return securityStatus;
				}
				switch (attribute)
				{
				case ContextAttribute.SessionKey:
					returnedAttributes = new SessionKey(array);
					return securityStatus;
				case ContextAttribute.PackageInfo:
				{
					SecurityPackageInfo securityPackageInfo = new SecurityPackageInfo(array);
					returnedAttributes = securityPackageInfo;
					return securityStatus;
				}
				}
			}
			else
			{
				switch (attribute)
				{
				case ContextAttribute.UniqueBindings:
				case ContextAttribute.EndpointBindings:
				{
					ChannelBindingToken channelBindingToken = new ChannelBindingToken(array);
					returnedAttributes = ((channelBindingToken.Buffer.Length > 0) ? channelBindingToken : null);
					return securityStatus;
				}
				case ContextAttribute.ClientSpecifiedTarget:
				{
					ClientSpecifiedTarget clientSpecifiedTarget = new ClientSpecifiedTarget(array);
					returnedAttributes = clientSpecifiedTarget.ToString();
					return securityStatus;
				}
				default:
					switch (attribute)
					{
					case ContextAttribute.RemoteCertificate:
					case ContextAttribute.LocalCertificate:
						using (SafeCertificateContext safeCertificateContext = new SafeCertificateContext(array))
						{
							returnedAttributes = new X509Certificate2(safeCertificateContext.DangerousGetHandle());
							return securityStatus;
						}
						break;
					default:
						switch (attribute)
						{
						case ContextAttribute.ConnectionInfo:
							break;
						case ContextAttribute.EapKeyBlock:
							returnedAttributes = new EapKeyBlock(array);
							return securityStatus;
						default:
							return SecurityStatus.Unsupported;
						}
						break;
					}
					returnedAttributes = new ConnectionInfo(array);
					return securityStatus;
				}
			}
			return SecurityStatus.Unsupported;
		}

		public SecurityStatus QueryCredentialsAttributes(CredentialsAttribute attribute, out object returnedAttributes)
		{
			returnedAttributes = null;
			if (attribute != CredentialsAttribute.Names)
			{
				return SecurityStatus.Unsupported;
			}
			int size = CredentialsNames.Size;
			byte[] array = new byte[size];
			SecurityStatus securityStatus = SspiNativeMethods.QueryCredentialsAttributes(ref this.credHandle.SspiHandle, attribute, array);
			if (securityStatus != SecurityStatus.OK)
			{
				return securityStatus;
			}
			if (attribute == CredentialsAttribute.Names)
			{
				returnedAttributes = new CredentialsNames(array);
				return securityStatus;
			}
			return SecurityStatus.Unsupported;
		}

		public static bool IsSecurityStatusFailure(SecurityStatus status)
		{
			return (status & SecurityStatus.ErrorMask) == SecurityStatus.ErrorMask;
		}

		public SecurityStatus VerifyServiceBindings()
		{
			if (this.extendedProtectionConfig.PolicySetting == ExtendedProtectionPolicySetting.None)
			{
				return SecurityStatus.OK;
			}
			if (this.channelBindingToken != null && !this.extendedProtectionConfig.ExtendedProtectionTlsTerminatedAtProxyScenario)
			{
				return SecurityStatus.OK;
			}
			SecurityPackageInfo securityPackageInfo;
			if (this.QueryPackageInfo(out securityPackageInfo) == SecurityStatus.OK && securityPackageInfo.Name.Equals("Kerberos"))
			{
				return SecurityStatus.OK;
			}
			string text;
			SecurityStatus securityStatus = this.QueryClientSpecifiedTarget(out text);
			if (securityStatus == SecurityStatus.Unsupported)
			{
				return SecurityStatus.ExtendedProtectionOSNotPatched;
			}
			if (securityStatus == SecurityStatus.TargetUnknown)
			{
				if (this.extendedProtectionConfig.PolicySetting == ExtendedProtectionPolicySetting.Allow)
				{
					return SecurityStatus.OK;
				}
				return SecurityStatus.ExtendedProtectionMissingSpn;
			}
			else
			{
				if (securityStatus != SecurityStatus.OK)
				{
					return securityStatus;
				}
				if (this.extendedProtectionConfig.IsValidTargetName(text))
				{
					return SecurityStatus.OK;
				}
				return SecurityStatus.ExtendedProtectionWrongSpn;
			}
		}

		public SecurityStatus CaptureChannelBindingToken(ChannelBindingType tokenType, out ChannelBindingToken token)
		{
			ContextAttribute attribute;
			switch (tokenType)
			{
			case ChannelBindingType.Unique:
				attribute = ContextAttribute.UniqueBindings;
				break;
			case ChannelBindingType.Endpoint:
				attribute = ContextAttribute.EndpointBindings;
				break;
			default:
				throw new ArgumentException("tokenType");
			}
			object obj;
			SecurityStatus result = this.QueryContextAttributes(attribute, out obj);
			token = (obj as ChannelBindingToken);
			return result;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SspiContext>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.contextHandle != null && !this.contextHandle.IsClosed)
			{
				this.contextHandle.Close();
				this.contextHandle = null;
			}
			if (this.credHandle != null && !this.credHandle.IsClosed)
			{
				if (this.isTls && this.credentialUse == CredentialUse.Outbound && this.state == ContextState.NegotiationComplete)
				{
					TlsCredentialCache.Add(this.certificate, this.targetName, this.credHandle);
				}
				else if (!this.isTls)
				{
					this.credHandle.Close();
				}
				this.credHandle = null;
			}
		}

		private SecurityStatus QueryClientSpecifiedTarget(out string target)
		{
			object obj;
			SecurityStatus result = this.QueryContextAttributes(ContextAttribute.ClientSpecifiedTarget, out obj);
			target = (obj as string);
			return result;
		}

		private static readonly ConcurrentDictionary<Tuple<SchannelProtocols, X509Certificate>, SafeCredentialsHandle> tlsInboundCredHandleCache = new ConcurrentDictionary<Tuple<SchannelProtocols, X509Certificate>, SafeCredentialsHandle>();

		private SafeCredentialsHandle credHandle;

		private SafeContextHandle contextHandle;

		private CredentialUse credentialUse;

		protected ContextState state;

		private ContextFlags requestedContextFlags;

		private ContextFlags returnedContextFlags;

		private string targetName;

		private ChannelBindingToken channelBindingToken;

		private ExtendedProtectionConfig extendedProtectionConfig = ExtendedProtectionConfig.NoExtendedProtection;

		private X509Certificate certificate;

		private int headerSize;

		private int maxMessageSize;

		private int maxTokenSize;

		private int trailerSize;

		private int securityTrailerSize;

		private int blockSize;

		private bool isTls;
	}
}
