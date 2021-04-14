using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("TransportRuleEditor", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	public class TransportRuleEditor : RuleEditor
	{
		public TransportRuleEditor()
		{
			this.dtpExpiryDate.ID = "expiryDate";
			this.dtpExpiryDate.HasTimePicker = true;
			this.dtpActivationDate.ID = "enableDate";
			this.dtpActivationDate.HasTimePicker = true;
			this.radRuleMode.ID = "ruleMode";
			this.txtComment.ID = "rulecomments";
			this.txtComment.Rows = 3;
			this.txtComment.TextMode = TextBoxMode.MultiLine;
			base.IsCopyMode = false;
		}

		protected override void CreateChildControls()
		{
			this.chkExpiryDate.Text = Strings.TransportRuleExpiryDate;
			this.chkActivationDate.Text = Strings.TransportRuleActivationDate;
			this.chkExpiryDate.ID = string.Format("{0}_label", this.dtpExpiryDate.ID);
			this.chkActivationDate.ID = string.Format("{0}_label", this.dtpActivationDate.ID);
			this.radRuleMode.CellPadding = 0;
			this.radRuleMode.Items.Add(new ListItem(Strings.EnforceRule, RuleMode.Enforce.ToString()));
			if (RbacPrincipal.Current.IsInRole("FFO") && !RbacPrincipal.Current.IsInRole("EOPPremium"))
			{
				this.radRuleMode.Items.Add(new ListItem(Strings.TestRuleDisabledFFO, RuleMode.Audit.ToString()));
			}
			else
			{
				this.radRuleMode.Items.Add(new ListItem(Strings.TestRuleEnabled, RuleMode.AuditAndNotify.ToString()));
				this.radRuleMode.Items.Add(new ListItem(Strings.TestRuleDisabled, RuleMode.Audit.ToString()));
			}
			this.radRuleMode.SelectedValue = RuleMode.Enforce.ToString();
			this.ruleModePanel.Controls.Add(new Label
			{
				Text = Strings.RuleModeDescription,
				ID = string.Format("{0}_label", this.radRuleMode.ID)
			});
			this.ruleModePanel.Controls.Add(this.radRuleMode);
			NumericInputExtender numericInputExtender = new NumericInputExtender();
			numericInputExtender.TargetControlID = this.numberControl.UniqueID;
			this.rulePriorityPanel.Controls.Add(new Label
			{
				Text = Strings.TransportRulePriority,
				ID = string.Format("{0}_label", this.numberControl.ID)
			});
			this.rulePriorityPanel.Controls.Add(new LiteralControl("<br />"));
			this.rulePriorityPanel.Controls.Add(this.numberControl);
			this.rulePriorityPanel.Controls.Add(numericInputExtender);
			this.staticOptionsPanel.Controls.Add(this.rulePriorityPanel);
			this.staticOptionsPanel.Controls.Add(this.ruleModePanel);
			this.activationExpiryPanel.Controls.Add(this.chkActivationDate);
			this.activationExpiryPanel.Controls.Add(this.dtpActivationDate);
			this.activationExpiryPanel.Controls.Add(this.chkExpiryDate);
			this.activationExpiryPanel.Controls.Add(this.dtpExpiryDate);
			this.ruleCommentsPanel.Controls.Add(new Label
			{
				Text = Strings.RuleComments,
				ID = string.Format("{0}_label", this.txtComment.ID)
			});
			this.ruleCommentsPanel.Controls.Add(new LiteralControl("<br />"));
			this.ruleCommentsPanel.Controls.Add(this.txtComment);
			this.evenMoreOptionsPanel.Controls.Add(this.activationExpiryPanel);
			this.evenMoreOptionsPanel.Controls.Add(this.ruleCommentsPanel);
			base.CreateChildControls();
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddProperty("AuditLevelPropertyName", "SetAuditSeverity", true);
			descriptor.AddProperty("SenderAddressLocationPropertyName", "SenderAddressLocation", true);
			descriptor.AddProperty("ActivationDatePropertyName", "ActivationDate", true);
			descriptor.AddProperty("ExpiryDatePropertyName", "ExpiryDate", true);
			descriptor.AddProperty("UseLegecyRegExParameterName", "UseLegacyRegex", true);
			descriptor.AddProperty("DLPPolicyParameterName", "DlpPolicy", true);
			descriptor.AddProperty("ModePropertyName", "Mode", true);
			descriptor.AddProperty("StopProcessingRuleParameterName", "StopRuleProcessing", true);
			descriptor.AddProperty("CommentsPropertyName", "Comments", true);
			descriptor.AddProperty("TransportRuleAgentErrorActionName", "RuleErrorAction", true);
			descriptor.AddProperty("PriorityParameterName", "Priority", true);
			descriptor.AddElementProperty("ActivationExpiryContainer", this.activationExpiryPanel.ClientID, false);
			descriptor.AddComponentProperty("ActivationDatePicker", this.dtpActivationDate.ClientID, false);
			descriptor.AddComponentProperty("ExpiryDatePicker", this.dtpExpiryDate.ClientID, false);
			descriptor.AddElementProperty("ActivationDateCheckbox", this.chkActivationDate.ClientID);
			descriptor.AddElementProperty("ExpiryDateCheckbox", this.chkExpiryDate.ClientID);
			descriptor.AddElementProperty("RuleModeContainer", this.ruleModePanel.ClientID, false);
			descriptor.AddComponentProperty("RuleModeList", this.radRuleMode.ClientID, this);
			descriptor.AddElementProperty("RuleCommentsContainer", this.ruleCommentsPanel.ClientID, false);
			descriptor.AddElementProperty("CommentsField", this.txtComment.ClientID, this);
			descriptor.AddProperty("NotifySenderActionName", "NotifySender", true);
			descriptor.AddProperty("RejectMessageActionName", "RejectMessage", true);
			descriptor.AddProperty("RejectMessageStatusCodeActionName", "RejectMessageEnhancedStatusCode", true);
			descriptor.AddProperty("AuditLevelValues", new EnumValue[]
			{
				new EnumValue(Strings.ReportSeverityLevelLow, "Low"),
				new EnumValue(Strings.ReportSeverityLevelMedium, "Medium"),
				new EnumValue(Strings.ReportSeverityLevelHigh, "High"),
				new EnumValue(Strings.ReportSeverityLevelAuditNone, string.Empty)
			});
			descriptor.AddProperty("PolicyGroupMembershipValues", this.QueryDLPPolicies());
			List<EnumValue> list = new List<EnumValue>();
			foreach (object obj in Enum.GetValues(typeof(SenderAddressLocation)))
			{
				list.Add(new EnumValue(LocalizedDescriptionAttribute.FromEnum(typeof(SenderAddressLocation), obj), obj.ToString()));
			}
			descriptor.AddProperty("SenderAddressLocationValues", list.ToArray());
			descriptor.AddElementProperty("RulePriorityContainer", this.rulePriorityPanel.ClientID, false);
			descriptor.AddElementProperty("PriorityTextbox", this.numberControl.ClientID, this);
			base.BuildScriptDescriptor(descriptor);
		}

		private EnumValue[] QueryDLPPolicies()
		{
			if (!RbacPrincipal.Current.IsInRole("Get-DLPPolicy"))
			{
				return new EnumValue[0];
			}
			WebServiceReference webServiceReference = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=DLPPolicy&Workflow=GetPolicyList");
			PowerShellResults<JsonDictionary<object>> list = webServiceReference.GetList(null, null);
			if (list.Output != null)
			{
				List<EnumValue> list2 = new List<EnumValue>();
				JsonDictionary<object>[] output = list.Output;
				for (int i = 0; i < output.Length; i++)
				{
					Dictionary<string, object> dictionary = output[i];
					string text = (string)dictionary["Name"];
					list2.Add(new EnumValue(AntiXssEncoder.HtmlEncode(text, false), text));
				}
				return list2.ToArray();
			}
			return new EnumValue[0];
		}

		protected override RulePhrase[] FilterActions(RulePhrase[] actions)
		{
			if (this.SimpleModeActions != null)
			{
				Array.ForEach<RulePhrase>(actions, delegate(RulePhrase a)
				{
					a.DisplayedInSimpleMode = this.SimpleModeActions.Contains(a.Name);
				});
			}
			return actions;
		}

		protected override RulePhrase[] FilterConditions(RulePhrase[] conditions)
		{
			if (this.SimpleModeConditions != null)
			{
				Array.ForEach<RulePhrase>(conditions, delegate(RulePhrase a)
				{
					a.DisplayedInSimpleMode = this.SimpleModeConditions.Contains(a.Name);
				});
			}
			return conditions;
		}

		protected override List<string> NonWritablePhraseNameList
		{
			get
			{
				List<string> items = base.NonWritablePhraseNameList;
				string[] array = new string[]
				{
					"ActivationDate",
					"ExpiryDate",
					"StopRuleProcessing",
					"Mode",
					"SetAuditSeverity",
					"SenderAddressLocation",
					"RuleErrorAction",
					"UseLegacyRegex",
					"Comments",
					"DlpPolicy",
					"Priority"
				};
				Array.ForEach<string>(array, delegate(string f)
				{
					StringBuilder stringBuilder = new StringBuilder(150);
					stringBuilder.Append(this.UseSetObject ? "Set-" : "New-");
					stringBuilder.Append(this.RuleService.TaskNoun);
					stringBuilder.Append("?");
					stringBuilder.Append(f);
					stringBuilder.Append(this.WriteScope);
					if (!RbacPrincipal.Current.IsInRole(stringBuilder.ToString()))
					{
						items.Add(f);
					}
				});
				return items;
			}
		}

		public EACHelpId StopProcessingRulesHelpId { get; set; }

		public EACHelpId UseLegacyRegexHelpId { get; set; }

		public EACHelpId DisclaimerLearnMoreHelpId { get; set; }

		public string DisclaimerMessage { get; set; }

		[DefaultValue(false)]
		public bool ForceDisclaimerOnNew { get; set; }

		public string[] SimpleModeActions { get; set; }

		public string[] SimpleModeConditions { get; set; }

		private DateTimePicker dtpExpiryDate = new DateTimePicker();

		private DateTimePicker dtpActivationDate = new DateTimePicker();

		private CheckBox chkExpiryDate = new CheckBox
		{
			CssClass = "RuleStopProcessingCheckBox"
		};

		private CheckBox chkActivationDate = new CheckBox
		{
			CssClass = "RuleStopProcessingCheckBox"
		};

		private EcpRadioButtonList radRuleMode = new EcpRadioButtonList();

		private TextBox txtComment = new TextBox
		{
			Columns = 60,
			MaxLength = 1024
		};

		private Panel activationExpiryPanel = new Panel
		{
			ID = "activationExpiryPanel"
		};

		private Panel ruleCommentsPanel = new Panel
		{
			ID = "ruleCommentsPanel"
		};

		private Panel ruleModePanel = new Panel
		{
			ID = "ruleModePanel"
		};

		private Panel rulePriorityPanel = new Panel
		{
			ID = "rulePriorityPanel"
		};

		private TextBox numberControl = new TextBox
		{
			ID = "priority",
			MaxLength = 3,
			Columns = 4
		};
	}
}
