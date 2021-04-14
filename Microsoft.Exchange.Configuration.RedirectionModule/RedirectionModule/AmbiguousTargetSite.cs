using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.RedirectionModule.LocStrings;

namespace Microsoft.Exchange.Configuration.RedirectionModule
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmbiguousTargetSite : RedirectionLogicException
	{
		public AmbiguousTargetSite(string domainName, int minorPartnerId, string identities) : base(Strings.AmbiguousTargetSite(domainName, minorPartnerId, identities))
		{
			this.domainName = domainName;
			this.minorPartnerId = minorPartnerId;
			this.identities = identities;
		}

		public AmbiguousTargetSite(string domainName, int minorPartnerId, string identities, Exception innerException) : base(Strings.AmbiguousTargetSite(domainName, minorPartnerId, identities), innerException)
		{
			this.domainName = domainName;
			this.minorPartnerId = minorPartnerId;
			this.identities = identities;
		}

		protected AmbiguousTargetSite(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domainName = (string)info.GetValue("domainName", typeof(string));
			this.minorPartnerId = (int)info.GetValue("minorPartnerId", typeof(int));
			this.identities = (string)info.GetValue("identities", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domainName", this.domainName);
			info.AddValue("minorPartnerId", this.minorPartnerId);
			info.AddValue("identities", this.identities);
		}

		public string DomainName
		{
			get
			{
				return this.domainName;
			}
		}

		public int MinorPartnerId
		{
			get
			{
				return this.minorPartnerId;
			}
		}

		public string Identities
		{
			get
			{
				return this.identities;
			}
		}

		private readonly string domainName;

		private readonly int minorPartnerId;

		private readonly string identities;
	}
}
