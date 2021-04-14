using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCallbackFailedException : EsentStateException
	{
		public EsentCallbackFailedException() : base("A callback failed", JET_err.CallbackFailed)
		{
		}

		private EsentCallbackFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
