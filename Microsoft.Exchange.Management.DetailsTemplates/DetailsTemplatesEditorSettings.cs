using System;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	[SettingsProvider(typeof(ExchangeSettingsProvider))]
	public class DetailsTemplatesEditorSettings : DataListViewSettings
	{
		public DetailsTemplatesEditorSettings(IComponent owner) : base(owner)
		{
		}

		[UserScopedSetting]
		[DefaultSettingValue("112")]
		public uint EditorXCoordinate
		{
			get
			{
				return (uint)this["EditorXCoordinate"];
			}
			set
			{
				this["EditorXCoordinate"] = value;
			}
		}

		[DefaultSettingValue("84")]
		[UserScopedSetting]
		public uint EditorYCoordinate
		{
			get
			{
				return (uint)this["EditorYCoordinate"];
			}
			set
			{
				this["EditorYCoordinate"] = value;
			}
		}

		[DefaultSettingValue("800")]
		[UserScopedSetting]
		public uint EditorWidth
		{
			get
			{
				return (uint)this["EditorWidth"];
			}
			set
			{
				this["EditorWidth"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("600")]
		public uint EditorHeight
		{
			get
			{
				return (uint)this["EditorHeight"];
			}
			set
			{
				this["EditorHeight"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("false")]
		public bool IsEditorMaximized
		{
			get
			{
				return (bool)this["IsEditorMaximized"];
			}
			set
			{
				this["IsEditorMaximized"] = value;
			}
		}
	}
}
