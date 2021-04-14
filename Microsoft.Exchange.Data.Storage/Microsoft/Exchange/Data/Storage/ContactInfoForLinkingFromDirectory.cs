using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ContactInfoForLinkingFromDirectory : IContactLinkingMatchProperties
	{
		public Guid GALLinkID { get; protected set; }

		public byte[] AddressBookEntryId { get; protected set; }

		public string[] SmtpAddressCache { get; protected set; }

		public HashSet<string> EmailAddresses { get; protected set; }

		public string IMAddress { get; protected set; }

		protected ContactInfoForLinkingFromDirectory(ADRawEntry person)
		{
			this.GALLinkID = person.Id.ObjectGuid;
			this.AddressBookEntryId = ContactInfoForLinkingFromDirectory.GetAddressBookId(person);
			this.SmtpAddressCache = ContactInfoForLinkingFromDirectory.GetSmtpAddressCache(person);
			this.EmailAddresses = ContactInfoForLinkingFromDirectory.GetEmailAddresses(person);
			this.IMAddress = (person[ADUserSchema.RTCSIPPrimaryUserAddress] as string);
		}

		public static ContactInfoForLinkingFromDirectory Create(ADRawEntry person)
		{
			Util.ThrowOnNullArgument(person, "person");
			return new ContactInfoForLinkingFromDirectory(person);
		}

		public static ContactInfoForLinkingFromDirectory Create(IRecipientSession adSession, ADObjectId adObjectId)
		{
			Util.ThrowOnNullArgument(adSession, "adSession");
			Util.ThrowOnNullArgument(adObjectId, "adObjectId");
			ADRawEntry adrawEntry = adSession.ReadADRawEntry(adObjectId, ContactInfoForLinkingFromDirectory.RequiredADProperties);
			if (adrawEntry != null)
			{
				return new ContactInfoForLinkingFromDirectory(adrawEntry);
			}
			return null;
		}

		private static HashSet<string> GetEmailAddresses(ADRawEntry person)
		{
			HashSet<string> hashSet = new HashSet<string>();
			object obj;
			if (person.TryGetValueWithoutDefault(ADRecipientSchema.EmailAddresses, out obj))
			{
				ProxyAddressCollection proxyAddressCollection = obj as ProxyAddressCollection;
				if (proxyAddressCollection != null)
				{
					foreach (ProxyAddress proxyAddress in proxyAddressCollection)
					{
						SmtpProxyAddress smtpProxyAddress = proxyAddress as SmtpProxyAddress;
						if (smtpProxyAddress != null)
						{
							string text = ContactInfoForLinking.CanonicalizeEmailAddress(smtpProxyAddress.SmtpAddress);
							if (!string.IsNullOrEmpty(text))
							{
								hashSet.Add(text);
							}
						}
					}
				}
			}
			return hashSet;
		}

		private static string[] GetSmtpAddressCache(ADRawEntry person)
		{
			List<string> list = new List<string>(1);
			object obj;
			if (person.TryGetValueWithoutDefault(ADRecipientSchema.EmailAddresses, out obj))
			{
				ProxyAddressCollection proxyAddressCollection = obj as ProxyAddressCollection;
				if (proxyAddressCollection != null)
				{
					foreach (ProxyAddress proxyAddress in proxyAddressCollection)
					{
						SmtpProxyAddress smtpProxyAddress = proxyAddress as SmtpProxyAddress;
						if (smtpProxyAddress != null)
						{
							string text = smtpProxyAddress.ToString();
							if (!string.IsNullOrEmpty(text))
							{
								list.Add(text);
							}
						}
					}
				}
			}
			return list.ToArray();
		}

		private static byte[] GetAddressBookId(ADRawEntry person)
		{
			string text = (string)person[ADRecipientSchema.LegacyExchangeDN];
			RecipientDisplayType? recipientDisplayType = (RecipientDisplayType?)person[ADRecipientSchema.RecipientDisplayType];
			Eidt eidt;
			if (!string.IsNullOrEmpty(text) && recipientDisplayType != null && ContactInfoForLinkingFromDirectory.TryGetEidt(recipientDisplayType.Value, out eidt))
			{
				return Microsoft.Exchange.Data.Storage.AddressBookEntryId.MakeAddressBookEntryID(text, eidt);
			}
			return null;
		}

		private static bool TryGetEidt(RecipientDisplayType type, out Eidt eidt)
		{
			if (type <= RecipientDisplayType.ArbitrationMailbox)
			{
				if (type <= RecipientDisplayType.ACLableSyncedUSGasContact)
				{
					if (type != RecipientDisplayType.ACLableSyncedRemoteMailUser && type != RecipientDisplayType.ACLableSyncedUSGasContact)
					{
						goto IL_76;
					}
				}
				else
				{
					if (type == RecipientDisplayType.MailboxUser)
					{
						goto IL_71;
					}
					switch (type)
					{
					case RecipientDisplayType.RemoteMailUser:
						break;
					case RecipientDisplayType.ConferenceRoomMailbox:
					case RecipientDisplayType.EquipmentMailbox:
					case RecipientDisplayType.ArbitrationMailbox:
						goto IL_71;
					case (RecipientDisplayType)9:
						goto IL_76;
					default:
						goto IL_76;
					}
				}
			}
			else if (type <= RecipientDisplayType.ACLableMailboxUser)
			{
				if (type != RecipientDisplayType.TeamMailboxUser && type != RecipientDisplayType.ACLableMailboxUser)
				{
					goto IL_76;
				}
				goto IL_71;
			}
			else if (type != RecipientDisplayType.ACLableRemoteMailUser)
			{
				if (type != RecipientDisplayType.ACLableTeamMailboxUser)
				{
					goto IL_76;
				}
				goto IL_71;
			}
			eidt = Eidt.RemoteUser;
			return true;
			IL_71:
			eidt = Eidt.User;
			return true;
			IL_76:
			eidt = Eidt.User;
			return false;
		}

		internal static readonly ADPropertyDefinition[] RequiredADProperties = new ADPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.RecipientDisplayType,
			ADRecipientSchema.EmailAddresses,
			ADUserSchema.RTCSIPPrimaryUserAddress
		};
	}
}
