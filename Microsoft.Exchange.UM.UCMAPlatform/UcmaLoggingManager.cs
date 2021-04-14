using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaLoggingManager : UMLoggingManager
	{
		public UcmaLoggingManager(DiagnosticHelper diag)
		{
			this.diag = diag;
		}

		internal override void EnterTurn(string turnName)
		{
			this.diag.Trace("Enter turn: {0}", new object[]
			{
				turnName
			});
		}

		internal override void ExitTurn()
		{
			this.diag.Trace("Exit turn", new object[0]);
		}

		internal override void EnterTask(string name)
		{
			this.diag.Trace("Enter task: {0}", new object[]
			{
				name
			});
		}

		internal override void ExitTask(UMNavigationState state, string message)
		{
			this.diag.Trace("Exit task state={0} message={1}", new object[]
			{
				state,
				message
			});
		}

		internal override void LogApplicationInformation(string format, params object[] args)
		{
			this.diag.Trace(format, args);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UcmaLoggingManager>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		private DiagnosticHelper diag;
	}
}
