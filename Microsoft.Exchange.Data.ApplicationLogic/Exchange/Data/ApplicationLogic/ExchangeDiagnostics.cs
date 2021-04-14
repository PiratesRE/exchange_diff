using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class ExchangeDiagnostics : DisposeTrackableBase
	{
		private IDiagnosable RegisteredDiagnosable { get; set; }

		public ExchangeDiagnostics(IDiagnosable diagnosable)
		{
			ProcessAccessManager.RegisterComponent(diagnosable);
			this.RegisteredDiagnosable = diagnosable;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.RegisteredDiagnosable != null)
				{
					ProcessAccessManager.UnregisterComponent(this.RegisteredDiagnosable);
				}
				this.RegisteredDiagnosable = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExchangeDiagnostics>(this);
		}
	}
}
