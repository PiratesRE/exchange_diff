using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Security.Authentication.Mailbox
{
	internal class MailboxUserProfile
	{
		public static DateTime GetLastLogonTime(string puid)
		{
			if (MailboxUserProfile.logonTimeCache.ContainsKey(puid))
			{
				return MailboxUserProfile.logonTimeCache[puid];
			}
			return DateTime.MinValue;
		}

		public static void SetLastLogonTime(string puid, DateTime logonTime)
		{
			MailboxUserProfile.logonTimeCache[puid] = logonTime;
		}

		public static DateTime GetLastLogonTimeFromMailbox(ADUser adUser, out string errorMessage)
		{
			errorMessage = null;
			try
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(adUser, null);
				return MailboxUserProfile.GetLastLogonTimeFromMailbox(exchangePrincipal);
			}
			catch (MapiTransientException ex)
			{
				errorMessage = ex.Message;
			}
			catch (ObjectNotFoundException ex2)
			{
				errorMessage = ex2.Message;
			}
			catch (StoragePermanentException ex3)
			{
				errorMessage = ex3.ToString();
			}
			catch (StorageTransientException ex4)
			{
				errorMessage = ex4.ToString();
			}
			return DateTime.MinValue;
		}

		public static DateTime GetLastLogonTimeFromMailbox(Guid mailboxGuid, Guid databaseGuid, OrganizationId organizationId, out string errorMessage)
		{
			errorMessage = null;
			if (Guid.Empty.Equals(mailboxGuid))
			{
				errorMessage = "mailboxGuid could not be empty.";
				return DateTime.MinValue;
			}
			if (Guid.Empty.Equals(databaseGuid))
			{
				errorMessage = "databaseGuid could not be empty.";
				return DateTime.MinValue;
			}
			try
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromMailboxData(mailboxGuid, databaseGuid, organizationId, MailboxUserProfile.cultureInfo);
				return MailboxUserProfile.GetLastLogonTimeFromMailbox(exchangePrincipal);
			}
			catch (MapiTransientException ex)
			{
				errorMessage = ex.Message;
			}
			catch (ObjectNotFoundException ex2)
			{
				errorMessage = ex2.Message;
			}
			catch (StoragePermanentException ex3)
			{
				errorMessage = ex3.ToString();
			}
			catch (StorageTransientException ex4)
			{
				errorMessage = ex4.ToString();
			}
			return DateTime.MinValue;
		}

		private static DateTime GetLastLogonTimeFromMailbox(ExchangePrincipal exchangePrincipal)
		{
			DateTime result;
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(exchangePrincipal, CultureInfo.InvariantCulture, "Client=LiveIdBasicAuth;Privilege:OpenAsSystemService"))
			{
				MailboxUserProfile.counters.NumberOfMailboxAccess.Increment();
				using (UserConfiguration userProfileUserConfiguration = MailboxUserProfile.GetUserProfileUserConfiguration(mailboxSession))
				{
					if (userProfileUserConfiguration != null)
					{
						DateTime minValue = DateTime.MinValue;
						IDictionary dictionary = userProfileUserConfiguration.GetDictionary();
						if (dictionary != null && dictionary.Contains("LastLogonTime"))
						{
							string text = (string)dictionary["LastLogonTime"];
							if (text != null)
							{
								DateTime.TryParse(text, out minValue);
							}
						}
						result = minValue;
					}
					else
					{
						result = DateTime.MinValue;
					}
				}
			}
			return result;
		}

		public static bool SetLastLogonTimeInMailbox(string puid, Guid mailboxGuid, Guid databaseGuid, OrganizationId organizationId, DateTime lastLogonTime, out string errorMessage)
		{
			bool result = false;
			errorMessage = null;
			if (Guid.Empty.Equals(mailboxGuid))
			{
				errorMessage = "mailboxGuid could not be empty.";
				return false;
			}
			if (Guid.Empty.Equals(databaseGuid))
			{
				errorMessage = "databaseGuid could not be empty.";
				return false;
			}
			try
			{
				MailboxUserProfile.logonTimeCache[puid] = lastLogonTime;
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromMailboxData(mailboxGuid, databaseGuid, organizationId, MailboxUserProfile.cultureInfo);
				result = MailboxUserProfile.SetLastLogonTimeInMailbox(puid, exchangePrincipal, lastLogonTime);
			}
			catch (WrongServerException ex)
			{
				errorMessage = ex.Message;
			}
			catch (ObjectNotFoundException ex2)
			{
				errorMessage = ex2.Message;
			}
			catch (StoragePermanentException ex3)
			{
				errorMessage = ex3.ToString();
			}
			catch (StorageTransientException ex4)
			{
				errorMessage = ex4.ToString();
			}
			return result;
		}

		public static bool SetLastLogonTimeInMailbox(ADUser adUser, DateTime lastLogonTime, out string errorMessage)
		{
			bool result = false;
			errorMessage = null;
			try
			{
				MailboxUserProfile.logonTimeCache[adUser.NetID.ToString()] = lastLogonTime;
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(adUser, null);
				result = MailboxUserProfile.SetLastLogonTimeInMailbox(adUser.NetID.ToString(), exchangePrincipal, lastLogonTime);
			}
			catch (WrongServerException ex)
			{
				errorMessage = ex.Message;
			}
			catch (ObjectNotFoundException ex2)
			{
				errorMessage = ex2.Message;
			}
			catch (StoragePermanentException ex3)
			{
				errorMessage = ex3.ToString();
			}
			catch (StorageTransientException ex4)
			{
				errorMessage = ex4.ToString();
			}
			return result;
		}

		internal static ADUser GetRPSProfileFromMailbox(ulong puid)
		{
			ADUser result;
			try
			{
				ADObjectId adobjectIdFromPuid = ConsumerIdentityHelper.GetADObjectIdFromPuid(puid);
				result = MailboxUserProfile.outlookUserSession.FindADUserByObjectId(adobjectIdFromPuid);
			}
			catch (ADDriverStoreAccessPermanentException ex)
			{
				if (ex.InnerException == null || !(ex.InnerException is MapiExceptionUserInformationNotFound))
				{
					throw;
				}
				result = null;
			}
			return result;
		}

		public static bool SetRPSProfileToMailbox(ulong puid, RPSProfile rpsProfile, out string errorMessage, bool updateOnly = true)
		{
			errorMessage = null;
			if (rpsProfile == null || rpsProfile.Profile == null)
			{
				errorMessage = "NullRpsProfile";
				return false;
			}
			bool result = false;
			try
			{
				ADUser rpsprofileFromMailbox = MailboxUserProfile.GetRPSProfileFromMailbox(puid);
				if (rpsprofileFromMailbox == null)
				{
					errorMessage = "No record in Mserv " + puid.ToString("X");
					return false;
				}
				IAggregateSession aggregateSession = MailboxUserProfile.outlookUserSession as IAggregateSession;
				aggregateSession.BackendWriteMode = BackendWriteMode.WriteToMbx;
				ProxyAddressCollection emailAddresses = rpsprofileFromMailbox.EmailAddresses;
				if (updateOnly && (byte)(rpsprofileFromMailbox.DirectoryBackendType & DirectoryBackendType.Mbx) != 4)
				{
					errorMessage = "<NoEntry>";
				}
				else
				{
					if (rpsProfile.MemberName != null)
					{
						ProxyAddress item = (ProxyAddress)ProxyAddress.Parse(rpsProfile.MemberName).ToPrimary();
						if (!emailAddresses.Contains(item))
						{
							ProxyAddress proxyAddress = null;
							foreach (ProxyAddress proxyAddress2 in emailAddresses)
							{
								if (proxyAddress2.IsPrimaryAddress)
								{
									proxyAddress = proxyAddress2;
									break;
								}
							}
							if (proxyAddress != null)
							{
								emailAddresses.Remove(proxyAddress);
							}
							emailAddresses.Add(item);
						}
					}
					if (!string.IsNullOrEmpty(rpsProfile.CurrentAlias) && !string.Equals(rpsProfile.CurrentAlias, rpsProfile.MemberName, StringComparison.OrdinalIgnoreCase))
					{
						ProxyAddress item2 = ProxyAddress.Parse(rpsProfile.CurrentAlias);
						if (!emailAddresses.Contains(item2))
						{
							emailAddresses.Add(item2);
						}
					}
					if (rpsProfile.Profile.Gender != null && (!(rpsprofileFromMailbox[ADUserSchema.Gender] is string) || !((string)rpsprofileFromMailbox[ADUserSchema.Gender]).Equals(rpsProfile.Profile.Gender.ToString(), StringComparison.OrdinalIgnoreCase)))
					{
						rpsprofileFromMailbox[ADUserSchema.Gender] = rpsProfile.Profile.Gender.ToString();
					}
					if (rpsProfile.Profile.Occupation != null && (!(rpsprofileFromMailbox[ADUserSchema.Occupation] is string) || !((string)rpsprofileFromMailbox[ADUserSchema.Occupation]).Equals(rpsProfile.Profile.Occupation, StringComparison.OrdinalIgnoreCase)))
					{
						rpsprofileFromMailbox[ADUserSchema.Occupation] = rpsProfile.Profile.Occupation;
					}
					if (rpsProfile.Profile.Region != 0 && (rpsprofileFromMailbox[ADUserSchema.Region] is string || !((string)rpsprofileFromMailbox[ADUserSchema.Region]).Equals(rpsProfile.Profile.Region.ToString(), StringComparison.OrdinalIgnoreCase)))
					{
						rpsprofileFromMailbox[ADUserSchema.Region] = rpsProfile.Profile.Region.ToString();
					}
					if (rpsProfile.Profile.TimeZone != 0 && (!(rpsprofileFromMailbox[ADUserSchema.Timezone] is string) || !((string)rpsprofileFromMailbox[ADUserSchema.Timezone]).Equals(rpsProfile.Profile.TimeZone.ToString(), StringComparison.OrdinalIgnoreCase)))
					{
						rpsprofileFromMailbox[ADUserSchema.Timezone] = rpsProfile.Profile.TimeZone.ToString();
					}
					if (rpsProfile.Profile.Birthday != DateTime.MinValue && (!(rpsprofileFromMailbox[ADUserSchema.BirthdayPrecision] is string) || !((string)rpsprofileFromMailbox[ADUserSchema.BirthdayPrecision]).Equals(rpsProfile.Profile.Birthday.ToString(), StringComparison.OrdinalIgnoreCase)))
					{
						rpsprofileFromMailbox[ADUserSchema.BirthdayPrecision] = rpsProfile.Profile.Birthday.ToString();
					}
					if (rpsProfile.Profile.AliasVersion != 0U && (!(rpsprofileFromMailbox[ADUserSchema.NameVersion] is string) || !((string)rpsprofileFromMailbox[ADUserSchema.NameVersion]).Equals(rpsProfile.Profile.AliasVersion.ToString(), StringComparison.OrdinalIgnoreCase)))
					{
						rpsprofileFromMailbox[ADUserSchema.NameVersion] = rpsProfile.Profile.AliasVersion.ToString();
					}
					if (rpsProfile.Profile.PostalCode != null && (!(rpsprofileFromMailbox[ADUserSchema.PostalCode] is string) || !((string)rpsprofileFromMailbox[ADUserSchema.PostalCode]).Equals(rpsProfile.Profile.PostalCode, StringComparison.OrdinalIgnoreCase)))
					{
						rpsprofileFromMailbox[ADUserSchema.PostalCode] = rpsProfile.Profile.PostalCode;
					}
					if (rpsProfile.Profile.FirstName != null && (!(rpsprofileFromMailbox[ADUserSchema.FirstName] is string) || !((string)rpsprofileFromMailbox[ADUserSchema.FirstName]).Equals(rpsProfile.Profile.FirstName, StringComparison.OrdinalIgnoreCase)))
					{
						rpsprofileFromMailbox[ADUserSchema.FirstName] = rpsProfile.Profile.FirstName;
					}
					if (rpsProfile.Profile.LastName != null && (!(rpsprofileFromMailbox[ADUserSchema.LastName] is string) || !((string)rpsprofileFromMailbox[ADUserSchema.LastName]).Equals(rpsProfile.Profile.LastName, StringComparison.OrdinalIgnoreCase)))
					{
						rpsprofileFromMailbox[ADUserSchema.LastName] = rpsProfile.Profile.LastName;
					}
					if (rpsProfile.Profile.Country != null && (!(rpsprofileFromMailbox[ADUserSchema.Country] is string) || !((string)rpsprofileFromMailbox[ADUserSchema.Country]).Equals(rpsProfile.Profile.Country, StringComparison.OrdinalIgnoreCase)))
					{
						rpsprofileFromMailbox[ADUserSchema.Country] = rpsProfile.Profile.Country;
					}
					MailboxUserProfile.outlookUserSession.Save(rpsprofileFromMailbox);
					result = true;
				}
			}
			catch (ADDriverStoreAccessPermanentException ex)
			{
				errorMessage = ex.Message;
			}
			catch (ADDriverStoreAccessTransientException ex2)
			{
				errorMessage = ex2.Message;
			}
			catch (MapiPermanentException ex3)
			{
				errorMessage = ex3.Message;
			}
			catch (MapiTransientException ex4)
			{
				errorMessage = ex4.Message;
			}
			return result;
		}

		private static bool SetLastLogonTimeInMailbox(string puid, ExchangePrincipal exchangePrincipal, DateTime lastLogonTime)
		{
			bool result = false;
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(exchangePrincipal, CultureInfo.InvariantCulture, "Client=LiveIdBasicAuth;Privilege:OpenAsSystemService"))
			{
				MailboxUserProfile.counters.NumberOfMailboxAccess.Increment();
				using (UserConfiguration userProfileUserConfiguration = MailboxUserProfile.GetUserProfileUserConfiguration(mailboxSession))
				{
					if (userProfileUserConfiguration != null)
					{
						IDictionary dictionary = userProfileUserConfiguration.GetDictionary();
						if (dictionary != null)
						{
							dictionary["LastLogonTime"] = lastLogonTime.ToString();
							userProfileUserConfiguration.Save();
							result = true;
						}
					}
				}
			}
			return result;
		}

		private static UserConfiguration GetUserProfileUserConfiguration(MailboxSession session)
		{
			return MailboxUserProfile.GetUserConfiguration("UserProfile", UserConfigurationTypes.Dictionary, session, true, false, null);
		}

		private static UserConfiguration GetUserConfiguration(string userConfigurationName, UserConfigurationTypes userConfigType, MailboxSession session, bool createIfMissing, bool isMailboxConfiguration, StoreObjectId messageId)
		{
			UserConfiguration userConfiguration = null;
			StoreId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.Inbox);
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				try
				{
					if (isMailboxConfiguration)
					{
						userConfiguration = session.UserConfigurationManager.GetMailboxConfiguration(userConfigurationName, userConfigType);
					}
					else
					{
						userConfiguration = session.UserConfigurationManager.GetFolderConfiguration(userConfigurationName, userConfigType, defaultFolderId, messageId);
					}
					disposeGuard.Add<UserConfiguration>(userConfiguration);
				}
				catch (ObjectNotFoundException)
				{
				}
				catch (CorruptDataException)
				{
				}
				if (userConfiguration == null && createIfMissing && !isMailboxConfiguration)
				{
					userConfiguration = MailboxUserProfile.ResetModel(userConfigurationName, userConfigType, session);
					disposeGuard.Add<UserConfiguration>(userConfiguration);
				}
				disposeGuard.Success();
			}
			return userConfiguration;
		}

		internal static UserConfiguration ResetModel(string userConfigurationName, UserConfigurationTypes userConfigType, MailboxSession session)
		{
			StoreId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.Inbox);
			UserConfiguration result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				UserConfiguration userConfiguration = session.UserConfigurationManager.CreateFolderConfiguration(userConfigurationName, userConfigType, defaultFolderId);
				disposeGuard.Add<UserConfiguration>(userConfiguration);
				userConfiguration.Save();
				disposeGuard.Success();
				result = userConfiguration;
			}
			return result;
		}

		private const string UserProfileConfigName = "UserProfile";

		private const string LastLogonTimeName = "LastLogonTime";

		private const string ClientInfo = "Client=LiveIdBasicAuth;Privilege:OpenAsSystemService";

		private static ConcurrentDictionary<string, DateTime> logonTimeCache = new ConcurrentDictionary<string, DateTime>(4, 10000);

		private static readonly LiveIdBasicAuthenticationCountersInstance counters = LiveIdBasicAuthenticationCounters.GetInstance(Process.GetCurrentProcess().ProcessName);

		private static readonly ICollection<CultureInfo> cultureInfo = new CultureInfo[]
		{
			CultureInfo.InvariantCulture
		};

		private static ITenantRecipientSession outlookUserSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, false, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromTenantAcceptedDomain("outlook.com"), 47, "outlookUserSession", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\Mailbox\\MailboxUserProfile.cs");

		private static ADPropertyDefinition[] mbxProperties = new ADPropertyDefinition[]
		{
			ADUserSchema.Gender,
			ADUserSchema.Occupation,
			ADUserSchema.Region,
			ADUserSchema.Country,
			ADUserSchema.Timezone,
			ADUserSchema.BirthdayPrecision,
			ADUserSchema.NameVersion,
			ADUserSchema.PostalCode,
			ADUserSchema.FirstName,
			ADUserSchema.LastName,
			ADUserSchema.Languages,
			ADRecipientSchema.EmailAddresses
		};
	}
}
