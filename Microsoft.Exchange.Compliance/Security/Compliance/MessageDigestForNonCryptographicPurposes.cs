using System;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Security.Compliance
{
	internal sealed class MessageDigestForNonCryptographicPurposes : HashAlgorithm
	{
		public MessageDigestForNonCryptographicPurposes()
		{
			this.HashSizeValue = 128;
			this.hashWords = new uint[4];
			this.temporaryState = new uint[16];
			this.remainder = new byte[64];
			this.Initialize();
		}

		public new static HashAlgorithm Create()
		{
			return new MessageDigestForNonCryptographicPurposes();
		}

		public override void Initialize()
		{
			this.hashWords[0] = 1732584193U;
			this.hashWords[1] = 4023233417U;
			this.hashWords[2] = 2562383102U;
			this.hashWords[3] = 271733878U;
		}

		protected override byte[] HashFinal()
		{
			int num = this.remainderSize;
			this.ZeroRemainder();
			this.remainder[num] = 128;
			if (num + 1 + 8 > 64)
			{
				this.ProcessArray(this.remainder, 0);
				this.remainderSize = 0;
				this.ZeroRemainder();
			}
			ulong num2 = (ulong)((long)this.lastMessageSize * 8L);
			for (int i = 8; i > 0; i--)
			{
				this.remainder[64 - i] = (byte)(num2 >> (8 - i) * 8 & 255UL);
			}
			this.ProcessArray(this.remainder, 0);
			byte[] array = new byte[16];
			for (int j = 0; j < 4; j++)
			{
				Buffer.BlockCopy(BitConverter.GetBytes(this.hashWords[j]), 0, array, j << 2, 4);
			}
			this.remainderSize = 0;
			this.lastMessageSize = 0;
			return array;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			int num = 0;
			if (this.remainderSize > 0)
			{
				int num2 = 64 - this.remainderSize;
				num = ((cbSize < num2) ? cbSize : num2);
				Buffer.BlockCopy(array, ibStart, this.remainder, this.remainderSize, num);
				this.remainderSize += num;
				if (this.remainderSize == 64)
				{
					this.ProcessArray(this.remainder, 0);
					this.remainderSize = 0;
				}
			}
			while (num + 64 <= cbSize)
			{
				this.ProcessArray(array, num + ibStart);
				num += 64;
			}
			if (num < cbSize)
			{
				int num3 = cbSize - num;
				Buffer.BlockCopy(array, num + ibStart, this.remainder, this.remainderSize, num3);
				this.remainderSize += num3;
			}
			this.lastMessageSize += cbSize;
		}

		private void ZeroRemainder()
		{
			for (int i = this.remainderSize; i < 64; i++)
			{
				this.remainder[i] = 0;
			}
		}

		private void ProcessArray(byte[] array, int offset)
		{
			for (uint num = 0U; num < 61U; num += 4U)
			{
				this.temporaryState[(int)((UIntPtr)(num >> 2))] = (uint)(checked((int)array[(int)((IntPtr)(unchecked((long)offset + (long)((ulong)(num + 3U)))))] << 24 | (int)array[(int)((IntPtr)(unchecked((long)offset + (long)((ulong)(num + 2U)))))] << 16 | (int)array[(int)((IntPtr)(unchecked((long)offset + (long)((ulong)(num + 1U)))))] << 8 | (int)array[(int)((IntPtr)(unchecked((long)offset + (long)((ulong)num))))]));
			}
			this.PerformTransformation(ref this.hashWords[0], ref this.hashWords[1], ref this.hashWords[2], ref this.hashWords[3]);
		}

		private void PerformTransformation(ref uint a, ref uint b, ref uint c, ref uint d)
		{
			uint num = a;
			uint num2 = b;
			uint num3 = c;
			uint num4 = d;
			a = b + (a + ((b & c) | (~b & d)) + this.temporaryState[0] + MessageDigestForNonCryptographicPurposes.T[0] >> 25 | a + ((b & c) | (~b & d)) + this.temporaryState[0] + MessageDigestForNonCryptographicPurposes.T[0] << 7);
			d = a + (d + ((a & b) | (~a & c)) + this.temporaryState[1] + MessageDigestForNonCryptographicPurposes.T[1] >> 20 | d + ((a & b) | (~a & c)) + this.temporaryState[1] + MessageDigestForNonCryptographicPurposes.T[1] << 12);
			c = d + (c + ((d & a) | (~d & b)) + this.temporaryState[2] + MessageDigestForNonCryptographicPurposes.T[2] >> 15 | c + ((d & a) | (~d & b)) + this.temporaryState[2] + MessageDigestForNonCryptographicPurposes.T[2] << 17);
			b = c + (b + ((c & d) | (~c & a)) + this.temporaryState[3] + MessageDigestForNonCryptographicPurposes.T[3] >> 10 | b + ((c & d) | (~c & a)) + this.temporaryState[3] + MessageDigestForNonCryptographicPurposes.T[3] << 22);
			a = b + (a + ((b & c) | (~b & d)) + this.temporaryState[4] + MessageDigestForNonCryptographicPurposes.T[4] >> 25 | a + ((b & c) | (~b & d)) + this.temporaryState[4] + MessageDigestForNonCryptographicPurposes.T[4] << 7);
			d = a + (d + ((a & b) | (~a & c)) + this.temporaryState[5] + MessageDigestForNonCryptographicPurposes.T[5] >> 20 | d + ((a & b) | (~a & c)) + this.temporaryState[5] + MessageDigestForNonCryptographicPurposes.T[5] << 12);
			c = d + (c + ((d & a) | (~d & b)) + this.temporaryState[6] + MessageDigestForNonCryptographicPurposes.T[6] >> 15 | c + ((d & a) | (~d & b)) + this.temporaryState[6] + MessageDigestForNonCryptographicPurposes.T[6] << 17);
			b = c + (b + ((c & d) | (~c & a)) + this.temporaryState[7] + MessageDigestForNonCryptographicPurposes.T[7] >> 10 | b + ((c & d) | (~c & a)) + this.temporaryState[7] + MessageDigestForNonCryptographicPurposes.T[7] << 22);
			a = b + (a + ((b & c) | (~b & d)) + this.temporaryState[8] + MessageDigestForNonCryptographicPurposes.T[8] >> 25 | a + ((b & c) | (~b & d)) + this.temporaryState[8] + MessageDigestForNonCryptographicPurposes.T[8] << 7);
			d = a + (d + ((a & b) | (~a & c)) + this.temporaryState[9] + MessageDigestForNonCryptographicPurposes.T[9] >> 20 | d + ((a & b) | (~a & c)) + this.temporaryState[9] + MessageDigestForNonCryptographicPurposes.T[9] << 12);
			c = d + (c + ((d & a) | (~d & b)) + this.temporaryState[10] + MessageDigestForNonCryptographicPurposes.T[10] >> 15 | c + ((d & a) | (~d & b)) + this.temporaryState[10] + MessageDigestForNonCryptographicPurposes.T[10] << 17);
			b = c + (b + ((c & d) | (~c & a)) + this.temporaryState[11] + MessageDigestForNonCryptographicPurposes.T[11] >> 10 | b + ((c & d) | (~c & a)) + this.temporaryState[11] + MessageDigestForNonCryptographicPurposes.T[11] << 22);
			a = b + (a + ((b & c) | (~b & d)) + this.temporaryState[12] + MessageDigestForNonCryptographicPurposes.T[12] >> 25 | a + ((b & c) | (~b & d)) + this.temporaryState[12] + MessageDigestForNonCryptographicPurposes.T[12] << 7);
			d = a + (d + ((a & b) | (~a & c)) + this.temporaryState[13] + MessageDigestForNonCryptographicPurposes.T[13] >> 20 | d + ((a & b) | (~a & c)) + this.temporaryState[13] + MessageDigestForNonCryptographicPurposes.T[13] << 12);
			c = d + (c + ((d & a) | (~d & b)) + this.temporaryState[14] + MessageDigestForNonCryptographicPurposes.T[14] >> 15 | c + ((d & a) | (~d & b)) + this.temporaryState[14] + MessageDigestForNonCryptographicPurposes.T[14] << 17);
			b = c + (b + ((c & d) | (~c & a)) + this.temporaryState[15] + MessageDigestForNonCryptographicPurposes.T[15] >> 10 | b + ((c & d) | (~c & a)) + this.temporaryState[15] + MessageDigestForNonCryptographicPurposes.T[15] << 22);
			a = b + (a + ((b & d) | (c & ~d)) + this.temporaryState[1] + MessageDigestForNonCryptographicPurposes.T[16] >> 27 | a + ((b & d) | (c & ~d)) + this.temporaryState[1] + MessageDigestForNonCryptographicPurposes.T[16] << 5);
			d = a + (d + ((a & c) | (b & ~c)) + this.temporaryState[6] + MessageDigestForNonCryptographicPurposes.T[17] >> 23 | d + ((a & c) | (b & ~c)) + this.temporaryState[6] + MessageDigestForNonCryptographicPurposes.T[17] << 9);
			c = d + (c + ((d & b) | (a & ~b)) + this.temporaryState[11] + MessageDigestForNonCryptographicPurposes.T[18] >> 18 | c + ((d & b) | (a & ~b)) + this.temporaryState[11] + MessageDigestForNonCryptographicPurposes.T[18] << 14);
			b = c + (b + ((c & a) | (d & ~a)) + this.temporaryState[0] + MessageDigestForNonCryptographicPurposes.T[19] >> 12 | b + ((c & a) | (d & ~a)) + this.temporaryState[0] + MessageDigestForNonCryptographicPurposes.T[19] << 20);
			a = b + (a + ((b & d) | (c & ~d)) + this.temporaryState[5] + MessageDigestForNonCryptographicPurposes.T[20] >> 27 | a + ((b & d) | (c & ~d)) + this.temporaryState[5] + MessageDigestForNonCryptographicPurposes.T[20] << 5);
			d = a + (d + ((a & c) | (b & ~c)) + this.temporaryState[10] + MessageDigestForNonCryptographicPurposes.T[21] >> 23 | d + ((a & c) | (b & ~c)) + this.temporaryState[10] + MessageDigestForNonCryptographicPurposes.T[21] << 9);
			c = d + (c + ((d & b) | (a & ~b)) + this.temporaryState[15] + MessageDigestForNonCryptographicPurposes.T[22] >> 18 | c + ((d & b) | (a & ~b)) + this.temporaryState[15] + MessageDigestForNonCryptographicPurposes.T[22] << 14);
			b = c + (b + ((c & a) | (d & ~a)) + this.temporaryState[4] + MessageDigestForNonCryptographicPurposes.T[23] >> 12 | b + ((c & a) | (d & ~a)) + this.temporaryState[4] + MessageDigestForNonCryptographicPurposes.T[23] << 20);
			a = b + (a + ((b & d) | (c & ~d)) + this.temporaryState[9] + MessageDigestForNonCryptographicPurposes.T[24] >> 27 | a + ((b & d) | (c & ~d)) + this.temporaryState[9] + MessageDigestForNonCryptographicPurposes.T[24] << 5);
			d = a + (d + ((a & c) | (b & ~c)) + this.temporaryState[14] + MessageDigestForNonCryptographicPurposes.T[25] >> 23 | d + ((a & c) | (b & ~c)) + this.temporaryState[14] + MessageDigestForNonCryptographicPurposes.T[25] << 9);
			c = d + (c + ((d & b) | (a & ~b)) + this.temporaryState[3] + MessageDigestForNonCryptographicPurposes.T[26] >> 18 | c + ((d & b) | (a & ~b)) + this.temporaryState[3] + MessageDigestForNonCryptographicPurposes.T[26] << 14);
			b = c + (b + ((c & a) | (d & ~a)) + this.temporaryState[8] + MessageDigestForNonCryptographicPurposes.T[27] >> 12 | b + ((c & a) | (d & ~a)) + this.temporaryState[8] + MessageDigestForNonCryptographicPurposes.T[27] << 20);
			a = b + (a + ((b & d) | (c & ~d)) + this.temporaryState[13] + MessageDigestForNonCryptographicPurposes.T[28] >> 27 | a + ((b & d) | (c & ~d)) + this.temporaryState[13] + MessageDigestForNonCryptographicPurposes.T[28] << 5);
			d = a + (d + ((a & c) | (b & ~c)) + this.temporaryState[2] + MessageDigestForNonCryptographicPurposes.T[29] >> 23 | d + ((a & c) | (b & ~c)) + this.temporaryState[2] + MessageDigestForNonCryptographicPurposes.T[29] << 9);
			c = d + (c + ((d & b) | (a & ~b)) + this.temporaryState[7] + MessageDigestForNonCryptographicPurposes.T[30] >> 18 | c + ((d & b) | (a & ~b)) + this.temporaryState[7] + MessageDigestForNonCryptographicPurposes.T[30] << 14);
			b = c + (b + ((c & a) | (d & ~a)) + this.temporaryState[12] + MessageDigestForNonCryptographicPurposes.T[31] >> 12 | b + ((c & a) | (d & ~a)) + this.temporaryState[12] + MessageDigestForNonCryptographicPurposes.T[31] << 20);
			a = b + (a + (b ^ c ^ d) + this.temporaryState[5] + MessageDigestForNonCryptographicPurposes.T[32] >> 28 | a + (b ^ c ^ d) + this.temporaryState[5] + MessageDigestForNonCryptographicPurposes.T[32] << 4);
			d = a + (d + (a ^ b ^ c) + this.temporaryState[8] + MessageDigestForNonCryptographicPurposes.T[33] >> 21 | d + (a ^ b ^ c) + this.temporaryState[8] + MessageDigestForNonCryptographicPurposes.T[33] << 11);
			c = d + (c + (d ^ a ^ b) + this.temporaryState[11] + MessageDigestForNonCryptographicPurposes.T[34] >> 16 | c + (d ^ a ^ b) + this.temporaryState[11] + MessageDigestForNonCryptographicPurposes.T[34] << 16);
			b = c + (b + (c ^ d ^ a) + this.temporaryState[14] + MessageDigestForNonCryptographicPurposes.T[35] >> 9 | b + (c ^ d ^ a) + this.temporaryState[14] + MessageDigestForNonCryptographicPurposes.T[35] << 23);
			a = b + (a + (b ^ c ^ d) + this.temporaryState[1] + MessageDigestForNonCryptographicPurposes.T[36] >> 28 | a + (b ^ c ^ d) + this.temporaryState[1] + MessageDigestForNonCryptographicPurposes.T[36] << 4);
			d = a + (d + (a ^ b ^ c) + this.temporaryState[4] + MessageDigestForNonCryptographicPurposes.T[37] >> 21 | d + (a ^ b ^ c) + this.temporaryState[4] + MessageDigestForNonCryptographicPurposes.T[37] << 11);
			c = d + (c + (d ^ a ^ b) + this.temporaryState[7] + MessageDigestForNonCryptographicPurposes.T[38] >> 16 | c + (d ^ a ^ b) + this.temporaryState[7] + MessageDigestForNonCryptographicPurposes.T[38] << 16);
			b = c + (b + (c ^ d ^ a) + this.temporaryState[10] + MessageDigestForNonCryptographicPurposes.T[39] >> 9 | b + (c ^ d ^ a) + this.temporaryState[10] + MessageDigestForNonCryptographicPurposes.T[39] << 23);
			a = b + (a + (b ^ c ^ d) + this.temporaryState[13] + MessageDigestForNonCryptographicPurposes.T[40] >> 28 | a + (b ^ c ^ d) + this.temporaryState[13] + MessageDigestForNonCryptographicPurposes.T[40] << 4);
			d = a + (d + (a ^ b ^ c) + this.temporaryState[0] + MessageDigestForNonCryptographicPurposes.T[41] >> 21 | d + (a ^ b ^ c) + this.temporaryState[0] + MessageDigestForNonCryptographicPurposes.T[41] << 11);
			c = d + (c + (d ^ a ^ b) + this.temporaryState[3] + MessageDigestForNonCryptographicPurposes.T[42] >> 16 | c + (d ^ a ^ b) + this.temporaryState[3] + MessageDigestForNonCryptographicPurposes.T[42] << 16);
			b = c + (b + (c ^ d ^ a) + this.temporaryState[6] + MessageDigestForNonCryptographicPurposes.T[43] >> 9 | b + (c ^ d ^ a) + this.temporaryState[6] + MessageDigestForNonCryptographicPurposes.T[43] << 23);
			a = b + (a + (b ^ c ^ d) + this.temporaryState[9] + MessageDigestForNonCryptographicPurposes.T[44] >> 28 | a + (b ^ c ^ d) + this.temporaryState[9] + MessageDigestForNonCryptographicPurposes.T[44] << 4);
			d = a + (d + (a ^ b ^ c) + this.temporaryState[12] + MessageDigestForNonCryptographicPurposes.T[45] >> 21 | d + (a ^ b ^ c) + this.temporaryState[12] + MessageDigestForNonCryptographicPurposes.T[45] << 11);
			c = d + (c + (d ^ a ^ b) + this.temporaryState[15] + MessageDigestForNonCryptographicPurposes.T[46] >> 16 | c + (d ^ a ^ b) + this.temporaryState[15] + MessageDigestForNonCryptographicPurposes.T[46] << 16);
			b = c + (b + (c ^ d ^ a) + this.temporaryState[2] + MessageDigestForNonCryptographicPurposes.T[47] >> 9 | b + (c ^ d ^ a) + this.temporaryState[2] + MessageDigestForNonCryptographicPurposes.T[47] << 23);
			a = b + (a + (c ^ (b | ~d)) + this.temporaryState[0] + MessageDigestForNonCryptographicPurposes.T[48] >> 26 | a + (c ^ (b | ~d)) + this.temporaryState[0] + MessageDigestForNonCryptographicPurposes.T[48] << 6);
			d = a + (d + (b ^ (a | ~c)) + this.temporaryState[7] + MessageDigestForNonCryptographicPurposes.T[49] >> 22 | d + (b ^ (a | ~c)) + this.temporaryState[7] + MessageDigestForNonCryptographicPurposes.T[49] << 10);
			c = d + (c + (a ^ (d | ~b)) + this.temporaryState[14] + MessageDigestForNonCryptographicPurposes.T[50] >> 17 | c + (a ^ (d | ~b)) + this.temporaryState[14] + MessageDigestForNonCryptographicPurposes.T[50] << 15);
			b = c + (b + (d ^ (c | ~a)) + this.temporaryState[5] + MessageDigestForNonCryptographicPurposes.T[51] >> 11 | b + (d ^ (c | ~a)) + this.temporaryState[5] + MessageDigestForNonCryptographicPurposes.T[51] << 21);
			a = b + (a + (c ^ (b | ~d)) + this.temporaryState[12] + MessageDigestForNonCryptographicPurposes.T[52] >> 26 | a + (c ^ (b | ~d)) + this.temporaryState[12] + MessageDigestForNonCryptographicPurposes.T[52] << 6);
			d = a + (d + (b ^ (a | ~c)) + this.temporaryState[3] + MessageDigestForNonCryptographicPurposes.T[53] >> 22 | d + (b ^ (a | ~c)) + this.temporaryState[3] + MessageDigestForNonCryptographicPurposes.T[53] << 10);
			c = d + (c + (a ^ (d | ~b)) + this.temporaryState[10] + MessageDigestForNonCryptographicPurposes.T[54] >> 17 | c + (a ^ (d | ~b)) + this.temporaryState[10] + MessageDigestForNonCryptographicPurposes.T[54] << 15);
			b = c + (b + (d ^ (c | ~a)) + this.temporaryState[1] + MessageDigestForNonCryptographicPurposes.T[55] >> 11 | b + (d ^ (c | ~a)) + this.temporaryState[1] + MessageDigestForNonCryptographicPurposes.T[55] << 21);
			a = b + (a + (c ^ (b | ~d)) + this.temporaryState[8] + MessageDigestForNonCryptographicPurposes.T[56] >> 26 | a + (c ^ (b | ~d)) + this.temporaryState[8] + MessageDigestForNonCryptographicPurposes.T[56] << 6);
			d = a + (d + (b ^ (a | ~c)) + this.temporaryState[15] + MessageDigestForNonCryptographicPurposes.T[57] >> 22 | d + (b ^ (a | ~c)) + this.temporaryState[15] + MessageDigestForNonCryptographicPurposes.T[57] << 10);
			c = d + (c + (a ^ (d | ~b)) + this.temporaryState[6] + MessageDigestForNonCryptographicPurposes.T[58] >> 17 | c + (a ^ (d | ~b)) + this.temporaryState[6] + MessageDigestForNonCryptographicPurposes.T[58] << 15);
			b = c + (b + (d ^ (c | ~a)) + this.temporaryState[13] + MessageDigestForNonCryptographicPurposes.T[59] >> 11 | b + (d ^ (c | ~a)) + this.temporaryState[13] + MessageDigestForNonCryptographicPurposes.T[59] << 21);
			a = b + (a + (c ^ (b | ~d)) + this.temporaryState[4] + MessageDigestForNonCryptographicPurposes.T[60] >> 26 | a + (c ^ (b | ~d)) + this.temporaryState[4] + MessageDigestForNonCryptographicPurposes.T[60] << 6);
			d = a + (d + (b ^ (a | ~c)) + this.temporaryState[11] + MessageDigestForNonCryptographicPurposes.T[61] >> 22 | d + (b ^ (a | ~c)) + this.temporaryState[11] + MessageDigestForNonCryptographicPurposes.T[61] << 10);
			c = d + (c + (a ^ (d | ~b)) + this.temporaryState[2] + MessageDigestForNonCryptographicPurposes.T[62] >> 17 | c + (a ^ (d | ~b)) + this.temporaryState[2] + MessageDigestForNonCryptographicPurposes.T[62] << 15);
			b = c + (b + (d ^ (c | ~a)) + this.temporaryState[9] + MessageDigestForNonCryptographicPurposes.T[63] >> 11 | b + (d ^ (c | ~a)) + this.temporaryState[9] + MessageDigestForNonCryptographicPurposes.T[63] << 21);
			a += num;
			b += num2;
			c += num3;
			d += num4;
		}

		private const int Md5ChunkSize = 64;

		private static readonly uint[] T = new uint[]
		{
			3614090360U,
			3905402710U,
			606105819U,
			3250441966U,
			4118548399U,
			1200080426U,
			2821735955U,
			4249261313U,
			1770035416U,
			2336552879U,
			4294925233U,
			2304563134U,
			1804603682U,
			4254626195U,
			2792965006U,
			1236535329U,
			4129170786U,
			3225465664U,
			643717713U,
			3921069994U,
			3593408605U,
			38016083U,
			3634488961U,
			3889429448U,
			568446438U,
			3275163606U,
			4107603335U,
			1163531501U,
			2850285829U,
			4243563512U,
			1735328473U,
			2368359562U,
			4294588738U,
			2272392833U,
			1839030562U,
			4259657740U,
			2763975236U,
			1272893353U,
			4139469664U,
			3200236656U,
			681279174U,
			3936430074U,
			3572445317U,
			76029189U,
			3654602809U,
			3873151461U,
			530742520U,
			3299628645U,
			4096336452U,
			1126891415U,
			2878612391U,
			4237533241U,
			1700485571U,
			2399980690U,
			4293915773U,
			2240044497U,
			1873313359U,
			4264355552U,
			2734768916U,
			1309151649U,
			4149444226U,
			3174756917U,
			718787259U,
			3951481745U
		};

		private uint[] hashWords;

		private uint[] temporaryState;

		private byte[] remainder;

		private int remainderSize;

		private int lastMessageSize;
	}
}
