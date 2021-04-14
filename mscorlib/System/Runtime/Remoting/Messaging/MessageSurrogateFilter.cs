using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public delegate bool MessageSurrogateFilter(string key, object value);
}
