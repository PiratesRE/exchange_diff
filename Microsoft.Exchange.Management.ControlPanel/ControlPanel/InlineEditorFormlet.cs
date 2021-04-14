using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class InlineEditorFormlet : FormletControlBase<StringArrayParameter, StringArrayModalEditor>
	{
		public InlineEditorFormlet()
		{
			if (string.IsNullOrEmpty(base.FormletDialogTitle))
			{
				base.FormletDialogTitle = Strings.StringArrayDialogTitle;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			base.Parameter = new StringArrayParameter("PopupInlineEditorParameter", new LocalizedString(base.FormletDialogTitle), LocalizedString.Empty, this.MaxLength, new LocalizedString(base.NoSelectionText), this.InputWaterMarkText, "return " + this.ValidationExpression, this.ValidationErrorMessage, this.DuplicateHandlingType);
			if (this.RequiredField != null)
			{
				base.Parameter.RequiredField = this.RequiredField.Value;
			}
		}

		public bool? RequiredField { get; set; }

		public int MaxLength { get; set; }

		public string InputWaterMarkText { get; set; }

		public string ValidationExpression { get; set; }

		public string ValidationErrorMessage { get; set; }

		public DuplicateHandlingType DuplicateHandlingType { get; set; }
	}
}
