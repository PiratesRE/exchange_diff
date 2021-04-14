using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class JET_TESTHOOKTESTINJECTION : IContentEquatable<JET_TESTHOOKTESTINJECTION>, IDeepCloneable<JET_TESTHOOKTESTINJECTION>
	{
		public uint ulID
		{
			[DebuggerStepThrough]
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public IntPtr pv
		{
			[DebuggerStepThrough]
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
			}
		}

		public TestHookInjectionType type
		{
			[DebuggerStepThrough]
			get
			{
				return this.injectionType;
			}
			set
			{
				this.injectionType = value;
			}
		}

		public uint ulProbability
		{
			[DebuggerStepThrough]
			get
			{
				return this.injectionProbability;
			}
			set
			{
				this.injectionProbability = value;
			}
		}

		public TestInjectionGrbit grbit
		{
			[DebuggerStepThrough]
			get
			{
				return this.injectionGrbit;
			}
			set
			{
				this.injectionGrbit = value;
			}
		}

		public bool ContentEquals(JET_TESTHOOKTESTINJECTION other)
		{
			if (other == null)
			{
				return false;
			}
			bool flag = this.ulID == other.ulID && this.type == other.type && this.ulProbability == other.ulProbability && this.grbit == other.grbit;
			if (flag)
			{
				switch (this.type)
				{
				case TestHookInjectionType.Fault:
				case TestHookInjectionType.ConfigOverride:
					flag = (this.pv == other.pv);
					break;
				}
			}
			return flag;
		}

		public JET_TESTHOOKTESTINJECTION DeepClone()
		{
			JET_TESTHOOKTESTINJECTION jet_TESTHOOKTESTINJECTION = (JET_TESTHOOKTESTINJECTION)base.MemberwiseClone();
			switch (this.type)
			{
			case TestHookInjectionType.Fault:
			case TestHookInjectionType.ConfigOverride:
				jet_TESTHOOKTESTINJECTION.pv = this.pv;
				break;
			}
			return jet_TESTHOOKTESTINJECTION;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_TESTHOOKTESTINJECTION({0}:{1}:{2})", new object[]
			{
				this.ulID,
				this.type,
				this.pv
			});
		}

		internal NATIVE_TESTHOOKTESTINJECTION GetNativeTestHookInjection()
		{
			return new NATIVE_TESTHOOKTESTINJECTION
			{
				cbStruct = checked((uint)Marshal.SizeOf(typeof(NATIVE_TESTHOOKTESTINJECTION))),
				ulID = this.ulID,
				pv = this.pv,
				type = (int)this.type,
				ulProbability = this.ulProbability,
				grbit = (uint)this.grbit
			};
		}

		private uint id;

		private IntPtr context;

		private TestHookInjectionType injectionType;

		private uint injectionProbability;

		private TestInjectionGrbit injectionGrbit;
	}
}
