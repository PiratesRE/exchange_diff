using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics.Components.TextProcessing;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.TextProcessing.Boomerang
{
	public class BoomerangProvider
	{
		private BoomerangProvider()
		{
			this.randomNumberGenerator = new RNGCryptoServiceProvider();
			this.boomerangMasterKey = this.InitializeBoomerangMasterKey();
			this.validIntervalsConfig = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Boomerang.BoomerangSettings.NumberOfValidIntervalsInDays;
		}

		~BoomerangProvider()
		{
			this.DisposeResources();
		}

		public static BoomerangProvider Instance
		{
			get
			{
				return BoomerangProvider.instance;
			}
		}

		public string GenerateBoomerangMessageId(string accountName, string localHostFqdn, Guid organizationId)
		{
			return this.GenerateBoomerangMessageId(accountName, localHostFqdn, organizationId, -1L);
		}

		public string FormatInternetMessageId(string id, string fqdn)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentNullException("id");
			}
			if (string.IsNullOrWhiteSpace(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			return string.Format("<{0}@{1}>", id, fqdn);
		}

		public bool ValidateBoomerangMessageId(string smtpSenderAddress, string messageId, Guid organizationId)
		{
			byte[] randomBytes = null;
			byte[] second = null;
			byte b = 0;
			byte b2 = 0;
			if (string.IsNullOrWhiteSpace(smtpSenderAddress) || string.IsNullOrWhiteSpace(messageId))
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Invalid inputs for validation. MessageId: {0}; Account Name: {1}", messageId, smtpSenderAddress);
				return false;
			}
			string text = smtpSenderAddress.ToLower();
			if (!this.TryParseBoomerangMessageId(messageId, out randomBytes, out second, out b, out b2))
			{
				return false;
			}
			if (BoomerangHelper.XorHashToByte(text) != b)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Account hint does not match. MessageId: {0}; Account Name: {1}", messageId, text);
				return false;
			}
			long timeIdentifier = BoomerangHelper.GetTimeIdentifier();
			SupportedHashAlgorithmType hashAlgorithmType = this.SelectHashAlgorithm(organizationId);
			for (long num = timeIdentifier + 1L; num > timeIdentifier - (long)((ulong)this.validIntervalsConfig); num -= 1L)
			{
				if (BoomerangHelper.XorHashToByte(BitConverter.GetBytes(num)) != b2)
				{
					ExTraceGlobals.BoomerangTracer.TraceDebug<string, long>((long)this.GetHashCode(), "Date hint does not match. MessageId: {0}; Date Tested: {1}", messageId, num);
				}
				else
				{
					byte[] array = this.GenerateMac(text, num, randomBytes, hashAlgorithmType);
					if (array != null && array.SequenceEqual(second))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal byte[] SetBoomerangMasterKeyForTesting(byte[] newTestKey)
		{
			byte[] result = (this.boomerangMasterKey != null) ? ((byte[])this.boomerangMasterKey.Clone()) : null;
			this.boomerangMasterKey = ((newTestKey != null) ? ((byte[])newTestKey.Clone()) : null);
			return result;
		}

		internal string GenerateBoomerangMessageId(string smtpSenderAddress, string localHostFqdn, Guid organizationId, long timeIdentifier)
		{
			if (string.IsNullOrWhiteSpace(localHostFqdn))
			{
				throw new ArgumentNullException("localHostFqdn");
			}
			if (string.IsNullOrWhiteSpace(smtpSenderAddress))
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug((long)this.GetHashCode(), "Sender Address is empty or null. Falling back to a GUID message ID.");
				return this.FallbackToGuid(localHostFqdn);
			}
			if (this.boomerangMasterKey == null)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug((long)this.GetHashCode(), "It was not possible to retrieve the boomerang master key. Falling back to a GUID message ID.");
				return this.FallbackToGuid(localHostFqdn);
			}
			string text = smtpSenderAddress.ToLower();
			string result;
			try
			{
				if (this.IsBoomerangMessageIdFlightingEnabled(organizationId))
				{
					SupportedHashAlgorithmType hashAlgorithmType = this.SelectHashAlgorithm(organizationId);
					if (timeIdentifier == -1L)
					{
						timeIdentifier = BoomerangHelper.GetTimeIdentifier();
					}
					string value = localHostFqdn.Split(new char[]
					{
						'.'
					})[0];
					byte[] array = this.GenerateRandomBytes(5);
					byte[] array2 = this.GenerateMac(text, timeIdentifier, array, hashAlgorithmType);
					byte b = BoomerangHelper.XorHashToByte(text);
					byte b2 = BoomerangHelper.XorHashToByte(BitConverter.GetBytes(timeIdentifier));
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(value);
					stringBuilder.Append(HexConverter.ByteArrayToHexString(array));
					stringBuilder.Append(HexConverter.ByteArrayToHexString(array2));
					stringBuilder.Append(HexConverter.ByteArrayToHexString(new byte[]
					{
						b
					}));
					stringBuilder.Append(HexConverter.ByteArrayToHexString(new byte[]
					{
						b2
					}));
					stringBuilder.Append("0");
					result = this.FormatInternetMessageId(stringBuilder.ToString(), localHostFqdn);
				}
				else
				{
					result = this.FallbackToGuid(localHostFqdn);
				}
			}
			catch (CryptographicUnexpectedOperationException arg)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<CryptographicUnexpectedOperationException>((long)this.GetHashCode(), "Failed to generate the MAC for this message. Exception: {0}", arg);
				result = this.FallbackToGuid(localHostFqdn);
			}
			catch (CryptographicException arg2)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<CryptographicException>((long)this.GetHashCode(), "Failed to generate random bytes. Exception: {0}", arg2);
				result = this.FallbackToGuid(localHostFqdn);
			}
			return result;
		}

		private byte[] GenerateRandomBytes(int numberOfBytes)
		{
			if (numberOfBytes <= 0)
			{
				throw new ArgumentOutOfRangeException("numberOfBytes");
			}
			byte[] array = new byte[numberOfBytes];
			this.randomNumberGenerator.GetBytes(array);
			return array;
		}

		private byte[] GenerateMac(string accountName, long dateProperty, byte[] randomBytes, SupportedHashAlgorithmType hashAlgorithmType)
		{
			HashAlgorithm hashAlgorithmByType = this.GetHashAlgorithmByType(hashAlgorithmType);
			byte[] bytes = BitConverter.GetBytes(dateProperty);
			byte[] bytes2 = Encoding.ASCII.GetBytes(accountName);
			byte[] array = (byte[])Constants.BoomerangCodeKey.Clone();
			byte[] array2 = (byte[])this.boomerangMasterKey.Clone();
			byte[] array3 = (byte[])randomBytes.Clone();
			byte[] result;
			using (hashAlgorithmByType)
			{
				if (hashAlgorithmByType.TransformBlock(array, 0, array.Length, array, 0) == 0 || hashAlgorithmByType.TransformBlock(array2, 0, array2.Length, array2, 0) == 0 || hashAlgorithmByType.TransformBlock(bytes, 0, bytes.Length, bytes, 0) == 0 || hashAlgorithmByType.TransformBlock(bytes2, 0, bytes2.Length, bytes2, 0) == 0 || hashAlgorithmByType.TransformFinalBlock(array3, 0, array3.Length).Length == 0)
				{
					result = null;
				}
				else
				{
					result = BoomerangHelper.XorHashToByteArray(hashAlgorithmByType.Hash, 5);
				}
			}
			return result;
		}

		private byte[] InitializeBoomerangMasterKey()
		{
			if (!this.IsBoomerangMessageIdFlightingEnabled(null))
			{
				return null;
			}
			RegistryKey registryKey = null;
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			byte[] result;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey(snapshot.Boomerang.BoomerangSettings.MasterKeyRegistryPath);
				if (registryKey == null)
				{
					ExTraceGlobals.BoomerangTracer.TraceDebug((long)this.GetHashCode(), "Registry key containing the Boomerang Master key was not found.");
					result = null;
				}
				else
				{
					string text = (string)registryKey.GetValue(snapshot.Boomerang.BoomerangSettings.MasterKeyRegistryKeyName);
					if (string.IsNullOrWhiteSpace(text))
					{
						ExTraceGlobals.BoomerangTracer.TraceDebug((long)this.GetHashCode(), "Boomerang Master Key is empty or null in the Registry.");
						result = null;
					}
					else
					{
						byte[] encryptedData = Convert.FromBase64String(text);
						result = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
					}
				}
			}
			catch (UnauthorizedAccessException arg)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<UnauthorizedAccessException>((long)this.GetHashCode(), "Failed to retrieve the boomerang master key. Exception: {0}", arg);
				result = null;
			}
			catch (SecurityException arg2)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<SecurityException>((long)this.GetHashCode(), "Failed to retrieve the boomerang master key. Exception: {0}", arg2);
				result = null;
			}
			catch (ObjectDisposedException arg3)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<ObjectDisposedException>((long)this.GetHashCode(), "Failed to retrieve the boomerang master key. Exception: {0}", arg3);
				result = null;
			}
			catch (IOException arg4)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<IOException>((long)this.GetHashCode(), "Failed to retrieve the boomerang master key. Exception: {0}", arg4);
				result = null;
			}
			catch (CryptographicException arg5)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<CryptographicException>((long)this.GetHashCode(), "Failed to retrieve the boomerang master key. Exception: {0}", arg5);
				result = null;
			}
			catch (FormatException arg6)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<FormatException>((long)this.GetHashCode(), "Failed to retrieve the boomerang master key. Exception: {0}", arg6);
				result = null;
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Dispose();
				}
			}
			return result;
		}

		private bool IsBoomerangMessageIdFlightingEnabled(Guid organizationId)
		{
			ConstraintCollection constraintCollection = null;
			if (organizationId != Guid.Empty)
			{
				constraintCollection = ConstraintCollection.CreateEmpty();
				constraintCollection.Add("ExternalDirectoryObjectId", organizationId.ToString());
			}
			return this.IsBoomerangMessageIdFlightingEnabled(constraintCollection);
		}

		private bool IsBoomerangMessageIdFlightingEnabled(ConstraintCollection constraints = null)
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, constraints, null);
			return snapshot != null && snapshot.Boomerang.BoomerangMessageId.Enabled;
		}

		private void DisposeResources()
		{
			if (this.randomNumberGenerator != null)
			{
				this.randomNumberGenerator.Dispose();
			}
		}

		private HashAlgorithm GetHashAlgorithmByType(SupportedHashAlgorithmType hashAlgorithmType)
		{
			switch (hashAlgorithmType)
			{
			case SupportedHashAlgorithmType.RsaSha1:
				return new SHA1Cng();
			}
			return new SHA256Cng();
		}

		private string FallbackToGuid(string fqdn)
		{
			return this.FormatInternetMessageId(Guid.NewGuid().ToString("N"), fqdn);
		}

		private SupportedHashAlgorithmType SelectHashAlgorithm(Guid organizationId)
		{
			if (BoomerangHelper.IsConsumerMailbox(organizationId))
			{
				return SupportedHashAlgorithmType.RsaSha1;
			}
			return SupportedHashAlgorithmType.RsaSha256;
		}

		private bool TryParseBoomerangMessageId(string messageId, out byte[] randomBytes, out byte[] mac, out byte accountHint, out byte dateHint)
		{
			randomBytes = null;
			mac = null;
			accountHint = 0;
			dateHint = 0;
			string[] array = messageId.Split(new char[]
			{
				'@'
			});
			Guid guid;
			if (array.Length != 2 || array[0].Length < BoomerangProvider.EncodedBoomerangDataLen || Guid.TryParse(array[0], out guid))
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<string>((long)this.GetHashCode(), "Message ID was not parsed as a Boomerang Id: {0}", messageId);
				return false;
			}
			string text = array[0];
			int num = text.Length - BoomerangProvider.EncodedBoomerangDataLen;
			if (string.CompareOrdinal("0", text.Substring(num + 24, 1)) != 0)
			{
				ExTraceGlobals.BoomerangTracer.TraceDebug<string>((long)this.GetHashCode(), "Message ID contains an invalid boomerang version", messageId);
				return false;
			}
			try
			{
				randomBytes = HexConverter.HexStringToByteArray(text.Substring(num, 10));
				mac = HexConverter.HexStringToByteArray(text.Substring(num + 10, 10));
				accountHint = HexConverter.HexStringToByteArray(text.Substring(num + 20, 2))[0];
				dateHint = HexConverter.HexStringToByteArray(text.Substring(num + 22, 2))[0];
				if (randomBytes == null || mac == null || randomBytes.Length != 5 || mac.Length != 5)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				if (ex is ArgumentOutOfRangeException || ex is FormatException || ex is OverflowException)
				{
					ExTraceGlobals.BoomerangTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Message ID tokens couldn't be properly parsed. MessageId: {0}; Exception: {1}", messageId, ex.ToString());
					return false;
				}
				throw;
			}
			return true;
		}

		internal const int EncodedMacOffset = 10;

		internal const int EncodedRcptHintOffset = 20;

		internal const int EncodedDateHintOffset = 22;

		internal const int EncodedVersionOffset = 24;

		internal static readonly int EncodedBoomerangDataLen = 24 + "0".Length;

		private static readonly BoomerangProvider instance = new BoomerangProvider();

		private readonly uint validIntervalsConfig = 30U;

		private RNGCryptoServiceProvider randomNumberGenerator;

		private byte[] boomerangMasterKey;
	}
}
