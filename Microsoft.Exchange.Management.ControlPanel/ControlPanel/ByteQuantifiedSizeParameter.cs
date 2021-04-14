using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class ByteQuantifiedSizeParameter : NumberParameter
	{
		public ByteQuantifiedSizeParameter(string name, LocalizedString displayName, LocalizedString description, long minValue, long maxValue, long defaultValue) : this(name, displayName, description, minValue, maxValue, defaultValue, LocalizedString.Empty)
		{
		}

		public ByteQuantifiedSizeParameter(string name, LocalizedString displayName, LocalizedString description, long minValue, long maxValue, long defaultValue, LocalizedString noSelectionText) : base(name, displayName, description, minValue, maxValue, defaultValue)
		{
			if (string.IsNullOrEmpty(noSelectionText))
			{
				this.noSelectionText = Strings.TransportRuleStringParameterNoSelectionText;
				return;
			}
			this.noSelectionText = noSelectionText;
		}

		[DataMember]
		public string[] Unit
		{
			get
			{
				return new string[]
				{
					Strings.MailboxUsageUnitKB,
					Strings.MailboxUsageUnitMB,
					Strings.MailboxUsageUnitGB,
					Strings.MailboxUsageUnitTB
				};
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
