using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	internal class PolicyNudgeConfiguration15 : PolicyNudgeConfiguration
	{
		internal override XmlElement SerializeConfiguration(XElement clientConfig, CachedOrganizationConfiguration serverConfig, ADObjectId senderADObjectId, XmlDocument xmlDoc)
		{
			PolicyNudges15 policyNudges = null;
			XmlElement result;
			using (XmlReader xmlReader = clientConfig.CreateReader())
			{
				try
				{
					SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(PolicyNudges15));
					safeXmlSerializer.UnknownAttribute += PolicyNudgeConfiguration15.Serializer_UnknownAttribute;
					policyNudges = (PolicyNudges15)safeXmlSerializer.Deserialize(xmlReader);
				}
				catch (InvalidOperationException)
				{
					policyNudges = null;
				}
				bool hasChanged = HasChangedVisitor15.HasChanged(policyNudges, serverConfig, senderADObjectId);
				result = SerializerVisitor15.Serialize(policyNudges, hasChanged, serverConfig, senderADObjectId, xmlDoc);
			}
			return result;
		}

		private static void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			IOtherAttributes otherAttributes = e.ObjectBeingDeserialized as IOtherAttributes;
			if (otherAttributes != null)
			{
				otherAttributes.OtherAttributes.Add(new OtherAttribute(e.Attr.LocalName, e.Attr.Value));
			}
		}
	}
}
