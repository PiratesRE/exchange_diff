using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class LanguageListPrompt : VariablePrompt<List<CultureInfo>>
	{
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CultureInfo cultureInfo in this.languages)
			{
				stringBuilder.Append(cultureInfo.ToString());
				stringBuilder.Append(",");
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"language",
				base.Config.PromptName,
				string.Empty,
				stringBuilder.ToString()
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "LanguageListPrompt returning ssmlstring: {0}.", new object[]
			{
				this.ssmlString
			});
			return this.ssmlString;
		}

		protected override void InternalInitialize()
		{
			this.languages = base.InitVal;
			this.ssmlString = string.Empty;
			foreach (CultureInfo initVal in this.languages)
			{
				LanguagePrompt languagePrompt = new LanguagePrompt();
				languagePrompt.Initialize(base.Config, base.Culture, initVal);
				this.ssmlString += languagePrompt.ToSsml();
			}
		}

		private List<CultureInfo> languages;

		private string ssmlString;
	}
}
