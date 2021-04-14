using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ControlValueProperty("Value")]
	[ClientScriptResource("FormletControlBase", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ToolboxData("<{0}:FormletControlBase runat=server></{0}:FormletControlBase>")]
	public abstract class FormletControlBase<P, E> : ScriptControlBase, INamingContainer where P : FormletParameter where E : Control, new()
	{
		protected P Parameter { get; set; }

		protected E Editor { get; set; }

		public FormletControlBase() : base(HtmlTextWriterTag.Div)
		{
			if (string.IsNullOrEmpty(this.NoSelectionText))
			{
				this.NoSelectionText = ClientStrings.SelectOneLink;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (string.IsNullOrEmpty(this.ID))
			{
				this.ID = typeof(E).ToString();
			}
			Control control = this.Page.FindControl(this.ID);
			if (control != null)
			{
				this.Editor = (control as E);
			}
			if (this.Editor == null)
			{
				this.Editor = Activator.CreateInstance<E>();
				this.Controls.Add(this.Editor);
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddScriptProperty("EditorParameter", this.Parameter.ToJsonString(null));
		}

		public string FormletDialogTitle { get; set; }

		public string NoSelectionText { get; set; }
	}
}
