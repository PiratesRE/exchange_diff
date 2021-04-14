using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.MessageSecurity.EdgeSync;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	[Serializable]
	public class EdgeSubscriptionStatus
	{
		public EdgeSubscriptionStatus(string name)
		{
			this.syncStatus = ValidationStatus.Inconclusive;
			this.utcNow = DateTime.UtcNow;
			this.name = name;
			this.leaseType = LeaseTokenType.None;
			this.TransportServerStatus = new EdgeConfigStatus();
			this.TransportConfigStatus = new EdgeConfigStatus();
			this.AcceptedDomainStatus = new EdgeConfigStatus();
			this.RemoteDomainStatus = new EdgeConfigStatus();
			this.SendConnectorStatus = new EdgeConfigStatus();
			this.MessageClassificationStatus = new EdgeConfigStatus();
			this.RecipientStatus = new EdgeConfigStatus();
			this.credentialRecords = new CredentialRecords();
			this.cookieRecords = new CookieRecords();
		}

		public ValidationStatus SyncStatus
		{
			get
			{
				return this.syncStatus;
			}
			set
			{
				this.syncStatus = value;
			}
		}

		public DateTime UtcNow
		{
			get
			{
				return this.utcNow;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string LeaseHolder
		{
			get
			{
				return this.leaseHolder;
			}
			set
			{
				this.leaseHolder = value;
			}
		}

		public LeaseTokenType LeaseType
		{
			get
			{
				return this.leaseType;
			}
			set
			{
				this.leaseType = value;
			}
		}

		public string FailureDetail
		{
			get
			{
				return this.failureDetail;
			}
			set
			{
				this.failureDetail = value;
			}
		}

		public DateTime LeaseExpiryUtc
		{
			get
			{
				return this.leaseExpiryUtc;
			}
			set
			{
				this.leaseExpiryUtc = value;
			}
		}

		public DateTime LastSynchronizedUtc
		{
			get
			{
				return this.lastSynchronizedUtc;
			}
			set
			{
				this.lastSynchronizedUtc = value;
			}
		}

		public EdgeConfigStatus TransportServerStatus
		{
			get
			{
				return this.transportServerStatus;
			}
			set
			{
				this.transportServerStatus = value;
			}
		}

		public EdgeConfigStatus TransportConfigStatus
		{
			get
			{
				return this.transportConfigStatus;
			}
			set
			{
				this.transportConfigStatus = value;
			}
		}

		public EdgeConfigStatus AcceptedDomainStatus
		{
			get
			{
				return this.acceptedDomainStatus;
			}
			set
			{
				this.acceptedDomainStatus = value;
			}
		}

		public EdgeConfigStatus RemoteDomainStatus
		{
			get
			{
				return this.remoteDomainStatus;
			}
			set
			{
				this.remoteDomainStatus = value;
			}
		}

		public EdgeConfigStatus SendConnectorStatus
		{
			get
			{
				return this.sendConnectorStatus;
			}
			set
			{
				this.sendConnectorStatus = value;
			}
		}

		public EdgeConfigStatus MessageClassificationStatus
		{
			get
			{
				return this.messageClassificationStatus;
			}
			set
			{
				this.messageClassificationStatus = value;
			}
		}

		public EdgeConfigStatus RecipientStatus
		{
			get
			{
				return this.recipientStatus;
			}
			set
			{
				this.recipientStatus = value;
			}
		}

		public CredentialRecords CredentialRecords
		{
			get
			{
				return this.credentialRecords;
			}
			set
			{
				this.credentialRecords = value;
			}
		}

		public CookieRecords CookieRecords
		{
			get
			{
				return this.cookieRecords;
			}
			set
			{
				this.cookieRecords = value;
			}
		}

		public string ToStringForm()
		{
			StringBuilder stringBuilder = new StringBuilder(500);
			stringBuilder.AppendLine("******************************");
			stringBuilder.Append("CurrentTime (UTC):");
			stringBuilder.AppendLine(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append("Name:");
			stringBuilder.AppendLine(this.name);
			stringBuilder.Append("SyncStatus:");
			stringBuilder.AppendLine(this.syncStatus.ToString());
			stringBuilder.Append("LeaseHolder:");
			stringBuilder.AppendLine(this.leaseHolder);
			stringBuilder.Append("LeaseType:");
			stringBuilder.AppendLine(this.leaseType.ToString());
			stringBuilder.Append("LeaseExpiry (UTC):");
			stringBuilder.AppendLine(this.leaseExpiryUtc.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append("LastSynchronized (UTC):");
			stringBuilder.AppendLine(this.lastSynchronizedUtc.ToString(CultureInfo.InvariantCulture));
			stringBuilder.AppendLine("Cookie Records (" + this.cookieRecords.Records.Count + "):");
			foreach (CookieRecord cookieRecord in this.cookieRecords.Records)
			{
				string value = string.Format(CultureInfo.InvariantCulture, "Domain:{0}; LastUpdated (UTC):{1}; DC:{2}", new object[]
				{
					cookieRecord.BaseDN,
					cookieRecord.LastUpdated.ToString(CultureInfo.InvariantCulture),
					cookieRecord.DomainController
				});
				stringBuilder.AppendLine(value);
			}
			stringBuilder.Append("Failure Details:");
			stringBuilder.AppendLine(this.failureDetail);
			return stringBuilder.ToString();
		}

		private readonly DateTime utcNow;

		private ValidationStatus syncStatus;

		private string name;

		private string leaseHolder;

		private DateTime leaseExpiryUtc;

		private LeaseTokenType leaseType;

		private DateTime lastSynchronizedUtc;

		private string failureDetail;

		private EdgeConfigStatus transportServerStatus;

		private EdgeConfigStatus transportConfigStatus;

		private EdgeConfigStatus acceptedDomainStatus;

		private EdgeConfigStatus remoteDomainStatus;

		private EdgeConfigStatus sendConnectorStatus;

		private EdgeConfigStatus messageClassificationStatus;

		private EdgeConfigStatus recipientStatus;

		private CredentialRecords credentialRecords;

		private CookieRecords cookieRecords;
	}
}
