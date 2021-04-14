using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ContainerNotFoundException : LocalizedException
	{
		public ContainerNotFoundException(string domain) : base(Strings.ContainerNotFoundException(domain))
		{
			this.domain = domain;
		}

		public ContainerNotFoundException(string domain, Exception innerException) : base(Strings.ContainerNotFoundException(domain), innerException)
		{
			this.domain = domain;
		}

		protected ContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domain = (string)info.GetValue("domain", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domain", this.domain);
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		private readonly string domain;
	}
}
