using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public class MachineToPersonMessagingPolicies
	{
		public MachineToPersonMessagingPolicies()
		{
		}

		public MachineToPersonMessagingPolicies(IEnumerable<PossibleRecipient> possibleRecipients)
		{
			if (possibleRecipients != null)
			{
				this.PossibleRecipients = new List<PossibleRecipient>(possibleRecipients);
			}
		}

		[XmlElement("PossibleRecipient")]
		public List<PossibleRecipient> PossibleRecipients
		{
			get
			{
				return AccessorTemplates.ListPropertyGetter<PossibleRecipient>(ref this.possibleRecipients);
			}
			set
			{
				AccessorTemplates.ListPropertySetter<PossibleRecipient>(ref this.possibleRecipients, value);
			}
		}

		[XmlIgnore]
		public IList<PossibleRecipient> EffectivePossibleRecipients
		{
			get
			{
				return PossibleRecipient.GetCandidates(this.PossibleRecipients, true);
			}
		}

		[XmlIgnore]
		public IList<PossibleRecipient> NoneffectivePossibleRecipients
		{
			get
			{
				return PossibleRecipient.GetCandidates(this.PossibleRecipients, false);
			}
		}

		private List<PossibleRecipient> possibleRecipients;
	}
}
