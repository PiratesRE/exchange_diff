using System;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public static class ResourceTarget
	{
		public static ResourceTarget.Filter Any
		{
			get
			{
				return (UserAgent userAgent) => true;
			}
		}

		public static ResourceTarget.Filter MouseOnly
		{
			get
			{
				return (UserAgent userAgent) => userAgent.Layout == LayoutType.Mouse;
			}
		}

		public static ResourceTarget.Filter TouchOnly
		{
			get
			{
				return (UserAgent userAgent) => userAgent.Layout == LayoutType.TouchNarrow || userAgent.Layout == LayoutType.TouchWide;
			}
		}

		public static ResourceTarget.Filter NarrowOnly
		{
			get
			{
				return (UserAgent userAgent) => userAgent.Layout == LayoutType.TouchNarrow;
			}
		}

		public static ResourceTarget.Filter WideOnly
		{
			get
			{
				return (UserAgent userAgent) => userAgent.Layout == LayoutType.TouchWide;
			}
		}

		public static ResourceTarget.Filter NarrowHighResolution
		{
			get
			{
				return (UserAgent userAgent) => userAgent.Layout == LayoutType.TouchNarrow;
			}
		}

		public static ResourceTarget.Filter WideHighResolution
		{
			get
			{
				return (UserAgent userAgent) => userAgent.Layout == LayoutType.TouchWide;
			}
		}

		public delegate bool Filter(UserAgent userAgent);
	}
}
