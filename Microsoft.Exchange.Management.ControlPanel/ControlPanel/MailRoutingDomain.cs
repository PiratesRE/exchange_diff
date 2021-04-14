using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailRoutingDomain : BaseRow
	{
		public MailRoutingDomain(AcceptedDomain domain) : base(domain)
		{
			this.acceptedDomain = domain;
		}

		[DataMember]
		public string DomainName
		{
			get
			{
				return this.acceptedDomain.DomainName.ToString();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public string DomainType
		{
			get
			{
				return this.acceptedDomain.DomainType.ToString();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public string DomainTypeString
		{
			get
			{
				return this.GetDomainTypeString();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public string CaptionText
		{
			get
			{
				return this.DomainName;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		private string GetDomainTypeString()
		{
			if (this.acceptedDomain.DomainType == AcceptedDomainType.Authoritative)
			{
				return Strings.AuthoritativeString;
			}
			if (this.acceptedDomain.DomainType == AcceptedDomainType.InternalRelay)
			{
				return Strings.InternalRelayString;
			}
			return this.acceptedDomain.DomainType.ToString();
		}

		private AcceptedDomain acceptedDomain;
	}
}
