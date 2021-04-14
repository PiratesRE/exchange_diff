using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal class RecipientSchema
	{
		public static ADPropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return RecipientSchema.propertyDefinitions;
			}
		}

		public static string[] AttributeNames
		{
			get
			{
				if (RecipientSchema.attributeNames != null)
				{
					return RecipientSchema.attributeNames;
				}
				string[] result;
				lock (RecipientSchema.syncObject)
				{
					if (RecipientSchema.attributeNames != null)
					{
						result = RecipientSchema.attributeNames;
					}
					else
					{
						List<string> list = new List<string>();
						IEnumerable<ADPropertyDefinition> enumerable = RecipientSchema.propertyDefinitions;
						foreach (ADPropertyDefinition adpropertyDefinition in enumerable)
						{
							if (adpropertyDefinition.LdapDisplayName != null)
							{
								string ldapDisplayName = adpropertyDefinition.LdapDisplayName;
								if (!list.Contains(ldapDisplayName))
								{
									list.Add(ldapDisplayName);
								}
							}
							else
							{
								foreach (PropertyDefinition propertyDefinition in adpropertyDefinition.SupportingProperties)
								{
									ADPropertyDefinition adpropertyDefinition2 = propertyDefinition as ADPropertyDefinition;
									string ldapDisplayName2 = adpropertyDefinition2.LdapDisplayName;
									if (ldapDisplayName2 == null)
									{
										throw new InvalidOperationException("Unable to determine ldapDisplayName for " + adpropertyDefinition.Name);
									}
									if (!list.Contains(ldapDisplayName2))
									{
										list.Add(ldapDisplayName2);
									}
								}
							}
						}
						list.Remove("homeMDB");
						list.Remove("groupType");
						list.Remove("objectSid");
						list.Remove("mailNickname");
						list.Remove("msExchHomeServerName");
						list.Remove("msExchMasterAccountSid");
						RecipientSchema.attributeNames = list.ToArray();
						result = RecipientSchema.attributeNames;
					}
				}
				return result;
			}
		}

		private static readonly object syncObject = new object();

		private static string[] attributeNames;

		private static ADPropertyDefinition[] propertyDefinitions = new ADPropertyDefinition[]
		{
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.RequireAllSendersAreAuthenticated,
			ADRecipientSchema.AntispamBypassEnabled,
			ADRecipientSchema.SafeSendersHash,
			ADRecipientSchema.SafeRecipientsHash,
			ADRecipientSchema.BlockedSendersHash,
			ADRecipientSchema.SCLDeleteThreshold,
			ADRecipientSchema.SCLDeleteEnabled,
			ADRecipientSchema.SCLRejectThreshold,
			ADRecipientSchema.SCLRejectEnabled,
			ADRecipientSchema.SCLQuarantineThreshold,
			ADRecipientSchema.SCLQuarantineEnabled,
			ADRecipientSchema.MasterAccountSid,
			ADRecipientSchema.WindowsLiveID,
			ADMailboxRecipientSchema.Sid,
			ADObjectSchema.ExchangeVersion,
			ADObjectSchema.ObjectClass
		};
	}
}
