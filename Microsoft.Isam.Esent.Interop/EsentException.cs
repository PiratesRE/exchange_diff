using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent
{
	[Serializable]
	public abstract class EsentException : Exception
	{
		protected EsentException()
		{
		}

		protected EsentException(string message) : base(message)
		{
		}

		protected EsentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
