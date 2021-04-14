using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RpcAssociationReplicatorRunNowParameters
	{
		public ICollection<IMailboxLocator> SlaveMailboxes { get; set; }

		public static RpcAssociationReplicatorRunNowParameters Parse(string input, IRecipientSession adSession)
		{
			SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(List<MailboxLocatorType>));
			RpcAssociationReplicatorRunNowParameters result;
			using (StringReader stringReader = new StringReader(input))
			{
				List<MailboxLocatorType> locators = safeXmlSerializer.Deserialize(stringReader) as List<MailboxLocatorType>;
				result = RpcAssociationReplicatorRunNowParameters.Instantiate(locators, adSession);
			}
			return result;
		}

		public override string ToString()
		{
			List<MailboxLocatorType> list = new List<MailboxLocatorType>(this.SlaveMailboxes.Count);
			foreach (IMailboxLocator locator in this.SlaveMailboxes)
			{
				list.Add(EwsAssociationDataConverter.Convert(locator));
			}
			SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(list.GetType());
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				safeXmlSerializer.Serialize(stringWriter, list);
				result = stringWriter.ToString();
			}
			return result;
		}

		private static RpcAssociationReplicatorRunNowParameters Instantiate(List<MailboxLocatorType> locators, IRecipientSession adSession)
		{
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			if (locators != null && locators.Count > 0)
			{
				List<IMailboxLocator> list = new List<IMailboxLocator>(locators.Count);
				for (int i = 0; i < locators.Count; i++)
				{
					list.Add(EwsAssociationDataConverter.Convert(locators[i], adSession));
				}
				return new RpcAssociationReplicatorRunNowParameters
				{
					SlaveMailboxes = list
				};
			}
			return new RpcAssociationReplicatorRunNowParameters();
		}

		private static readonly Trace Tracer = ExTraceGlobals.AssociationReplicationTracer;
	}
}
