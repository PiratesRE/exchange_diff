using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class WrongServerException : ConnectionFailedPermanentException
	{
		public Guid MdbGuid { get; private set; }

		public string RightServerFqdn { get; private set; }

		public int RightServerVersion { get; private set; }

		public WrongServerException(LocalizedString message) : base(message)
		{
		}

		public WrongServerException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		public WrongServerException(LocalizedString message, Guid mdbGuid, string rightServerFqdn, int rightServerVersion, Exception innerException = null) : base(message, innerException)
		{
			this.MdbGuid = mdbGuid;
			this.RightServerFqdn = rightServerFqdn;
			this.RightServerVersion = rightServerVersion;
		}

		protected WrongServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public string RightServerToString()
		{
			if (string.IsNullOrEmpty(this.RightServerFqdn))
			{
				return string.Empty;
			}
			return string.Format("{0}~{1}~{2}", this.MdbGuid, this.RightServerFqdn, this.RightServerVersion);
		}
	}
}
