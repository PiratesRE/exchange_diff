using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class TimePeriodParameter : FormletParameter
	{
		public TimePeriodParameter(string name) : base(name, LocalizedString.Empty, LocalizedString.Empty)
		{
			this.noSelectionText = Strings.TimePeriodParameterNoSelectionText;
			base.FormletType = typeof(TimePeriodEditor);
		}
	}
}
