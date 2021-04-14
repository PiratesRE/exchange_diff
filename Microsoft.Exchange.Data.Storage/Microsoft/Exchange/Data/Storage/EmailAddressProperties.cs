using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EmailAddressProperties
	{
		public static NativeStorePropertyDefinition[] AllProperties
		{
			get
			{
				if (EmailAddressProperties.allProperties == null)
				{
					List<NativeStorePropertyDefinition> list = new List<NativeStorePropertyDefinition>(EmailAddressProperties.PropertySets.Length * 3);
					foreach (EmailAddressProperties emailAddressProperties in EmailAddressProperties.PropertySets)
					{
						list.Add(emailAddressProperties.RoutingType);
						list.Add(emailAddressProperties.Address);
						list.Add(emailAddressProperties.DisplayName);
						list.Add(emailAddressProperties.OriginalDisplayName);
						list.Add(emailAddressProperties.OriginalEntryId);
					}
					EmailAddressProperties.allProperties = list.ToArray();
				}
				return EmailAddressProperties.allProperties;
			}
		}

		private EmailAddressProperties()
		{
		}

		public NativeStorePropertyDefinition RoutingType { get; private set; }

		public NativeStorePropertyDefinition Address { get; private set; }

		public NativeStorePropertyDefinition DisplayName { get; private set; }

		public NativeStorePropertyDefinition OriginalDisplayName { get; private set; }

		public NativeStorePropertyDefinition OriginalEntryId { get; private set; }

		public EmailAddressIndex EmailAddressIndex { get; private set; }

		public NativeStorePropertyDefinition[] Properties
		{
			get
			{
				return new NativeStorePropertyDefinition[]
				{
					this.RoutingType,
					this.Address,
					this.DisplayName,
					this.OriginalDisplayName,
					this.OriginalEntryId
				};
			}
		}

		public SortBy[] AscendingSortBy
		{
			get
			{
				if (this.ascendingSortBy == null)
				{
					this.ascendingSortBy = new SortBy[]
					{
						new SortBy(this.Address, SortOrder.Ascending)
					};
				}
				return this.ascendingSortBy;
			}
		}

		public EmailAddress GetFrom(IStorePropertyBag propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(this.Address, null);
			string valueOrDefault2 = propertyBag.GetValueOrDefault<string>(this.DisplayName, null);
			string valueOrDefault3 = propertyBag.GetValueOrDefault<string>(this.OriginalDisplayName, null);
			string text = propertyBag.GetValueOrDefault<string>(this.RoutingType, null);
			if (!string.IsNullOrEmpty(valueOrDefault) && string.IsNullOrEmpty(text))
			{
				Participant participant = null;
				if (Participant.TryParse(valueOrDefault, out participant))
				{
					text = participant.RoutingType;
				}
			}
			return new EmailAddress
			{
				RoutingType = text,
				Address = valueOrDefault,
				Name = valueOrDefault2,
				OriginalDisplayName = valueOrDefault3
			};
		}

		public void SetTo(IStorePropertyBag propertyBag, EmailAddress emailAddress)
		{
			EmailAddressProperties.SetOrDeleteValue(propertyBag, this.RoutingType, emailAddress.RoutingType);
			EmailAddressProperties.SetOrDeleteValue(propertyBag, this.Address, emailAddress.Address);
			EmailAddressProperties.SetOrDeleteValue(propertyBag, this.DisplayName, emailAddress.Name);
			EmailAddressProperties.SetOrDeleteValue(propertyBag, this.OriginalDisplayName, emailAddress.OriginalDisplayName);
		}

		private static void SetOrDeleteValue(IStorePropertyBag propertyBag, NativeStorePropertyDefinition property, string value)
		{
			if (value == null)
			{
				propertyBag.Delete(property);
				return;
			}
			propertyBag[property] = value;
		}

		private static NativeStorePropertyDefinition[] allProperties;

		public static readonly EmailAddressProperties Email1 = new EmailAddressProperties
		{
			RoutingType = InternalSchema.Email1AddrType,
			Address = InternalSchema.Email1EmailAddress,
			DisplayName = InternalSchema.Email1DisplayName,
			OriginalDisplayName = InternalSchema.Email1OriginalDisplayName,
			EmailAddressIndex = EmailAddressIndex.Email1,
			OriginalEntryId = InternalSchema.Email1OriginalEntryID
		};

		public static readonly EmailAddressProperties Email2 = new EmailAddressProperties
		{
			RoutingType = InternalSchema.Email2AddrType,
			Address = InternalSchema.Email2EmailAddress,
			DisplayName = InternalSchema.Email2DisplayName,
			OriginalDisplayName = InternalSchema.Email2OriginalDisplayName,
			EmailAddressIndex = EmailAddressIndex.Email2,
			OriginalEntryId = InternalSchema.Email2OriginalEntryID
		};

		public static readonly EmailAddressProperties Email3 = new EmailAddressProperties
		{
			RoutingType = InternalSchema.Email3AddrType,
			Address = InternalSchema.Email3EmailAddress,
			DisplayName = InternalSchema.Email3DisplayName,
			OriginalDisplayName = InternalSchema.Email3OriginalDisplayName,
			EmailAddressIndex = EmailAddressIndex.Email3,
			OriginalEntryId = InternalSchema.Email3OriginalEntryID
		};

		public static readonly EmailAddressProperties[] PropertySets = new EmailAddressProperties[]
		{
			EmailAddressProperties.Email1,
			EmailAddressProperties.Email2,
			EmailAddressProperties.Email3
		};

		private SortBy[] ascendingSortBy;
	}
}
