using System;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class RemoteDomain
	{
		public abstract string NameSpecification { get; }

		public abstract string NonMimeCharset { get; }

		public abstract bool IsInternal { get; }
	}
}
