using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	[DebuggerDisplay("Name = {PropertyName}; Namespace = {Namespace}; Id = {PropertyId}")]
	internal abstract class PropertyBase : MessageTraceEntityBase
	{
		public PropertyBase(string nameSpace, string name, object value)
		{
			this.PropertyId = Guid.NewGuid();
			this.Namespace = nameSpace;
			this.PropertyName = name;
			this.SetProperty(value);
		}

		public PropertyBase(string nameSpace, string name, int? value)
		{
			this.PropertyId = Guid.NewGuid();
			this.Namespace = nameSpace;
			this.PropertyName = name;
			this.SetProperty(value);
		}

		public PropertyBase(string nameSpace, string name, string value)
		{
			this.PropertyId = Guid.NewGuid();
			this.Namespace = nameSpace;
			this.PropertyName = name;
			this.SetProperty(value);
		}

		public PropertyBase(string nameSpace, string name, DateTime? value)
		{
			this.PropertyId = Guid.NewGuid();
			this.Namespace = nameSpace;
			this.PropertyName = name;
			this.SetProperty(value);
		}

		public PropertyBase(string nameSpace, string name, decimal? value)
		{
			this.PropertyId = Guid.NewGuid();
			this.Namespace = nameSpace;
			this.PropertyName = name;
			this.SetProperty(value);
		}

		public PropertyBase(string nameSpace, string name, BlobType value)
		{
			this.PropertyId = Guid.NewGuid();
			this.Namespace = nameSpace;
			this.PropertyName = name;
			this.SetProperty(value);
		}

		public PropertyBase(string nameSpace, string name, Guid value)
		{
			this.PropertyId = Guid.NewGuid();
			this.Namespace = nameSpace;
			this.PropertyName = name;
			this.SetProperty(value);
		}

		public PropertyBase(string nameSpace, string name, long? value)
		{
			this.PropertyId = Guid.NewGuid();
			this.Namespace = nameSpace;
			this.PropertyName = name;
			this.SetProperty(value);
		}

		public PropertyBase(string nameSpace, string name, bool value)
		{
			this.PropertyId = Guid.NewGuid();
			this.Namespace = nameSpace;
			this.PropertyName = name;
			this.SetProperty(new bool?(value));
		}

		protected PropertyBase()
		{
			this.PropertyId = Guid.NewGuid();
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[CommonMessageTraceSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[CommonMessageTraceSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public Guid PropertyId
		{
			get
			{
				return (Guid)this[PropertyBase.PropertyIdProperty];
			}
			set
			{
				this[PropertyBase.PropertyIdProperty] = value;
			}
		}

		public Guid ParentId
		{
			get
			{
				return (Guid)this[PropertyBase.ParentIdProperty];
			}
			set
			{
				this[PropertyBase.ParentIdProperty] = value;
			}
		}

		public Guid ExMessageId
		{
			get
			{
				return (Guid)this[CommonMessageTraceSchema.ExMessageIdProperty];
			}
			set
			{
				this[CommonMessageTraceSchema.ExMessageIdProperty] = value;
			}
		}

		public string Namespace
		{
			get
			{
				return (string)this[PropertyBase.NamespaceProperty];
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Namespace cannot be set to null value.");
				}
				this[PropertyBase.NamespaceProperty] = value;
			}
		}

		public string PropertyName
		{
			get
			{
				return (string)this[PropertyBase.PropertyNameProperty];
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Name cannot be null");
				}
				this[PropertyBase.PropertyNameProperty] = value;
			}
		}

		public int PropertyIndex
		{
			get
			{
				return (int)this[PropertyBase.PropertyIndexProperty];
			}
			set
			{
				this[PropertyBase.PropertyIndexProperty] = value;
			}
		}

		public Guid PropertyValueGuid
		{
			get
			{
				return (Guid)this[PropertyBase.PropertyValueGuidProperty];
			}
			set
			{
				this.SetProperty(value);
			}
		}

		public int? PropertyValueInteger
		{
			get
			{
				return (int?)this[PropertyBase.PropertyValueIntegerProperty];
			}
			set
			{
				this.SetProperty(value);
			}
		}

		public long? PropertyValueLong
		{
			get
			{
				return (long?)this[PropertyBase.PropertyValueLongProperty];
			}
			set
			{
				this.SetProperty(value);
			}
		}

		public string PropertyValueString
		{
			get
			{
				return (string)this[PropertyBase.PropertyValueStringProperty];
			}
			set
			{
				this.SetProperty(value);
			}
		}

		public DateTime? PropertyValueDatetime
		{
			get
			{
				return (DateTime?)this[PropertyBase.PropertyValueDatetimeProperty];
			}
			set
			{
				this.SetProperty(value);
			}
		}

		public bool? PropertyValueBit
		{
			get
			{
				return (bool?)this[PropertyBase.PropertyValueBitProperty];
			}
			set
			{
				this.SetProperty(value);
			}
		}

		public decimal? PropertyValueDecimal
		{
			get
			{
				return (decimal?)this[PropertyBase.PropertyValueDecimalProperty];
			}
			set
			{
				this.SetProperty(value);
			}
		}

		public BlobType PropertyValueBlob
		{
			get
			{
				return new BlobType((string)this[PropertyBase.PropertyValueBlobProperty]);
			}
			set
			{
				this[PropertyBase.PropertyValueBlobProperty] = value.Value;
			}
		}

		private void SetProperty(Guid value)
		{
			this[PropertyBase.PropertyValueGuidProperty] = value;
		}

		private void SetProperty(object value)
		{
			if (value != null)
			{
				Type type = value.GetType();
				if (type == typeof(BlobType))
				{
					this[PropertyBase.PropertyValueBlobProperty] = ((BlobType)value).Value;
					return;
				}
				if (type == typeof(bool))
				{
					this[PropertyBase.PropertyValueBitProperty] = (bool)value;
					return;
				}
				if (type == typeof(DateTime))
				{
					this[PropertyBase.PropertyValueDatetimeProperty] = (DateTime?)value;
					return;
				}
				if (type == typeof(decimal))
				{
					this[PropertyBase.PropertyValueDecimalProperty] = (decimal?)value;
					return;
				}
				if (type == typeof(float))
				{
					this[PropertyBase.PropertyValueDecimalProperty] = new decimal?((decimal)((float)value));
					return;
				}
				if (type == typeof(Guid))
				{
					this[PropertyBase.PropertyValueGuidProperty] = (Guid)value;
					return;
				}
				if (type == typeof(int))
				{
					this[PropertyBase.PropertyValueIntegerProperty] = (int?)value;
					return;
				}
				if (type == typeof(long))
				{
					this[PropertyBase.PropertyValueLongProperty] = (long?)value;
					return;
				}
				if (!(type == typeof(string)))
				{
					throw new NotSupportedException(string.Format("PropertyValueType {0} is not supported in PropertyBase", type));
				}
				string text = (string)value;
				if (!string.IsNullOrEmpty(text))
				{
					if (text.Length > 320)
					{
						this[PropertyBase.PropertyValueBlobProperty] = text;
						return;
					}
					this[PropertyBase.PropertyValueStringProperty] = text;
					return;
				}
			}
		}

		private void SetProperty(int? value)
		{
			this[PropertyBase.PropertyValueIntegerProperty] = value;
		}

		private void SetProperty(string value)
		{
			this[PropertyBase.PropertyValueStringProperty] = value;
		}

		private void SetProperty(long? value)
		{
			this[PropertyBase.PropertyValueLongProperty] = value;
		}

		private void SetProperty(DateTime? value)
		{
			this[PropertyBase.PropertyValueDatetimeProperty] = value;
		}

		private void SetProperty(bool? value)
		{
			this[PropertyBase.PropertyValueBitProperty] = value;
		}

		private void SetProperty(decimal? value)
		{
			this[PropertyBase.PropertyValueDecimalProperty] = value;
		}

		private void SetProperty(BlobType value)
		{
			this[PropertyBase.PropertyValueBlobProperty] = value.Value;
		}

		public const int MaxNumOfBytesInDBExtendedPropertyStringType = 320;

		internal static readonly HygienePropertyDefinition[] BaseProperties = new HygienePropertyDefinition[]
		{
			PropertyBase.PropertyIdProperty,
			PropertyBase.ParentIdProperty,
			CommonMessageTraceSchema.ExMessageIdProperty,
			PropertyBase.NamespaceProperty,
			PropertyBase.PropertyNameProperty,
			PropertyBase.PropertyIndexProperty,
			PropertyBase.PropertyValueGuidProperty,
			PropertyBase.PropertyValueIntegerProperty,
			PropertyBase.PropertyValueLongProperty,
			PropertyBase.PropertyValueStringProperty,
			PropertyBase.PropertyValueDatetimeProperty,
			PropertyBase.PropertyValueBitProperty,
			PropertyBase.PropertyValueDecimalProperty,
			PropertyBase.PropertyValueBlobProperty,
			PropertyBase.EventHashKeyProperty,
			PropertyBase.EmailHashKeyProperty,
			PropertyBase.ParentObjectIdProperty,
			PropertyBase.RefObjectIdProperty,
			PropertyBase.RefNameProperty,
			PropertyBase.PropIdProperty
		};

		internal static readonly HygienePropertyDefinition PropertyIdProperty = new HygienePropertyDefinition("PropertyId", typeof(Guid));

		internal static readonly HygienePropertyDefinition ParentIdProperty = new HygienePropertyDefinition("ParentId", typeof(Guid));

		internal static readonly HygienePropertyDefinition NamespaceProperty = new HygienePropertyDefinition("Namespace", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EventHashKeyProperty = CommonMessageTraceSchema.EventHashKeyProperty;

		internal static readonly HygienePropertyDefinition EmailHashKeyProperty = CommonMessageTraceSchema.EmailHashKeyProperty;

		internal static readonly HygienePropertyDefinition ParentObjectIdProperty = new HygienePropertyDefinition("ParentObjectId", typeof(Guid?));

		internal static readonly HygienePropertyDefinition RefObjectIdProperty = new HygienePropertyDefinition("RefObjectId", typeof(Guid?));

		internal static readonly HygienePropertyDefinition RefNameProperty = new HygienePropertyDefinition("RefName", typeof(string));

		internal static readonly HygienePropertyDefinition PropIdProperty = new HygienePropertyDefinition("PropId", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PropertyNameProperty = new HygienePropertyDefinition("PropertyName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PropertyIndexProperty = new HygienePropertyDefinition("PropertyIndex", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PropertyValueGuidProperty = new HygienePropertyDefinition("PropertyValueGuid", typeof(Guid?));

		internal static readonly HygienePropertyDefinition PropertyValueIntegerProperty = new HygienePropertyDefinition("PropertyValueInteger", typeof(int?));

		internal static readonly HygienePropertyDefinition PropertyValueLongProperty = new HygienePropertyDefinition("PropertyValueLong", typeof(long?));

		internal static readonly HygienePropertyDefinition PropertyValueStringProperty = new HygienePropertyDefinition("PropertyValueString", typeof(string));

		internal static readonly HygienePropertyDefinition PropertyValueDatetimeProperty = new HygienePropertyDefinition("PropertyValueDatetime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition PropertyValueBitProperty = new HygienePropertyDefinition("PropertyValueBit", typeof(bool?));

		internal static readonly HygienePropertyDefinition PropertyValueDecimalProperty = new HygienePropertyDefinition("PropertyValueDecimal", typeof(decimal?));

		internal static readonly HygienePropertyDefinition PropertyValueBlobProperty = new HygienePropertyDefinition("PropertyValueBlob", typeof(string));
	}
}
