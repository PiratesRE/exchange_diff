using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class JET_SPACEHINTS : IContentEquatable<JET_SPACEHINTS>, IDeepCloneable<JET_SPACEHINTS>
	{
		public int ulInitialDensity
		{
			[DebuggerStepThrough]
			get
			{
				return this.initialDensity;
			}
			set
			{
				this.initialDensity = value;
			}
		}

		public int cbInitial
		{
			[DebuggerStepThrough]
			get
			{
				return this.initialSize;
			}
			set
			{
				this.initialSize = value;
			}
		}

		public SpaceHintsGrbit grbit
		{
			[DebuggerStepThrough]
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		public int ulMaintDensity
		{
			[DebuggerStepThrough]
			get
			{
				return this.maintenanceDensity;
			}
			set
			{
				this.maintenanceDensity = value;
			}
		}

		public int ulGrowth
		{
			[DebuggerStepThrough]
			get
			{
				return this.growthPercent;
			}
			set
			{
				this.growthPercent = value;
			}
		}

		public int cbMinExtent
		{
			[DebuggerStepThrough]
			get
			{
				return this.minimumExtent;
			}
			set
			{
				this.minimumExtent = value;
			}
		}

		public int cbMaxExtent
		{
			[DebuggerStepThrough]
			get
			{
				return this.maximumExtent;
			}
			set
			{
				this.maximumExtent = value;
			}
		}

		public bool ContentEquals(JET_SPACEHINTS other)
		{
			return other != null && (this.ulInitialDensity == other.ulInitialDensity && this.cbInitial == other.cbInitial && this.grbit == other.grbit && this.ulMaintDensity == other.ulMaintDensity && this.ulGrowth == other.ulGrowth && this.cbMinExtent == other.cbMinExtent) && this.cbMaxExtent == other.cbMaxExtent;
		}

		public JET_SPACEHINTS DeepClone()
		{
			return (JET_SPACEHINTS)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SPACEHINTS({0})", new object[]
			{
				this.grbit
			});
		}

		internal NATIVE_SPACEHINTS GetNativeSpaceHints()
		{
			return checked(new NATIVE_SPACEHINTS
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_SPACEHINTS)),
				ulInitialDensity = (uint)this.ulInitialDensity,
				cbInitial = (uint)this.cbInitial,
				grbit = (uint)this.grbit,
				ulMaintDensity = (uint)this.ulMaintDensity,
				ulGrowth = (uint)this.ulGrowth,
				cbMinExtent = (uint)this.cbMinExtent,
				cbMaxExtent = (uint)this.cbMaxExtent
			});
		}

		internal void SetFromNativeSpaceHints(NATIVE_SPACEHINTS value)
		{
			checked
			{
				this.ulInitialDensity = (int)value.ulInitialDensity;
				this.cbInitial = (int)value.cbInitial;
				this.grbit = (SpaceHintsGrbit)value.grbit;
				this.ulMaintDensity = (int)value.ulMaintDensity;
				this.ulGrowth = (int)value.ulGrowth;
				this.cbMinExtent = (int)value.cbMinExtent;
				this.cbMaxExtent = (int)value.cbMaxExtent;
			}
		}

		private int initialDensity;

		private int initialSize;

		private SpaceHintsGrbit options;

		private int maintenanceDensity;

		private int growthPercent;

		private int minimumExtent;

		private int maximumExtent;
	}
}
