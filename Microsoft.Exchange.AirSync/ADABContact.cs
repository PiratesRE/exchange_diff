using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.ABProviderFramework;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class ADABContact : ABContact
	{
		public ADABContact(ABSession ownerSession, ADRecipient recipient) : base(ownerSession)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (recipient.Id == null)
			{
				throw new ArgumentException("recipient.Id can't be null.", "recipient.Id");
			}
			switch (recipient.RecipientType)
			{
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.DynamicDistributionGroup:
				throw new ArgumentException("RecipientType " + recipient.RecipientType.ToString() + " shouldn't be wrapped in an ADABContact.", "recipient");
			default:
				this.recipient = recipient;
				return;
			}
		}

		protected override ABObjectId GetId()
		{
			ADABObjectId adabobjectId;
			if (this.id == null && ADABUtils.GetId(this.recipient, out adabobjectId))
			{
				this.id = adabobjectId;
			}
			return this.id;
		}

		protected override string GetLegacyExchangeDN()
		{
			return this.recipient.LegacyExchangeDN;
		}

		protected override bool GetCanEmail()
		{
			return ADABUtils.CanEmailRecipientType(this.recipient.RecipientType);
		}

		protected override string GetAlias()
		{
			return this.recipient.Alias;
		}

		protected override string GetEmailAddress()
		{
			string result;
			if (ADABUtils.GetEmailAddress(this.recipient, out result))
			{
				return result;
			}
			return null;
		}

		protected override string GetDisplayName()
		{
			return this.recipient.DisplayName;
		}

		protected override string GetBusinessPhoneNumber()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.Phone;
		}

		protected override string GetCompanyName()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.Company;
		}

		protected override string GetDepartmentName()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.Department;
		}

		protected override string GetBusinessFaxNumber()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.Fax;
		}

		protected override string GetGivenName()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.FirstName;
		}

		protected override string GetHomePhoneNumber()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.HomePhone;
		}

		protected override string GetInitials()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.Initials;
		}

		protected override string GetMobilePhoneNumber()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.MobilePhone;
		}

		protected override string GetOfficeLocation()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.Office;
		}

		protected override string GetSurname()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.LastName;
		}

		protected override string GetTitle()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.Title;
		}

		protected override string GetWorkAddressPostOfficeBox()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return ADABContact.GetFirstValue(iadorgPerson.PostOfficeBox);
		}

		protected override string GetWorkAddressStreet()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.StreetAddress;
		}

		protected override string GetWorkAddressCity()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.City;
		}

		protected override string GetWorkAddressState()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.StateOrProvince;
		}

		protected override string GetWorkAddressPostalCode()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.PostalCode;
		}

		protected override string GetWorkAddressCountry()
		{
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return null;
			}
			return iadorgPerson.CountryOrRegionDisplayName;
		}

		protected override Uri GetWebPage()
		{
			Uri result;
			if (!ADABUtils.GetWebPage(this.recipient, out result))
			{
				return null;
			}
			return result;
		}

		protected override byte[] GetPicture()
		{
			return this.recipient.ThumbnailPhoto;
		}

		private static string GetFirstValue(MultiValuedProperty<string> mvp)
		{
			if (mvp == null || mvp.Count == 0)
			{
				return null;
			}
			return mvp[0];
		}

		private ADRecipient recipient;

		private ADABObjectId id;
	}
}
