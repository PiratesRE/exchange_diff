using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidFqdnException : LocalizedException
	{
		public InvalidFqdnException(string fqdn) : base(Strings.InvalidFqdn(fqdn))
		{
			this.fqdn = fqdn;
		}

		public InvalidFqdnException(string fqdn, Exception innerException) : base(Strings.InvalidFqdn(fqdn), innerException)
		{
			this.fqdn = fqdn;
		}

		protected InvalidFqdnException(SerializationInfo info, StreamingContext context) : base(info, context)
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
