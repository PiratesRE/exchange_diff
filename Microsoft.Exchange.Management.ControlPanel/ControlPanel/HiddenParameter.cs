using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class HiddenParameter : FormletParameter
	{
		public HiddenParameter(string name, object value) : base(name, LocalizedString.Empty, LocalizedString.Empty)
		{
			this.Value = value;
			base.ExactMatch = true;
		}

		public HiddenParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, string[] taskParameterNames, object value) : base(name, dialogTitle, dialogLabel, taskParameterNames)
		{
			this.Value = value;
			base.ExactMatch = true;
		}

		[DataMember]
		public object Value { get; set; }
	}
}
