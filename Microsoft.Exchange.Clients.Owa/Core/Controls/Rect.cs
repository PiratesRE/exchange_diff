using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public class Rect
	{
		public Rect()
		{
		}

		public Rect(Rect source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.x = source.X;
			this.y = source.Y;
			this.width = source.Width;
			this.height = source.Height;
		}

		public double X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		public double Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		public double Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		public double Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		public bool IntersectsY(Rect otherRect)
		{
			if (otherRect == null)
			{
				throw new ArgumentNullException("otherRect");
			}
			if (this.height == 0.0)
			{
				return this.y >= otherRect.y && this.y <= otherRect.y + otherRect.height;
			}
			if (otherRect.height == 0.0)
			{
				return otherRect.y >= this.y && otherRect.y <= this.y + this.height;
			}
			return this.y + this.height > otherRect.y && this.y < otherRect.y + otherRect.height;
		}

		public void Add(Rect otherRect)
		{
			if (otherRect == null)
			{
				throw new ArgumentNullException("otherRect");
			}
			double val = this.y + this.height;
			double val2 = otherRect.y + otherRect.height;
			double num = Math.Max(val, val2);
			double val3 = this.x + this.width;
			double val4 = otherRect.x + otherRect.width;
			double num2 = Math.Max(val3, val4);
			this.x = Math.Min(this.x, otherRect.x);
			this.width = num2 - this.x;
			this.y = Math.Min(this.y, otherRect.y);
			this.height = num - this.y;
		}

		private double x;

		private double y;

		private double width;

		private double height;
	}
}
