using System;

namespace System.Runtime.Remoting.Proxies
{
	[Serializable]
	internal enum CallType
	{
		InvalidCall,
		MethodCall,
		ConstructorCall
	}
}
