using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal sealed class WinFormsCommandInteractionHandler : CommandInteractionHandler
	{
		internal IUIService UIService
		{
			get
			{
				return this.uiService;
			}
		}

		public WinFormsCommandInteractionHandler(IUIService uiService)
		{
			if (uiService == null)
			{
				throw new ArgumentNullException("uiService");
			}
			this.uiService = uiService;
		}

		public override ConfirmationChoice ShowConfirmationDialog(string message, ConfirmationChoice defaultChoice)
		{
			Control control = this.uiService.GetDialogOwnerWindow() as Control;
			if (control != null && control.InvokeRequired)
			{
				return (ConfirmationChoice)control.Invoke(new WinFormsCommandInteractionHandler.ShowConfirmationDialogDelegate(this.ShowConfirmationDialog), new object[]
				{
					message,
					defaultChoice
				});
			}
			ConfirmationChoice userChoice;
			using (PromptForChoicesDialog promptForChoicesDialog = new PromptForChoicesDialog(message, defaultChoice))
			{
				this.uiService.ShowDialog(promptForChoicesDialog);
				userChoice = promptForChoicesDialog.UserChoice;
			}
			return userChoice;
		}

		private IUIService uiService;

		private delegate ConfirmationChoice ShowConfirmationDialogDelegate(string message, ConfirmationChoice defaultChoice);
	}
}
