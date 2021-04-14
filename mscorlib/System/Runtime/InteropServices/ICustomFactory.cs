using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	public interface ICustomFactory
	{
		MarshalByRefObject CreateInstance(Type serverType);
	}
}
