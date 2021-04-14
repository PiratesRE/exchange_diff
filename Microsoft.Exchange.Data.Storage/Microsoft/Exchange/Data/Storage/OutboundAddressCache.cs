using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OutboundAddressCache : ConversionAddressCache
	{
		internal OutboundAddressCache(OutboundConversionOptions options, ConversionLimitsTracker limitsTracker) : base(limitsTracker, options.UseSimpleDisplayName, options.EwsOutboundMimeConversion)
		{
			this.outboundOptions = options;
		}

		internal void CopyDataFromItem(Item item)
		{
			base.ReplyTo.Clear();
			base.ReplyTo.Resync(true);
			foreach (StorePropertyDefinition propertyDefinition in base.CacheProperties)
			{
				object obj = item.TryGetProperty(propertyDefinition);
				if (!(obj is PropertyError))
				{
					this.propertyBag[propertyDefinition] = obj;
				}
			}
			base.ReplyTo.Resync(true);
			if (item is MessageItem || item is Task)
			{
				this.CopyRecipientData(item);
				return;
			}
			this.CopyAttendeeData(item);
		}

		private void CopyAttendeeData(Item item)
		{
			CalendarItemBase calendarItemBase = item as CalendarItemBase;
			if (calendarItemBase != null)
			{
				foreach (Attendee attendee in calendarItemBase.AttendeeCollection)
				{
					ConversionRecipientEntry conversionRecipientEntry = new ConversionRecipientEntry(attendee.Participant, (RecipientItemType)attendee.AttendeeType);
					foreach (PropertyDefinition propertyDefinition in attendee.CoreRecipient.PropertyBag.AllFoundProperties)
					{
						StorePropertyDefinition property = (StorePropertyDefinition)propertyDefinition;
						object obj = attendee.TryGetProperty(property);
						if (!(obj is PropertyError))
						{
							conversionRecipientEntry.SetProperty(property, obj, false);
						}
					}
					base.AddRecipient(conversionRecipientEntry);
				}
			}
		}

		private void CopyRecipientData(Item item)
		{
			RecipientCollection recipientCollection = null;
			CoreRecipientCollection recipientCollection2 = item.CoreItem.GetRecipientCollection(true);
			if (recipientCollection2 != null)
			{
				recipientCollection = new RecipientCollection(recipientCollection2);
			}
			foreach (Recipient recipient in recipientCollection)
			{
				ConversionRecipientEntry conversionRecipientEntry = new ConversionRecipientEntry(recipient.Participant, recipient.RecipientItemType);
				foreach (PropertyDefinition propertyDefinition in recipient.CoreRecipient.PropertyBag.AllFoundProperties)
				{
					StorePropertyDefinition property = (StorePropertyDefinition)propertyDefinition;
					object obj = recipient.TryGetProperty(property);
					if (!(obj is PropertyError))
					{
						conversionRecipientEntry.SetProperty(property, obj, false);
					}
				}
				base.AddRecipient(conversionRecipientEntry);
			}
		}

		internal MemoryPropertyBag Properties
		{
			get
			{
				return this.propertyBag;
			}
		}

		internal ConversionRecipientList Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		internal ADRawEntry GetCachedRecipient(Participant participant)
		{
			if (this.outboundOptions == null || this.outboundOptions.RecipientCache == null)
			{
				return null;
			}
			if (participant == null)
			{
				return null;
			}
			ProxyAddress proxyAddressForParticipant = OutboundAddressCache.GetProxyAddressForParticipant(participant);
			if (proxyAddressForParticipant == null)
			{
				return null;
			}
			Result<ADRawEntry> result;
			if (this.outboundOptions.RecipientCache.TryGetValue(proxyAddressForParticipant, out result) && result.Error == null)
			{
				return result.Data;
			}
			return null;
		}

		internal bool? IsDelegateOfPrincipal(Participant principal, Participant delegateParticipant)
		{
			if (this.outboundOptions == null || this.outboundOptions.RecipientCache == null)
			{
				return null;
			}
			ProxyAddress proxyAddressForParticipant = OutboundAddressCache.GetProxyAddressForParticipant(principal);
			if (proxyAddressForParticipant == null)
			{
				return null;
			}
			Result<ADRawEntry> result;
			if (this.outboundOptions.RecipientCache.TryGetValue(proxyAddressForParticipant, out result))
			{
				ADRawEntry data = result.Data;
				if (data == null)
				{
					return null;
				}
				MultiValuedProperty<ADObjectId> multiValuedProperty = data[ADRecipientSchema.GrantSendOnBehalfTo] as MultiValuedProperty<ADObjectId>;
				if (multiValuedProperty == null || multiValuedProperty.Count == 0)
				{
					return new bool?(false);
				}
				ProxyAddress proxyAddressForParticipant2 = OutboundAddressCache.GetProxyAddressForParticipant(delegateParticipant);
				if (proxyAddressForParticipant2 == null)
				{
					return null;
				}
				Result<ADRawEntry> result2;
				if (this.outboundOptions.RecipientCache.TryGetValue(proxyAddressForParticipant2, out result2))
				{
					ADRawEntry data2 = result2.Data;
					if (data2 == null)
					{
						return null;
					}
					ADObjectId adobjectId = data2[ADObjectSchema.Id] as ADObjectId;
					if (adobjectId == null)
					{
						goto IL_13A;
					}
					using (MultiValuedProperty<ADObjectId>.Enumerator enumerator = multiValuedProperty.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ADObjectId x = enumerator.Current;
							if (ADObjectId.Equals(x, adobjectId))
							{
								return new bool?(true);
							}
						}
						goto IL_13A;
					}
				}
				return null;
			}
			IL_13A:
			return new bool?(false);
		}

		protected override IADRecipientCache GetRecipientCache(int count)
		{
			return this.outboundOptions.InternalGetRecipientCache(count);
		}

		protected override bool CanResolveParticipant(Participant participant)
		{
			return this.outboundOptions.IsSenderTrusted || (participant != null && participant.RoutingType == "EX");
		}

		protected override string TargetResolutionType
		{
			get
			{
				return "SMTP";
			}
		}

		private static ProxyAddress GetProxyAddressForParticipant(Participant participant)
		{
			if (string.IsNullOrEmpty(participant.RoutingType) || string.IsNullOrEmpty(participant.EmailAddress))
			{
				return null;
			}
			if (participant.RoutingType.Equals("SMTP", StringComparison.OrdinalIgnoreCase))
			{
				if (SmtpProxyAddress.IsValidProxyAddress(participant.EmailAddress))
				{
					return new SmtpProxyAddress(participant.EmailAddress, true);
				}
				return null;
			}
			else
			{
				if (!participant.RoutingType.Equals("EX", StringComparison.OrdinalIgnoreCase))
				{
					return null;
				}
				return ProxyAddress.Parse(ProxyAddressPrefix.LegacyDN.PrimaryPrefix, participant.EmailAddress);
			}
		}

		private OutboundConversionOptions outboundOptions;
	}
}
