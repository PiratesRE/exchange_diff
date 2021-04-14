using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SuitabilityException : ADTransientException
	{
		public SuitabilityException(LocalizedString message, string serverFqdn) : this(message)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("serverFqdn", serverFqdn);
			this.ServerFqdn = serverFqdn;
		}

		public SuitabilityException(LocalizedString message) : base(message)
		{
		}

		public SuitabilityException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SuitabilityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal string ServerFqdn { get; set; }

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
