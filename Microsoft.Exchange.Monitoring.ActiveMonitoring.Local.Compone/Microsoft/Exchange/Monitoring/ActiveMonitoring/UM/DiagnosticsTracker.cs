using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class DiagnosticsTracker
	{
		public IList<DiagnosticsItemBase> Items
		{
			get
			{
				return this.items;
			}
		}

		public DiagnosticsItemBase TrackDiagnostics(string header)
		{
			DiagnosticsItemBase diagnosticsItemBase = DiagnosticsItemFactory.Create(header);
			if (diagnosticsItemBase.IsValid)
			{
				this.items.Add(diagnosticsItemBase);
			}
			return diagnosticsItemBase;
		}

		public DiagnosticsItemBase TrackLocalDiagnostics(int errorid, string reason, params string[] additional)
		{
			string header = DiagnosticsItemFactory.FormatDiagnostics(errorid, reason, additional);
			return this.TrackDiagnostics(header);
		}

		private List<DiagnosticsItemBase> items = new List<DiagnosticsItemBase>(3);
	}
}
