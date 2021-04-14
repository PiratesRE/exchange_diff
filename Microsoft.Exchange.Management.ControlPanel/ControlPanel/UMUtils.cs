using System;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class UMUtils
	{
		public static string GetAudioQualityIconAndAlternateText(float? nmos, out string alternateText)
		{
			float? num = nmos;
			float num2 = (num != null) ? num.GetValueOrDefault() : AudioQuality.UnknownValue;
			switch (AudioQuality.GetQualityOfAudio(num2))
			{
			case 0:
				alternateText = Strings.UMAudioQualityExcellent;
				return CommandSprite.GetCssClass(CommandSprite.SpriteId.AudioQualityFiveBars);
			case 1:
				alternateText = Strings.UMAudioQualityGood;
				return CommandSprite.GetCssClass(CommandSprite.SpriteId.AudioQualityFourBars);
			case 2:
				alternateText = Strings.UMAudioQualityAverage;
				return CommandSprite.GetCssClass(CommandSprite.SpriteId.AudioQualityThreeBars);
			case 3:
				alternateText = Strings.UMAudioQualityPoor;
				return CommandSprite.GetCssClass(CommandSprite.SpriteId.AudioQualityTwoBars);
			case 4:
				alternateText = Strings.UMAudioQualityBad;
				return CommandSprite.GetCssClass(CommandSprite.SpriteId.AudioQualityOneBar);
			}
			alternateText = Strings.UMAudioQualityNotAvailable;
			return CommandSprite.GetCssClass(CommandSprite.SpriteId.AudioQualityNotAvailable);
		}

		public static string AppendMillisecondSuffix(string metric)
		{
			if (string.IsNullOrEmpty(metric))
			{
				return string.Empty;
			}
			return Strings.UMCallDataRecordMillisecondSuffix(metric);
		}

		public static Identity CreateUniqueUMReportingRowIdentity()
		{
			return new Identity(Guid.NewGuid().ToString(), Strings.UMAudioQualityDetails);
		}

		public static string FormatFloat(float? metric)
		{
			if (metric == null)
			{
				return string.Empty;
			}
			return metric.Value.ToString("F1");
		}

		public static string FormatAudioQualityMetricDisplay(string metric)
		{
			if (string.IsNullOrEmpty(metric))
			{
				return "-";
			}
			return metric;
		}

		internal const string PercentFormatString = "#0.0%";

		internal const string UMReportNoDataString = "-";
	}
}
