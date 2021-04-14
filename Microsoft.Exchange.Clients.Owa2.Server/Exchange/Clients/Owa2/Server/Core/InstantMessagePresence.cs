using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class InstantMessagePresence
	{
		[IgnoreDataMember]
		public InstantMessagePresenceType Presence { get; set; }

		[DataMember(Name = "Presence")]
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
