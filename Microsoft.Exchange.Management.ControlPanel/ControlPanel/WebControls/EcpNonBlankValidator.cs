using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpNonBlankValidator : EcpCustomValidator
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.ClientValidationFunction = "EvaluateIsNonBlank";
			base.DefaultErrorMessage = Strings.NonBlankValidatorMessage;
			base.ValidateEmptyText = true;
		}

		public override string TypeId
		{
			get
			{
				return "nonBlank";
			}
		}
	}
}
