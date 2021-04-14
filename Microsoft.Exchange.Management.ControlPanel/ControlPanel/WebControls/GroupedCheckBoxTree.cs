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
	[ClientScriptResource("GroupedCheckBoxTree", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class GroupedCheckBoxTree : ScriptControlBase
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
			Dictionary<GroupHeader, Dictionary<GroupedCheckBoxTreeItem, List<GroupedCheckBoxTreeItem>>> dictionary = this.BuildTree();
			this.Groups.Add(this.DefaultGroup);
			this.CssClass += " GroupedCheckBoxControl";
			this.CreateGroupControls(dictionary, this);
			if (dictionary.Count == 0)
			{
				Label label = new Label();
				label.Text = Strings.NoGroupedItems;
				this.Controls.Add(label);
			}
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
					List<GroupedCheckBoxTreeItem> list = new List<GroupedCheckBoxTreeItem>();
					foreach (GroupedCheckBoxTreeItem item in source.Cast<GroupedCheckBoxTreeItem>())
					{
						list.Add(item);
					}
					this.results = new PowerShellResults<GroupedCheckBoxTreeItem>
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

		private Dictionary<GroupHeader, Dictionary<GroupedCheckBoxTreeItem, List<GroupedCheckBoxTreeItem>>> BuildTree()
		{
			Dictionary<string, GroupedCheckBoxTreeItem> hash = this.BuildDictionary(this.results.Output);
			Dictionary<GroupHeader, Dictionary<GroupedCheckBoxTreeItem, List<GroupedCheckBoxTreeItem>>> dictionary = new Dictionary<GroupHeader, Dictionary<GroupedCheckBoxTreeItem, List<GroupedCheckBoxTreeItem>>>();
			foreach (GroupedCheckBoxTreeItem groupedCheckBoxTreeItem in this.results.Output)
			{
				if (string.IsNullOrEmpty(groupedCheckBoxTreeItem.Parent))
				{
					GroupHeader headerFor = this.GetHeaderFor(groupedCheckBoxTreeItem);
					if (!dictionary.ContainsKey(headerFor))
					{
						dictionary[headerFor] = new Dictionary<GroupedCheckBoxTreeItem, List<GroupedCheckBoxTreeItem>>();
					}
					if (!dictionary[headerFor].ContainsKey(groupedCheckBoxTreeItem))
					{
						dictionary[headerFor].Add(groupedCheckBoxTreeItem, new List<GroupedCheckBoxTreeItem>());
					}
				}
			}
			foreach (GroupedCheckBoxTreeItem groupedCheckBoxTreeItem2 in this.results.Output)
			{
				if (!string.IsNullOrEmpty(groupedCheckBoxTreeItem2.Parent))
				{
					GroupedCheckBoxTreeItem rootFor = this.GetRootFor(groupedCheckBoxTreeItem2, hash);
					dictionary[this.GetHeaderFor(groupedCheckBoxTreeItem2)][rootFor].Add(groupedCheckBoxTreeItem2);
				}
			}
			return dictionary;
		}

		private GroupedCheckBoxTreeItem GetRootFor(GroupedCheckBoxTreeItem item, Dictionary<string, GroupedCheckBoxTreeItem> hash)
		{
			if (item.Parent == null)
			{
				return item;
			}
			return hash[item.Group];
		}

		private Dictionary<string, GroupedCheckBoxTreeItem> BuildDictionary(GroupedCheckBoxTreeItem[] items)
		{
			Dictionary<string, GroupedCheckBoxTreeItem> dictionary = new Dictionary<string, GroupedCheckBoxTreeItem>();
			foreach (GroupedCheckBoxTreeItem groupedCheckBoxTreeItem in items)
			{
				if (groupedCheckBoxTreeItem.Parent == null)
				{
					dictionary[groupedCheckBoxTreeItem.Group] = groupedCheckBoxTreeItem;
				}
			}
			return dictionary;
		}

		private GroupHeader GetHeaderFor(GroupedCheckBoxTreeItem item)
		{
			IEnumerable<GroupHeader> source = from x in this.Groups
			where x.ID == item.Group
			select x;
			if (source.Count<GroupHeader>() <= 0)
			{
				return this.DefaultGroup;
			}
			return source.First<GroupHeader>();
		}

		private void CreateGroupControls(Dictionary<GroupHeader, Dictionary<GroupedCheckBoxTreeItem, List<GroupedCheckBoxTreeItem>>> tree, Control parent)
		{
			foreach (GroupHeader groupHeader in this.Groups)
			{
				if (tree.ContainsKey(groupHeader))
				{
					Panel panel = new Panel();
					Panel panel2 = panel;
					panel2.CssClass += " GroupedCheckBoxGroup";
					this.Controls.Add(panel);
					this.CreateGroupHeaderControl(groupHeader, panel);
					Dictionary<GroupedCheckBoxTreeItem, List<GroupedCheckBoxTreeItem>> dictionary = tree[groupHeader];
					foreach (GroupedCheckBoxTreeItem groupedCheckBoxTreeItem in dictionary.Keys)
					{
						this.CreateGroupItemControls(groupedCheckBoxTreeItem, dictionary[groupedCheckBoxTreeItem], panel);
					}
				}
			}
		}

		private void CreateGroupHeaderControl(GroupHeader header, Panel groupPanel)
		{
			Panel child = GroupedCheckBoxList.CreateSimplePanel("GroupedCheckBoxGroupCaption", header.Text);
			groupPanel.Controls.Add(child);
		}

		private void CreateGroupItemControls(GroupedCheckBoxTreeItem root, IEnumerable<GroupedCheckBoxTreeItem> children, Control parent)
		{
			HtmlGenericControl htmlGenericControl = new HtmlGenericControl("ul");
			htmlGenericControl.Attributes.Add("role", "group");
			parent.Controls.Add(htmlGenericControl);
			HtmlGenericControl htmlGenericControl2 = new HtmlGenericControl("li");
			htmlGenericControl.Controls.Add(htmlGenericControl2);
			CheckBox checkBox = this.CreateCheckboxControl(root, htmlGenericControl2, true);
			this.CreateItemControl(root, htmlGenericControl2);
			this.CreateFooterControl(htmlGenericControl2);
			if (children.Count<GroupedCheckBoxTreeItem>() > 0)
			{
				this.CreateGroupSubList(children, checkBox.ID, htmlGenericControl2);
			}
		}

		private void CreateGroupSubList(IEnumerable<GroupedCheckBoxTreeItem> items, string root, Control parent)
		{
			HtmlGenericControl htmlGenericControl = new HtmlGenericControl("ul");
			htmlGenericControl.ID = root + "_sublist";
			htmlGenericControl.Attributes.Add("class", " GroupedCheckBoxSubList");
			parent.Controls.Add(htmlGenericControl);
			foreach (GroupedCheckBoxTreeItem item in items)
			{
				HtmlGenericControl htmlGenericControl2 = new HtmlGenericControl("li");
				htmlGenericControl.Controls.Add(htmlGenericControl2);
				this.CreateCheckboxControl(item, htmlGenericControl2, false);
				this.CreateItemControl(item, htmlGenericControl2);
				this.CreateFooterControl(htmlGenericControl2);
			}
		}

		private CheckBox CreateCheckboxControl(GroupedCheckBoxTreeItem item, HtmlGenericControl parent, bool root)
		{
			CheckBox checkBox = new CheckBox();
			string value = " GroupedCheckBox " + (root ? " GroupedCheckBoxParent" : string.Empty);
			checkBox.InputAttributes.Add("class", value);
			checkBox.InputAttributes.Add("value", item.Identity.ToJsonString(null));
			checkBox.ID = item.Identity.RawIdentity;
			parent.Controls.Add(checkBox);
			return checkBox;
		}

		private void CreateItemControl(GroupedCheckBoxTreeItem item, HtmlGenericControl parent)
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
		}

		private PowerShellResults<GroupedCheckBoxTreeItem> results;

		private BindingCollection filterParameters = new BindingCollection();
	}
}
