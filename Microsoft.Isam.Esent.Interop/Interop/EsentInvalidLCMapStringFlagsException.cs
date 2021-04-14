using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidLCMapStringFlagsException : EsentUsageException
	{
		public EsentInvalidLCMapStringFlagsException() : base("Invalid flags for LCMapString()", JET_err.InvalidLCMapStringFlags)
		{
		}

		private EsentInvalidLCMapStringFlagsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
