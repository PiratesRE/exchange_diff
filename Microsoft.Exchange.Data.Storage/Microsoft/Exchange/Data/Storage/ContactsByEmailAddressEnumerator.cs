using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactsByEmailAddressEnumerator : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		public ContactsByEmailAddressEnumerator(IFolder contactsFolder, PropertyDefinition[] requestedProperties, string emailAddress) : this(contactsFolder, requestedProperties, new string[]
		{
			emailAddress
		})
		{
		}

		public ContactsByEmailAddressEnumerator(IFolder contactsFolder, PropertyDefinition[] requestedProperties, IEnumerable<string> emailAddresses)
		{
			ArgumentValidator.ThrowIfNull("contactsFolder", contactsFolder);
			ArgumentValidator.ThrowIfNull("requestedProperties", requestedProperties);
			ArgumentValidator.ThrowIfNull("emailAddress", emailAddresses);
			this.contactsFolder = contactsFolder;
			this.requestedProperties = requestedProperties;
			this.emailAddresses = emailAddresses;
		}

		public IEnumerator<IStorePropertyBag> GetEnumerator()
		{
			foreach (PropertyDefinition property in ContactsByEmailAddressEnumerator.EmailAddressSearchProperties)
			{
				IEnumerable<IStorePropertyBag> contactsEnumerator = new ContactsByPropertyValueEnumerator(this.contactsFolder, property, this.emailAddresses, this.requestedProperties);
				foreach (IStorePropertyBag storePropertyBag in contactsEnumerator)
				{
					yield return storePropertyBag;
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generics interface of GetEnumerator.");
		}

		private readonly PropertyDefinition[] requestedProperties;

		private readonly IEnumerable<string> emailAddresses;

		public static PropertyDefinition[] EmailAddressSearchProperties = new PropertyDefinition[]
		{
			InternalSchema.Email1EmailAddress,
			InternalSchema.Email2EmailAddress,
			InternalSchema.Email3EmailAddress,
			ContactProtectedPropertiesSchema.ProtectedEmailAddress
		};

		private readonly IFolder contactsFolder;
	}
}
