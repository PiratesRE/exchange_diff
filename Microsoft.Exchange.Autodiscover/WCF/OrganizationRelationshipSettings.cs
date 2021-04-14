using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public sealed class OrganizationRelationshipSettings
	{
		public OrganizationRelationshipSettings()
		{
		}

		internal OrganizationRelationshipSettings(OrganizationRelationship organizationRelationship)
		{
			if (organizationRelationship.DomainNames != null)
			{
				this.DomainNames = new DomainCollection();
				foreach (SmtpDomain smtpDomain in organizationRelationship.DomainNames)
				{
					this.DomainNames.Add(smtpDomain.Domain);
				}
			}
			this.Name = organizationRelationship.Name;
			this.TargetApplicationUri = organizationRelationship.TargetApplicationUri;
			this.FreeBusyAccessLevel = Enum.GetName(typeof(FreeBusyAccessLevel), organizationRelationship.FreeBusyAccessLevel);
			this.FreeBusyAccessEnabled = organizationRelationship.FreeBusyAccessEnabled;
			this.TargetSharingEpr = organizationRelationship.TargetSharingEpr;
			this.TargetAutodiscoverEpr = organizationRelationship.TargetAutodiscoverEpr;
			this.MailboxMoveEnabled = organizationRelationship.MailboxMoveEnabled;
			this.DeliveryReportEnabled = organizationRelationship.DeliveryReportEnabled;
			this.MailTipsAccessEnabled = organizationRelationship.MailTipsAccessEnabled;
			this.MailTipsAccessLevel = Enum.GetName(typeof(MailTipsAccessLevel), organizationRelationship.MailTipsAccessLevel);
		}

		[DataMember(IsRequired = true)]
		public string Name { get; set; }

		[DataMember(IsRequired = true)]
		public DomainCollection DomainNames { get; set; }

		[DataMember(IsRequired = true)]
		public Uri TargetApplicationUri { get; set; }

		[DataMember(IsRequired = true)]
		public string FreeBusyAccessLevel { get; set; }

		[DataMember(IsRequired = true)]
		public bool FreeBusyAccessEnabled { get; set; }

		[DataMember(IsRequired = true)]
		public Uri TargetSharingEpr { get; set; }

		[DataMember(IsRequired = true)]
		public Uri TargetAutodiscoverEpr { get; set; }

		[DataMember(IsRequired = true)]
		public bool MailboxMoveEnabled { get; set; }

		[DataMember(IsRequired = true)]
		public bool DeliveryReportEnabled { get; set; }

		[DataMember(IsRequired = true)]
		public bool MailTipsAccessEnabled { get; set; }

		[DataMember(IsRequired = true)]
		public string MailTipsAccessLevel { get; set; }
	}
}
