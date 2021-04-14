using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDefaultValueTooBigException : EsentUsageException
	{
		public EsentDefaultValueTooBigException() : base("Default value exceeds maximum size", JET_err.DefaultValueTooBig)
		{
		}

		private EsentDefaultValueTooBigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
