using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[KnownType(typeof(NewMailSubscription))]
	[XmlInclude(typeof(RowSubscription))]
	[KnownType(typeof(RowSubscription))]
	[XmlInclude(typeof(UnseenCountSubscription))]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlInclude(typeof(NewMailSubscription))]
	[KnownType(typeof(UnseenCountSubscription))]
	public abstract class BaseSubscription
	{
		protected BaseSubscription(NotificationType notificationType)
		{
			this.NotificationType = notificationType;
			this.CultureInfo = CultureInfo.InvariantCulture;
		}

		[DataMember(EmitDefaultValue = false)]
		public NotificationType NotificationType { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public string ConsumerSubscriptionId { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public CultureInfo CultureInfo { get; set; }

		[DataMember(EmitDefaultValue = false, Name = "Culture")]
		[XmlElement("Culture")]
		public string CultureInfoForSerialization
		{
			get
			{
				if (this.CultureInfo != null)
				{
					return this.CultureInfo.Name;
				}
				return null;
			}
			set
			{
				this.CultureInfo = (string.IsNullOrEmpty(value) ? CultureInfo.InvariantCulture : new CultureInfo(value));
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsValid
		{
			get
			{
				return this.Validate();
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public virtual IEnumerable<Tuple<string, object>> Differentiators
		{
			get
			{
				return new Tuple<string, object>[]
				{
					new Tuple<string, object>("NT", this.NotificationType)
				};
			}
		}

		protected virtual bool Validate()
		{
			return Enum.IsDefined(typeof(NotificationType), this.NotificationType) && this.CultureInfo != null;
		}
	}
}
