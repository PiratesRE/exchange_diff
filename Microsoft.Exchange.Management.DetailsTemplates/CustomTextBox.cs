using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	[Designer(typeof(CustomControlDesigner))]
	internal sealed class CustomTextBox : TextBox, IDetailsTemplateControlBound
	{
		public CustomTextBox()
		{
			this.AutoSize = false;
		}

		[TypeConverter(typeof(MAPITypeConverter))]
		public string AttributeName
		{
			get
			{
				return this.detailsTemplateControl.AttributeName;
			}
			set
			{
				this.detailsTemplateControl.AttributeName = value;
			}
		}

		public bool ConfirmationRequired
		{
			get
			{
				return this.detailsTemplateControl.ConfirmationRequired;
			}
			set
			{
				this.detailsTemplateControl.ConfirmationRequired = value;
			}
		}

		public new bool Multiline
		{
			get
			{
				return this.detailsTemplateControl.Multiline;
			}
			set
			{
				this.detailsTemplateControl.Multiline = value;
				base.Multiline = value;
			}
		}

		public new bool ReadOnly
		{
			get
			{
				return this.detailsTemplateControl.ReadOnly;
			}
			set
			{
				this.detailsTemplateControl.ReadOnly = value;
			}
		}

		public new bool UseSystemPasswordChar
		{
			get
			{
				return this.detailsTemplateControl.UseSystemPasswordChar;
			}
			set
			{
				this.detailsTemplateControl.UseSystemPasswordChar = value;
			}
		}

		public override int MaxLength
		{
			get
			{
				return this.detailsTemplateControl.MaxLength;
			}
			set
			{
				this.detailsTemplateControl.MaxLength = value;
			}
		}

		[Browsable(false)]
		public DetailsTemplateControl DetailsTemplateControl
		{
			get
			{
				return this.detailsTemplateControl;
			}
			set
			{
				this.detailsTemplateControl = (value as EditControl);
				base.Multiline = this.detailsTemplateControl.Multiline;
			}
		}

		private EditControl detailsTemplateControl = new EditControl();
	}
}
