using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OneOffParticipantEntryId : ParticipantEntryId
	{
		internal OneOffParticipantEntryId(Participant participant) : this(participant.DisplayName, participant.EmailAddress, participant.RoutingType)
		{
			bool valueOrDefault = participant.GetValueOrDefault<bool>(ParticipantSchema.SendRichInfo);
			uint num = (uint)participant.GetValueOrDefault<int>(ParticipantSchema.SendInternetEncoding);
			if ((num & 4286709759U) != 0U)
			{
				num = 0U;
			}
			this.flags = ((this.flags & ~(OneOffFlag.NoTnef | OneOffFlag.SendInternetEncodingMask)) | (valueOrDefault ? OneOffFlag.Simple : OneOffFlag.NoTnef) | (OneOffFlag)(num & 8257536U));
		}

		internal OneOffParticipantEntryId(string displayName, string address, string addressType) : this(displayName, addressType, address, (OneOffFlag)2147549184U)
		{
		}

		internal OneOffParticipantEntryId(string displayName, string addressType, string address, OneOffFlag flags)
		{
			EnumValidator.AssertValid<OneOffFlag>(flags);
			this.emailDisplayName = Util.NullIf<string>(displayName, string.Empty);
			this.emailAddressType = Util.NullIf<string>(addressType, string.Empty);
			this.emailAddress = Util.NullIf<string>(address, string.Empty);
			this.flags = flags;
		}

		internal OneOffParticipantEntryId(ParticipantEntryId.Reader reader)
		{
			this.flags = (OneOffFlag)reader.ReadUInt32();
			Encoding encoding = ((this.flags & (OneOffFlag)2147483648U) == (OneOffFlag)2147483648U) ? Encoding.Unicode : CTSGlobals.AsciiEncoding;
			this.emailDisplayName = Util.NullIf<string>(reader.ReadZString(encoding), string.Empty);
			this.emailAddressType = Util.NullIf<string>(reader.ReadZString(encoding), string.Empty);
			this.emailAddress = Util.NullIf<string>(reader.ReadZString(encoding), string.Empty);
			reader.EnsureEnd();
			if (this.emailAddress == "Unknown")
			{
				this.emailAddress = null;
			}
		}

		internal string EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		internal string EmailAddressType
		{
			get
			{
				return this.emailAddressType;
			}
		}

		internal string EmailDisplayName
		{
			get
			{
				return this.emailDisplayName;
			}
		}

		public override string ToString()
		{
			return string.Format("OneOff: \"{0}\" {1} [{2}]; {3}", new object[]
			{
				this.emailDisplayName,
				this.emailAddress,
				this.emailAddressType,
				this.flags
			});
		}

		internal static OneOffParticipantEntryId TryFromParticipant(Participant participant, ParticipantEntryIdConsumer consumer)
		{
			if (!(participant.Origin is OneOffParticipantOrigin))
			{
				Participant participant2 = participant.ChangeOrigin(new OneOffParticipantOrigin());
				if (!participant.AreAddressesEqual(participant2))
				{
					return null;
				}
				participant = participant2;
			}
			if (participant.RoutingType != null)
			{
				return new OneOffParticipantEntryId(participant);
			}
			return null;
		}

		internal override IEnumerable<PropValue> GetParticipantProperties()
		{
			List<PropValue> list = Participant.ListCoreProperties(this.emailDisplayName, this.emailAddress, this.emailAddressType);
			if ((this.flags & OneOffFlag.NoTnef) != OneOffFlag.NoTnef)
			{
				list.Add(new PropValue(ParticipantSchema.SendRichInfo, (this.flags & OneOffFlag.NoTnef) == OneOffFlag.Simple));
			}
			if ((this.flags & OneOffFlag.SendInternetEncodingMask) != OneOffFlag.Simple)
			{
				list.Add(new PropValue(ParticipantSchema.SendInternetEncoding, (int)(this.flags & OneOffFlag.SendInternetEncodingMask)));
			}
			return list;
		}

		internal Participant ToParticipant()
		{
			return new Participant(this.EmailDisplayName, this.EmailAddress, this.EmailAddressType);
		}

		protected override void Serialize(ParticipantEntryId.Writer writer)
		{
			writer.WriteEntryHeader(ParticipantEntryId.OneOffProviderGuid);
			writer.Write((uint)this.flags);
			Encoding encoding = ((this.flags & (OneOffFlag)2147483648U) == (OneOffFlag)2147483648U) ? Encoding.Unicode : CTSGlobals.AsciiEncoding;
			writer.WriteZString(this.emailDisplayName ?? string.Empty, encoding);
			writer.WriteZString(this.emailAddressType ?? string.Empty, encoding);
			writer.WriteZString(this.emailAddress ?? string.Empty, encoding);
		}

		private const string OutlookPlaceholderForMissingAddress = "Unknown";

		private const OneOffFlag DefaultOneOffFlag = OneOffFlag.NoTnef | OneOffFlag.Unicode;

		private readonly string emailAddress;

		private readonly string emailDisplayName;

		private readonly string emailAddressType;

		private readonly OneOffFlag flags;
	}
}
