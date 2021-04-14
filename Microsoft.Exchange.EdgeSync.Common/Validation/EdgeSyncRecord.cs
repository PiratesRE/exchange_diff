using System;
using System.Text;
using Microsoft.Exchange.EdgeSync.Common.Internal;
using Microsoft.Exchange.MessageSecurity.EdgeSync;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	[Serializable]
	public sealed class EdgeSyncRecord
	{
		private EdgeSyncRecord(string service, string context, ValidationStatus status, string detail, LeaseToken leaseToken, Cookie cookie, string additionalInfo)
		{
			this.status = status;
			this.detail = detail;
			this.alertTime = leaseToken.AlertTime;
			this.leaseHolder = leaseToken.Path;
			this.leaseType = leaseToken.Type;
			this.leaseExpiry = leaseToken.Expiry;
			this.lastSynchronized = leaseToken.LastSync;
			this.cookie = cookie;
			this.now = DateTime.UtcNow;
			this.additionalInfo = (string.IsNullOrEmpty(additionalInfo) ? "N/A" : additionalInfo);
			this.service = service;
			this.context = context;
		}

		public string AdditionalInfo
		{
			get
			{
				return this.additionalInfo;
			}
		}

		public string Detail
		{
			get
			{
				return this.detail;
			}
		}

		public DateTime CurrentTime
		{
			get
			{
				return this.now;
			}
		}

		public ValidationStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		public string StatusSummary
		{
			get
			{
				string result = string.Empty;
				switch (this.status)
				{
				case ValidationStatus.NoSyncConfigured:
					result = Strings.EdgeSyncNotConfigured(this.service);
					break;
				case ValidationStatus.Normal:
					result = Strings.EdgeSyncNormal(this.service);
					break;
				case ValidationStatus.Warning:
					result = Strings.EdgeSyncAbnormal(this.service, this.context);
					break;
				case ValidationStatus.Failed:
					result = Strings.EdgeSyncFailed(this.service, this.context);
					break;
				case ValidationStatus.Inconclusive:
					result = Strings.EdgeSyncInconclusive(this.service, this.context);
					break;
				case ValidationStatus.FailedUrgent:
					result = Strings.EdgeSyncFailedUrgent(this.service, this.context);
					break;
				}
				return result;
			}
		}

		public string LeaseHolder
		{
			get
			{
				return this.leaseHolder;
			}
		}

		public LeaseTokenType LeaseType
		{
			get
			{
				return this.leaseType;
			}
		}

		public DateTime LeaseExpiry
		{
			get
			{
				return this.leaseExpiry;
			}
		}

		public DateTime AlertTime
		{
			get
			{
				return this.alertTime;
			}
		}

		public DateTime LastSynchronized
		{
			get
			{
				return this.lastSynchronized;
			}
			set
			{
				this.lastSynchronized = value;
			}
		}

		public string CookieDomainController
		{
			get
			{
				if (this.cookie == null)
				{
					return string.Empty;
				}
				return this.cookie.DomainController;
			}
		}

		public DateTime CookieLastUpdated
		{
			get
			{
				if (this.cookie == null)
				{
					return DateTime.MinValue;
				}
				return this.cookie.LastUpdated;
			}
		}

		public string CookieBaseDN
		{
			get
			{
				if (this.cookie == null)
				{
					return string.Empty;
				}
				return this.cookie.BaseDN;
			}
		}

		public int CookieLength
		{
			get
			{
				if (this.cookie == null || this.cookie.CookieValue == null)
				{
					return 0;
				}
				return this.cookie.CookieValue.Length;
			}
		}

		public static EdgeSyncRecord GetEdgeSyncConnectorNotConfiguredForEntireForestRecord(string service)
		{
			return new EdgeSyncRecord(service, null, ValidationStatus.NoSyncConfigured, "No EdgeSync connector has been configured or enabled for the entire forest", LeaseToken.Empty, null, null);
		}

		public static EdgeSyncRecord GetEdgeSyncConnectorNotConfiguredForCurrentSiteRecord(string service, string siteName)
		{
			return new EdgeSyncRecord(service, null, ValidationStatus.Normal, "No EdgeSync connector has been configured or enabled for the current site " + siteName, LeaseToken.Empty, null, null);
		}

		public static EdgeSyncRecord GetEdgeSyncServiceNotConfiguredForCurrentSiteRecord(string service, string siteName)
		{
			return new EdgeSyncRecord(service, null, ValidationStatus.NoSyncConfigured, "No EdgeSync service config has been configured for the current site " + siteName, LeaseToken.Empty, null, null);
		}

		public static EdgeSyncRecord GetNormalRecord(string service, string detail, LeaseToken leaseToken, Cookie cookie, string additionalInfo)
		{
			return new EdgeSyncRecord(service, null, ValidationStatus.Normal, detail, leaseToken, cookie, additionalInfo);
		}

		public static EdgeSyncRecord GetFailedRecord(string service, string context, string detail, LeaseToken leaseToken, Cookie cookie, string additionalInfo)
		{
			return EdgeSyncRecord.GetFailedRecord(service, context, detail, leaseToken, cookie, additionalInfo, false);
		}

		public static EdgeSyncRecord GetFailedRecord(string service, string context, string detail, LeaseToken leaseToken, Cookie cookie, string additionalInfo, bool isUrgent)
		{
			return new EdgeSyncRecord(service, context, isUrgent ? ValidationStatus.FailedUrgent : ValidationStatus.Failed, detail, leaseToken, cookie, additionalInfo);
		}

		public static EdgeSyncRecord GetInconclusiveRecord(string service, string context, string detail, LeaseToken leaseToken, Cookie cookie, string additionalInfo)
		{
			return new EdgeSyncRecord(service, context, ValidationStatus.Inconclusive, detail, leaseToken, cookie, additionalInfo);
		}

		public static EdgeSyncRecord GetWarningRecord(string service, string context, string detail, LeaseToken leaseToken, Cookie cookie)
		{
			return new EdgeSyncRecord(service, context, ValidationStatus.Warning, detail, leaseToken, cookie, null);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CurrentTime:");
			stringBuilder.AppendLine(this.now.ToString());
			stringBuilder.Append("Status:");
			stringBuilder.AppendLine(this.status.ToString());
			stringBuilder.Append("StatusSummary:");
			stringBuilder.AppendLine(this.StatusSummary);
			stringBuilder.Append("Detail:");
			stringBuilder.AppendLine(this.detail);
			stringBuilder.Append("LeaseHolder:");
			stringBuilder.AppendLine(this.leaseHolder);
			stringBuilder.Append("LeaseType:");
			stringBuilder.AppendLine(this.leaseType.ToString());
			stringBuilder.Append("LeaseExpiry:");
			stringBuilder.AppendLine(this.leaseExpiry.ToString());
			stringBuilder.Append("LastSynchronized:");
			stringBuilder.AppendLine(this.lastSynchronized.ToString());
			stringBuilder.Append("CookieBaseDN:");
			stringBuilder.AppendLine(this.CookieBaseDN);
			stringBuilder.Append("CookieDomainController:");
			stringBuilder.AppendLine(this.CookieDomainController);
			stringBuilder.Append("CookieLastUpdated:");
			stringBuilder.AppendLine(this.CookieLastUpdated.ToString());
			stringBuilder.Append("CookieLength:");
			stringBuilder.AppendLine(this.CookieLength.ToString());
			stringBuilder.Append("AdditionalInfo:");
			stringBuilder.AppendLine(this.additionalInfo.ToString());
			return stringBuilder.ToString();
		}

		private string additionalInfo;

		private string detail;

		private ValidationStatus status;

		private string leaseHolder;

		private DateTime leaseExpiry;

		private DateTime alertTime;

		private LeaseTokenType leaseType;

		private DateTime lastSynchronized;

		private DateTime now;

		private Cookie cookie;

		private string service;

		private string context;
	}
}
