using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class MailboxGreetingPrompt : VariablePrompt<object>
	{
		protected abstract string PromptType { get; }

		protected abstract PromptConfigBase PromptConfig { get; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				this.PromptType,
				base.Config.PromptName,
				string.Empty,
				base.SbLog.ToString()
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Business hours prompt returning ssmlstring: {0}.", new object[]
			{
				base.SbSsml.ToString()
			});
			return base.SbSsml.ToString();
		}

		protected override void InternalInitialize()
		{
			if (base.InitVal == null)
			{
				return;
			}
			base.AddPrompts(this.GetGreetingPrompt());
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "MailboxGreetingPrompt successfully intialized with ssml '{0}'.", new object[]
			{
				base.SbSsml.ToString()
			});
		}

		private ArrayList GetGreetingPrompt()
		{
			ArrayList arrayList = GlobCfg.DefaultPromptHelper.Build(null, base.Culture, new PromptConfigBase[]
			{
				this.PromptConfig
			});
			VariablePrompt<object>.SetActualPromptValues(arrayList, "userName", base.InitVal);
			return arrayList;
		}
	}
}
