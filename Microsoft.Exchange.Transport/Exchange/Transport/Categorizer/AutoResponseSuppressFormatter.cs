using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class AutoResponseSuppressFormatter
	{
		public AutoResponseSuppressFormatter()
		{
			this.enumValueToStringMap = new Dictionary<int, string>(64);
			for (int i = 0; i < 64; i++)
			{
				this.enumValueToStringMap.Add(i, ((AutoResponseSuppress)i).ToString());
			}
		}

		public string Format(AutoResponseSuppress autoResponseSuppress)
		{
			string result;
			if (this.enumValueToStringMap.TryGetValue((int)autoResponseSuppress, out result))
			{
				return result;
			}
			int num = (int)autoResponseSuppress;
			return num.ToString();
		}

		private Dictionary<int, string> enumValueToStringMap;
	}
}
