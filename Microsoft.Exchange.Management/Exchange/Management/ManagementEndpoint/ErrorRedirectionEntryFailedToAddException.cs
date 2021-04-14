using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ManagementEndpoint
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorRedirectionEntryFailedToAddException : LocalizedException
	{
		public ErrorRedirectionEntryFailedToAddException(string domainName) : base(Strings.ErrorRedirectionEntryFailedToAdd(domainName))
		{
			this.domainName = domainName;
		}

		public ErrorRedirectionEntryFailedToAddException(string domainName, Exception innerException) : base(Strings.ErrorRedirectionEntryFailedToAdd(domainName), innerException)
		{
			this.domainName = domainName;
		}

		protected ErrorRedirectionEntryFailedToAddException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domainName = (string)info.GetValue("domainName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domainName", this.domainName);
		}

		public string DomainName
		{
			get
			{
				return this.domainName;
			}
		}

		private readonly string domainName;
	}
}
