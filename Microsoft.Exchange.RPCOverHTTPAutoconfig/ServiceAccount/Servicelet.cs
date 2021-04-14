using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.ServiceAccount;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.ServiceHost;
using Microsoft.Exchange.Servicelets.RPCHTTP.Messages;

namespace Microsoft.Exchange.Servicelets.ServiceAccount
{
	public class Servicelet : Servicelet
	{
		public override void Work()
		{
			Servicelet.GeneralTracer.TraceDebug(0L, "Starting ServiceAccount Servicelet");
			this.updateLogonTimeInterval = this.GetConfigTimeSpan("ServiceAccountRefreshInterval", Servicelet.ServiceAccountRefreshIntervalDefault);
			bool flag = true;
			RegistryWatcher registryWatcher = null;
			try
			{
				AlternateServiceAccountConfiguration.EnsureCanDoCryptoOperations();
				goto IL_BF;
			}
			catch (InvalidOperationException arg)
			{
				Servicelet.GeneralTracer.TraceError<InvalidOperationException>(0L, "ServiceAccount Servicelet terminated: {0}", arg);
				return;
			}
			IL_4D:
			if (registryWatcher == null)
			{
				registryWatcher = this.CreateRegistryWatcher();
			}
			Servicelet.Reason arg2 = this.WaitForEvent(registryWatcher, (int)this.updateLogonTimeInterval.TotalMilliseconds);
			Servicelet.GeneralTracer.TraceDebug<Servicelet.Reason>(0L, "Reason: {0}", arg2);
			switch (arg2)
			{
			case Servicelet.Reason.Unknown:
				Thread.Sleep(Servicelet.waitAfterUnknownFailureInterval);
				registryWatcher = null;
				break;
			case Servicelet.Reason.Timeout:
				this.UpdateLogonTime();
				break;
			case Servicelet.Reason.RegistryUpdated:
				this.ReloadServiceAccounts();
				this.UpdateLogonTime();
				break;
			case Servicelet.Reason.ShuttingDown:
				flag = false;
				break;
			}
			IL_BF:
			if (flag)
			{
				goto IL_4D;
			}
			this.RemoveCredentials(this.currentConfig);
			Servicelet.GeneralTracer.Information(0L, "ServiceAccount Servicelet stopped");
		}

		private void UpdateLogonTime()
		{
			AlternateServiceAccountConfiguration alternateServiceAccountConfiguration = this.currentConfig;
			if (alternateServiceAccountConfiguration == null || alternateServiceAccountConfiguration.EffectiveCredentials.Count == 0)
			{
				return;
			}
			SecurityStatus securityStatus = SecurityStatus.OK;
			int num = 0;
			foreach (AlternateServiceAccountCredential alternateServiceAccountCredential in alternateServiceAccountConfiguration.EffectiveCredentials)
			{
				if (alternateServiceAccountCredential.IsValid)
				{
					using (AuthenticationContext authenticationContext = new AuthenticationContext())
					{
						securityStatus = authenticationContext.LogonUser(alternateServiceAccountCredential.UserName, alternateServiceAccountCredential.Domain, alternateServiceAccountCredential.Password);
						Servicelet.GeneralTracer.Information<AlternateServiceAccountCredential, SecurityStatus>(0L, "LogonUser for {0}: {1}", alternateServiceAccountCredential, securityStatus);
						if (securityStatus == SecurityStatus.OK)
						{
							num++;
						}
					}
				}
			}
			if (num == 0)
			{
				Servicelet.EventLog.LogEvent(MSExchangeRPCHTTPAutoconfigEventLogConstants.Tuple_ServiceAccountNoWorkingCredentials, string.Empty, new object[]
				{
					securityStatus
				});
			}
		}

		private void ReloadServiceAccounts()
		{
			AlternateServiceAccountConfiguration config = null;
			try
			{
				config = AlternateServiceAccountConfiguration.LoadWithPasswordsFromRegistry();
			}
			catch (DataSourceTransientException arg)
			{
				Servicelet.GeneralTracer.TraceError<DataSourceTransientException>(0L, "DataSourceTransientException: {0}", arg);
				return;
			}
			catch (DataSourceOperationException arg2)
			{
				Servicelet.GeneralTracer.TraceError<DataSourceOperationException>(0L, "DataSourceOperationException: {0}", arg2);
				return;
			}
			this.RemoveCredentials(this.currentConfig);
			this.AddCredentials(config);
			this.currentConfig = config;
		}

		private void RemoveCredentials(AlternateServiceAccountConfiguration config)
		{
			if (config == null || config.EffectiveCredentials.Count == 0)
			{
				return;
			}
			using (IEnumerator<AlternateServiceAccountCredential> enumerator = config.EffectiveCredentials.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AlternateServiceAccountCredential cred = enumerator.Current;
					if (cred.IsValid)
					{
						Servicelet.GeneralTracer.Information<AlternateServiceAccountCredential>(0L, "Removing {0}", cred);
						this.RunKerberosOperation(cred, delegate
						{
							Kerberos.RemoveExtraCredentials(cred.UserName, cred.Domain, Kerberos.LuidLocalSystem);
						});
						this.RunKerberosOperation(cred, delegate
						{
							Kerberos.RemoveExtraCredentials(cred.UserName, cred.Domain, Kerberos.LuidNetworkService);
						});
					}
					else
					{
						Servicelet.GeneralTracer.Information<AlternateServiceAccountCredential, string>(0L, "Will not remove {0}: {1}", cred, cred.ParseError.Message);
					}
				}
			}
			Servicelet.EventLog.LogEvent(MSExchangeRPCHTTPAutoconfigEventLogConstants.Tuple_ServiceAccountCredentialsRemoved, string.Empty, new object[0]);
		}

		private void AddCredentials(AlternateServiceAccountConfiguration config)
		{
			if (config == null || config.EffectiveCredentials.Count == 0)
			{
				return;
			}
			for (int i = config.EffectiveCredentials.Count - 1; i >= 0; i--)
			{
				AlternateServiceAccountCredential cred = config.EffectiveCredentials[i];
				if (cred.IsValid)
				{
					Servicelet.GeneralTracer.Information<AlternateServiceAccountCredential>(0L, "Adding {0}", cred);
					this.RunKerberosOperation(cred, delegate
					{
						Kerberos.AddExtraCredentials(cred.UserName, cred.Domain, cred.Password, Kerberos.LuidLocalSystem);
					});
					this.RunKerberosOperation(cred, delegate
					{
						Kerberos.AddExtraCredentials(cred.UserName, cred.Domain, cred.Password, Kerberos.LuidNetworkService);
					});
				}
				else
				{
					Servicelet.GeneralTracer.Information<AlternateServiceAccountCredential, string>(0L, "Will not add {0}: {1}", cred, cred.ParseError.Message);
					Servicelet.EventLog.LogEvent(MSExchangeRPCHTTPAutoconfigEventLogConstants.Tuple_ServiceAccountCredentialException, cred.ToString(), new object[]
					{
						cred.ToString(),
						cred.ParseError.Message
					});
				}
			}
			Servicelet.EventLog.LogEvent(MSExchangeRPCHTTPAutoconfigEventLogConstants.Tuple_ServiceAccountCredentialsAdded, string.Empty, new object[0]);
		}

		private void RunKerberosOperation(AlternateServiceAccountCredential cred, Action operation)
		{
			try
			{
				Privilege.RunWithPrivilege("SeTcbPrivilege", true, delegate
				{
					operation();
				});
			}
			catch (Win32Exception ex)
			{
				if (ex.NativeErrorCode == 1168)
				{
					Servicelet.GeneralTracer.TraceDebug<int, string>(0L, "Win32Exception {0}: {1}", ex.NativeErrorCode, ex.Message);
				}
				else
				{
					Servicelet.EventLog.LogEvent(MSExchangeRPCHTTPAutoconfigEventLogConstants.Tuple_ServiceAccountKerberosError, cred.ToString(), new object[]
					{
						cred.ToString(),
						ex.NativeErrorCode.ToString(CultureInfo.InvariantCulture),
						ex.Message
					});
					Servicelet.GeneralTracer.TraceError<int, string>(0L, "Win32Exception {0}: {1}", ex.NativeErrorCode, ex.Message);
				}
			}
		}

		private Servicelet.Reason WaitForEvent(RegistryWatcher watcher, int timeout)
		{
			if (watcher == null)
			{
				return Servicelet.Reason.Unknown;
			}
			DateTime utcNow = DateTime.UtcNow;
			bool flag = watcher.IsChanged(timeout, base.StopEvent);
			if (flag)
			{
				return Servicelet.Reason.RegistryUpdated;
			}
			if (base.StopEvent.WaitOne(0))
			{
				return Servicelet.Reason.ShuttingDown;
			}
			if (timeout != -1)
			{
				TimeSpan arg = DateTime.UtcNow - utcNow;
				Servicelet.GeneralTracer.TraceDebug<TimeSpan>(0L, "Elapsed time: {0}", arg);
				if (arg.TotalMilliseconds >= (double)timeout)
				{
					return Servicelet.Reason.Timeout;
				}
			}
			return Servicelet.Reason.Unknown;
		}

		private RegistryWatcher CreateRegistryWatcher()
		{
			Servicelet.GeneralTracer.TraceDebug(0L, "CreateRegistryWatcher");
			try
			{
				return AlternateServiceAccountConfiguration.CreateRegistryWatcher();
			}
			catch (DataSourceTransientException ex)
			{
				Servicelet.GeneralTracer.TraceError<string>(0L, "DataSourceTransientException: {0}", ex.Message);
			}
			catch (DataSourceOperationException ex2)
			{
				Servicelet.GeneralTracer.TraceError<string>(0L, "DataSourceOperationException: {0}", ex2.Message);
			}
			return null;
		}

		private TimeSpan GetConfigTimeSpan(string key, TimeSpan defaultValue)
		{
			string text;
			try
			{
				text = ConfigurationManager.AppSettings[key];
			}
			catch (ConfigurationErrorsException arg)
			{
				text = null;
				Servicelet.GeneralTracer.TraceError<ConfigurationErrorsException>(0L, "ConvifugationErrorsException {0}", arg);
			}
			TimeSpan timeSpan;
			if (string.IsNullOrEmpty(text) || !TimeSpan.TryParse(text, out timeSpan))
			{
				Servicelet.GeneralTracer.TraceDebug<string, TimeSpan>(0L, "Config[{0}]: {1} (default)", key, defaultValue);
				return defaultValue;
			}
			Servicelet.GeneralTracer.TraceDebug<string, TimeSpan>(0L, "Config[{0}]: {1}", key, timeSpan);
			return timeSpan;
		}

		private const string ServiceAccountRefreshIntervalKey = "ServiceAccountRefreshInterval";

		private static readonly TimeSpan ServiceAccountRefreshIntervalDefault = TimeSpan.FromHours(8.0);

		private static readonly Trace GeneralTracer = ExTraceGlobals.GeneralTracer;

		private static readonly int waitAfterUnknownFailureInterval = (int)TimeSpan.FromMinutes(1.0).TotalMilliseconds;

		private static readonly Guid ComponentGuid = new Guid("76986af5-10f0-40d2-aac4-62e85132c65a");

		private static readonly ExEventLog EventLog = new ExEventLog(Servicelet.ComponentGuid, "MSExchange RPC Over HTTP Autoconfig");

		private AlternateServiceAccountConfiguration currentConfig;

		private TimeSpan updateLogonTimeInterval;

		private enum Reason
		{
			Unknown,
			Timeout,
			RegistryUpdated,
			ShuttingDown
		}
	}
}
