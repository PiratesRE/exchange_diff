using System;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public class AttachmentAddResult
	{
		public static AttachmentAddResult NoError
		{
			get
			{
				return new AttachmentAddResult(AttachmentAddResultCode.NoError, null);
			}
		}

		public AttachmentAddResultCode ResultCode
		{
			get
			{
				return this.resultCode;
			}
		}

		public SanitizedHtmlString Message
		{
			get
			{
				return this.message;
			}
		}

		private AttachmentAddResult(AttachmentAddResultCode resultCode, SanitizedHtmlString message)
		{
			this.SetResult(resultCode, message);
		}

		public void SetResult(AttachmentAddResultCode resultCode, SanitizedHtmlString message)
		{
			if (resultCode != AttachmentAddResultCode.NoError && SanitizedStringBase<OwaHtml>.IsNullOrEmpty(message))
			{
				throw new ArgumentException("Must specify a message if result code is not NoError.");
			}
			this.resultCode = resultCode;
			this.message = message;
		}

		private AttachmentAddResultCode resultCode;

		private SanitizedHtmlString message;
	}
}
