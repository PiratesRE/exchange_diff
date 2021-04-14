using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("PopupForm", "Microsoft.Exchange.Management.ControlPanel.Client.Pickers.js")]
	public class PopupForm : BaseForm
	{
		public PopupForm()
		{
			base.CommitButtonText = Strings.OkButtonText;
		}

		internal override bool PassingDataOnClient
		{
			get
			{
				return true;
			}
		}

		public string ConverterType { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (!string.IsNullOrEmpty(this.ConverterType))
			{
				descriptor.AddScriptProperty("Converter", "new " + this.ConverterType + "()");
			}
		}
	}
}
