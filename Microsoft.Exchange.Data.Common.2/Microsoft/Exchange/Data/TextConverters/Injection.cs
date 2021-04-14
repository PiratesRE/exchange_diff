using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Rtf;
using Microsoft.Exchange.Data.TextConverters.Internal.Text;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal abstract class Injection : IDisposable
	{
		public HeaderFooterFormat HeaderFooterFormat
		{
			get
			{
				return this.injectionFormat;
			}
		}

		public bool HaveHead
		{
			get
			{
				return this.injectHead != null;
			}
		}

		public bool HaveTail
		{
			get
			{
				return this.injectTail != null;
			}
		}

		public bool HeadDone
		{
			get
			{
				return this.headInjected;
			}
		}

		public bool TailDone
		{
			get
			{
				return this.tailInjected;
			}
		}

		public abstract void Inject(bool head, TextOutput output);

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public virtual void Reset()
		{
			this.headInjected = false;
			this.tailInjected = false;
		}

		public abstract void CompileForRtf(RtfOutput output);

		public abstract void InjectRtf(bool head, bool immediatelyAfterText);

		public abstract void InjectRtfFonts(int firstAvailableFontHandle);

		public abstract void InjectRtfColors(int nextColorIndex);

		protected HeaderFooterFormat injectionFormat;

		protected string injectHead;

		protected string injectTail;

		protected bool headInjected;

		protected bool tailInjected;

		protected bool testBoundaryConditions;

		protected Stream traceStream;
	}
}
