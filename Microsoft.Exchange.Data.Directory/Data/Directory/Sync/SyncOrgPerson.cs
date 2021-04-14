using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal abstract class SyncOrgPerson : SyncRecipient
	{
		public SyncOrgPerson(SyncDirection syncDirection) : base(syncDirection)
		{
		}

		public SyncProperty<string> AssistantName
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.AssistantName];
			}
			set
			{
				base[SyncOrgPersonSchema.AssistantName] = value;
			}
		}

		public SyncProperty<string> C
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.C];
			}
			set
			{
				base[SyncOrgPersonSchema.C] = value;
			}
		}

		public SyncProperty<string> City
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.City];
			}
			set
			{
				base[SyncOrgPersonSchema.City] = value;
			}
		}

		public SyncProperty<string> Co
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.Co];
			}
			set
			{
				base[SyncOrgPersonSchema.Co] = value;
			}
		}

		public SyncProperty<string> Company
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.Company];
			}
			set
			{
				base[SyncOrgPersonSchema.Company] = value;
			}
		}

		public SyncProperty<int> CountryCode
		{
			get
			{
				return (SyncProperty<int>)base[SyncOrgPersonSchema.CountryCode];
			}
			set
			{
				base[SyncOrgPersonSchema.CountryCode] = value;
			}
		}

		public SyncProperty<string> Department
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.Department];
			}
			set
			{
				base[SyncOrgPersonSchema.Department] = value;
			}
		}

		public SyncProperty<string> Fax
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.Fax];
			}
			set
			{
				base[SyncOrgPersonSchema.Fax] = value;
			}
		}

		public SyncProperty<string> FirstName
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.FirstName];
			}
			set
			{
				base[SyncOrgPersonSchema.FirstName] = value;
			}
		}

		public SyncProperty<string> HomePhone
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.HomePhone];
			}
			set
			{
				base[SyncOrgPersonSchema.HomePhone] = value;
			}
		}

		public SyncProperty<string> Initials
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.Initials];
			}
			set
			{
				base[SyncOrgPersonSchema.Initials] = value;
			}
		}

		public SyncProperty<string> LastName
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.LastName];
			}
			set
			{
				base[SyncOrgPersonSchema.LastName] = value;
			}
		}

		public SyncProperty<string> MobilePhone
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.MobilePhone];
			}
			set
			{
				base[SyncOrgPersonSchema.MobilePhone] = value;
			}
		}

		public SyncProperty<string> Notes
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.Notes];
			}
			set
			{
				base[SyncOrgPersonSchema.Notes] = value;
			}
		}

		public SyncProperty<string> Office
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.Office];
			}
			set
			{
				base[SyncOrgPersonSchema.Office] = value;
			}
		}

		public SyncProperty<string> OtherHomePhone
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.OtherHomePhone];
			}
			set
			{
				base[SyncOrgPersonSchema.OtherHomePhone] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> OtherTelephone
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncOrgPersonSchema.OtherTelephone];
			}
			set
			{
				base[SyncOrgPersonSchema.OtherTelephone] = value;
			}
		}

		public SyncProperty<string> Pager
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.Pager];
			}
			set
			{
				base[SyncOrgPersonSchema.Pager] = value;
			}
		}

		public SyncProperty<string> Phone
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.Phone];
			}
			set
			{
				base[SyncOrgPersonSchema.Phone] = value;
			}
		}

		public SyncProperty<string> PostalCode
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.PostalCode];
			}
			set
			{
				base[SyncOrgPersonSchema.PostalCode] = value;
			}
		}

		public SyncProperty<string> StateOrProvince
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.StateOrProvince];
			}
			set
			{
				base[SyncOrgPersonSchema.StateOrProvince] = value;
			}
		}

		public SyncProperty<string> StreetAddress
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.StreetAddress];
			}
			set
			{
				base[SyncOrgPersonSchema.StreetAddress] = value;
			}
		}

		public SyncProperty<string> TelephoneAssistant
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.TelephoneAssistant];
			}
			set
			{
				base[SyncOrgPersonSchema.TelephoneAssistant] = value;
			}
		}

		public SyncProperty<string> Title
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.Title];
			}
			set
			{
				base[SyncOrgPersonSchema.Title] = value;
			}
		}

		public SyncProperty<string> WebPage
		{
			get
			{
				return (SyncProperty<string>)base[SyncOrgPersonSchema.WebPage];
			}
			set
			{
				base[SyncOrgPersonSchema.WebPage] = value;
			}
		}

		protected override SyncPropertyDefinition[] MinimumForwardSyncProperties
		{
			get
			{
				List<SyncPropertyDefinition> list = base.MinimumForwardSyncProperties.ToList<SyncPropertyDefinition>();
				list.AddRange(new SyncPropertyDefinition[]
				{
					SyncRecipientSchema.ExternalEmailAddress
				});
				return list.ToArray();
			}
		}
	}
}
