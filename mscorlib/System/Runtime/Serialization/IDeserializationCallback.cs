using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface IDeserializationCallback
	{
		void OnDeserialization(object sender);
	}
}
