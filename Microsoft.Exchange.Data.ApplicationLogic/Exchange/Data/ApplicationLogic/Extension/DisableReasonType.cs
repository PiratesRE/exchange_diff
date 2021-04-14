using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	[XmlType(TypeName = "DisableReasonType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum DisableReasonType
	{
		NotDisabled,
		NoError,
		NoReason,
		OutlookClientPerformance,
		OWAClientPerformance,
		MobileClientPerformance
	}
}
