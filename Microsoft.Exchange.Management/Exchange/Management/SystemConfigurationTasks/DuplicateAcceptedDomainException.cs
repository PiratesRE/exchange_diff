using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DuplicateAcceptedDomainException : ADObjectAlreadyExistsException
	{
		public DuplicateAcceptedDomainException(string domain) : base(Strings.DuplicateAcceptedDomain(domain))
		{
			this.domain = domain;
		}

		public DuplicateAcceptedDomainException(string domain, Exception innerException) : base(Strings.DuplicateAcceptedDomain(domain), innerException)
		{
			this.domain = domain;
		}

		protected DuplicateAcceptedDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
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
