using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class PresenceChange
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string SipUri { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string UserName { get; set; }

		[IgnoreDataMember]
		public InstantMessagePresenceType Presence { get; set; }

		[DataMember(Name = "Presence", Order = 3)]
		public string PresenceString
		{
			get
			{
				return this.Presence.ToString();
			}
			set
			{
				this.Presence = InstantMessageUtilities.ParseEnumValue<InstantMessagePresenceType>(value, InstantMessagePresenceType.None);
			}
		}
	}
}
