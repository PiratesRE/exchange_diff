using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DigitPrompt : VariablePrompt<string>
	{
		protected string Digits
		{
			get
			{
				return this.digits;
			}
			set
			{
				this.digits = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"digit",
				base.Config.PromptName,
				string.Empty,
				this.digits.ToString()
			});
		}

		internal override string ToSsml()
		{
			return this.ssmlString;
		}

		protected override void InternalInitialize()
		{
			this.digits = base.InitVal;
			StringBuilder stringBuilder = new StringBuilder();
			string text = this.digits;
			int i = 0;
			while (i < text.Length)
			{
				char c = text[i];
				string path = string.Empty;
				if (char.IsDigit(c) || c == 'A' || c == 'B' || c == 'C' || c == 'D')
				{
					path = string.Format(CultureInfo.InvariantCulture, "Digit-{0}.wav", new object[]
					{
						c
					});
					goto IL_C9;
				}
				if ('*' == c)
				{
					path = string.Format(CultureInfo.InvariantCulture, "Digit-{0}.wav", new object[]
					{
						"Star"
					});
					goto IL_C9;
				}
				if ('#' == c)
				{
					path = string.Format(CultureInfo.InvariantCulture, "Digit-{0}.wav", new object[]
					{
						"Pound"
					});
					goto IL_C9;
				}
				IL_12C:
				i++;
				continue;
				IL_C9:
				string text2 = Path.Combine(Util.WavPathFromCulture(base.Culture), path);
				if (File.Exists(text2))
				{
					stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "<audio src=\"{0}\" />", new object[]
					{
						text2
					}));
					goto IL_12C;
				}
				stringBuilder.Append(this.AddProsodyWithVolume(" " + c + " "));
				goto IL_12C;
			}
			this.ssmlString = stringBuilder.ToString();
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DigitPrompt successfully intialized text {0}.", new object[]
			{
				this.digits
			});
		}

		internal const string RecordedFileTemplate = "Digit-{0}.wav";

		private string digits;

		private string ssmlString;
	}
}
