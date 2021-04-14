using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationRpcClientAccessComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationRpcClientAccessComponent() : base("RpcClientAccess")
		{
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "FilterModernCalendarItemsMomtIcs", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "BlockInsufficientClientVersions", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "StreamInsightUploader", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "FilterModernCalendarItemsMomtSearch", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "RpcHttpClientAccessRulesEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "DetectCharsetAndConvertHtmlBodyOnSave", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "MimumResponseSizeEnforcement", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "XtcEndpoint", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "IncludeTheBodyPropertyBeingOpeningWhenEvaluatingIfAnyBodyPropertyIsDirty", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "ServerInformation", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("RpcClientAccess.settings.ini", "FilterModernCalendarItemsMomtView", typeof(IFeature), false));
		}

		public VariantConfigurationSection FilterModernCalendarItemsMomtIcs
		{
			get
			{
				return base["FilterModernCalendarItemsMomtIcs"];
			}
		}

		public VariantConfigurationSection BlockInsufficientClientVersions
		{
			get
			{
				return base["BlockInsufficientClientVersions"];
			}
		}

		public VariantConfigurationSection StreamInsightUploader
		{
			get
			{
				return base["StreamInsightUploader"];
			}
		}

		public VariantConfigurationSection FilterModernCalendarItemsMomtSearch
		{
			get
			{
				return base["FilterModernCalendarItemsMomtSearch"];
			}
		}

		public VariantConfigurationSection RpcHttpClientAccessRulesEnabled
		{
			get
			{
				return base["RpcHttpClientAccessRulesEnabled"];
			}
		}

		public VariantConfigurationSection DetectCharsetAndConvertHtmlBodyOnSave
		{
			get
			{
				return base["DetectCharsetAndConvertHtmlBodyOnSave"];
			}
		}

		public VariantConfigurationSection MimumResponseSizeEnforcement
		{
			get
			{
				return base["MimumResponseSizeEnforcement"];
			}
		}

		public VariantConfigurationSection XtcEndpoint
		{
			get
			{
				return base["XtcEndpoint"];
			}
		}

		public VariantConfigurationSection IncludeTheBodyPropertyBeingOpeningWhenEvaluatingIfAnyBodyPropertyIsDirty
		{
			get
			{
				return base["IncludeTheBodyPropertyBeingOpeningWhenEvaluatingIfAnyBodyPropertyIsDirty"];
			}
		}

		public VariantConfigurationSection ServerInformation
		{
			get
			{
				return base["ServerInformation"];
			}
		}

		public VariantConfigurationSection FilterModernCalendarItemsMomtView
		{
			get
			{
				return base["FilterModernCalendarItemsMomtView"];
			}
		}
	}
}
