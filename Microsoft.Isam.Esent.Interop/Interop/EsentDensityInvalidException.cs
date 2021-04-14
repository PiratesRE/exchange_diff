using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDensityInvalidException : EsentUsageException
	{
		public EsentDensityInvalidException() : base("Bad file/index density", JET_err.DensityInvalid)
		{
		}

		private EsentDensityInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
