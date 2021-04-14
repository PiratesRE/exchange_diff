using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class RIPEMD160Managed : RIPEMD160
	{
		public RIPEMD160Managed()
		{
			if (CryptoConfig.AllowOnlyFipsAlgorithms && AppContextSwitches.UseLegacyFipsThrow)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Cryptography_NonCompliantFIPSAlgorithm"));
			}
			this._stateMD160 = new uint[5];
			this._blockDWords = new uint[16];
			this._buffer = new byte[64];
			this.InitializeState();
		}

		public override void Initialize()
		{
			this.InitializeState();
			Array.Clear(this._blockDWords, 0, this._blockDWords.Length);
			Array.Clear(this._buffer, 0, this._buffer.Length);
		}

		[SecuritySafeCritical]
		protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
		{
			this._HashData(rgb, ibStart, cbSize);
		}

		[SecuritySafeCritical]
		protected override byte[] HashFinal()
		{
			return this._EndHash();
		}

		private void InitializeState()
		{
			this._count = 0L;
			this._stateMD160[0] = 1732584193U;
			this._stateMD160[1] = 4023233417U;
			this._stateMD160[2] = 2562383102U;
			this._stateMD160[3] = 271733878U;
			this._stateMD160[4] = 3285377520U;
		}

		[SecurityCritical]
		private unsafe void _HashData(byte[] partIn, int ibStart, int cbSize)
		{
			int i = cbSize;
			int num = ibStart;
			int num2 = (int)(this._count & 63L);
			this._count += (long)i;
			fixed (uint* stateMD = this._stateMD160)
			{
				fixed (byte* buffer = this._buffer)
				{
					fixed (uint* blockDWords = this._blockDWords)
					{
						if (num2 > 0 && num2 + i >= 64)
						{
							Buffer.InternalBlockCopy(partIn, num, this._buffer, num2, 64 - num2);
							num += 64 - num2;
							i -= 64 - num2;
							RIPEMD160Managed.MDTransform(blockDWords, stateMD, buffer);
							num2 = 0;
						}
						while (i >= 64)
						{
							Buffer.InternalBlockCopy(partIn, num, this._buffer, 0, 64);
							num += 64;
							i -= 64;
							RIPEMD160Managed.MDTransform(blockDWords, stateMD, buffer);
						}
						if (i > 0)
						{
							Buffer.InternalBlockCopy(partIn, num, this._buffer, num2, i);
						}
					}
				}
			}
		}

		[SecurityCritical]
		private byte[] _EndHash()
		{
			byte[] array = new byte[20];
			int num = 64 - (int)(this._count & 63L);
			if (num <= 8)
			{
				num += 64;
			}
			byte[] array2 = new byte[num];
			array2[0] = 128;
			long num2 = this._count * 8L;
			array2[num - 1] = (byte)(num2 >> 56 & 255L);
			array2[num - 2] = (byte)(num2 >> 48 & 255L);
			array2[num - 3] = (byte)(num2 >> 40 & 255L);
			array2[num - 4] = (byte)(num2 >> 32 & 255L);
			array2[num - 5] = (byte)(num2 >> 24 & 255L);
			array2[num - 6] = (byte)(num2 >> 16 & 255L);
			array2[num - 7] = (byte)(num2 >> 8 & 255L);
			array2[num - 8] = (byte)(num2 & 255L);
			this._HashData(array2, 0, array2.Length);
			Utils.DWORDToLittleEndian(array, this._stateMD160, 5);
			this.HashValue = array;
			return array;
		}

		[SecurityCritical]
		private unsafe static void MDTransform(uint* blockDWords, uint* state, byte* block)
		{
			uint num = *state;
			uint num2 = state[1];
			uint num3 = state[2];
			uint num4 = state[3];
			uint num5 = state[4];
			uint num6 = num;
			uint num7 = num2;
			uint num8 = num3;
			uint num9 = num4;
			uint num10 = num5;
			Utils.DWORDFromLittleEndian(blockDWords, 16, block);
			num += *blockDWords + RIPEMD160Managed.F(num2, num3, num4);
			num = (num << 11 | num >> 21) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += blockDWords[1] + RIPEMD160Managed.F(num, num2, num3);
			num5 = (num5 << 14 | num5 >> 18) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += blockDWords[2] + RIPEMD160Managed.F(num5, num, num2);
			num4 = (num4 << 15 | num4 >> 17) + num3;
			num = (num << 10 | num >> 22);
			num3 += blockDWords[3] + RIPEMD160Managed.F(num4, num5, num);
			num3 = (num3 << 12 | num3 >> 20) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += blockDWords[4] + RIPEMD160Managed.F(num3, num4, num5);
			num2 = (num2 << 5 | num2 >> 27) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += blockDWords[5] + RIPEMD160Managed.F(num2, num3, num4);
			num = (num << 8 | num >> 24) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += blockDWords[6] + RIPEMD160Managed.F(num, num2, num3);
			num5 = (num5 << 7 | num5 >> 25) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += blockDWords[7] + RIPEMD160Managed.F(num5, num, num2);
			num4 = (num4 << 9 | num4 >> 23) + num3;
			num = (num << 10 | num >> 22);
			num3 += blockDWords[8] + RIPEMD160Managed.F(num4, num5, num);
			num3 = (num3 << 11 | num3 >> 21) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += blockDWords[9] + RIPEMD160Managed.F(num3, num4, num5);
			num2 = (num2 << 13 | num2 >> 19) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += blockDWords[10] + RIPEMD160Managed.F(num2, num3, num4);
			num = (num << 14 | num >> 18) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += blockDWords[11] + RIPEMD160Managed.F(num, num2, num3);
			num5 = (num5 << 15 | num5 >> 17) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += blockDWords[12] + RIPEMD160Managed.F(num5, num, num2);
			num4 = (num4 << 6 | num4 >> 26) + num3;
			num = (num << 10 | num >> 22);
			num3 += blockDWords[13] + RIPEMD160Managed.F(num4, num5, num);
			num3 = (num3 << 7 | num3 >> 25) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += blockDWords[14] + RIPEMD160Managed.F(num3, num4, num5);
			num2 = (num2 << 9 | num2 >> 23) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += blockDWords[15] + RIPEMD160Managed.F(num2, num3, num4);
			num = (num << 8 | num >> 24) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.G(num, num2, num3) + blockDWords[7] + 1518500249U;
			num5 = (num5 << 7 | num5 >> 25) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.G(num5, num, num2) + blockDWords[4] + 1518500249U;
			num4 = (num4 << 6 | num4 >> 26) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.G(num4, num5, num) + blockDWords[13] + 1518500249U;
			num3 = (num3 << 8 | num3 >> 24) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.G(num3, num4, num5) + blockDWords[1] + 1518500249U;
			num2 = (num2 << 13 | num2 >> 19) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.G(num2, num3, num4) + blockDWords[10] + 1518500249U;
			num = (num << 11 | num >> 21) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.G(num, num2, num3) + blockDWords[6] + 1518500249U;
			num5 = (num5 << 9 | num5 >> 23) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.G(num5, num, num2) + blockDWords[15] + 1518500249U;
			num4 = (num4 << 7 | num4 >> 25) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.G(num4, num5, num) + blockDWords[3] + 1518500249U;
			num3 = (num3 << 15 | num3 >> 17) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.G(num3, num4, num5) + blockDWords[12] + 1518500249U;
			num2 = (num2 << 7 | num2 >> 25) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.G(num2, num3, num4) + *blockDWords + 1518500249U;
			num = (num << 12 | num >> 20) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.G(num, num2, num3) + blockDWords[9] + 1518500249U;
			num5 = (num5 << 15 | num5 >> 17) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.G(num5, num, num2) + blockDWords[5] + 1518500249U;
			num4 = (num4 << 9 | num4 >> 23) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.G(num4, num5, num) + blockDWords[2] + 1518500249U;
			num3 = (num3 << 11 | num3 >> 21) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.G(num3, num4, num5) + blockDWords[14] + 1518500249U;
			num2 = (num2 << 7 | num2 >> 25) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.G(num2, num3, num4) + blockDWords[11] + 1518500249U;
			num = (num << 13 | num >> 19) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.G(num, num2, num3) + blockDWords[8] + 1518500249U;
			num5 = (num5 << 12 | num5 >> 20) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.H(num5, num, num2) + blockDWords[3] + 1859775393U;
			num4 = (num4 << 11 | num4 >> 21) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.H(num4, num5, num) + blockDWords[10] + 1859775393U;
			num3 = (num3 << 13 | num3 >> 19) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.H(num3, num4, num5) + blockDWords[14] + 1859775393U;
			num2 = (num2 << 6 | num2 >> 26) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.H(num2, num3, num4) + blockDWords[4] + 1859775393U;
			num = (num << 7 | num >> 25) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.H(num, num2, num3) + blockDWords[9] + 1859775393U;
			num5 = (num5 << 14 | num5 >> 18) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.H(num5, num, num2) + blockDWords[15] + 1859775393U;
			num4 = (num4 << 9 | num4 >> 23) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.H(num4, num5, num) + blockDWords[8] + 1859775393U;
			num3 = (num3 << 13 | num3 >> 19) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.H(num3, num4, num5) + blockDWords[1] + 1859775393U;
			num2 = (num2 << 15 | num2 >> 17) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.H(num2, num3, num4) + blockDWords[2] + 1859775393U;
			num = (num << 14 | num >> 18) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.H(num, num2, num3) + blockDWords[7] + 1859775393U;
			num5 = (num5 << 8 | num5 >> 24) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.H(num5, num, num2) + *blockDWords + 1859775393U;
			num4 = (num4 << 13 | num4 >> 19) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.H(num4, num5, num) + blockDWords[6] + 1859775393U;
			num3 = (num3 << 6 | num3 >> 26) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.H(num3, num4, num5) + blockDWords[13] + 1859775393U;
			num2 = (num2 << 5 | num2 >> 27) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.H(num2, num3, num4) + blockDWords[11] + 1859775393U;
			num = (num << 12 | num >> 20) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.H(num, num2, num3) + blockDWords[5] + 1859775393U;
			num5 = (num5 << 7 | num5 >> 25) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.H(num5, num, num2) + blockDWords[12] + 1859775393U;
			num4 = (num4 << 5 | num4 >> 27) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.I(num4, num5, num) + blockDWords[1] + 2400959708U;
			num3 = (num3 << 11 | num3 >> 21) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.I(num3, num4, num5) + blockDWords[9] + 2400959708U;
			num2 = (num2 << 12 | num2 >> 20) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.I(num2, num3, num4) + blockDWords[11] + 2400959708U;
			num = (num << 14 | num >> 18) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.I(num, num2, num3) + blockDWords[10] + 2400959708U;
			num5 = (num5 << 15 | num5 >> 17) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.I(num5, num, num2) + *blockDWords + 2400959708U;
			num4 = (num4 << 14 | num4 >> 18) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.I(num4, num5, num) + blockDWords[8] + 2400959708U;
			num3 = (num3 << 15 | num3 >> 17) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.I(num3, num4, num5) + blockDWords[12] + 2400959708U;
			num2 = (num2 << 9 | num2 >> 23) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.I(num2, num3, num4) + blockDWords[4] + 2400959708U;
			num = (num << 8 | num >> 24) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.I(num, num2, num3) + blockDWords[13] + 2400959708U;
			num5 = (num5 << 9 | num5 >> 23) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.I(num5, num, num2) + blockDWords[3] + 2400959708U;
			num4 = (num4 << 14 | num4 >> 18) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.I(num4, num5, num) + blockDWords[7] + 2400959708U;
			num3 = (num3 << 5 | num3 >> 27) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.I(num3, num4, num5) + blockDWords[15] + 2400959708U;
			num2 = (num2 << 6 | num2 >> 26) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.I(num2, num3, num4) + blockDWords[14] + 2400959708U;
			num = (num << 8 | num >> 24) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.I(num, num2, num3) + blockDWords[5] + 2400959708U;
			num5 = (num5 << 6 | num5 >> 26) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.I(num5, num, num2) + blockDWords[6] + 2400959708U;
			num4 = (num4 << 5 | num4 >> 27) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.I(num4, num5, num) + blockDWords[2] + 2400959708U;
			num3 = (num3 << 12 | num3 >> 20) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.J(num3, num4, num5) + blockDWords[4] + 2840853838U;
			num2 = (num2 << 9 | num2 >> 23) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.J(num2, num3, num4) + *blockDWords + 2840853838U;
			num = (num << 15 | num >> 17) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.J(num, num2, num3) + blockDWords[5] + 2840853838U;
			num5 = (num5 << 5 | num5 >> 27) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.J(num5, num, num2) + blockDWords[9] + 2840853838U;
			num4 = (num4 << 11 | num4 >> 21) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.J(num4, num5, num) + blockDWords[7] + 2840853838U;
			num3 = (num3 << 6 | num3 >> 26) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.J(num3, num4, num5) + blockDWords[12] + 2840853838U;
			num2 = (num2 << 8 | num2 >> 24) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.J(num2, num3, num4) + blockDWords[2] + 2840853838U;
			num = (num << 13 | num >> 19) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.J(num, num2, num3) + blockDWords[10] + 2840853838U;
			num5 = (num5 << 12 | num5 >> 20) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.J(num5, num, num2) + blockDWords[14] + 2840853838U;
			num4 = (num4 << 5 | num4 >> 27) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.J(num4, num5, num) + blockDWords[1] + 2840853838U;
			num3 = (num3 << 12 | num3 >> 20) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.J(num3, num4, num5) + blockDWords[3] + 2840853838U;
			num2 = (num2 << 13 | num2 >> 19) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num += RIPEMD160Managed.J(num2, num3, num4) + blockDWords[8] + 2840853838U;
			num = (num << 14 | num >> 18) + num5;
			num3 = (num3 << 10 | num3 >> 22);
			num5 += RIPEMD160Managed.J(num, num2, num3) + blockDWords[11] + 2840853838U;
			num5 = (num5 << 11 | num5 >> 21) + num4;
			num2 = (num2 << 10 | num2 >> 22);
			num4 += RIPEMD160Managed.J(num5, num, num2) + blockDWords[6] + 2840853838U;
			num4 = (num4 << 8 | num4 >> 24) + num3;
			num = (num << 10 | num >> 22);
			num3 += RIPEMD160Managed.J(num4, num5, num) + blockDWords[15] + 2840853838U;
			num3 = (num3 << 5 | num3 >> 27) + num2;
			num5 = (num5 << 10 | num5 >> 22);
			num2 += RIPEMD160Managed.J(num3, num4, num5) + blockDWords[13] + 2840853838U;
			num2 = (num2 << 6 | num2 >> 26) + num;
			num4 = (num4 << 10 | num4 >> 22);
			num6 += RIPEMD160Managed.J(num7, num8, num9) + blockDWords[5] + 1352829926U;
			num6 = (num6 << 8 | num6 >> 24) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.J(num6, num7, num8) + blockDWords[14] + 1352829926U;
			num10 = (num10 << 9 | num10 >> 23) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.J(num10, num6, num7) + blockDWords[7] + 1352829926U;
			num9 = (num9 << 9 | num9 >> 23) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.J(num9, num10, num6) + *blockDWords + 1352829926U;
			num8 = (num8 << 11 | num8 >> 21) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.J(num8, num9, num10) + blockDWords[9] + 1352829926U;
			num7 = (num7 << 13 | num7 >> 19) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.J(num7, num8, num9) + blockDWords[2] + 1352829926U;
			num6 = (num6 << 15 | num6 >> 17) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.J(num6, num7, num8) + blockDWords[11] + 1352829926U;
			num10 = (num10 << 15 | num10 >> 17) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.J(num10, num6, num7) + blockDWords[4] + 1352829926U;
			num9 = (num9 << 5 | num9 >> 27) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.J(num9, num10, num6) + blockDWords[13] + 1352829926U;
			num8 = (num8 << 7 | num8 >> 25) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.J(num8, num9, num10) + blockDWords[6] + 1352829926U;
			num7 = (num7 << 7 | num7 >> 25) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.J(num7, num8, num9) + blockDWords[15] + 1352829926U;
			num6 = (num6 << 8 | num6 >> 24) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.J(num6, num7, num8) + blockDWords[8] + 1352829926U;
			num10 = (num10 << 11 | num10 >> 21) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.J(num10, num6, num7) + blockDWords[1] + 1352829926U;
			num9 = (num9 << 14 | num9 >> 18) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.J(num9, num10, num6) + blockDWords[10] + 1352829926U;
			num8 = (num8 << 14 | num8 >> 18) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.J(num8, num9, num10) + blockDWords[3] + 1352829926U;
			num7 = (num7 << 12 | num7 >> 20) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.J(num7, num8, num9) + blockDWords[12] + 1352829926U;
			num6 = (num6 << 6 | num6 >> 26) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.I(num6, num7, num8) + blockDWords[6] + 1548603684U;
			num10 = (num10 << 9 | num10 >> 23) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.I(num10, num6, num7) + blockDWords[11] + 1548603684U;
			num9 = (num9 << 13 | num9 >> 19) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.I(num9, num10, num6) + blockDWords[3] + 1548603684U;
			num8 = (num8 << 15 | num8 >> 17) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.I(num8, num9, num10) + blockDWords[7] + 1548603684U;
			num7 = (num7 << 7 | num7 >> 25) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.I(num7, num8, num9) + *blockDWords + 1548603684U;
			num6 = (num6 << 12 | num6 >> 20) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.I(num6, num7, num8) + blockDWords[13] + 1548603684U;
			num10 = (num10 << 8 | num10 >> 24) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.I(num10, num6, num7) + blockDWords[5] + 1548603684U;
			num9 = (num9 << 9 | num9 >> 23) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.I(num9, num10, num6) + blockDWords[10] + 1548603684U;
			num8 = (num8 << 11 | num8 >> 21) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.I(num8, num9, num10) + blockDWords[14] + 1548603684U;
			num7 = (num7 << 7 | num7 >> 25) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.I(num7, num8, num9) + blockDWords[15] + 1548603684U;
			num6 = (num6 << 7 | num6 >> 25) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.I(num6, num7, num8) + blockDWords[8] + 1548603684U;
			num10 = (num10 << 12 | num10 >> 20) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.I(num10, num6, num7) + blockDWords[12] + 1548603684U;
			num9 = (num9 << 7 | num9 >> 25) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.I(num9, num10, num6) + blockDWords[4] + 1548603684U;
			num8 = (num8 << 6 | num8 >> 26) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.I(num8, num9, num10) + blockDWords[9] + 1548603684U;
			num7 = (num7 << 15 | num7 >> 17) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.I(num7, num8, num9) + blockDWords[1] + 1548603684U;
			num6 = (num6 << 13 | num6 >> 19) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.I(num6, num7, num8) + blockDWords[2] + 1548603684U;
			num10 = (num10 << 11 | num10 >> 21) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.H(num10, num6, num7) + blockDWords[15] + 1836072691U;
			num9 = (num9 << 9 | num9 >> 23) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.H(num9, num10, num6) + blockDWords[5] + 1836072691U;
			num8 = (num8 << 7 | num8 >> 25) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.H(num8, num9, num10) + blockDWords[1] + 1836072691U;
			num7 = (num7 << 15 | num7 >> 17) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.H(num7, num8, num9) + blockDWords[3] + 1836072691U;
			num6 = (num6 << 11 | num6 >> 21) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.H(num6, num7, num8) + blockDWords[7] + 1836072691U;
			num10 = (num10 << 8 | num10 >> 24) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.H(num10, num6, num7) + blockDWords[14] + 1836072691U;
			num9 = (num9 << 6 | num9 >> 26) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.H(num9, num10, num6) + blockDWords[6] + 1836072691U;
			num8 = (num8 << 6 | num8 >> 26) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.H(num8, num9, num10) + blockDWords[9] + 1836072691U;
			num7 = (num7 << 14 | num7 >> 18) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.H(num7, num8, num9) + blockDWords[11] + 1836072691U;
			num6 = (num6 << 12 | num6 >> 20) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.H(num6, num7, num8) + blockDWords[8] + 1836072691U;
			num10 = (num10 << 13 | num10 >> 19) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.H(num10, num6, num7) + blockDWords[12] + 1836072691U;
			num9 = (num9 << 5 | num9 >> 27) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.H(num9, num10, num6) + blockDWords[2] + 1836072691U;
			num8 = (num8 << 14 | num8 >> 18) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.H(num8, num9, num10) + blockDWords[10] + 1836072691U;
			num7 = (num7 << 13 | num7 >> 19) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.H(num7, num8, num9) + *blockDWords + 1836072691U;
			num6 = (num6 << 13 | num6 >> 19) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.H(num6, num7, num8) + blockDWords[4] + 1836072691U;
			num10 = (num10 << 7 | num10 >> 25) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.H(num10, num6, num7) + blockDWords[13] + 1836072691U;
			num9 = (num9 << 5 | num9 >> 27) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.G(num9, num10, num6) + blockDWords[8] + 2053994217U;
			num8 = (num8 << 15 | num8 >> 17) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.G(num8, num9, num10) + blockDWords[6] + 2053994217U;
			num7 = (num7 << 5 | num7 >> 27) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.G(num7, num8, num9) + blockDWords[4] + 2053994217U;
			num6 = (num6 << 8 | num6 >> 24) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.G(num6, num7, num8) + blockDWords[1] + 2053994217U;
			num10 = (num10 << 11 | num10 >> 21) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.G(num10, num6, num7) + blockDWords[3] + 2053994217U;
			num9 = (num9 << 14 | num9 >> 18) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.G(num9, num10, num6) + blockDWords[11] + 2053994217U;
			num8 = (num8 << 14 | num8 >> 18) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.G(num8, num9, num10) + blockDWords[15] + 2053994217U;
			num7 = (num7 << 6 | num7 >> 26) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.G(num7, num8, num9) + *blockDWords + 2053994217U;
			num6 = (num6 << 14 | num6 >> 18) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.G(num6, num7, num8) + blockDWords[5] + 2053994217U;
			num10 = (num10 << 6 | num10 >> 26) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.G(num10, num6, num7) + blockDWords[12] + 2053994217U;
			num9 = (num9 << 9 | num9 >> 23) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.G(num9, num10, num6) + blockDWords[2] + 2053994217U;
			num8 = (num8 << 12 | num8 >> 20) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.G(num8, num9, num10) + blockDWords[13] + 2053994217U;
			num7 = (num7 << 9 | num7 >> 23) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.G(num7, num8, num9) + blockDWords[9] + 2053994217U;
			num6 = (num6 << 12 | num6 >> 20) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.G(num6, num7, num8) + blockDWords[7] + 2053994217U;
			num10 = (num10 << 5 | num10 >> 27) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.G(num10, num6, num7) + blockDWords[10] + 2053994217U;
			num9 = (num9 << 15 | num9 >> 17) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.G(num9, num10, num6) + blockDWords[14] + 2053994217U;
			num8 = (num8 << 8 | num8 >> 24) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.F(num8, num9, num10) + blockDWords[12];
			num7 = (num7 << 8 | num7 >> 24) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.F(num7, num8, num9) + blockDWords[15];
			num6 = (num6 << 5 | num6 >> 27) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.F(num6, num7, num8) + blockDWords[10];
			num10 = (num10 << 12 | num10 >> 20) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.F(num10, num6, num7) + blockDWords[4];
			num9 = (num9 << 9 | num9 >> 23) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.F(num9, num10, num6) + blockDWords[1];
			num8 = (num8 << 12 | num8 >> 20) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.F(num8, num9, num10) + blockDWords[5];
			num7 = (num7 << 5 | num7 >> 27) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.F(num7, num8, num9) + blockDWords[8];
			num6 = (num6 << 14 | num6 >> 18) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.F(num6, num7, num8) + blockDWords[7];
			num10 = (num10 << 6 | num10 >> 26) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.F(num10, num6, num7) + blockDWords[6];
			num9 = (num9 << 8 | num9 >> 24) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.F(num9, num10, num6) + blockDWords[2];
			num8 = (num8 << 13 | num8 >> 19) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.F(num8, num9, num10) + blockDWords[13];
			num7 = (num7 << 6 | num7 >> 26) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num6 += RIPEMD160Managed.F(num7, num8, num9) + blockDWords[14];
			num6 = (num6 << 5 | num6 >> 27) + num10;
			num8 = (num8 << 10 | num8 >> 22);
			num10 += RIPEMD160Managed.F(num6, num7, num8) + *blockDWords;
			num10 = (num10 << 15 | num10 >> 17) + num9;
			num7 = (num7 << 10 | num7 >> 22);
			num9 += RIPEMD160Managed.F(num10, num6, num7) + blockDWords[3];
			num9 = (num9 << 13 | num9 >> 19) + num8;
			num6 = (num6 << 10 | num6 >> 22);
			num8 += RIPEMD160Managed.F(num9, num10, num6) + blockDWords[9];
			num8 = (num8 << 11 | num8 >> 21) + num7;
			num10 = (num10 << 10 | num10 >> 22);
			num7 += RIPEMD160Managed.F(num8, num9, num10) + blockDWords[11];
			num7 = (num7 << 11 | num7 >> 21) + num6;
			num9 = (num9 << 10 | num9 >> 22);
			num9 += num3 + state[1];
			state[1] = state[2] + num4 + num10;
			state[2] = state[3] + num5 + num6;
			state[3] = state[4] + num + num7;
			state[4] = *state + num2 + num8;
			*state = num9;
		}

		private static uint F(uint x, uint y, uint z)
		{
			return x ^ y ^ z;
		}

		private static uint G(uint x, uint y, uint z)
		{
			return (x & y) | (~x & z);
		}

		private static uint H(uint x, uint y, uint z)
		{
			return (x | ~y) ^ z;
		}

		private static uint I(uint x, uint y, uint z)
		{
			return (x & z) | (y & ~z);
		}

		private static uint J(uint x, uint y, uint z)
		{
			return x ^ (y | ~z);
		}

		private byte[] _buffer;

		private long _count;

		private uint[] _stateMD160;

		private uint[] _blockDWords;
	}
}
