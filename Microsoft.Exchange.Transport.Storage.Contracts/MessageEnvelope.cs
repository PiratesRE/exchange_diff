using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class MessageEnvelope
	{
		public MessageEnvelope(DeliveryPriority deliveryPriority, Guid organizationId, DateTime timeReceived, RoutingAddress fromAddress, MailDirectionality directionality, MimeDocument mimeDocument, long mimeSize, string subject, long msgId, IEnumerable<string> recipients)
		{
			this.deliveryPriority = deliveryPriority;
			this.externalOrganizationId = organizationId;
			this.timeReceived = timeReceived;
			this.fromAddress = fromAddress;
			this.directionality = directionality;
			this.mimeDocument = mimeDocument;
			this.mimeSize = mimeSize;
			this.subject = subject;
			this.msgId = msgId;
			this.recipients = recipients;
		}

		public DeliveryPriority DeliveryPriority
		{
			get
			{
				return this.deliveryPriority;
			}
		}

		public Guid ExternalOrganizationId
		{
			get
			{
				return this.externalOrganizationId;
			}
		}

		public DateTime TimeReceived
		{
			get
			{
				return this.timeReceived;
			}
		}

		public RoutingAddress FromAddress
		{
			get
			{
				return this.fromAddress;
			}
		}

		public MailDirectionality Directionality
		{
			get
			{
				return this.directionality;
			}
		}

		public MimeDocument MimeDocument
		{
			get
			{
				return this.mimeDocument;
			}
		}

		public long MimeSize
		{
			get
			{
				return this.mimeSize;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
		}

		public IEnumerable<string> Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		public long MsgId
		{
			get
			{
				return this.msgId;
			}
		}

		public void AddProperty<T>(string name, T value)
		{
			if (this.properties == null)
			{
				this.properties = new Dictionary<string, object>();
			}
			this.properties[name] = value;
		}

		public bool TryAddProperty<T>(string name, T value)
		{
			if (this.properties == null)
			{
				this.properties = new Dictionary<string, object>();
			}
			if (!this.properties.ContainsKey(name))
			{
				this.properties[name] = value;
				return true;
			}
			return false;
		}

		public bool TryGetProperty<T>(string name, out T value)
		{
			value = default(T);
			if (this.properties == null)
			{
				return false;
			}
			object obj;
			if (!this.properties.TryGetValue(name, out obj))
			{
				return false;
			}
			Type typeFromHandle = typeof(T);
			if (obj == null)
			{
				return !typeFromHandle.IsValueType || Nullable.GetUnderlyingType(typeFromHandle) != null;
			}
			Type type = obj.GetType();
			if (typeFromHandle.IsAssignableFrom(type))
			{
				value = (T)((object)obj);
				return true;
			}
			return false;
		}

		public IEnumerable<Tuple<string, Type>> GetPropertyNames(string prefix)
		{
			if (this.properties == null || prefix == null)
			{
				return null;
			}
			List<Tuple<string, Type>> list = new List<Tuple<string, Type>>();
			foreach (KeyValuePair<string, object> keyValuePair in this.properties)
			{
				if (keyValuePair.Key.StartsWith(prefix))
				{
					if (keyValuePair.Value == null)
					{
						list.Add(Tuple.Create<string, Type>(keyValuePair.Key, null));
					}
					else
					{
						list.Add(Tuple.Create<string, Type>(keyValuePair.Key, keyValuePair.Value.GetType()));
					}
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			return list;
		}

		internal static readonly string AttributionNamespace = "Transport.Attribution.";

		internal static readonly string AccountForestProperty = MessageEnvelope.AttributionNamespace + "AccountForest";

		private readonly DeliveryPriority deliveryPriority;

		private readonly Guid externalOrganizationId;

		private readonly DateTime timeReceived;

		private readonly RoutingAddress fromAddress;

		private readonly MailDirectionality directionality;

		private readonly MimeDocument mimeDocument;

		private readonly long mimeSize;

		private readonly string subject;

		private readonly long msgId;

		private IEnumerable<string> recipients;

		private Dictionary<string, object> properties;
	}
}
