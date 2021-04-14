using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRemoveFederatedAcceptedDomainException : LocalizedException
	{
		public CannotRemoveFederatedAcceptedDomainException(string domain) : base(Strings.CannotRemoveFederatedAcceptedDomain(domain))
		{
			this.domain = domain;
		}

		public CannotRemoveFederatedAcceptedDomainException(string domain, Exception innerException) : base(Strings.CannotRemoveFederatedAcceptedDomain(domain), innerException)
		{
			this.domain = domain;
		}

		protected CannotRemoveFederatedAcceptedDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
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
