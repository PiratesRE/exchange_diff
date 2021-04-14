using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SharingDomain
	{
		[DataMember]
		public string DomainName { get; set; }

		[DataMember]
		public string SharingStatusString
		{
			get
			{
				switch (this.SharingStatus)
				{
				case 0:
					return Strings.Succeeded;
				case 1:
					return Strings.Pending;
				case 2:
					return Strings.Failed;
				default:
					return string.Empty;
				}
			}
			set
			{
			}
		}

		[DataMember]
		public int SharingStatus { get; set; }

		[DataMember]
		public string Proof { get; set; }

		[DataMember]
		public string RawIdentity { get; set; }

		public static explicit operator SmtpDomain(SharingDomain sharingDomain)
		{
			return new SmtpDomain(sharingDomain.DomainName);
		}

		public static explicit operator SharingDomain(SmtpDomain smtpDomain)
		{
			return new SharingDomain
			{
				DomainName = smtpDomain.Domain,
				SharingStatus = 2
			};
		}

		public static explicit operator SharingDomain(FederatedDomain federatedDomain)
		{
			return new SharingDomain
			{
				DomainName = federatedDomain.Domain.Domain,
				SharingStatus = 0
			};
		}

		public override string ToString()
		{
			return string.Format("SharingDomain:{0}", this.DomainName);
		}
	}
}
