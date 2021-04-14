using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidAdSiteException : LocalizedException
	{
		public InvalidAdSiteException(string adSite) : base(Strings.InvalidAdSite(adSite))
		{
			this.adSite = adSite;
		}

		public InvalidAdSiteException(string adSite, Exception innerException) : base(Strings.InvalidAdSite(adSite), innerException)
		{
			this.adSite = adSite;
		}

		protected InvalidAdSiteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.adSite = (string)info.GetValue("adSite", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("adSite", this.adSite);
		}

		public string AdSite
		{
			get
			{
				return this.adSite;
			}
		}

		private readonly string adSite;
	}
}
