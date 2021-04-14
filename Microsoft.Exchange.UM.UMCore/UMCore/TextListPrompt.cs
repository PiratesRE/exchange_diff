using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TextListPrompt : VariablePrompt<List<string>>
	{
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in this.promptNameList)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(",");
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"textList",
				base.Config.PromptName,
				string.Empty,
				stringBuilder.ToString()
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "TextListPrompt returning ssmlstring: {0}.", new object[]
			{
				this.ssmlString
			});
			return this.ssmlString;
		}

		protected override void InternalInitialize()
		{
			this.promptNameList = base.InitVal;
			this.ssmlString = string.Empty;
			foreach (string initVal in this.promptNameList)
			{
				TextPrompt textPrompt = new TextPrompt();
				textPrompt.Initialize(base.Config, base.Culture, initVal);
				this.ssmlString += textPrompt.ToSsml();
				this.ssmlString += "<break time=\"400ms\"/>";
			}
		}

		internal const string BreakSsml = "<break time=\"400ms\"/>";

		private List<string> promptNameList;

		private string ssmlString;
	}
}
