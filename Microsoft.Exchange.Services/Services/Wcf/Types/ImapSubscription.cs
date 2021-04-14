using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ImapSubscription : Subscription
	{
		[IgnoreDataMember]
		public IMAPAuthenticationMechanism IncomingAuth { get; set; }

		[DataMember(Name = "IncomingAuth", IsRequired = false, EmitDefaultValue = false)]
		public string IncomingAuthString
		{
			get
			{
				return EnumUtilities.ToString<IMAPAuthenticationMechanism>(this.IncomingAuth);
			}
			set
			{
				this.IncomingAuth = EnumUtilities.Parse<IMAPAuthenticationMechanism>(value);
			}
		}

		[DataMember]
		public int IncomingPort { get; set; }

		[IgnoreDataMember]
		public IMAPSecurityMechanism IncomingSecurity { get; set; }

		[DataMember(Name = "IncomingSecurity", IsRequired = false, EmitDefaultValue = false)]
		public string IncomingSecurityString
		{
			get
			{
				return EnumUtilities.ToString<IMAPSecurityMechanism>(this.IncomingSecurity);
			}
			set
			{
				this.IncomingSecurity = EnumUtilities.Parse<IMAPSecurityMechanism>(value);
			}
		}

		[DataMember]
		public string IncomingServer { get; set; }

		[DataMember]
		public string IncomingUserName { get; set; }
	}
}
