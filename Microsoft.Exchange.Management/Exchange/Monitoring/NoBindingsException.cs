using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoBindingsException : LocalizedException
	{
		public NoBindingsException(string fqdn) : base(Strings.NoBindings(fqdn))
		{
			this.fqdn = fqdn;
		}

		public NoBindingsException(string fqdn, Exception innerException) : base(Strings.NoBindings(fqdn), innerException)
		{
			this.fqdn = fqdn;
		}

		protected NoBindingsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
