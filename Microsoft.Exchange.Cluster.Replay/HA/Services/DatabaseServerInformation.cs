using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HA.Services
{
	[DataContract(Name = "DatabaseServerInformation", Namespace = "http://www.outlook.com/highavailability/ServerLocator/v1/")]
	public class DatabaseServerInformation
	{
		[DataMember(Name = "DatabaseGuid", IsRequired = false, Order = 0)]
		public Guid DatabaseGuid
		{
			get
			{
				return this.m_databaseGuid;
			}
			set
			{
				this.m_databaseGuid = value;
			}
		}

		[DataMember(Name = "ServerFqdn", IsRequired = false, Order = 1)]
		public string ServerFqdn
		{
			get
			{
				return this.m_serverFqdn;
			}
			set
			{
				this.m_serverFqdn = value;
			}
		}

		[DataMember(Name = "RequestSentUtc", IsRequired = false, Order = 2)]
		public DateTime RequestSentUtc
		{
			get
			{
				return this.m_requestSentUtc;
			}
			set
			{
				this.m_requestSentUtc = value;
			}
		}

		[DataMember(Name = "RequestReceivedUtc", IsRequired = false, Order = 3)]
		public DateTime RequestReceivedUtc
		{
			get
			{
				return this.m_requestReceivedUtc;
			}
			set
			{
				this.m_requestReceivedUtc = value;
			}
		}

		[DataMember(Name = "ReplySentUtc", IsRequired = false, Order = 4)]
		public DateTime ReplySentUtc
		{
			get
			{
				return this.m_replySentUtc;
			}
			set
			{
				this.m_replySentUtc = value;
			}
		}

		[DataMember(Name = "ServerVersion", IsRequired = false, Order = 5)]
		public int ServerVersion
		{
			get
			{
				return this.m_serverVersion;
			}
			set
			{
				this.m_serverVersion = value;
			}
		}

		[DataMember(Name = "MountedTimeUtc", IsRequired = false, Order = 6)]
		public DateTime MountedTimeUtc
		{
			get
			{
				return this.m_mountedTimeUtc;
			}
			set
			{
				this.m_mountedTimeUtc = value;
			}
		}

		[DataMember(Name = "LastMountedServerFqdn", IsRequired = false, Order = 7)]
		public string LastMountedServerFqdn
		{
			get
			{
				return this.m_lastMountedServerFqdn;
			}
			set
			{
				this.m_lastMountedServerFqdn = value;
			}
		}

		[DataMember(Name = "FailoverSequenceNumber", IsRequired = false, Order = 8)]
		public long FailoverSequenceNumber
		{
			get
			{
				return this.m_failoverSequenceNumber;
			}
			set
			{
				this.m_failoverSequenceNumber = value;
			}
		}

		private Guid m_databaseGuid;

		private string m_serverFqdn;

		private DateTime m_requestSentUtc;

		private DateTime m_requestReceivedUtc;

		private DateTime m_replySentUtc;

		private int m_serverVersion;

		private DateTime m_mountedTimeUtc;

		private string m_lastMountedServerFqdn;

		private long m_failoverSequenceNumber;
	}
}
