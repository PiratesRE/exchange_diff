using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TimeZonePrompt : VariablePrompt<ExTimeZone>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"timeZone",
				base.Config.PromptName,
				string.Empty,
				this.timeZone.ToString()
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "TimeZonePrompt returning ssmlstring: {0}.", new object[]
			{
				this.ssmlString
			});
			return this.ssmlString;
		}

		protected override void InternalInitialize()
		{
			if (base.InitVal == null)
			{
				return;
			}
			this.timeZone = base.InitVal;
			this.IntializeSSML();
		}

		private static Dictionary<string, int> CreateTimeZoneMap()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary["Dateline Standard Time"] = 0;
			dictionary["Samoa Standard Time"] = 1;
			dictionary["Hawaiian Standard Time"] = 2;
			dictionary["Alaskan Standard Time"] = 3;
			dictionary["Pacific Standard Time"] = 4;
			dictionary["US Mountain Standard Time"] = 15;
			dictionary["Mexico Standard Time 2"] = 13;
			dictionary["Mountain Standard Time"] = 10;
			dictionary["Central America Standard Time"] = 33;
			dictionary["Central Standard Time"] = 20;
			dictionary["Mexico Standard Time"] = 30;
			dictionary["Canada Central Standard Time"] = 25;
			dictionary["SA Pacific Standard Time"] = 45;
			dictionary["Eastern Standard Time"] = 35;
			dictionary["US Eastern Standard Time"] = 40;
			dictionary["Atlantic Standard Time"] = 50;
			dictionary["SA Western Standard Time"] = 55;
			dictionary["Pacific SA Standard Time"] = 56;
			dictionary["Newfoundland Standard Time"] = 60;
			dictionary["E. South America Standard Time"] = 65;
			dictionary["SA Eastern Standard Time"] = 70;
			dictionary["Greenland Standard Time"] = 73;
			dictionary["Mid-Atlantic Standard Time"] = 75;
			dictionary["Azores Standard Time"] = 80;
			dictionary["Cape Verde Standard Time"] = 83;
			dictionary["Greenwich Standard Time"] = 90;
			dictionary["GMT Standard Time"] = 85;
			dictionary["W. Europe Standard Time"] = 110;
			dictionary["Central Europe Standard Time"] = 95;
			dictionary["Romance Standard Time"] = 105;
			dictionary["Central European Standard Time"] = 100;
			dictionary["W. Central Africa Standard Time"] = 113;
			dictionary["GTB Standard Time"] = 130;
			dictionary["E. Europe Standard Time"] = 115;
			dictionary["Egypt Standard Time"] = 120;
			dictionary["South Africa Standard Time"] = 140;
			dictionary["FLE Standard Time"] = 125;
			dictionary["Israel Standard Time"] = 135;
			dictionary["Arabic Standard Time"] = 158;
			dictionary["Arab Standard Time"] = 150;
			dictionary["Russian Standard Time"] = 145;
			dictionary["E. Africa Standard Time"] = 155;
			dictionary["Iran Standard Time"] = 160;
			dictionary["Arabian Standard Time"] = 165;
			dictionary["Caucasus Standard Time"] = 170;
			dictionary["Afghanistan Standard Time"] = 175;
			dictionary["Ekaterinburg Standard Time"] = 180;
			dictionary["West Asia Standard Time"] = 185;
			dictionary["India Standard Time"] = 190;
			dictionary["Nepal Standard Time"] = 193;
			dictionary["N. Central Asia Standard Time"] = 201;
			dictionary["Central Asia Standard Time"] = 195;
			dictionary["Sri Lanka Standard Time"] = 200;
			dictionary["Myanmar Standard Time"] = 203;
			dictionary["SE Asia Standard Time"] = 205;
			dictionary["North Asia Standard Time"] = 207;
			dictionary["China Standard Time"] = 210;
			dictionary["North Asia East Standard Time"] = 227;
			dictionary["Singapore Standard Time"] = 215;
			dictionary["W. Australia Standard Time"] = 225;
			dictionary["Taipei Standard Time"] = 220;
			dictionary["Tokyo Standard Time"] = 235;
			dictionary["Korea Standard Time"] = 230;
			dictionary["Yakutsk Standard Time"] = 240;
			dictionary["Cen. Australia Standard Time"] = 250;
			dictionary["AUS Central Standard Time"] = 245;
			dictionary["E. Australia Standard Time"] = 260;
			dictionary["AUS Eastern Standard Time"] = 255;
			dictionary["West Pacific Standard Time"] = 275;
			dictionary["Tasmania Standard Time"] = 265;
			dictionary["Vladivostok Standard Time"] = 270;
			dictionary["Central Pacific Standard Time"] = 280;
			dictionary["New Zealand Standard Time"] = 290;
			dictionary["Fiji Standard Time"] = 285;
			dictionary["Tonga Standard Time"] = 300;
			return dictionary;
		}

		private void IntializeSSML()
		{
			if (!this.TryGetTimeZoneFileSSML(out this.ssmlString))
			{
				this.InitializeDefaultSSML();
			}
		}

		private bool TryGetTimeZoneFileSSML(out string ssml)
		{
			ssml = null;
			int num;
			if (!TimeZonePrompt.timeZoneMap.TryGetValue(this.timeZone.DisplayName, out num))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "TimeZonePrompt: did not find a time zone id for KeyName '{0}'.", new object[]
				{
					this.timeZone.DisplayName
				});
				return false;
			}
			string text = Path.Combine(Util.WavPathFromCulture(base.Culture), string.Format(CultureInfo.InvariantCulture, "TimeZone-{0}.wav", new object[]
			{
				num
			}));
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "TimeZonePrompt looking for file '{0}' (KeyName: '{1}').", new object[]
			{
				text,
				this.timeZone.DisplayName
			});
			if (File.Exists(text))
			{
				ssml = string.Format(CultureInfo.InvariantCulture, "<audio src=\"{0}\" />", new object[]
				{
					text
				});
			}
			return ssml != null;
		}

		private void InitializeDefaultSSML()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "TimeZonePrompt: Initializing default SSML.", new object[0]);
			Regex regex = new Regex("\\((.*?)\\)");
			string inText = regex.Replace(this.timeZone.LocalizableDisplayName.ToString(base.Culture), string.Empty, 1);
			this.ssmlString = this.AddProsodyWithVolume(SpeechUtils.XmlEncode(inText));
		}

		private const string TZFileNameFormat = "TimeZone-{0}.wav";

		private static Dictionary<string, int> timeZoneMap = TimeZonePrompt.CreateTimeZoneMap();

		private ExTimeZone timeZone;

		private string ssmlString;
	}
}
