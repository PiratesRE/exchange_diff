using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class IncidentReportContentParameter : FormletParameter
	{
		public IncidentReportContentParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, LocalizedString noSelectionText) : base(name, dialogTitle, dialogLabel, new string[]
		{
			"IncidentReportContent"
		})
		{
			base.FormletType = typeof(IncidentReportContentEditor);
			this.noSelectionText = noSelectionText;
			base.RequiredField = true;
		}
	}
}
