using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class VirusMessageDeletedException : VirusException
	{
		public VirusMessageDeletedException(LocalizedString message) : base(message)
		{
		}

		public VirusMessageDeletedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected VirusMessageDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
