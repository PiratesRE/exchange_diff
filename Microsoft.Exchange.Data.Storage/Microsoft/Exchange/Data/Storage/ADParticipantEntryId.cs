using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ADParticipantEntryId : ParticipantEntryId
	{
		internal ADParticipantEntryId(string legacyDN, LegacyRecipientDisplayType? legacyRecipientDisplayType, bool useWabFormat)
		{
			this.legacyDN = legacyDN;
			this.legacyRecipientDisplayType = legacyRecipientDisplayType;
			this.flags = ADParticipantEntryId.ReplaceObjectTypeInformation(ParticipantEntryId.WabEntryFlag.HomeFax | ParticipantEntryId.WabEntryFlag.OtherFax | ParticipantEntryId.WabEntryFlag.Outlook, ref this.legacyRecipientDisplayType, this.legacyDN);
			this.useWabFormat = useWabFormat;
		}

		internal ADParticipantEntryId(ParticipantEntryId.WabEntryFlag? flags, ParticipantEntryId.Reader reader)
		{
			uint num = reader.ReadUInt32();
			if (num != 1U)
			{
				throw new NotSupportedException(ServerStrings.ExUnsupportedABProvider("Exchange WAB", num.ToString()));
			}
			this.legacyRecipientDisplayType = new LegacyRecipientDisplayType?((LegacyRecipientDisplayType)reader.ReadUInt32());
			if (flags == null && this.legacyRecipientDisplayType == LegacyRecipientDisplayType.MailUser)
			{
				this.legacyRecipientDisplayType = null;
			}
			this.legacyDN = reader.ReadZString(CTSGlobals.AsciiEncoding);
			this.flags = ADParticipantEntryId.ReplaceObjectTypeInformation(flags ?? ParticipantEntryId.WabEntryFlag.Envelope, ref this.legacyRecipientDisplayType, this.legacyDN);
			this.useWabFormat = (flags != null);
			reader.EnsureEnd();
		}

		internal override bool? IsDL
		{
			get
			{
				if (this.legacyRecipientDisplayType == null)
				{
					return null;
				}
				return new bool?((byte)(this.flags & ParticipantEntryId.WabEntryFlag.ObjectTypeMask) == 6);
			}
		}

		internal string LegacyDN
		{
			get
			{
				return this.legacyDN;
			}
		}

		public override string ToString()
		{
			return string.Format("AD contact{0}: \"{1}\"", (this.IsDL == true) ? " (DL)" : string.Empty, this.legacyDN);
		}

		internal static ADParticipantEntryId TryFromParticipant(Participant participant, ParticipantEntryIdConsumer consumer)
		{
			if ((consumer & ParticipantEntryIdConsumer.SupportsADParticipantEntryId) != ParticipantEntryIdConsumer.SupportsNone && participant.RoutingType == "EX")
			{
				return new ADParticipantEntryId(participant.EmailAddress, participant.GetValueAsNullable<LegacyRecipientDisplayType>(ParticipantSchema.DisplayType), (consumer & ParticipantEntryIdConsumer.SupportsWindowsAddressBookEnvelope) != ParticipantEntryIdConsumer.SupportsNone);
			}
			return null;
		}

		internal override IEnumerable<PropValue> GetParticipantProperties()
		{
			List<PropValue> list = new List<PropValue>();
			list.Add(new PropValue(ParticipantSchema.EmailAddress, this.legacyDN));
			list.Add(new PropValue(ParticipantSchema.RoutingType, "EX"));
			if (this.legacyRecipientDisplayType != null)
			{
				list.Add(new PropValue(ParticipantSchema.DisplayType, this.legacyRecipientDisplayType));
			}
			return list;
		}

		protected override void Serialize(ParticipantEntryId.Writer writer)
		{
			if (this.useWabFormat && this.legacyRecipientDisplayType != null)
			{
				writer.WriteEntryHeader(ParticipantEntryId.WabProviderGuid);
				writer.Write((byte)this.flags);
			}
			writer.WriteEntryHeader(ParticipantEntryId.ExchangeProviderGuid);
			writer.Write(1U);
			writer.Write((uint)(this.legacyRecipientDisplayType ?? LegacyRecipientDisplayType.MailUser));
			writer.WriteZString(this.legacyDN, CTSGlobals.AsciiEncoding);
		}

		private static ParticipantEntryId.WabEntryFlag ReplaceObjectTypeInformation(ParticipantEntryId.WabEntryFlag input, ref LegacyRecipientDisplayType? legacyRecipientDisplayType, string legacyDN)
		{
			LegacyRecipientDisplayType valueOrDefault = legacyRecipientDisplayType.GetValueOrDefault();
			ParticipantEntryId.WabEntryFlag wabEntryFlag;
			if (legacyRecipientDisplayType != null)
			{
				switch (valueOrDefault)
				{
				case LegacyRecipientDisplayType.MailUser:
				case LegacyRecipientDisplayType.Forum:
				case LegacyRecipientDisplayType.RemoteMailUser:
					goto IL_33;
				case LegacyRecipientDisplayType.DistributionList:
				case LegacyRecipientDisplayType.DynamicDistributionList:
					wabEntryFlag = ParticipantEntryId.WabEntryFlag.DirectoryDL;
					goto IL_61;
				}
				ExTraceGlobals.StorageTracer.TraceDebug<string, LegacyRecipientDisplayType?>(0L, "Cannot construct ADParticipantEntryId (legDN=\"{0}\") with DisplayType={1}. Defaulting to MailUser.", legacyDN, legacyRecipientDisplayType);
				legacyRecipientDisplayType = new LegacyRecipientDisplayType?(LegacyRecipientDisplayType.MailUser);
			}
			IL_33:
			wabEntryFlag = ParticipantEntryId.WabEntryFlag.DirectoryPerson;
			IL_61:
			return (input & (ParticipantEntryId.WabEntryFlag.HomeFax | ParticipantEntryId.WabEntryFlag.OtherFax | ParticipantEntryId.WabEntryFlag.EmailIndex1 | ParticipantEntryId.WabEntryFlag.Outlook)) | wabEntryFlag;
		}

		private readonly ParticipantEntryId.WabEntryFlag flags;

		private readonly string legacyDN;

		private readonly LegacyRecipientDisplayType? legacyRecipientDisplayType;

		private readonly bool useWabFormat;
	}
}
