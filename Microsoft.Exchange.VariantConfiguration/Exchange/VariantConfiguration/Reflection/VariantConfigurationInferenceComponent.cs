using System;
using Microsoft.Exchange.Inference.Common;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationInferenceComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationInferenceComponent() : base("Inference")
		{
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "ActivityLogging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceTrainingConfigurationSettings", typeof(IInferenceTrainingConfigurationSettings), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceGroupingModel", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceLatentLabelModel", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceClutterInvitation", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceEventBasedAssistant", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceAutoEnableClutter", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceClutterModelConfigurationSettings", typeof(IClutterModelConfigurationSettings), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceClutterDataSelectionSettings", typeof(IClutterDataSelectionSettings), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceClutterAutoEnablementNotice", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceModelComparison", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceFolderBasedClutter", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Inference.settings.ini", "InferenceStampTracking", typeof(IFeature), false));
		}

		public VariantConfigurationSection ActivityLogging
		{
			get
			{
				return base["ActivityLogging"];
			}
		}

		public VariantConfigurationSection InferenceTrainingConfigurationSettings
		{
			get
			{
				return base["InferenceTrainingConfigurationSettings"];
			}
		}

		public VariantConfigurationSection InferenceGroupingModel
		{
			get
			{
				return base["InferenceGroupingModel"];
			}
		}

		public VariantConfigurationSection InferenceLatentLabelModel
		{
			get
			{
				return base["InferenceLatentLabelModel"];
			}
		}

		public VariantConfigurationSection InferenceClutterInvitation
		{
			get
			{
				return base["InferenceClutterInvitation"];
			}
		}

		public VariantConfigurationSection InferenceEventBasedAssistant
		{
			get
			{
				return base["InferenceEventBasedAssistant"];
			}
		}

		public VariantConfigurationSection InferenceAutoEnableClutter
		{
			get
			{
				return base["InferenceAutoEnableClutter"];
			}
		}

		public VariantConfigurationSection InferenceClutterModelConfigurationSettings
		{
			get
			{
				return base["InferenceClutterModelConfigurationSettings"];
			}
		}

		public VariantConfigurationSection InferenceClutterDataSelectionSettings
		{
			get
			{
				return base["InferenceClutterDataSelectionSettings"];
			}
		}

		public VariantConfigurationSection InferenceClutterAutoEnablementNotice
		{
			get
			{
				return base["InferenceClutterAutoEnablementNotice"];
			}
		}

		public VariantConfigurationSection InferenceModelComparison
		{
			get
			{
				return base["InferenceModelComparison"];
			}
		}

		public VariantConfigurationSection InferenceFolderBasedClutter
		{
			get
			{
				return base["InferenceFolderBasedClutter"];
			}
		}

		public VariantConfigurationSection InferenceStampTracking
		{
			get
			{
				return base["InferenceStampTracking"];
			}
		}
	}
}
