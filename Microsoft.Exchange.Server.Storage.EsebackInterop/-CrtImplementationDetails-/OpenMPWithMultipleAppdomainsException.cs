using System;
using System.Runtime.Serialization;

namespace <CrtImplementationDetails>
{
	[Serializable]
	internal class OpenMPWithMultipleAppdomainsException : Exception
	{
		protected OpenMPWithMultipleAppdomainsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public OpenMPWithMultipleAppdomainsException()
		{
		}
	}
}
