using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class TabIndexComparer : IComparer<Control>
	{
		public bool Win32Sort
		{
			get
			{
				return this.win32Sort;
			}
			set
			{
				this.win32Sort = value;
			}
		}

		public int Compare(Control lhs, Control rhs)
		{
			if (!this.Win32Sort)
			{
				bool flag = lhs is GroupBox;
				bool flag2 = rhs is GroupBox;
				if (flag && !flag2)
				{
					return 1;
				}
				if (!flag && flag2)
				{
					return -1;
				}
				if (lhs.TabIndex > rhs.TabIndex)
				{
					return -1;
				}
				if (lhs.TabIndex == rhs.TabIndex)
				{
					return 0;
				}
				return 1;
			}
			else
			{
				if (lhs.TabIndex < rhs.TabIndex)
				{
					return -1;
				}
				if (lhs.TabIndex == rhs.TabIndex)
				{
					return 0;
				}
				return 1;
			}
		}

		public TabIndexComparer(bool win32Sort)
		{
			this.win32Sort = win32Sort;
		}

		private bool win32Sort = true;
	}
}
