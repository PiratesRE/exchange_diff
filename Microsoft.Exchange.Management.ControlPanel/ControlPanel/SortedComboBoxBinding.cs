using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SortedComboBoxBinding : ClientControlBinding
	{
		public SortedComboBoxBinding(Control control, string clientPropertyName, SortDirection sortedDirection) : base(control, clientPropertyName)
		{
			this.SortedDirection = sortedDirection;
		}

		public SortDirection SortedDirection { get; set; }

		protected override string ToJavaScriptWhenVisible(IControlResolver resolver)
		{
			return string.Format("new SortedComboBoxBinding('{0}','{1}',{2})", this.ClientID, base.ClientPropertyName, (this.SortedDirection == SortDirection.Ascending).ToString().ToLowerInvariant());
		}
	}
}
