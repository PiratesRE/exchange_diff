using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	public sealed class CommonTestTasks : Task
	{
		internal static UserWithCredential GetDefaultTestAccount(CommonTestTasks.ClientAccessContext context)
		{
			SmtpAddress? smtpAddress;
			if (TestConnectivityCredentialsManager.IsExchangeMultiTenant())
			{
				smtpAddress = TestConnectivityCredentialsManager.GetMultiTenantAutomatedTaskUser(context.Instance, context.ConfigurationSession, context.Site);
			}
			else
			{
				smtpAddress = TestConnectivityCredentialsManager.GetEnterpriseAutomatedTaskUser(context.Site, context.WindowsDomain);
			}
			if (smtpAddress == null)
			{
				throw new MailboxNotFoundException(new MailboxIdParameter(), null);
			}
			MailboxIdParameter localMailboxId = new MailboxIdParameter(string.Format("{0}\\{1}", smtpAddress.Value.Domain, smtpAddress.Value.Local));
			ADUser aduser = CommonTestTasks.EnsureSingleObject<ADUser>(() => localMailboxId.GetObjects<ADUser>(null, context.RecipientSession));
			if (aduser == null)
			{
				throw new MailboxNotFoundException(new MailboxIdParameter(smtpAddress.ToString()), null);
			}
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(aduser, null);
			if (exchangePrincipal == null)
			{
				throw new MailboxNotFoundException(new MailboxIdParameter(smtpAddress.ToString()), null);
			}
			NetworkCredential networkCredential = new NetworkCredential(smtpAddress.Value.ToString(), string.Empty, context.WindowsDomain);
			NetworkCredential networkCredential2 = CommonTestTasks.MakeCasCredential(networkCredential);
			bool flag = false;
			LocalizedException ex;
			if (Datacenter.IsLiveIDForExchangeLogin(true) || context.MonitoringContext)
			{
				ex = TestConnectivityCredentialsManager.LoadAutomatedTestCasConnectivityInfo(exchangePrincipal, networkCredential2);
			}
			else
			{
				ex = TestConnectivityCredentialsManager.ResetAutomatedCredentialsAndVerify(exchangePrincipal, networkCredential2, false, out flag);
			}
			if (ex != null)
			{
				throw ex;
			}
			networkCredential.Domain = smtpAddress.Value.Domain;
			networkCredential.Password = networkCredential2.Password;
			return new UserWithCredential
			{
				User = aduser,
				Credential = networkCredential
			};
		}

		internal static NetworkCredential MakeCasCredential(NetworkCredential networkCredential)
		{
			NetworkCredential networkCredential2 = new NetworkCredential(networkCredential.UserName, networkCredential.Password, networkCredential.Domain);
			if (!Datacenter.IsMultiTenancyEnabled())
			{
				networkCredential2.UserName = SmtpAddress.Parse(networkCredential.UserName).Local;
			}
			return networkCredential2;
		}

		internal static T EnsureSingleObject<T>(Func<IEnumerable<T>> getObjects) where T : class
		{
			T t = default(T);
			foreach (T t2 in getObjects())
			{
				if (t != null)
				{
					throw new DataValidationException(new ObjectValidationError(Strings.MoreThanOneObjects(typeof(T).ToString()), null, null));
				}
				t = t2;
			}
			return t;
		}

		internal struct ClientAccessContext
		{
			internal Task Instance;

			internal bool MonitoringContext;

			internal ITopologyConfigurationSession ConfigurationSession;

			internal IRecipientSession RecipientSession;

			internal string WindowsDomain;

			internal ADSite Site;
		}
	}
}
