using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Dkm.Proxy;

namespace Microsoft.Exchange.Security.Dkm
{
	internal sealed class ExchangeGroupKey : IExchangeGroupKey
	{
		public ExchangeGroupKey(string dkmPath = null, string containerName = "Microsoft Exchange DKM")
		{
			this.groupKeyObject = null;
			this.dkmPath = dkmPath;
			this.containerName = containerName;
		}

		public string ParentContainerDN
		{
			get
			{
				return this.parentContainerDN;
			}
			set
			{
				this.parentContainerDN = value;
			}
		}

		private DkmProxy GroupKey
		{
			get
			{
				DkmProxy result;
				lock (ExchangeGroupKey.lockObject)
				{
					if (this.groupKeyObject == null)
					{
						this.groupKeyObject = new DkmProxy(this.containerName, this.dkmPath, this.parentContainerDN);
					}
					result = this.groupKeyObject;
				}
				return result;
			}
		}

		public string ClearStringToEncryptedString(string clearString)
		{
			if (string.IsNullOrEmpty(clearString))
			{
				return null;
			}
			string result;
			using (MemoryStream memoryStream = this.ClearStringToMemoryStream(clearString))
			{
				using (MemoryStream memoryStream2 = this.GroupKey.Protect(memoryStream))
				{
					string text = this.MemoryStreamToBase64String(memoryStream2);
					result = text;
				}
			}
			return result;
		}

		public string ByteArrayToEncryptedString(byte[] bytes)
		{
			if (bytes == null || bytes.Length == 0)
			{
				return null;
			}
			string result;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				using (MemoryStream memoryStream2 = this.GroupKey.Protect(memoryStream))
				{
					string text = this.MemoryStreamToBase64String(memoryStream2);
					result = text;
				}
			}
			return result;
		}

		public string SecureStringToEncryptedString(SecureString secureString)
		{
			if (secureString == null || secureString.Length == 0)
			{
				return null;
			}
			string result;
			using (MemoryStream memoryStream = this.SecureStringToMemoryStream(secureString))
			{
				using (MemoryStream memoryStream2 = this.GroupKey.Protect(memoryStream))
				{
					string text = this.MemoryStreamToBase64String(memoryStream2);
					result = text;
				}
			}
			return result;
		}

		public SecureString EncryptedStringToSecureString(string encryptedString)
		{
			if (string.IsNullOrEmpty(encryptedString))
			{
				return null;
			}
			SecureString result;
			using (MemoryStream memoryStream = this.Base64StringToMemoryStream(encryptedString))
			{
				using (MemoryStream memoryStream2 = this.GroupKey.Unprotect(memoryStream))
				{
					SecureString secureString = this.MemoryStreamToSecureString(memoryStream2);
					result = secureString;
				}
			}
			return result;
		}

		public bool TrySecureStringToEncryptedString(SecureString secureString, out string encryptedString, out Exception exception)
		{
			exception = null;
			encryptedString = null;
			try
			{
				encryptedString = this.SecureStringToEncryptedString(secureString);
				return true;
			}
			catch (CryptographicException ex)
			{
				exception = ex;
			}
			catch (InvalidDataException ex2)
			{
				exception = ex2;
			}
			catch (Exception ex3)
			{
				if (!this.GroupKey.IsDkmException(ex3))
				{
					throw;
				}
				exception = ex3;
			}
			return false;
		}

		public bool TryEncryptedStringToBuffer(string encryptedString, out byte[] buffer, out Exception exception)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("encryptedString", encryptedString);
			exception = null;
			buffer = null;
			try
			{
				using (MemoryStream memoryStream = this.Base64StringToMemoryStream(encryptedString))
				{
					using (MemoryStream memoryStream2 = this.GroupKey.Unprotect(memoryStream))
					{
						buffer = new byte[memoryStream2.Length];
						memoryStream2.Position = 0L;
						memoryStream2.Read(buffer, 0, buffer.Length);
						return true;
					}
				}
			}
			catch (CryptographicException ex)
			{
				exception = ex;
			}
			catch (InvalidDataException ex2)
			{
				exception = ex2;
			}
			catch (FormatException ex3)
			{
				exception = ex3;
			}
			catch (Exception ex4)
			{
				if (!this.GroupKey.IsDkmException(ex4))
				{
					throw;
				}
				exception = ex4;
			}
			return false;
		}

		public bool TryEncryptedStringToSecureString(string encryptedString, out SecureString secureString, out Exception exception)
		{
			exception = null;
			secureString = null;
			try
			{
				secureString = this.EncryptedStringToSecureString(encryptedString);
				return true;
			}
			catch (CryptographicException ex)
			{
				exception = ex;
			}
			catch (InvalidDataException ex2)
			{
				exception = ex2;
			}
			catch (FormatException ex3)
			{
				exception = ex3;
			}
			catch (Exception ex4)
			{
				if (!this.GroupKey.IsDkmException(ex4))
				{
					throw;
				}
				exception = ex4;
			}
			return false;
		}

		public bool TryByteArrayToEncryptedString(byte[] bytes, out string encryptedString, out Exception exception)
		{
			exception = null;
			encryptedString = null;
			try
			{
				encryptedString = this.ByteArrayToEncryptedString(bytes);
				return true;
			}
			catch (CryptographicException ex)
			{
				exception = ex;
			}
			catch (InvalidDataException ex2)
			{
				exception = ex2;
			}
			catch (Exception ex3)
			{
				if (!this.GroupKey.IsDkmException(ex3))
				{
					throw;
				}
				exception = ex3;
			}
			return false;
		}

		public bool IsDkmException(Exception e)
		{
			return this.GroupKey.IsDkmException(e);
		}

		private MemoryStream Base64StringToMemoryStream(string clearString)
		{
			if (string.IsNullOrEmpty(clearString))
			{
				return null;
			}
			byte[] buffer = Convert.FromBase64String(clearString);
			return new MemoryStream(buffer);
		}

		private MemoryStream ClearStringToMemoryStream(string clearString)
		{
			if (string.IsNullOrEmpty(clearString))
			{
				return null;
			}
			byte[] bytes = Encoding.Unicode.GetBytes(clearString);
			return new MemoryStream(bytes);
		}

		private string MemoryStreamToBase64String(MemoryStream memoryStream)
		{
			if (memoryStream == null)
			{
				return null;
			}
			if (memoryStream.Length < 1L)
			{
				return null;
			}
			if (memoryStream.Position != 0L)
			{
				memoryStream.Seek(0L, SeekOrigin.Begin);
			}
			byte[] array = new byte[memoryStream.Length];
			memoryStream.Read(array, 0, (int)memoryStream.Length);
			string result = Convert.ToBase64String(array);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return result;
		}

		private MemoryStream SecureStringToMemoryStream(SecureString secureString)
		{
			if (secureString == null)
			{
				return null;
			}
			IntPtr intPtr = IntPtr.Zero;
			MemoryStream result = null;
			try
			{
				intPtr = Marshal.SecureStringToBSTR(secureString);
				int num = Marshal.ReadInt32(intPtr, -4);
				byte[] array = new byte[num];
				Marshal.Copy(intPtr, array, 0, num);
				result = new MemoryStream(array);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeBSTR(intPtr);
				}
			}
			return result;
		}

		private SecureString MemoryStreamToSecureString(MemoryStream memoryStream)
		{
			if (memoryStream == null)
			{
				return null;
			}
			if (memoryStream.Position != 0L)
			{
				memoryStream.Seek(0L, SeekOrigin.Begin);
			}
			SecureString secureString = new SecureString();
			byte[] buffer = memoryStream.GetBuffer();
			int num = 0;
			while ((long)num < memoryStream.Length)
			{
				byte b = buffer[num];
				byte b2 = buffer[num + 1];
				char c = (char)((int)b | (int)b2 << 8);
				secureString.AppendChar(c);
				buffer[num] = 0;
				buffer[num + 1] = 0;
				num += 2;
			}
			memoryStream.Seek(0L, SeekOrigin.Begin);
			secureString.MakeReadOnly();
			return secureString;
		}

		private static readonly object lockObject = new object();

		private readonly string dkmPath;

		private readonly string containerName;

		private string parentContainerDN;

		private DkmProxy groupKeyObject;
	}
}
