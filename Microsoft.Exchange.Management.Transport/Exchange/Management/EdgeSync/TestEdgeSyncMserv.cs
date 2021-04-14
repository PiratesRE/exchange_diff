using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Mserve;
using Microsoft.Exchange.EdgeSync.Validation.Mserv;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.Mserve;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[Cmdlet("Test", "EdgeSyncMserv", DefaultParameterSetName = "Health", SupportsShouldProcess = true)]
	public sealed class TestEdgeSyncMserv : TestEdgeSyncBase
	{
		[Parameter(Mandatory = false, ParameterSetName = "ValidateAddress")]
		public SmtpAddress[] EmailAddresses
		{
			get
			{
				return (SmtpAddress[])base.Fields["EmailAddresses"];
			}
			set
			{
				base.Fields["EmailAddresses"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "ValidateAddress")]
		public MailboxIdParameter[] Mailboxes
		{
			get
			{
				return (MailboxIdParameter[])base.Fields["Mailboxes"];
			}
			set
			{
				base.Fields["Mailboxes"] = value;
			}
		}

		protected override string CmdletMonitoringEventSource
		{
			get
			{
				return "MSExchange Monitoring EdgeSyncMserv";
			}
		}

		protected override string Service
		{
			get
			{
				return "Hotmail MSERV";
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestEdgeSyncMserv;
			}
		}

		internal override bool ReadConnectorLeasePath(IConfigurationSession session, ADObjectId rootId, out string primaryLeasePath, out string backupLeasePath, out bool hasOneConnectorEnabledInCurrentForest)
		{
			string text;
			backupLeasePath = (text = null);
			primaryLeasePath = text;
			hasOneConnectorEnabledInCurrentForest = false;
			EdgeSyncMservConnector edgeSyncMservConnector = base.FindSiteEdgeSyncConnector<EdgeSyncMservConnector>(session, rootId, out hasOneConnectorEnabledInCurrentForest);
			if (edgeSyncMservConnector == null)
			{
				return false;
			}
			primaryLeasePath = Path.Combine(edgeSyncMservConnector.PrimaryLeaseLocation, "mserv.lease");
			backupLeasePath = Path.Combine(edgeSyncMservConnector.BackupLeaseLocation, "mserv.lease");
			return true;
		}

		internal override ADObjectId GetCookieContainerId(IConfigurationSession session)
		{
			return MserveTargetConnection.GetCookieContainerId(session);
		}

		protected override EnhancedTimeSpan GetSyncInterval(EdgeSyncServiceConfig config)
		{
			return config.RecipientSyncInterval;
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				if (this.EmailAddresses != null || this.Mailboxes != null)
				{
					if (this.EmailAddresses != null)
					{
						this.PerformLookup(base.DomainController, this.EmailAddresses);
					}
					if (this.Mailboxes != null)
					{
						this.PerformLookup(base.DomainController, this.Mailboxes);
					}
					return;
				}
			}
			catch (InvalidOperationException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
			}
			catch (MserveException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ReadError, null);
			}
			catch (TransientException exception3)
			{
				base.WriteError(exception3, ErrorCategory.ReadError, null);
			}
			catch (ADOperationException exception4)
			{
				base.WriteError(exception4, ErrorCategory.ReadError, null);
			}
			base.TestGeneralSyncHealth();
		}

		private void PerformLookup(string domainController, SmtpAddress[] addresses)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < addresses.Length; i++)
			{
				string item = (string)addresses[i];
				list.Add(item);
			}
			this.PerformLookup(domainController, list);
		}

		private void PerformLookup(string domainController, MailboxIdParameter[] mailboxes)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 234, "PerformLookup", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\TestEdgeSyncMserv.cs");
			List<string> list = new List<string>();
			foreach (MailboxIdParameter mailboxIdParameter in mailboxes)
			{
				IEnumerable<ADRecipient> objects = mailboxIdParameter.GetObjects<ADRecipient>(null, tenantOrRootOrgRecipientSession);
				using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						foreach (string item in enumerator.Current.EmailAddresses.ToStringArray())
						{
							list.Add(item);
						}
					}
				}
			}
			this.PerformLookup(domainController, list);
		}

		private void PerformLookup(string domainController, List<string> addresses)
		{
			MserveWebService mserveWebService = EdgeSyncMservConnector.CreateDefaultMserveWebService(domainController);
			if (mserveWebService == null)
			{
				throw new InvalidOperationException("Invalid MServ configuration.");
			}
			List<RecipientSyncOperation> list;
			foreach (string text in addresses)
			{
				RecipientSyncOperation recipientSyncOperation = new RecipientSyncOperation();
				if (text.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase))
				{
					recipientSyncOperation.ReadEntries.Add(text.Substring(5));
				}
				else
				{
					recipientSyncOperation.ReadEntries.Add(text);
				}
				list = mserveWebService.Synchronize(recipientSyncOperation);
				foreach (RecipientSyncOperation recipientSyncOperation2 in list)
				{
					base.WriteObject(new MservRecipientRecord(recipientSyncOperation2.ReadEntries[0], recipientSyncOperation2.PartnerId));
				}
			}
			list = mserveWebService.Synchronize();
			foreach (RecipientSyncOperation recipientSyncOperation3 in list)
			{
				base.WriteObject(new MservRecipientRecord(recipientSyncOperation3.ReadEntries[0], recipientSyncOperation3.PartnerId));
			}
		}

		private const string CmdletNoun = "EdgeSyncMserv";

		private const string ServiceName = "Hotmail MSERV";
	}
}
