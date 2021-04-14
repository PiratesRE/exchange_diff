using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal abstract class MailboxDatabaseEndpointDelegate
	{
		public abstract void Initialize(List<MailboxDatabaseInfo> validMailboxDatabases, List<MailboxDatabaseInfo> allMailBoxDataBases);

		public abstract bool DetectChange(List<MailboxDatabaseInfo> mailboxDatabases);

		protected static void FillOutMonitoringAccountInfo(MailboxDatabaseInfo dbInfo, ADUser monitoringUser)
		{
			dbInfo.MonitoringAccount = monitoringUser.Name;
			dbInfo.MonitoringAccountDisplayName = monitoringUser.DisplayName;
			dbInfo.MonitoringAccountDomain = DirectoryAccessor.Instance.DefaultMonitoringDomain.DomainName.Domain.ToString();
			dbInfo.MonitoringAccountSid = ((monitoringUser.Sid != null) ? monitoringUser.Sid.ToString() : null);
			dbInfo.MonitoringAccountPuid = ((monitoringUser[ADUserSchema.NetID] != null) ? monitoringUser[ADUserSchema.NetID].ToString() : null);
			dbInfo.MonitoringAccountPartitionId = DirectoryAccessor.Instance.MonitoringTenantPartitionId;
			dbInfo.MonitoringAccountOrganizationId = DirectoryAccessor.Instance.MonitoringTenantOrganizationId;
			dbInfo.MonitoringAccountLegacyDN = monitoringUser.LegacyExchangeDN;
			dbInfo.MonitoringAccountMailboxGuid = monitoringUser.ExchangeGuid;
			dbInfo.MonitoringAccountMailboxArchiveGuid = monitoringUser.ArchiveGuid;
			dbInfo.MonitoringMailboxLegacyExchangeDN = monitoringUser.LegacyExchangeDN;
			dbInfo.MonitoringAccountSipAddress = monitoringUser.EmailAddresses.GetSipUri();
			dbInfo.MonitoringAccountUserPrincipalName = monitoringUser.UserPrincipalName;
			dbInfo.MonitoringAccountExchangeLoginName = (Datacenter.IsLiveIDForExchangeLogin(false) ? monitoringUser.WindowsLiveID.ToString() : monitoringUser.UserPrincipalName);
			dbInfo.MonitoringAccountWindowsLoginName = ((DirectoryAccessor.Instance.MonitoringTenantForestFqdn != null) ? string.Format("{0}@{1}", monitoringUser.SamAccountName, DirectoryAccessor.Instance.MonitoringTenantForestFqdn) : monitoringUser.UserPrincipalName);
		}

		protected static void ValidateMonitoringMailboxBasicTests(MailboxDatabaseInfo info)
		{
			if (string.IsNullOrWhiteSpace(info.MonitoringAccount))
			{
				throw new InvalidMailboxDatabaseEndpointException(Strings.MonitoringAccountUnavailable(info.MailboxDatabaseName));
			}
			if (string.IsNullOrWhiteSpace(info.MonitoringAccountDomain))
			{
				throw new InvalidMailboxDatabaseEndpointException(Strings.MonitoringAccountDomainUnavailable(info.MailboxDatabaseName));
			}
			string text = info.MonitoringAccount + "@" + info.MonitoringAccountDomain;
			if (!SmtpAddress.IsValidSmtpAddress(text))
			{
				throw new InvalidMailboxDatabaseEndpointException(Strings.MonitoringAccountImproper(info.MailboxDatabaseName, text));
			}
			if (info.SystemMailboxGuid == Guid.Empty)
			{
				throw new InvalidMailboxDatabaseEndpointException(Strings.InvalidSystemMailbox(info.MailboxDatabaseName));
			}
		}

		protected string GetStoredMonitoringMailboxPassword(ADUser monitoringMailbox, TracingContext traceContext)
		{
			WTFDiagnostics.TraceFunction<ADUser>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, traceContext, "Retrieving monitoring mailbox {0} password", monitoringMailbox, null, "GetStoredMonitoringMailboxPassword", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseEndpointDelegate.cs", 146);
			return LocalMonitoringMailboxManagement.GetStaticPassword(traceContext);
		}

		protected LiveIdAuthResult AuthenticateTestForDataCenterMailbox(string userPrincipalName, string credential, TracingContext traceContext, out string authError)
		{
			authError = null;
			if (string.IsNullOrEmpty(credential))
			{
				return LiveIdAuthResult.InvalidCreds;
			}
			if (!MailboxDatabaseEndpointDelegate.RunningInDatacenter)
			{
				return LiveIdAuthResult.Success;
			}
			LiveIdAuthResult liveIdAuthResult = LiveIdAuthResult.AuthFailure;
			byte[] bytes = Encoding.Default.GetBytes(userPrincipalName);
			byte[] bytes2 = Encoding.Default.GetBytes(credential);
			for (int i = 0; i < 5; i++)
			{
				try
				{
					LiveIdBasicAuthentication liveIdBasicAuthentication = new LiveIdBasicAuthentication();
					liveIdBasicAuthentication.ApplicationName = "M.E.Monitoring.ActiveMonitoring.Common.DirectoryAccessor";
					liveIdBasicAuthentication.UserIpAddress = null;
					liveIdBasicAuthentication.UserAgent = "MailboxAlreadyHasStaticPassword";
					liveIdBasicAuthentication.SyncAD = true;
					liveIdBasicAuthentication.SyncADBackEndOnly = true;
					IAsyncResult asyncResult = liveIdBasicAuthentication.BeginGetCommonAccessToken(bytes, bytes2, null, Guid.Empty, null, null);
					LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
					if (lazyAsyncResult != null)
					{
						lazyAsyncResult.InternalWaitForCompletion();
					}
					else
					{
						asyncResult.AsyncWaitHandle.WaitOne();
						asyncResult.AsyncWaitHandle.Close();
					}
					string arg;
					liveIdAuthResult = liveIdBasicAuthentication.EndGetCommonAccessToken(asyncResult, out arg);
					if (liveIdAuthResult != LiveIdAuthResult.Success)
					{
						authError = liveIdBasicAuthentication.LastRequestErrorMessage;
					}
					WTFDiagnostics.TraceInformation<string, string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, traceContext, "[MailboxDatabaseEndpointDelegate.TestMonitoringMailboxCredential] LiveIdAuthResult: {0}; serializedCat: {1}", liveIdAuthResult.ToString(), arg, null, "AuthenticateTestForDataCenterMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseEndpointDelegate.cs", 230);
					switch (liveIdAuthResult)
					{
					case LiveIdAuthResult.UserNotFoundInAD:
					case LiveIdAuthResult.LiveServerUnreachable:
					case LiveIdAuthResult.FederatedStsUnreachable:
					case LiveIdAuthResult.OperationTimedOut:
					case LiveIdAuthResult.CommunicationFailure:
					case LiveIdAuthResult.AuthFailure:
						Thread.Sleep(TimeSpan.FromSeconds(5.0));
						break;
					default:
						return liveIdAuthResult;
					}
				}
				catch (CommunicationObjectFaultedException)
				{
					Thread.Sleep(TimeSpan.FromSeconds(5.0));
				}
				catch (Exception ex)
				{
					WTFDiagnostics.TraceError<string>(Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.CommonComponentsTracer, traceContext, "[MailboxDatabaseEndpointDelegate.TestMonitoringMailboxCredential] Failed with exception: {0}", ex.ToString(), null, "AuthenticateTestForDataCenterMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Discovery\\MailboxDatabaseEndpointDelegate.cs", 277);
					break;
				}
			}
			return liveIdAuthResult;
		}

		protected static readonly bool RunningInDatacenter = VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.DirectoryAccessor.Enabled;
	}
}
