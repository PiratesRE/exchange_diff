using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfDbtimeValuesException : EsentFragmentationException
	{
		public EsentOutOfDbtimeValuesException() : base("Dbtime counter has reached maximum value (perform offline defrag to reclaim free/unused Dbtime values)", JET_err.OutOfDbtimeValues)
		{
		}

		private EsentOutOfDbtimeValuesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
