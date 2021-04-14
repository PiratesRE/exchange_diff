using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public sealed class OwaEventSegmentationAttribute : Attribute
	{
		public OwaEventSegmentationAttribute(Feature features)
		{
			this.segmentationFlags = (ulong)features;
		}

		internal ulong SegmentationFlags
		{
			get
			{
				return this.segmentationFlags;
			}
		}

		private ulong segmentationFlags;
	}
}
