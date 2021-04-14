using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidSiteForOrganizationException : LocalizedException
	{
		public InvalidSiteForOrganizationException(string organization, string redirectionUri) : base(Strings.InvalidSiteForOrganizationMessage(organization, redirectionUri))
		{
			this.organization = organization;
			this.redirectionUri = redirectionUri;
		}

		public InvalidSiteForOrganizationException(string organization, string redirectionUri, Exception innerException) : base(Strings.InvalidSiteForOrganizationMessage(organization, redirectionUri), innerException)
		{
			this.organization = organization;
			this.redirectionUri = redirectionUri;
		}

		protected InvalidSiteForOrganizationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.organization = (string)info.GetValue("organization", typeof(string));
			this.redirectionUri = (string)info.GetValue("redirectionUri", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("organization", this.organization);
			info.AddValue("redirectionUri", this.redirectionUri);
		}

		public string Organization
		{
			get
			{
				return this.organization;
			}
		}

		public string RedirectionUri
		{
			get
			{
				return this.redirectionUri;
			}
		}

		private readonly string organization;

		private readonly string redirectionUri;
	}
}
