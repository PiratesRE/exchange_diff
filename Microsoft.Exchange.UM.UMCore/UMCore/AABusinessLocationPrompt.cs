using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AABusinessLocationPrompt : VariablePrompt<AutoAttendantLocationContext>
	{
		protected override PromptConfigBase PreviewConfig
		{
			get
			{
				return GlobCfg.DefaultPromptsForPreview.AABusinessLocation;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"aaBusinessLocation",
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
			this.selectedMenuDescription = base.InitVal.LocationMenuDescription;
			base.Culture = this.aa.Language.Culture;
			if (string.IsNullOrEmpty(this.aa.BusinessLocation))
			{
				base.AddPrompts(this.GetBusinessAddressNotSetPrompt());
			}
			else
			{
				base.AddPrompts(this.GetBusinessAddressPrompt());
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "AABusinessLocationPrompt successfully intialized with ssml '{0}'.", new object[]
			{
				base.SbSsml.ToString()
			});
		}

		private ArrayList GetBusinessAddressPrompt()
		{
			ArrayList arrayList = GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
			{
				GlobCfg.DefaultPromptsForAA.BusinessAddress
			});
			VariablePrompt<string>.SetActualPromptValues(arrayList, "businessAddress", this.aa.BusinessLocation);
			return arrayList;
		}

		private ArrayList GetBusinessAddressNotSetPrompt()
		{
			ArrayList arrayList = GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
			{
				GlobCfg.DefaultPromptsForAA.BusinessAddressNotSet
			});
			VariablePrompt<string>.SetActualPromptValues(arrayList, "selectedMenu", this.selectedMenuDescription);
			return arrayList;
		}

		private UMAutoAttendant aa;

		private string selectedMenuDescription;
	}
}
