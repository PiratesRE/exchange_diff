using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal sealed class PasswordBlob
	{
		internal PasswordBlob(byte[] blobdata)
		{
			this.Initialize(blobdata, 0);
		}

		internal PasswordBlob(byte[] blobdata, int startIdx)
		{
			this.Initialize(blobdata, startIdx);
		}

		internal PasswordBlob(EncryptedBuffer pwd, string algorithm, int iterations)
		{
			this.salt = new byte[8];
			PasswordBlob.rng.GetBytes(this.salt);
			this.Initialize(pwd, algorithm, iterations);
		}

		private PasswordBlob(EncryptedBuffer pwd, byte[] salt, string algorithm, int iterations)
		{
			if (salt.Length != 8)
			{
				throw new UserConfigurationException(Strings.TamperedPin);
			}
			this.salt = new byte[8];
			salt.CopyTo(this.salt, 0);
			this.Initialize(pwd, algorithm, iterations);
		}

		internal byte[] Blob
		{
			get
			{
				return this.blob;
			}
		}

		internal string Algorithm
		{
			get
			{
				return this.algorithm;
			}
		}

		internal int Iterations
		{
			get
			{
				return this.iterations;
			}
		}

		public static bool operator ==(PasswordBlob lhs, PasswordBlob rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(PasswordBlob lhs, PasswordBlob rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			PasswordBlob passwordBlob;
			if (o is PasswordBlob)
			{
				passwordBlob = (PasswordBlob)o;
			}
			else
			{
				if (!(o is EncryptedBuffer))
				{
					return false;
				}
				passwordBlob = new PasswordBlob((EncryptedBuffer)o, this.salt, this.algorithm, this.iterations);
			}
			if (this.blob.Length != passwordBlob.blob.Length)
			{
				return false;
			}
			for (int i = 0; i < this.blob.Length; i++)
			{
				if (passwordBlob.blob[i] != this.blob[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (this.hash != null)
			{
				return BitConverter.ToInt32(this.hash, 0);
			}
			return 0;
		}

		private void Initialize(EncryptedBuffer pwd, string algorithm, int iterations)
		{
			this.algorithm = algorithm;
			this.iterations = iterations;
			byte[] bytes = Encoding.UTF8.GetBytes(algorithm);
			CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "initializing password blob with algorithm={0} and iterations={1}.", new object[]
			{
				algorithm,
				iterations
			});
			using (SafeBuffer decrypted = pwd.Decrypted)
			{
				using (PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(decrypted.Buffer, this.salt, algorithm, iterations))
				{
					this.hash = passwordDeriveBytes.GetBytes(16);
				}
				int num = 0;
				this.blob = new byte[32 + bytes.Length];
				this.hash.CopyTo(this.blob, num);
				num += this.hash.Length;
				this.salt.CopyTo(this.blob, num);
				num += this.salt.Length;
				byte[] bytes2 = BitConverter.GetBytes(iterations);
				bytes2.CopyTo(this.blob, num);
				num += bytes2.Length;
				byte[] bytes3 = BitConverter.GetBytes(bytes.Length);
				bytes3.CopyTo(this.blob, num);
				num += bytes3.Length;
				bytes.CopyTo(this.blob, num);
			}
		}

		private void Initialize(byte[] rawdata, int startIdx)
		{
			if (checked(startIdx + 32) > rawdata.Length)
			{
				CallIdTracer.TraceError(ExTraceGlobals.AuthenticationTracer, this, "failed to intialize password blob from rawdata because rawdata is the wrong size.", new object[0]);
				throw new UserConfigurationException(Strings.TamperedPin);
			}
			int num = 0;
			this.hash = new byte[16];
			Array.Copy(rawdata, startIdx + num, this.hash, 0, 16);
			num += 16;
			this.salt = new byte[8];
			Array.Copy(rawdata, startIdx + num, this.salt, 0, 8);
			num += 8;
			this.iterations = BitConverter.ToInt32(rawdata, startIdx + num);
			num += 4;
			int num2 = BitConverter.ToInt32(rawdata, startIdx + num);
			num += 4;
			if (checked(startIdx + 32 + num2) > rawdata.Length)
			{
				CallIdTracer.TraceError(ExTraceGlobals.AuthenticationTracer, this, "failed to intialize password blob from rawdata because rawdata is the wrong size.", new object[0]);
				throw new UserConfigurationException(Strings.TamperedPin);
			}
			this.algorithm = Encoding.UTF8.GetString(rawdata, startIdx + num, num2);
			num += num2;
			this.blob = new byte[num];
			Array.Copy(rawdata, startIdx, this.blob, 0, num);
		}

		internal const int CbFixedBlobSize = 32;

		private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

		private byte[] salt;

		private byte[] hash;

		private byte[] blob;

		private string algorithm;

		private int iterations;
	}
}
