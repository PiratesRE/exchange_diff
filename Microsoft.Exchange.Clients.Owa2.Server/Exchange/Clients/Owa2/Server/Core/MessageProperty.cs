using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class MessageProperty : Property
	{
		public MessageProperty(string propertyName, Type type) : base(propertyName, type)
		{
		}

		public static MessageProperty Create(string propertyName)
		{
			KeyValuePair<string, Type> keyValuePair;
			if (MessageProperty.knownProperties.TryGetValue(propertyName, out keyValuePair))
			{
				return new MessageProperty(keyValuePair.Key, keyValuePair.Value);
			}
			return new MessageProperty(propertyName, typeof(string));
		}

		protected override object OnGetValue(RulesEvaluationContext baseContext)
		{
			OwaRulesEvaluationContext owaRulesEvaluationContext = (OwaRulesEvaluationContext)baseContext;
			object result = null;
			string name;
			if ((name = base.Name) != null)
			{
				if (!(name == "Message.From"))
				{
					if (!(name == "Message.To"))
					{
						if (name == "Message.DataClassifications")
						{
							result = owaRulesEvaluationContext.DataClassifications;
						}
					}
					else
					{
						ShortList<string> recipients = owaRulesEvaluationContext.Recipients;
						if (recipients != null && recipients.Any<string>())
						{
							result = recipients;
						}
					}
				}
				else
				{
					result = new ShortList<string>
					{
						owaRulesEvaluationContext.FromAddress
					};
				}
			}
			return result;
		}

		private static Dictionary<string, KeyValuePair<string, Type>> InitializeProperties()
		{
			Dictionary<string, KeyValuePair<string, Type>> dictionary = new Dictionary<string, KeyValuePair<string, Type>>(3);
			MessageProperty.AddProperty("Message.From", typeof(ShortList<string>), dictionary);
			MessageProperty.AddProperty("Message.To", typeof(ShortList<string>), dictionary);
			MessageProperty.AddProperty("Message.DataClassifications", typeof(IEnumerable<DiscoveredDataClassification>), dictionary);
			return dictionary;
		}

		private static void AddProperty(string name, Type type, Dictionary<string, KeyValuePair<string, Type>> properties)
		{
			properties.Add(name, new KeyValuePair<string, Type>(name, type));
		}

		public const string From = "Message.From";

		public const string To = "Message.To";

		public const string DataClassifications = "Message.DataClassifications";

		private static readonly Dictionary<string, KeyValuePair<string, Type>> knownProperties = MessageProperty.InitializeProperties();
	}
}
