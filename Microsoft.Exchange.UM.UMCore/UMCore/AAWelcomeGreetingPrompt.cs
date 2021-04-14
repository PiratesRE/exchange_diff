using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AAWelcomeGreetingPrompt : VariablePrompt<AutoAttendantContext>
	{
		protected override PromptConfigBase PreviewConfig
		{
			get
			{
				return GlobCfg.DefaultPromptsForPreview.AAWelcomeGreeting;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"aaWelcomeGreeting",
				base.Config.PromptName,
				(this.aa == null) ? string.Empty : this.aa.ToString(),
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
			this.aa = base.InitVal.AutoAttendant;
			base.Culture = this.aa.Language.Culture;
			bool isBusinessHours = base.InitVal.IsBusinessHours;
			if (this.aa.BusinessHoursWelcomeGreetingEnabled && isBusinessHours)
			{
				this.AddAAWelcomeCustomPrompt(this.aa.BusinessHoursWelcomeGreetingFilename);
			}
			else if (this.aa.AfterHoursWelcomeGreetingEnabled && !isBusinessHours)
			{
				this.AddAAWelcomeCustomPrompt(this.aa.AfterHoursWelcomeGreetingFilename);
			}
			else if (string.IsNullOrEmpty(this.aa.BusinessName))
			{
				if (isBusinessHours)
				{
					base.AddPrompts(GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
					{
						GlobCfg.DefaultPromptsForAA.AABusinessHoursWelcome
					}));
				}
				else
				{
					base.AddPrompts(GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
					{
						GlobCfg.DefaultPromptsForAA.AAAfterHoursWelcome
					}));
				}
			}
			else
			{
				base.AddPrompts(this.GetAAWelcomeWithBusinessNamePrompt());
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "AAWelcomeGreetingPrompt successfully intialized with ssml '{0}'.", new object[]
			{
				base.SbSsml.ToString()
			});
		}

		private void AddAAWelcomeCustomPrompt(string promptFileName)
		{
			UMConfigCache umconfigCache = new UMConfigCache();
			string prompt = umconfigCache.GetPrompt<UMAutoAttendant>(this.aa, promptFileName);
			PromptConfigBase promptConfigBase = PromptConfigBase.Create(prompt, "wave", string.Empty);
			base.AddPrompts(GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
			{
				promptConfigBase
			}));
		}

		private ArrayList GetAAWelcomeWithBusinessNamePrompt()
		{
			ArrayList arrayList = GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
			{
				GlobCfg.DefaultPromptsForAA.AAWelcomeWithBusinessName
			});
			VariablePrompt<object>.SetActualPromptValues(arrayList, "businessName", this.aa.BusinessName);
			return arrayList;
		}

		private UMAutoAttendant aa;
	}
}
