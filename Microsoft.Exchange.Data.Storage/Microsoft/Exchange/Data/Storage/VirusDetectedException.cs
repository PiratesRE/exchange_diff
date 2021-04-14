using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class VirusDetectedException : VirusException
	{
		public VirusDetectedException(LocalizedString message) : base(message)
		{
		}

		public VirusDetectedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected VirusDetectedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
