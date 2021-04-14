using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AACustomMenuPrompt : VariablePrompt<AutoAttendantContext>
	{
		protected override PromptConfigBase PreviewConfig
		{
			get
			{
				return GlobCfg.DefaultPromptsForPreview.AACustomMenu;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"aaCustomMenu",
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

		internal string[] GetMenu(IEnumerable<CustomMenuKeyMapping> customExtensions)
		{
			string[] array = new string[]
			{
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty
			};
			foreach (CustomMenuKeyMapping customMenuKeyMapping in customExtensions)
			{
				array[(int)customMenuKeyMapping.MappedKey] = customMenuKeyMapping.Description;
			}
			return array;
		}

		protected override void InternalInitialize()
		{
			if (base.InitVal == null)
			{
				return;
			}
			this.aa = base.InitVal.AutoAttendant;
			base.Culture = this.aa.Language.Culture;
			this.menu = this.GetMenu(base.InitVal.IsBusinessHours ? base.InitVal.AutoAttendant.BusinessHoursKeyMapping : base.InitVal.AutoAttendant.AfterHoursKeyMapping);
			if (this.aa.SpeechEnabled)
			{
				this.InitializeSpeechCustomMenu();
			}
			else
			{
				this.InitializeDtmfCustomMenu();
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "AACustomMenuPrompt successfully intialized with ssml '{0}'.", new object[]
			{
				base.SbSsml.ToString()
			});
		}

		private void InitializeDtmfCustomMenu()
		{
			this.AddKeyPrompts();
			if (this.aa.CallSomeoneEnabled || this.aa.SendVoiceMsgEnabled)
			{
				this.AddCallSomeonePrompt();
			}
		}

		private void InitializeSpeechCustomMenu()
		{
			if (this.aa.NameLookupEnabled && (this.aa.CallSomeoneEnabled || this.aa.SendVoiceMsgEnabled))
			{
				base.AddPrompts(GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
				{
					GlobCfg.DefaultPromptsForAA.PleaseSayTheName
				}));
			}
			else
			{
				base.AddPrompts(GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
				{
					GlobCfg.DefaultPromptsForAA.PleaseChooseFrom
				}));
			}
			this.AddDepartmentListPrompt();
			this.AddTimeoutOptionPrompt();
		}

		private void AddKeyPrompts()
		{
			for (int i = 1; i <= 11; i++)
			{
				if (!string.IsNullOrEmpty(this.menu[i]))
				{
					this.AddKeyPrompt(i);
				}
			}
		}

		private void AddDepartmentListPrompt()
		{
			List<string> list = new List<string>();
			for (int i = 1; i < 11; i++)
			{
				if (!string.IsNullOrEmpty(this.menu[i]))
				{
					list.Add(this.menu[i]);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			TextListPrompt textListPrompt = new TextListPrompt();
			textListPrompt.Initialize(base.Config, base.Culture, list);
			base.AddPrompt(textListPrompt);
		}

		private void AddCallSomeonePrompt()
		{
			base.AddPrompts(GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
			{
				GlobCfg.DefaultPromptsForAA.CallSomeone
			}));
		}

		private void AddKeyPrompt(int i)
		{
			ArrayList prompts = GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
			{
				GlobCfg.DefaultPromptsForAA.CustomMenuConfig[i]
			});
			VariablePrompt<string>.SetActualPromptValues(prompts, "departmentName", this.menu[i]);
			base.AddPrompts(prompts);
		}

		private void AddTimeoutOptionPrompt()
		{
			if (string.IsNullOrEmpty(this.menu[11]))
			{
				return;
			}
			this.AddKeyPrompt(11);
		}

		private UMAutoAttendant aa;

		private string[] menu;
	}
}
