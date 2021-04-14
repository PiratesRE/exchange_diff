using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(ListView))]
	[ClientScriptResource("VDirListView", "Microsoft.Exchange.Management.ControlPanel.Client.OrgSettings.js")]
	public class VDirListView : ListView
	{
		protected override void OnLoad(EventArgs e)
		{
			try
			{
				WebServiceReference webServiceReference = new WebServiceReference("~/DDI/DDIService.svc?schema=VirtualDirectory&workflow=GetServerDropDown");
				this.serverListResult = webServiceReference.GetList(null, null);
				if (this.serverListResult.Output != null && this.serverListResult.Output.Length > 0)
				{
					base.Views = new List<ListItem>();
					JsonDictionary<object>[] output = this.serverListResult.Output;
					for (int i = 0; i < output.Length; i++)
					{
						Dictionary<string, object> dictionary = output[i];
						base.Views.Add(new ListItem((string)dictionary["Fqdn"], (string)dictionary["Fqdn"]));
					}
					base.Views.Sort((ListItem item1, ListItem item2) => item1.Text.CompareTo(item2.Text));
				}
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			finally
			{
				base.Views.Insert(0, new ListItem(Strings.AllServers, string.Empty));
			}
			ListItem[] items = new ListItem[]
			{
				new ListItem(Strings.AllVDirTypes, "All"),
				new ListItem("Autodiscover", "Autodiscover"),
				new ListItem("EAS", "EAS"),
				new ListItem("ECP", "ECP"),
				new ListItem("EWS", "EWS"),
				new ListItem("OAB", "OAB"),
				new ListItem("OWA", "OWA"),
				new ListItem("PowerShell", "PowerShell")
			};
			this.vDirDropDown = new FilterDropDown();
			this.vDirDropDown.LabelText = Strings.SelectVDirTypeLabel;
			this.vDirDropDown.ID = "VDirTypeDropDown";
			this.vDirDropDown.Items.AddRange(items);
			this.vDirDropDown.Width = Unit.Percentage(100.0);
			ComponentBinding componentBinding = new ComponentBinding(this.vDirDropDown, "filterValue");
			componentBinding.Name = "SelectedVDirType";
			WebServiceListSource webServiceListSource = (WebServiceListSource)this.FindControl("listSource");
			webServiceListSource.FilterParameters.Add(componentBinding);
			this.vDirDropDown.Style.Add(HtmlTextWriterStyle.MarginTop, "30");
			this.FindControl("ViewFilterDropDown").Parent.Controls.Add(this.vDirDropDown);
			base.OnLoad(e);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddScriptProperty("ServerListResult", this.serverListResult.ToJsonString(null));
			descriptor.AddComponentProperty("VDirTypeDropDown", this.vDirDropDown.ClientID);
		}

		private FilterDropDown vDirDropDown;

		private PowerShellResults<JsonDictionary<object>> serverListResult;
	}
}
