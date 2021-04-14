using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.EdgeSync.Common.Internal;

namespace Microsoft.Exchange.EdgeSync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EdgeSyncServiceConfigNotFoundException : LocalizedException
	{
		public EdgeSyncServiceConfigNotFoundException(string site, string dn) : base(Strings.EdgeSyncServiceConfigNotFoundException(site, dn))
		{
			this.site = site;
			this.dn = dn;
		}

		public EdgeSyncServiceConfigNotFoundException(string site, string dn, Exception innerException) : base(Strings.EdgeSyncServiceConfigNotFoundException(site, dn), innerException)
		{
			this.site = site;
			this.dn = dn;
		}

		protected EdgeSyncServiceConfigNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.site = (string)info.GetValue("site", typeof(string));
			this.dn = (string)info.GetValue("dn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("site", this.site);
			info.AddValue("dn", this.dn);
		}

		public string Site
		{
			get
			{
				return this.site;
			}
		}

		public string Dn
		{
			get
			{
				return this.dn;
			}
		}

		private readonly string site;

		private readonly string dn;
	}
}
