using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ServerModelConfigurationWrapper : IServerModelConfiguration
	{
		private ServerModelConfigurationWrapper()
		{
			this.configurationSettings = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Inference.InferenceClutterModelConfigurationSettings;
		}

		public static Hookable<Func<IServerModelConfiguration>> HookableServerModelConfiguration
		{
			get
			{
				return ServerModelConfigurationWrapper.HookableWrapper;
			}
		}

		public static IServerModelConfiguration CurrentWrapper
		{
			get
			{
				return ServerModelConfigurationWrapper.HookableWrapper.Value();
			}
		}

		public int MaxModelVersion
		{
			get
			{
				return this.configurationSettings.MaxModelVersion;
			}
		}

		public int MinModelVersion
		{
			get
			{
				return this.configurationSettings.MinModelVersion;
			}
		}

		public int NumberOfVersionCrumbsToRecord
		{
			get
			{
				return this.configurationSettings.NumberOfVersionCrumbsToRecord;
			}
		}

		public bool AllowTrainingOnMutipleModelVersions
		{
			get
			{
				return this.configurationSettings.AllowTrainingOnMutipleModelVersions;
			}
		}

		public int NumberOfModelVersionToTrain
		{
			get
			{
				return this.configurationSettings.NumberOfModelVersionToTrain;
			}
		}

		public IEnumerable<int> BlockedModelVersions
		{
			get
			{
				return this.configurationSettings.BlockedModelVersions;
			}
		}

		public IEnumerable<int> ClassificationModelVersions
		{
			get
			{
				return this.configurationSettings.ClassificationModelVersions;
			}
		}

		public IEnumerable<int> DeprecatedModelVersions
		{
			get
			{
				return this.configurationSettings.DeprecatedModelVersions;
			}
		}

		public double ProbabilityBehaviourSwitchPerWeek
		{
			get
			{
				return this.configurationSettings.ProbabilityBehaviourSwitchPerWeek;
			}
		}

		public double SymmetricNoise
		{
			get
			{
				return this.configurationSettings.SymmetricNoise;
			}
		}

		private static readonly Hookable<Func<IServerModelConfiguration>> HookableWrapper = Hookable<Func<IServerModelConfiguration>>.Create(true, () => new ServerModelConfigurationWrapper());

		private readonly IClutterModelConfigurationSettings configurationSettings;
	}
}
