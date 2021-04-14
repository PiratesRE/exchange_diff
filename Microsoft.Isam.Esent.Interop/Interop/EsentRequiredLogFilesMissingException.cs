using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRequiredLogFilesMissingException : EsentInconsistentException
	{
		public EsentRequiredLogFilesMissingException() : base("The required log files for recovery is missing.", JET_err.RequiredLogFilesMissing)
		{
		}

		private EsentRequiredLogFilesMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
