using System;
using System.Globalization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SpokenNamePrompt : VariablePrompt<object>
	{
		public SpokenNamePrompt()
		{
		}

		internal SpokenNamePrompt(string promptName, CultureInfo culture, object value) : base(promptName, culture, value)
		{
		}

		public override string ToString()
		{
			return this.compositePrompt.ToString();
		}

		internal override string ToSsml()
		{
			return this.compositePrompt.ToSsml();
		}

		protected override void InternalInitialize()
		{
			if (base.InitVal is string)
			{
				this.compositePrompt = new NamePrompt();
				((VariablePrompt<string>)this.compositePrompt).Initialize(base.Config, base.Culture, (string)base.InitVal);
				return;
			}
			if (base.InitVal is ITempWavFile)
			{
				this.compositePrompt = new TempFilePrompt();
				((VariablePrompt<ITempWavFile>)this.compositePrompt).Initialize(base.Config, base.Culture, (ITempWavFile)base.InitVal);
				return;
			}
			if (base.InitVal is DisambiguatedName)
			{
				this.compositePrompt = new DisambiguatedNamePrompt();
				((VariablePrompt<DisambiguatedName>)this.compositePrompt).Initialize(base.Config, base.Culture, (DisambiguatedName)base.InitVal);
				return;
			}
			if (base.InitVal == null)
			{
				this.compositePrompt = new TextPrompt();
				((VariablePrompt<string>)this.compositePrompt).Initialize(base.Config, base.Culture, string.Empty);
				return;
			}
			throw new ArgumentException(base.InitVal.GetType().ToString());
		}

		private Prompt compositePrompt;
	}
}
