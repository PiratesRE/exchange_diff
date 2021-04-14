using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ContactDisplayNamePropertyRule : PriorityBasedDisplayNamePropertyRule
	{
		protected override IList<PriorityBasedDisplayNamePropertyRule.CandidateProperty> GetCandidateProperties()
		{
			return new List<PriorityBasedDisplayNamePropertyRule.CandidateProperty>
			{
				new PriorityBasedDisplayNamePropertyRule.CandidateProperty(new List<NativeStorePropertyDefinition>
				{
					InternalSchema.GivenName,
					InternalSchema.MiddleName,
					InternalSchema.Surname,
					InternalSchema.DisplayName
				}, new PriorityBasedDisplayNamePropertyRule.CandidateProperty.DisplayNameValueDelegate(ContactDisplayNamePropertyRule.GetDisplayNamesFromNameProperties)),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.CompanyName),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.Email1DisplayName),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.Email1EmailAddress),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.Email2DisplayName),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.Email2EmailAddress),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.Email3DisplayName),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.Email3EmailAddress),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.IMAddress),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.MobilePhone),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.HomePhone),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.BusinessPhoneNumber),
				new PriorityBasedDisplayNamePropertyRule.SimpleCandidateProperty(InternalSchema.OtherTelephone)
			};
		}

		private static void GetDisplayNamesFromNameProperties(ICorePropertyBag propertyBag, out string displayNameFirstLast, out string displayNameLastFirst)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.GivenName, string.Empty);
			string valueOrDefault2 = propertyBag.GetValueOrDefault<string>(InternalSchema.MiddleName, string.Empty);
			string valueOrDefault3 = propertyBag.GetValueOrDefault<string>(InternalSchema.Surname, string.Empty);
			string valueOrDefault4 = propertyBag.GetValueOrDefault<string>(InternalSchema.DisplayName, string.Empty);
			displayNameFirstLast = ContactDisplayNamePropertyRule.BuildDisplayName(valueOrDefault4, " ", new string[]
			{
				valueOrDefault,
				valueOrDefault2,
				valueOrDefault3
			});
			displayNameLastFirst = ContactDisplayNamePropertyRule.BuildDisplayName(valueOrDefault4, " ", new string[]
			{
				valueOrDefault3,
				valueOrDefault,
				valueOrDefault2
			});
		}

		private static string BuildDisplayName(string defaultValue, string separator, params string[] nameParts)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			foreach (string value in nameParts)
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(separator);
					}
					stringBuilder.Append(value);
				}
			}
			if (stringBuilder.Length > 0)
			{
				return stringBuilder.ToString();
			}
			return defaultValue;
		}

		internal const string FirstNameLastNameSeparator = " ";
	}
}
