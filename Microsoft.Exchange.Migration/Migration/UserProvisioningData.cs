using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Migration
{
	internal class UserProvisioningData : RecipientProvisioningData
	{
		internal UserProvisioningData()
		{
			base.Action = ProvisioningAction.CreateNew;
			base.ProvisioningType = ProvisioningType.User;
		}

		public string FirstName
		{
			get
			{
				return (string)base[ADOrgPersonSchema.FirstName];
			}
			set
			{
				base[ADOrgPersonSchema.FirstName] = value;
			}
		}

		public string Initials
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Initials];
			}
			set
			{
				base[ADOrgPersonSchema.Initials] = value;
			}
		}

		public string LastName
		{
			get
			{
				return (string)base[ADOrgPersonSchema.LastName];
			}
			set
			{
				base[ADOrgPersonSchema.LastName] = value;
			}
		}

		public string MailboxPlan
		{
			get
			{
				return (string)base[ADRecipientSchema.MailboxPlan];
			}
			set
			{
				base[ADRecipientSchema.MailboxPlan] = value;
			}
		}

		public bool ResetPasswordOnNextLogon
		{
			get
			{
				object obj = base[ADUserSchema.ResetPasswordOnNextLogon];
				return obj != null && (bool)obj;
			}
			set
			{
				base[ADUserSchema.ResetPasswordOnNextLogon] = value;
			}
		}

		public string WindowsLiveID
		{
			get
			{
				return (string)base[ADRecipientSchema.WindowsLiveID];
			}
			set
			{
				base[ADRecipientSchema.WindowsLiveID] = value;
			}
		}

		public string MicrosoftOnlineServicesID
		{
			get
			{
				return (string)base["MicrosoftOnlineServicesID"];
			}
			set
			{
				base["MicrosoftOnlineServicesID"] = value;
			}
		}

		public bool UseExistingLiveId
		{
			get
			{
				object obj = base["UseExistingLiveId"];
				return obj != null && (bool)obj;
			}
			set
			{
				base["UseExistingLiveId"] = value;
			}
		}

		public bool UseExistingMicrosoftOnlineServicesID
		{
			get
			{
				object obj = base["UseExistingMicrosoftOnlineServicesID"];
				return obj != null && (bool)obj;
			}
			set
			{
				base["UseExistingMicrosoftOnlineServicesID"] = value;
			}
		}

		public string Password
		{
			get
			{
				return (string)base["Password"];
			}
			set
			{
				base["Password"] = value;
			}
		}

		public string FederatedIdentity
		{
			get
			{
				return (string)base["FederatedIdentity"];
			}
			set
			{
				base["FederatedIdentity"] = value;
			}
		}

		public bool EvictLiveId
		{
			get
			{
				object obj = base["EvictLiveID"];
				return obj != null && (bool)obj;
			}
			set
			{
				base["EvictLiveID"] = value;
			}
		}

		public bool ImportLiveId
		{
			get
			{
				object obj = base["ImportLiveID"];
				return obj != null && (bool)obj;
			}
			set
			{
				base["ImportLiveID"] = value;
			}
		}

		public string City
		{
			get
			{
				return (string)base[ADOrgPersonSchema.City];
			}
			set
			{
				base[ADOrgPersonSchema.City] = value;
			}
		}

		public string Company
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Company];
			}
			set
			{
				base[ADOrgPersonSchema.Company] = value;
			}
		}

		public string CountryOrRegion
		{
			get
			{
				return (string)base[ADOrgPersonSchema.CountryOrRegion];
			}
			set
			{
				base[ADOrgPersonSchema.CountryOrRegion] = value;
			}
		}

		public string Department
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Department];
			}
			set
			{
				base[ADOrgPersonSchema.Department] = value;
			}
		}

		public string Fax
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Fax];
			}
			set
			{
				base[ADOrgPersonSchema.Fax] = value;
			}
		}

		public string HomePhone
		{
			get
			{
				return (string)base[ADOrgPersonSchema.HomePhone];
			}
			set
			{
				base[ADOrgPersonSchema.HomePhone] = value;
			}
		}

		public string[] Languages
		{
			get
			{
				return (string[])base[ADOrgPersonSchema.Languages];
			}
			set
			{
				base[ADOrgPersonSchema.Languages] = value;
			}
		}

		public string MobilePhone
		{
			get
			{
				return (string)base[ADOrgPersonSchema.MobilePhone];
			}
			set
			{
				base[ADOrgPersonSchema.MobilePhone] = value;
			}
		}

		public string Notes
		{
			get
			{
				return (string)base[ADRecipientSchema.Notes];
			}
			set
			{
				base[ADRecipientSchema.Notes] = value;
			}
		}

		public string Office
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Office];
			}
			set
			{
				base[ADOrgPersonSchema.Office] = value;
			}
		}

		public string Phone
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Phone];
			}
			set
			{
				base[ADOrgPersonSchema.Phone] = value;
			}
		}

		public string PostalCode
		{
			get
			{
				return (string)base[ADOrgPersonSchema.PostalCode];
			}
			set
			{
				base[ADOrgPersonSchema.PostalCode] = value;
			}
		}

		public string StateOrProvince
		{
			get
			{
				return (string)base[ADOrgPersonSchema.StateOrProvince];
			}
			set
			{
				base[ADOrgPersonSchema.StateOrProvince] = value;
			}
		}

		public string StreetAddress
		{
			get
			{
				return (string)base[ADOrgPersonSchema.StreetAddress];
			}
			set
			{
				base[ADOrgPersonSchema.StreetAddress] = value;
			}
		}

		public string Title
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Title];
			}
			set
			{
				base[ADOrgPersonSchema.Title] = value;
			}
		}

		public string WebPage
		{
			get
			{
				return (string)base[ADRecipientSchema.WebPage];
			}
			set
			{
				base[ADRecipientSchema.WebPage] = value;
			}
		}

		public string ResourceType
		{
			get
			{
				return (string)base[ADRecipientSchema.ResourceType];
			}
			set
			{
				base[ADRecipientSchema.ResourceType] = value;
			}
		}

		public byte[] UMSpokenName
		{
			get
			{
				return (byte[])base[ADRecipientSchema.UMSpokenName];
			}
			set
			{
				base[ADRecipientSchema.UMSpokenName] = value;
			}
		}

		public int ResourceCapacity
		{
			get
			{
				object obj = base[ADRecipientSchema.ResourceCapacity];
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
			set
			{
				base[ADRecipientSchema.ResourceCapacity] = value;
			}
		}

		public static UserProvisioningData Create(string name, string firstName, string lastName, string id, string password, bool isBPOS)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(name, "name");
			MigrationUtil.ThrowOnNullOrEmptyArgument(firstName, "firstName");
			MigrationUtil.ThrowOnNullOrEmptyArgument(lastName, "lastName");
			MigrationUtil.ThrowOnNullOrEmptyArgument(id, "id");
			MigrationUtil.ThrowOnNullOrEmptyArgument(password, "password");
			UserProvisioningData userProvisioningData = new UserProvisioningData();
			userProvisioningData.Name = name;
			userProvisioningData.FirstName = firstName;
			userProvisioningData.LastName = lastName;
			UserProvisioningData.SetBposAwareProvisioningParameters(isBPOS, id, password, false, userProvisioningData);
			return userProvisioningData;
		}

		public static UserProvisioningData Create(string name, string id, bool useExistingId, string password, bool isBPOS)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(name, "name");
			MigrationUtil.ThrowOnNullOrEmptyArgument(id, "id");
			if (useExistingId && !string.IsNullOrEmpty(password))
			{
				throw new ArgumentException("Password should not be provided if useExistingId is set to true");
			}
			if (!useExistingId && string.IsNullOrEmpty(password))
			{
				throw new ArgumentException("Password should be provided if useExistingId is set to false");
			}
			UserProvisioningData userProvisioningData = new UserProvisioningData();
			userProvisioningData.Name = name;
			UserProvisioningData.SetBposAwareProvisioningParameters(isBPOS, id, password, useExistingId, userProvisioningData);
			return userProvisioningData;
		}

		public static UserProvisioningData CreateResource(string name, string displayName, string resourceType, int resourceCapacity, string password)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(name, "name");
			MigrationUtil.ThrowOnNullOrEmptyArgument(displayName, "displayName");
			MigrationUtil.ThrowOnNullOrEmptyArgument(resourceType, "resourceType");
			MigrationUtil.ThrowOnLessThanZeroArgument((long)resourceCapacity, "resourceCapacity");
			MigrationUtil.ThrowOnNullOrEmptyArgument(password, "password");
			UserProvisioningData userProvisioningData = new UserProvisioningData();
			userProvisioningData.Name = name;
			userProvisioningData.DisplayName = displayName;
			userProvisioningData.ResourceType = resourceType;
			userProvisioningData.ResourceCapacity = resourceCapacity;
			userProvisioningData.Password = password;
			userProvisioningData[ADRecipientSchema.IsResource] = true;
			return userProvisioningData;
		}

		public static UserProvisioningData CreateWithFederatedIdentity(string name, string firstName, string lastName, string id, string federatedIdentity, bool isBPOS)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(name, "name");
			MigrationUtil.ThrowOnNullOrEmptyArgument(firstName, "firstName");
			MigrationUtil.ThrowOnNullOrEmptyArgument(lastName, "lastName");
			MigrationUtil.ThrowOnNullOrEmptyArgument(id, "id");
			MigrationUtil.ThrowOnNullOrEmptyArgument(federatedIdentity, "federatedIdentity");
			UserProvisioningData userProvisioningData = new UserProvisioningData();
			userProvisioningData.Name = name;
			userProvisioningData.FirstName = firstName;
			userProvisioningData.LastName = lastName;
			userProvisioningData.FederatedIdentity = federatedIdentity;
			UserProvisioningData.SetBposAwareProvisioningParameters(isBPOS, id, null, false, userProvisioningData);
			return userProvisioningData;
		}

		private static void SetBposAwareProvisioningParameters(bool isBpos, string id, string password, bool useExistingId, UserProvisioningData data)
		{
			if (isBpos)
			{
				data.IsBPOS = isBpos;
				data.MicrosoftOnlineServicesID = id;
				if (useExistingId)
				{
					data.UseExistingMicrosoftOnlineServicesID = useExistingId;
				}
			}
			else
			{
				data.WindowsLiveID = id;
				if (useExistingId)
				{
					data.UseExistingLiveId = useExistingId;
				}
			}
			if (!useExistingId && !string.IsNullOrEmpty(password))
			{
				data.Password = password;
			}
		}

		public const string UseExistingLiveIdParameterName = "UseExistingLiveId";

		public const string UseExistingMicrosoftOnlineServicesIDParameterName = "UseExistingMicrosoftOnlineServicesID";

		public const string FederatedIdentityParameterName = "FederatedIdentity";

		public const string EvictLiveIDParameterName = "EvictLiveID";

		public const string ImportLiveIDParameterName = "ImportLiveID";
	}
}
