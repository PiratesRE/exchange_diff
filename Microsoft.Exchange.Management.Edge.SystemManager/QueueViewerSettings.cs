using System;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	[SettingsProvider(typeof(ExchangeSettingsProvider))]
	public class QueueViewerSettings : ExchangeSettings
	{
		public QueueViewerSettings(IComponent owner) : base(owner)
		{
		}

		[DefaultSettingValue("00:00:30")]
		[UserScopedSetting]
		public TimeSpan RefreshInterval
		{
			get
			{
				return (TimeSpan)this["RefreshInterval"];
			}
			set
			{
				this["RefreshInterval"] = value;
			}
		}

		[DefaultSettingValue("true")]
		[UserScopedSetting]
		public bool AutoRefreshEnabled
		{
			get
			{
				return (bool)this["AutoRefreshEnabled"];
			}
			set
			{
				this["AutoRefreshEnabled"] = value;
			}
		}

		[DefaultSettingValue("1000")]
		[UserScopedSetting]
		public int PageSize
		{
			get
			{
				return (int)this["PageSize"];
			}
			set
			{
				this["PageSize"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public bool UseDefaultServer
		{
			get
			{
				return (bool)this["UseDefaultServer"];
			}
			set
			{
				this["UseDefaultServer"] = value;
			}
		}

		[DefaultSettingValue("localhost")]
		[UserScopedSetting]
		public string DefaultServerName
		{
			get
			{
				return (string)this["DefaultServerName"];
			}
			set
			{
				this["DefaultServerName"] = value;
			}
		}
	}
}
