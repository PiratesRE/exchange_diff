using System;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoRequest
	{
		public PhotoRequest()
		{
			this.PerformanceLogger = NullPerformanceDataLogger.Instance;
		}

		public string TargetSmtpAddress { get; set; }

		public string TargetPrimarySmtpAddress { get; set; }

		public ADObjectId TargetAdObjectId { get; set; }

		public ExchangePrincipal TargetPrincipal { get; set; }

		public ADRecipient TargetRecipient { get; set; }

		public PersonId TargetPersonId { get; set; }

		public StoreObjectId TargetContactId { get; set; }

		public UserPhotoSize Size { get; set; }

		public bool Preview { get; set; }

		public string ETag { get; set; }

		public UploadCommand UploadCommand { get; set; }

		public Stream RawUploadedPhoto { get; set; }

		public PhotoPrincipal Requestor { get; set; }

		public IMailboxSession RequestorMailboxSession { get; set; }

		public ADObjectId UploadTo { get; set; }

		public IPerformanceDataLogger PerformanceLogger { get; set; }

		public Func<ExchangePrincipal, IMailboxSession> HostOwnedTargetMailboxSessionGetter { get; set; }

		public PhotoHandlers HandlersToSkip { get; set; }

		public bool Trace { get; set; }

		public bool? Self { get; set; }

		public bool? IsTargetKnownToBeLocalToThisServer { get; set; }

		public bool? IsTargetMailboxLikelyOnThisServer { get; set; }

		public bool RequestorFromExternalOrganization { get; set; }

		public string ClientRequestId { get; set; }

		public object ClientContextForRemoteForestRequests { get; set; }

		public bool ShouldSkipHandlers(PhotoHandlers handlers)
		{
			return (this.HandlersToSkip & handlers) == handlers;
		}
	}
}
