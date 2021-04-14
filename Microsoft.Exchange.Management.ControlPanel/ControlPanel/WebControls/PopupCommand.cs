using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class PopupCommand : NavigateCommand
	{
		static PopupCommand()
		{
			PopupCommand.DefaultPopupSize = new Size(510, 564);
			PopupCommand.DefaultBookmarkedPopupSize = new Size(PopupCommand.DefaultBookmarkedPopupWidth, PopupCommand.DefaultBookmarkedPopupHeight);
		}

		public PopupCommand() : this(null, CommandSprite.SpriteId.NONE)
		{
		}

		public PopupCommand(string text, CommandSprite.SpriteId imageID) : base(text, imageID)
		{
			this.TargetFrame = "_blank";
			this.HasBookmark = true;
		}

		[TypeConverter(typeof(SizeConverter))]
		public virtual Size DialogSize
		{
			get
			{
				if (this.dialogSize != null)
				{
					return this.dialogSize.Value;
				}
				if (!this.HasBookmark)
				{
					return PopupCommand.DefaultPopupSize;
				}
				return PopupCommand.DefaultBookmarkedPopupSize;
			}
			set
			{
				this.dialogSize = new Size?(value);
			}
		}

		[DefaultValue(typeof(Point), "-1, -1")]
		[TypeConverter(typeof(PointConverter))]
		public virtual Point Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}

		[DefaultValue(true)]
		public virtual bool Resizable { get; set; }

		[DefaultValue(false)]
		public virtual bool UseDefaultWindow { get; set; }

		[DefaultValue(false)]
		public virtual bool ShowAddressBar { get; set; }

		[DefaultValue(false)]
		public virtual bool ShowMenuBar { get; set; }

		[DefaultValue(true)]
		public virtual bool ShowStatusBar { get; set; }

		[DefaultValue(false)]
		public virtual bool ShowToolBar { get; set; }

		[DefaultValue(false)]
		public virtual bool SingleInstance { get; set; }

		public bool HasBookmark
		{
			get
			{
				return this.hasBookmark;
			}
			set
			{
				this.hasBookmark = value;
			}
		}

		internal const int DefaultPopupWidth = 510;

		internal const int DefaultPopupHeight = 564;

		internal static readonly int DefaultBookmarkedPopupWidth = 710;

		internal static readonly int DefaultBookmarkedPopupHeight = 564;

		internal static readonly Size DefaultPopupSize = new Size(510, 564);

		internal static readonly Size DefaultBookmarkedPopupSize = new Size(PopupCommand.DefaultBookmarkedPopupWidth, PopupCommand.DefaultBookmarkedPopupHeight);

		private Size? dialogSize = null;

		private bool hasBookmark;

		private Point position = new Point(-1, -1);
	}
}
