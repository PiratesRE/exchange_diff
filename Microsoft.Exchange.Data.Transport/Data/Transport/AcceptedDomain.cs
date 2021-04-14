using System;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class AcceptedDomain
	{
		public abstract bool IsInCorporation { get; }

		public abstract bool UseAddressBook { get; }

		public abstract string NameSpecification { get; }

		public abstract Guid TenantId { get; }
	}
}
