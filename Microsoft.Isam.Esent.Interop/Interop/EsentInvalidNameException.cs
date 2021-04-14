using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidNameException : EsentUsageException
	{
		public EsentInvalidNameException() : base("Invalid name", JET_err.InvalidName)
		{
		}

		private EsentInvalidNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
