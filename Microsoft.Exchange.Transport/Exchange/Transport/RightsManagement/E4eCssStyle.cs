using System;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal abstract class E4eCssStyle
	{
		internal abstract string ArrowImgBase64 { get; }

		internal abstract string LockImgBase64 { get; }

		internal abstract string RegularTextStyle { get; }

		internal abstract string DisclaimerTextStyle { get; }

		internal abstract string HostedTextStyle { get; }

		internal abstract string AnchorTagStyle { get; }

		internal abstract string EmailTextAnchorStyle { get; }

		internal abstract string LogoSizeStyle { get; }

		internal abstract string LockSizeStyle { get; }

		internal abstract string BoldTextStyle { get; }

		internal abstract string HeaderDivStyle { get; }

		internal abstract string HeaderTextStyle { get; }

		internal abstract string ViewMessageOTPButtonStyle { get; }

		internal abstract string ViewportMetaTag { get; }

		internal abstract string MainContentDivStyle { get; }

		internal abstract string EncryptedMessageDivStyle { get; }

		internal abstract string ViewMessageButtonDivStyle { get; }

		internal abstract string HostedMessageTableStyle { get; }

		internal abstract string EmailAddressSpanStyle { get; }

		internal abstract string ButtonStyle(string base64Image);
	}
}
