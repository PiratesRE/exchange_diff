using System;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Monitoring;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal sealed class UmConnectivityCredentialsHelper
	{
		internal UmConnectivityCredentialsHelper(ADSite site, Server server)
		{
			UmConnectivityCredentialsHelper.DebugTrace("Inside UmConnectivityCredentialsHelper(). ADSite = {0}, Server = {1}", new object[]
			{
				site,
				server
			});
			SmtpAddress? enterpriseAutomatedTaskUser = TestConnectivityCredentialsManager.GetEnterpriseAutomatedTaskUser(site, server.Domain);
			this.userName = TestCasConnectivity.GetInstanceUserNameFromTestUser(enterpriseAutomatedTaskUser);
			this.domain = server.Domain;
		}

		internal ADUser User
		{
			get
			{
				return this.aduser;
			}
		}

		internal UMDialPlan UserDP
		{
			get
			{
				return this.userDP;
			}
		}

		internal bool IsUserFound
		{
			get
			{
				return this.isUserFound;
			}
		}

		internal bool IsExchangePrincipalFound
		{
			get
			{
				return this.isExchangePrincipalFound;
			}
		}

		internal bool IsUserUMEnabled
		{
			get
			{
				return this.isUserUMEnabled;
			}
		}

		internal bool SuccessfullyGotPin
		{
			get
			{
				return this.successfullyGotPin;
			}
		}

		internal string UMPin
		{
			get
			{
				return this.umPin;
			}
		}

		internal ExchangePrincipal ExPrincipal
		{
			get
			{
				return this.exp;
			}
		}

		internal string UserName
		{
			get
			{
				return this.userName;
			}
		}

		internal string UserDomain
		{
			get
			{
				return this.domain;
			}
		}

		internal static bool IsMailboxServer(Server serv)
		{
			if (serv != null && serv.IsMailboxServer && serv.IsExchange2007OrLater && serv.Id != null)
			{
				Guid objectGuid = serv.Id.ObjectGuid;
				return true;
			}
			return false;
		}

		internal static bool ResetMailboxPassword(ExchangePrincipal ep, NetworkCredential nc)
		{
			UmConnectivityCredentialsHelper.DebugTrace("Inside UmConnectivityCredentialsHelper: ResetMailboxPassword", new object[0]);
			bool flag = false;
			LocalizedException ex = TestConnectivityCredentialsManager.ResetAutomatedCredentialsAndVerify(ep, nc, false, out flag);
			if (ex != null)
			{
				UmConnectivityCredentialsHelper.DebugTrace("Inside ResetMailboxPassword(): TestConnectivityCredentialsManager.ResetAutomatedCredentialsAndVerify returned : {0} ", new object[]
				{
					ex.ToString()
				});
				return false;
			}
			return true;
		}

		internal static bool ResetUMPin(ADUser aduser, string passwd)
		{
			UmConnectivityCredentialsHelper.DebugTrace("Inside UmConnectivityCredentialsHelper: ResetUMPin", new object[0]);
			string pin;
			try
			{
				UMMailboxPolicy policyFromUser = Utility.GetPolicyFromUser(aduser);
				if (!UmConnectivityCredentialsHelper.GetRandomPINFromPasswd(passwd, policyFromUser.MinPINLength, out pin))
				{
					UmConnectivityCredentialsHelper.DebugTrace("Inside ResetUMPin(): didnt get pin", new object[0]);
					return false;
				}
			}
			catch (LocalizedException ex)
			{
				UmConnectivityCredentialsHelper.DebugTrace("Inside ResetUMPin(): got Exception = {0}", new object[]
				{
					ex.ToString()
				});
				return false;
			}
			LocalizedException ex2 = UmConnectivityCredentialsHelper.SaveUMPin(aduser, pin);
			if (ex2 != null)
			{
				UmConnectivityCredentialsHelper.DebugTrace("Inside ResetUMPin(): SaveUMPin Exception = {0}", new object[]
				{
					ex2.ToString()
				});
				return false;
			}
			return true;
		}

		internal static LocalizedException SaveUMPin(ADUser user, string pin)
		{
			try
			{
				Utils.SetUserPassword(user, pin, false, false);
			}
			catch (LocalizedException result)
			{
				return result;
			}
			return null;
		}

		internal void InitializeUser(bool dontFetchPassword)
		{
			UmConnectivityCredentialsHelper.DebugTrace("Inside InitializeUser().", new object[0]);
			this.isUserFound = UmConnectivityCredentialsHelper.FindUser(this.userName, this.domain, out this.aduser);
			if (this.isUserFound)
			{
				UmConnectivityCredentialsHelper.DebugTrace("Inside InitializeUser(). User found", new object[0]);
				this.isUserUMEnabled = this.UserUMEnabled(this.aduser);
			}
			if (this.isUserUMEnabled)
			{
				UmConnectivityCredentialsHelper.DebugTrace("Inside InitializeUser(). User UM Enabled", new object[0]);
				this.isExchangePrincipalFound = UmConnectivityCredentialsHelper.FindExchangePrincipal(this.aduser, out this.exp);
			}
			if (!dontFetchPassword && this.isExchangePrincipalFound)
			{
				UmConnectivityCredentialsHelper.DebugTrace("Inside InitializeUser(). ExchangePrincipal found", new object[0]);
				this.successfullyGotPin = this.GeneratePinFromPassword();
				UmConnectivityCredentialsHelper.DebugTrace("Inside InitializeUser(). successfullyGotPin = {0}", new object[]
				{
					this.successfullyGotPin
				});
			}
		}

		private static void DebugTrace(string formatString, params object[] formatObjects)
		{
			ExTraceGlobals.DiagnosticTracer.TraceDebug(0L, formatString, formatObjects);
		}

		private static bool GetRandomPINFromPasswd(string passwd, int len, out string pin)
		{
			pin = null;
			int num = Math.Max(len, 10);
			if (passwd == null)
			{
				return false;
			}
			byte[] bytes;
			using (SHA1Cng sha1Cng = new SHA1Cng())
			{
				bytes = sha1Cng.ComputeHash(Encoding.ASCII.GetBytes(passwd));
			}
			StringBuilder stringBuilder = new StringBuilder(Encoding.ASCII.GetString(bytes));
			int length = stringBuilder.Length;
			if (num > length)
			{
				stringBuilder.Append('0', num - length);
			}
			string temp = stringBuilder.ToString().Substring(0, num);
			pin = UmConnectivityCredentialsHelper.GetNumericPinFromString(temp);
			UmConnectivityCredentialsHelper.DebugTrace("Inside GetRandomPINFromPasswd(): pin = {0}", new object[]
			{
				pin
			});
			return true;
		}

		private static string GetNumericPinFromString(string temp)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in temp)
			{
				int value = (int)(c % '\n');
				stringBuilder.Append(value);
			}
			UmConnectivityCredentialsHelper.DebugTrace("Inside GetNumericPinFromString(): passed string = {0}, generated numeric pin ={1}", new object[]
			{
				temp,
				stringBuilder.ToString()
			});
			return stringBuilder.ToString();
		}

		private static bool FindPassword(ExchangePrincipal ep, NetworkCredential nc)
		{
			UmConnectivityCredentialsHelper.DebugTrace("Inside FindPassword()", new object[0]);
			LocalizedException ex = TestConnectivityCredentialsManager.LoadAutomatedTestCasConnectivityInfo(ep, nc);
			if (ex != null)
			{
				UmConnectivityCredentialsHelper.DebugTrace("Inside FindPassword(): TestConnectivityCredentialsManager.LoadAutomatedTestCasConnectivityInfo returned : {0}", new object[]
				{
					ex.ToString()
				});
				return false;
			}
			return true;
		}

		private static bool FindExchangePrincipal(ADUser user, out ExchangePrincipal ep)
		{
			UmConnectivityCredentialsHelper.DebugTrace("Inside FindExchangePrincipal()", new object[0]);
			ep = null;
			try
			{
				ep = ExchangePrincipal.FromADUser(user, RemotingOptions.AllowCrossSite);
			}
			catch (ObjectNotFoundException)
			{
				return false;
			}
			return true;
		}

		private static bool FindUser(string username, string domain, out ADUser user)
		{
			UmConnectivityCredentialsHelper.DebugTrace("Inside FindUser()", new object[0]);
			user = null;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(domain), 500, "FindUser", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\UmConnectivityCredentialsHelper.cs");
			string sUserPrincipalName = username + "@" + domain;
			try
			{
				using (WindowsIdentity windowsIdentity = new WindowsIdentity(sUserPrincipalName))
				{
					user = (ADUser)tenantOrRootOrgRecipientSession.FindBySid(windowsIdentity.User);
				}
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (ArgumentException)
			{
			}
			catch (LocalizedException)
			{
			}
			if (user == null)
			{
				string accountName = domain + "\\" + username;
				try
				{
					user = (ADUser)tenantOrRootOrgRecipientSession.FindByAccountName<ADRecipient>(domain, accountName);
				}
				catch (LocalizedException)
				{
				}
			}
			if (user == null)
			{
				if (username.Length > 20)
				{
					username = username.Substring(0, 20);
				}
				try
				{
					user = (ADUser)tenantOrRootOrgRecipientSession.FindByAccountName<ADRecipient>(domain, username);
				}
				catch (LocalizedException)
				{
				}
			}
			return user != null;
		}

		private bool GeneratePinFromPassword()
		{
			UmConnectivityCredentialsHelper.DebugTrace("Inside GeneratePinFromPassword()", new object[0]);
			NetworkCredential networkCredential = new NetworkCredential(this.userName, string.Empty, this.domain);
			if (!UmConnectivityCredentialsHelper.FindPassword(this.exp, networkCredential))
			{
				UmConnectivityCredentialsHelper.DebugTrace("Inside GeneratePinFromPassword(): didnt find passwd", new object[0]);
				return false;
			}
			try
			{
				UMMailboxPolicy policyFromUser = Utility.GetPolicyFromUser(this.aduser);
				if (!UmConnectivityCredentialsHelper.GetRandomPINFromPasswd(networkCredential.Password, policyFromUser.MinPINLength, out this.umPin))
				{
					UmConnectivityCredentialsHelper.DebugTrace("Inside GeneratePinFromPassword(): didnt get pin", new object[0]);
					return false;
				}
			}
			catch (LocalizedException ex)
			{
				UmConnectivityCredentialsHelper.DebugTrace("Inside GeneratePinFromPassword(): got Exception = {0}", new object[]
				{
					ex.ToString()
				});
				return false;
			}
			return true;
		}

		private bool UserUMEnabled(ADUser user)
		{
			UmConnectivityCredentialsHelper.DebugTrace("UmConnectivityCredentialsHelper::UserUMEnabled()", new object[0]);
			if (user != null)
			{
				using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(user))
				{
					if (umsubscriber != null)
					{
						this.userDP = umsubscriber.DialPlan;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private const int TestMailboxUMPinLengthMin = 10;

		private readonly string userName;

		private readonly string domain;

		private ADUser aduser;

		private bool isUserFound;

		private bool isUserUMEnabled;

		private bool isExchangePrincipalFound;

		private ExchangePrincipal exp;

		private bool successfullyGotPin;

		private string umPin;

		private UMDialPlan userDP;
	}
}
