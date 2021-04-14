using System;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaSignalingHeader : PlatformSignalingHeader
	{
		public UcmaSignalingHeader(string name, string value) : base(name, value)
		{
		}

		public static UcmaSignalingHeader FromSignalingHeader(SignalingHeader h, string context)
		{
			string text = null;
			string value = null;
			UcmaSignalingHeader result;
			try
			{
				text = h.Name;
				value = h.GetValue();
				result = new UcmaSignalingHeader(text, value);
			}
			catch (MessageParsingException)
			{
				throw new InvalidSIPHeaderException(context, text, value);
			}
			return result;
		}

		public override PlatformSipUri ParseUri()
		{
			PlatformSipUri result;
			try
			{
				SignalingHeader signalingHeader = new SignalingHeader(base.Name, base.Value);
				SignalingHeaderParser signalingHeaderParser = new SignalingHeaderParser(signalingHeader);
				result = Platform.Builder.CreateSipUri(signalingHeaderParser.Uri.ToString());
			}
			catch (MessageParsingException ex)
			{
				throw new ArgumentException(ex.Message);
			}
			return result;
		}
	}
}
