using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class MessageProperty : Property
	{
		protected MessageProperty(string propertyName, Type type) : base(propertyName, type)
		{
		}

		public static MessageProperty Create(string propertyName)
		{
			if (propertyName.Equals("Message.Body", StringComparison.OrdinalIgnoreCase))
			{
				return new BodyProperty();
			}
			KeyValuePair<string, Type> keyValuePair;
			if (MessageProperty.knownProperties.TryGetValue(propertyName, out keyValuePair))
			{
				return new MessageProperty(keyValuePair.Key, keyValuePair.Value);
			}
			throw new RulesValidationException(RulesStrings.InvalidPropertyName(propertyName));
		}

		protected override object OnGetValue(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			string name;
			if ((name = base.Name) != null)
			{
				if (<PrivateImplementationDetails>{E5009AC9-61BF-4F4C-814F-F0F6ED8AB7B2}.$$method0x600016f-1 == null)
				{
					<PrivateImplementationDetails>{E5009AC9-61BF-4F4C-814F-F0F6ED8AB7B2}.$$method0x600016f-1 = new Dictionary<string, int>(28)
					{
						{
							"Message.Subject",
							0
						},
						{
							"Message.Sender",
							1
						},
						{
							"Message.MessageId",
							2
						},
						{
							"Message.InReplyTo",
							3
						},
						{
							"Message.References",
							4
						},
						{
							"Message.ReturnPath",
							5
						},
						{
							"Message.Comments",
							6
						},
						{
							"Message.Keywords",
							7
						},
						{
							"Message.ResentMessageId",
							8
						},
						{
							"Message.From",
							9
						},
						{
							"Message.SenderDomain",
							10
						},
						{
							"Message.To",
							11
						},
						{
							"Message.Cc",
							12
						},
						{
							"Message.ToCc",
							13
						},
						{
							"Message.Bcc",
							14
						},
						{
							"Message.ReplyTo",
							15
						},
						{
							"Message.SclValue",
							16
						},
						{
							"Message.AttachmentNames",
							17
						},
						{
							"Message.AttachmentExtensions",
							18
						},
						{
							"Message.AttachmentTypes",
							19
						},
						{
							"Message.MaxAttachmentSize",
							20
						},
						{
							"Message.ContentCharacterSets",
							21
						},
						{
							"Message.EnvelopeFrom",
							22
						},
						{
							"Message.EnvelopeRecipients",
							23
						},
						{
							"Message.Auth",
							24
						},
						{
							"Message.DataClassifications",
							25
						},
						{
							"Message.Size",
							26
						},
						{
							"Message.SenderIp",
							27
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{E5009AC9-61BF-4F4C-814F-F0F6ED8AB7B2}.$$method0x600016f-1.TryGetValue(name, out num))
				{
					object obj;
					switch (num)
					{
					case 0:
						obj = MessageProperty.GetSubjectProperty(transportRulesEvaluationContext);
						break;
					case 1:
						obj = transportRulesEvaluationContext.Message.Sender;
						break;
					case 2:
						obj = transportRulesEvaluationContext.Message.MessageId;
						break;
					case 3:
						obj = transportRulesEvaluationContext.Message.InReplyTo;
						break;
					case 4:
						obj = transportRulesEvaluationContext.Message.References;
						break;
					case 5:
						obj = transportRulesEvaluationContext.Message.ReturnPath;
						break;
					case 6:
						obj = transportRulesEvaluationContext.Message.Comments;
						break;
					case 7:
						obj = transportRulesEvaluationContext.Message.Keywords;
						break;
					case 8:
						obj = transportRulesEvaluationContext.Message.ResentMessageId;
						break;
					case 9:
						obj = MessageProperty.GetMessageFromValue(transportRulesEvaluationContext);
						break;
					case 10:
						obj = MessageProperty.GetSenderDomainValue(transportRulesEvaluationContext);
						break;
					case 11:
						obj = transportRulesEvaluationContext.Message.To;
						break;
					case 12:
						obj = transportRulesEvaluationContext.Message.Cc;
						break;
					case 13:
						obj = transportRulesEvaluationContext.Message.ToCc;
						break;
					case 14:
						obj = transportRulesEvaluationContext.Message.Bcc;
						break;
					case 15:
						obj = transportRulesEvaluationContext.Message.ReplyTo;
						break;
					case 16:
						obj = transportRulesEvaluationContext.Message.SclValue;
						break;
					case 17:
						obj = transportRulesEvaluationContext.Message.AttachmentNames;
						break;
					case 18:
						obj = transportRulesEvaluationContext.Message.AttachmentExtensions;
						break;
					case 19:
						obj = transportRulesEvaluationContext.Message.AttachmentTypes;
						break;
					case 20:
						obj = transportRulesEvaluationContext.Message.MaxAttachmentSize;
						break;
					case 21:
						obj = transportRulesEvaluationContext.Message.ContentCharacterSets;
						break;
					case 22:
						obj = transportRulesEvaluationContext.Message.EnvelopeFrom;
						break;
					case 23:
						obj = transportRulesEvaluationContext.Message.EnvelopeRecipients;
						break;
					case 24:
						obj = TransportUtils.GetMessageAuth(transportRulesEvaluationContext);
						break;
					case 25:
						obj = transportRulesEvaluationContext.DataClassifications;
						break;
					case 26:
						obj = transportRulesEvaluationContext.Message.Size;
						break;
					case 27:
						obj = TransportUtils.GetOriginalClientIpAddress(transportRulesEvaluationContext.Message);
						break;
					default:
						goto IL_3D2;
					}
					TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, string.Format("{0} property value evaluated as rule condition: '{1}'", base.Name, obj ?? "null"));
					if (transportRulesEvaluationContext.IsTestMessage && obj is IEnumerable<string>)
					{
						TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, string.Format("property is a collection of values: '{0}'", string.Join(",", obj as IEnumerable<string>)));
					}
					return obj;
				}
			}
			IL_3D2:
			throw new InvalidOperationException("Unknown Property Name");
		}

		internal static string GetDomain(string address)
		{
			if (!string.IsNullOrEmpty(address))
			{
				return new SmtpAddress(address).Domain;
			}
			return string.Empty;
		}

		internal static List<string> BuildDistinctMultivaluedProperty(string item1, string item2 = null)
		{
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(item1))
			{
				list.Add(item1);
			}
			if (!string.IsNullOrEmpty(item2) && !item2.Equals(item1, StringComparison.OrdinalIgnoreCase))
			{
				list.Add(item2);
			}
			if (!list.Any<string>())
			{
				list.Add(string.Empty);
			}
			return list;
		}

		private static object GetSenderDomainValue(TransportRulesEvaluationContext context)
		{
			TransportRule transportRule = context.CurrentRule as TransportRule;
			switch (transportRule.SenderAddressLocation)
			{
			case SenderAddressLocation.Header:
				return MessageProperty.BuildDistinctMultivaluedProperty(MessageProperty.GetDomain(context.Message.From), null);
			case SenderAddressLocation.Envelope:
				return MessageProperty.BuildDistinctMultivaluedProperty(MessageProperty.GetDomain(context.Message.EnvelopeFrom), null);
			case SenderAddressLocation.HeaderOrEnvelope:
				return MessageProperty.BuildDistinctMultivaluedProperty(MessageProperty.GetDomain(context.Message.From), MessageProperty.GetDomain(context.Message.EnvelopeFrom));
			default:
				return null;
			}
		}

		internal static object GetMessageFromValue(TransportRulesEvaluationContext context)
		{
			TransportRule transportRule = context.CurrentRule as TransportRule;
			switch (transportRule.SenderAddressLocation)
			{
			case SenderAddressLocation.Header:
				return MessageProperty.BuildDistinctMultivaluedProperty(context.Message.From, null);
			case SenderAddressLocation.Envelope:
				return MessageProperty.BuildDistinctMultivaluedProperty(context.Message.EnvelopeFrom, null);
			case SenderAddressLocation.HeaderOrEnvelope:
				return MessageProperty.BuildDistinctMultivaluedProperty(context.Message.From, context.Message.EnvelopeFrom);
			default:
				return null;
			}
		}

		private static Dictionary<string, KeyValuePair<string, Type>> InitializeProperties()
		{
			Dictionary<string, KeyValuePair<string, Type>> dictionary = new Dictionary<string, KeyValuePair<string, Type>>(50);
			MessageProperty.AddProperty("Message.Subject", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.Sender", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.MessageId", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.InReplyTo", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.References", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.ReturnPath", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.Comments", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.Keywords", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.ResentMessageId", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.From", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.To", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.Cc", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.ToCc", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.Bcc", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.ReplyTo", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.SclValue", typeof(int), dictionary);
			MessageProperty.AddProperty("Message.AttachmentNames", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.AttachmentExtensions", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.MaxAttachmentSize", typeof(ulong), dictionary);
			MessageProperty.AddProperty("Message.Size", typeof(ulong), dictionary);
			MessageProperty.AddProperty("Message.AttachmentTypes", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.ContentCharacterSets", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.EnvelopeFrom", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.EnvelopeRecipients", typeof(List<string>), dictionary);
			MessageProperty.AddProperty("Message.Auth", typeof(string), dictionary);
			MessageProperty.AddProperty("Message.DataClassifications", typeof(IEnumerable<DiscoveredDataClassification>), dictionary);
			MessageProperty.AddProperty("Message.SenderIp", typeof(IPAddress), dictionary);
			MessageProperty.AddProperty("Message.SenderDomain", typeof(List<string>), dictionary);
			return dictionary;
		}

		private static void AddProperty(string name, Type type, Dictionary<string, KeyValuePair<string, Type>> properties)
		{
			properties.Add(name, new KeyValuePair<string, Type>(name, type));
		}

		private static string GetSubjectProperty(TransportRulesEvaluationContext context)
		{
			string result;
			try
			{
				result = context.Message.Subject.Normalize(NormalizationForm.FormKC);
			}
			catch (ArgumentException arg)
			{
				TransportRulesEvaluator.Trace(context.TransportRulesTracer, context.MailItem, string.Format("Subject normalization has failed {0}", arg));
				result = context.Message.Subject;
			}
			return result;
		}

		internal const string Subject = "Message.Subject";

		private const string Sender = "Message.Sender";

		private const string MessageId = "Message.MessageId";

		private const string InReplyTo = "Message.InReplyTo";

		private const string References = "Message.References";

		private const string ReturnPath = "Message.ReturnPath";

		private const string Comments = "Message.Comments";

		private const string Keywords = "Message.Keywords";

		private const string ResentMessageId = "Message.ResentMessageId";

		private const string From = "Message.From";

		private const string To = "Message.To";

		private const string Cc = "Message.Cc";

		private const string ToCc = "Message.ToCc";

		private const string Bcc = "Message.Bcc";

		private const string ReplyTo = "Message.ReplyTo";

		private const string SclValue = "Message.SclValue";

		private const string EnvelopeFrom = "Message.EnvelopeFrom";

		private const string EnvelopeRecipients = "Message.EnvelopeRecipients";

		private const string Auth = "Message.Auth";

		private const string DataClassifications = "Message.DataClassifications";

		private const string Size = "Message.Size";

		private const string SenderIp = "Message.SenderIp";

		private static readonly Dictionary<string, KeyValuePair<string, Type>> knownProperties = MessageProperty.InitializeProperties();
	}
}
