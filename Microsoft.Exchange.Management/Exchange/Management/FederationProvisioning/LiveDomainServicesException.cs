using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	[Serializable]
	internal sealed class LiveDomainServicesException : FederationException
	{
		public DomainError? DomainError { get; set; }

		public LiveDomainServicesException() : base(LocalizedString.Empty)
		{
		}

		public LiveDomainServicesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public LiveDomainServicesException(LocalizedString message) : base(message)
		{
		}

		public LiveDomainServicesException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		public LiveDomainServicesException(DomainError domainError, LocalizedString message) : base(message)
		{
			this.DomainError = new DomainError?(domainError);
		}

		public LiveDomainServicesException(DomainError domainError, LocalizedString message, Exception innerException) : base(message, innerException)
		{
			this.DomainError = new DomainError?(domainError);
		}
	}
}
