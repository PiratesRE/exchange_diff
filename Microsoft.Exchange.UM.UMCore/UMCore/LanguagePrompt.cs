using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class LanguagePrompt : VariablePrompt<CultureInfo>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"language",
				base.Config.PromptName,
				string.Empty,
				this.language.ToString()
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "LanguagePrompt returning ssmlstring: {0}.", new object[]
			{
				this.ssmlString
			});
			return this.ssmlString;
		}

		protected override void InternalInitialize()
		{
			this.language = UmCultures.GetDisambiguousLanguageFamily(base.InitVal);
			this.IntializeSSML();
		}

		private void IntializeSSML()
		{
			if (!this.TryGetLanguageFileSSML(out this.ssmlString))
			{
				this.InitializeDefaultSSML();
			}
		}

		private bool TryGetLanguageFileSSML(out string ssml)
		{
			ssml = null;
			string text = Path.Combine(Util.WavPathFromCulture(base.Culture), string.Format(CultureInfo.InvariantCulture, "Language-{0}.wav", new object[]
			{
				UmCultures.GetLanguagePromptLCID(this.language)
			}));
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "LanguagePrompt looking for file '{0}'.", new object[]
			{
				text
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
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "LanguagePrompt: Initializing default SSML.", new object[0]);
			string name = string.Format(CultureInfo.InvariantCulture, "Language-{0}", new object[]
			{
				UmCultures.GetLanguagePromptLCID(this.language)
			});
			string text = PromptConfigBase.PromptResourceManager.GetString(name, base.Culture);
			if (text == null)
			{
				text = this.language.DisplayName;
			}
			this.ssmlString = this.AddProsodyWithVolume(SpeechUtils.XmlEncode(text));
		}

		private const string LanguageFileNameFormat = "Language-{0}.wav";

		private CultureInfo language;

		private string ssmlString;
	}
}
