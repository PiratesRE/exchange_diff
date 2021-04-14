using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common
{
	[Serializable]
	public class MonitoringItemIdentity : ConfigurableObject
	{
		public MonitoringItemIdentity() : base(new SimpleProviderPropertyBag())
		{
		}

		internal MonitoringItemIdentity(string server, RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity rpcIdentity) : this()
		{
			this.Server = server;
			this.HealthSetName = rpcIdentity.HealthSetName;
			this.Name = rpcIdentity.Name;
			this.TargetResource = rpcIdentity.TargetResource;
			this.ItemType = this.ParseEnum<MonitorItemType>(rpcIdentity.ItemType, MonitorItemType.Unknown);
			this[SimpleProviderObjectSchema.Identity] = new MonitoringItemIdentity.MonitorIdentityId(this.HealthSetName, this.Name, this.TargetResource);
		}

		public string Server
		{
			get
			{
				return (string)this[MonitorIdentitySchema.Server];
			}
			private set
			{
				this[MonitorIdentitySchema.Server] = value;
			}
		}

		public string HealthSetName
		{
			get
			{
				return (string)this[MonitorIdentitySchema.HealthSetName];
			}
			private set
			{
				this[MonitorIdentitySchema.HealthSetName] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[MonitorIdentitySchema.Name];
			}
			private set
			{
				this[MonitorIdentitySchema.Name] = value;
			}
		}

		public string TargetResource
		{
			get
			{
				return (string)this[MonitorIdentitySchema.TargetResource];
			}
			private set
			{
				this[MonitorIdentitySchema.TargetResource] = value;
			}
		}

		public MonitorItemType ItemType
		{
			get
			{
				return (MonitorItemType)this[MonitorIdentitySchema.ItemType];
			}
			private set
			{
				this[MonitorIdentitySchema.ItemType] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MonitoringItemIdentity.schema;
			}
		}

		internal T ParseEnum<T>(string strEnum, T defaultValue) where T : struct
		{
			T result = defaultValue;
			if (!string.IsNullOrEmpty(strEnum) && !Enum.TryParse<T>(strEnum, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		private static MonitorIdentitySchema schema = ObjectSchema.GetInstance<MonitorIdentitySchema>();

		[Serializable]
		public class MonitorIdentityId : ObjectId
		{
			public MonitorIdentityId(string healthSetName, string monitorName, string targetResource)
			{
				this.identity = string.Format("{0}\\{1}{2}", healthSetName, monitorName, string.IsNullOrEmpty(targetResource) ? string.Empty : ("\\" + targetResource));
			}

			public static bool IsValidFormat(string monitorIdentity)
			{
				if (string.IsNullOrEmpty(monitorIdentity))
				{
					return false;
				}
				int num = monitorIdentity.Split(new char[]
				{
					'\\'
				}).Length;
				return num >= 2 && num <= 3;
			}

			public static string GetHealthSet(string monitorIdentity)
			{
				if (!MonitoringItemIdentity.MonitorIdentityId.IsValidFormat(monitorIdentity))
				{
					return null;
				}
				string[] array = monitorIdentity.Split(new char[]
				{
					'\\'
				});
				return array[0];
			}

			public static string GetMonitor(string monitorIdentity)
			{
				if (!MonitoringItemIdentity.MonitorIdentityId.IsValidFormat(monitorIdentity))
				{
					return null;
				}
				string[] array = monitorIdentity.Split(new char[]
				{
					'\\'
				});
				return array[1];
			}

			public static string GetTargetResource(string monitorIdentity)
			{
				if (!MonitoringItemIdentity.MonitorIdentityId.IsValidFormat(monitorIdentity))
				{
					return null;
				}
				string[] array = monitorIdentity.Split(new char[]
				{
					'\\'
				});
				if (array.Length > 2)
				{
					return array[2];
				}
				return string.Empty;
			}

			public override string ToString()
			{
				return this.identity;
			}

			public override byte[] GetBytes()
			{
				return Encoding.Unicode.GetBytes(this.ToString());
			}

			private readonly string identity;

			internal enum Components
			{
				HealthSetName,
				MonitorName,
				TargetResourceName,
				TotalCount
			}
		}
	}
}
