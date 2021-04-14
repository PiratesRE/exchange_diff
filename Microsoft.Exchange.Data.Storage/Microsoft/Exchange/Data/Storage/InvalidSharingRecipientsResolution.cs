using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class InvalidSharingRecipientsResolution
	{
		public InvalidSharingRecipientsResolutionType Resolution
		{
			get
			{
				return this.resolution;
			}
		}

		public StoreObjectId FolderId
		{
			get
			{
				InvalidSharingRecipientsResolutionType invalidSharingRecipientsResolutionType = this.Resolution;
				if (invalidSharingRecipientsResolutionType == InvalidSharingRecipientsResolutionType.PublishAndTryAgain)
				{
					return this.folderId;
				}
				throw new InvalidOperationException("FolderId");
			}
		}

		public string BrowseUrl
		{
			get
			{
				InvalidSharingRecipientsResolutionType invalidSharingRecipientsResolutionType = this.Resolution;
				if (invalidSharingRecipientsResolutionType == InvalidSharingRecipientsResolutionType.SendPublishLinks)
				{
					return this.browseUrl;
				}
				throw new InvalidOperationException("BrowseUrl");
			}
		}

		public string ICalUrl
		{
			get
			{
				InvalidSharingRecipientsResolutionType invalidSharingRecipientsResolutionType = this.Resolution;
				if (invalidSharingRecipientsResolutionType == InvalidSharingRecipientsResolutionType.SendPublishLinks)
				{
					return this.iCalUrl;
				}
				throw new InvalidOperationException("ICalUrl");
			}
		}

		internal InvalidSharingRecipientsResolution(StoreObjectId folderId)
		{
			Util.ThrowOnNullArgument(folderId, "folderId");
			this.resolution = InvalidSharingRecipientsResolutionType.PublishAndTryAgain;
			this.folderId = folderId;
		}

		internal InvalidSharingRecipientsResolution(string browseUrl, string iCalUrl)
		{
			Util.ThrowOnNullOrEmptyArgument(browseUrl, "browseUrl");
			Util.ThrowOnNullOrEmptyArgument(iCalUrl, "iCalUrl");
			this.resolution = InvalidSharingRecipientsResolutionType.SendPublishLinks;
			this.browseUrl = browseUrl;
			this.iCalUrl = iCalUrl;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("Resolution: " + this.Resolution.ToString());
			switch (this.Resolution)
			{
			case InvalidSharingRecipientsResolutionType.PublishAndTryAgain:
				stringBuilder.Append(", FolderId:" + this.FolderId.ToBase64String());
				break;
			case InvalidSharingRecipientsResolutionType.SendPublishLinks:
				stringBuilder.Append(", BrowseUrl:" + this.BrowseUrl);
				stringBuilder.Append(", ICalUrl:" + this.ICalUrl);
				break;
			default:
				stringBuilder.Append(" <Unknown Resolution Value>");
				break;
			}
			return stringBuilder.ToString();
		}

		private readonly InvalidSharingRecipientsResolutionType resolution;

		private readonly StoreObjectId folderId;

		private readonly string browseUrl;

		private readonly string iCalUrl;
	}
}
