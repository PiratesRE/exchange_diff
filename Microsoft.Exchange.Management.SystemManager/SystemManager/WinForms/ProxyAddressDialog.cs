using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract partial class ProxyAddressDialog : ExchangeDialog
	{
		public ExchangePage ContentPage
		{
			get
			{
				return this.exchangePage;
			}
		}

		public ProxyAddressDialog()
		{
			this.InitializeComponent();
			this.AutoSize = true;
			base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.MinimumSize = new Size(443, 0);
			this.MaximumSize = new Size(443, 1024);
			this.exchangePage.BindingSource.DataSource = typeof(ProxyAddressBaseDataHandler);
			this.exchangePage.Context = new DataContext(this.DataHandler);
			this.exchangePage.OnSetActive();
		}

		protected abstract ProxyAddressBaseDataHandler DataHandler { get; }

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (base.DialogResult == DialogResult.OK)
			{
				this.exchangePage.InputValidationProvider.WriteBindings();
				e.Cancel = !this.exchangePage.OnKillActive();
			}
		}

		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public ProxyAddressBase ProxyAddressBase
		{
			get
			{
				return this.DataHandler.ProxyAddressBase;
			}
			set
			{
				this.DataHandler.ProxyAddressBase = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public ProxyAddressBase OriginalProxyAddressBase
		{
			get
			{
				return this.DataHandler.OriginalProxyAddressBase;
			}
			set
			{
				this.DataHandler.OriginalProxyAddressBase = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public List<ProxyAddressBase> ProxyAddresses
		{
			get
			{
				return this.DataHandler.ProxyAddresses;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string Prefix
		{
			get
			{
				return this.DataHandler.Prefix;
			}
			set
			{
				this.DataHandler.Prefix = value;
			}
		}

		protected virtual UIValidationError[] GetValidationErrors()
		{
			return UIValidationError.None;
		}

		protected void UpdateError()
		{
			this.exchangePage.UpdateError();
		}

		private class ProxyAddressContentPage : ExchangePage
		{
			public ProxyAddressContentPage() : this(null)
			{
			}

			public ProxyAddressContentPage(ProxyAddressDialog dialog)
			{
				base.Name = "ProxyAddressContentPage";
				this.proxyAddressDialog = dialog;
			}

			protected override UIValidationError[] GetValidationErrors()
			{
				return this.proxyAddressDialog.GetValidationErrors();
			}

			private ProxyAddressDialog proxyAddressDialog;
		}
	}
}
