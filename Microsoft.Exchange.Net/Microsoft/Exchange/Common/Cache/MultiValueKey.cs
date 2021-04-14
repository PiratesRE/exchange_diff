using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common.Cache
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MultiValueKey
	{
		public MultiValueKey(params object[] keys)
		{
			if (keys == null || keys.Length == 0)
			{
				throw new ArgumentNullException("keys");
			}
			for (int i = 0; i < keys.Length; i++)
			{
				if (keys[i] == null)
				{
					throw new ArgumentNullException("keys", "One or more keys are null!");
				}
			}
			this.keys = keys;
			this.KeyLength = keys.Length;
		}

		public object GetKey(int index)
		{
			if (index < 0 || index >= this.KeyLength)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this.keys[index];
		}

		public override int GetHashCode()
		{
			int num = 23;
			foreach (object obj in this.keys)
			{
				num = num * 37 + obj.GetHashCode();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			MultiValueKey multiValueKey = obj as MultiValueKey;
			bool flag = true;
			bool flag2 = multiValueKey != null && multiValueKey.keys.Length == this.keys.Length;
			if (flag2)
			{
				for (int i = 0; i < this.keys.Length; i++)
				{
					if (!multiValueKey.keys[i].Equals(this.keys[i]))
					{
						flag = false;
						break;
					}
				}
			}
			return flag2 && flag;
		}

		public override string ToString()
		{
			char value = ',';
			char value2 = ' ';
			StringBuilder stringBuilder = new StringBuilder();
			int num = this.keys.Length;
			for (int i = 0; i < num; i++)
			{
				object obj = this.keys[i];
				stringBuilder.Append(obj.ToString());
				if (i < num - 1)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(value2);
				}
			}
			return stringBuilder.ToString();
		}

		public readonly int KeyLength;

		private readonly object[] keys;
	}
}
