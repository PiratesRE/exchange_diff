using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class USGContainerNotFoundException : LocalizedException
	{
		public USGContainerNotFoundException(string ou, string domain) : base(Strings.USGContainerNotFoundException(ou, domain))
		{
			this.ou = ou;
			this.domain = domain;
		}

		public USGContainerNotFoundException(string ou, string domain, Exception innerException) : base(Strings.USGContainerNotFoundException(ou, domain), innerException)
		{
			this.ou = ou;
			this.domain = domain;
		}

		protected USGContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ou = (string)info.GetValue("ou", typeof(string));
			this.domain = (string)info.GetValue("domain", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ou", this.ou);
			info.AddValue("domain", this.domain);
		}

		public string Ou
		{
			get
			{
				return this.ou;
			}
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		private readonly string ou;

		private readonly string domain;
	}
}
