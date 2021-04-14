using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[DefaultProperty("BindingSource")]
	public class BindableUserControlBase : ExchangeUserControl
	{
		public BindingSource BindingSource
		{
			get
			{
				return this.bindingSource;
			}
		}

		public BindableUserControlBase()
		{
			this.bindingSource = new BindingSource(base.Components);
			base.Name = "BindableUserControlBase";
		}

		[AccessedThroughProperty("BindingSource")]
		private BindingSource bindingSource;
	}
}
