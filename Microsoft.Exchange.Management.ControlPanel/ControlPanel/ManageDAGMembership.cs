using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ManageDAGMembership : BaseForm
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			string text = base.Request.QueryString["dagManageType"];
			if (text != null && text.Equals("AddOnly", StringComparison.OrdinalIgnoreCase))
			{
				this.ceServers.DisableRemove = true;
				base.Title = Strings.AddDAGMembershipTitle;
				this.ceServers_label.Text = Strings.DAGMembershipAddServers;
				base.Caption = Strings.AddDAGMembershipCaption;
			}
		}

		private const string DagManageTypeKey = "dagManageType";

		private const string AddOnly = "AddOnly";

		protected EcpCollectionEditor ceServers;

		protected Label ceServers_label;
	}
}
