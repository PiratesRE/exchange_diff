using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ServerPickerContent : PickerContent
	{
		protected override void CreateChildControls()
		{
			string text = this.Page.Request.QueryString["workflow"];
			if (!string.IsNullOrEmpty(text))
			{
				base.ServiceUrl = new WebServiceReference(string.Format("{0}&{1}={2}", base.ServiceUrl.ServiceUrl, "workflow", text));
			}
			base.CreateChildControls();
		}

		protected override void OnPreRender(EventArgs e)
		{
			string text = this.Page.Request.QueryString["workflow"];
			if (!string.IsNullOrEmpty(text))
			{
				string a;
				if ((a = text) != null)
				{
					if (a == "GetNameAndRole")
					{
						base.ListView.Columns.Clear();
						base.ListView.Columns.Add(this.CreateNameColumn(30));
						base.ListView.Columns.Add(this.CreateServerRoleColumn(70));
						goto IL_DC;
					}
					if (a == "GetEdgeServer")
					{
						base.ListView.Columns.Clear();
						base.ListView.Columns.Add(this.CreateNameColumn(40));
						base.ListView.Columns.Add(this.CreateAdminDisplayVersionColumn(60));
						goto IL_DC;
					}
				}
				throw new BadQueryParameterException("workflow");
			}
			IL_DC:
			base.OnPreRender(e);
		}

		private ColumnHeader CreateNameColumn(int percentage)
		{
			return new ColumnHeader
			{
				Name = "Name",
				Text = Strings.Name,
				Width = Unit.Percentage((double)percentage)
			};
		}

		private ColumnHeader CreateServerRoleColumn(int percentage)
		{
			return new ColumnHeader
			{
				Name = "ServerRole",
				Text = Strings.ServerPickerRole,
				Width = Unit.Percentage((double)percentage)
			};
		}

		private ColumnHeader CreateAdminDisplayVersionColumn(int percentage)
		{
			return new ColumnHeader
			{
				Name = "AdminDisplayVersion",
				Text = Strings.ServerPickerVersion,
				Width = Unit.Percentage((double)percentage)
			};
		}

		private const string WorkflowQueryString = "workflow";
	}
}
