using System;
using System.ComponentModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class ErrorInformationBaseExtensions
	{
		public static InfoCore ToInfo(this ErrorInformationBase error)
		{
			InfoCore infoCore = new InfoCore();
			if (error.Exception is WarningException)
			{
				infoCore.MessageBoxType = ModalDialogType.Warning;
				infoCore.JsonTitle = ClientStrings.Warning;
			}
			else
			{
				infoCore.MessageBoxType = ModalDialogType.Error;
				infoCore.JsonTitle = ClientStrings.Error;
				infoCore.StackTrace = error.CallStack;
			}
			if (error.Exception is LocalizedException)
			{
				infoCore.HelpUrl = HelpUtil.BuildErrorAssistanceUrl(error.Exception as LocalizedException);
				infoCore.Help = ClientStrings.ClickHereForHelp;
			}
			infoCore.Message = error.Message;
			infoCore.Details = ((error.Exception.InnerException != null) ? error.Exception.InnerException.GetFullMessage() : string.Empty);
			return infoCore;
		}

		public static InfoCore[] ToInfos(this ErrorInformationBase[] errors)
		{
			return Array.ConvertAll<ErrorInformationBase, InfoCore>(errors, (ErrorInformationBase x) => x.ToInfo());
		}
	}
}
