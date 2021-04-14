using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("MicroEditListView", "Microsoft.Exchange.Management.ControlPanel.Client.List.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class MicroEditListView : ListView
	{
		protected override void OnLoad(EventArgs e)
		{
			string text = null;
			if (this.MicroEditPanel != null)
			{
				text = this.MicroEditPanel.Roles;
			}
			bool flag = string.IsNullOrEmpty(text) || LoginUtil.IsInRoles(this.Context.User, text.Split(new char[]
			{
				','
			}));
			List<ColumnHeader> list = base.Columns.FindAll((ColumnHeader x) => x is MicroEditColumnHeader);
			if (this.MicroEditPanel == null || !flag)
			{
				list.ForEach(delegate(ColumnHeader x)
				{
					base.Columns.Remove(x);
				});
			}
			else if (list.Count<ColumnHeader>() == 0)
			{
				base.Columns.Add(new MicroEditColumnHeader());
			}
			if (this.MicroEditPanel != null && this.MicroEditPanel.ServiceUrl == null)
			{
				this.MicroEditPanel.ServiceUrl = base.ServiceUrl;
			}
			base.OnLoad(e);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (this.MicroEditPanel != null)
			{
				descriptor.AddComponentProperty("MicroEditPanel", this.MicroEditPanel.ClientID);
				descriptor.AddProperty("MicroEditColumnTitle", this.MicroEditPanel.Title);
			}
		}

		public MicroEditPanel MicroEditPanel { get; set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.MicroEditPanel != null)
			{
				this.Controls.Add(this.MicroEditPanel);
			}
		}
	}
}
