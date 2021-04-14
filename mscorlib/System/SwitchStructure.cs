using System;

namespace System
{
	internal struct SwitchStructure
	{
		internal SwitchStructure(string n, int v)
		{
			this.name = n;
			this.value = v;
		}

		internal string name;

		internal int value;
	}
}
