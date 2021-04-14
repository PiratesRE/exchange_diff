using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("SimpleEntryEditor", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ParseChildren(true)]
	[ToolboxData("<{0}:SimpleEntryEditor runat=server></{0}:SimpleEntryEditor>")]
	public abstract class SimpleEntryEditor<T> : ScriptControlBase where T : Control, new()
	{
		protected SimpleEntryEditor() : base(HtmlTextWriterTag.Div)
		{
			base.Style.Add(HtmlTextWriterStyle.Display, "none");
			this.Controls.Add(this.EditControl);
			EncodingLabel encodingLabel = new EncodingLabel();
			encodingLabel.ID = "label";
			encodingLabel.CssClass = "HiddenForScreenReader";
			T t = this.EditControl;
			t.Controls.Add(encodingLabel);
		}

		[Browsable(false)]
		public new double Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		[Browsable(false)]
		public string EditControlID
		{
			get
			{
				T t = this.EditControl;
				return t.ClientID;
			}
		}

		[Browsable(false)]
		public string TypeName
		{
			get
			{
				return base.GetType().Name;
			}
		}

		[Browsable(false)]
		public T EditControl
		{
			get
			{
				return this.editControl;
			}
		}

		protected sealed override List<ScriptDescriptor> CreateScriptDescriptors()
		{
			List<ScriptDescriptor> list = new List<ScriptDescriptor>();
			ScriptComponentDescriptor item = new ScriptComponentDescriptor(this.ClientControlType);
			list.Add(item);
			return list;
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("Width", this.Width);
			descriptor.AddProperty("EditControlID", this.EditControlID, true);
			descriptor.AddProperty("TypeName", this.TypeName, true);
		}

		private double width = -1.0;

		private T editControl = Activator.CreateInstance<T>();
	}
}
