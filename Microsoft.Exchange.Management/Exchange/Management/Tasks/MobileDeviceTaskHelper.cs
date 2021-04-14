using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.AirSync;

namespace Microsoft.Exchange.Management.Tasks
{
	public static class MobileDeviceTaskHelper
	{
		internal static ExchangePrincipal GetExchangePrincipal(ADSessionSettings sessionSettings, IRecipientSession recipientSession, MailboxIdParameter mailbox, string cmdletName, out Exception localizedError)
		{
			ADUser aduser = null;
			return MobileDeviceTaskHelper.GetExchangePrincipal(sessionSettings, recipientSession, mailbox, cmdletName, out localizedError, out aduser);
		}

		internal static ExchangePrincipal GetExchangePrincipal(ADSessionSettings sessionSettings, IRecipientSession recipientSession, MailboxIdParameter mailbox, string cmdletName, out Exception localizedError, out ADUser adUser)
		{
			localizedError = null;
			adUser = null;
			mailbox.SearchWithDisplayName = false;
			IEnumerable<ADRecipient> objects = mailbox.GetObjects<ADRecipient>(null, recipientSession);
			ExchangePrincipal result;
			using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					localizedError = new RecipientNotFoundException(mailbox.ToString());
					result = null;
				}
				else
				{
					ADUser aduser = enumerator.Current as ADUser;
					if (enumerator.MoveNext())
					{
						localizedError = new RecipientNotUniqueException(mailbox.ToString());
						result = null;
					}
					else if (aduser == null || (aduser.RecipientType != RecipientType.UserMailbox && aduser.RecipientType != RecipientType.MailUser))
					{
						localizedError = new RecipientNotValidException(mailbox.ToString());
						result = null;
					}
					else if ((int)aduser.ExchangeVersion.ExchangeBuild.Major > Server.CurrentExchangeMajorVersion || (int)aduser.ExchangeVersion.ExchangeBuild.Major < Server.Exchange2009MajorVersion)
					{
						localizedError = new ServerVersionNotSupportedException(cmdletName, Server.CurrentExchangeMajorVersion, (int)aduser.ExchangeVersion.ExchangeBuild.Major);
						result = null;
					}
					else
					{
						ADSessionSettings adSettings = sessionSettings;
						IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, sessionSettings, 125, "GetExchangePrincipal", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\AirSync\\MobileDeviceTaskHelper.cs");
						if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(tenantOrRootOrgRecipientSession, aduser))
						{
							IDirectorySession directorySession = TaskHelper.UnderscopeSessionToOrganization(tenantOrRootOrgRecipientSession, aduser.OrganizationId, sessionSettings, true);
							adSettings = directorySession.SessionSettings;
						}
						ExchangePrincipal exchangePrincipal = null;
						try
						{
							adUser = aduser;
							exchangePrincipal = ExchangePrincipal.FromADUser(adSettings, aduser, RemotingOptions.AllowCrossSite);
						}
						catch (UserHasNoMailboxException ex)
						{
							localizedError = ex;
							return null;
						}
						if (exchangePrincipal == null)
						{
							localizedError = new ExchangePrincipalNotFoundException(aduser.ToString());
							result = null;
						}
						else
						{
							result = exchangePrincipal;
						}
					}
				}
			}
			return result;
		}

		internal static List<string> ValidateAddresses(IRecipientSession recipientSession, ADObjectId executingUserId, MultiValuedProperty<string> notificationEmailAddresses, out IList<LocalizedString> localizedWarnings)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			List<string> list = new List<string>(((notificationEmailAddresses != null) ? notificationEmailAddresses.Count : 0) + 1);
			localizedWarnings = new List<LocalizedString>(list.Count);
			if (executingUserId != null)
			{
				ADRecipient adrecipient = recipientSession.Read(executingUserId);
				if (adrecipient == null || !MobileDeviceTaskHelper.IsUserMailboxEnabled(adrecipient))
				{
					localizedWarnings.Add(Strings.LogonUserIsNotAValidADRecipient(executingUserId.ToString()));
				}
				else
				{
					list.Add(adrecipient.PrimarySmtpAddress.ToString());
				}
			}
			if (notificationEmailAddresses != null)
			{
				foreach (string text in notificationEmailAddresses)
				{
					if (!SmtpAddress.IsValidSmtpAddress(text))
					{
						ADRecipient[] array = recipientSession.FindByANR(text, 2, null);
						if (array == null || array.Length != 1 || !MobileDeviceTaskHelper.IsUserMailboxEnabled(array[0]))
						{
							localizedWarnings.Add(Strings.InvalidSmtpAddressOrAlias(text));
						}
						else
						{
							list.Add(array[0].PrimarySmtpAddress.ToString());
						}
					}
					else
					{
						IRecipientSession recipientSession2 = recipientSession;
						if (GlobalSettings.IsMultiTenancyEnabled)
						{
							try
							{
								ADSessionSettings sessionSettings = ADSessionSettings.FromTenantAcceptedDomain(SmtpAddress.Parse(text).Domain);
								recipientSession2 = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 240, "ValidateAddresses", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\AirSync\\MobileDeviceTaskHelper.cs");
							}
							catch (CannotResolveTenantNameException)
							{
								localizedWarnings.Add(Strings.InvalidSmtpAddressOrAlias(text));
								recipientSession2 = null;
							}
						}
						if (recipientSession2 != null)
						{
							ADRecipient adrecipient2 = recipientSession2.FindByProxyAddress(ProxyAddress.Parse(text));
							if (adrecipient2 == null)
							{
								localizedWarnings.Add(Strings.InvalidSmtpAddressOrAlias(text));
							}
							else
							{
								list.Add(adrecipient2.PrimarySmtpAddress.ToString());
							}
						}
					}
				}
			}
			return list;
		}

		internal static DeviceInfo GetDeviceInfo(MailboxSession mailboxSession, MobileDeviceIdParameter identity)
		{
			DeviceInfo[] allDeviceInfo = DeviceInfo.GetAllDeviceInfo(mailboxSession, MobileClientType.EAS | MobileClientType.MOWA);
			if (allDeviceInfo != null)
			{
				foreach (DeviceInfo deviceInfo in allDeviceInfo)
				{
					string protocol = null;
					DeviceIdentity.TryGetProtocol(identity.ClientType, out protocol);
					if (deviceInfo.DeviceIdentity.Equals(identity.DeviceId, identity.DeviceType, protocol))
					{
						return deviceInfo;
					}
				}
			}
			return null;
		}

		internal static bool IsRunningUnderMyOptionsRole(Task task, IRecipientSession recipientSession, ADSessionSettings settings)
		{
			if (task.ExchangeRunspaceConfig == null)
			{
				return false;
			}
			ADObjectId entryId;
			if (!task.ExchangeRunspaceConfig.TryGetExecutingUserId(out entryId))
			{
				return false;
			}
			RbacScope rbacScope = new RbacScope(ScopeType.Self);
			ADUser aduser = (ADUser)recipientSession.Read(entryId);
			if (aduser == null)
			{
				return false;
			}
			rbacScope.PopulateRootAndFilter(task.ExecutingUserOrganizationId, aduser);
			return settings.RecipientReadScope.Equals(rbacScope);
		}

		private static bool IsUserMailboxEnabled(ADRecipient recipient)
		{
			ADUser aduser = recipient as ADUser;
			return aduser != null && aduser.IsMailboxEnabled;
		}
	}
}
