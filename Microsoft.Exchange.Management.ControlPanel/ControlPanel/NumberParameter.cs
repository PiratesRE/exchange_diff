using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class NumberParameter : FormletParameter
	{
		public NumberParameter(string name, LocalizedString displayName, LocalizedString description, long minValue, long maxValue, long defaultValue) : base(name, displayName, description)
		{
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.DefaultValue = defaultValue;
			base.FormletType = typeof(NumberModalEditor);
		}

		[DataMember]
		public long MinValue { get; private set; }

		[DataMember]
		public long MaxValue { get; private set; }

		[DataMember]
		public long DefaultValue { get; private set; }
	}
}
