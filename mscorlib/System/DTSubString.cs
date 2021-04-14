using System;

namespace System
{
	internal struct DTSubString
	{
		internal char this[int relativeIndex]
		{
			get
			{
				return this.s[this.index + relativeIndex];
			}
		}

		internal string s;

		internal int index;

		internal int length;

		internal DTSubStringType type;

		internal int value;
	}
}
