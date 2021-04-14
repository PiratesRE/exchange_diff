using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class PersonalAutoAttendantListToolbar : Toolbar
	{
		public PersonalAutoAttendantListToolbar() : base(ToolbarType.Options)
		{
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.NewPersonalAutoAttendant);
			base.RenderButton(ToolbarButtons.EditTextOnly);
			base.RenderButton(ToolbarButtons.DeleteWithText);
			base.RenderButton(ToolbarButtons.MoveUp);
			base.RenderButton(ToolbarButtons.MoveDown);
		}
	}
}
