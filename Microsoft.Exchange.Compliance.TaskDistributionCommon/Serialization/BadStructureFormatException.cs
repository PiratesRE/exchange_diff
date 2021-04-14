using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization
{
	[Serializable]
	public class BadStructureFormatException : Exception
	{
		public BadStructureFormatException()
		{
		}

		public BadStructureFormatException(string message) : base(message)
		{
		}

		public BadStructureFormatException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected BadStructureFormatException(SerializationInfo info, StreamingContext context)
		{
		}
	}
}
