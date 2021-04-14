using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Calendar.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.AvailabilityService.Probes
{
	public class AvailabilityServiceLocalProbe : AvailabilityServiceProbe
	{
		protected override string ComponentId
		{
			get
			{
				return "AvailabilityService_LAM_Probe";
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Initialize(ExTraceGlobals.AvailabilityServiceTracer);
			ExchangeServerRoleEndpoint exchangeServerRoleEndpoint = LocalEndpointManager.Instance.ExchangeServerRoleEndpoint;
			if (!ExEnvironment.IsTest)
			{
				this.DoWorkOffBoxRequest(cancellationToken);
				return;
			}
			base.DoWorkInternal(cancellationToken);
		}

		protected override void UpdateProbeResultAttributes()
		{
			base.UpdateProbeResultAttributes();
			if (base.IsProbeFailed)
			{
				this.UpdateTargetBEServerName();
				base.Result.StateAttribute14 = this.targetBEServer;
			}
		}

		protected override void ThrowProbeError()
		{
			if (!base.IsProbeFailed)
			{
				return;
			}
			Server exchangeServerByName = DirectoryAccessor.Instance.GetExchangeServerByName(this.targetBEServer);
			if (DirectoryAccessor.Instance.IsMonitoringOffline(exchangeServerByName))
			{
				this.probeErrorCode = "Server In Maintenance";
				base.Result.FailureCategory = (int)AvailabilityServiceProbeUtil.KnownErrors[this.probeErrorCode];
				base.Result.StateAttribute1 = ((AvailabilityServiceProbeUtil.FailingComponent)base.Result.FailureCategory).ToString();
				base.Result.StateAttribute2 = this.probeErrorCode;
				return;
			}
			throw new AvailabilityServiceValidationException(base.ProbeErrorMessage);
		}

		private void DoWorkOffBoxRequest(CancellationToken cancellationToken)
		{
			MailboxDatabaseInfo activeDatabaseCopy = this.GetActiveDatabaseCopy();
			if (activeDatabaseCopy == null)
			{
				base.LogTrace("At least one active database will be required to make off-box requests. Skip the run.");
				this.probeErrorCode = "No Active Copy Database On Mailbox";
				base.Result.FailureCategory = (int)AvailabilityServiceProbeUtil.KnownErrors[this.probeErrorCode];
				base.Result.StateAttribute1 = ((AvailabilityServiceProbeUtil.FailingComponent)base.Result.FailureCategory).ToString();
				base.Result.StateAttribute2 = this.probeErrorCode;
				return;
			}
			base.LogTrace("Override monitoring account for requester with the one from GetActiveDatabaseCopy().");
			base.Definition.Account = activeDatabaseCopy.MonitoringAccount + "@" + activeDatabaseCopy.MonitoringAccountDomain;
			base.Definition.AccountPassword = activeDatabaseCopy.MonitoringAccountPassword;
			base.Definition.AccountDisplayName = activeDatabaseCopy.MonitoringAccount;
			base.Result.StateAttribute23 = "onenote:///\\\\exstore\\files\\userfiles\\gisellid\\Supportability%20for%20Calendar\\Availability%20Service.one#Battle%20Card%20E15%20Availability%20Service%20Probe&section-id={9C4DDCB7-B82B-4D3D-AD32-000FBD7878F4}&page-id={205CE2D5-D729-41E0-B7EA-DB95A3B5B4F8}&end";
			base.DoWorkInternal(cancellationToken);
		}

		private MailboxDatabaseInfo GetActiveDatabaseCopy()
		{
			ICollection<MailboxDatabaseInfo> mailboxDatabaseInfoCollectionForBackend = LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
			if (mailboxDatabaseInfoCollectionForBackend != null && mailboxDatabaseInfoCollectionForBackend.Count > 0)
			{
				int num = 0;
				int num2 = AvailabilityServiceLocalProbe.randomNumberGenerator.Next(mailboxDatabaseInfoCollectionForBackend.Count);
				MailboxDatabaseInfo mailboxDatabaseInfo;
				for (;;)
				{
					mailboxDatabaseInfo = mailboxDatabaseInfoCollectionForBackend.ElementAt(num2);
					Guid mailboxDatabaseGuid = mailboxDatabaseInfo.MailboxDatabaseGuid;
					if (DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(mailboxDatabaseInfo.MailboxDatabaseGuid))
					{
						break;
					}
					if (++num2 == mailboxDatabaseInfoCollectionForBackend.Count)
					{
						num2 = 0;
					}
					if (++num >= mailboxDatabaseInfoCollectionForBackend.Count)
					{
						goto IL_6E;
					}
				}
				return mailboxDatabaseInfo;
			}
			IL_6E:
			return null;
		}

		private void UpdateTargetBEServerName()
		{
			if (ExEnvironment.IsTest && !string.IsNullOrEmpty(base.Result.StateAttribute13))
			{
				this.targetBEServer = base.Result.StateAttribute13;
				return;
			}
			if (base.Definition.Attributes.ContainsKey("DatabaseGuid"))
			{
				string text = base.Definition.Attributes["DatabaseGuid"];
				if (!string.IsNullOrEmpty(text))
				{
					this.targetBEServer = DirectoryAccessor.Instance.GetDatabaseActiveHost(new Guid(text));
				}
			}
		}

		private string targetBEServer = string.Empty;

		private static Random randomNumberGenerator = new Random();
	}
}
