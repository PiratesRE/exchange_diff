using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeErrorProvider : ErrorProvider
	{
		public ExchangeErrorProvider(IContainer container) : base(container)
		{
		}

		public override bool RightToLeft
		{
			get
			{
				return LayoutHelper.CultureInfoIsRightToLeft;
			}
			set
			{
			}
		}
	}
}
