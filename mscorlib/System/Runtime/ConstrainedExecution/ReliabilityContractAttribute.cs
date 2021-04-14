using System;

namespace System.Runtime.ConstrainedExecution
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Interface, Inherited = false)]
	public sealed class ReliabilityContractAttribute : Attribute
	{
		public ReliabilityContractAttribute(Consistency consistencyGuarantee, Cer cer)
		{
			this._consistency = consistencyGuarantee;
			this._cer = cer;
		}

		public Consistency ConsistencyGuarantee
		{
			get
			{
				return this._consistency;
			}
		}

		public Cer Cer
		{
			get
			{
				return this._cer;
			}
		}

		private Consistency _consistency;

		private Cer _cer;
	}
}
