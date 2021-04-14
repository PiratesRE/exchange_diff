using System;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace System.Diagnostics.Contracts
{
	[__DynamicallyInvokable]
	public sealed class ContractFailedEventArgs : EventArgs
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[__DynamicallyInvokable]
		public ContractFailedEventArgs(ContractFailureKind failureKind, string message, string condition, Exception originalException)
		{
			this._failureKind = failureKind;
			this._message = message;
			this._condition = condition;
			this._originalException = originalException;
		}

		[__DynamicallyInvokable]
		public string Message
		{
			[__DynamicallyInvokable]
			get
			{
				return this._message;
			}
		}

		[__DynamicallyInvokable]
		public string Condition
		{
			[__DynamicallyInvokable]
			get
			{
				return this._condition;
			}
		}

		[__DynamicallyInvokable]
		public ContractFailureKind FailureKind
		{
			[__DynamicallyInvokable]
			get
			{
				return this._failureKind;
			}
		}

		[__DynamicallyInvokable]
		public Exception OriginalException
		{
			[__DynamicallyInvokable]
			get
			{
				return this._originalException;
			}
		}

		[__DynamicallyInvokable]
		public bool Handled
		{
			[__DynamicallyInvokable]
			get
			{
				return this._handled;
			}
		}

		[SecurityCritical]
		[__DynamicallyInvokable]
		public void SetHandled()
		{
			this._handled = true;
		}

		[__DynamicallyInvokable]
		public bool Unwind
		{
			[__DynamicallyInvokable]
			get
			{
				return this._unwind;
			}
		}

		[SecurityCritical]
		[__DynamicallyInvokable]
		public void SetUnwind()
		{
			this._unwind = true;
		}

		private ContractFailureKind _failureKind;

		private string _message;

		private string _condition;

		private Exception _originalException;

		private bool _handled;

		private bool _unwind;

		internal Exception thrownDuringHandler;
	}
}
