using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics.Components.ObjectModel;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[Serializable]
	public class DataSourceInfo
	{
		public DataSourceInfo()
		{
			ExTraceGlobals.DataSourceInfoTracer.Information((long)this.GetHashCode(), "DataSourceInfo::DataSourceInfo - initializing data source info.");
		}

		public DataSourceInfo(string path)
		{
			ExTraceGlobals.DataSourceInfoTracer.Information((long)this.GetHashCode(), "DataSourceInfo::DataSourceInfo - initializing data source info.");
		}

		public DataSourceInfo(DataSourceInfo template)
		{
			ExTraceGlobals.DataSourceInfoTracer.Information((long)this.GetHashCode(), "DataSourceInfo::DataSourceInfo - initializing data source info from template.");
			if (template != null)
			{
				this.managementServer = template.ManagementServer;
				this.connectionString = template.ConnectionString;
				this.userName = template.UserName;
			}
		}

		public static string DefaultManagementServer
		{
			get
			{
				return DataSourceInfo.defaultManagementServer;
			}
			set
			{
				DataSourceInfo.defaultManagementServer = value;
			}
		}

		public string ConnectionString
		{
			get
			{
				return this.connectionString;
			}
		}

		public string ManagementServer
		{
			get
			{
				return this.managementServer;
			}
			set
			{
				ExTraceGlobals.DataSourceInfoTracer.Information<string>((long)this.GetHashCode(), "DataSourceInfo::ManagementServer - setting ManagementServer to {0}.", (this.managementServer == null) ? "null" : this.ManagementServer);
				this.managementServer = value;
				this.OnInstanceDataChanged();
			}
		}

		public string ManagementEndpointUrl
		{
			get
			{
				if (this.managementServer != null)
				{
					return "tcp://" + this.managementServer + ":8085/ExchangeAdministration";
				}
				return null;
			}
		}

		public string UserName
		{
			get
			{
				if (this.userName == null)
				{
					using (WindowsIdentity current = WindowsIdentity.GetCurrent())
					{
						this.userName = current.Name;
					}
				}
				return this.userName;
			}
		}

		protected string ConnectionStringInternal
		{
			get
			{
				return this.connectionString;
			}
			set
			{
				this.connectionString = value;
			}
		}

		public DataSourceInfo Duplicate()
		{
			return (DataSourceInfo)base.MemberwiseClone();
		}

		protected virtual void OnInstanceDataChanged()
		{
		}

		private static string defaultManagementServer = "localhost";

		private string connectionString;

		private string managementServer = DataSourceInfo.DefaultManagementServer;

		private string userName;
	}
}
