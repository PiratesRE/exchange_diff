using System;

namespace Microsoft.Exchange.Security.Compliance
{
	public class HMACMD5ForNonCryptographicPurposes : HMACForNonCryptographicPurposes
	{
		public HMACMD5ForNonCryptographicPurposes(byte[] key) : base(key, new MessageDigestForNonCryptographicPurposes())
		{
		}

		public override bool CanReuseTransform
		{
			get
			{
				return false;
			}
		}

		public override bool CanTransformMultipleBlocks
		{
			get
			{
				return false;
			}
		}
	}
}
