using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class JournalRuleScopeParameter : HiddenParameter
	{
		public JournalRuleScopeParameter(string defaultText) : base(defaultText, LocalizedString.Empty, LocalizedString.Empty, new string[]
		{
			"Scope"
		}, true)
		{
			this.DefaultText = defaultText;
		}

		[DataMember]
		public string DefaultText { get; private set; }
	}
}
