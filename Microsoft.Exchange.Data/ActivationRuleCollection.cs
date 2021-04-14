using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[CollectionDataContract(Name = "Rules", ItemName = "Rule")]
	public class ActivationRuleCollection : Collection<ActivationRule>
	{
	}
}
