using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogGenerationMismatchException : EsentInconsistentException
	{
		public EsentLogGenerationMismatchException() : base("Name of logfile does not match internal generation number", JET_err.LogGenerationMismatch)
		{
		}

		private EsentLogGenerationMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
