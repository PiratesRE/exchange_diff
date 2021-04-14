using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConversionItemParticipants : IConversionParticipantList
	{
		internal static HashSet<NativeStorePropertyDefinition> AllCacheProperties
		{
			get
			{
				return ConversionItemParticipants.cacheProperties;
			}
		}

		private static HashSet<NativeStorePropertyDefinition> GetCacheProperties(bool skipSkippable)
		{
			HashSet<NativeStorePropertyDefinition> hashSet = new HashSet<NativeStorePropertyDefinition>();
			foreach (ConversionItemParticipants.ParticipantDefinitionEntry participantDefinitionEntry in ConversionItemParticipants.ParticipantEntries)
			{
				if (!skipSkippable || !participantDefinitionEntry.IsSkippable)
				{
					EmbeddedParticipantProperty participantProperty = participantDefinitionEntry.ParticipantProperty;
					hashSet.Add(participantProperty.DisplayNamePropertyDefinition);
					hashSet.Add(participantProperty.EmailAddressPropertyDefinition);
					hashSet.Add(participantProperty.RoutingTypePropertyDefinition);
					hashSet.Add(participantProperty.EntryIdPropertyDefinition);
					if (participantProperty.SmtpAddressPropertyDefinition != null)
					{
						hashSet.Add(participantProperty.SmtpAddressPropertyDefinition);
					}
					if (participantProperty.SipUriPropertyDefinition != null)
					{
						hashSet.Add(participantProperty.SipUriPropertyDefinition);
					}
					if (participantProperty.SidPropertyDefinition != null)
					{
						hashSet.Add(participantProperty.SidPropertyDefinition);
					}
					if (participantProperty.GuidPropertyDefinition != null)
					{
						hashSet.Add(participantProperty.GuidPropertyDefinition);
					}
				}
			}
			return hashSet;
		}

		internal static bool IsAnyCacheProperty(NativeStorePropertyDefinition propertyDefinition)
		{
			return ConversionItemParticipants.AllCacheProperties.Contains(propertyDefinition);
		}

		internal static EmbeddedParticipantProperty GetEmbeddedParticipantProperty(ConversionItemParticipants.ParticipantIndex index)
		{
			return ConversionItemParticipants.ParticipantEntries[(int)index].ParticipantProperty;
		}

		internal HashSet<NativeStorePropertyDefinition> CacheProperties
		{
			get
			{
				if (!this.skipSkippable)
				{
					return ConversionItemParticipants.cacheProperties;
				}
				return ConversionItemParticipants.cachePropertiesSkipNonMime;
			}
		}

		internal bool IsCacheProperty(NativeStorePropertyDefinition propertyDefinition)
		{
			return this.CacheProperties.Contains(propertyDefinition);
		}

		internal ConversionItemParticipants(MemoryPropertyBag propertyBag, bool skipSkippable)
		{
			this.propertyBag = propertyBag;
			this.skipSkippable = skipSkippable;
		}

		internal object TryGetProperty(NativeStorePropertyDefinition property, object value)
		{
			return this.propertyBag[property];
		}

		public int Count
		{
			get
			{
				return 11;
			}
		}

		public bool IsConversionParticipantAlwaysResolvable(int index)
		{
			return ConversionItemParticipants.ParticipantEntries[index].IsAlwaysResolvable;
		}

		public Participant this[int index]
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<Participant>(ConversionItemParticipants.ParticipantEntries[index].ParticipantProperty);
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(ConversionItemParticipants.ParticipantEntries[index].ParticipantProperty, value);
			}
		}

		internal Participant this[ConversionItemParticipants.ParticipantIndex index]
		{
			get
			{
				return this[(int)index];
			}
			set
			{
				this[(int)index] = value;
			}
		}

		internal static readonly ConversionItemParticipants.ParticipantDefinitionEntry[] ParticipantEntries = new ConversionItemParticipants.ParticipantDefinitionEntry[]
		{
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.ReceivedBy, ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable | ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsSkippable),
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.ReceivedRepresenting, ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable | ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsSkippable),
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.From, ConversionItemParticipants.ParticipantDefinitionEntryFlags.None),
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.Sender, ConversionItemParticipants.ParticipantDefinitionEntryFlags.None),
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.OriginalFrom, ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable),
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.OriginalSender, ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable),
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.OriginalAuthor, ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable),
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.ReadReceiptAddressee, ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable),
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.ContactEmail1, ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable),
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.ContactEmail2, ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable),
			new ConversionItemParticipants.ParticipantDefinitionEntry(InternalSchema.ContactEmail3, ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable)
		};

		private static HashSet<NativeStorePropertyDefinition> cacheProperties = ConversionItemParticipants.GetCacheProperties(false);

		private static HashSet<NativeStorePropertyDefinition> cachePropertiesSkipNonMime = ConversionItemParticipants.GetCacheProperties(true);

		protected readonly bool skipSkippable;

		protected MemoryPropertyBag propertyBag;

		internal enum ParticipantIndex
		{
			ReceivedBy,
			ReceivedRepresenting,
			From,
			Sender,
			OriginalFrom,
			OriginalSender,
			OriginalAuthor,
			ReadReceipt,
			ContactEmail1,
			ContactEmail2,
			ContactEmail3,
			TotalItemParticipants
		}

		[Flags]
		internal enum ParticipantDefinitionEntryFlags
		{
			None = 0,
			IsAlwaysResolvable = 1,
			IsSkippable = 2
		}

		internal class ParticipantDefinitionEntry
		{
			internal ParticipantDefinitionEntry(EmbeddedParticipantProperty participantDefinition, ConversionItemParticipants.ParticipantDefinitionEntryFlags flags)
			{
				this.participantProperty = participantDefinition;
				this.flags = flags;
			}

			internal EmbeddedParticipantProperty ParticipantProperty
			{
				get
				{
					return this.participantProperty;
				}
			}

			internal bool IsAlwaysResolvable
			{
				get
				{
					return (this.flags & ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable) == ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsAlwaysResolvable;
				}
			}

			internal bool IsSkippable
			{
				get
				{
					return (this.flags & ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsSkippable) == ConversionItemParticipants.ParticipantDefinitionEntryFlags.IsSkippable;
				}
			}

			private EmbeddedParticipantProperty participantProperty;

			private ConversionItemParticipants.ParticipantDefinitionEntryFlags flags;
		}
	}
}
