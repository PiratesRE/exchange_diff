using System;
using System.Threading;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal class StaticLinkTopics
	{
		public static string GetPatchDownloadUrl(string downloadVersion)
		{
			return string.Format("{0}&version={1}&locale={2}", "http://go.microsoft.com/fwlink/?LinkId=179178", downloadVersion, Thread.CurrentThread.CurrentUICulture.Name);
		}

		public const string MicrosoftDownload = "http://www.microsoft.com/download";

		public const string ToolsWebsite = "http://go.microsoft.com/fwlink/?LinkId=186692";

		public const string MoreCEIPUrl = "http://go.microsoft.com/fwlink/?LinkId=50163";

		public const string NewCertificateRecommendDocuments = "http://go.microsoft.com/fwlink/?LinkID=115184";

		public const string CertificateRequestHelpURL = "http://go.microsoft.com/fwlink/?LinkId=115674";

		public const string SelfSignedCertificateHelpURL = "http://go.microsoft.com/fwlink/?LinkId=119806";

		public const string WildCardCertificateHelpURL = "http://go.microsoft.com/fwlink/?LinkId=115674";

		public const string ExchangeBlogLink = "http://go.microsoft.com/fwlink/?LinkId=92313";

		public const string PatchDownloader = "http://go.microsoft.com/fwlink/?LinkId=179178";

		public const string SubmitfeedbackLink = "http://go.microsoft.com/fwlink/?LinkId=71967";

		public const string CeipProgramLink = "http://go.microsoft.com/fwlink/?LinkID=64471";

		public const string CeipPrivacyStatementLink = "http://go.microsoft.com/fwlink/?linkid=52097";

		public const string ExchangeTechNetLink = "http://go.microsoft.com/fwlink/?LinkId=130589";

		public const string ExchangeOnlineLearnMoreLink = "http://go.microsoft.com/fwlink/?LinkId=187621";

		public const string RemoteMailboxLicenseInformationMoreLink = "http://go.microsoft.com/fwlink/?LinkId=183883";

		public const string IDCRLComponentDownloadLink = "http://go.microsoft.com/fwlink/?LinkId=200953";

		public const string Office365HelpLink = "http://go.microsoft.com/fwlink/p/?LinkId=258351";
	}
}
