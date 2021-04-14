using System;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal interface ISubmissionConfiguration
	{
		IAppConfiguration App { get; }

		void Load();

		void Unload();

		void ConfigUpdate();
	}
}
