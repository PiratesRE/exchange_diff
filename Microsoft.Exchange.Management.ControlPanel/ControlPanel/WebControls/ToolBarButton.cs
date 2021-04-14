using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class ToolBarButton : ToolBarItem
	{
		public ToolBarButton(Command command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			this.command = command;
			this.ImageID = command.ImageId;
			this.ImageAltText = command.ImageAltText;
			this.CssClass += " ToolBarButton";
		}

		[Browsable(false)]
		public Command Command
		{
			get
			{
				return this.command;
			}
		}

		[Themeable(true)]
		[UrlProperty]
		public CommandSprite.SpriteId ImageID
		{
			get
			{
				return this.imageId;
			}
			set
			{
				this.imageId = value;
			}
		}

		[DefaultValue("")]
		public string ImageAltText
		{
			get
			{
				return this.imageAltText ?? string.Empty;
			}
			set
			{
				this.imageAltText = value;
			}
		}

		protected override void RenderChildren(HtmlTextWriter writer)
		{
			HyperLink hyperLink = new HyperLink();
			hyperLink.NavigateUrl = "#";
			hyperLink.CssClass = "ToolBarButtonLnk";
			hyperLink.Attributes.Add("onclick", "javascript:return false;");
			hyperLink.Attributes.Add("role", "button");
			CommandSprite commandSprite = null;
			if (this.imageId != CommandSprite.SpriteId.NONE || !string.IsNullOrEmpty(this.Command.CustomSpriteCss))
			{
				commandSprite = new CommandSprite();
				commandSprite.CssClass = "ToolBarImage";
				if (this.imageId != CommandSprite.SpriteId.NONE)
				{
					commandSprite.ImageId = this.ImageID;
				}
				else
				{
					CommandSprite commandSprite2 = commandSprite;
					commandSprite2.CssClass = commandSprite2.CssClass + " " + this.Command.CustomSpriteCss;
				}
				if (string.IsNullOrEmpty(this.Command.Text))
				{
					if (string.IsNullOrEmpty(this.ImageAltText))
					{
						ArgumentNullException ex = new ArgumentNullException("ImageAltText must be set if Command.Text is null/empty.");
						ex.Data["Name"] = this.Command.Name;
						ex.Data["Type"] = this.Command.GetType().FullName;
						ex.Data["ClientId"] = this.ClientID;
						throw ex;
					}
					commandSprite.AlternateText = this.ImageAltText;
					hyperLink.ToolTip = this.ImageAltText;
				}
				hyperLink.Controls.Add(commandSprite);
			}
			else if (!base.DesignMode && string.IsNullOrEmpty(this.Command.Text) && string.IsNullOrEmpty(this.command.CustomSpriteCss))
			{
				throw new ArgumentNullException("Text or ImageId property must be set.");
			}
			if (!string.IsNullOrEmpty(this.Command.Text) || (commandSprite != null && !string.IsNullOrEmpty(commandSprite.AlternateText)))
			{
				EncodingLabel encodingLabel = new EncodingLabel();
				if (!string.IsNullOrEmpty(this.Command.Text))
				{
					encodingLabel.Text = this.Command.Text;
					encodingLabel.CssClass = "ToolBarButtonSpan";
				}
				else
				{
					encodingLabel.Text = commandSprite.AlternateText;
					encodingLabel.CssClass = "ToolBarButtonImageAlter";
				}
				hyperLink.Controls.Add(encodingLabel);
			}
			this.Controls.Add(hyperLink);
			base.RenderChildren(writer);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.Command.PreRender(this);
		}

		public override string ToJavaScript()
		{
			StringBuilder stringBuilder = new StringBuilder("new ToolBarButton(");
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
				(!string.IsNullOrEmpty(this.command.CustomSpriteCss)) ? this.command.CustomSpriteCss : CommandSprite.GetCssClass(this.Command.ImageId),
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

		private Command command;

		private CommandSprite.SpriteId imageId;

		private string imageAltText;
	}
}
