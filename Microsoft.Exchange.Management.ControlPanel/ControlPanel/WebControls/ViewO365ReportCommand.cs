using System;
using System.Configuration;
using System.Drawing;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class ViewO365ReportCommand : WizardCommand
	{
		public ViewO365ReportCommand() : this(string.Empty, CommandSprite.SpriteId.NONE)
		{
		}

		public ViewO365ReportCommand(string text, CommandSprite.SpriteId imageID) : base(text, imageID)
		{
			this.DialogSize = ViewO365ReportCommand.DefaultDialogSize;
			this.OnClientClick = "ViewO365ReportCommandHandler";
			base.ImageAltText = Strings.ViewDetailsCommandText;
			this.BypassUrlCheck = true;
			base.Visible = !string.IsNullOrEmpty(ViewO365ReportCommand.o365Url);
			this.ServiceId = "Exchange";
		}

		public override string NavigateUrl
		{
			get
			{
				return ViewO365ReportCommand.o365Url + string.Format("Reports/ReportDetails.aspx?ServiceId={0}&CategoryId={1}&ReportId={2}", this.ServiceId, this.CategoryId, this.ReportId);
			}
			set
			{
				base.NavigateUrl = string.Empty;
			}
		}

		public string ReportId { get; set; }

		public string CategoryId { get; set; }

		public string ServiceId { get; set; }

		private const int DefaultViewDetailWidth = 800;

		private const int DefaultViewDetailHeight = 600;

		private static readonly Size DefaultDialogSize = new Size(800, 600);

		private static readonly string o365Url = ConfigurationManager.AppSettings["O365Url"];
	}
}
