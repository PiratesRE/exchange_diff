using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "EmailAddress")]
	[XmlType(TypeName = "EmailAddressType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class EmailAddressWrapper
	{
		[XmlElement("Name")]
		[DataMember(Name = "Name", EmitDefaultValue = false, Order = 1)]
		public string Name { get; set; }

		[XmlElement("EmailAddress")]
		[DataMember(Name = "EmailAddress", EmitDefaultValue = false, Order = 2)]
		public string EmailAddress { get; set; }

		[XmlElement("RoutingType")]
		[DataMember(Name = "RoutingType", EmitDefaultValue = false, Order = 3)]
		public string RoutingType { get; set; }

		[XmlElement("MailboxType")]
		[DataMember(Name = "MailboxType", EmitDefaultValue = false, Order = 4)]
		public string MailboxType { get; set; }

		[XmlElement("ItemId")]
		[DataMember(Name = "ItemId", EmitDefaultValue = false, Order = 5)]
		public ItemId ItemId { get; set; }

		[XmlIgnore]
		[DataMember(Name = "SipUri", EmitDefaultValue = false, Order = 6)]
		public string SipUri { get; set; }

		[DataMember(Name = "Submitted", EmitDefaultValue = false, Order = 7)]
		[XmlIgnore]
		public bool? Submitted { get; set; }

		[DataMember(Name = "OriginalDisplayName", EmitDefaultValue = false, Order = 8)]
		[XmlElement("OriginalDisplayName")]
		public string OriginalDisplayName { get; set; }

		[DataMember(Name = "EmailAddressIndex", EmitDefaultValue = false, Order = 9)]
		[XmlIgnore]
		public string EmailAddressIndex { get; set; }

		internal static EmailAddressWrapper FromParticipant(IParticipant participant)
		{
			return EmailAddressWrapper.FromParticipant(participant, EWSSettings.ParticipantInformation);
		}

		internal static EmailAddressWrapper FromParticipant(IParticipant participant, ParticipantInformationDictionary convertedParticipantsCache)
		{
			if (participant == null)
			{
				return null;
			}
			ParticipantInformation participantInformation;
			if (!convertedParticipantsCache.TryGetParticipant(participant, out participantInformation))
			{
				participantInformation = ParticipantInformationDictionary.ConvertToParticipantInformation(participant);
				convertedParticipantsCache.AddParticipant(participant, participantInformation);
			}
			return EmailAddressWrapper.FromParticipantInformation(participantInformation);
		}

		internal static EmailAddressWrapper FromParticipantInformation(ParticipantInformation participant)
		{
			return new EmailAddressWrapper
			{
				Name = participant.DisplayName,
				EmailAddress = participant.EmailAddress,
				RoutingType = participant.RoutingType,
				MailboxType = participant.MailboxType.ToString(),
				SipUri = participant.SipUri,
				Submitted = participant.Submitted
			};
		}

		public static IEqualityComparer<EmailAddressWrapper> AddressEqualityComparer
		{
			get
			{
				return EmailAddressWrapper.AddressEqualityComparerImpl.Default;
			}
		}

		private class AddressEqualityComparerImpl : IEqualityComparer<EmailAddressWrapper>
		{
			public bool Equals(EmailAddressWrapper x, EmailAddressWrapper y)
			{
				return object.ReferenceEquals(x, y) || (x != null && y != null && string.Compare(x.RoutingType, y.RoutingType, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(x.EmailAddress, y.EmailAddress, StringComparison.OrdinalIgnoreCase) == 0);
			}

			public int GetHashCode(EmailAddressWrapper x)
			{
				int num = 0;
				if (x != null)
				{
					num = ((x.EmailAddress != null) ? x.EmailAddress.GetHashCode() : 0);
					if (num == 0)
					{
						num = ((x.OriginalDisplayName != null) ? x.OriginalDisplayName.GetHashCode() : 0);
					}
					if (num == 0)
					{
						num = ((x.Name != null) ? x.Name.GetHashCode() : 0);
					}
				}
				return num;
			}

			public static EmailAddressWrapper.AddressEqualityComparerImpl Default = new EmailAddressWrapper.AddressEqualityComparerImpl();
		}
	}
}
