using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ConversionAddressCache : ConversionAddressCollection
	{
		internal HashSet<NativeStorePropertyDefinition> CacheProperties
		{
			get
			{
				return this.cacheProperties;
			}
		}

		internal static HashSet<NativeStorePropertyDefinition> AllCacheProperties
		{
			get
			{
				return ConversionAddressCache.mergeCacheProperties;
			}
		}

		internal bool IsCacheProperty(NativeStorePropertyDefinition property)
		{
			return this.itemParticipants.CacheProperties.Contains(property);
		}

		internal static bool IsAnyCacheProperty(NativeStorePropertyDefinition property)
		{
			return ConversionAddressCache.AllCacheProperties.Contains(property);
		}

		internal ConversionAddressCache(ConversionLimitsTracker limitsTracker, bool useSimpleDisplayName, bool ewsOutboundMimeConversion) : base(useSimpleDisplayName, ewsOutboundMimeConversion)
		{
			this.limitsTracker = limitsTracker;
			if (!this.limitsTracker.EnforceLimits)
			{
				this.disableLengthValidation = true;
			}
			this.propertyBag = new MemoryPropertyBag();
			this.propertyBag.SetAllPropertiesLoaded();
			this.recipients = new ConversionRecipientList();
			this.replyTo = new ReplyTo(this.propertyBag, false);
			this.itemParticipants = new ConversionItemParticipants(this.propertyBag, this.ewsOutboundMimeConversion);
			base.AddParticipantList(this.recipients);
			base.AddParticipantList(this.ReplyTo);
			base.AddParticipantList(this.itemParticipants);
			this.cacheProperties = new HashSet<NativeStorePropertyDefinition>(Util.MergeArrays<NativeStorePropertyDefinition>(new ICollection<NativeStorePropertyDefinition>[]
			{
				this.itemParticipants.CacheProperties,
				new NativeStorePropertyDefinition[]
				{
					InternalSchema.MapiReplyToBlob,
					InternalSchema.MapiReplyToNames
				}
			}));
		}

		internal void SetProperty(NativeStorePropertyDefinition property, object value)
		{
			this.propertyBag[property] = value;
		}

		internal void AddRecipients(List<Participant> participants, RecipientItemType recipientType)
		{
			foreach (Participant participant in participants)
			{
				ConversionRecipientEntry recipientEntry = new ConversionRecipientEntry(participant, recipientType);
				this.AddRecipient(recipientEntry);
			}
		}

		internal void AddRecipient(ConversionRecipientEntry recipientEntry)
		{
			this.limitsTracker.CountRecipient();
			this.recipients.Add(recipientEntry);
		}

		internal void AddReplyTo(List<Participant> participants)
		{
			foreach (Participant participant in participants)
			{
				Participant item = participant;
				if (participant.RoutingType == null)
				{
					item = new Participant(participant.DisplayName, participant.DisplayName, "SMTP");
				}
				this.ReplyTo.Add(item);
			}
		}

		internal ConversionItemParticipants Participants
		{
			get
			{
				return this.itemParticipants;
			}
		}

		internal ReplyTo ReplyTo
		{
			get
			{
				if (this.replyTo == null)
				{
					this.replyTo = new ReplyTo(this.propertyBag);
				}
				return this.replyTo;
			}
		}

		internal void Resolve()
		{
			this.ReplyTo.Resync(true);
			ConversionAddressCollection.ParticipantResolutionList participantResolutionList = base.CreateResolutionList();
			base.ResolveParticipants(participantResolutionList);
			base.SetResolvedParticipants(participantResolutionList);
			this.ReplyTo.Resync(true);
		}

		internal void Cleanup()
		{
			this.recipients.Clear();
			this.propertyBag.Clear();
			this.propertyBag.SetAllPropertiesLoaded();
			this.replyTo = null;
		}

		protected ConversionRecipientList recipients;

		protected ReplyTo replyTo;

		protected ConversionItemParticipants itemParticipants;

		protected MemoryPropertyBag propertyBag;

		protected ConversionLimitsTracker limitsTracker;

		protected HashSet<NativeStorePropertyDefinition> cacheProperties;

		private static HashSet<NativeStorePropertyDefinition> mergeCacheProperties = new HashSet<NativeStorePropertyDefinition>(Util.MergeArrays<NativeStorePropertyDefinition>(new ICollection<NativeStorePropertyDefinition>[]
		{
			ConversionItemParticipants.AllCacheProperties,
			new NativeStorePropertyDefinition[]
			{
				InternalSchema.MapiReplyToBlob,
				InternalSchema.MapiReplyToNames
			}
		}));
	}
}
