using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Storage
{
	internal static class ParseRecipientHelper
	{
		internal static string[] ParseRecipientChunk(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Length == 0)
			{
				return new string[0];
			}
			int length = address.Length;
			List<string> list = new List<string>(length / ParseRecipientHelper.averageRecipientLength);
			int i = 0;
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			while (i < length)
			{
				char c = address[i];
				if (c != '"')
				{
					if (c != ';')
					{
						switch (c)
						{
						case '[':
							if (!flag2)
							{
								flag = true;
							}
							break;
						case ']':
							if (flag && !flag2)
							{
								flag = false;
							}
							break;
						}
					}
					else if (!flag && !flag2)
					{
						list.Add(address.Substring(num, i - num));
						num = i + 1;
					}
				}
				else
				{
					flag2 = !flag2;
				}
				i++;
			}
			if (num < i)
			{
				list.Add(address.Substring(num, length - num));
			}
			int num2 = length / list.Count;
			if (num2 < ParseRecipientHelper.averageRecipientLength)
			{
				ParseRecipientHelper.averageRecipientLength = num2;
			}
			return list.ToArray();
		}

		private static int averageRecipientLength = int.MaxValue;
	}
}
