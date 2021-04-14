using System;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal static class PropertyTagExtensions
	{
		public static bool IsBody(this PropertyId propertyId)
		{
			return propertyId == PropertyId.Body;
		}

		public static bool IsRtfCompressed(this PropertyId propertyId)
		{
			return propertyId == PropertyId.RtfCompressed;
		}

		public static bool IsHtml(this PropertyId propertyId)
		{
			return propertyId == PropertyId.Html;
		}

		public static bool IsRtfInSync(this PropertyId propertyId)
		{
			return propertyId == PropertyId.RtfInSync;
		}

		public static bool IsBody(this PropertyTag propertyTag)
		{
			return propertyTag.PropertyId.IsBody() && propertyTag.IsStringProperty;
		}

		public static bool IsRtfCompressed(this PropertyTag propertyTag)
		{
			return propertyTag == PropertyTag.RtfCompressed;
		}

		public static bool IsHtml(this PropertyTag propertyTag)
		{
			return propertyTag == PropertyTag.Html;
		}

		public static bool IsBodyProperty(this PropertyTag propertyTag)
		{
			return propertyTag.PropertyId.IsBody() || propertyTag.PropertyId.IsRtfCompressed() || propertyTag.PropertyId.IsHtml();
		}
	}
}
