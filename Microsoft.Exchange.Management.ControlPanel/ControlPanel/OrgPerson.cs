using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(OrgPerson))]
	public class OrgPerson : MailboxRecipientRow
	{
		protected OrgPerson(MailEnabledOrgPerson recipient) : base(recipient)
		{
			this.MailEnabledOrgPerson = recipient;
		}

		private protected MailEnabledOrgPerson MailEnabledOrgPerson { protected get; private set; }

		public OrgPersonPresentationObject OrgPersonObject { get; set; }

		[DataMember]
		public string ProfileCaption
		{
			get
			{
				return Strings.ProfileCaption(this.MailEnabledOrgPerson.DisplayName);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ChangePhotoCaption
		{
			get
			{
				return Strings.ChangePhotoCaption(this.MailEnabledOrgPerson.DisplayName);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string FirstName
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.FirstName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Initials
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.Initials;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string LastName
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.LastName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string EmailAddress { get; protected set; }

		[DataMember]
		public string StreetAddress
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.StreetAddress;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string City
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.City;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string StateOrProvince
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.StateOrProvince;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PostalCode
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.PostalCode;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CountryOrRegion
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				if (!(null != this.OrgPersonObject.CountryOrRegion))
				{
					return string.Empty;
				}
				return this.OrgPersonObject.CountryOrRegion.Name;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CountryOrRegionDisplayName
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return null;
				}
				if (!(null != this.OrgPersonObject.CountryOrRegion))
				{
					return string.Empty;
				}
				return this.OrgPersonObject.CountryOrRegion.DisplayName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Office
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.Office;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Phone
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.Phone;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Fax
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.Fax;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string HomePhone
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.HomePhone;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MobilePhone
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.MobilePhone;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Notes
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.Notes;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Title
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.Title;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Department
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.Department;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Company
		{
			get
			{
				if (this.OrgPersonObject == null)
				{
					return string.Empty;
				}
				return this.OrgPersonObject.Company;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public RecipientObjectResolverRow Manager
		{
			get
			{
				if (this.OrgPersonObject != null && this.OrgPersonObject.Manager != null)
				{
					return RecipientObjectResolver.Instance.ResolveObjects(new ADObjectId[]
					{
						this.OrgPersonObject.Manager
					}).FirstOrDefault<RecipientObjectResolverRow>();
				}
				return null;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> DirectReports
		{
			get
			{
				if (this.OrgPersonObject != null && this.OrgPersonObject.DirectReports != null)
				{
					return RecipientObjectResolver.Instance.ResolveObjects(this.OrgPersonObject.DirectReports);
				}
				return null;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<string> EmailAddresses
		{
			get
			{
				if (this.MailEnabledOrgPerson == null)
				{
					return null;
				}
				return from address in this.MailEnabledOrgPerson.EmailAddresses
				where address is SmtpProxyAddress
				where !address.IsPrimaryAddress
				orderby ((SmtpProxyAddress)address).SmtpAddress
				select ((SmtpProxyAddress)address).SmtpAddress;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PrimaryEmailAddress
		{
			get
			{
				if (this.MailEnabledOrgPerson == null)
				{
					return string.Empty;
				}
				return this.MailEnabledOrgPerson.PrimarySmtpAddress.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MailTip
		{
			get
			{
				if (this.MailEnabledOrgPerson.MailTip != null)
				{
					return this.MailEnabledOrgPerson.MailTip;
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
