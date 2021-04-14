using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecordTooBigForBackwardCompatibilityException : EsentStateException
	{
		public EsentRecordTooBigForBackwardCompatibilityException() : base("record would be too big if represented in a database format from a previous version of Jet", JET_err.RecordTooBigForBackwardCompatibility)
		{
		}

		private EsentRecordTooBigForBackwardCompatibilityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
