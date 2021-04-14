using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Inference.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CommonConfiguration : CommonConfigurationBase
	{
		public static ICommonConfiguration Singleton
		{
			get
			{
				return CommonConfiguration.singletonHook.Value;
			}
		}

		public override bool OutlookActivityProcessingEnabled
		{
			get
			{
				if (this.outlookActivityProcessingEnabled == null)
				{
					this.outlookActivityProcessingEnabled = new bool?(CommonConfiguration.GetInferenceRegistryValue<bool>("OutlookActivityProcessingEnabled", true));
				}
				return this.outlookActivityProcessingEnabled.Value;
			}
		}

		public override bool OutlookActivityProcessingEnabledInEba
		{
			get
			{
				if (this.outlookActivityProcessingEnabledInEba == null)
				{
					this.outlookActivityProcessingEnabledInEba = new bool?(CommonConfiguration.GetInferenceRegistryValue<bool>("OutlookActivityProcessingEnabledInEba", true));
				}
				return this.outlookActivityProcessingEnabledInEba.Value;
			}
		}

		public override TimeSpan OutlookActivityProcessingCutoffWindow
		{
			get
			{
				if (this.outlookActivityProcessingCutoffWindow == null)
				{
					string inferenceRegistryValue = CommonConfiguration.GetInferenceRegistryValue<string>("OutlookActivityProcessingCutoffWindow", null);
					TimeSpan value;
					if (inferenceRegistryValue != null && TimeSpan.TryParse(inferenceRegistryValue, out value))
					{
						this.outlookActivityProcessingCutoffWindow = new TimeSpan?(value);
					}
					else
					{
						this.outlookActivityProcessingCutoffWindow = new TimeSpan?(TimeSpan.FromDays(2.0));
					}
				}
				return this.outlookActivityProcessingCutoffWindow.Value;
			}
		}

		public override bool PersistedLabelsEnabled
		{
			get
			{
				if (this.persistedLabelsEnabled == null)
				{
					this.persistedLabelsEnabled = new bool?(CommonConfiguration.GetInferenceRegistryValue<bool>("PersistedLabelsEnabled", true));
				}
				return this.persistedLabelsEnabled.Value;
			}
		}

		internal CommonConfiguration()
		{
		}

		private static T GetInferenceRegistryValue<T>(string valueName, T defaultValue = default(T))
		{
			return RegistryReader.Instance.GetValue<T>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference", valueName, defaultValue);
		}

		private const string InferenceRegistryBaseKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference";

		private const string outlookActivityProcessingEnabledValueName = "OutlookActivityProcessingEnabled";

		private const string outlookActivityProcessingCutoffWindowValueName = "OutlookActivityProcessingCutoffWindow";

		private const string outlookActivityProcessingEnabledInEbaValueName = "OutlookActivityProcessingEnabledInEba";

		private const string persistedLabelsEnabledValueName = "PersistedLabelsEnabled";

		internal static readonly Hookable<ICommonConfiguration> singletonHook = Hookable<ICommonConfiguration>.Create(true, new CommonConfiguration());

		private bool? outlookActivityProcessingEnabled = null;

		private TimeSpan? outlookActivityProcessingCutoffWindow = null;

		private bool? outlookActivityProcessingEnabledInEba = null;

		private bool? persistedLabelsEnabled = null;
	}
}
