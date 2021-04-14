using System;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class StringArrayModalEditor : SimpleEntryEditor<InlineEditor>
	{
		public StringArrayModalEditor()
		{
			base.EditControl.InputWaterMarkText = Strings.InlineEditorInputWaterMarkText;
			base.EditControl.CssClass = "inlineEditorFormlet";
		}
	}
}
