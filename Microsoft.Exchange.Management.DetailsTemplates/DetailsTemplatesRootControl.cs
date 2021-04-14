using System;
using System.ComponentModel;
using System.Drawing;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class DetailsTemplatesRootControl : ExchangeUserControl
	{
		protected override bool ProcessMnemonic(char charCode)
		{
			return false;
		}

		[ReadOnly(true)]
		public new Point Location
		{
			get
			{
				return base.Location;
			}
		}
	}
}
