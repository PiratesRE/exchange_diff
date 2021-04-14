using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTestInjectionNotSupportedException : EsentStateException
	{
		public EsentTestInjectionNotSupportedException() : base("Test injection not supported", JET_err.TestInjectionNotSupported)
		{
		}

		private EsentTestInjectionNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
