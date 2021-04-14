using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorNoCASServersInSitePermanentException : MailboxReplicationPermanentException
	{
		public ErrorNoCASServersInSitePermanentException(string site, string minVersion) : base(MrsStrings.ErrorNoCASServersInSite(site, minVersion))
		{
			this.site = site;
			this.minVersion = minVersion;
		}

		public ErrorNoCASServersInSitePermanentException(string site, string minVersion, Exception innerException) : base(MrsStrings.ErrorNoCASServersInSite(site, minVersion), innerException)
		{
			this.site = site;
			this.minVersion = minVersion;
		}

		protected ErrorNoCASServersInSitePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.site = (string)info.GetValue("site", typeof(string));
			this.minVersion = (string)info.GetValue("minVersion", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("site", this.site);
			info.AddValue("minVersion", this.minVersion);
		}

		public string Site
		{
			get
			{
				return this.site;
			}
		}

		public string MinVersion
		{
			get
			{
				return this.minVersion;
			}
		}

		private readonly string site;

		private readonly string minVersion;
	}
}
