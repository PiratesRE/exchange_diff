using System;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	[Serializable]
	internal class StringHasher
	{
		public StringHasher()
		{
			this.scenario = UsageScenario.Production;
		}

		public StringHasher(UsageScenario scenario)
		{
			this.scenario = scenario;
		}

		public ulong GetHash(string input)
		{
			byte[] array = new byte[input.Length];
			for (int i = 0; i < input.Length; i++)
			{
				byte b = (byte)input[i];
				if (b >= 65 && b <= 90)
				{
					b = b + 97 - 65;
				}
				array[i] = b;
			}
			return this.GetHash(array);
		}

		private ulong GetHash(byte[] input)
		{
			ulong num = 0UL;
			if (this.hasher == null && UsageScenario.Test != this.scenario)
			{
				this.hasher = new SHA256Cng();
			}
			if (this.hasher == null)
			{
				num = (ulong)input[0];
			}
			else
			{
				byte[] array = this.hasher.ComputeHash(input);
				for (int i = 0; i < 8; i++)
				{
					num <<= 8;
					num |= (ulong)array[i];
				}
			}
			return num;
		}

		private UsageScenario scenario;

		private SHA256Cng hasher;
	}
}
