using System;
using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class BaseValidatorAdapter : ControlAdapter
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.Control.Display = ValidatorDisplay.Dynamic;
			this.Control.Text = (this.Control.ErrorMessage = null);
			this.Control.Attributes["TypeId"] = ((IEcpValidator)this.Control).TypeId;
			if (this.Control is CompareValidator)
			{
				CompareValidator compareValidator = this.Control as CompareValidator;
				Control control = this.Control.NamingContainer.FindControl(compareValidator.ControlToCompare);
				this.Control.Attributes["ControlToCompare"] = control.ClientID;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			this.TrySetEcpErrorMessage(((IEcpValidator)this.Control).DefaultErrorMessage);
			base.Render(writer);
		}

		private void TrySetEcpErrorMessage(string mesage)
		{
			if (string.IsNullOrEmpty(this.Control.Attributes["EcpErrorMessage"]))
			{
				this.Control.Attributes["EcpErrorMessage"] = mesage;
			}
		}

		private new BaseValidator Control
		{
			get
			{
				return (BaseValidator)base.Control;
			}
		}

		private const string EcpErrorMessageAttributeName = "EcpErrorMessage";

		private const string TypeIdAttributeName = "TypeId";

		private const string ControlToCompare = "ControlToCompare";
	}
}
