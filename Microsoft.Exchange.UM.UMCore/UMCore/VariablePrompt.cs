using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class VariablePrompt<T> : Prompt
	{
		public VariablePrompt()
		{
		}

		internal VariablePrompt(string promptName, CultureInfo culture, T value)
		{
			PromptSetting config = new PromptSetting(promptName);
			this.Initialize(config, culture, value);
		}

		protected T InitVal
		{
			get
			{
				return this.initVal;
			}
			set
			{
				this.initVal = value;
			}
		}

		protected StringBuilder SbSsml
		{
			get
			{
				return this.sbSsml;
			}
			set
			{
				this.sbSsml = value;
			}
		}

		protected StringBuilder SbLog
		{
			get
			{
				return this.sbLog;
			}
			set
			{
				this.sbLog = value;
			}
		}

		protected virtual PromptConfigBase PreviewConfig
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal static void SetActualPromptValues(ArrayList prompts, string varName, T varValue)
		{
			foreach (object obj in prompts)
			{
				Prompt prompt = (Prompt)obj;
				VariablePrompt<T> variablePrompt = prompt as VariablePrompt<T>;
				if (variablePrompt != null && string.Equals(variablePrompt.PromptName, varName, StringComparison.OrdinalIgnoreCase))
				{
					variablePrompt.SetInitVal(varValue);
				}
			}
		}

		internal static ArrayList GetPreview<P>(P prompt, CultureInfo c, T initVal) where P : VariablePrompt<T>
		{
			ArrayList arrayList = new ArrayList();
			PromptSetting config = new PromptSetting(prompt.PreviewConfig);
			prompt.Initialize(config, c, initVal);
			arrayList.Add(prompt);
			return arrayList;
		}

		internal virtual void SetInitVal(T initVal)
		{
			this.initVal = initVal;
			this.InternalInitialize();
		}

		internal virtual void SetInitValOnly(T promptValue)
		{
			this.initVal = promptValue;
		}

		internal void Initialize(PromptSetting config, CultureInfo c, T initVal)
		{
			this.initVal = initVal;
			this.SbLog.AppendLine();
			if (!base.IsInitialized)
			{
				base.Initialize(config, c);
			}
		}

		protected virtual string AddProsodyWithVolume(string text)
		{
			return Util.AddProsodyWithVolume(base.Culture, text);
		}

		protected void AddPrompts(ArrayList prompts)
		{
			foreach (object obj in prompts)
			{
				Prompt p = (Prompt)obj;
				this.AddPrompt(p);
			}
		}

		protected void AddPrompt(Prompt p)
		{
			this.SbSsml.Append(p.ToSsml());
			this.SbLog.AppendLine();
			this.SbLog.Append(p.ToString());
		}

		private StringBuilder sbSsml = new StringBuilder();

		private StringBuilder sbLog = new StringBuilder();

		private T initVal;
	}
}
