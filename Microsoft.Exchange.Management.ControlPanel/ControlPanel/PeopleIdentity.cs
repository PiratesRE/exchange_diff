using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class PeopleIdentity : INamedIdentity
	{
		public PeopleIdentity(string displayName, string legacyDN, string sMTPAddress, int addressOrigin, string routingType, int recipientFlag)
		{
			this.DisplayName = displayName;
			this.Address = legacyDN;
			this.SMTPAddress = sMTPAddress;
			this.RoutingType = routingType;
			this.AddressOrigin = addressOrigin;
			this.RecipientFlag = recipientFlag;
			this.IgnoreDisplayNameInIdentity = false;
		}

		[DataMember]
		public string DisplayName { get; private set; }

		[DataMember]
		public string Address { get; internal set; }

		[DataMember]
		public string SMTPAddress { get; private set; }

		[DataMember]
		public string RoutingType { get; private set; }

		[DataMember]
		public int AddressOrigin { get; private set; }

		[DataMember]
		public int RecipientFlag { get; private set; }

		public bool IgnoreDisplayNameInIdentity { get; set; }

		string INamedIdentity.Identity
		{
			get
			{
				if (!(this.RoutingType == "SMTP"))
				{
					return this.Address;
				}
				if (!this.IgnoreDisplayNameInIdentity)
				{
					return string.Concat(new string[]
					{
						"\"",
						this.DisplayName,
						"\"<",
						this.SMTPAddress,
						">"
					});
				}
				return this.SMTPAddress;
			}
		}

		internal static PeopleIdentity FromIdParameter(object value)
		{
			if (value is PeopleIdentity)
			{
				return (PeopleIdentity)value;
			}
			return null;
		}

		internal static PeopleIdentity[] FromIdParameters(object value)
		{
			if (value is PeopleIdentity[])
			{
				return (PeopleIdentity[])value;
			}
			if (value is string[])
			{
				return (from v in (string[])value
				select PeopleIdentity.FromIdParameter(v)).ToArray<PeopleIdentity>();
			}
			return null;
		}

		public const string ExchangeRoutingType = "EX";

		public const string SmtpRoutingType = "SMTP";
	}
}
