using System;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class WarningReportEventArgs : RunGuidEventArgs, IDisposable
	{
		public WarningReportEventArgs(Guid guid, string warning, int objectIndex, MonadCommand command) : base(guid)
		{
			if (warning == null)
			{
				throw new ArgumentNullException("warning");
			}
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			this.SetWarningMessageHelpUrl(warning);
			this.objectIndex = objectIndex;
			this.command = command;
		}

		public string WarningMessage
		{
			get
			{
				return this.warningMessage;
			}
		}

		public string HelpUrl
		{
			get
			{
				return this.helpUrl;
			}
		}

		public int ObjectIndex
		{
			get
			{
				return this.objectIndex;
			}
		}

		public MonadCommand Command
		{
			get
			{
				return this.command;
			}
		}

		public static string CombineWarningMessageHelpUrl(string warningMessage, string helpUrl)
		{
			if (warningMessage == null)
			{
				throw new ArgumentNullException("warningMessage");
			}
			if (helpUrl == null)
			{
				throw new ArgumentNullException("helpUrl");
			}
			return string.Format("{0}{1}{2}", warningMessage, " For more information see the help link: ", helpUrl);
		}

		public void Dispose()
		{
		}

		private void SetWarningMessageHelpUrl(string warning)
		{
			int num = warning.IndexOf(" For more information see the help link: ");
			if (num >= 0)
			{
				this.warningMessage = warning.Substring(0, num);
				this.helpUrl = warning.Substring(num + " For more information see the help link: ".Length);
				return;
			}
			this.warningMessage = warning;
			this.helpUrl = string.Empty;
		}

		private const string HelpUrlSeparator = " For more information see the help link: ";

		private string warningMessage;

		private string helpUrl;

		private int objectIndex;

		private MonadCommand command;
	}
}
