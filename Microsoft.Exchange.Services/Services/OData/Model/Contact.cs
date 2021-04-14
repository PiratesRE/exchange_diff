using System;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class Contact : Item
	{
		public string ParentFolderId
		{
			get
			{
				return (string)base[ContactSchema.ParentFolderId];
			}
			set
			{
				base[ContactSchema.ParentFolderId] = value;
			}
		}

		public DateTimeOffset Birthday
		{
			get
			{
				return (DateTimeOffset)base[ContactSchema.Birthday];
			}
			set
			{
				base[ContactSchema.Birthday] = value;
			}
		}

		public string FileAs
		{
			get
			{
				return (string)base[ContactSchema.FileAs];
			}
			set
			{
				base[ContactSchema.FileAs] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)base[ContactSchema.DisplayName];
			}
			set
			{
				base[ContactSchema.DisplayName] = value;
			}
		}

		public string GivenName
		{
			get
			{
				return (string)base[ContactSchema.GivenName];
			}
			set
			{
				base[ContactSchema.GivenName] = value;
			}
		}

		public string Initials
		{
			get
			{
				return (string)base[ContactSchema.Initials];
			}
			set
			{
				base[ContactSchema.Initials] = value;
			}
		}

		public string MiddleName
		{
			get
			{
				return (string)base[ContactSchema.MiddleName];
			}
			set
			{
				base[ContactSchema.MiddleName] = value;
			}
		}

		public string NickName
		{
			get
			{
				return (string)base[ContactSchema.NickName];
			}
			set
			{
				base[ContactSchema.NickName] = value;
			}
		}

		public string Surname
		{
			get
			{
				return (string)base[ContactSchema.Surname];
			}
			set
			{
				base[ContactSchema.Surname] = value;
			}
		}

		public string Title
		{
			get
			{
				return (string)base[ContactSchema.Title];
			}
			set
			{
				base[ContactSchema.Title] = value;
			}
		}

		public string Generation
		{
			get
			{
				return (string)base[ContactSchema.Generation];
			}
			set
			{
				base[ContactSchema.Generation] = value;
			}
		}

		public string EmailAddress1
		{
			get
			{
				return (string)base[ContactSchema.EmailAddress1];
			}
			set
			{
				base[ContactSchema.EmailAddress1] = value;
			}
		}

		public string EmailAddress2
		{
			get
			{
				return (string)base[ContactSchema.EmailAddress2];
			}
			set
			{
				base[ContactSchema.EmailAddress2] = value;
			}
		}

		public string EmailAddress3
		{
			get
			{
				return (string)base[ContactSchema.EmailAddress3];
			}
			set
			{
				base[ContactSchema.EmailAddress3] = value;
			}
		}

		public string ImAddress1
		{
			get
			{
				return (string)base[ContactSchema.ImAddress1];
			}
			set
			{
				base[ContactSchema.ImAddress1] = value;
			}
		}

		public string ImAddress2
		{
			get
			{
				return (string)base[ContactSchema.ImAddress2];
			}
			set
			{
				base[ContactSchema.ImAddress2] = value;
			}
		}

		public string ImAddress3
		{
			get
			{
				return (string)base[ContactSchema.ImAddress3];
			}
			set
			{
				base[ContactSchema.ImAddress3] = value;
			}
		}

		public string JobTitle
		{
			get
			{
				return (string)base[ContactSchema.JobTitle];
			}
			set
			{
				base[ContactSchema.JobTitle] = value;
			}
		}

		public string CompanyName
		{
			get
			{
				return (string)base[ContactSchema.CompanyName];
			}
			set
			{
				base[ContactSchema.CompanyName] = value;
			}
		}

		public string Department
		{
			get
			{
				return (string)base[ContactSchema.Department];
			}
			set
			{
				base[ContactSchema.Department] = value;
			}
		}

		public string OfficeLocation
		{
			get
			{
				return (string)base[ContactSchema.OfficeLocation];
			}
			set
			{
				base[ContactSchema.OfficeLocation] = value;
			}
		}

		public string Profession
		{
			get
			{
				return (string)base[ContactSchema.Profession];
			}
			set
			{
				base[ContactSchema.Profession] = value;
			}
		}

		public string BusinessHomePage
		{
			get
			{
				return (string)base[ContactSchema.BusinessHomePage];
			}
			set
			{
				base[ContactSchema.BusinessHomePage] = value;
			}
		}

		public string AssistantName
		{
			get
			{
				return (string)base[ContactSchema.AssistantName];
			}
			set
			{
				base[ContactSchema.AssistantName] = value;
			}
		}

		public string Manager
		{
			get
			{
				return (string)base[ContactSchema.Manager];
			}
			set
			{
				base[ContactSchema.Manager] = value;
			}
		}

		public string HomePhone1
		{
			get
			{
				return (string)base[ContactSchema.HomePhone1];
			}
			set
			{
				base[ContactSchema.HomePhone1] = value;
			}
		}

		public string HomePhone2
		{
			get
			{
				return (string)base[ContactSchema.HomePhone2];
			}
			set
			{
				base[ContactSchema.HomePhone2] = value;
			}
		}

		public string BusinessPhone1
		{
			get
			{
				return (string)base[ContactSchema.BusinessPhone1];
			}
			set
			{
				base[ContactSchema.BusinessPhone1] = value;
			}
		}

		public string BusinessPhone2
		{
			get
			{
				return (string)base[ContactSchema.BusinessPhone2];
			}
			set
			{
				base[ContactSchema.BusinessPhone2] = value;
			}
		}

		public string MobilePhone1
		{
			get
			{
				return (string)base[ContactSchema.MobilePhone1];
			}
			set
			{
				base[ContactSchema.MobilePhone1] = value;
			}
		}

		public string OtherPhone
		{
			get
			{
				return (string)base[ContactSchema.OtherPhone];
			}
			set
			{
				base[ContactSchema.OtherPhone] = value;
			}
		}

		public PhysicalAddress HomeAddress
		{
			get
			{
				return (PhysicalAddress)base[ContactSchema.HomeAddress];
			}
			set
			{
				base[ContactSchema.HomeAddress] = value;
			}
		}

		public PhysicalAddress BusinessAddress
		{
			get
			{
				return (PhysicalAddress)base[ContactSchema.BusinessAddress];
			}
			set
			{
				base[ContactSchema.BusinessAddress] = value;
			}
		}

		public PhysicalAddress OtherAddress
		{
			get
			{
				return (PhysicalAddress)base[ContactSchema.OtherAddress];
			}
			set
			{
				base[ContactSchema.OtherAddress] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return ContactSchema.SchemaInstance;
			}
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(Contact).Namespace, typeof(Contact).Name, Microsoft.Exchange.Services.OData.Model.Item.EdmEntityType);
	}
}
