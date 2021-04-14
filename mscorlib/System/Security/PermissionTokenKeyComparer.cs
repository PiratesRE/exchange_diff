using System;
using System.Collections;
using System.Globalization;

namespace System.Security
{
	[Serializable]
	internal sealed class PermissionTokenKeyComparer : IEqualityComparer
	{
		public PermissionTokenKeyComparer()
		{
			this._caseSensitiveComparer = new Comparer(CultureInfo.InvariantCulture);
			this._info = CultureInfo.InvariantCulture.TextInfo;
		}

		[SecuritySafeCritical]
		public int Compare(object a, object b)
		{
			string text = a as string;
			string text2 = b as string;
			if (text == null || text2 == null)
			{
				return this._caseSensitiveComparer.Compare(a, b);
			}
			int num = this._caseSensitiveComparer.Compare(a, b);
			if (num == 0)
			{
				return 0;
			}
			if (SecurityManager.IsSameType(text, text2))
			{
				return 0;
			}
			return num;
		}

		public bool Equals(object a, object b)
		{
			return a == b || (a != null && b != null && this.Compare(a, b) == 0);
		}

		public int GetHashCode(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			string text = obj as string;
			if (text == null)
			{
				return obj.GetHashCode();
			}
			int num = text.IndexOf(',');
			if (num == -1)
			{
				num = text.Length;
			}
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				num2 = (num2 << 7 ^ (int)text[i] ^ num2 >> 25);
			}
			return num2;
		}

		private Comparer _caseSensitiveComparer;

		private TextInfo _info;
	}
}
