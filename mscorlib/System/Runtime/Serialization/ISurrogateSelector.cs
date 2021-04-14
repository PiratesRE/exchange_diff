using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface ISurrogateSelector
	{
		[SecurityCritical]
		void ChainSelector(ISurrogateSelector selector);

		[SecurityCritical]
		ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector);

		[SecurityCritical]
		ISurrogateSelector GetNextSelector();
	}
}
