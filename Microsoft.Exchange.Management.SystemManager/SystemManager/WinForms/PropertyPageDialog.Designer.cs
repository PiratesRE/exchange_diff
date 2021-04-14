namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class PropertyPageDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeDialog
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new global::System.ComponentModel.Container();
			base.SuspendLayout();
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			base.CancelVisible = true;
			base.ClientSize = new global::System.Drawing.Size(421, 435);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Name = "PropertyPageDialog";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;
	}
}
