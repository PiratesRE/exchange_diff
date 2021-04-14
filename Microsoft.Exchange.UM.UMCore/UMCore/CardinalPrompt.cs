using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CardinalPrompt : VariablePrompt<int>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"cardinal",
				base.Config.PromptName,
				string.Empty,
				this.digits.ToString(CultureInfo.InvariantCulture)
			});
		}

		internal override string ToSsml()
		{
			return this.ssmlString;
		}

		protected override void InternalInitialize()
		{
			this.digits = base.InitVal;
			string text = Path.Combine(Util.WavPathFromCulture(base.Culture), string.Format(CultureInfo.InvariantCulture, "Cardinal-{0}.wav", new object[]
			{
				this.digits
			}));
			if (File.Exists(text))
			{
				this.ssmlString = string.Format(CultureInfo.InvariantCulture, "<audio src=\"{0}\" />", new object[]
				{
					text
				});
			}
			else
			{
				this.ssmlString = this.AddProsodyWithVolume(string.Format(CultureInfo.InvariantCulture, "<say-as type=\"number:cardinal\">{0}</say-as>", new object[]
				{
					this.digits
				}));
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "CardinalPrompt successfully intialized text {0}.", new object[]
			{
				this.digits
			});
		}

		internal const string SSMLTemplate = "<say-as type=\"number:cardinal\">{0}</say-as>";

		internal const string RecordedFileTemplate = "Cardinal-{0}.wav";

		private int digits;

		private string ssmlString;
	}
}
