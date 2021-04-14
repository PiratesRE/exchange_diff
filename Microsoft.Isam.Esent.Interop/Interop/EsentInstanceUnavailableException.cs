using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInstanceUnavailableException : EsentFatalException
	{
		public EsentInstanceUnavailableException() : base("This instance cannot be used because it encountered a fatal error", JET_err.InstanceUnavailable)
		{
		}

		private EsentInstanceUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
