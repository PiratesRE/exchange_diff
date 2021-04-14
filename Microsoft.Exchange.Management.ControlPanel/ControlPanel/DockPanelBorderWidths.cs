using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class DockPanelBorderWidths
	{
		public DockPanelBorderWidths(string widthString)
		{
			if (!string.IsNullOrEmpty(widthString))
			{
				string[] array = widthString.Split(new char[]
				{
					' '
				});
				int num = array.Length;
				try
				{
					this.Top = ((num > 0) ? int.Parse(array[0]) : 0);
					this.Right = ((num > 1) ? int.Parse(array[1]) : this.Top);
					this.Bottom = ((num > 2) ? int.Parse(array[2]) : this.Top);
					this.Left = ((num > 3) ? int.Parse(array[3]) : this.Right);
				}
				catch (FormatException ex)
				{
					throw new ArgumentException("Invalid border width value: " + ex.Source, ex);
				}
			}
		}

		public int Top { get; set; }

		public int Left { get; set; }

		public int Right { get; set; }

		public int Bottom { get; set; }

		public int HorizontalWidth
		{
			get
			{
				return this.Left + this.Right;
			}
		}

		public int VerticalWidth
		{
			get
			{
				return this.Top + this.Bottom;
			}
		}

		public string FormatCssString(bool isRtl)
		{
			string format = isRtl ? "{0}px {3}px {2}px {1}px" : "{0}px {1}px {2}px {3}px";
			return string.Format(format, new object[]
			{
				this.Top,
				this.Right,
				this.Bottom,
				this.Left
			});
		}
	}
}
