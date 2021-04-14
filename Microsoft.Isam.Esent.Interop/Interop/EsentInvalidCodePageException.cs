using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidCodePageException : EsentUsageException
	{
		public EsentInvalidCodePageException() : base("Invalid or unknown code page", JET_err.InvalidCodePage)
		{
		}

		private EsentInvalidCodePageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
