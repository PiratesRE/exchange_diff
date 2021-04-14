using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "PushNotificationAppConfigXml")]
	[Serializable]
	public sealed class PushNotificationAppConfigXml : XMLSerializableBase, IEquatable<PushNotificationAppConfigXml>
	{
		[XmlElement]
		public bool? Enabled { get; set; }

		[XmlElement]
		public string ExchangeMinimumVersion { get; set; }

		[XmlElement]
		public string ExchangeMaximumVersion { get; set; }

		[XmlElement]
		public int? QueueSize { get; set; }

		[XmlElement]
		public int? NumberOfChannels { get; set; }

		[XmlElement]
		public int? BackOffTimeInSeconds { get; set; }

		[XmlElement]
		public string AuthId { get; set; }

		[XmlElement]
		public string AuthKey { get; set; }

		[XmlElement]
		public string AuthKeyFallback { get; set; }

		[XmlElement]
		public bool? IsAuthKeyEncrypted { get; set; }

		[XmlElement]
		public string Url { get; set; }

		[XmlElement]
		public int? Port { get; set; }

		[XmlElement]
		public string SecondaryUrl { get; set; }

		[XmlElement]
		public int? SecondaryPort { get; set; }

		[XmlElement]
		public string UriTemplate { get; set; }

		[XmlElement]
		public string RegistrationTemplate { get; set; }

		[XmlElement]
		public bool? RegistrationEnabled { get; set; }

		[XmlElement]
		public bool? MultifactorRegistrationEnabled { get; set; }

		[XmlElement]
		public string PartitionName { get; set; }

		[XmlElement]
		public bool? IsDefaultPartitionName { get; set; }

		[XmlElement]
		public DateTime? LastUpdateTimeUtc { get; set; }

		public bool ShouldSerializeEnabled()
		{
			return this.Enabled != null;
		}

		public bool ShouldSerializeQueueSize()
		{
			return this.QueueSize != null;
		}

		public bool ShouldSerializeNumberOfChannels()
		{
			return this.NumberOfChannels != null;
		}

		public bool ShouldSerializeBackOffTimeInSeconds()
		{
			return this.BackOffTimeInSeconds != null;
		}

		public bool ShouldSerializeIsAuthKeyEncrypted()
		{
			return this.IsAuthKeyEncrypted != null;
		}

		public bool ShouldSerializePort()
		{
			return this.Port != null;
		}

		public bool ShouldSerializeSecondaryPort()
		{
			return this.SecondaryPort != null;
		}

		public bool ShouldSerializeRegistrationEnabled()
		{
			return this.RegistrationEnabled != null;
		}

		public bool ShouldSerializeMultifactorRegistrationEnabled()
		{
			return this.MultifactorRegistrationEnabled != null;
		}

		public bool ShouldSerializeIsDefaultPartitionName()
		{
			return this.IsDefaultPartitionName != null;
		}

		public bool ShouldSerializeLastUpdateTimeUtc()
		{
			return this.LastUpdateTimeUtc != null;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as PushNotificationAppConfigXml);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(PushNotificationAppConfigXml other)
		{
			return other != null && this.ToString().Equals(other.ToString());
		}

		protected override void OnDeserialized()
		{
			if (base.UnknownAttributes == null)
			{
				return;
			}
			foreach (XmlAttribute xmlAttribute in base.UnknownAttributes)
			{
				if (this.Enabled == null && xmlAttribute.Name.Equals("ENA"))
				{
					this.Enabled = new bool?(bool.Parse(xmlAttribute.Value));
				}
				if (this.ExchangeMinimumVersion == null && xmlAttribute.Name.Equals("EMINV"))
				{
					this.ExchangeMinimumVersion = xmlAttribute.Value;
				}
				if (this.ExchangeMaximumVersion == null && xmlAttribute.Name.Equals("EMAXV"))
				{
					this.ExchangeMaximumVersion = xmlAttribute.Value;
				}
				if (this.QueueSize == null && xmlAttribute.Name.Equals("QSZ"))
				{
					this.QueueSize = new int?(int.Parse(xmlAttribute.Value));
				}
				if (this.NumberOfChannels == null && xmlAttribute.Name.Equals("NWT"))
				{
					this.NumberOfChannels = new int?(int.Parse(xmlAttribute.Value));
				}
				if (this.BackOffTimeInSeconds == null && xmlAttribute.Name.Equals("BFT"))
				{
					this.BackOffTimeInSeconds = new int?(int.Parse(xmlAttribute.Value));
				}
				if (this.AuthKey == null && xmlAttribute.Name.Equals("ANKP"))
				{
					this.AuthKey = xmlAttribute.Value;
				}
				if (this.AuthKeyFallback == null && xmlAttribute.Name.Equals("ANKF"))
				{
					this.AuthKeyFallback = xmlAttribute.Value;
				}
				if (this.IsAuthKeyEncrypted == null && xmlAttribute.Name.Equals("EANK"))
				{
					this.IsAuthKeyEncrypted = new bool?(bool.Parse(xmlAttribute.Value));
				}
				if (this.Url == null && xmlAttribute.Name.Equals("URL"))
				{
					this.Url = xmlAttribute.Value;
				}
				if (this.Port == null && xmlAttribute.Name.Equals("PRT"))
				{
					this.Port = new int?(int.Parse(xmlAttribute.Value));
				}
				if (this.SecondaryUrl == null && xmlAttribute.Name.Equals("SURL"))
				{
					this.SecondaryUrl = xmlAttribute.Value;
				}
				if (this.SecondaryPort == null && xmlAttribute.Name.Equals("SPRT"))
				{
					this.SecondaryPort = new int?(int.Parse(xmlAttribute.Value));
				}
			}
		}

		[Conditional("DEBUG")]
		private static void ValidateClassStructure()
		{
			Type typeFromHandle = typeof(Nullable<>);
			Type typeFromHandle2 = typeof(PushNotificationAppConfigXml);
			foreach (PropertyInfo propertyInfo in typeFromHandle2.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
			{
				if (propertyInfo.PropertyType.IsValueType && propertyInfo.GetCustomAttribute<XmlElementAttribute>() != null)
				{
					if (!propertyInfo.PropertyType.IsGenericType || !(propertyInfo.PropertyType.GetGenericTypeDefinition() == typeFromHandle))
					{
						throw new NotSupportedException(string.Format("PushNotificationAppConfigXml's property {0} must be defined as nullable", propertyInfo.Name));
					}
					if (typeFromHandle2.GetMethod(string.Format("ShouldSerialize{0}", propertyInfo.Name)) == null)
					{
						throw new NotSupportedException(string.Format("A ShouldSerialize method should be added for PushNotificationAppConfigXml's property {0}", propertyInfo.Name));
					}
				}
			}
		}
	}
}
