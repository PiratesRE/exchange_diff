using System;
using System.Globalization;

namespace Microsoft.Exchange.AirSync
{
	internal struct HeartBeatInterval
	{
		private HeartBeatInterval(int lowHbi, int highHbi)
		{
			this.lowInterval = lowHbi;
			this.highInterval = highHbi;
		}

		public static HeartBeatInterval Read()
		{
			string text = GlobalSettingsSchema.GetAppSetting(GlobalSettingsSchema.Supporting_MinHeartbeatInterval);
			string text2 = GlobalSettingsSchema.GetAppSetting(GlobalSettingsSchema.Supporting_MaxHeartbeatInterval);
			if (string.IsNullOrEmpty(text))
			{
				text = 60.ToString();
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = 3540.ToString();
			}
			int num;
			if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueNotParsable, new string[]
				{
					"MinHeartbeatInterval",
					typeof(int).Name,
					text ?? "$null",
					60.ToString()
				});
				num = 60;
			}
			int num2;
			if (!int.TryParse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueNotParsable, new string[]
				{
					"MaxHeartbeatInterval",
					typeof(int).Name,
					text2 ?? "$null",
					3540.ToString()
				});
				num2 = 3540;
			}
			if (num > num2)
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_InvalidHbiLimits, new string[]
				{
					60.ToString(CultureInfo.InvariantCulture),
					3540.ToString(CultureInfo.InvariantCulture)
				});
				num = 60;
				num2 = 3540;
			}
			else if (num2 > 3540)
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_MaxHbiTooHigh, new string[]
				{
					3540.ToString(CultureInfo.InvariantCulture),
					3540.ToString(CultureInfo.InvariantCulture),
					60.ToString(CultureInfo.InvariantCulture)
				});
				num = 60;
				num2 = 3540;
			}
			else if (num < 1)
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_MinHbiTooLow, new string[]
				{
					60.ToString(CultureInfo.InvariantCulture),
					3540.ToString(CultureInfo.InvariantCulture)
				});
				num = 60;
				num2 = 3540;
			}
			return new HeartBeatInterval(num, num2);
		}

		public int LowInterval
		{
			get
			{
				return this.lowInterval;
			}
		}

		public int HighInterval
		{
			get
			{
				return this.highInterval;
			}
		}

		public override string ToString()
		{
			return string.Format("Low: {0}, High: {1}", this.LowInterval, this.HighInterval);
		}

		public const string LowAppSettingName = "MinHeartbeatInterval";

		public const string HighAppSettingName = "MaxHeartbeatInterval";

		public const int DefaultLowHbi = 60;

		public const int MinLowHbi = 1;

		public const int MaxLowHbi = 3540;

		public const int DefaultHighHbi = 3540;

		public const int MinHighHbi = 1;

		public const int MaxHighHbi = 3540;

		private int lowInterval;

		private int highInterval;

		public static readonly HeartBeatInterval Default = new HeartBeatInterval(60, 3540);
	}
}
