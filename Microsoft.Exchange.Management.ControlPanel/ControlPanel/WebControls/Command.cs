using System;
using System.ComponentModel;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class Command
	{
		public Command() : this(null, CommandSprite.SpriteId.NONE)
		{
		}

		public Command(string text, CommandSprite.SpriteId imageID)
		{
			this.OnClientClick = base.GetType().Name + "Handler";
			this.SelectionMode = SelectionMode.SelectionIndependent;
			this.RefreshAction = RefreshAction.SingleRow;
			this.ConfirmDialogType = ModalDialogType.Warning;
			this.Text = text;
			this.ImageId = imageID;
			this.Visible = true;
			this.CustomSpriteCss = string.Empty;
		}

		[DefaultValue("")]
		public string Condition { get; set; }

		[DefaultValue("")]
		public string GroupId { get; set; }

		[Localizable(true)]
		[DefaultValue("")]
		public string SingleSelectionConfirmMessage { get; set; }

		[DefaultValue("")]
		[Localizable(true)]
		public string MultiSelectionConfirmMessage { get; set; }

		[DefaultValue("")]
		[Localizable(true)]
		public string SelectionConfirmMessageDetail { get; set; }

		[DefaultValue(false)]
		public virtual bool DefaultCommand { get; set; }

		[DefaultValue("")]
		public string ShortCut
		{
			get
			{
				return this.shortCut;
			}
			set
			{
				this.shortCut = value;
				this.CoerceRefreshAction();
			}
		}

		[DefaultValue(RefreshAction.SingleRow)]
		public RefreshAction RefreshAction { get; set; }

		private void CoerceRefreshAction()
		{
			if (this.ShortCut == "Delete" && this.RefreshAction == RefreshAction.SingleRow)
			{
				this.RefreshAction = RefreshAction.RemoveSingleRow;
			}
		}

		[DefaultValue(false)]
		public bool HideOnDisable { get; set; }

		[DefaultValue(true)]
		public bool Visible { get; set; }

		[Bindable(true)]
		public CommandSprite.SpriteId ImageId { get; set; }

		[Bindable(true)]
		public string CustomSpriteCss { get; set; }

		[Bindable(false)]
		[DefaultValue("")]
		public virtual string Name { get; set; }

		public virtual string OnClientClick { get; set; }

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] Roles
		{
			get
			{
				if (this.roles == null)
				{
					return new string[0];
				}
				return (string[])this.roles.Clone();
			}
			set
			{
				if (value == null)
				{
					this.roles = value;
					return;
				}
				this.roles = (string[])value.Clone();
			}
		}

		[DefaultValue(SelectionMode.SelectionIndependent)]
		public virtual SelectionMode SelectionMode { get; set; }

		[DefaultValue(false)]
		public virtual bool UseCustomConfirmDialog { get; set; }

		[DefaultValue(ModalDialogType.Warning)]
		public ModalDialogType ConfirmDialogType { get; set; }

		[DefaultValue("")]
		[Localizable(true)]
		public string Text { get; set; }

		[Localizable(true)]
		[DefaultValue("")]
		public string Description { get; set; }

		[DefaultValue("false")]
		public bool AsMoreOption { get; set; }

		public string ClientCommandHandler { get; set; }

		[Localizable(true)]
		[DefaultValue("")]
		public string ImageAltText { get; set; }

		public virtual bool IsAccessibleToUser(IPrincipal user)
		{
			return this.roles == null || LoginUtil.IsInRoles(user, this.roles);
		}

		protected internal virtual void PreRender(Control c)
		{
		}

		private string[] roles;

		private string shortCut;
	}
}
