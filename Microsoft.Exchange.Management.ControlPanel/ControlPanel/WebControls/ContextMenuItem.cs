using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class ContextMenuItem : MenuItem
	{
		public ContextMenuItem(Command command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			this.Command = command;
			this.ImageId = command.ImageId;
			this.CssClass = "MenuItem";
			base.Attributes.Add("role", "menuitem");
			if (command.SelectionMode != SelectionMode.SelectionIndependent)
			{
				this.CssClass += " DisabledMenuItem";
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			HyperLink hyperLink = new HyperLink();
			hyperLink.NavigateUrl = this.NavigateUrl;
			hyperLink.Attributes.Add("onclick", this.OnClientClick);
			WebControl webControl = this.CreateImageControl();
			if (webControl != null)
			{
				hyperLink.Controls.Add(webControl);
			}
			else
			{
				this.CssClass += " MenuItemNoImage";
			}
			WebControl webControl2 = this.CreateContentControl();
			if (webControl2 != null)
			{
				hyperLink.Controls.Add(webControl2);
			}
			this.Controls.Add(hyperLink);
		}

		private WebControl CreateImageControl()
		{
			CommandSprite commandSprite = new CommandSprite();
			commandSprite.CssClass = "MenuItemImage";
			commandSprite.ImageId = this.ImageId;
			if (!string.IsNullOrEmpty(this.Command.ImageAltText))
			{
				commandSprite.AlternateText = this.Command.ImageAltText;
			}
			if (commandSprite.IsRenderable)
			{
				return commandSprite;
			}
			commandSprite.Dispose();
			return null;
		}

		private WebControl CreateContentControl()
		{
			if (!string.IsNullOrEmpty(this.Command.Text))
			{
				return new EncodingLabel
				{
					CssClass = "MenuItemContent",
					Text = this.Command.Text
				};
			}
			return null;
		}

		internal string OnClientClick
		{
			get
			{
				return this.onClientClick ?? "javascript:return false;";
			}
			set
			{
				this.onClientClick = value;
			}
		}

		internal string NavigateUrl
		{
			get
			{
				return this.navigateUrl ?? "#";
			}
			set
			{
				this.navigateUrl = value;
			}
		}

		public Command Command { get; set; }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.Command.PreRender(this);
		}

		[Themeable(true)]
		public CommandSprite.SpriteId ImageId { get; set; }

		public override string ToJavaScript()
		{
			StringBuilder stringBuilder = new StringBuilder("new ContextMenuItem(");
			stringBuilder.Append(string.Format("\"{0}\",{1},\"{2}\",\"{3}\",{4},\"{5}\",{6},\"{7}\",{8},{9},\"{10}\",\"{11}\",\"{12}\",\"{13}\",{14}", new object[]
			{
				this.Command.Name,
				this.Command.DefaultCommand.ToJavaScript(),
				this.Command.ShortCut,
				this.Command.SelectionMode,
				string.IsNullOrEmpty(this.Command.OnClientClick) ? "null" : ("function($_){ return " + this.Command.OnClientClick + "($_)}"),
				this.Command.RefreshAction,
				string.IsNullOrEmpty(this.Command.Condition) ? "null" : ("function($_){ return " + this.Command.Condition + "}"),
				this.Command.GroupId,
				this.Command.HideOnDisable.ToJavaScript(),
				this.Command.Visible.ToJavaScript(),
				this.Command.ConfirmDialogType,
				HttpUtility.JavaScriptStringEncode(this.Command.SingleSelectionConfirmMessage),
				HttpUtility.JavaScriptStringEncode(this.Command.SelectionConfirmMessageDetail),
				HttpUtility.JavaScriptStringEncode(this.Command.MultiSelectionConfirmMessage),
				this.Command.UseCustomConfirmDialog.ToJavaScript()
			}));
			PopupCommand popupCommand = this.Command as PopupCommand;
			if (popupCommand != null)
			{
				stringBuilder.Append(string.Format(",\"{0}\",{1},{2},{3},{4},{5},{6},{7},\"{8}\",\"{9}\",{10},{11},\"{12}\",\"{13}\"", new object[]
				{
					popupCommand.SelectionParameterName,
					popupCommand.UseDefaultWindow.ToJavaScript(),
					popupCommand.ShowAddressBar.ToJavaScript(),
					popupCommand.ShowMenuBar.ToJavaScript(),
					popupCommand.ShowStatusBar.ToJavaScript(),
					popupCommand.ShowToolBar.ToJavaScript(),
					popupCommand.Resizable.ToJavaScript(),
					popupCommand.SingleInstance.ToJavaScript(),
					HttpUtility.JavaScriptStringEncode(base.ResolveClientUrl(popupCommand.NavigateUrl)),
					popupCommand.TargetFrame,
					popupCommand.Position.X,
					popupCommand.Position.Y,
					popupCommand.DialogSize.Height,
					popupCommand.DialogSize.Width
				}));
			}
			else
			{
				stringBuilder.Append(",\"\",false,false,false,false,false,false,false,\"\",\"\",-1,-1,\"\",\"\"");
			}
			TaskCommand taskCommand = this.Command as TaskCommand;
			if (taskCommand != null)
			{
				stringBuilder.Append(string.Format(",\"{0}\",{1},\"{2}\",\"{3}\",\"{4}\"", new object[]
				{
					taskCommand.ActionName,
					taskCommand.IsLongRunning.ToJavaScript(),
					HttpUtility.JavaScriptStringEncode(taskCommand.InProgressDescription),
					HttpUtility.JavaScriptStringEncode(taskCommand.StoppedDescription),
					HttpUtility.JavaScriptStringEncode(taskCommand.CompletedDescription)
				}));
			}
			else
			{
				stringBuilder.Append(",\"\",false,\"\",\"\",\"\"");
			}
			stringBuilder.Append(string.Format(",{0},\"{1}\",\"{2}\",\"{3}\",\"{4}\"", new object[]
			{
				true.ToJavaScript(),
				HttpUtility.JavaScriptStringEncode(this.Command.Text),
				CommandSprite.GetCssClass(this.Command.ImageId),
				HttpUtility.JavaScriptStringEncode(this.Command.ImageAltText),
				HttpUtility.JavaScriptStringEncode(this.Command.Description)
			}));
			if (!string.IsNullOrEmpty(this.Command.ClientCommandHandler))
			{
				stringBuilder.Append(string.Format(", new {0}()", this.Command.ClientCommandHandler));
			}
			else
			{
				stringBuilder.Append(",\"\"");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private string onClientClick;

		private string navigateUrl;
	}
}
