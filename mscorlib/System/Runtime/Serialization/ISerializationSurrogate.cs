using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface ISerializationSurrogate
	{
		[SecurityCritical]
		void GetObjectData(object obj, SerializationInfo info, StreamingContext context);

		[SecurityCritical]
		object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector);
	}
}
