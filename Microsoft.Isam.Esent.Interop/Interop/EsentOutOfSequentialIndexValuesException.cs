using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfSequentialIndexValuesException : EsentFragmentationException
	{
		public EsentOutOfSequentialIndexValuesException() : base("Sequential index counter has reached maximum value (perform offline defrag to reclaim free/unused SequentialIndex values)", JET_err.OutOfSequentialIndexValues)
		{
		}

		private EsentOutOfSequentialIndexValuesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
