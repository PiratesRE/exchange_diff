using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidFqdnUnderExchangeServerAuthException : LocalizedException
	{
		public InvalidFqdnUnderExchangeServerAuthException(string fqdn, string netbios) : base(Strings.InvalidFqdnUnderExchangeServerAuth(fqdn, netbios))
		{
			this.fqdn = fqdn;
			this.netbios = netbios;
		}

		public InvalidFqdnUnderExchangeServerAuthException(string fqdn, string netbios, Exception innerException) : base(Strings.InvalidFqdnUnderExchangeServerAuth(fqdn, netbios), innerException)
		{
			this.fqdn = fqdn;
			this.netbios = netbios;
		}

		protected InvalidFqdnUnderExchangeServerAuthException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fqdn = (string)info.GetValue("fqdn", typeof(string));
			this.netbios = (string)info.GetValue("netbios", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fqdn", this.fqdn);
			info.AddValue("netbios", this.netbios);
		}

		public string Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		public string Netbios
		{
			get
			{
				return this.netbios;
			}
		}

		private readonly string fqdn;

		private readonly string netbios;
	}
}
