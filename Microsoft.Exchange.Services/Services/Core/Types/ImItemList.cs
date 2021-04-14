using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ImItemList")]
	[XmlType("ImItemListType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ImItemList
	{
		[XmlArray]
		[DataMember(Order = 1)]
		[XmlArrayItem("ImGroup", typeof(ImGroup))]
		public ImGroup[] Groups { get; set; }

		[XmlArray]
		[XmlArrayItem("Persona", typeof(Persona))]
		[DataMember(Order = 2)]
		public Persona[] Personas { get; set; }

		internal static ImItemList LoadFrom(RawImItemList rawImItemList, ExtendedPropertyUri[] extendedProperties, MailboxSession session)
		{
			ImGroup[] array = null;
			if (rawImItemList.Groups.Length > 0)
			{
				array = new ImGroup[rawImItemList.Groups.Length];
				for (int i = 0; i < rawImItemList.Groups.Length; i++)
				{
					array[i] = ImGroup.LoadFromRawImGroup(rawImItemList.Groups[i], session);
				}
			}
			Persona[] array2 = null;
			int num = 0;
			if (rawImItemList.Personas.Length > 0)
			{
				PropertyDefinition[] array3 = Array<PropertyDefinition>.Empty;
				if (extendedProperties != null)
				{
					array3 = new PropertyDefinition[extendedProperties.Length];
					for (int j = 0; j < extendedProperties.Length; j++)
					{
						array3[j] = extendedProperties[j].ToPropertyDefinition();
					}
				}
				array2 = new Persona[rawImItemList.Personas.Length];
				for (int k = 0; k < rawImItemList.Personas.Length; k++)
				{
					PersonId personId = rawImItemList.Personas[k];
					ItemId personaId = IdConverter.PersonaIdFromPersonId(session.MailboxOwner.MailboxInfo.MailboxGuid, personId);
					array2[k] = Persona.LoadFromPersonaId(session, null, personaId, Persona.FullPersonaShape, array3, null);
					num += array2[k].Attributions.Length;
				}
			}
			if (array != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "ImGroupCount", array.Length);
			}
			else
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "ImGroupCount", 0);
			}
			if (array2 != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "PersonaCount", array2.Length);
			}
			else
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "PersonaCount", 0);
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "ContactCount", num);
			return new ImItemList
			{
				Groups = array,
				Personas = array2
			};
		}
	}
}
