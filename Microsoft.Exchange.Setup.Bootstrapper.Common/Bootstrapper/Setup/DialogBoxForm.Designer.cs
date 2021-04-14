namespace Microsoft.Exchange.Bootstrapper.Setup
{
	public partial class DialogBoxForm : global::System.Windows.Forms.Form
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.okButton = new global::System.Windows.Forms.Button();
			this.statusText = new global::System.Windows.Forms.TextBox();
			base.SuspendLayout();
			this.okButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.okButton.BackColor = global::System.Drawing.Color.White;
			this.okButton.FlatStyle = global::System.Windows.Forms.FlatStyle.Flat;
			this.okButton.ForeColor = global::System.Drawing.Color.DimGray;
			this.okButton.Location = new global::System.Drawing.Point(128, 83);
			this.okButton.Name = "okButton";
			this.okButton.Size = new global::System.Drawing.Size(70, 35);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "[OK]";
			this.okButton.UseVisualStyleBackColor = false;
			this.okButton.Click += new global::System.EventHandler(this.OkButton_Click);
			this.statusText.BackColor = global::System.Drawing.Color.White;
			this.statusText.BorderStyle = global::System.Windows.Forms.BorderStyle.None;
			this.statusText.ForeColor = global::System.Drawing.Color.DimGray;
			this.statusText.Location = new global::System.Drawing.Point(12, 12);
			this.statusText.Multiline = true;
			this.statusText.Name = "statusText";
			this.statusText.ReadOnly = true;
			this.statusText.Size = new global::System.Drawing.Size(300, 65);
			this.statusText.TabIndex = 1;
			this.statusText.Text = "[Text]";
			this.AutoSize = true;
			this.BackColor = global::System.Drawing.Color.White;
			base.ClientSize = new global::System.Drawing.Size(325, 125);
			base.Controls.Add(this.statusText);
			base.Controls.Add(this.okButton);
			this.Font = new global::System.Drawing.Font("Segoe UI", 12f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Pixel);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DialogBoxForm";
			base.ShowInTaskbar = false;
			this.Text = "[Form Text]";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.TextBox statusText;

		private global::System.Windows.Forms.Button okButton;
	}
}
