using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public enum FileMode
	{
		CreateNew = 1,
		Create,
		Open,
		OpenOrCreate,
		Truncate,
		Append
	}
}
