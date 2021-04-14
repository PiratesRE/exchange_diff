using System;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.Psws;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class FFOMailUser
	{
		public static string GenerateUserTypeText(object userType)
		{
			if (userType == DBNull.Value)
			{
				return string.Empty;
			}
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)userType;
			if (recipientTypeDetails != RecipientTypeDetails.UserMailbox)
			{
				if (recipientTypeDetails == RecipientTypeDetails.MailContact)
				{
					return Strings.MailContact;
				}
				if (recipientTypeDetails != RecipientTypeDetails.MailUser)
				{
					return userType.ToString();
				}
			}
			return Strings.MailUser;
		}

		public static void GetFFOMailUserList(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			try
			{
				table.BeginLoadData();
				GetRecipientCmdlet getRecipientCmdlet = new GetRecipientCmdlet
				{
					Filter = inputRow["SearchText"].ToString().ToRecipeintFilterString("((RecipientTypeDetails -eq 'UserMailbox') -or (RecipientTypeDetails -eq 'MailUser'))"),
					Properties = "Identity,DisplayName,PrimarySmtpAddress,RecipientTypeDetails"
				};
				getRecipientCmdlet.Authenticator = PswsAuthenticator.Create();
				getRecipientCmdlet.HostServerName = AppConfigLoader.GetConfigStringValue("PswsHostName", null);
				foreach (Recipient recipient in getRecipientCmdlet.Run())
				{
					DataRow dataRow = table.NewRow();
					dataRow["Identity"] = new Identity(recipient.Guid.ToString());
					dataRow["DisplayName"] = recipient.DisplayName;
					dataRow["PrimarySmtpAddress"] = recipient.PrimarySmtpAddress;
					dataRow["RecipientTypeDetails"] = FFOMailUser.GenerateUserTypeText(Enum.Parse(typeof(RecipientTypeDetails), recipient.RecipientTypeDetails, true));
					table.Rows.Add(dataRow);
				}
			}
			finally
			{
				table.EndLoadData();
			}
		}

		public static void NewFFOMailUser(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			NewMailUserCmdlet newMailUserCmdlet = new NewMailUserCmdlet();
			newMailUserCmdlet.Authenticator = PswsAuthenticator.Create();
			newMailUserCmdlet.HostServerName = AppConfigLoader.GetConfigStringValue("PswsHostName", null);
			if (!string.IsNullOrEmpty(inputRow["FirstName"].ToString()))
			{
				newMailUserCmdlet.FirstName = inputRow["FirstName"].ToString();
			}
			if (!string.IsNullOrEmpty(inputRow["Initials"].ToString()))
			{
				newMailUserCmdlet.Initials = inputRow["Initials"].ToString();
			}
			if (!string.IsNullOrEmpty(inputRow["LastName"].ToString()))
			{
				newMailUserCmdlet.LastName = inputRow["LastName"].ToString();
			}
			if (!string.IsNullOrEmpty(inputRow["ExternalEmailAddress"].ToString()))
			{
				newMailUserCmdlet.ExternalEmailAddress = inputRow["ExternalEmailAddress"].ToString();
			}
			newMailUserCmdlet.Alias = inputRow["Alias"].ToString();
			newMailUserCmdlet.DisplayName = inputRow["MailUserDisplayName"].ToString();
			newMailUserCmdlet.Name = inputRow["MailUserDisplayName"].ToString();
			newMailUserCmdlet.Password = inputRow["PlainPassword"].ToString();
			newMailUserCmdlet.PrimarySmtpAddress = inputRow["PrimarySmtpAddress"].ToString();
			newMailUserCmdlet.MicrosoftOnlineServicesID = inputRow["MicrosoftOnlineServicesID"].ToString();
			newMailUserCmdlet.Run();
			if (!string.IsNullOrEmpty(newMailUserCmdlet.Error))
			{
				throw new Exception(newMailUserCmdlet.Error);
			}
		}

		public static void GetFFOMailUser(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			GetRecipientCmdlet getRecipientCmdlet = new GetRecipientCmdlet
			{
				Identity = ((Identity)inputRow["Identity"]).RawIdentity
			};
			getRecipientCmdlet.Authenticator = PswsAuthenticator.Create();
			getRecipientCmdlet.HostServerName = AppConfigLoader.GetConfigStringValue("PswsHostName", null);
			Recipient[] array = getRecipientCmdlet.Run();
			if (array == null || array.Length == 0)
			{
				throw new ExArgumentOutOfRangeException();
			}
			Recipient recipient = array.First<Recipient>();
			DataRow dataRow = table.Rows[0];
			dataRow["Identity"] = new Identity(recipient.Guid.ToString());
			dataRow["FirstName"] = recipient.FirstName;
			dataRow["LastName"] = recipient.LastName;
			dataRow["DisplayName"] = recipient.DisplayName;
			dataRow["WindowsLiveID"] = recipient.WindowsLiveID;
			dataRow["City"] = recipient.City;
			dataRow["StateOrProvince"] = recipient.StateOrProvince;
			dataRow["PostalCode"] = recipient.PostalCode;
			dataRow["CountryOrRegion"] = recipient.CountryOrRegion.Name;
			dataRow["Phone"] = recipient.Phone;
			dataRow["Office"] = recipient.Office;
			dataRow["Notes"] = recipient.Notes;
			dataRow["Title"] = recipient.Title;
			dataRow["Department"] = recipient.Department;
			dataRow["Company"] = recipient.Company;
			dataRow["Alias"] = recipient.Alias;
			dataRow["PrimarySmtpAddress"] = recipient.PrimarySmtpAddress;
			dataRow["Name"] = recipient.Name;
			GetUserCmdlet getUserCmdlet = new GetUserCmdlet
			{
				Identity = ((Identity)inputRow["Identity"]).RawIdentity
			};
			getUserCmdlet.Authenticator = PswsAuthenticator.Create();
			getUserCmdlet.HostServerName = AppConfigLoader.GetConfigStringValue("PswsHostName", null);
			User[] array2 = getUserCmdlet.Run();
			if (array2 == null || array2.Length == 0)
			{
				throw new ExArgumentOutOfRangeException();
			}
			User user = array2.First<User>();
			dataRow["Initials"] = user.Initials;
			dataRow["StreetAddress"] = user.StreetAddress;
			dataRow["MobilePhone"] = user.MobilePhone;
			dataRow["Fax"] = user.Fax;
			dataRow["HomePhone"] = user.HomePhone;
			dataRow["WebPage"] = user.WebPage;
		}

		public static void GetFFOMailUserSDO(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			GetRecipientCmdlet getRecipientCmdlet = new GetRecipientCmdlet
			{
				Identity = ((Identity)inputRow["Identity"]).RawIdentity
			};
			getRecipientCmdlet.Authenticator = PswsAuthenticator.Create();
			getRecipientCmdlet.HostServerName = AppConfigLoader.GetConfigStringValue("PswsHostName", null);
			Recipient[] array = getRecipientCmdlet.Run();
			if (array == null || array.Length == 0)
			{
				throw new ExArgumentOutOfRangeException();
			}
			Recipient recipient = array.First<Recipient>();
			DataRow dataRow = table.Rows[0];
			dataRow["Identity"] = new Identity(recipient.Guid.ToString());
			dataRow["DisplayName"] = recipient.DisplayName;
			dataRow["Alias"] = recipient.Alias;
			dataRow["PrimarySmtpAddress"] = recipient.PrimarySmtpAddress;
			dataRow["Name"] = recipient.Name;
			dataRow["FirstName"] = recipient.FirstName;
			dataRow["LastName"] = recipient.LastName;
			dataRow["Title"] = recipient.Title;
			dataRow["Department"] = recipient.Department;
			dataRow["Office"] = recipient.Office;
			dataRow["Phone"] = recipient.Phone;
		}

		public static void RemoveFFOMailUser(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			RemoveMailUserCmdlet removeMailUserCmdlet = new RemoveMailUserCmdlet();
			removeMailUserCmdlet.Authenticator = PswsAuthenticator.Create();
			removeMailUserCmdlet.HostServerName = AppConfigLoader.GetConfigStringValue("PswsHostName", null);
			removeMailUserCmdlet.Identity = ((Identity)inputRow["Identity"]).RawIdentity;
			removeMailUserCmdlet.Run();
			if (!string.IsNullOrEmpty(removeMailUserCmdlet.Error))
			{
				throw new Exception(removeMailUserCmdlet.Error);
			}
		}

		public static void SetFFOMailUser(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			SetUserCmdlet setUserCmdlet = new SetUserCmdlet();
			setUserCmdlet.Authenticator = PswsAuthenticator.Create();
			setUserCmdlet.HostServerName = AppConfigLoader.GetConfigStringValue("PswsHostName", null);
			setUserCmdlet.Identity = ((Identity)inputRow["Identity"]).RawIdentity;
			if (!DBNull.Value.Equals(inputRow["DisplayName"]))
			{
				setUserCmdlet.DisplayName = inputRow["DisplayName"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["FirstName"]))
			{
				setUserCmdlet.FirstName = inputRow["FirstName"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["LastName"]))
			{
				setUserCmdlet.LastName = inputRow["LastName"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["Initials"]))
			{
				setUserCmdlet.Initials = inputRow["Initials"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["City"]))
			{
				setUserCmdlet.City = inputRow["City"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["Office"]))
			{
				setUserCmdlet.Office = inputRow["Office"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["StateOrProvince"]))
			{
				setUserCmdlet.StateOrProvince = inputRow["StateOrProvince"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["PostalCode"]))
			{
				setUserCmdlet.PostalCode = inputRow["PostalCode"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["CountryOrRegion"]))
			{
				setUserCmdlet.CountryOrRegion = inputRow["CountryOrRegion"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["Phone"]))
			{
				setUserCmdlet.Phone = inputRow["Phone"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["Notes"]))
			{
				setUserCmdlet.Notes = inputRow["Notes"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["Title"]))
			{
				setUserCmdlet.Title = inputRow["Title"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["Department"]))
			{
				setUserCmdlet.Department = inputRow["Department"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["Company"]))
			{
				setUserCmdlet.Company = inputRow["Company"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["StreetAddress"]))
			{
				setUserCmdlet.StreetAddress = inputRow["StreetAddress"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["MobilePhone"]))
			{
				setUserCmdlet.MobilePhone = inputRow["MobilePhone"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["Fax"]))
			{
				setUserCmdlet.Fax = inputRow["Fax"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["HomePhone"]))
			{
				setUserCmdlet.HomePhone = inputRow["HomePhone"].ToString();
			}
			if (!DBNull.Value.Equals(inputRow["WebPage"]))
			{
				setUserCmdlet.WebPage = inputRow["WebPage"].ToString();
			}
			setUserCmdlet.Run();
			if (!string.IsNullOrEmpty(setUserCmdlet.Error))
			{
				throw new Exception(setUserCmdlet.Error);
			}
		}

		private const string PswsHostName = "PswsHostName";

		private const string propertiesList = "Identity,DisplayName,PrimarySmtpAddress,RecipientTypeDetails";
	}
}
