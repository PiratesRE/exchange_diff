using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using Microsoft.Isam.Esent.Interop.Vista;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Isam.Esent.Interop
{
	public class Instance : SafeHandleZeroOrMinusOneIsInvalid
	{
		[SecurityPermission(SecurityAction.LinkDemand)]
		public Instance(string name) : this(name, name, TermGrbit.None)
		{
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
		public Instance(string name, string displayName) : this(name, displayName, TermGrbit.None)
		{
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
		public Instance(string name, string displayName, TermGrbit termGrbit) : base(true)
		{
			this.name = name;
			this.displayName = displayName;
			this.termGrbit = termGrbit;
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_INSTANCE instance;
			try
			{
				base.SetHandle(JET_INSTANCE.Nil.Value);
			}
			finally
			{
				Api.JetCreateInstance2(out instance, this.name, this.displayName, CreateInstanceGrbit.None);
				base.SetHandle(instance.Value);
			}
			this.parameters = new InstanceParameters(instance);
		}

		public JET_INSTANCE JetInstance
		{
			[SecurityPermission(SecurityAction.LinkDemand)]
			get
			{
				this.CheckObjectIsNotDisposed();
				return this.CreateInstanceFromHandle();
			}
		}

		public InstanceParameters Parameters
		{
			[SecurityPermission(SecurityAction.LinkDemand)]
			get
			{
				this.CheckObjectIsNotDisposed();
				return this.parameters;
			}
		}

		public TermGrbit TermGrbit
		{
			[SecurityPermission(SecurityAction.LinkDemand)]
			get
			{
				this.CheckObjectIsNotDisposed();
				return this.termGrbit;
			}
			[SecurityPermission(SecurityAction.LinkDemand)]
			set
			{
				this.CheckObjectIsNotDisposed();
				this.termGrbit = value;
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
		public static implicit operator JET_INSTANCE(Instance instance)
		{
			return instance.JetInstance;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[]
			{
				this.displayName,
				this.name
			});
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
		public void Init()
		{
			this.Init(InitGrbit.None);
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
		public void Init(InitGrbit grbit)
		{
			this.CheckObjectIsNotDisposed();
			JET_INSTANCE jetInstance = this.JetInstance;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				Api.JetInit2(ref jetInstance, grbit);
			}
			finally
			{
				base.SetHandle(jetInstance.Value);
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
		public void Init(JET_RSTINFO recoveryOptions, InitGrbit grbit)
		{
			this.CheckObjectIsNotDisposed();
			JET_INSTANCE jetInstance = this.JetInstance;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				VistaApi.JetInit3(ref jetInstance, recoveryOptions, grbit);
			}
			finally
			{
				base.SetHandle(jetInstance.Value);
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
		public void Term()
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				try
				{
					Api.JetTerm2(this.JetInstance, this.termGrbit);
				}
				catch (EsentDirtyShutdownException)
				{
					base.SetHandleAsInvalid();
					throw;
				}
				base.SetHandleAsInvalid();
			}
		}

		protected override bool ReleaseHandle()
		{
			JET_INSTANCE instance = this.CreateInstanceFromHandle();
			return 0 == Api.Impl.JetTerm2(instance, this.termGrbit);
		}

		private JET_INSTANCE CreateInstanceFromHandle()
		{
			return new JET_INSTANCE
			{
				Value = this.handle
			};
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
		private void CheckObjectIsNotDisposed()
		{
			if (this.IsInvalid || base.IsClosed)
			{
				throw new ObjectDisposedException("Instance");
			}
		}

		private readonly InstanceParameters parameters;

		private readonly string name;

		private readonly string displayName;

		private TermGrbit termGrbit;
	}
}
