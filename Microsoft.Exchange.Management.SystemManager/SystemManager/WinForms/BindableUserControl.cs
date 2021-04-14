using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class BindableUserControl : BindableUserControlBase
	{
		public BindableUserControl()
		{
			this.validator = new NullValidator();
			base.Name = "BindableUserControl";
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IValidator Validator
		{
			get
			{
				return this.validator;
			}
			set
			{
				this.validator = ((value == null) ? new NullValidator() : value);
			}
		}

		private IValidator validator;
	}
}
