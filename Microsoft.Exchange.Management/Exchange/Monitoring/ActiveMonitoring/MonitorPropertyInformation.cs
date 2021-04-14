using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	[Serializable]
	public class MonitorPropertyInformation : ConfigurableObject
	{
		public MonitorPropertyInformation() : base(new SimpleProviderPropertyBag())
		{
		}

		internal MonitorPropertyInformation(string server, PropertyInformation propertyInfo) : this()
		{
			this.Server = server;
			this.PropertyName = propertyInfo.Name;
			this.Description = propertyInfo.Description;
			this.IsMandatory = propertyInfo.IsMandatory;
			this[SimpleProviderObjectSchema.Identity] = new MonitorPropertyInformation.MonitorPropertyInformationId(this.PropertyName, this.Description, this.IsMandatory);
		}

		public string Server
		{
			get
			{
				return (string)this[MonitorPropertyInformationSchema.Server];
			}
			private set
			{
				this[MonitorPropertyInformationSchema.Server] = value;
			}
		}

		public string PropertyName
		{
			get
			{
				return (string)this[MonitorPropertyInformationSchema.PropertyName];
			}
			private set
			{
				this[MonitorPropertyInformationSchema.PropertyName] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[MonitorPropertyInformationSchema.Description];
			}
			private set
			{
				this[MonitorPropertyInformationSchema.Description] = value;
			}
		}

		public bool IsMandatory
		{
			get
			{
				return (bool)this[MonitorPropertyInformationSchema.IsMandatory];
			}
			private set
			{
				this[MonitorPropertyInformationSchema.IsMandatory] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MonitorPropertyInformation.schema;
			}
		}

		private static MonitorPropertyInformationSchema schema = ObjectSchema.GetInstance<MonitorPropertyInformationSchema>();

		[Serializable]
		public class MonitorPropertyInformationId : ObjectId
		{
			public MonitorPropertyInformationId(string propertyName, string propertyDescription, bool isMandatory)
			{
				this.identity = string.Format("{0}\\{1}\\{2}", propertyName, propertyDescription, isMandatory.ToString());
			}

			public override string ToString()
			{
				return this.identity;
			}

			public override byte[] GetBytes()
			{
				return Encoding.Unicode.GetBytes(this.ToString());
			}

			private readonly string identity;
		}
	}
}
