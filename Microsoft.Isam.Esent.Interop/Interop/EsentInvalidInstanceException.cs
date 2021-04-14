using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidInstanceException : EsentUsageException
	{
		public EsentInvalidInstanceException() : base("Invalid instance handle", JET_err.InvalidInstance)
		{
		}

		private EsentInvalidInstanceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
