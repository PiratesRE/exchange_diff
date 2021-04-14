using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public class ColumnHeader
	{
		public Strings.IDs TextID
		{
			get
			{
				return this.textID;
			}
			set
			{
				this.textID = value;
			}
		}

		public ThemeFileId Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
			}
		}

		public bool IsImageHeader
		{
			get
			{
				return this.image != ThemeFileId.None;
			}
		}

		public bool IsCheckBoxHeader
		{
			get
			{
				return this.isCheckBoxHeader;
			}
		}

		private ColumnHeader(Strings.IDs textID, ThemeFileId image, bool isCheckbox)
		{
			this.textID = textID;
			if (isCheckbox)
			{
				this.image = ThemeFileId.None;
				this.isCheckBoxHeader = true;
				return;
			}
			this.image = image;
			this.isCheckBoxHeader = false;
		}

		public ColumnHeader(Strings.IDs textID) : this(textID, ThemeFileId.None, false)
		{
		}

		public ColumnHeader(bool isCheckbox) : this(-1018465893, ThemeFileId.None, true)
		{
		}

		public ColumnHeader(ThemeFileId image) : this(-1018465893, image, false)
		{
		}

		private Strings.IDs textID = -1018465893;

		private ThemeFileId image;

		private bool isCheckBoxHeader;
	}
}
