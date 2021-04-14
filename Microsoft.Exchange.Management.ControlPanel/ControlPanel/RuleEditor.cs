using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("RuleEditor", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	public class RuleEditor : ScriptControlBase, INamingContainer
	{
		public RuleEditor() : base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "RuleEditor";
			base.Attributes.Add("mode", "simple");
			this.parametersPanel.ID = "parametersPanel";
			this.AdvModeDialogMinSize = RuleEditor.DefaultAdvModeMinSize;
			this.evenMoreOptionsPanel.ID = "evenMoreOptionsPanel";
			this.staticOptionsPanel.ID = "staticOptionsPanel";
			this.defaultValues = new Dictionary<string, string>();
			this.AllowTypingInPicker = true;
			this.PreferOwaPicker = true;
			this.IsCopyMode = false;
		}

		public string Caption { get; set; }

		public string ActionLabel { get; set; }

		public string ConditionLabel { get; set; }

		public string ExceptionLabel { get; set; }

		public string ParametersPanel
		{
			get
			{
				return this.parametersPanel.ClientID;
			}
		}

		public string MoreOptionLink
		{
			get
			{
				return this.lnkMoreOption.ClientID;
			}
		}

		[DefaultValue(null)]
		public string WriteScope { get; set; }

		[DefaultValue(true)]
		public bool PreferOwaPicker { get; set; }

		[DefaultValue(true)]
		public bool AllowTypingInPicker { get; set; }

		[TypeConverter(typeof(SizeConverter))]
		public Size AdvModeDialogMinSize { get; set; }

		public bool UseSetObject
		{
			get
			{
				return this.PropertyControl.UseSetObject;
			}
		}

		public bool SupportAdvancedMode
		{
			get
			{
				return this.supportAdvancedMode;
			}
			set
			{
				this.supportAdvancedMode = value;
			}
		}

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] DefaultActions
		{
			get
			{
				return this.defaultActions;
			}
			set
			{
				this.defaultActions = value;
			}
		}

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] DefaultConditions
		{
			get
			{
				return this.defaultConditions;
			}
			set
			{
				this.defaultConditions = value;
			}
		}

		public bool IsCopyMode { get; set; }

		public Dictionary<string, string> DefaultValues
		{
			get
			{
				return this.defaultValues;
			}
		}

		protected virtual List<string> NonWritablePhraseNameList
		{
			get
			{
				if (this.nonWritablePhraseNameList == null)
				{
					this.nonWritablePhraseNameList = new List<string>();
					this.nonWritablePhraseNameList.AddRange(from condition in this.RuleService.SupportedConditions
					where !this.HasPermissionsOnPhrase(condition)
					select condition.Name);
					this.nonWritablePhraseNameList.AddRange(from action in this.RuleService.SupportedActions
					where !this.HasPermissionsOnPhrase(action)
					select action.Name);
					this.nonWritablePhraseNameList.AddRange(from exception in this.RuleService.SupportedExceptions
					where !this.HasPermissionsOnPhrase(exception)
					select exception.Name);
					StringBuilder stringBuilder = new StringBuilder(50);
					stringBuilder.Append(this.UseSetObject ? "Set-" : "New-");
					stringBuilder.Append(this.RuleService.TaskNoun);
					stringBuilder.Append("?");
					stringBuilder.Append("Name");
					stringBuilder.Append(this.WriteScope);
					if (!RbacPrincipal.Current.IsInRole(stringBuilder.ToString()))
					{
						this.nonWritablePhraseNameList.Add("Name");
					}
				}
				return this.nonWritablePhraseNameList;
			}
		}

		protected RuleDataService RuleService
		{
			get
			{
				return (RuleDataService)this.PropertyControl.ServiceUrl.ServiceInstance;
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
						throw new InvalidOperationException("RuleEditor control must be put inside a Properties control.");
					}
				}
				return this.propertiesControl;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("Caption", this.Caption, true);
			descriptor.AddProperty("ActionLabel", this.ActionLabel, true);
			descriptor.AddProperty("ConditionLabel", this.ConditionLabel, true);
			descriptor.AddProperty("ExceptionLabel", this.ExceptionLabel, true);
			descriptor.AddProperty("AdvModeDialogMinHeight", this.AdvModeDialogMinSize.Height.ToString(), true);
			descriptor.AddProperty("AdvModeDialogMinWidth", this.AdvModeDialogMinSize.Width.ToString(), true);
			descriptor.AddProperty("RuleNameMaxLength", this.RuleService.RuleNameMaxLength.ToString(), true);
			descriptor.AddElementProperty("MoreOptionButton", this.MoreOptionLink, this);
			descriptor.AddElementProperty("StaticOptionsPanel", this.staticOptionsPanel.ClientID, this);
			descriptor.AddElementProperty("MoreOptionsPanel", this.evenMoreOptionsPanel.ClientID, this);
			descriptor.AddElementProperty("ParametersPanel", this.ParametersPanel, this);
			if (!this.UseSetObject)
			{
				descriptor.AddProperty("UseSetObject", false);
			}
			descriptor.AddProperty("SupportAdvancedMode", this.SupportAdvancedMode);
			descriptor.AddScriptProperty("NonWritablePhraseNames", this.NonWritablePhraseNameList.ToJsonString(null));
			descriptor.AddScriptProperty("AllConditions", this.RuleService.SupportedConditions.ToJsonString(null));
			descriptor.AddScriptProperty("AllActions", this.RuleService.SupportedActions.ToJsonString(null));
			descriptor.AddScriptProperty("AllExceptions", this.RuleService.SupportedExceptions.ToJsonString(null));
			if (this.defaultConditions != null)
			{
				descriptor.AddScriptProperty("DefaultConditions", this.defaultConditions.ToJsonString(null));
			}
			if (this.defaultActions != null)
			{
				descriptor.AddScriptProperty("DefaultActions", this.defaultActions.ToJsonString(null));
			}
			if (this.defaultValues.Count != 0)
			{
				descriptor.AddScriptProperty("DefaultValues", new JsonDictionary<string>(this.defaultValues).ToJsonString(null));
			}
			descriptor.AddProperty("IsCopyMode", this.IsCopyMode);
		}

		protected override void CreateChildControls()
		{
			this.Controls.Add(this.parametersPanel);
			RulePhrase[] phrases = this.FilterConditions(this.RuleService.SupportedConditions);
			RulePhrase[] phrases2 = this.FilterActions(this.RuleService.SupportedActions);
			RulePhrase[] phrases3 = this.FilterExceptions(this.RuleService.SupportedExceptions);
			List<Type> list = new List<Type>();
			RuleEditor.GetRequiredFormlets(phrases, list);
			RuleEditor.GetRequiredFormlets(phrases2, list);
			RuleEditor.GetRequiredFormlets(phrases3, list);
			Panel panel = new Panel();
			panel.Style.Add(HtmlTextWriterStyle.Display, "none");
			foreach (Type type in list)
			{
				if (string.Equals(type.Name, "PeoplePicker", StringComparison.Ordinal))
				{
					PeoplePicker peoplePicker = (PeoplePicker)Activator.CreateInstance(type);
					peoplePicker.PreferOwaPicker = this.PreferOwaPicker;
					peoplePicker.AllowTyping = this.AllowTypingInPicker;
					panel.Controls.Add(peoplePicker);
				}
				else
				{
					panel.Controls.Add((Control)Activator.CreateInstance(type));
				}
			}
			this.Controls.Add(panel);
			this.Controls.Add(this.staticOptionsPanel);
			Panel panel2 = new Panel();
			panel2.CssClass = "MoreOptionDiv";
			this.lnkMoreOption = new HtmlAnchor();
			this.lnkMoreOption.HRef = "javascript:void(0);";
			this.lnkMoreOption.ID = "btnMoreOption";
			this.lnkMoreOption.Attributes.Add("class", "MoreOptionLnk");
			this.lnkMoreOption.Controls.Add(new LiteralControl(Strings.RuleMoreOptions));
			panel2.Controls.Add(this.lnkMoreOption);
			this.Controls.Add(panel2);
			this.evenMoreOptionsPanel.CssClass = "hidden";
			this.Controls.Add(this.evenMoreOptionsPanel);
			base.CreateChildControls();
		}

		private bool HasPermissionsOnPhrase(RulePhrase phrase)
		{
			string str = (this.UseSetObject ? "Set-" : "New-") + this.RuleService.TaskNoun + "?";
			foreach (FormletParameter formletParameter in phrase.Parameters)
			{
				if (formletParameter.TaskParameterNames != null)
				{
					string str2 = string.IsNullOrEmpty(phrase.AdditionalRoles) ? string.Empty : (phrase.AdditionalRoles + "+");
					string str3 = string.Join("&", formletParameter.TaskParameterNames);
					if (!RbacPrincipal.Current.IsInRole(str2 + str + str3 + this.WriteScope))
					{
						return false;
					}
				}
			}
			return true;
		}

		internal static void GetRequiredFormlets(RulePhrase[] phrases, List<Type> requiredFormlets)
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

		protected virtual RulePhrase[] FilterConditions(RulePhrase[] conditions)
		{
			return conditions;
		}

		protected virtual RulePhrase[] FilterActions(RulePhrase[] actions)
		{
			return actions;
		}

		protected virtual RulePhrase[] FilterExceptions(RulePhrase[] exceptions)
		{
			return exceptions;
		}

		private const string NameParameterString = "Name";

		private const int DefaultAdvModeMinWidth = 700;

		private const int DefaultAdvModeMinHeight = 450;

		private static readonly Size DefaultAdvModeMinSize = new Size(700, 450);

		private Panel parametersPanel = new Panel();

		private HtmlAnchor lnkMoreOption;

		private List<string> nonWritablePhraseNameList;

		private Properties propertiesControl;

		private bool supportAdvancedMode = true;

		private string[] defaultActions;

		private string[] defaultConditions;

		private Dictionary<string, string> defaultValues;

		protected Panel evenMoreOptionsPanel = new Panel();

		protected Panel staticOptionsPanel = new Panel();
	}
}
