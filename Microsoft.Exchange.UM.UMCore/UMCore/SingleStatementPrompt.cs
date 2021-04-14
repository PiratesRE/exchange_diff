using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SingleStatementPrompt : Prompt
	{
		internal SingleStatementPrompt(params Prompt[] parameterPrompts)
		{
			this.parameterPrompts = ((parameterPrompts != null) ? new List<Prompt>(parameterPrompts) : new List<Prompt>());
		}

		public List<Prompt> PromptList
		{
			get
			{
				return this.promptList;
			}
		}

		public override string ToString()
		{
			foreach (Prompt prompt in this.promptList)
			{
				if (this.log.Length > 0)
				{
					this.log.AppendLine();
				}
				this.log.Append(prompt.ToString());
			}
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"statement",
				base.Config.PromptName,
				string.Empty,
				this.log.ToString()
			});
		}

		internal override string ToSsml()
		{
			foreach (Prompt prompt in this.promptList)
			{
				this.ssml.Append(prompt.ToSsml());
			}
			return this.ssml.ToString();
		}

		protected override void InternalInitialize()
		{
			this.promptList = PromptUtils.CreateStatementPrompt(base.PromptName, this.parameterPrompts, base.Culture);
		}

		private List<Prompt> parameterPrompts;

		private List<Prompt> promptList;

		private StringBuilder ssml = new StringBuilder();

		private StringBuilder log = new StringBuilder();
	}
}
