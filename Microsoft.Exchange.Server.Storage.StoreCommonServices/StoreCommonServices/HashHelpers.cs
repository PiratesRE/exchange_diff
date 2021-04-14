using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class HashHelpers
	{
		public static int GetConversationTopicHash(string input)
		{
			return HashHelpers.GetInternetMessageIdHash(input);
		}

		public static int GetInternetMessageIdHash(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return 0;
			}
			uint num = 0U;
			string text = input.ToUpperInvariant();
			foreach (uint num2 in text)
			{
				num ^= num2;
				for (int j = 0; j < 16; j++)
				{
					uint num3 = num & 1U;
					num >>= 1;
					if (num3 != 0U)
					{
						num ^= 3988292384U;
					}
				}
			}
			return (int)num;
		}

		public static int GetConversationIdHash(byte[] inputBytes)
		{
			if (inputBytes == null || inputBytes.Length == 0)
			{
				return 0;
			}
			uint num = 0U;
			foreach (uint num2 in inputBytes)
			{
				num ^= num2;
				for (int j = 0; j < 8; j++)
				{
					uint num3 = num & 1U;
					num >>= 1;
					if (num3 != 0U)
					{
						num ^= 3988292384U;
					}
				}
			}
			return (int)num;
		}
	}
}
