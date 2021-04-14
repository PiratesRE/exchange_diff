using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal class ConditionalResults
	{
		public ConditionalResults(BaseConditionalRegistration registration, RegistrationResult result, Dictionary<PropertyDefinition, object> data)
		{
			this.Completed = (ExDateTime)TimeProvider.UtcNow;
			this.Registration = registration;
			this.Result = result;
			this.Data = data;
		}

		public BaseConditionalRegistration Registration { get; private set; }

		public ExDateTime Completed { get; private set; }

		public RegistrationResult Result { get; private set; }

		public Dictionary<PropertyDefinition, object> Data { get; private set; }

		public XElement GetXmlResults()
		{
			XElement xelement = new XElement("Registration");
			XElement xelement2 = new XElement("CreationDate");
			xelement.Add(xelement2);
			xelement2.Add(new XText(this.Registration.Created.ToString()));
			XElement xelement3 = new XElement("Description");
			xelement.Add(xelement3);
			ConditionalRegistration conditionalRegistration = this.Registration as ConditionalRegistration;
			if (conditionalRegistration != null)
			{
				xelement3.Add(new XCData(conditionalRegistration.Description));
			}
			else
			{
				xelement3.Add(new XCData((this.Registration as PersistentConditionalRegistration).Cookie));
			}
			XElement xelement4 = new XElement("OriginalFilter");
			xelement.Add(xelement4);
			xelement4.Add(new XCData(this.Registration.OriginalFilter));
			XElement xelement5 = new XElement("ParsedFilter");
			xelement.Add(xelement5);
			xelement5.Add(new XCData(this.Registration.QueryFilter.ToString()));
			XElement xelement6 = new XElement("Fetch");
			xelement.Add(xelement6);
			xelement6.Add(new XText(this.Registration.OriginalPropertiesToFetch));
			ConditionalRegistration conditionalRegistration2 = this.Registration as ConditionalRegistration;
			if (conditionalRegistration2 != null)
			{
				XElement xelement7 = new XElement("MaxHits");
				xelement.Add(xelement7);
				xelement7.Add(new XText(conditionalRegistration2.MaxHits.ToString()));
			}
			XElement xelement8 = new XElement("Results");
			xelement.Add(xelement8);
			xelement8.Add(new XText(this.Result.ToString()));
			XElement xelement9 = new XElement("CompleteDate");
			xelement.Add(xelement9);
			xelement9.Add(new XText(this.Completed.ToString()));
			XElement xelement10 = new XElement("Data");
			xelement.Add(xelement10);
			if (this.Data == null)
			{
				xelement10.Add(new XCData("NO DATA"));
			}
			else
			{
				foreach (KeyValuePair<PropertyDefinition, object> keyValuePair in this.Data)
				{
					XElement xelement11 = new XElement(keyValuePair.Key.Name);
					xelement10.Add(xelement11);
					if (keyValuePair.Value is string || keyValuePair.Value.GetType().IsValueType)
					{
						xelement11.Add(new XCData(keyValuePair.Value.ToString()));
					}
					else
					{
						XmlSerializer xmlSerializer = new XmlSerializer(keyValuePair.Value.GetType());
						using (MemoryStream memoryStream = new MemoryStream())
						{
							xmlSerializer.Serialize(memoryStream, keyValuePair.Value);
							memoryStream.Position = 0L;
							StreamReader streamReader = new StreamReader(memoryStream);
							xelement11.Add(new XCData(streamReader.ReadToEnd()));
							streamReader.Close();
						}
					}
				}
			}
			return xelement;
		}
	}
}
