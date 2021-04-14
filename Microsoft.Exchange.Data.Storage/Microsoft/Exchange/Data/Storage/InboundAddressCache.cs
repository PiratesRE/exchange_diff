using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class InboundAddressCache : ConversionAddressCache
	{
		internal InboundAddressCache(InboundConversionOptions options, ConversionLimitsTracker limitsTracker, MimeMessageLevel messageLevel) : base(limitsTracker, false, false)
		{
			this.inboundOptions = options;
			this.mimeMessageLevel = messageLevel;
			this.tnefRecipientList = null;
		}

		internal void AddDependentAddressCache(InboundAddressCache tnefAddressCache)
		{
			this.tnefRecipientList = tnefAddressCache.recipients;
			base.AddParticipantList(tnefAddressCache.recipients);
			base.ReplyTo.Resync(true);
			tnefAddressCache.ReplyTo.Resync(true);
			if (this.ReplaceMatchingEntries(base.ReplyTo, tnefAddressCache.ReplyTo))
			{
				base.ReplyTo.Resync(true);
			}
			if (base.ReplyTo.Count == 0 && tnefAddressCache.ReplyTo.Count != 0)
			{
				this.propertyBag[InternalSchema.MapiReplyToBlob] = tnefAddressCache.ReplyTo.Blob;
				this.propertyBag[InternalSchema.MapiReplyToNames] = tnefAddressCache.ReplyTo.Names;
				this.replyTo = null;
			}
			foreach (ConversionItemParticipants.ParticipantDefinitionEntry participantDefinitionEntry in ConversionItemParticipants.ParticipantEntries)
			{
				Participant valueOrDefault = this.propertyBag.GetValueOrDefault<Participant>(participantDefinitionEntry.ParticipantProperty);
				if (valueOrDefault == null)
				{
					valueOrDefault = tnefAddressCache.propertyBag.GetValueOrDefault<Participant>(participantDefinitionEntry.ParticipantProperty);
					if (valueOrDefault != null && (participantDefinitionEntry.IsAlwaysResolvable || this.CanResolveParticipant(valueOrDefault) || valueOrDefault.RoutingType != "EX"))
					{
						this.propertyBag.SetOrDeleteProperty(participantDefinitionEntry.ParticipantProperty, valueOrDefault);
					}
				}
			}
		}

		internal void ClearRecipients()
		{
			this.limitsTracker.RollbackRecipients(this.recipients.Count);
			this.recipients.Clear();
		}

		internal void CopyDataToItem(ICoreItem coreItem)
		{
			this.CopyDataToItem(coreItem, false);
		}

		internal void CopyDataToItem(ICoreItem coreItem, bool importResourceFromTnef)
		{
			base.ReplyTo.Resync(true);
			foreach (ConversionItemParticipants.ParticipantDefinitionEntry participantDefinitionEntry in ConversionItemParticipants.ParticipantEntries)
			{
				CoreObject.GetPersistablePropertyBag(coreItem).SetOrDeleteProperty(participantDefinitionEntry.ParticipantProperty, this.propertyBag.TryGetProperty(participantDefinitionEntry.ParticipantProperty));
			}
			CoreObject.GetPersistablePropertyBag(coreItem).SetOrDeleteProperty(InternalSchema.MapiReplyToBlob, this.propertyBag.TryGetProperty(InternalSchema.MapiReplyToBlob));
			CoreObject.GetPersistablePropertyBag(coreItem).SetOrDeleteProperty(InternalSchema.MapiReplyToNames, this.propertyBag.TryGetProperty(InternalSchema.MapiReplyToNames));
			if (this.tnefRecipientList != null)
			{
				Dictionary<ConversionRecipientEntry, ConversionRecipientEntry> dictionary = new Dictionary<ConversionRecipientEntry, ConversionRecipientEntry>();
				foreach (ConversionRecipientEntry conversionRecipientEntry in this.tnefRecipientList)
				{
					dictionary[conversionRecipientEntry] = conversionRecipientEntry;
				}
				foreach (ConversionRecipientEntry conversionRecipientEntry2 in this.recipients)
				{
					ConversionRecipientEntry conversionRecipientEntry3 = null;
					if (dictionary.TryGetValue(conversionRecipientEntry2, out conversionRecipientEntry3) && conversionRecipientEntry3 != null)
					{
						conversionRecipientEntry2.CopyDependentProperties(conversionRecipientEntry3);
					}
				}
			}
			foreach (ConversionRecipientEntry conversionRecipientEntry4 in this.recipients)
			{
				if (RecipientItemType.Bcc == conversionRecipientEntry4.RecipientItemType)
				{
					importResourceFromTnef = false;
				}
				this.CopyRecipientToMessage(coreItem, conversionRecipientEntry4);
			}
			if (importResourceFromTnef)
			{
				foreach (ConversionRecipientEntry conversionRecipientEntry5 in this.tnefRecipientList)
				{
					if (RecipientItemType.Bcc == conversionRecipientEntry5.RecipientItemType && (conversionRecipientEntry5.Participant.GetValueOrDefault<bool>(ParticipantSchema.IsRoom, false) || conversionRecipientEntry5.Participant.GetValueOrDefault<bool>(ParticipantSchema.IsResource, false)))
					{
						this.CopyRecipientToMessage(coreItem, conversionRecipientEntry5);
					}
				}
			}
		}

		private void CopyRecipientToMessage(ICoreItem coreItem, ConversionRecipientEntry entry)
		{
			CoreRecipient coreRecipient = coreItem.Recipients.CreateCoreRecipient(new CoreRecipient.SetDefaultPropertiesDelegate(Recipient.SetDefaultRecipientProperties), entry.Participant);
			coreRecipient.RecipientItemType = entry.RecipientItemType;
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in entry.AllNativeProperties)
			{
				object obj = entry.TryGetProperty(nativeStorePropertyDefinition);
				coreRecipient.PropertyBag.TryGetProperty(nativeStorePropertyDefinition);
				if (!(obj is PropertyError) && !RecipientBase.ImmutableProperties.Contains(nativeStorePropertyDefinition))
				{
					coreRecipient.PropertyBag.SetProperty(nativeStorePropertyDefinition, obj);
				}
			}
			coreRecipient.InternalUpdateParticipant(entry.Participant);
		}

		protected override bool CanResolveParticipant(Participant participant)
		{
			return this.IsResolvingAllParticipants;
		}

		public bool IsResolvingAllParticipants
		{
			get
			{
				if (this.mimeMessageLevel != MimeMessageLevel.AttachedMessage)
				{
					return this.inboundOptions.IsSenderTrusted;
				}
				return !this.inboundOptions.ApplyTrustToAttachedMessages || this.inboundOptions.IsSenderTrusted;
			}
		}

		protected override string TargetResolutionType
		{
			get
			{
				return "EX";
			}
		}

		internal InboundConversionOptions Options
		{
			get
			{
				return this.inboundOptions;
			}
		}

		internal IList<ConversionRecipientEntry> Recipients
		{
			get
			{
				return new ReadOnlyCollection<ConversionRecipientEntry>(this.recipients);
			}
		}

		private bool ReplaceMatchingEntries(IConversionParticipantList primary, IConversionParticipantList secondary)
		{
			if (primary.Count != secondary.Count)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < primary.Count; i++)
			{
				if ((this.CanResolveParticipant(primary[i]) || primary.IsConversionParticipantAlwaysResolvable(i)) && primary[i].AreAddressesEqual(secondary[i]))
				{
					primary[i] = secondary[i];
					result = true;
				}
			}
			return result;
		}

		protected override IADRecipientCache GetRecipientCache(int count)
		{
			IADRecipientCache iadrecipientCache = this.inboundOptions.RecipientCache;
			if (iadrecipientCache == null)
			{
				IRecipientSession recipientSession = this.inboundOptions.UserADSession;
				if (recipientSession == null)
				{
					try
					{
						recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1431, "GetRecipientCache", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ContentConversion\\ConversionAddressCache.cs");
					}
					catch (ExAssertException)
					{
						throw new ArgumentException(InboundConversionOptions.NoScopedTenantInfoNotice);
					}
				}
				iadrecipientCache = new ADRecipientCache<ADRawEntry>(recipientSession, Util.CollectionToArray<ADPropertyDefinition>(ParticipantSchema.SupportedADProperties), count);
			}
			return iadrecipientCache;
		}

		private InboundConversionOptions inboundOptions;

		private MimeMessageLevel mimeMessageLevel;

		private ConversionRecipientList tnefRecipientList;
	}
}
