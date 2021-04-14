using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "UserSearchDefinition", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class UserSearchDefinition : SearchDefinition
	{
		[DataMember]
		public AccountSkuIdentifier AccountSku
		{
			get
			{
				return this.AccountSkuField;
			}
			set
			{
				this.AccountSkuField = value;
			}
		}

		[DataMember]
		public bool? BlackberryUsersOnly
		{
			get
			{
				return this.BlackberryUsersOnlyField;
			}
			set
			{
				this.BlackberryUsersOnlyField = value;
			}
		}

		[DataMember]
		public string City
		{
			get
			{
				return this.CityField;
			}
			set
			{
				this.CityField = value;
			}
		}

		[DataMember]
		public string Country
		{
			get
			{
				return this.CountryField;
			}
			set
			{
				this.CountryField = value;
			}
		}

		[DataMember]
		public string Department
		{
			get
			{
				return this.DepartmentField;
			}
			set
			{
				this.DepartmentField = value;
			}
		}

		[DataMember]
		public string DomainName
		{
			get
			{
				return this.DomainNameField;
			}
			set
			{
				this.DomainNameField = value;
			}
		}

		[DataMember]
		public UserEnabledFilter? EnabledFilter
		{
			get
			{
				return this.EnabledFilterField;
			}
			set
			{
				this.EnabledFilterField = value;
			}
		}

		[DataMember]
		public bool? HasErrorsOnly
		{
			get
			{
				return this.HasErrorsOnlyField;
			}
			set
			{
				this.HasErrorsOnlyField = value;
			}
		}

		[DataMember]
		public string[] IncludedProperties
		{
			get
			{
				return this.IncludedPropertiesField;
			}
			set
			{
				this.IncludedPropertiesField = value;
			}
		}

		[DataMember]
		public bool? LicenseReconciliationNeededOnly
		{
			get
			{
				return this.LicenseReconciliationNeededOnlyField;
			}
			set
			{
				this.LicenseReconciliationNeededOnlyField = value;
			}
		}

		[DataMember]
		public bool? ReturnDeletedUsers
		{
			get
			{
				return this.ReturnDeletedUsersField;
			}
			set
			{
				this.ReturnDeletedUsersField = value;
			}
		}

		[DataMember]
		public string State
		{
			get
			{
				return this.StateField;
			}
			set
			{
				this.StateField = value;
			}
		}

		[DataMember]
		public bool? Synchronized
		{
			get
			{
				return this.SynchronizedField;
			}
			set
			{
				this.SynchronizedField = value;
			}
		}

		[DataMember]
		public string Title
		{
			get
			{
				return this.TitleField;
			}
			set
			{
				this.TitleField = value;
			}
		}

		[DataMember]
		public bool? UnlicensedUsersOnly
		{
			get
			{
				return this.UnlicensedUsersOnlyField;
			}
			set
			{
				this.UnlicensedUsersOnlyField = value;
			}
		}

		[DataMember]
		public string UsageLocation
		{
			get
			{
				return this.UsageLocationField;
			}
			set
			{
				this.UsageLocationField = value;
			}
		}

		private AccountSkuIdentifier AccountSkuField;

		private bool? BlackberryUsersOnlyField;

		private string CityField;

		private string CountryField;

		private string DepartmentField;

		private string DomainNameField;

		private UserEnabledFilter? EnabledFilterField;

		private bool? HasErrorsOnlyField;

		private string[] IncludedPropertiesField;

		private bool? LicenseReconciliationNeededOnlyField;

		private bool? ReturnDeletedUsersField;

		private string StateField;

		private bool? SynchronizedField;

		private string TitleField;

		private bool? UnlicensedUsersOnlyField;

		private string UsageLocationField;
	}
}
