using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class ObjectArrayParameter : FormletParameter
	{
		public ObjectArrayParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel) : base(name, dialogTitle, dialogLabel)
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public bool UseAndDelimiter { get; set; }
	}
}
