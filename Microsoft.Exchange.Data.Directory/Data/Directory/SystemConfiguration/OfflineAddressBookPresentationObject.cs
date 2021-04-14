using System;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class OfflineAddressBookPresentationObject : ADPresentationObject
	{
		internal OfflineAddressBookPresentationObject(OfflineAddressBook backingOab) : base(backingOab)
		{
		}

		public OfflineAddressBookPresentationObject()
		{
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return OfflineAddressBookPresentationObject.SchemaInstance;
			}
		}

		public ADObjectId Server
		{
			get
			{
				if (this.IsE15OrLater())
				{
					return null;
				}
				return (ADObjectId)this[OfflineAddressBookPresentationSchema.Server];
			}
		}

		public ADObjectId GeneratingMailbox
		{
			get
			{
				return (ADObjectId)this[OfflineAddressBookPresentationSchema.GeneratingMailbox];
			}
		}

		public MultiValuedProperty<ADObjectId> AddressLists
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OfflineAddressBookPresentationSchema.AddressLists];
			}
		}

		public MultiValuedProperty<OfflineAddressBookVersion> Versions
		{
			get
			{
				return (MultiValuedProperty<OfflineAddressBookVersion>)this[OfflineAddressBookPresentationSchema.Versions];
			}
		}

		public bool IsDefault
		{
			get
			{
				return (bool)this[OfflineAddressBookPresentationSchema.IsDefault];
			}
		}

		public ADObjectId PublicFolderDatabase
		{
			get
			{
				return (ADObjectId)this[OfflineAddressBookPresentationSchema.PublicFolderDatabase];
			}
		}

		public bool PublicFolderDistributionEnabled
		{
			get
			{
				return (bool)this[OfflineAddressBookPresentationSchema.PublicFolderDistributionEnabled];
			}
		}

		public bool GlobalWebDistributionEnabled
		{
			get
			{
				return (bool)this[OfflineAddressBookPresentationSchema.GlobalWebDistributionEnabled];
			}
		}

		public bool WebDistributionEnabled
		{
			get
			{
				return (bool)this[OfflineAddressBookPresentationSchema.WebDistributionEnabled];
			}
		}

		public bool ShadowMailboxDistributionEnabled
		{
			get
			{
				return (bool)this[OfflineAddressBookPresentationSchema.ShadowMailboxDistributionEnabled];
			}
		}

		public DateTime? LastTouchedTime
		{
			get
			{
				return (DateTime?)this[OfflineAddressBookPresentationSchema.LastTouchedTime];
			}
		}

		public DateTime? LastRequestedTime
		{
			get
			{
				return (DateTime?)this[OfflineAddressBookPresentationSchema.LastRequestedTime];
			}
		}

		public DateTime? LastFailedTime
		{
			get
			{
				return (DateTime?)this[OfflineAddressBookPresentationSchema.LastFailedTime];
			}
		}

		public int? LastNumberOfRecords
		{
			get
			{
				return (int?)this[OfflineAddressBookPresentationSchema.LastNumberOfRecords];
			}
		}

		public OfflineAddressBookLastGeneratingData LastGeneratingData
		{
			get
			{
				return (OfflineAddressBookLastGeneratingData)this[OfflineAddressBookPresentationSchema.LastGeneratingData];
			}
		}

		public int MaxBinaryPropertySize
		{
			get
			{
				return (int)this[OfflineAddressBookPresentationSchema.MaxBinaryPropertySize];
			}
		}

		public int MaxMultivaluedBinaryPropertySize
		{
			get
			{
				return (int)this[OfflineAddressBookPresentationSchema.MaxMultivaluedBinaryPropertySize];
			}
		}

		public int MaxStringPropertySize
		{
			get
			{
				return (int)this[OfflineAddressBookPresentationSchema.MaxStringPropertySize];
			}
		}

		public int MaxMultivaluedStringPropertySize
		{
			get
			{
				return (int)this[OfflineAddressBookPresentationSchema.MaxMultivaluedStringPropertySize];
			}
		}

		public MultiValuedProperty<OfflineAddressBookMapiProperty> ConfiguredAttributes
		{
			get
			{
				return (MultiValuedProperty<OfflineAddressBookMapiProperty>)this[OfflineAddressBookPresentationSchema.ConfiguredAttributes];
			}
		}

		public Unlimited<int>? DiffRetentionPeriod
		{
			get
			{
				return (Unlimited<int>?)this[OfflineAddressBookPresentationSchema.DiffRetentionPeriod];
			}
		}

		public Schedule Schedule
		{
			get
			{
				return (Schedule)this[OfflineAddressBookPresentationSchema.Schedule];
			}
		}

		public MultiValuedProperty<ADObjectId> VirtualDirectories
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OfflineAddressBookPresentationSchema.VirtualDirectories];
			}
		}

		public string AdminDisplayName
		{
			get
			{
				return (string)this[OfflineAddressBookPresentationSchema.AdminDisplayName];
			}
		}

		private bool IsE15OrLater()
		{
			return ((OfflineAddressBook)base.DataObject).IsE15OrLater();
		}

		private static readonly OfflineAddressBookPresentationSchema SchemaInstance = ObjectSchema.GetInstance<OfflineAddressBookPresentationSchema>();
	}
}
