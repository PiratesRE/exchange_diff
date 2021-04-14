using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("ToolBar", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class ToolBar : ScriptControlBase
	{
		public ToolBar()
		{
			this.items = new List<ToolBarItem>();
		}

		[RefreshProperties(RefreshProperties.All)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[MergableProperty(false)]
		public CommandCollection Commands
		{
			get
			{
				if (this.commands == null)
				{
					this.commands = new CommandCollection();
				}
				return this.commands;
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override Unit Height
		{
			get
			{
				return base.Height;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		[DefaultValue(false)]
		public bool RightAlign { get; set; }

		public string OwnerControlID { get; set; }

		public string CssForDropDownContextMenu { get; set; }

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			string cssClass = this.CssClass;
			if (string.IsNullOrEmpty(cssClass))
			{
				this.CssClass = "ToolBar";
			}
			else
			{
				this.CssClass += " ToolBar";
			}
			if (this.RightAlign)
			{
				this.CssClass += " ToolBarRightAlign";
			}
			base.AddAttributesToRender(writer);
			this.CssClass = cssClass;
		}

		internal void ApplyRolesFilter()
		{
			if (!this.rolesFilterApplied)
			{
				IPrincipal user = this.Context.User;
				for (int i = this.Commands.Count - 1; i >= 0; i--)
				{
					Command command = this.Commands[i];
					if (command is DropDownCommand)
					{
						((DropDownCommand)command).ApplyRolesFilter(user);
					}
					if (!command.IsAccessibleToUser(user))
					{
						this.Commands.RemoveAt(i);
					}
				}
				this.Commands.MakeReadOnly();
				this.rolesFilterApplied = true;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.ApplyRolesFilter();
			if (!string.IsNullOrEmpty(this.OwnerControlID))
			{
				Control control = this.Parent;
				while (control != null)
				{
					Control control2 = control.FindControl(this.OwnerControlID);
					if (control2 != null)
					{
						this.OwnerControlID = control2.ClientID;
						control = null;
					}
					else
					{
						control = control.Parent;
					}
				}
			}
			base.Attributes.Add("role", "toolbar");
		}

		protected override void RenderChildren(HtmlTextWriter writer)
		{
			int num = int.MaxValue;
			IEnumerable<Command> enumerable = from cmd in this.Commands
			where cmd.AsMoreOption
			select cmd;
			foreach (Command command in enumerable)
			{
				this.additionalItems.Add(new ContextMenuItem(command));
			}
			Command[] array;
			if (this.RightAlign)
			{
				array = (from cmd in this.Commands.Reverse<Command>()
				where !cmd.AsMoreOption
				select cmd).ToArray<Command>();
			}
			else
			{
				array = (from cmd in this.Commands
				where !cmd.AsMoreOption
				select cmd).ToArray<Command>();
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i] is SeparatorCommand))
				{
					bool flag = array[i].SelectionMode == SelectionMode.SelectionIndependent;
					bool flag2 = array[i].Visible && (flag || !array[i].HideOnDisable);
					if (flag2 && i < num)
					{
						num = i;
					}
					if (i > 0)
					{
						ToolBarSeparator toolBarSeparator = new ToolBarSeparator();
						if (!flag2 || num >= i)
						{
							toolBarSeparator.Style.Add(HtmlTextWriterStyle.Display, "none");
						}
						this.items.Add(toolBarSeparator);
						this.Controls.Add(toolBarSeparator);
					}
					ToolBarItem toolBarItem;
					if (array[i] is DropDownCommand)
					{
						toolBarItem = new ToolBarSplitButton((DropDownCommand)array[i])
						{
							HideArrow = ((DropDownCommand)array[i]).HideArrow
						};
					}
					else if (array[i] is InlineSearchBarCommand)
					{
						toolBarItem = new ToolBarMoveButton((InlineSearchBarCommand)array[i]);
					}
					else
					{
						toolBarItem = new ToolBarButton(array[i]);
						ToolBarItem toolBarItem2 = toolBarItem;
						toolBarItem2.CssClass += (flag ? " EnabledToolBarItem" : " DisabledToolBarItem");
					}
					if (!flag2)
					{
						toolBarItem.Style.Add(HtmlTextWriterStyle.Display, "none");
					}
					this.items.Add(toolBarItem);
					this.Controls.Add(toolBarItem);
				}
			}
			base.RenderChildren(writer);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddScriptProperty("Items", this.BuildClientItems(from item in this.items
			select item.ToJavaScript()));
			descriptor.AddComponentProperty("OwnerControl", this.OwnerControlID, true);
			descriptor.AddProperty("CssForDropDownContextMenu", this.CssForDropDownContextMenu, true);
			descriptor.AddScriptProperty("AdditionalItems", this.BuildClientItems(from item in this.additionalItems
			select item.ToJavaScript()));
		}

		private string BuildClientItems(IEnumerable<string> items)
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			stringBuilder.Append(string.Join(",", items.ToArray<string>()));
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		private CommandCollection commands;

		private bool rolesFilterApplied;

		private List<ToolBarItem> items;

		private List<MenuItem> additionalItems = new List<MenuItem>();
	}
}
