using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Mdb
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ServerNotFoundException : ComponentFailedPermanentException
	{
		public ServerNotFoundException(string fqdn) : base(Strings.ServerNotFound(fqdn))
		{
			this.fqdn = fqdn;
		}

		public ServerNotFoundException(string fqdn, Exception innerException) : base(Strings.ServerNotFound(fqdn), innerException)
		{
			this.fqdn = fqdn;
		}

		protected ServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
