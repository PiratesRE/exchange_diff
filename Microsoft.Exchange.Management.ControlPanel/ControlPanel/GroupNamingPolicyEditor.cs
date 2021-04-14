using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("GroupNamingPolicyEditor", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	public class GroupNamingPolicyEditor : ScriptControlBase, INamingContainer
	{
		public RulePhrase[] SupportedPrefixes
		{
			get
			{
				return GroupNamingPolicyEditor.supportedRules;
			}
		}

		public RulePhrase[] SupportedSuffixes
		{
			get
			{
				return GroupNamingPolicyEditor.supportedRules;
			}
		}

		public GroupNamingPolicyEditor() : base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "GroupNamingPolicyEditor";
			this.parametersPanel.ID = "parametersPanel";
		}

		public string PrefixLabel { get; set; }

		public string SuffixLabel { get; set; }

		public string ParametersPanel
		{
			get
			{
				return this.parametersPanel.ClientID;
			}
		}

		[DefaultValue(null)]
		public string WriteScope { get; set; }

		public bool UseSetObject
		{
			get
			{
				return this.PropertyControl.UseSetObject;
			}
		}

		private Properties PropertyControl
		{
			get
			{
				if (this.propertiesControl == null)
				{
					for (Control parent = this.Parent; parent != null; parent = parent.Parent)
					{
						if (parent.GetType() == typeof(Properties))
						{
							this.propertiesControl = (Properties)parent;
							break;
						}
					}
					if (this.propertiesControl == null)
					{
						throw new InvalidOperationException("GroupNamingPolicyEditor control must be put inside a Properties control.");
					}
				}
				return this.propertiesControl;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("GroupNameLocString", DistributionGroupNamingPolicy.GroupNameLocString);
			descriptor.AddProperty("PrefixLabel", this.PrefixLabel, true);
			descriptor.AddProperty("SuffixLabel", this.SuffixLabel, true);
			descriptor.AddProperty("MaxLength", 1024.ToString(), true);
			descriptor.AddElementProperty("ParametersPanel", this.ParametersPanel, this);
			if (!this.UseSetObject)
			{
				descriptor.AddProperty("UseSetObject", false);
			}
			EnumParameter enumParameter = (EnumParameter)this.SupportedPrefixes[0].Parameters[0];
			Array.Sort<EnumValue>(enumParameter.Values, (EnumValue val1, EnumValue val2) => val1.DisplayText.CompareTo(val2.DisplayText));
			descriptor.AddScriptProperty("AllPrefixes", this.SupportedPrefixes.ToJsonString(null));
			descriptor.AddScriptProperty("AllSuffixes", this.SupportedSuffixes.ToJsonString(null));
		}

		protected override void CreateChildControls()
		{
			this.Controls.Add(this.parametersPanel);
			RulePhrase[] supportedPrefixes = this.SupportedPrefixes;
			RulePhrase[] supportedSuffixes = this.SupportedSuffixes;
			List<Type> list = new List<Type>();
			this.GetRequiredFormlets(supportedPrefixes, list);
			this.GetRequiredFormlets(supportedSuffixes, list);
			Panel panel = new Panel();
			panel.Style.Add(HtmlTextWriterStyle.Display, "none");
			foreach (Type type in list)
			{
				panel.Controls.Add((Control)Activator.CreateInstance(type));
			}
			this.Controls.Add(panel);
			base.CreateChildControls();
		}

		private void GetRequiredFormlets(RulePhrase[] phrases, List<Type> requiredFormlets)
		{
			if (!phrases.IsNullOrEmpty())
			{
				foreach (RulePhrase rulePhrase in phrases)
				{
					if (!rulePhrase.Parameters.IsNullOrEmpty())
					{
						foreach (FormletParameter formletParameter in rulePhrase.Parameters)
						{
							if (formletParameter.FormletType != null && !requiredFormlets.Contains(formletParameter.FormletType))
							{
								requiredFormlets.Add(formletParameter.FormletType);
							}
						}
					}
				}
			}
		}

		private static RulePhrase[] supportedRules = new RulePhrase[]
		{
			new RulePhrase("Attribute", Strings.GroupNamingPolicyAttribute, new FormletParameter[]
			{
				new EnumParameter("Attribute", Strings.GroupNamingPolicyAttributeDialigTitle, Strings.GroupNamingPolicyAttributeDialigLabel, typeof(GroupNamingPolicyAttributeEnum), null)
			}, null, false),
			new RulePhrase("Text", Strings.GroupNamingPolicyText, new FormletParameter[]
			{
				new StringParameter("Text", Strings.GroupNamingPolicyTextDialigTitle, Strings.GroupNamingPolicyTextDialigLabel, typeof(GroupNamingPolicyText), false)
			}, null, false)
		};

		private Panel parametersPanel = new Panel();

		private Properties propertiesControl;
	}
}
