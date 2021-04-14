using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyTestInjectionsException : EsentUsageException
	{
		public EsentTooManyTestInjectionsException() : base("Internal test injection limit hit", JET_err.TooManyTestInjections)
		{
		}

		private EsentTooManyTestInjectionsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
