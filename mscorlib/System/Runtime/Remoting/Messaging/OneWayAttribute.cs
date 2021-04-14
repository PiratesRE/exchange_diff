using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[AttributeUsage(AttributeTargets.Method)]
	[ComVisible(true)]
	public class OneWayAttribute : Attribute
	{
	}
}
