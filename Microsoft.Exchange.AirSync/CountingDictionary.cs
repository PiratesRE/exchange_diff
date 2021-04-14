using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.AirSync
{
	internal class CountingDictionary<T>
	{
		public int Increment(T key, int incrementBy = 1)
		{
			int result;
			lock (this.instanceLock)
			{
				int num;
				if (this.map.TryGetValue(key, out num))
				{
					num += incrementBy;
				}
				else
				{
					num = incrementBy;
				}
				this.map[key] = num;
				result = num;
			}
			return result;
		}

		public int GetCount(T key)
		{
			int result;
			lock (this.instanceLock)
			{
				int num;
				if (!this.map.TryGetValue(key, out num))
				{
					result = 0;
				}
				else
				{
					result = num;
				}
			}
			return result;
		}

		public void Clear()
		{
			lock (this.instanceLock)
			{
				this.map.Clear();
			}
		}

		public override string ToString()
		{
			string result;
			lock (this.instanceLock)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<T, int> keyValuePair in this.map)
				{
					stringBuilder.AppendFormat("{0}:{1} ", keyValuePair.Key, keyValuePair.Value);
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private Dictionary<T, int> map = new Dictionary<T, int>();

		private object instanceLock = new object();
	}
}
