using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DuplicateX400DomainException : LocalizedException
	{
		public DuplicateX400DomainException(X400Domain domain) : base(Strings.DuplicateX400Domain(domain))
		{
			this.domain = domain;
		}

		public DuplicateX400DomainException(X400Domain domain, Exception innerException) : base(Strings.DuplicateX400Domain(domain), innerException)
		{
			this.domain = domain;
		}

		protected DuplicateX400DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domain = (X400Domain)info.GetValue("domain", typeof(X400Domain));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domain", this.domain);
		}

		public X400Domain Domain
		{
			get
			{
				return this.domain;
			}
		}

		private readonly X400Domain domain;
	}
}
