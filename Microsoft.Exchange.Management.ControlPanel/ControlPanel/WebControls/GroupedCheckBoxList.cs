using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("GroupedCheckBoxList", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class GroupedCheckBoxList : ScriptControlBase
	{
		public List<GroupHeader> Groups { get; set; }

		private GroupHeader DefaultGroup { get; set; }

		[DefaultValue("Ungrouped")]
		public string DefaultGroupText { get; set; }

		[DefaultValue("GetList")]
		public string WebServiceMethodName { get; set; }

		[DefaultValue(null)]
		[UrlProperty("*.svc")]
		public WebServiceReference GroupWebService { get; set; }

		[DefaultValue(false)]
		public bool ReadOnly { get; set; }

		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BindingCollection FilterParameters
		{
			get
			{
				return this.filterParameters;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			this.DefaultGroup = new GroupHeader
			{
				ID = "Ungrouped",
				Text = (this.DefaultGroupText ?? Strings.DefaultGroupText)
			};
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.InvokeDataItemWebService();
			Dictionary<GroupHeader, List<GroupedCheckBoxListItem>> dictionary = this.GroupDataItems();
			this.Groups.Add(this.DefaultGroup);
			this.CssClass += " GroupedCheckBoxControl";
			if (this.ReadOnly)
			{
				this.CssClass += " ReadOnlyGroupedCheckBoxControl";
			}
			this.CreateGroupControls(dictionary, this);
			if (dictionary.Count == 0)
			{
				Label label = new Label();
				label.Text = Strings.NoGroupedItems;
				this.Controls.Add(label);
			}
		}

		internal static Panel CreateSimplePanel(string className, string divContent)
		{
			return new Panel
			{
				CssClass = className,
				Controls = 
				{
					new LiteralControl(divContent)
				}
			};
		}

		private void InvokeDataItemWebService()
		{
			MethodInfo method = this.GroupWebService.ServiceType.GetMethod(this.WebServiceMethodName ?? "GetList");
			try
			{
				object filter = this.GetFilter();
				MethodBase methodBase = method;
				object serviceInstance = this.GroupWebService.ServiceInstance;
				object[] array = new object[2];
				array[0] = filter;
				PowerShellResults powerShellResults = (PowerShellResults)methodBase.Invoke(serviceInstance, array);
				if (powerShellResults.Succeeded)
				{
					IEnumerable source = (IEnumerable)powerShellResults;
					List<GroupedCheckBoxListItem> list = new List<GroupedCheckBoxListItem>();
					foreach (GroupedCheckBoxListItem item in source.Cast<GroupedCheckBoxListItem>())
					{
						list.Add(item);
					}
					this.results = new PowerShellResults<GroupedCheckBoxListItem>
					{
						Output = list.ToArray()
					};
				}
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private object GetFilter()
		{
			object obj = null;
			if (this.FilterParameters != null && this.FilterParameters.Count > 0)
			{
				Type @interface = this.GroupWebService.ServiceType.GetInterface(typeof(IGetListService<, >).FullName);
				Type type = @interface.GetGenericArguments()[0];
				obj = Activator.CreateInstance(type);
				foreach (Binding binding in this.FilterParameters)
				{
					ISupportServerSideEvaluate supportServerSideEvaluate = binding as ISupportServerSideEvaluate;
					type.GetProperty(binding.Name).SetValue(obj, supportServerSideEvaluate.Value, null);
				}
			}
			return obj;
		}

		private Dictionary<GroupHeader, List<GroupedCheckBoxListItem>> GroupDataItems()
		{
			Dictionary<GroupHeader, List<GroupedCheckBoxListItem>> dictionary = new Dictionary<GroupHeader, List<GroupedCheckBoxListItem>>();
			if (this.results.Failed || this.results.Output == null)
			{
				return dictionary;
			}
			List<GroupedCheckBoxListItem> list = new List<GroupedCheckBoxListItem>();
			foreach (GroupedCheckBoxListItem groupedCheckBoxListItem in this.results.Output)
			{
				bool flag = false;
				foreach (GroupHeader groupHeader in this.Groups)
				{
					if (groupedCheckBoxListItem.Group == groupHeader.ID)
					{
						this.AddItemToGroup(groupedCheckBoxListItem, groupHeader, dictionary);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(groupedCheckBoxListItem);
				}
			}
			if (list.Count > 0)
			{
				dictionary[this.DefaultGroup] = new List<GroupedCheckBoxListItem>(list);
			}
			return dictionary;
		}

		private void AddItemToGroup(GroupedCheckBoxListItem item, GroupHeader group, Dictionary<GroupHeader, List<GroupedCheckBoxListItem>> groupings)
		{
			if (!groupings.ContainsKey(group))
			{
				groupings[group] = new List<GroupedCheckBoxListItem>();
			}
			groupings[group].Add(item);
		}

		private void CreateGroupControls(Dictionary<GroupHeader, List<GroupedCheckBoxListItem>> groups, Control parent)
		{
			foreach (GroupHeader groupHeader in this.Groups)
			{
				if (groups.ContainsKey(groupHeader))
				{
					Panel panel = new Panel();
					Panel panel2 = panel;
					panel2.CssClass += " GroupedCheckBoxGroup";
					parent.Controls.Add(panel);
					this.CreateGroupHeaderControl(groupHeader, panel);
					this.CreateGroupItemControls(groups[groupHeader], panel);
				}
			}
		}

		private void CreateGroupHeaderControl(GroupHeader header, Panel groupPanel)
		{
			Panel child = GroupedCheckBoxList.CreateSimplePanel("GroupedCheckBoxGroupCaption", header.Text);
			groupPanel.Controls.Add(child);
		}

		private void CreateGroupItemControls(IEnumerable<GroupedCheckBoxListItem> items, Control parent)
		{
			HtmlGenericControl htmlGenericControl = new HtmlGenericControl("ul");
			htmlGenericControl.Attributes.Add("role", "group");
			parent.Controls.Add(htmlGenericControl);
			foreach (GroupedCheckBoxListItem groupedCheckBoxListItem in items)
			{
				HtmlGenericControl htmlGenericControl2 = new HtmlGenericControl("li");
				htmlGenericControl.Controls.Add(htmlGenericControl2);
				if (this.ReadOnly)
				{
					htmlGenericControl2.Attributes.Add("value", groupedCheckBoxListItem.Identity.ToJsonString(null));
				}
				else
				{
					this.CreateCheckboxControl(groupedCheckBoxListItem, htmlGenericControl2);
				}
				this.CreateItemControl(groupedCheckBoxListItem, htmlGenericControl2);
				this.CreateFooterControl(htmlGenericControl2);
			}
		}

		private void CreateCheckboxControl(GroupedCheckBoxListItem item, HtmlGenericControl parent)
		{
			CheckBox checkBox = new CheckBox();
			CheckBox checkBox2 = checkBox;
			checkBox2.CssClass += " GroupedCheckBox";
			checkBox.InputAttributes.Add("value", item.Identity.ToJsonString(null));
			checkBox.ID = item.Identity.RawIdentity;
			parent.Controls.Add(checkBox);
		}

		private void CreateItemControl(GroupedCheckBoxListItem item, HtmlGenericControl parent)
		{
			Panel panel = new Panel();
			Panel panel2 = panel;
			panel2.CssClass += " GroupedCheckBoxItem";
			panel.ID = item.Identity.RawIdentity + "_label";
			panel.Attributes.Add("aria-hidden", "false");
			parent.Controls.Add(panel);
			Panel child = GroupedCheckBoxList.CreateSimplePanel("GroupedCheckBoxItemCaption", item.Name);
			panel.Controls.Add(child);
			Panel child2 = GroupedCheckBoxList.CreateSimplePanel("GroupedCheckBoxItemDescription", item.Description);
			panel.Controls.Add(child2);
		}

		private void CreateFooterControl(HtmlGenericControl listItem)
		{
			Panel panel = new Panel();
			Panel panel2 = panel;
			panel2.CssClass += " GroupedCheckBoxFooter";
			listItem.Controls.Add(panel);
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("ReadOnly", this.ReadOnly);
		}

		private PowerShellResults<GroupedCheckBoxListItem> results;

		private BindingCollection filterParameters = new BindingCollection();
	}
}
