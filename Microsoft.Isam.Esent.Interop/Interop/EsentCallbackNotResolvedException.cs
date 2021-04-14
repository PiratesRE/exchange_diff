using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCallbackNotResolvedException : EsentUsageException
	{
		public EsentCallbackNotResolvedException() : base("A callback function could not be found", JET_err.CallbackNotResolved)
		{
		}

		private EsentCallbackNotResolvedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
