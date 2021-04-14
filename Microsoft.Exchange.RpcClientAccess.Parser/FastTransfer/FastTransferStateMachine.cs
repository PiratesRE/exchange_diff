using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct FastTransferStateMachine : IEquatable<FastTransferStateMachine>, IDisposeTrackable, IDisposable
	{
		internal FastTransferStateMachine(IEnumerator<FastTransferStateMachine?> stateMachine)
		{
			this = new FastTransferStateMachine(null, stateMachine);
		}

		internal FastTransferStateMachine(IDisposable supportingObject, IEnumerator<FastTransferStateMachine?> stateMachine)
		{
			this.supportingObject = supportingObject;
			this.stateMachine = (stateMachine ?? FastTransferStateMachine.emptyStateMachine);
			this.disposeTracker = DisposeTracker.Get<FastTransferStateMachine>(default(FastTransferStateMachine));
		}

		private bool IsValid
		{
			get
			{
				return this.stateMachine != null;
			}
		}

		public override int GetHashCode()
		{
			if (!this.IsValid)
			{
				return 0;
			}
			return this.stateMachine.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && this.Equals((FastTransferStateMachine)obj);
		}

		[DebuggerStepThrough]
		public FastTransferStateMachine? Step()
		{
			if (this.stateMachine.MoveNext())
			{
				return new FastTransferStateMachine?(this.stateMachine.Current ?? this);
			}
			return null;
		}

		public override string ToString()
		{
			if (!this.IsValid)
			{
				return base.ToString();
			}
			Type type = this.stateMachine.GetType();
			FieldInfo fieldInfo = type.GetTypeInfo().GetDeclaredField("<>1__state");
			if (fieldInfo == null || fieldInfo.IsStatic || fieldInfo.IsPublic)
			{
				fieldInfo = null;
			}
			Match match = FastTransferStateMachine.GetStateMachineNameParser().Match(type.FullName);
			return string.Format("{0}.{1}@{2}", match.Success ? match.Groups["supportingClass"].Value : "?", match.Success ? match.Groups["method"].Value : "?", (fieldInfo != null) ? fieldInfo.GetValue(this.stateMachine) : "?");
		}

		public bool Equals(FastTransferStateMachine other)
		{
			return this.stateMachine == other.stateMachine;
		}

		public void Dispose()
		{
			Util.DisposeIfPresent(this.stateMachine);
			Util.DisposeIfPresent(this.supportingObject);
			Util.DisposeIfPresent(this.disposeTracker);
		}

		DisposeTracker IDisposeTrackable.GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferStateMachine>(this);
		}

		void IDisposeTrackable.SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private static Regex GetStateMachineNameParser()
		{
			if (FastTransferStateMachine.reStateMachineName == null)
			{
				FastTransferStateMachine.reStateMachineName = new Regex("^[.\\w]+\\.(FastTransfer)? (?'supportingClass'\\w+) \\+ [\\w.<>+]* [.<] (?'method'\\w+) >d__\\w+$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
			}
			return FastTransferStateMachine.reStateMachineName;
		}

		private static IEnumerator<FastTransferStateMachine?> EmptyStateMachine()
		{
			yield break;
		}

		[Conditional("DEBUG")]
		private void CheckValid()
		{
			if (!this.IsValid)
			{
				throw new InvalidOperationException("State machine has not been initialized");
			}
		}

		private static readonly IEnumerator<FastTransferStateMachine?> emptyStateMachine = FastTransferStateMachine.EmptyStateMachine();

		private static Regex reStateMachineName;

		private readonly IDisposable supportingObject;

		private readonly IEnumerator<FastTransferStateMachine?> stateMachine;

		private readonly DisposeTracker disposeTracker;
	}
}
