using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ABContact : ABObject
	{
		public ABContact(ABSession ownerSession) : base(ownerSession, ABContact.allContactPropertiesCollection)
		{
		}

		public override ABObjectSchema Schema
		{
			get
			{
				return ABContact.schema;
			}
		}

		public Uri WebPage
		{
			get
			{
				return (Uri)this[ABContactSchema.WebPage];
			}
		}

		public string BusinessPhoneNumber
		{
			get
			{
				return (string)this[ABContactSchema.BusinessPhoneNumber];
			}
		}

		public string CompanyName
		{
			get
			{
				return (string)this[ABContactSchema.CompanyName];
			}
		}

		public string DepartmentName
		{
			get
			{
				return (string)this[ABContactSchema.DepartmentName];
			}
		}

		public string BusinessFaxNumber
		{
			get
			{
				return (string)this[ABContactSchema.BusinessFaxNumber];
			}
		}

		public string GivenName
		{
			get
			{
				return (string)this[ABContactSchema.GivenName];
			}
		}

		public string HomePhoneNumber
		{
			get
			{
				return (string)this[ABContactSchema.HomePhoneNumber];
			}
		}

		public string Initials
		{
			get
			{
				return (string)this[ABContactSchema.Initials];
			}
		}

		public ABObjectId Manager
		{
			get
			{
				return null;
			}
		}

		public string MobilePhoneNumber
		{
			get
			{
				return (string)this[ABContactSchema.MobilePhoneNumber];
			}
		}

		public string OfficeLocation
		{
			get
			{
				return (string)this[ABContactSchema.OfficeLocation];
			}
		}

		public string Surname
		{
			get
			{
				return (string)this[ABContactSchema.Surname];
			}
		}

		public string Title
		{
			get
			{
				return (string)this[ABContactSchema.Title];
			}
		}

		public string WorkAddressPostOfficeBox
		{
			get
			{
				return (string)this[ABContactSchema.WorkAddressPostOfficeBox];
			}
		}

		public string WorkAddressStreet
		{
			get
			{
				return (string)this[ABContactSchema.WorkAddressStreet];
			}
		}

		public string WorkAddressCity
		{
			get
			{
				return (string)this[ABContactSchema.WorkAddressCity];
			}
		}

		public string WorkAddressState
		{
			get
			{
				return (string)this[ABContactSchema.WorkAddressState];
			}
		}

		public string WorkAddressPostalCode
		{
			get
			{
				return (string)this[ABContactSchema.WorkAddressPostalCode];
			}
		}

		public string WorkAddressCountry
		{
			get
			{
				return (string)this[ABContactSchema.WorkAddressCountry];
			}
		}

		public byte[] Picture
		{
			get
			{
				return (byte[])this[ABContactSchema.Picture];
			}
		}

		protected virtual string GetHomePhoneNumber()
		{
			return null;
		}

		protected virtual string GetBusinessPhoneNumber()
		{
			return null;
		}

		protected virtual string GetCompanyName()
		{
			return null;
		}

		protected virtual string GetDepartmentName()
		{
			return null;
		}

		protected virtual string GetBusinessFaxNumber()
		{
			return null;
		}

		protected virtual string GetGivenName()
		{
			return null;
		}

		protected virtual string GetInitials()
		{
			return null;
		}

		protected virtual string GetMobilePhoneNumber()
		{
			return null;
		}

		protected virtual string GetOfficeLocation()
		{
			return null;
		}

		protected virtual string GetSurname()
		{
			return null;
		}

		protected virtual string GetTitle()
		{
			return null;
		}

		protected virtual string GetWorkAddressStreet()
		{
			return null;
		}

		protected virtual string GetWorkAddressCity()
		{
			return null;
		}

		protected virtual string GetWorkAddressCountry()
		{
			return null;
		}

		protected virtual string GetWorkAddressState()
		{
			return null;
		}

		protected virtual string GetWorkAddressPostalCode()
		{
			return null;
		}

		protected virtual string GetWorkAddressPostOfficeBox()
		{
			return null;
		}

		protected virtual Uri GetWebPage()
		{
			return null;
		}

		protected virtual byte[] GetPicture()
		{
			return null;
		}

		protected override bool InternalTryGetValue(ABPropertyDefinition property, out object value)
		{
			if (property == ABContactSchema.BusinessPhoneNumber)
			{
				value = this.GetBusinessPhoneNumber();
				return true;
			}
			if (property == ABContactSchema.CompanyName)
			{
				value = this.GetCompanyName();
				return true;
			}
			if (property == ABContactSchema.DepartmentName)
			{
				value = this.GetDepartmentName();
				return true;
			}
			if (property == ABContactSchema.BusinessFaxNumber)
			{
				value = this.GetBusinessFaxNumber();
				return true;
			}
			if (property == ABContactSchema.GivenName)
			{
				value = this.GetGivenName();
				return true;
			}
			if (property == ABContactSchema.HomePhoneNumber)
			{
				value = this.GetHomePhoneNumber();
				return true;
			}
			if (property == ABContactSchema.Initials)
			{
				value = this.GetInitials();
				return true;
			}
			if (property == ABContactSchema.MobilePhoneNumber)
			{
				value = this.GetMobilePhoneNumber();
				return true;
			}
			if (property == ABContactSchema.OfficeLocation)
			{
				value = this.GetOfficeLocation();
				return true;
			}
			if (property == ABContactSchema.Surname)
			{
				value = this.GetSurname();
				return true;
			}
			if (property == ABContactSchema.Title)
			{
				value = this.GetTitle();
				return true;
			}
			if (property == ABContactSchema.WorkAddressCity)
			{
				value = this.GetWorkAddressCity();
				return true;
			}
			if (property == ABContactSchema.WorkAddressCountry)
			{
				value = this.GetWorkAddressCountry();
				return true;
			}
			if (property == ABContactSchema.WorkAddressPostalCode)
			{
				value = this.GetWorkAddressPostalCode();
				return true;
			}
			if (property == ABContactSchema.WorkAddressPostOfficeBox)
			{
				value = this.GetWorkAddressPostOfficeBox();
				return true;
			}
			if (property == ABContactSchema.WorkAddressState)
			{
				value = this.GetWorkAddressState();
				return true;
			}
			if (property == ABContactSchema.WorkAddressStreet)
			{
				value = this.GetWorkAddressStreet();
				return true;
			}
			if (property == ABContactSchema.WebPage)
			{
				value = this.GetWebPage();
				return true;
			}
			if (property == ABContactSchema.Picture)
			{
				value = this.GetPicture();
				return true;
			}
			return base.InternalTryGetValue(property, out value);
		}

		private static ABContactSchema schema = new ABContactSchema();

		private static ABPropertyDefinitionCollection allContactPropertiesCollection = ABPropertyDefinitionCollection.FromPropertyDefinitionCollection(ABContact.schema.AllProperties);
	}
}
