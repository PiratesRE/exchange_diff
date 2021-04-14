using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactEmailAddressesEnumerator : IEnumerable<Tuple<EmailAddress, EmailAddressIndex>>, IEnumerable
	{
		public ContactEmailAddressesEnumerator(IStorePropertyBag propertyBag, string clientInfoString)
		{
			this.propertyBag = propertyBag;
			this.clientInfoString = clientInfoString;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generics interface of GetEnumerator.");
		}

		public IEnumerator<Tuple<EmailAddress, EmailAddressIndex>> GetEnumerator()
		{
			string partnerNetworkId = this.propertyBag.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
			if (StringComparer.OrdinalIgnoreCase.Equals(partnerNetworkId, WellKnownNetworkNames.Facebook))
			{
				if (ClientInfo.OWA.IsMatch(this.clientInfoString))
				{
					string protectedEmail = this.propertyBag.TryGetProperty(InternalSchema.ProtectedEmailAddress) as string;
					if (!string.IsNullOrWhiteSpace(protectedEmail))
					{
						EmailAddress emailAddress = new EmailAddress
						{
							RoutingType = "smtp",
							Address = protectedEmail,
							Name = protectedEmail
						};
						yield return new Tuple<EmailAddress, EmailAddressIndex>(emailAddress, EmailAddressIndex.None);
					}
				}
			}
			else
			{
				foreach (EmailAddressProperties properties in EmailAddressProperties.PropertySets)
				{
					EmailAddress emailAddress2 = properties.GetFrom(this.propertyBag);
					if (emailAddress2 != null)
					{
						yield return new Tuple<EmailAddress, EmailAddressIndex>(emailAddress2, properties.EmailAddressIndex);
					}
				}
			}
			yield break;
		}

		public static readonly StorePropertyDefinition[] Properties = PropertyDefinitionCollection.Merge<StorePropertyDefinition>(EmailAddressProperties.AllProperties, new StorePropertyDefinition[]
		{
			ContactSchema.PartnerNetworkId,
			InternalSchema.ProtectedEmailAddress
		});

		private readonly IStorePropertyBag propertyBag;

		private readonly string clientInfoString;
	}
}
