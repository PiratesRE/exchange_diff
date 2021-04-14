using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.MailTips;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class MailTipsState
	{
		public MailTipsState(RecipientInfo[] recipientsInfo, RecipientInfo senderInfo, bool doesNeedConfig, string logonUserLegDn, string logonUserPrimarySmtpAddress, ClientSecurityContext clientSecurityContext, ExTimeZone logonUserTimeZone, CultureInfo logonUserCulture, OrganizationId logonUserOrgId, ADObjectId queryBaseDn, bool shouldHideByDefault, PendingRequestManager pendingRequestManager, string serverName, string weekdayDateTimeFormat)
		{
			this.RecipientsInfo = recipientsInfo;
			this.SenderInfo = senderInfo;
			this.DoesNeedConfig = doesNeedConfig;
			this.LogonUserLegDn = logonUserLegDn;
			this.LogonUserPrimarySmtpAddress = logonUserPrimarySmtpAddress;
			this.ClientSecurityContext = clientSecurityContext;
			this.LogonUserTimeZone = (logonUserTimeZone ?? (ExTimeZone.CurrentTimeZone ?? ExTimeZone.UtcTimeZone));
			this.LogonUserCulture = logonUserCulture;
			this.LogonUserOrgId = logonUserOrgId;
			this.QueryBaseDn = queryBaseDn;
			this.ShouldHideByDefault = shouldHideByDefault;
			this.PendingRequestManager = pendingRequestManager;
			this.ServerName = serverName;
			this.MailTipsResult = new List<MailTips>(this.RecipientsInfo.Length);
			this.WeekdayDateTimeFormat = weekdayDateTimeFormat;
		}

		public RecipientInfo[] RecipientsInfo { get; private set; }

		public RecipientInfo SenderInfo { get; private set; }

		public bool DoesNeedConfig { get; private set; }

		public IBudget Budget { get; internal set; }

		public CachedOrganizationConfiguration CachedOrganizationConfiguration { get; internal set; }

		public string LogonUserLegDn { get; private set; }

		public string LogonUserPrimarySmtpAddress { get; private set; }

		public ClientSecurityContext ClientSecurityContext { get; private set; }

		public ExTimeZone LogonUserTimeZone { get; private set; }

		public CultureInfo LogonUserCulture { get; private set; }

		public OrganizationId LogonUserOrgId { get; private set; }

		public ProxyAddress SendingAs { get; internal set; }

		public List<MailTips> MailTipsResult { get; private set; }

		public bool ShouldHideByDefault { get; private set; }

		public PendingRequestManager PendingRequestManager { get; private set; }

		public string ServerName { get; private set; }

		public ADObjectId QueryBaseDn { get; private set; }

		public string WeekdayDateTimeFormat { get; private set; }

		internal RequestLogger RequestLogger { get; set; }

		internal GetMailTipsQuery GetMailTipsQuery { get; set; }

		public int GetEstimatedStringLength()
		{
			int num = 0;
			if (this.RecipientsInfo != null)
			{
				num = 100 * this.RecipientsInfo.Length;
			}
			return 450 + 100 * num;
		}

		public override string ToString()
		{
			int estimatedStringLength = this.GetEstimatedStringLength();
			StringBuilder stringBuilder = new StringBuilder(estimatedStringLength);
			if (this.SenderInfo != null)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Sender: {0}, ", new object[]
				{
					this.SenderInfo.ToProxyAddress()
				});
			}
			if (this.RecipientsInfo != null)
			{
				stringBuilder.Append("Recipients: ");
				foreach (RecipientInfo recipientInfo in this.RecipientsInfo)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}, ", new object[]
					{
						recipientInfo.ToProxyAddress()
					});
				}
			}
			if (!string.IsNullOrEmpty(this.LogonUserLegDn))
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "LogonUser: {0}, ", new object[]
				{
					this.LogonUserLegDn
				});
			}
			if (!string.IsNullOrEmpty(this.ServerName))
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "ServerName: {0}", new object[]
				{
					this.ServerName
				});
			}
			return stringBuilder.ToString();
		}
	}
}
