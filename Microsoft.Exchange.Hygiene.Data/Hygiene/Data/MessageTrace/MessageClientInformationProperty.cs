﻿using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal sealed class MessageClientInformationProperty : PropertyBase
	{
		public MessageClientInformationProperty(string nameSpace, string name, object value) : base(nameSpace, name, value)
		{
		}

		public MessageClientInformationProperty(string nameSpace, string name, int value) : base(nameSpace, name, new int?(value))
		{
		}

		public MessageClientInformationProperty(string nameSpace, string name, string value) : base(nameSpace, name, value)
		{
		}

		public MessageClientInformationProperty(string nameSpace, string name, DateTime value) : base(nameSpace, name, new DateTime?(value))
		{
		}

		public MessageClientInformationProperty(string nameSpace, string name, decimal value) : base(nameSpace, name, new decimal?(value))
		{
		}

		public MessageClientInformationProperty(string nameSpace, string name, BlobType value) : base(nameSpace, name, value)
		{
		}

		public MessageClientInformationProperty(string nameSpace, string name, Guid value) : base(nameSpace, name, value)
		{
		}

		public MessageClientInformationProperty(string nameSpace, string name, long value) : base(nameSpace, name, new long?(value))
		{
		}

		public MessageClientInformationProperty(string nameSpace, string name, bool value) : base(nameSpace, name, value)
		{
		}

		public MessageClientInformationProperty()
		{
		}

		public Guid ClientInformationPropertyId
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

		public Guid ClientInformationId
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

		public override void Accept(IMessageTraceVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override HygienePropertyDefinition[] GetAllProperties()
		{
			return MessageClientInformationProperty.Properties;
		}

		internal static readonly HygienePropertyDefinition[] Properties = new HygienePropertyDefinition[]
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
	}
}
