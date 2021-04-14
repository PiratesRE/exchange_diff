using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[Serializable]
	public class RecipientEnforcementProvisioningPolicy : EnforcementProvisioningPolicy
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return RecipientEnforcementProvisioningPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RecipientEnforcementProvisioningPolicy.MostDerivedClass;
			}
		}

		internal override ICollection<Type> SupportedPresentationObjectTypes
		{
			get
			{
				return ProvisioningHelper.AllSupportedRecipientTypes;
			}
		}

		internal override IEnumerable<IProvisioningEnforcement> ProvisioningEnforcementRules
		{
			get
			{
				return RecipientEnforcementProvisioningPolicy.provisioningEnforcements;
			}
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		public RecipientEnforcementProvisioningPolicy()
		{
			base.Name = "Recipient Quota Policy";
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> DistributionListCountQuota
		{
			get
			{
				return (Unlimited<int>)this[RecipientEnforcementProvisioningPolicySchema.DistributionListCountQuota];
			}
			set
			{
				this[RecipientEnforcementProvisioningPolicySchema.DistributionListCountQuota] = value;
			}
		}

		public int? DistributionListCount
		{
			get
			{
				return (int?)this[RecipientEnforcementProvisioningPolicySchema.DistributionListCount];
			}
			internal set
			{
				this[RecipientEnforcementProvisioningPolicySchema.DistributionListCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MailboxCountQuota
		{
			get
			{
				return (Unlimited<int>)this[RecipientEnforcementProvisioningPolicySchema.MailboxCountQuota];
			}
			set
			{
				this[RecipientEnforcementProvisioningPolicySchema.MailboxCountQuota] = value;
			}
		}

		public int? MailboxCount
		{
			get
			{
				return (int?)this[RecipientEnforcementProvisioningPolicySchema.MailboxCount];
			}
			internal set
			{
				this[RecipientEnforcementProvisioningPolicySchema.MailboxCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MailUserCountQuota
		{
			get
			{
				return (Unlimited<int>)this[RecipientEnforcementProvisioningPolicySchema.MailUserCountQuota];
			}
			set
			{
				this[RecipientEnforcementProvisioningPolicySchema.MailUserCountQuota] = value;
			}
		}

		public int? MailUserCount
		{
			get
			{
				return (int?)this[RecipientEnforcementProvisioningPolicySchema.MailUserCount];
			}
			internal set
			{
				this[RecipientEnforcementProvisioningPolicySchema.MailUserCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> ContactCountQuota
		{
			get
			{
				return (Unlimited<int>)this[RecipientEnforcementProvisioningPolicySchema.ContactCountQuota];
			}
			set
			{
				this[RecipientEnforcementProvisioningPolicySchema.ContactCountQuota] = value;
			}
		}

		public int? ContactCount
		{
			get
			{
				return (int?)this[RecipientEnforcementProvisioningPolicySchema.ContactCount];
			}
			internal set
			{
				this[RecipientEnforcementProvisioningPolicySchema.ContactCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> TeamMailboxCountQuota
		{
			get
			{
				return (Unlimited<int>)this[RecipientEnforcementProvisioningPolicySchema.TeamMailboxCountQuota];
			}
			set
			{
				this[RecipientEnforcementProvisioningPolicySchema.TeamMailboxCountQuota] = value;
			}
		}

		public int? TeamMailboxCount
		{
			get
			{
				return (int?)this[RecipientEnforcementProvisioningPolicySchema.TeamMailboxCount];
			}
			internal set
			{
				this[RecipientEnforcementProvisioningPolicySchema.TeamMailboxCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> PublicFolderMailboxCountQuota
		{
			get
			{
				return (Unlimited<int>)this[RecipientEnforcementProvisioningPolicySchema.PublicFolderMailboxCountQuota];
			}
			set
			{
				this[RecipientEnforcementProvisioningPolicySchema.PublicFolderMailboxCountQuota] = value;
			}
		}

		public int? PublicFolderMailboxCount
		{
			get
			{
				return (int?)this[RecipientEnforcementProvisioningPolicySchema.PublicFolderMailboxCount];
			}
			internal set
			{
				this[RecipientEnforcementProvisioningPolicySchema.PublicFolderMailboxCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MailPublicFolderCountQuota
		{
			get
			{
				return (Unlimited<int>)this[RecipientEnforcementProvisioningPolicySchema.MailPublicFolderCountQuota];
			}
			set
			{
				this[RecipientEnforcementProvisioningPolicySchema.MailPublicFolderCountQuota] = value;
			}
		}

		public int? MailPublicFolderCount
		{
			get
			{
				return (int?)this[RecipientEnforcementProvisioningPolicySchema.MailPublicFolderCount];
			}
			internal set
			{
				this[RecipientEnforcementProvisioningPolicySchema.MailPublicFolderCount] = value;
			}
		}

		internal MultiValuedProperty<string> ObjectCountQuota
		{
			get
			{
				return (MultiValuedProperty<string>)this[RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota];
			}
			set
			{
				this[RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota] = value;
			}
		}

		internal static object DistributionListCountQuotaGetter(IPropertyBag propertyBag)
		{
			return RecipientEnforcementProvisioningPolicy.ObjectCountQuotaGetter(RecipientEnforcementProvisioningPolicySchema.DistributionListCountQuota, "All Groups(VLV)", propertyBag);
		}

		internal static void DistributionListCountQuotaSetter(object value, IPropertyBag propertyBag)
		{
			RecipientEnforcementProvisioningPolicy.ObjectCountQuotaSetter("All Groups(VLV)", (Unlimited<int>)value, propertyBag);
		}

		internal static object MailboxCountQuotaGetter(IPropertyBag propertyBag)
		{
			return RecipientEnforcementProvisioningPolicy.ObjectCountQuotaGetter(RecipientEnforcementProvisioningPolicySchema.MailboxCountQuota, "All Mailboxes(VLV)", propertyBag);
		}

		internal static void MailboxCountQuotaSetter(object value, IPropertyBag propertyBag)
		{
			RecipientEnforcementProvisioningPolicy.ObjectCountQuotaSetter("All Mailboxes(VLV)", (Unlimited<int>)value, propertyBag);
		}

		internal static object MailUserCountQuotaGetter(IPropertyBag propertyBag)
		{
			return RecipientEnforcementProvisioningPolicy.ObjectCountQuotaGetter(RecipientEnforcementProvisioningPolicySchema.MailUserCountQuota, "All Mail Users(VLV)", propertyBag);
		}

		internal static void MailUserCountQuotaSetter(object value, IPropertyBag propertyBag)
		{
			RecipientEnforcementProvisioningPolicy.ObjectCountQuotaSetter("All Mail Users(VLV)", (Unlimited<int>)value, propertyBag);
		}

		internal static object ContactCountQuotaGetter(IPropertyBag propertyBag)
		{
			return RecipientEnforcementProvisioningPolicy.ObjectCountQuotaGetter(RecipientEnforcementProvisioningPolicySchema.ContactCountQuota, "All Contacts(VLV)", propertyBag);
		}

		internal static void ContactCountQuotaSetter(object value, IPropertyBag propertyBag)
		{
			RecipientEnforcementProvisioningPolicy.ObjectCountQuotaSetter("All Contacts(VLV)", (Unlimited<int>)value, propertyBag);
		}

		internal static object TeamMailboxCountQuotaGetter(IPropertyBag propertyBag)
		{
			return RecipientEnforcementProvisioningPolicy.ObjectCountQuotaGetter(RecipientEnforcementProvisioningPolicySchema.TeamMailboxCountQuota, "TeamMailboxes(VLV)", propertyBag);
		}

		internal static void TeamMailboxCountQuotaSetter(object value, IPropertyBag propertyBag)
		{
			RecipientEnforcementProvisioningPolicy.ObjectCountQuotaSetter("TeamMailboxes(VLV)", (Unlimited<int>)value, propertyBag);
		}

		internal static object PublicFolderMailboxCountQuotaGetter(IPropertyBag propertyBag)
		{
			return RecipientEnforcementProvisioningPolicy.ObjectCountQuotaGetter(RecipientEnforcementProvisioningPolicySchema.PublicFolderMailboxCountQuota, "PublicFolderMailboxes(VLV)", propertyBag);
		}

		internal static void PublicFolderMailboxCountQuotaSetter(object value, IPropertyBag propertyBag)
		{
			RecipientEnforcementProvisioningPolicy.ObjectCountQuotaSetter("PublicFolderMailboxes(VLV)", (Unlimited<int>)value, propertyBag);
		}

		internal static object MailPublicFolderCountQuotaGetter(IPropertyBag propertyBag)
		{
			return RecipientEnforcementProvisioningPolicy.ObjectCountQuotaGetter(RecipientEnforcementProvisioningPolicySchema.MailPublicFolderCountQuota, "MailPublicFolders(VLV)", propertyBag);
		}

		internal static void MailPublicFolderCountQuotaSetter(object value, IPropertyBag propertyBag)
		{
			RecipientEnforcementProvisioningPolicy.ObjectCountQuotaSetter("MailPublicFolders(VLV)", (Unlimited<int>)value, propertyBag);
		}

		internal static Unlimited<int> ObjectCountQuotaGetter(ADPropertyDefinition adPropertyDefinition, string key, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota];
			if (multiValuedProperty == null || multiValuedProperty.Count == 0)
			{
				return Unlimited<int>.UnlimitedValue;
			}
			foreach (string text in multiValuedProperty)
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculatePropertyGeneric(adPropertyDefinition.Name), adPropertyDefinition, propertyBag[RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota]));
				}
				if (string.Equals(key, array[0], StringComparison.OrdinalIgnoreCase))
				{
					Unlimited<int> result = 0;
					Exception ex = null;
					try
					{
						result = Unlimited<int>.Parse(array[1]);
					}
					catch (ArgumentNullException ex2)
					{
						ex = ex2;
					}
					catch (FormatException ex3)
					{
						ex = ex3;
					}
					catch (OverflowException ex4)
					{
						ex = ex4;
					}
					if (ex != null)
					{
						throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(adPropertyDefinition.Name, ex.Message), adPropertyDefinition, propertyBag[RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota]), ex);
					}
					return result;
				}
			}
			return (Unlimited<int>)adPropertyDefinition.DefaultValue;
		}

		internal static void ObjectCountQuotaSetter(string key, Unlimited<int> countQuota, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota];
			foreach (string text in multiValuedProperty)
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length == 2 && string.Equals(key, array[0], StringComparison.OrdinalIgnoreCase))
				{
					multiValuedProperty.Remove(text);
					break;
				}
			}
			if (countQuota != Unlimited<int>.UnlimitedValue)
			{
				string item = string.Format("{0}:{1}", key, countQuota);
				multiValuedProperty.Add(item);
			}
			propertyBag[RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota] = multiValuedProperty;
		}

		internal const string PolicyName = "Recipient Quota Policy";

		private static RecipientEnforcementProvisioningPolicySchema schema = ObjectSchema.GetInstance<RecipientEnforcementProvisioningPolicySchema>();

		private static IProvisioningEnforcement[] provisioningEnforcements = new IProvisioningEnforcement[]
		{
			new RecipientResourceCountQuota(RecipientEnforcementProvisioningPolicySchema.DistributionListCountQuota, "All Groups(VLV)", new Type[]
			{
				typeof(DistributionGroup),
				typeof(DynamicDistributionGroup),
				typeof(SyncDistributionGroup)
			}),
			new RecipientResourceCountQuota(RecipientEnforcementProvisioningPolicySchema.MailboxCountQuota, "All Mailboxes(VLV)", new Type[]
			{
				typeof(Mailbox),
				typeof(SyncMailbox)
			}, CannedSystemAddressLists.RecipientTypeDetailsForAllMailboxesAL),
			new RecipientResourceCountQuota(RecipientEnforcementProvisioningPolicySchema.MailUserCountQuota, "All Mail Users(VLV)", new Type[]
			{
				typeof(MailUser),
				typeof(SyncMailUser),
				typeof(RemoteMailbox)
			}),
			new RecipientResourceCountQuota(RecipientEnforcementProvisioningPolicySchema.ContactCountQuota, "All Contacts(VLV)", new Type[]
			{
				typeof(MailContact),
				typeof(SyncMailContact)
			}),
			new RecipientResourceCountQuota(RecipientEnforcementProvisioningPolicySchema.TeamMailboxCountQuota, "TeamMailboxes(VLV)", new Type[]
			{
				typeof(Mailbox),
				typeof(SyncMailbox)
			}, CannedSystemAddressLists.RecipientTypeDetailsForAllTeamMailboxesAL),
			new RecipientResourceCountQuota(RecipientEnforcementProvisioningPolicySchema.PublicFolderMailboxCountQuota, "PublicFolderMailboxes(VLV)", new Type[]
			{
				typeof(Mailbox),
				typeof(SyncMailbox)
			}, CannedSystemAddressLists.RecipientTypeDetailsForAllPublicFolderMailboxesAL),
			new RecipientResourceCountQuota(RecipientEnforcementProvisioningPolicySchema.MailPublicFolderCountQuota, "MailPublicFolders(VLV)", new Type[]
			{
				typeof(MailPublicFolder),
				typeof(ADPublicFolder)
			})
		};

		internal new static string MostDerivedClass = "msExchRecipientEnforcementPolicy";
	}
}
