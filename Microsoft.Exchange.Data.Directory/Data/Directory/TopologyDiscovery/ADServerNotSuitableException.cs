using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADServerNotSuitableException : ADOperationException
	{
		public ADServerNotSuitableException(LocalizedString message, string serverFqdn) : this(message)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("serverFqdn", serverFqdn);
			this.ServerFqdn = serverFqdn;
		}

		public ADServerNotSuitableException(LocalizedString message) : base(message)
		{
		}

		public ADServerNotSuitableException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ADServerNotSuitableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal string ServerFqdn { get; set; }

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
