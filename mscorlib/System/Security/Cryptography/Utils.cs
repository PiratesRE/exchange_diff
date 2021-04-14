using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using Microsoft.Win32;

namespace System.Security.Cryptography
{
	internal static class Utils
	{
		private static object InternalSyncObject
		{
			get
			{
				return Utils.s_InternalSyncObject;
			}
		}

		internal static SafeProvHandle StaticProvHandle
		{
			[SecurityCritical]
			get
			{
				if (Utils._safeProvHandle == null)
				{
					object internalSyncObject = Utils.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (Utils._safeProvHandle == null)
						{
							Utils._safeProvHandle = Utils.AcquireProvHandle(new CspParameters(24));
						}
					}
				}
				return Utils._safeProvHandle;
			}
		}

		internal static SafeProvHandle StaticDssProvHandle
		{
			[SecurityCritical]
			get
			{
				if (Utils._safeDssProvHandle == null)
				{
					object internalSyncObject = Utils.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (Utils._safeDssProvHandle == null)
						{
							Utils._safeDssProvHandle = Utils.CreateProvHandle(new CspParameters(13), true);
						}
					}
				}
				return Utils._safeDssProvHandle;
			}
		}

		[SecurityCritical]
		internal static SafeProvHandle AcquireProvHandle(CspParameters parameters)
		{
			if (parameters == null)
			{
				parameters = new CspParameters(24);
			}
			SafeProvHandle invalidHandle = SafeProvHandle.InvalidHandle;
			Utils._AcquireCSP(parameters, ref invalidHandle);
			return invalidHandle;
		}

		[SecurityCritical]
		internal static SafeProvHandle CreateProvHandle(CspParameters parameters, bool randomKeyContainer)
		{
			SafeProvHandle invalidHandle = SafeProvHandle.InvalidHandle;
			int num = Utils._OpenCSP(parameters, 0U, ref invalidHandle);
			KeyContainerPermission keyContainerPermission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
			if (num != 0)
			{
				if ((parameters.Flags & CspProviderFlags.UseExistingKey) != CspProviderFlags.NoFlags || (num != -2146893799 && num != -2146893802 && num != -2147024894))
				{
					throw new CryptographicException(num);
				}
				if (!randomKeyContainer && !CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
				{
					KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.Create);
					keyContainerPermission.AccessEntries.Add(accessEntry);
					keyContainerPermission.Demand();
				}
				Utils._CreateCSP(parameters, randomKeyContainer, ref invalidHandle);
			}
			else if (!randomKeyContainer && !CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
			{
				KeyContainerPermissionAccessEntry accessEntry2 = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.Open);
				keyContainerPermission.AccessEntries.Add(accessEntry2);
				keyContainerPermission.Demand();
			}
			return invalidHandle;
		}

		[SecurityCritical]
		internal static CryptoKeySecurity GetKeySetSecurityInfo(SafeProvHandle hProv, AccessControlSections accessControlSections)
		{
			SecurityInfos securityInfos = (SecurityInfos)0;
			Privilege privilege = null;
			if ((accessControlSections & AccessControlSections.Owner) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.Owner;
			}
			if ((accessControlSections & AccessControlSections.Group) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.Group;
			}
			if ((accessControlSections & AccessControlSections.Access) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.DiscretionaryAcl;
			}
			byte[] array = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			int num;
			try
			{
				if ((accessControlSections & AccessControlSections.Audit) != AccessControlSections.None)
				{
					securityInfos |= SecurityInfos.SystemAcl;
					privilege = new Privilege("SeSecurityPrivilege");
					privilege.Enable();
				}
				array = Utils._GetKeySetSecurityInfo(hProv, securityInfos, out num);
			}
			finally
			{
				if (privilege != null)
				{
					privilege.Revert();
				}
			}
			if (num == 0 && (array == null || array.Length == 0))
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NoSecurityDescriptor"));
			}
			if (num == 8)
			{
				throw new OutOfMemoryException();
			}
			if (num == 5)
			{
				throw new UnauthorizedAccessException();
			}
			if (num == 1314)
			{
				throw new PrivilegeNotHeldException("SeSecurityPrivilege");
			}
			if (num != 0)
			{
				throw new CryptographicException(num);
			}
			CommonSecurityDescriptor securityDescriptor = new CommonSecurityDescriptor(false, false, new RawSecurityDescriptor(array, 0), true);
			return new CryptoKeySecurity(securityDescriptor);
		}

		[SecurityCritical]
		internal static void SetKeySetSecurityInfo(SafeProvHandle hProv, CryptoKeySecurity cryptoKeySecurity, AccessControlSections accessControlSections)
		{
			SecurityInfos securityInfos = (SecurityInfos)0;
			Privilege privilege = null;
			if ((accessControlSections & AccessControlSections.Owner) != AccessControlSections.None && cryptoKeySecurity._securityDescriptor.Owner != null)
			{
				securityInfos |= SecurityInfos.Owner;
			}
			if ((accessControlSections & AccessControlSections.Group) != AccessControlSections.None && cryptoKeySecurity._securityDescriptor.Group != null)
			{
				securityInfos |= SecurityInfos.Group;
			}
			if ((accessControlSections & AccessControlSections.Audit) != AccessControlSections.None)
			{
				securityInfos |= SecurityInfos.SystemAcl;
			}
			if ((accessControlSections & AccessControlSections.Access) != AccessControlSections.None && cryptoKeySecurity._securityDescriptor.IsDiscretionaryAclPresent)
			{
				securityInfos |= SecurityInfos.DiscretionaryAcl;
			}
			if (securityInfos == (SecurityInfos)0)
			{
				return;
			}
			int num = 0;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				if ((securityInfos & SecurityInfos.SystemAcl) != (SecurityInfos)0)
				{
					privilege = new Privilege("SeSecurityPrivilege");
					privilege.Enable();
				}
				byte[] securityDescriptorBinaryForm = cryptoKeySecurity.GetSecurityDescriptorBinaryForm();
				if (securityDescriptorBinaryForm != null && securityDescriptorBinaryForm.Length != 0)
				{
					num = Utils.SetKeySetSecurityInfo(hProv, securityInfos, securityDescriptorBinaryForm);
				}
			}
			finally
			{
				if (privilege != null)
				{
					privilege.Revert();
				}
			}
			if (num == 5 || num == 1307 || num == 1308)
			{
				throw new UnauthorizedAccessException();
			}
			if (num == 1314)
			{
				throw new PrivilegeNotHeldException("SeSecurityPrivilege");
			}
			if (num == 6)
			{
				throw new NotSupportedException(Environment.GetResourceString("AccessControl_InvalidHandle"));
			}
			if (num != 0)
			{
				throw new CryptographicException(num);
			}
		}

		[SecurityCritical]
		internal static byte[] ExportCspBlobHelper(bool includePrivateParameters, CspParameters parameters, SafeKeyHandle safeKeyHandle)
		{
			if (includePrivateParameters && !CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
			{
				KeyContainerPermission keyContainerPermission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
				KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.Export);
				keyContainerPermission.AccessEntries.Add(accessEntry);
				keyContainerPermission.Demand();
			}
			byte[] result = null;
			Utils.ExportCspBlob(safeKeyHandle, includePrivateParameters ? 7 : 6, JitHelpers.GetObjectHandleOnStack<byte[]>(ref result));
			return result;
		}

		[SecuritySafeCritical]
		internal unsafe static void GetKeyPairHelper(CspAlgorithmType keyType, CspParameters parameters, bool randomKeyContainer, int dwKeySize, ref SafeProvHandle safeProvHandle, ref SafeKeyHandle safeKeyHandle)
		{
			SafeProvHandle safeProvHandle2 = Utils.CreateProvHandle(parameters, randomKeyContainer);
			if (parameters.CryptoKeySecurity != null)
			{
				KeyContainerPermission keyContainerPermission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
				KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.ChangeAcl);
				keyContainerPermission.AccessEntries.Add(accessEntry);
				keyContainerPermission.Demand();
				Utils.SetKeySetSecurityInfo(safeProvHandle2, parameters.CryptoKeySecurity, parameters.CryptoKeySecurity.ChangedAccessControlSections);
			}
			if (parameters.ParentWindowHandle != IntPtr.Zero)
			{
				IntPtr parentWindowHandle = parameters.ParentWindowHandle;
				IntPtr pbData = parentWindowHandle;
				if (!AppContextSwitches.DoNotAddrOfCspParentWindowHandle)
				{
					pbData = new IntPtr((void*)(&parentWindowHandle));
				}
				Utils.SetProviderParameter(safeProvHandle2, parameters.KeyNumber, 10U, pbData);
			}
			else if (parameters.KeyPassword != null)
			{
				IntPtr intPtr = Marshal.SecureStringToCoTaskMemAnsi(parameters.KeyPassword);
				try
				{
					Utils.SetProviderParameter(safeProvHandle2, parameters.KeyNumber, 11U, intPtr);
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						Marshal.ZeroFreeCoTaskMemAnsi(intPtr);
					}
				}
			}
			safeProvHandle = safeProvHandle2;
			SafeKeyHandle invalidHandle = SafeKeyHandle.InvalidHandle;
			int num = Utils._GetUserKey(safeProvHandle, parameters.KeyNumber, ref invalidHandle);
			if (num != 0)
			{
				if ((parameters.Flags & CspProviderFlags.UseExistingKey) != CspProviderFlags.NoFlags || num != -2146893811)
				{
					throw new CryptographicException(num);
				}
				Utils._GenerateKey(safeProvHandle, parameters.KeyNumber, parameters.Flags, dwKeySize, ref invalidHandle);
			}
			byte[] array = Utils._GetKeyParameter(invalidHandle, 9U);
			int num2 = (int)array[0] | (int)array[1] << 8 | (int)array[2] << 16 | (int)array[3] << 24;
			if ((keyType == CspAlgorithmType.Rsa && num2 != 41984 && num2 != 9216) || (keyType == CspAlgorithmType.Dss && num2 != 8704))
			{
				invalidHandle.Dispose();
				throw new CryptographicException(Environment.GetResourceString("Cryptography_CSP_WrongKeySpec"));
			}
			safeKeyHandle = invalidHandle;
		}

		[SecurityCritical]
		internal static void ImportCspBlobHelper(CspAlgorithmType keyType, byte[] keyBlob, bool publicOnly, ref CspParameters parameters, bool randomKeyContainer, ref SafeProvHandle safeProvHandle, ref SafeKeyHandle safeKeyHandle)
		{
			if (safeKeyHandle != null && !safeKeyHandle.IsClosed)
			{
				safeKeyHandle.Dispose();
			}
			safeKeyHandle = SafeKeyHandle.InvalidHandle;
			if (publicOnly)
			{
				parameters.KeyNumber = Utils._ImportCspBlob(keyBlob, (keyType == CspAlgorithmType.Dss) ? Utils.StaticDssProvHandle : Utils.StaticProvHandle, CspProviderFlags.NoFlags, ref safeKeyHandle);
				return;
			}
			if (!CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
			{
				KeyContainerPermission keyContainerPermission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
				KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.Import);
				keyContainerPermission.AccessEntries.Add(accessEntry);
				keyContainerPermission.Demand();
			}
			if (safeProvHandle == null)
			{
				safeProvHandle = Utils.CreateProvHandle(parameters, randomKeyContainer);
			}
			parameters.KeyNumber = Utils._ImportCspBlob(keyBlob, safeProvHandle, parameters.Flags, ref safeKeyHandle);
		}

		[SecurityCritical]
		internal static CspParameters SaveCspParameters(CspAlgorithmType keyType, CspParameters userParameters, CspProviderFlags defaultFlags, ref bool randomKeyContainer)
		{
			CspParameters cspParameters;
			if (userParameters == null)
			{
				cspParameters = new CspParameters((keyType == CspAlgorithmType.Dss) ? 13 : 24, null, null, defaultFlags);
			}
			else
			{
				Utils.ValidateCspFlags(userParameters.Flags);
				cspParameters = new CspParameters(userParameters);
			}
			if (cspParameters.KeyNumber == -1)
			{
				cspParameters.KeyNumber = ((keyType == CspAlgorithmType.Dss) ? 2 : 1);
			}
			else if (cspParameters.KeyNumber == 8704 || cspParameters.KeyNumber == 9216)
			{
				cspParameters.KeyNumber = 2;
			}
			else if (cspParameters.KeyNumber == 41984)
			{
				cspParameters.KeyNumber = 1;
			}
			randomKeyContainer = ((cspParameters.Flags & CspProviderFlags.CreateEphemeralKey) == CspProviderFlags.CreateEphemeralKey);
			if (cspParameters.KeyContainerName == null && (cspParameters.Flags & CspProviderFlags.UseDefaultKeyContainer) == CspProviderFlags.NoFlags)
			{
				cspParameters.Flags |= CspProviderFlags.CreateEphemeralKey;
				randomKeyContainer = true;
			}
			return cspParameters;
		}

		[SecurityCritical]
		private static void ValidateCspFlags(CspProviderFlags flags)
		{
			if ((flags & CspProviderFlags.UseExistingKey) != CspProviderFlags.NoFlags)
			{
				CspProviderFlags cspProviderFlags = CspProviderFlags.UseNonExportableKey | CspProviderFlags.UseArchivableKey | CspProviderFlags.UseUserProtectedKey;
				if ((flags & cspProviderFlags) != CspProviderFlags.NoFlags)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"));
				}
			}
			if ((flags & CspProviderFlags.UseUserProtectedKey) != CspProviderFlags.NoFlags)
			{
				if (!Environment.UserInteractive)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Cryptography_NotInteractive"));
				}
				UIPermission uipermission = new UIPermission(UIPermissionWindow.SafeTopLevelWindows);
				uipermission.Demand();
			}
		}

		internal static RNGCryptoServiceProvider StaticRandomNumberGenerator
		{
			get
			{
				if (Utils._rng == null)
				{
					Utils._rng = new RNGCryptoServiceProvider();
				}
				return Utils._rng;
			}
		}

		internal static byte[] GenerateRandom(int keySize)
		{
			byte[] array = new byte[keySize];
			Utils.StaticRandomNumberGenerator.GetBytes(array);
			return array;
		}

		[SecurityCritical]
		[RegistryPermission(SecurityAction.Assert, Read = "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Control\\Lsa")]
		internal static bool ReadLegacyFipsPolicy()
		{
			bool result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Lsa", false))
				{
					if (registryKey == null)
					{
						result = false;
					}
					else
					{
						object value = registryKey.GetValue("FIPSAlgorithmPolicy");
						if (value == null)
						{
							result = false;
						}
						else if (registryKey.GetValueKind("FIPSAlgorithmPolicy") != RegistryValueKind.DWord)
						{
							result = true;
						}
						else
						{
							result = ((int)value != 0);
						}
					}
				}
			}
			catch (SecurityException)
			{
				result = true;
			}
			return result;
		}

		[SecurityCritical]
		internal static bool HasAlgorithm(int dwCalg, int dwKeySize)
		{
			bool result = false;
			object internalSyncObject = Utils.InternalSyncObject;
			lock (internalSyncObject)
			{
				result = Utils.SearchForAlgorithm(Utils.StaticProvHandle, dwCalg, dwKeySize);
			}
			return result;
		}

		internal static int ObjToAlgId(object hashAlg, OidGroup group)
		{
			if (hashAlg == null)
			{
				throw new ArgumentNullException("hashAlg");
			}
			string text = null;
			string text2 = hashAlg as string;
			if (text2 != null)
			{
				text = CryptoConfig.MapNameToOID(text2, group);
				if (text == null)
				{
					text = text2;
				}
			}
			else if (hashAlg is HashAlgorithm)
			{
				text = CryptoConfig.MapNameToOID(hashAlg.GetType().ToString(), group);
			}
			else if (hashAlg is Type)
			{
				text = CryptoConfig.MapNameToOID(hashAlg.ToString(), group);
			}
			if (text == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidValue"));
			}
			return X509Utils.GetAlgIdFromOid(text, group);
		}

		internal static HashAlgorithm ObjToHashAlgorithm(object hashAlg)
		{
			if (hashAlg == null)
			{
				throw new ArgumentNullException("hashAlg");
			}
			HashAlgorithm hashAlgorithm = null;
			if (hashAlg is string)
			{
				hashAlgorithm = (HashAlgorithm)CryptoConfig.CreateFromName((string)hashAlg);
				if (hashAlgorithm == null)
				{
					string friendlyNameFromOid = X509Utils.GetFriendlyNameFromOid((string)hashAlg, OidGroup.HashAlgorithm);
					if (friendlyNameFromOid != null)
					{
						hashAlgorithm = (HashAlgorithm)CryptoConfig.CreateFromName(friendlyNameFromOid);
					}
				}
			}
			else if (hashAlg is HashAlgorithm)
			{
				hashAlgorithm = (HashAlgorithm)hashAlg;
			}
			else if (hashAlg is Type)
			{
				hashAlgorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlg.ToString());
			}
			if (hashAlgorithm == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidValue"));
			}
			return hashAlgorithm;
		}

		internal static string DiscardWhiteSpaces(string inputBuffer)
		{
			return Utils.DiscardWhiteSpaces(inputBuffer, 0, inputBuffer.Length);
		}

		internal static string DiscardWhiteSpaces(string inputBuffer, int inputOffset, int inputCount)
		{
			int num = 0;
			for (int i = 0; i < inputCount; i++)
			{
				if (char.IsWhiteSpace(inputBuffer[inputOffset + i]))
				{
					num++;
				}
			}
			char[] array = new char[inputCount - num];
			num = 0;
			for (int i = 0; i < inputCount; i++)
			{
				if (!char.IsWhiteSpace(inputBuffer[inputOffset + i]))
				{
					array[num++] = inputBuffer[inputOffset + i];
				}
			}
			return new string(array);
		}

		internal static int ConvertByteArrayToInt(byte[] input)
		{
			int num = 0;
			for (int i = 0; i < input.Length; i++)
			{
				num *= 256;
				num += (int)input[i];
			}
			return num;
		}

		internal static byte[] ConvertIntToByteArray(int dwInput)
		{
			byte[] array = new byte[8];
			int num = 0;
			if (dwInput == 0)
			{
				return new byte[1];
			}
			int i = dwInput;
			while (i > 0)
			{
				int num2 = i % 256;
				array[num] = (byte)num2;
				i = (i - num2) / 256;
				num++;
			}
			byte[] array2 = new byte[num];
			for (int j = 0; j < num; j++)
			{
				array2[j] = array[num - j - 1];
			}
			return array2;
		}

		internal static void ConvertIntToByteArray(uint dwInput, ref byte[] counter)
		{
			uint num = dwInput;
			int num2 = 0;
			Array.Clear(counter, 0, counter.Length);
			if (dwInput == 0U)
			{
				return;
			}
			while (num > 0U)
			{
				uint num3 = num % 256U;
				counter[3 - num2] = (byte)num3;
				num = (num - num3) / 256U;
				num2++;
			}
		}

		internal static byte[] FixupKeyParity(byte[] key)
		{
			byte[] array = new byte[key.Length];
			for (int i = 0; i < key.Length; i++)
			{
				array[i] = (key[i] & 254);
				byte b = (byte)((int)(array[i] & 15) ^ array[i] >> 4);
				byte b2 = (byte)((int)(b & 3) ^ b >> 2);
				if ((byte)((int)(b2 & 1) ^ b2 >> 1) == 0)
				{
					byte[] array2 = array;
					int num = i;
					array2[num] |= 1;
				}
			}
			return array;
		}

		[SecurityCritical]
		internal unsafe static void DWORDFromLittleEndian(uint* x, int digits, byte* block)
		{
			int i = 0;
			int num = 0;
			while (i < digits)
			{
				x[i] = (uint)((int)block[num] | (int)block[num + 1] << 8 | (int)block[num + 2] << 16 | (int)block[num + 3] << 24);
				i++;
				num += 4;
			}
		}

		internal static void DWORDToLittleEndian(byte[] block, uint[] x, int digits)
		{
			int i = 0;
			int num = 0;
			while (i < digits)
			{
				block[num] = (byte)(x[i] & 255U);
				block[num + 1] = (byte)(x[i] >> 8 & 255U);
				block[num + 2] = (byte)(x[i] >> 16 & 255U);
				block[num + 3] = (byte)(x[i] >> 24 & 255U);
				i++;
				num += 4;
			}
		}

		[SecurityCritical]
		internal unsafe static void DWORDFromBigEndian(uint* x, int digits, byte* block)
		{
			int i = 0;
			int num = 0;
			while (i < digits)
			{
				x[i] = (uint)((int)block[num] << 24 | (int)block[num + 1] << 16 | (int)block[num + 2] << 8 | (int)block[num + 3]);
				i++;
				num += 4;
			}
		}

		internal static void DWORDToBigEndian(byte[] block, uint[] x, int digits)
		{
			int i = 0;
			int num = 0;
			while (i < digits)
			{
				block[num] = (byte)(x[i] >> 24 & 255U);
				block[num + 1] = (byte)(x[i] >> 16 & 255U);
				block[num + 2] = (byte)(x[i] >> 8 & 255U);
				block[num + 3] = (byte)(x[i] & 255U);
				i++;
				num += 4;
			}
		}

		[SecurityCritical]
		internal unsafe static void QuadWordFromBigEndian(ulong* x, int digits, byte* block)
		{
			int i = 0;
			int num = 0;
			while (i < digits)
			{
				x[i] = ((ulong)block[num] << 56 | (ulong)block[num + 1] << 48 | (ulong)block[num + 2] << 40 | (ulong)block[num + 3] << 32 | (ulong)block[num + 4] << 24 | (ulong)block[num + 5] << 16 | (ulong)block[num + 6] << 8 | (ulong)block[num + 7]);
				i++;
				num += 8;
			}
		}

		internal static void QuadWordToBigEndian(byte[] block, ulong[] x, int digits)
		{
			int i = 0;
			int num = 0;
			while (i < digits)
			{
				block[num] = (byte)(x[i] >> 56 & 255UL);
				block[num + 1] = (byte)(x[i] >> 48 & 255UL);
				block[num + 2] = (byte)(x[i] >> 40 & 255UL);
				block[num + 3] = (byte)(x[i] >> 32 & 255UL);
				block[num + 4] = (byte)(x[i] >> 24 & 255UL);
				block[num + 5] = (byte)(x[i] >> 16 & 255UL);
				block[num + 6] = (byte)(x[i] >> 8 & 255UL);
				block[num + 7] = (byte)(x[i] & 255UL);
				i++;
				num += 8;
			}
		}

		internal static byte[] Int(uint i)
		{
			return new byte[]
			{
				(byte)(i >> 24),
				(byte)(i >> 16),
				(byte)(i >> 8),
				(byte)i
			};
		}

		[SecurityCritical]
		internal static byte[] RsaOaepEncrypt(RSA rsa, HashAlgorithm hash, PKCS1MaskGenerationMethod mgf, RandomNumberGenerator rng, byte[] data)
		{
			int num = rsa.KeySize / 8;
			int num2 = hash.HashSize / 8;
			if (data.Length + 2 + 2 * num2 > num)
			{
				throw new CryptographicException(string.Format(null, Environment.GetResourceString("Cryptography_Padding_EncDataTooBig"), num - 2 - 2 * num2));
			}
			hash.ComputeHash(EmptyArray<byte>.Value);
			byte[] array = new byte[num - num2];
			Buffer.InternalBlockCopy(hash.Hash, 0, array, 0, num2);
			array[array.Length - data.Length - 1] = 1;
			Buffer.InternalBlockCopy(data, 0, array, array.Length - data.Length, data.Length);
			byte[] array2 = new byte[num2];
			rng.GetBytes(array2);
			byte[] array3 = mgf.GenerateMask(array2, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] ^= array3[i];
			}
			array3 = mgf.GenerateMask(array, num2);
			for (int j = 0; j < array2.Length; j++)
			{
				byte[] array4 = array2;
				int num3 = j;
				array4[num3] ^= array3[j];
			}
			byte[] array5 = new byte[num];
			Buffer.InternalBlockCopy(array2, 0, array5, 0, array2.Length);
			Buffer.InternalBlockCopy(array, 0, array5, array2.Length, array.Length);
			return rsa.EncryptValue(array5);
		}

		[SecurityCritical]
		internal static byte[] RsaOaepDecrypt(RSA rsa, HashAlgorithm hash, PKCS1MaskGenerationMethod mgf, byte[] encryptedData)
		{
			int num = rsa.KeySize / 8;
			byte[] array = null;
			try
			{
				array = rsa.DecryptValue(encryptedData);
			}
			catch (CryptographicException)
			{
				throw new CryptographicException(Environment.GetResourceString("Cryptography_OAEPDecoding"));
			}
			int num2 = hash.HashSize / 8;
			int num3 = num - array.Length;
			if (num3 < 0 || num3 >= num2)
			{
				throw new CryptographicException(Environment.GetResourceString("Cryptography_OAEPDecoding"));
			}
			byte[] array2 = new byte[num2];
			Buffer.InternalBlockCopy(array, 0, array2, num3, array2.Length - num3);
			byte[] array3 = new byte[array.Length - array2.Length + num3];
			Buffer.InternalBlockCopy(array, array2.Length - num3, array3, 0, array3.Length);
			byte[] array4 = mgf.GenerateMask(array3, array2.Length);
			int i;
			for (i = 0; i < array2.Length; i++)
			{
				byte[] array5 = array2;
				int num4 = i;
				array5[num4] ^= array4[i];
			}
			array4 = mgf.GenerateMask(array2, array3.Length);
			for (i = 0; i < array3.Length; i++)
			{
				array3[i] ^= array4[i];
			}
			hash.ComputeHash(EmptyArray<byte>.Value);
			byte[] hash2 = hash.Hash;
			for (i = 0; i < num2; i++)
			{
				if (array3[i] != hash2[i])
				{
					throw new CryptographicException(Environment.GetResourceString("Cryptography_OAEPDecoding"));
				}
			}
			while (i < array3.Length && array3[i] != 1)
			{
				if (array3[i] != 0)
				{
					throw new CryptographicException(Environment.GetResourceString("Cryptography_OAEPDecoding"));
				}
				i++;
			}
			if (i == array3.Length)
			{
				throw new CryptographicException(Environment.GetResourceString("Cryptography_OAEPDecoding"));
			}
			i++;
			byte[] array6 = new byte[array3.Length - i];
			Buffer.InternalBlockCopy(array3, i, array6, 0, array6.Length);
			return array6;
		}

		[SecurityCritical]
		internal static byte[] RsaPkcs1Padding(RSA rsa, byte[] oid, byte[] hash)
		{
			int num = rsa.KeySize / 8;
			byte[] array = new byte[num];
			byte[] array2 = new byte[oid.Length + 8 + hash.Length];
			array2[0] = 48;
			int num2 = array2.Length - 2;
			array2[1] = (byte)num2;
			array2[2] = 48;
			num2 = oid.Length + 2;
			array2[3] = (byte)num2;
			Buffer.InternalBlockCopy(oid, 0, array2, 4, oid.Length);
			array2[4 + oid.Length] = 5;
			array2[4 + oid.Length + 1] = 0;
			array2[4 + oid.Length + 2] = 4;
			array2[4 + oid.Length + 3] = (byte)hash.Length;
			Buffer.InternalBlockCopy(hash, 0, array2, oid.Length + 8, hash.Length);
			int num3 = num - array2.Length;
			if (num3 <= 2)
			{
				throw new CryptographicUnexpectedOperationException(Environment.GetResourceString("Cryptography_InvalidOID"));
			}
			array[0] = 0;
			array[1] = 1;
			for (int i = 2; i < num3 - 1; i++)
			{
				array[i] = byte.MaxValue;
			}
			array[num3 - 1] = 0;
			Buffer.InternalBlockCopy(array2, 0, array, num3, array2.Length);
			return array;
		}

		internal static bool CompareBigIntArrays(byte[] lhs, byte[] rhs)
		{
			if (lhs == null)
			{
				return rhs == null;
			}
			int i = 0;
			int num = 0;
			while (i < lhs.Length)
			{
				if (lhs[i] != 0)
				{
					break;
				}
				i++;
			}
			while (num < rhs.Length && rhs[num] == 0)
			{
				num++;
			}
			int num2 = lhs.Length - i;
			if (rhs.Length - num != num2)
			{
				return false;
			}
			for (int j = 0; j < num2; j++)
			{
				if (lhs[i + j] != rhs[num + j])
				{
					return false;
				}
			}
			return true;
		}

		internal static HashAlgorithmName OidToHashAlgorithmName(string oid)
		{
			if (oid == "1.3.14.3.2.26")
			{
				return HashAlgorithmName.SHA1;
			}
			if (oid == "2.16.840.1.101.3.4.2.1")
			{
				return HashAlgorithmName.SHA256;
			}
			if (oid == "2.16.840.1.101.3.4.2.2")
			{
				return HashAlgorithmName.SHA384;
			}
			if (!(oid == "2.16.840.1.101.3.4.2.3"))
			{
				throw new NotSupportedException();
			}
			return HashAlgorithmName.SHA512;
		}

		internal static bool DoesRsaKeyOverride(RSA rsaKey, string methodName, Type[] parameterTypes)
		{
			Type type = rsaKey.GetType();
			if (rsaKey is RSACryptoServiceProvider)
			{
				return true;
			}
			string fullName = type.FullName;
			return fullName == "System.Security.Cryptography.RSACng" || Utils.DoesRsaKeyOverrideSlowPath(type, methodName, parameterTypes);
		}

		private static bool DoesRsaKeyOverrideSlowPath(Type t, string methodName, Type[] parameterTypes)
		{
			MethodInfo method = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, null, parameterTypes, null);
			Type declaringType = method.DeclaringType;
			return !(declaringType == typeof(RSA));
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern SafeHashHandle CreateHash(SafeProvHandle hProv, int algid);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void EndHash(SafeHashHandle hHash, ObjectHandleOnStack retHash);

		[SecurityCritical]
		internal static byte[] EndHash(SafeHashHandle hHash)
		{
			byte[] result = null;
			Utils.EndHash(hHash, JitHelpers.GetObjectHandleOnStack<byte[]>(ref result));
			return result;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void ExportCspBlob(SafeKeyHandle hKey, int blobType, ObjectHandleOnStack retBlob);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern bool GetPersistKeyInCsp(SafeProvHandle hProv);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void HashData(SafeHashHandle hHash, byte[] data, int cbData, int ibStart, int cbSize);

		[SecurityCritical]
		internal static void HashData(SafeHashHandle hHash, byte[] data, int ibStart, int cbSize)
		{
			Utils.HashData(hHash, data, data.Length, ibStart, cbSize);
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern bool SearchForAlgorithm(SafeProvHandle hProv, int algID, int keyLength);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void SetKeyParamDw(SafeKeyHandle hKey, int param, int dwValue);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void SetKeyParamRgb(SafeKeyHandle hKey, int param, byte[] value, int cbValue);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern int SetKeySetSecurityInfo(SafeProvHandle hProv, SecurityInfos securityInfo, byte[] sd);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void SetPersistKeyInCsp(SafeProvHandle hProv, bool fPersistKeyInCsp);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void SetProviderParameter(SafeProvHandle hProv, int keyNumber, uint paramID, IntPtr pbData);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void SignValue(SafeKeyHandle hKey, int keyNumber, int calgKey, int calgHash, byte[] hash, int cbHash, ObjectHandleOnStack retSignature);

		[SecurityCritical]
		internal static byte[] SignValue(SafeKeyHandle hKey, int keyNumber, int calgKey, int calgHash, byte[] hash)
		{
			byte[] result = null;
			Utils.SignValue(hKey, keyNumber, calgKey, calgHash, hash, hash.Length, JitHelpers.GetObjectHandleOnStack<byte[]>(ref result));
			return result;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern bool VerifySign(SafeKeyHandle hKey, int calgKey, int calgHash, byte[] hash, int cbHash, byte[] signature, int cbSignature);

		[SecurityCritical]
		internal static bool VerifySign(SafeKeyHandle hKey, int calgKey, int calgHash, byte[] hash, byte[] signature)
		{
			return Utils.VerifySign(hKey, calgKey, calgHash, hash, hash.Length, signature, signature.Length);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void _CreateCSP(CspParameters param, bool randomKeyContainer, ref SafeProvHandle hProv);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int _DecryptData(SafeKeyHandle hKey, byte[] data, int ib, int cb, ref byte[] outputBuffer, int outputOffset, PaddingMode PaddingMode, bool fDone);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int _EncryptData(SafeKeyHandle hKey, byte[] data, int ib, int cb, ref byte[] outputBuffer, int outputOffset, PaddingMode PaddingMode, bool fDone);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void _ExportKey(SafeKeyHandle hKey, int blobType, object cspObject);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void _GenerateKey(SafeProvHandle hProv, int algid, CspProviderFlags flags, int keySize, ref SafeKeyHandle hKey);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool _GetEnforceFipsPolicySetting();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern byte[] _GetKeyParameter(SafeKeyHandle hKey, uint paramID);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern byte[] _GetKeySetSecurityInfo(SafeProvHandle hProv, SecurityInfos securityInfo, out int error);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object _GetProviderParameter(SafeProvHandle hProv, int keyNumber, uint paramID);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int _GetUserKey(SafeProvHandle hProv, int keyNumber, ref SafeKeyHandle hKey);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void _ImportBulkKey(SafeProvHandle hProv, int algid, bool useSalt, byte[] key, ref SafeKeyHandle hKey);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int _ImportCspBlob(byte[] keyBlob, SafeProvHandle hProv, CspProviderFlags flags, ref SafeKeyHandle hKey);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void _ImportKey(SafeProvHandle hCSP, int keyNumber, CspProviderFlags flags, object cspObject, ref SafeKeyHandle hKey);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool _ProduceLegacyHmacValues();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int _OpenCSP(CspParameters param, uint flags, ref SafeProvHandle hProv);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void _AcquireCSP(CspParameters param, ref SafeProvHandle hProv);

		internal const int DefaultRsaProviderType = 24;

		private static object s_InternalSyncObject = new object();

		[SecurityCritical]
		private static volatile SafeProvHandle _safeProvHandle;

		[SecurityCritical]
		private static volatile SafeProvHandle _safeDssProvHandle;

		private static volatile RNGCryptoServiceProvider _rng;
	}
}
