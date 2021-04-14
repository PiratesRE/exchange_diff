using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public class Emitter
	{
		public Emitter()
		{
		}

		public Emitter(EmitterType type, int priority, bool exclusive, IEnumerable<E164Number> numbers)
		{
			this.Type = type;
			if (numbers != null)
			{
				this.PhoneNumbers = new List<E164Number>(numbers);
			}
		}

		[XmlElement("Type")]
		public EmitterType Type { get; set; }

		[XmlElement("Priority")]
		public int Priority { get; set; }

		[XmlElement("Exclusive")]
		public bool Exclusive { get; set; }

		[XmlElement("PhoneNumber")]
		public List<E164Number> PhoneNumbers
		{
			get
			{
				return AccessorTemplates.ListPropertyGetter<E164Number>(ref this.phoneNumbers);
			}
			set
			{
				AccessorTemplates.ListPropertySetter<E164Number>(ref this.phoneNumbers, value);
			}
		}

		private List<E164Number> phoneNumbers;
	}
}
