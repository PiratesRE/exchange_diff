using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class ContextMenu : WebControl
	{
		public ContextMenu(CommandCollection commands)
		{
			foreach (Command command in commands)
			{
				if (command.Visible)
				{
					MenuItem item;
					if (command is SeparatorCommand)
					{
						item = new MenuSeparator();
					}
					else
					{
						item = new ContextMenuItem(command);
					}
					this.Items.Add(item);
				}
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		public IList<MenuItem> Items
		{
			get
			{
				if (this.items == null)
				{
					this.items = new List<MenuItem>();
				}
				return this.items;
			}
		}

		protected override void CreateChildControls()
		{
			base.EnsureID();
			foreach (MenuItem child in this.Items)
			{
				this.Controls.Add(child);
			}
			base.Attributes.Add("role", "menu");
		}

		public string ToJavaScript()
		{
			StringBuilder stringBuilder = new StringBuilder("new ContextMenu(");
			stringBuilder.Append(string.Format("$get('{0}'),[", this.ClientID));
			stringBuilder.Append(string.Join(",", (from o in this.Items
			select o.ToJavaScript()).ToArray<string>()));
			stringBuilder.Append("])");
			return stringBuilder.ToString();
		}

		private IList<MenuItem> items;
	}
}
