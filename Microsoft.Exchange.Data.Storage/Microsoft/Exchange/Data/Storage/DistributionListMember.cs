using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DistributionListMember : IDistributionListMember, IRecipientBase
	{
		internal DistributionListMember(DistributionList distributionList, Participant participant)
		{
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			this.distributionList = distributionList;
			this.participant = participant;
			this.memberStatus = MemberStatus.Normal;
			this.mainEntryId = ParticipantEntryId.FromParticipant(participant, ParticipantEntryIdConsumer.DLMemberList);
			if (this.mainEntryId == null)
			{
				throw new InvalidParticipantException(ServerStrings.ExOperationNotSupportedForRoutingType("DistributionList.Add", participant.RoutingType), ParticipantValidationStatus.OperationNotSupportedForRoutingType);
			}
		}

		internal DistributionListMember(DistributionList distributionList, ParticipantEntryId mainEntryId, OneOffParticipantEntryId oneOffEntryId, byte[] extraBytes) : this(distributionList, mainEntryId, oneOffEntryId)
		{
			if (extraBytes != null)
			{
				this.extraBytes = extraBytes;
			}
		}

		internal DistributionListMember(DistributionList distributionList, ParticipantEntryId mainEntryId, OneOffParticipantEntryId oneOffEntryId)
		{
			this.distributionList = distributionList;
			this.memberStatus = MemberStatus.Normal;
			Participant.Builder builder = new Participant.Builder();
			ADParticipantEntryId adparticipantEntryId = mainEntryId as ADParticipantEntryId;
			if (adparticipantEntryId != null)
			{
				builder.SetPropertiesFrom(adparticipantEntryId);
				if (oneOffEntryId != null)
				{
					builder.DisplayName = oneOffEntryId.EmailDisplayName;
					if (!string.IsNullOrEmpty(oneOffEntryId.EmailAddress) && Participant.RoutingTypeEquals(oneOffEntryId.EmailAddressType, "SMTP"))
					{
						builder[ParticipantSchema.SmtpAddress] = oneOffEntryId.EmailAddress;
					}
				}
				this.participant = builder.ToParticipant();
			}
			else
			{
				StoreParticipantEntryId storeParticipantEntryId = mainEntryId as StoreParticipantEntryId;
				if (storeParticipantEntryId != null && oneOffEntryId != null)
				{
					builder.SetPropertiesFrom(oneOffEntryId);
					builder.SetPropertiesFrom(storeParticipantEntryId);
					this.participant = builder.ToParticipant();
				}
				else
				{
					OneOffParticipantEntryId oneOffParticipantEntryId = mainEntryId as OneOffParticipantEntryId;
					if (oneOffParticipantEntryId == null)
					{
						oneOffParticipantEntryId = oneOffEntryId;
						this.memberStatus = MemberStatus.Demoted;
					}
					if (oneOffParticipantEntryId != null)
					{
						builder.SetPropertiesFrom(oneOffParticipantEntryId);
						this.participant = builder.ToParticipant();
					}
					else
					{
						this.memberStatus = MemberStatus.Unrecognized;
					}
				}
			}
			if (this.mainEntryId == null)
			{
				this.mainEntryId = mainEntryId;
			}
			if (this.oneOffEntryId == null)
			{
				this.oneOffEntryId = oneOffEntryId;
			}
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Loaded a {1} DL member \"{0}\". MainEntryId=\"{2}\", OneOffEntryId=\"{3}\"", new object[]
			{
				this.participant,
				this.memberStatus,
				this.mainEntryId,
				this.oneOffEntryId
			});
		}

		public ParticipantEntryId MainEntryId
		{
			get
			{
				return this.mainEntryId;
			}
		}

		internal OneOffParticipantEntryId OneOffEntryId
		{
			get
			{
				if (this.oneOffEntryId == null && this.participant != null)
				{
					this.oneOffEntryId = new OneOffParticipantEntryId(this.participant);
				}
				return this.oneOffEntryId;
			}
		}

		internal byte[] ExtraBytes
		{
			get
			{
				return this.extraBytes ?? Array<byte>.Empty;
			}
			set
			{
				this.extraBytes = value;
			}
		}

		internal static DistributionListMember CopyFrom(DistributionList distributionList, DistributionListMember member)
		{
			if (member != null)
			{
				return new DistributionListMember(distributionList, member.mainEntryId, member.oneOffEntryId);
			}
			if (member.Participant != null)
			{
				return new DistributionListMember(distributionList, member.Participant);
			}
			throw new NotSupportedException(ServerStrings.ExCantCopyBadAlienDLMember);
		}

		public DistributionList DistributionList
		{
			get
			{
				return this.distributionList;
			}
		}

		public MemberStatus MemberStatus
		{
			get
			{
				return this.memberStatus;
			}
		}

		public RecipientId Id
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool? IsDistributionList()
		{
			return this.mainEntryId.IsDL;
		}

		public Participant Participant
		{
			get
			{
				return this.participant;
			}
		}

		private readonly DistributionList distributionList;

		private readonly ParticipantEntryId mainEntryId;

		private readonly MemberStatus memberStatus;

		private OneOffParticipantEntryId oneOffEntryId;

		private byte[] extraBytes;

		private readonly Participant participant;
	}
}
