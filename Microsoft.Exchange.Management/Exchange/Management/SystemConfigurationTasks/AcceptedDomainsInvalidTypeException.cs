using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcceptedDomainsInvalidTypeException : FederationException
	{
		public AcceptedDomainsInvalidTypeException(string domain) : base(Strings.ErrorAcceptedDomainsInvalidType(domain))
		{
			this.domain = domain;
		}

		public AcceptedDomainsInvalidTypeException(string domain, Exception innerException) : base(Strings.ErrorAcceptedDomainsInvalidType(domain), innerException)
		{
			this.domain = domain;
		}

		protected AcceptedDomainsInvalidTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
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
