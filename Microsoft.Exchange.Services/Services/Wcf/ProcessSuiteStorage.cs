using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class ProcessSuiteStorage : ServiceCommand<SuiteStorageResponse>
	{
		public ProcessSuiteStorage(CallContext callContext, SuiteStorageRequest request) : base(callContext)
		{
			this.request = request;
			string name = typeof(ProcessSuiteStorage).Name;
			OwsLogRegistry.Register(name, typeof(SuiteStorage), new Type[0]);
		}

		protected override SuiteStorageResponse InternalExecute()
		{
			this.response = new SuiteStorageResponse();
			this.mailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			if (this.SetSettings(this.GetWriteSettings("User")) && this.SetOrgSettings(this.GetWriteSettings("Org")))
			{
				this.Load();
			}
			return this.response;
		}

		private void GetOrgMailboxExchangePrincipal()
		{
			this.orgMailboxExchangePrincipal = OrgSuiteStorageHelper.GetOrgMailbox(this.mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.Domain, base.CallContext.ProtocolLog);
		}

		private bool SetSettings(SuiteStorageType[] writeSettings)
		{
			if (writeSettings == null || writeSettings.Length == 0)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[ProcessSuiteStorage::SetSettings] No settings will be written to user mailbox storage.");
				base.CallContext.ProtocolLog.Set(SuiteStorage.SetUserSettings, "None");
				return true;
			}
			return this.SetSettings(this.mailboxSession, writeSettings);
		}

		private bool SetSettings(MailboxSession mailboxSession, SuiteStorageType[] writeSettings)
		{
			bool result;
			using (UserConfiguration userConfiguration = ProcessSuiteStorage.GetUserConfiguration(new UserConfigurationManager(mailboxSession), base.CallContext.ProtocolLog))
			{
				result = this.UpdateStorage(userConfiguration, writeSettings);
			}
			return result;
		}

		private bool SetOrgSettings(SuiteStorageType[] writeSettings)
		{
			if (writeSettings == null || writeSettings.Length == 0)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[ProcessSuiteStorage::SetOrgSettings] No settings will be written to org mailbox storage.");
				base.CallContext.ProtocolLog.Set(SuiteStorage.SetOrgSettings, "None");
				return true;
			}
			ManagementRoleType managementRoleType = new ManagementRoleType
			{
				UserRoles = new string[]
				{
					"OrganizationConfiguration"
				}
			};
			if (base.CallContext.IsOAuthUser)
			{
				managementRoleType.ApplicationRoles = new string[]
				{
					"OrganizationConfiguration"
				};
			}
			AuthZClientInfo effectiveCaller = base.CallContext.EffectiveCaller;
			effectiveCaller.ApplyManagementRole(managementRoleType, WebMethodEntry.JsonWebMethodEntry);
			bool result;
			try
			{
				AuthZBehavior authZBehavior = base.CallContext.EffectiveCaller.GetAuthZBehavior();
				if (!authZBehavior.IsAllowedToPrivilegedOpenMailbox(base.CallContext.AccessingPrincipal))
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[ProcessSuiteStorage::SetOrgSettings] User is not in admin role.");
					base.CallContext.ProtocolLog.Set(SuiteStorage.AuthZUserNotInAdminRole, base.CallContext.AccessingPrincipal.MailboxInfo.DisplayName);
					result = false;
				}
				else
				{
					if (this.orgMailboxExchangePrincipal == null)
					{
						this.GetOrgMailboxExchangePrincipal();
					}
					if (this.orgMailboxExchangePrincipal == null)
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[ProcessSuiteStorage::SetOrgSettings] Failed to get the org mailbox.");
						base.CallContext.ProtocolLog.Set(SuiteStorage.SetOrgSettingsExMbxPrincipal, "IsNull");
						result = false;
					}
					else
					{
						base.CallContext.ProtocolLog.Set(SuiteStorage.SetOrgSettingsExMbxPrincipal, this.orgMailboxExchangePrincipal.MailboxInfo.MailboxGuid.ToString());
						using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(this.orgMailboxExchangePrincipal, CultureInfo.CurrentCulture, this.mailboxSession.ClientInfoString))
						{
							result = this.SetSettings(mailboxSession, writeSettings);
						}
					}
				}
			}
			catch (ServiceAccessDeniedException value)
			{
				ExTraceGlobals.ExceptionTracer.TraceError(0L, "[ProcessSuiteStorage::SetOrgSettings] ServiceAccessDeniedException");
				base.CallContext.ProtocolLog.Set(SuiteStorage.MessageUnableToLoadRBACSettingsException, value);
				throw;
			}
			catch (NullReferenceException ex)
			{
				string text = string.Empty;
				if (base.CallContext.EffectiveCaller.ClientSecurityContext != null)
				{
					try
					{
						text = base.CallContext.EffectiveCaller.ClientSecurityContext.UserSid.ToString();
					}
					catch (InvalidOperationException)
					{
						text = "UserSid is invalid.";
					}
				}
				string value2 = string.Format("UserKind: {0}, LogonType: {1}, User SID: {2}, UserAgent: {3}, Exception: {4}", new object[]
				{
					base.CallContext.UserKindSource,
					base.CallContext.LogonType,
					text,
					base.CallContext.UserAgent,
					ex
				});
				ExTraceGlobals.ExceptionTracer.TraceError(0L, "[ProcessSuiteStorage::SetOrgSettings] NullReferenceException");
				base.CallContext.ProtocolLog.Set(SuiteStorage.MessageUnableToLoadRBACSettingsException, value2);
				throw;
			}
			return result;
		}

		private void Load()
		{
			List<SuiteStorageType> list = new List<SuiteStorageType>();
			list.AddRange(this.LoadUserSettings());
			list.AddRange(this.LoadOrgSettings());
			this.response.Settings = list.ToArray();
		}

		private List<SuiteStorageType> LoadUserSettings()
		{
			List<SuiteStorageType> result = new List<SuiteStorageType>();
			SuiteStorageKeyType[] readSettings = this.GetReadSettings("User");
			if (readSettings == null || readSettings.Length == 0)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[ProcessSuiteStorage::LoadUserSettings] No user settings requested.");
				base.CallContext.ProtocolLog.Set(SuiteStorage.LoadUserSettings, "None");
				return result;
			}
			try
			{
				using (UserConfiguration userConfiguration = ProcessSuiteStorage.GetUserConfiguration(new UserConfigurationManager(this.mailboxSession), base.CallContext.ProtocolLog))
				{
					result = this.ReadStorage(userConfiguration, readSettings);
				}
			}
			catch (QuotaExceededException)
			{
			}
			return result;
		}

		private List<SuiteStorageType> LoadOrgSettings()
		{
			List<SuiteStorageType> result = new List<SuiteStorageType>();
			SuiteStorageKeyType[] readSettings = this.GetReadSettings("Org");
			if (readSettings == null || readSettings.Length == 0)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[ProcessSuiteStorage::LoadOrgSettings] No user settings requested.");
				base.CallContext.ProtocolLog.Set(SuiteStorage.LoadOrgSettings, "None");
				return result;
			}
			if (this.orgMailboxExchangePrincipal == null)
			{
				this.GetOrgMailboxExchangePrincipal();
			}
			if (this.orgMailboxExchangePrincipal == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[ProcessSuiteStorage::LoadUserSettings] Failed to get org mailbox.");
				base.CallContext.ProtocolLog.Set(SuiteStorage.LoadOrgSettingsExMbxPrincipal, "IsNull");
				return result;
			}
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(this.orgMailboxExchangePrincipal, CultureInfo.CurrentCulture, this.mailboxSession.ClientInfoString))
			{
				using (UserConfiguration userConfiguration = ProcessSuiteStorage.GetUserConfiguration(new UserConfigurationManager(mailboxSession), base.CallContext.ProtocolLog))
				{
					result = this.ReadStorage(userConfiguration, readSettings);
				}
			}
			return result;
		}

		private static string ConcatName(string ns, string name)
		{
			string arg = string.IsNullOrEmpty(ns) ? ns : string.Format("{0}/", ns);
			return string.Format("{0}{1}", arg, name);
		}

		private static SuiteStorageType CreateStorageValue(SuiteStorageKeyType suiteStorageKey, string value)
		{
			return new SuiteStorageType
			{
				Key = suiteStorageKey,
				Value = value
			};
		}

		private static UserConfiguration GetUserConfiguration(UserConfigurationManager userConfigurationManager, RequestDetailsLogger logger)
		{
			UserConfiguration result = null;
			try
			{
				result = userConfigurationManager.GetMailboxConfiguration("Suite.Storage", UserConfigurationTypes.Dictionary);
			}
			catch (ObjectNotFoundException)
			{
				result = ProcessSuiteStorage.CreateMailboxConfiguration(userConfigurationManager, logger);
			}
			return result;
		}

		private static UserConfiguration CreateMailboxConfiguration(UserConfigurationManager userConfigurationManager, RequestDetailsLogger logger)
		{
			UserConfiguration userConfiguration = null;
			try
			{
				userConfiguration = userConfigurationManager.CreateMailboxConfiguration("Suite.Storage", UserConfigurationTypes.Dictionary);
				userConfiguration.Save();
			}
			catch (QuotaExceededException ex)
			{
				logger.Set(SuiteStorage.QuotaExceededException, ex.ToString());
				throw;
			}
			catch (SaveConflictException ex2)
			{
				logger.Set(SuiteStorage.SaveConflictException, ex2.ToString());
				throw;
			}
			return userConfiguration;
		}

		private bool UpdateStorage(UserConfiguration configuration, SuiteStorageType[] writeSettings)
		{
			int num = 0;
			int num2 = 0;
			string value = null;
			if (configuration == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[ProcessSuiteStorage::UpdateStorage] Failed to create mailbox user configuraiton.");
				base.CallContext.ProtocolLog.Set(SuiteStorage.UpdateStorageUpdateSetting, "IsNull");
				return false;
			}
			IDictionary dictionary = configuration.GetDictionary();
			foreach (SuiteStorageType suiteStorageType in writeSettings)
			{
				value = suiteStorageType.Key.Scope;
				string key = ProcessSuiteStorage.ConcatName(suiteStorageType.Key.Namespace, suiteStorageType.Key.Name);
				if (dictionary[key] == null)
				{
					dictionary.Add(key, suiteStorageType.Value);
				}
				else
				{
					dictionary[key] = suiteStorageType.Value;
				}
				num += Encoding.UTF8.GetByteCount(suiteStorageType.Value);
				num2++;
			}
			string[] value2 = (from s in writeSettings
			select ProcessSuiteStorage.ConcatName(s.Key.Namespace, s.Key.Name)).ToArray<string>();
			base.CallContext.ProtocolLog.Set(SuiteStorage.UpdateStorageUpdateSetting, string.Join(",", value2));
			try
			{
				configuration.Save();
			}
			catch (StoragePermanentException ex)
			{
				ExTraceGlobals.ExceptionTracer.TraceError(0L, "[ProcessSuiteStorage::UpdateStorage] StoragePermanentException.");
				base.CallContext.ProtocolLog.Set(SuiteStorage.StoragePermanentException, ex.ToString());
				throw;
			}
			catch (StorageTransientException ex2)
			{
				ExTraceGlobals.ExceptionTracer.TraceError(0L, "[ProcessSuiteStorage::UpdateStorage] StorageTransientException.");
				base.CallContext.ProtocolLog.Set(SuiteStorage.StorageTransientException, ex2.ToString());
				throw;
			}
			base.CallContext.ProtocolLog.Set(SuiteStorage.UpdateStorageMailboxScope, value);
			base.CallContext.ProtocolLog.Set(SuiteStorage.UpdateStorageUpdateSettingTotal, num2);
			base.CallContext.ProtocolLog.Set(SuiteStorage.UpdateStorageTotalBytes, num);
			return true;
		}

		private List<SuiteStorageType> ReadStorage(UserConfiguration configuration, SuiteStorageKeyType[] readSettings)
		{
			int num = 0;
			int num2 = 0;
			string value = null;
			List<SuiteStorageType> list = new List<SuiteStorageType>();
			if (configuration == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[ProcessSuiteStorage::ReadStorage] Failed to create mailbox user configuraiton.");
				base.CallContext.ProtocolLog.Set(SuiteStorage.ReadStorageUserConfiguration, "IsNull");
				return list;
			}
			IDictionary dictionary = configuration.GetDictionary();
			foreach (SuiteStorageKeyType suiteStorageKeyType in readSettings)
			{
				value = suiteStorageKeyType.Scope;
				string text = ProcessSuiteStorage.ConcatName(suiteStorageKeyType.Namespace, suiteStorageKeyType.Name);
				if (dictionary[text] != null)
				{
					list.Add(ProcessSuiteStorage.CreateStorageValue(suiteStorageKeyType, dictionary[text].ToString()));
					num += Encoding.UTF8.GetByteCount(dictionary[text].ToString());
					num2++;
				}
				else
				{
					base.CallContext.ProtocolLog.Set(SuiteStorage.ReadStorageSettingNotFound, text);
				}
			}
			string[] value2 = (from s in readSettings
			select ProcessSuiteStorage.ConcatName(s.Namespace, s.Name)).ToArray<string>();
			base.CallContext.ProtocolLog.Set(SuiteStorage.ReadStorageSetting, string.Join(",", value2));
			base.CallContext.ProtocolLog.Set(SuiteStorage.ReadStorageMailboxScope, value);
			base.CallContext.ProtocolLog.Set(SuiteStorage.ReadStorageSettingTotal, num2);
			base.CallContext.ProtocolLog.Set(SuiteStorage.ReadStorageTotalBytes, num);
			return list;
		}

		private SuiteStorageType[] GetWriteSettings(string scope)
		{
			SuiteStorageType[] result = null;
			if (this.request.WriteSettings != null)
			{
				IEnumerable<SuiteStorageType> source = from s in this.request.WriteSettings
				where s.Key.Scope == scope
				select s;
				result = source.ToArray<SuiteStorageType>();
			}
			return result;
		}

		private SuiteStorageKeyType[] GetReadSettings(string scope)
		{
			SuiteStorageKeyType[] result = null;
			if (this.request.WriteSettings != null)
			{
				IEnumerable<SuiteStorageKeyType> source = from s in this.request.ReadSettings
				where s.Scope == scope
				select s;
				result = source.ToArray<SuiteStorageKeyType>();
			}
			return result;
		}

		private MailboxSession mailboxSession;

		private SuiteStorageRequest request;

		private SuiteStorageResponse response;

		private ExchangePrincipal orgMailboxExchangePrincipal;
	}
}
