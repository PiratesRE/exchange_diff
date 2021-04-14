using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class ADRecipientOrAddress : ISerializable
	{
		internal ADRecipientOrAddress(Participant participant)
		{
			this.participant = participant;
		}

		public ADRecipientOrAddress(SerializationInfo info, StreamingContext context)
		{
			string displayName = (string)info.GetValue("displayName", typeof(string));
			string emailAddress = (string)info.GetValue("address", typeof(string));
			string routingType = (string)info.GetValue("routingType", typeof(string));
			this.participant = new Participant(displayName, emailAddress, routingType);
		}

		internal Participant Participant
		{
			get
			{
				return this.participant;
			}
		}

		public string Address
		{
			get
			{
				return this.participant.EmailAddress ?? string.Empty;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.participant.DisplayName ?? string.Empty;
			}
		}

		public string RoutingType
		{
			get
			{
				return this.participant.RoutingType ?? string.Empty;
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("displayName", this.DisplayName);
			info.AddValue("address", this.Address);
			info.AddValue("routingType", this.RoutingType);
		}

		public override string ToString()
		{
			return this.participant.ToString(AddressFormat.OutlookFormat);
		}

		public override bool Equals(object obj)
		{
			ADRecipientOrAddress adrecipientOrAddress = obj as ADRecipientOrAddress;
			return adrecipientOrAddress != null && object.Equals(this.participant, adrecipientOrAddress.participant);
		}

		public override int GetHashCode()
		{
			return this.participant.GetHashCode();
		}

		private const string DisplayNameKey = "displayName";

		private const string AddressKey = "address";

		private const string RoutingTypeKey = "routingType";

		[NonSerialized]
		private Participant participant;
	}
}
