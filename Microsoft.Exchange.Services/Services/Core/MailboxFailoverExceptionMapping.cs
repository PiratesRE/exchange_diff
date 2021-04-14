using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class MailboxFailoverExceptionMapping : StaticExceptionMapping
	{
		public MailboxFailoverExceptionMapping(Type exceptionType) : base(exceptionType, ExceptionMappingBase.Attributes.StopsBatchProcessing, ResponseCodeType.ErrorMailboxStoreUnavailable, CoreResources.IDs.ErrorMailboxStoreUnavailable)
		{
		}

		protected override void DoServiceErrorPostProcessing(LocalizedException exception, ServiceError error)
		{
			MailboxFailoverExceptionMapping.WriteFailoverHeaders(exception);
			if (exception is MailboxInSiteFailoverException || exception is MailboxOfflineException)
			{
				error.IsTransient = true;
			}
		}

		internal static void WriteFailoverHeaders(LocalizedException localizedException)
		{
			if (Global.WriteFailoverTypeHeader)
			{
				MailboxOfflineException ex = localizedException as MailboxOfflineException;
				if (ex != null)
				{
					EWSSettings.FailoverType = "Offline";
					return;
				}
				MailboxInSiteFailoverException ex2 = localizedException as MailboxInSiteFailoverException;
				if (ex2 != null)
				{
					EWSSettings.FailoverType = "In-Site";
					return;
				}
				MailboxCrossSiteFailoverException ex3 = localizedException as MailboxCrossSiteFailoverException;
				if (ex3 != null)
				{
					EWSSettings.FailoverType = "Cross-Site@" + ex3.DatabaseLocationInfo.ServerFqdn;
				}
			}
		}
	}
}
