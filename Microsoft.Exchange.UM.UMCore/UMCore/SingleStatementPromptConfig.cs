using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SingleStatementPromptConfig : StatementPromptConfig
	{
		internal SingleStatementPromptConfig(List<PromptConfigBase> parameterPrompts, string name, string conditionString, ActivityManagerConfig managerConfig) : base(parameterPrompts, name, "statement", conditionString, managerConfig)
		{
			this.BuildAllCulturePromptConfigs(managerConfig);
		}

		internal override void AddPrompts(ArrayList promptList, ActivityManager manager, CultureInfo culture)
		{
			ArrayList arrayList = null;
			try
			{
				arrayList = this.allCulturePromptConfigs[culture];
			}
			catch (KeyNotFoundException innerException)
			{
				throw new FsmConfigurationException(Strings.MissingResourcePrompt(base.PromptName, culture.LCID), innerException);
			}
			foreach (object obj in arrayList)
			{
				PromptConfigBase promptConfigBase = (PromptConfigBase)obj;
				promptConfigBase.AddPrompts(promptList, manager, culture);
			}
		}

		internal override void Validate()
		{
			ICollection<CultureInfo> collection;
			if (base.PromptName.StartsWith("vui", StringComparison.Ordinal))
			{
				collection = GlobCfg.VuiCultures;
			}
			else
			{
				collection = UmCultures.GetSupportedPromptCultures();
			}
			foreach (CultureInfo key in collection)
			{
				foreach (object obj in this.allCulturePromptConfigs[key])
				{
					PromptConfigBase promptConfigBase = (PromptConfigBase)obj;
					promptConfigBase.Validate();
				}
			}
		}

		private void BuildAllCulturePromptConfigs(ActivityManagerConfig managerConfig)
		{
			ICollection<CultureInfo> collection;
			if (base.PromptName.StartsWith("vui", StringComparison.Ordinal))
			{
				collection = GlobCfg.VuiCultures;
			}
			else
			{
				collection = UmCultures.GetSupportedPromptCultures();
			}
			this.allCulturePromptConfigs = new Dictionary<CultureInfo, ArrayList>(collection.Count);
			foreach (CultureInfo cultureInfo in collection)
			{
				List<PromptConfigBase> c = PromptUtils.CreateStatementPromptConfig(base.PromptName, base.ConditionString, base.ParameterPrompts, cultureInfo, managerConfig);
				this.allCulturePromptConfigs[cultureInfo] = new ArrayList(c);
			}
		}

		private Dictionary<CultureInfo, ArrayList> allCulturePromptConfigs;
	}
}
