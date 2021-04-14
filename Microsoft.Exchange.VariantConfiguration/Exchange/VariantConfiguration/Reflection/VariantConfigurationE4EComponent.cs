using System;
using Microsoft.Exchange.Flighting;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationE4EComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationE4EComponent() : base("E4E")
		{
			base.Add(new VariantConfigurationSection("E4E.settings.ini", "OTP", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("E4E.settings.ini", "Version", typeof(IVersion), false));
			base.Add(new VariantConfigurationSection("E4E.settings.ini", "LogoffViaOwa", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("E4E.settings.ini", "MsodsGraphQuery", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("E4E.settings.ini", "E4E", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("E4E.settings.ini", "TouchLayout", typeof(IFeature), false));
		}

		public VariantConfigurationSection OTP
		{
			get
			{
				return base["OTP"];
			}
		}

		public VariantConfigurationSection Version
		{
			get
			{
				return base["Version"];
			}
		}

		public VariantConfigurationSection LogoffViaOwa
		{
			get
			{
				return base["LogoffViaOwa"];
			}
		}

		public VariantConfigurationSection MsodsGraphQuery
		{
			get
			{
				return base["MsodsGraphQuery"];
			}
		}

		public VariantConfigurationSection E4E
		{
			get
			{
				return base["E4E"];
			}
		}

		public VariantConfigurationSection TouchLayout
		{
			get
			{
				return base["TouchLayout"];
			}
		}
	}
}
