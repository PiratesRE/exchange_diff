using System;
using Microsoft.Exchange.Security.Compliance;

namespace Microsoft.Exchange.Net
{
	public class Md5Hasher : IDisposable
	{
		public Md5Hasher()
		{
			this.hasherImplementation = new MessageDigestForNonCryptographicPurposes();
		}

		public int HashSize
		{
			get
			{
				return this.hasherImplementation.HashSize;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (this.hasherImplementation != null)
			{
				this.hasherImplementation.Dispose();
				this.hasherImplementation = null;
			}
		}

		public byte[] ComputeHash(byte[] buffer)
		{
			return this.hasherImplementation.ComputeHash(buffer);
		}

		private MessageDigestForNonCryptographicPurposes hasherImplementation;
	}
}
