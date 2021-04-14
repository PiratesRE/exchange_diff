using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class PopSubscription : Subscription
	{
		[IgnoreDataMember]
		public AuthenticationMechanism IncomingAuth { get; set; }

		[DataMember(Name = "IncomingAuth", IsRequired = false, EmitDefaultValue = false)]
		public string IncomingAuthString
		{
			get
			{
				return EnumUtilities.ToString<AuthenticationMechanism>(this.IncomingAuth);
			}
			set
			{
				this.IncomingAuth = EnumUtilities.Parse<AuthenticationMechanism>(value);
			}
		}

		[DataMember]
		public int IncomingPort { get; set; }

		[IgnoreDataMember]
		public SecurityMechanism IncomingSecurity { get; set; }

		[DataMember(Name = "IncomingSecurity", IsRequired = false, EmitDefaultValue = false)]
		public string IncomingSecurityString
		{
			get
			{
				return EnumUtilities.ToString<SecurityMechanism>(this.IncomingSecurity);
			}
			set
			{
				this.IncomingSecurity = EnumUtilities.Parse<SecurityMechanism>(value);
			}
		}

		[DataMember]
		public string IncomingServer { get; set; }

		[DataMember]
		public string IncomingUserName { get; set; }

		[DataMember]
		public bool LeaveOnServer { get; set; }
	}
}
