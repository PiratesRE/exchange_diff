using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public delegate void AppDomainInitializer(string[] args);
}
