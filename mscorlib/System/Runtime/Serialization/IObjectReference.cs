using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface IObjectReference
	{
		[SecurityCritical]
		object GetRealObject(StreamingContext context);
	}
}
