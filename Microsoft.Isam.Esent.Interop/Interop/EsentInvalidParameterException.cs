using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidParameterException : EsentUsageException
	{
		public EsentInvalidParameterException() : base("Invalid API parameter", JET_err.InvalidParameter)
		{
		}

		private EsentInvalidParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
