using System;
using System.Linq;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DLPRuleEditor : TransportRuleEditor
	{
		public string DlpPolicy
		{
			get
			{
				return this.dlpPolicy;
			}
			set
			{
				this.dlpPolicy = value;
			}
		}

		public string DlpPolicyMode
		{
			get
			{
				return this.dlpPolicyMode;
			}
			set
			{
				this.dlpPolicyMode = value;
			}
		}

		protected override RulePhrase[] FilterConditions(RulePhrase[] conditions)
		{
			Array.ForEach<RulePhrase>(conditions, delegate(RulePhrase r)
			{
				r.DisplayedInSimpleMode = DLPRuleEditor.simpleModeConditions.Contains(r.Name);
			});
			return conditions;
		}

		protected override RulePhrase[] FilterActions(RulePhrase[] actions)
		{
			Array.ForEach<RulePhrase>(actions, delegate(RulePhrase r)
			{
				r.DisplayedInSimpleMode = DLPRuleEditor.simpleModeActions.Contains(r.Name);
			});
			return actions;
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddProperty("DLPPolicy", this.DlpPolicy, true);
			descriptor.AddProperty("DLPPolicyMode", this.DlpPolicyMode, true);
			base.BuildScriptDescriptor(descriptor);
		}

		private static readonly string[] simpleModeConditions = new string[]
		{
			"From",
			"SentTo",
			"FromScope",
			"SentToScope",
			"FromMemberOf",
			"SentToMemberOf",
			"SubjectOrBodyContains",
			"FromAddressContains",
			"RecipientAddressContains",
			"AttachmentContainsWords",
			"MessageContainsDataClassifications",
			"HasSenderOverride"
		};

		private static readonly string[] simpleModeActions = new string[]
		{
			"ModerateMessageByUser",
			"RedirectMessage",
			"RejectMessage",
			"DeleteMessage",
			"BlindCopyTo",
			"AppendHtmlDisclaimer",
			"NotifySender",
			"GenerateIncidentReport"
		};

		private string dlpPolicy = string.Empty;

		private string dlpPolicyMode = string.Empty;
	}
}
