using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ResolutionSet")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", TypeName = "ResolutionSet")]
	[Serializable]
	public class ResolutionSet
	{
		[DataMember(Name = "TotalItemsInView", IsRequired = true)]
		[XmlAttribute]
		public int TotalItemsInView { get; set; }

		[DataMember(Name = "IncludesLastItemInRange", IsRequired = true)]
		[XmlAttribute]
		public bool IncludesLastItemInRange { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlElement("Resolution", IsNullable = false, Type = typeof(ResolutionType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public ResolutionType[] Resolutions
		{
			get
			{
				return this.resolutions.ToArray();
			}
			set
			{
				this.resolutions = new List<ResolutionType>(value);
			}
		}

		internal void Add(ResolutionType resolution)
		{
			if (this.resolutions == null)
			{
				this.resolutions = new List<ResolutionType>();
			}
			this.resolutions.Add(resolution);
		}

		private List<ResolutionType> resolutions;
	}
}
