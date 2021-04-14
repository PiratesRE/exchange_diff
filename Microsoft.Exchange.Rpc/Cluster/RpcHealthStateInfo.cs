using System;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Serializable]
	internal sealed class RpcHealthStateInfo
	{
		public RpcHealthStateInfo(string componentName, int priority, string resultName, string dbName, int healthStatus, DateTime lastReportedTime)
		{
			this.m_componentName = componentName;
			this.m_priority = priority;
			this.m_resultName = resultName;
			this.m_databaseName = dbName;
			this.m_healthStatus = healthStatus;
			this.m_lastReportedTime = lastReportedTime;
		}

		public RpcHealthStateInfo()
		{
		}

		public string ComponentName
		{
			get
			{
				return this.m_componentName;
			}
			set
			{
				this.m_componentName = value;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.m_databaseName;
			}
			set
			{
				this.m_databaseName = value;
			}
		}

		public string ResultName
		{
			get
			{
				return this.m_resultName;
			}
			set
			{
				this.m_resultName = value;
			}
		}

		public int Priority
		{
			get
			{
				return this.m_priority;
			}
			set
			{
				this.m_priority = value;
			}
		}

		public int HealthStatus
		{
			get
			{
				return this.m_healthStatus;
			}
			set
			{
				this.m_healthStatus = value;
			}
		}

		public DateTime LastReportedTime
		{
			get
			{
				return this.m_lastReportedTime;
			}
			set
			{
				this.m_lastReportedTime = value;
			}
		}

		private string m_componentName;

		private string m_resultName;

		private int m_healthStatus;

		private int m_priority;

		private DateTime m_lastReportedTime;

		private string m_databaseName;
	}
}
