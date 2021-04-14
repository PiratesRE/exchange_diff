using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerInMMException : SuitabilityException
	{
		public ServerInMMException(string fqdn) : base(DirectoryStrings.ErrorIsServerInMaintenanceMode(fqdn))
		{
			this.fqdn = fqdn;
		}

		public ServerInMMException(string fqdn, Exception innerException) : base(DirectoryStrings.ErrorIsServerInMaintenanceMode(fqdn), innerException)
		{
			this.fqdn = fqdn;
		}

		protected ServerInMMException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fqdn = (string)info.GetValue("fqdn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fqdn", this.fqdn);
		}

		public string Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		private readonly string fqdn;
	}
}
