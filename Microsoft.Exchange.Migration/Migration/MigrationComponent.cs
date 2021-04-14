using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal abstract class MigrationComponent : IDiagnosable
	{
		internal MigrationComponent(string name, WaitHandle stopEvent)
		{
			this.Name = name;
			this.StopEvent = stopEvent;
			this.DiagnosticInfo = new MigrationComponentDiagnosticInfo();
		}

		internal MigrationComponentDiagnosticInfo DiagnosticInfo { get; private set; }

		internal string Name { get; private set; }

		private protected WaitHandle StopEvent { protected get; private set; }

		public string GetDiagnosticComponentName()
		{
			return this.Name;
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return new XElement(this.GetDiagnosticComponentName(), new object[]
			{
				new XAttribute("name", this.Name),
				new XElement("duration", this.DiagnosticInfo.Duration),
				new XElement("lastRunTime", this.DiagnosticInfo.LastRunTime.UniversalTime),
				new XElement("lastWorkTime", this.DiagnosticInfo.LastWorkTime.UniversalTime)
			});
		}

		internal abstract bool ShouldProcess();

		internal abstract bool Process(IMigrationJobCache data);
	}
}
