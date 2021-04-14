using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class JET_TESTHOOKEVICTCACHE : IContentEquatable<JET_TESTHOOKEVICTCACHE>, IDeepCloneable<JET_TESTHOOKEVICTCACHE>
	{
		public JET_DBID ulTargetContext
		{
			[DebuggerStepThrough]
			get
			{
				return this.targetContext;
			}
			set
			{
				this.targetContext = value;
			}
		}

		public int ulTargetData
		{
			[DebuggerStepThrough]
			get
			{
				return this.targetData;
			}
			set
			{
				this.targetData = value;
			}
		}

		public EvictCacheGrbit grbit
		{
			[DebuggerStepThrough]
			get
			{
				return this.evictGrbit;
			}
			set
			{
				this.evictGrbit = value;
			}
		}

		public bool ContentEquals(JET_TESTHOOKEVICTCACHE other)
		{
			return other != null && (this.ulTargetContext == other.ulTargetContext && this.ulTargetData == other.ulTargetData) && this.grbit == other.grbit;
		}

		public JET_TESTHOOKEVICTCACHE DeepClone()
		{
			return (JET_TESTHOOKEVICTCACHE)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_TESTHOOKEVICTCACHE({0}:{1}:{2})", new object[]
			{
				this.ulTargetContext,
				this.ulTargetData,
				this.grbit
			});
		}

		internal NATIVE_TESTHOOKEVICTCACHE GetNativeTestHookEvictCache()
		{
			return new NATIVE_TESTHOOKEVICTCACHE
			{
				cbStruct = checked((uint)Marshal.SizeOf(typeof(NATIVE_TESTHOOKEVICTCACHE))),
				ulTargetContext = (IntPtr)((long)((ulong)this.ulTargetContext.Value)),
				ulTargetData = (IntPtr)this.ulTargetData,
				grbit = checked((uint)this.grbit)
			};
		}

		private JET_DBID targetContext;

		private int targetData;

		private EvictCacheGrbit evictGrbit;
	}
}
