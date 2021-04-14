using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoReceiveConnectorsException : LocalizedException
	{
		public NoReceiveConnectorsException(string fqdn) : base(Strings.NoReceiveConnectors(fqdn))
		{
			this.fqdn = fqdn;
		}

		public NoReceiveConnectorsException(string fqdn, Exception innerException) : base(Strings.NoReceiveConnectors(fqdn), innerException)
		{
			this.fqdn = fqdn;
		}

		protected NoReceiveConnectorsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
