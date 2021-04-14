using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Diagnostics
{
	[Serializable]
	internal class ExAssertException : BaseException
	{
		public ExAssertException(string assertText) : base(assertText)
		{
		}

		private ExAssertException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
