using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class WatermarkMap : Dictionary<Guid, long>
	{
		public static WatermarkMap Empty
		{
			get
			{
				return new WatermarkMap();
			}
		}

		internal static WatermarkMap Parse(string rawstring)
		{
			WatermarkMap empty = WatermarkMap.Empty;
			string[] array = rawstring.Split(new string[]
			{
				";"
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				string[] array3 = text.Split(new string[]
				{
					":"
				}, StringSplitOptions.None);
				if (array3.Length != 2)
				{
					throw new FormatException();
				}
				Guid key = new Guid(array3[0]);
				long value = long.Parse(array3[1]);
				if (!empty.ContainsKey(key))
				{
					empty.Add(key, value);
				}
			}
			return empty;
		}

		internal static WatermarkMap Parse(byte[] rawBytes)
		{
			string @string = Encoding.UTF8.GetString(rawBytes);
			return WatermarkMap.Parse(@string);
		}

		internal string SerializeToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.Count * 56);
			foreach (KeyValuePair<Guid, long> keyValuePair in this)
			{
				stringBuilder.AppendFormat("{0}:{1};", keyValuePair.Key, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		internal byte[] SerializeToBytes()
		{
			string s = this.SerializeToString();
			return Encoding.UTF8.GetBytes(s);
		}

		internal bool ContainsAllChanges(WatermarkMap source)
		{
			foreach (KeyValuePair<Guid, long> keyValuePair in source)
			{
				long num;
				if (!base.TryGetValue(keyValuePair.Key, out num))
				{
					return false;
				}
				if (num < keyValuePair.Value)
				{
					return false;
				}
			}
			return true;
		}
	}
}
