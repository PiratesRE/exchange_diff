using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfAutoincrementValuesException : EsentFragmentationException
	{
		public EsentOutOfAutoincrementValuesException() : base("Auto-increment counter has reached maximum value (offline defrag WILL NOT be able to reclaim free/unused Auto-increment values).", JET_err.OutOfAutoincrementValues)
		{
		}

		private EsentOutOfAutoincrementValuesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
