using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MesoContainerNotFoundException : LocalizedException
	{
		public MesoContainerNotFoundException(string domain) : base(Strings.MesoContainerNotFoundException(domain))
		{
			this.domain = domain;
		}

		public MesoContainerNotFoundException(string domain, Exception innerException) : base(Strings.MesoContainerNotFoundException(domain), innerException)
		{
			this.domain = domain;
		}

		protected MesoContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
