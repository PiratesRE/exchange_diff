using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class VariablePromptConfig<TPrompt, TPromptVariable> : PromptConfigBase where TPrompt : VariablePrompt<TPromptVariable>, new()
	{
		internal VariablePromptConfig(string name, string type, string conditionString, ActivityManagerConfig managerConfig) : base(name, type, conditionString, managerConfig)
		{
			QualifiedName variableName = new QualifiedName(name, managerConfig);
			this.fsmVariable = FsmVariable<TPromptVariable>.Create(variableName, managerConfig);
		}

		internal override void AddPrompts(ArrayList promptList, ActivityManager manager, CultureInfo culture)
		{
			TPrompt tprompt = Activator.CreateInstance<TPrompt>();
			if (manager != null)
			{
				PromptSetting config = new PromptSetting(this);
				tprompt.Initialize(config, culture, this.fsmVariable.GetValue(manager));
				tprompt.SetProsodyRate(manager.ProsodyRate);
				tprompt.TTSLanguage = manager.MessagePlayerContext.Language;
			}
			else
			{
				PromptSetting config2 = new PromptSetting(this);
				tprompt.Initialize(config2, culture);
			}
			promptList.Add(tprompt);
		}

		internal override void Validate()
		{
		}

		internal override string GetSuffixVariable(ActivityManager manager)
		{
			TPromptVariable value = this.fsmVariable.GetValue(manager);
			return value.ToString();
		}

		private FsmVariable<TPromptVariable> fsmVariable;
	}
}
