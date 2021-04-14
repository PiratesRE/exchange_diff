using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class StatementPromptConfig : PromptConfigBase
	{
		internal StatementPromptConfig(List<PromptConfigBase> parameterPrompts, string name, string type, string conditionString, ActivityManagerConfig managerConfig) : base(name, type, conditionString, managerConfig)
		{
			this.parameterPrompts = (parameterPrompts ?? new List<PromptConfigBase>());
		}

		protected List<PromptConfigBase> ParameterPrompts
		{
			get
			{
				return this.parameterPrompts;
			}
			set
			{
				this.parameterPrompts = value;
			}
		}

		private List<PromptConfigBase> parameterPrompts;
	}
}
