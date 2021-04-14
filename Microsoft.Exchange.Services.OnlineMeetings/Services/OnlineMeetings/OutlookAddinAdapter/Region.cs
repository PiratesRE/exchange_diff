using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("Region")]
	[KnownType(typeof(Region))]
	[DataContract(Name = "Region")]
	public class Region
	{
		[XmlAttribute("Name")]
		[DataMember(Name = "Name", EmitDefaultValue = true)]
		public string Name { get; set; }

		[XmlElement("AccessNumber")]
		[DataMember(Name = "AccessNumber", EmitDefaultValue = false)]
		public AccessNumber[] AccessNumbers
		{
			get
			{
				return this.accessNumbersList.ToArray();
			}
			set
			{
				this.accessNumbersList.Clear();
				this.accessNumbersList.AddRange(value);
			}
		}

		internal List<AccessNumber> AccessNumbersList
		{
			get
			{
				return this.accessNumbersList;
			}
		}

		internal static Region[] ConvertFrom(DialInInformation dialIn, CultureInfo userCulture)
		{
			Dictionary<string, Region> dictionary = new Dictionary<string, Region>();
			int num = (userCulture != null) ? userCulture.LCID : 0;
			bool flag = false;
			foreach (DialInRegion dialInRegion in dialIn.DialInRegions)
			{
				List<AccessNumber> list = new List<AccessNumber>();
				foreach (string name in dialInRegion.Languages)
				{
					if (!string.IsNullOrWhiteSpace(dialInRegion.Number))
					{
						AccessNumber accessNumber = new AccessNumber();
						accessNumber.Number = dialInRegion.Number;
						try
						{
							accessNumber.LanguageID = new CultureInfo(name).LCID;
						}
						catch (CultureNotFoundException)
						{
						}
						if (accessNumber.LanguageID == num)
						{
							if (!flag)
							{
								list.Clear();
								flag = true;
							}
						}
						else if (flag)
						{
							continue;
						}
						list.Add(accessNumber);
					}
				}
				if (list.Any<AccessNumber>())
				{
					if (!dictionary.ContainsKey(dialInRegion.Name))
					{
						Region region = new Region();
						region.Name = dialInRegion.Name;
						dictionary.Add(dialInRegion.Name, region);
					}
					dictionary[dialInRegion.Name].AccessNumbersList.AddRange(list);
				}
			}
			return dictionary.Values.ToArray<Region>();
		}

		private readonly List<AccessNumber> accessNumbersList = new List<AccessNumber>();
	}
}
