using System;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.HttpProxy
{
	internal class OAuthActAsUserAnchorMailbox : UserBasedAnchorMailbox
	{
		public OAuthActAsUserAnchorMailbox(OAuthActAsUser actAsUser, IRequestContext requestContext) : base(AnchorSource.OAuthActAsUser, actAsUser, requestContext)
		{
			this.actAsUser = actAsUser;
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			ADRawEntry result;
			try
			{
				ADRawEntry ret = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => this.actAsUser.ADRawEntry);
				result = base.CheckForNullAndThrowIfApplicable<ADRawEntry>(ret);
			}
			catch (InvalidOAuthTokenException ex)
			{
				throw new HttpException((ex.ErrorCategory == OAuthErrorCategory.InternalError) ? 500 : 401, string.Empty, ex);
			}
			return result;
		}

		private readonly OAuthActAsUser actAsUser;
	}
}
