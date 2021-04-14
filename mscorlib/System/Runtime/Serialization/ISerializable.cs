using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface ISerializable
	{
		[SecurityCritical]
		void GetObjectData(SerializationInfo info, StreamingContext context);
	}
}
