using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class SetDisplayPictureResult
	{
		public static SetDisplayPictureResult NoError
		{
			get
			{
				return new SetDisplayPictureResult(SetDisplayPictureResultCode.NoError);
			}
		}

		public SetDisplayPictureResultCode ResultCode
		{
			get
			{
				return this.resultCode;
			}
		}

		public SanitizedHtmlString ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		public string ImageSmallHtml
		{
			get
			{
				return this.imageSmallHtml;
			}
		}

		public string ImageLargeHtml
		{
			get
			{
				return this.imageLargeHtml;
			}
		}

		private SetDisplayPictureResult(SetDisplayPictureResultCode resultCode)
		{
			this.resultCode = resultCode;
			this.errorMessage = new SanitizedHtmlString();
		}

		public void SetErrorResult(SetDisplayPictureResultCode resultCode, SanitizedHtmlString errorMessage)
		{
			if (resultCode != SetDisplayPictureResultCode.NoError && SanitizedStringBase<OwaHtml>.IsNullOrEmpty(errorMessage))
			{
				throw new ArgumentException("Must specify an error message if result code is not NoError.");
			}
			this.resultCode = resultCode;
			this.errorMessage = errorMessage;
		}

		public void SetSuccessResult(string imageSmallHtml, string imageLargeHtml)
		{
			if (string.IsNullOrEmpty(imageSmallHtml))
			{
				throw new ArgumentException("Must specify small image html for successful result.");
			}
			if (string.IsNullOrEmpty(imageLargeHtml))
			{
				throw new ArgumentException("Must specify large image html for successful result.");
			}
			this.imageSmallHtml = imageSmallHtml;
			this.imageLargeHtml = imageLargeHtml;
		}

		private SetDisplayPictureResultCode resultCode;

		private SanitizedHtmlString errorMessage;

		private string imageSmallHtml = string.Empty;

		private string imageLargeHtml = string.Empty;
	}
}
