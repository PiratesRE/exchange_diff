using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class CallerIdsParameter : ObjectArrayParameter
	{
		public CallerIdsParameter(string name) : base(name, LocalizedString.Empty, LocalizedString.Empty)
		{
			this.noSelectionText = Strings.CallerIdParameterNoSelectionText;
			base.FormletType = typeof(CallerIdsEditor);
		}
	}
}
