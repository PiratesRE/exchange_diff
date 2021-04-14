using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.RedirectionModule.LocStrings;

namespace Microsoft.Exchange.Configuration.RedirectionModule
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToResolveTargetSite : RedirectionLogicException
	{
		public FailedToResolveTargetSite(string domainName, int minorPartnerId) : base(Strings.FailedToResolveTargetSite(domainName, minorPartnerId))
		{
			this.domainName = domainName;
			this.minorPartnerId = minorPartnerId;
		}

		public FailedToResolveTargetSite(string domainName, int minorPartnerId, Exception innerException) : base(Strings.FailedToResolveTargetSite(domainName, minorPartnerId), innerException)
		{
			this.domainName = domainName;
			this.minorPartnerId = minorPartnerId;
		}

		protected FailedToResolveTargetSite(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domainName = (string)info.GetValue("domainName", typeof(string));
			this.minorPartnerId = (int)info.GetValue("minorPartnerId", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domainName", this.domainName);
			info.AddValue("minorPartnerId", this.minorPartnerId);
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

		private readonly string domainName;

		private readonly int minorPartnerId;
	}
}
