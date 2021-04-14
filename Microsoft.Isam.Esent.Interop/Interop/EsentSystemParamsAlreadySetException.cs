using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSystemParamsAlreadySetException : EsentStateException
	{
		public EsentSystemParamsAlreadySetException() : base("Global system parameters have already been set", JET_err.SystemParamsAlreadySet)
		{
		}

		private EsentSystemParamsAlreadySetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
