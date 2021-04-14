using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class EncryptedBuffer
	{
		internal EncryptedBuffer(byte[] input) : this(input, string.Empty)
		{
		}

		internal EncryptedBuffer(byte[] input, string stopTones)
		{
			if (input == null || input.Length == 0)
			{
				this.sbuf = new byte[0];
				return;
			}
			int num = input.Length;
			byte[] bytes = Encoding.ASCII.GetBytes(stopTones);
			if (-1 != Array.IndexOf<byte>(bytes, input[num - 1]))
			{
				num--;
				if (num == 0)
				{
					this.sbuf = new byte[0];
					return;
				}
			}
			this.entropy = new byte[8];
			EncryptedBuffer.rng.GetBytes(this.entropy);
			if (num != input.Length)
			{
				using (SafeBuffer safeBuffer = new SafeBuffer(num))
				{
					Array.Copy(input, safeBuffer.Buffer, num);
					this.sbuf = ProtectedData.Protect(safeBuffer.Buffer, this.entropy, DataProtectionScope.CurrentUser);
					return;
				}
			}
			this.sbuf = ProtectedData.Protect(input, this.entropy, DataProtectionScope.CurrentUser);
		}

		internal SafeBuffer Decrypted
		{
			get
			{
				SafeBuffer result;
				if (this.sbuf != null && this.sbuf.Length > 0)
				{
					result = new SafeBuffer(ProtectedData.Unprotect(this.sbuf, this.entropy, DataProtectionScope.CurrentUser));
				}
				else
				{
					result = new SafeBuffer(new byte[0]);
				}
				return result;
			}
		}

		public static bool operator ==(EncryptedBuffer lhs, EncryptedBuffer rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(EncryptedBuffer lhs, EncryptedBuffer rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			EncryptedBuffer encryptedBuffer = o as EncryptedBuffer;
			if (encryptedBuffer == null)
			{
				return false;
			}
			using (SafeBuffer decrypted = this.Decrypted)
			{
				using (SafeBuffer decrypted2 = encryptedBuffer.Decrypted)
				{
					if (decrypted == null || decrypted2 == null)
					{
						return decrypted == null && null == decrypted2;
					}
					if (decrypted.Buffer == null || decrypted2.Buffer == null)
					{
						return decrypted.Buffer == null && null == decrypted2.Buffer;
					}
					if (decrypted.Buffer.Length != decrypted2.Buffer.Length)
					{
						return false;
					}
					for (int i = 0; i < decrypted.Buffer.Length; i++)
					{
						if (decrypted.Buffer[i] != decrypted2.Buffer[i])
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (this.sbuf == null || this.sbuf.Length < 4)
			{
				return 0;
			}
			return BitConverter.ToInt32(this.sbuf, 0);
		}

		private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

		private byte[] sbuf;

		private byte[] entropy;
	}
}
