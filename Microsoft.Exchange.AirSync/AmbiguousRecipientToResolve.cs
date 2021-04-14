using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Microsoft.Exchange.AirSync
{
	internal class AmbiguousRecipientToResolve
	{
		internal AmbiguousRecipientToResolve(string name)
		{
			this.Name = name;
		}

		internal bool CompleteList { get; set; }

		internal bool ExactMatchFound { get; set; }

		internal string Name { get; private set; }

		internal int ResolvedNamesCount { get; set; }

		internal List<ResolvedRecipient> ResolvedTo { get; set; }

		internal StatusCode Status { get; set; }

		internal PictureOptions PictureOptions { get; set; }

		internal void BuildXmlResponse(XmlDocument xmlResponse, XmlNode parentNode)
		{
			XmlNode xmlNode = xmlResponse.CreateElement("Response", "ResolveRecipients:");
			parentNode.AppendChild(xmlNode);
			XmlNode xmlNode2 = xmlResponse.CreateElement("To", "ResolveRecipients:");
			xmlNode2.InnerText = this.Name;
			xmlNode.AppendChild(xmlNode2);
			XmlNode xmlNode3 = xmlResponse.CreateElement("Status", "ResolveRecipients:");
			xmlNode3.InnerText = ((int)this.Status).ToString(CultureInfo.InvariantCulture);
			xmlNode.AppendChild(xmlNode3);
			if (this.Status != StatusCode.Sync_ProtocolError)
			{
				XmlNode xmlNode4 = xmlResponse.CreateElement("RecipientCount", "ResolveRecipients:");
				xmlNode4.InnerText = this.ResolvedNamesCount.ToString(CultureInfo.InvariantCulture);
				xmlNode.AppendChild(xmlNode4);
				int num = 0;
				foreach (ResolvedRecipient resolvedRecipient in this.ResolvedTo)
				{
					resolvedRecipient.PictureOptions = this.PictureOptions;
					bool flag;
					resolvedRecipient.BuildXmlResponse(xmlResponse, xmlNode, this.PictureOptions == null || num >= this.PictureOptions.MaxPictures, out flag);
					if (flag)
					{
						num++;
					}
				}
			}
		}
	}
}
