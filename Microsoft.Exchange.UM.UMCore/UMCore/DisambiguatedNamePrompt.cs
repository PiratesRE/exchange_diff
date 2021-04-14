using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DisambiguatedNamePrompt : VariablePrompt<DisambiguatedName>
	{
		public override string ToString()
		{
			string text = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", new object[]
			{
				this.disambiguatedName.Name,
				this.disambiguatedName.DisambiguationText
			});
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"disambiguatedName",
				base.Config.PromptName,
				string.Empty,
				text
			});
		}

		internal override string ToSsml()
		{
			return this.namePrompt.ToSsml() + this.disambiguationTextPrompt.ToSsml();
		}

		protected override void InternalInitialize()
		{
			this.disambiguatedName = base.InitVal;
			if (this.disambiguatedName == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Wrong type = {0}", new object[]
				{
					(base.InitVal == null) ? "<null>" : base.InitVal.GetType().ToString()
				}), "initValue");
			}
			this.namePrompt = new NamePrompt();
			this.namePrompt.Initialize(base.Config, base.Culture, Strings.DisambiguatedNamePrefix(this.disambiguatedName.Name).ToString(base.Culture));
			this.disambiguationTextPrompt = null;
			switch (this.disambiguatedName.DisambiguationField)
			{
			case DisambiguationFieldEnum.Title:
				this.disambiguationTextPrompt = new TextPrompt();
				break;
			case DisambiguationFieldEnum.Department:
				this.disambiguationTextPrompt = new TextPrompt();
				break;
			case DisambiguationFieldEnum.Location:
				this.disambiguationTextPrompt = new AddressPrompt();
				break;
			default:
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, "Invalid value for initValue.DisambiguationField = {0}.", new object[]
				{
					this.disambiguatedName.DisambiguationField
				});
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value = {0}", new object[]
				{
					this.disambiguatedName.DisambiguationField
				}), "initValue.DisambiguationField");
			}
			this.disambiguationTextPrompt.Initialize(base.Config, base.Culture, this.disambiguatedName.DisambiguationText);
		}

		private DisambiguatedName disambiguatedName;

		private NamePrompt namePrompt;

		private TextPrompt disambiguationTextPrompt;
	}
}
