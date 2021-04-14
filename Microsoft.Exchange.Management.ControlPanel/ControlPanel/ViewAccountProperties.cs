using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("ViewAccountProperties", "Microsoft.Exchange.Management.ControlPanel.Client.Users.js")]
	public sealed class ViewAccountProperties : Properties
	{
		public Image Image
		{
			get
			{
				return this.image;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.image = this.FindImage(this, "imgUserPhoto");
			Identity identity = Identity.FromExecutingUserId();
			foreach (PopupLauncher popupLauncher in this.GetVisibleControls<PopupLauncher>(this))
			{
				popupLauncher.NavigationUrl = EcpUrl.AppendQueryParameter(popupLauncher.NavigationUrl, "id", identity.RawIdentity);
			}
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "ViewAccountProperties";
			scriptDescriptor.AddElementProperty("Image", this.image.ClientID);
			return scriptDescriptor;
		}

		private Image FindImage(Control parent, string id)
		{
			if (parent.Visible)
			{
				Image image = parent as Image;
				if (image != null && string.Equals(id, image.ID, StringComparison.OrdinalIgnoreCase))
				{
					return image;
				}
				foreach (object obj in parent.Controls)
				{
					Control parent2 = (Control)obj;
					image = this.FindImage(parent2, id);
					if (image != null)
					{
						return image;
					}
				}
			}
			return null;
		}

		private IEnumerable<T> GetVisibleControls<T>(Control parent) where T : class
		{
			if (parent.Visible)
			{
				T control = parent as T;
				if (control != null)
				{
					yield return control;
				}
				foreach (object obj in parent.Controls)
				{
					Control subControl = (Control)obj;
					foreach (T c in this.GetVisibleControls<T>(subControl))
					{
						yield return c;
					}
				}
			}
			yield break;
		}

		private Image image;
	}
}
