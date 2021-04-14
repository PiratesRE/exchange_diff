using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	internal class SuppressingPiiContext : DisposeTrackableBase
	{
		private SuppressingPiiContext(bool needPiiSuppression, PiiMap piiMap)
		{
			this.needPiiSuppression = needPiiSuppression;
			this.piiMap = piiMap;
			this.previousContext = SuppressingPiiContext.currentContext;
			SuppressingPiiContext.currentContext = this;
		}

		public static bool NeedPiiSuppression
		{
			get
			{
				SuppressingPiiContext suppressingPiiContext = SuppressingPiiContext.currentContext;
				return suppressingPiiContext != null && suppressingPiiContext.needPiiSuppression;
			}
		}

		public static PiiMap PiiMap
		{
			get
			{
				SuppressingPiiContext suppressingPiiContext = SuppressingPiiContext.currentContext;
				if (suppressingPiiContext == null)
				{
					return null;
				}
				return suppressingPiiContext.piiMap;
			}
		}

		internal static IDisposable Create(bool needPiiSuppression, PiiMap piiMap)
		{
			return new SuppressingPiiContext(needPiiSuppression, piiMap);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				SuppressingPiiContext.currentContext = this.previousContext;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SuppressingPiiContext>(this);
		}

		[ThreadStatic]
		private static SuppressingPiiContext currentContext;

		private readonly SuppressingPiiContext previousContext;

		private readonly bool needPiiSuppression;

		private readonly PiiMap piiMap;
	}
}
