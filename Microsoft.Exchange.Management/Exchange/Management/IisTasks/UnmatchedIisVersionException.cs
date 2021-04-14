using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.IisTasks
{
	[Serializable]
	public class UnmatchedIisVersionException : LocalizedException
	{
		public UnmatchedIisVersionException() : base(Strings.ExceptionInvalidIisVersion)
		{
		}

		public UnmatchedIisVersionException(LocalizedString message) : base(message)
		{
		}

		public UnmatchedIisVersionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected UnmatchedIisVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
