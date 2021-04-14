using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToReleaseUriException : FederationException
	{
		public UnableToReleaseUriException(string uri, string domain, string appId, string errdetails) : base(Strings.ErrorUnableToReleaseUri(uri, domain, appId, errdetails))
		{
			this.uri = uri;
			this.domain = domain;
			this.appId = appId;
			this.errdetails = errdetails;
		}

		public UnableToReleaseUriException(string uri, string domain, string appId, string errdetails, Exception innerException) : base(Strings.ErrorUnableToReleaseUri(uri, domain, appId, errdetails), innerException)
		{
			this.uri = uri;
			this.domain = domain;
			this.appId = appId;
			this.errdetails = errdetails;
		}

		protected UnableToReleaseUriException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.uri = (string)info.GetValue("uri", typeof(string));
			this.domain = (string)info.GetValue("domain", typeof(string));
			this.appId = (string)info.GetValue("appId", typeof(string));
			this.errdetails = (string)info.GetValue("errdetails", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("uri", this.uri);
			info.AddValue("domain", this.domain);
			info.AddValue("appId", this.appId);
			info.AddValue("errdetails", this.errdetails);
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public string AppId
		{
			get
			{
				return this.appId;
			}
		}

		public string Errdetails
		{
			get
			{
				return this.errdetails;
			}
		}

		private readonly string uri;

		private readonly string domain;

		private readonly string appId;

		private readonly string errdetails;
	}
}
