using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public sealed class ADSchemaAttributeObject : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADSchemaAttributeObject.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "attributeSchema";
			}
		}

		public Guid SchemaIDGuid
		{
			get
			{
				return (Guid)this[ADSchemaAttributeSchema.SchemaIDGuid];
			}
		}

		public int? RangeUpper
		{
			get
			{
				return (int?)this[ADSchemaAttributeSchema.RangeUpper];
			}
		}

		public int? RangeLower
		{
			get
			{
				return (int?)this[ADSchemaAttributeSchema.RangeLower];
			}
		}

		public int LinkID
		{
			get
			{
				return (int)this[ADSchemaAttributeSchema.LinkID];
			}
		}

		public int MapiID
		{
			get
			{
				return (int)this[ADSchemaAttributeSchema.MapiID];
			}
		}

		public string LdapDisplayName
		{
			get
			{
				return (string)this[ADSchemaAttributeSchema.LdapDisplayName];
			}
		}

		public AttributeSyntax OMSyntax
		{
			get
			{
				return (AttributeSyntax)this[ADSchemaAttributeSchema.OMSyntax];
			}
		}

		public bool IsMemberOfPartialAttributeSet
		{
			get
			{
				return (bool)this[ADSchemaAttributeSchema.IsMemberOfPartialAttributeSet];
			}
		}

		public bool IsSingleValued
		{
			get
			{
				return (bool)this[ADSchemaAttributeSchema.IsSingleValued];
			}
		}

		public string AttributeID
		{
			get
			{
				return (string)this[ADSchemaAttributeSchema.AttributeID];
			}
		}

		public DataSyntax DataSyntax
		{
			get
			{
				return (DataSyntax)this[ADSchemaAttributeSchema.DataSyntax];
			}
		}

		internal static object SyntaxGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ADSchemaAttributeSchema.RawAttributeSyntax];
			int num = (int)((AttributeSyntax)propertyBag[ADSchemaAttributeSchema.OMSyntax]);
			byte[] values = (byte[])propertyBag[ADSchemaAttributeSchema.OMObjectClass];
			DataSyntax dataSyntax = DataSyntax.UnDefined;
			string key;
			switch (key = text)
			{
			case "2.5.5.0":
				dataSyntax = DataSyntax.UnDefined;
				break;
			case "2.5.5.1":
				dataSyntax = DataSyntax.DSDN;
				break;
			case "2.5.5.2":
				dataSyntax = DataSyntax.ObjectIdentifier;
				break;
			case "2.5.5.3":
				dataSyntax = DataSyntax.CaseSensitive;
				break;
			case "2.5.5.4":
				dataSyntax = DataSyntax.Teletex;
				break;
			case "2.5.5.5":
				if (num == 19)
				{
					dataSyntax = DataSyntax.Printable;
				}
				else if (num == 22)
				{
					dataSyntax = DataSyntax.IA5;
				}
				break;
			case "2.5.5.6":
				dataSyntax = DataSyntax.Numeric;
				break;
			case "2.5.5.7":
			{
				string y = ADSchemaAttributeObject.ToHexString(values);
				if (StringComparer.OrdinalIgnoreCase.Equals("2A864886F7140101010B", y))
				{
					dataSyntax = DataSyntax.DNBinary;
				}
				else if (StringComparer.OrdinalIgnoreCase.Equals("56060102050B1D", y))
				{
					dataSyntax = DataSyntax.ORName;
				}
				break;
			}
			case "2.5.5.8":
				dataSyntax = DataSyntax.Boolean;
				break;
			case "2.5.5.9":
				if (num == 2)
				{
					dataSyntax = DataSyntax.Integer;
				}
				else if (num == 10)
				{
					dataSyntax = DataSyntax.Enumeration;
				}
				break;
			case "2.5.5.10":
				if (num == 4)
				{
					dataSyntax = DataSyntax.Octet;
				}
				else if (num == 127)
				{
					dataSyntax = DataSyntax.ReplicaLink;
				}
				break;
			case "2.5.5.11":
				if (num == 23)
				{
					dataSyntax = DataSyntax.UTCTime;
				}
				else if (num == 24)
				{
					dataSyntax = DataSyntax.GeneralizedTime;
				}
				break;
			case "2.5.5.12":
				dataSyntax = DataSyntax.Unicode;
				break;
			case "2.5.5.13":
				dataSyntax = DataSyntax.PresentationAddress;
				break;
			case "2.5.5.14":
			{
				string y = ADSchemaAttributeObject.ToHexString(values);
				if (StringComparer.OrdinalIgnoreCase.Equals("2B0C0287731C00853E", y))
				{
					dataSyntax = DataSyntax.AccessPoint;
				}
				else if (StringComparer.OrdinalIgnoreCase.Equals("2A864886F7140101010C", y))
				{
					dataSyntax = DataSyntax.DNString;
				}
				break;
			}
			case "2.5.5.15":
				dataSyntax = DataSyntax.NTSecDesc;
				break;
			case "2.5.5.16":
				dataSyntax = DataSyntax.LargeInteger;
				break;
			case "2.5.5.17":
				dataSyntax = DataSyntax.Sid;
				break;
			}
			return dataSyntax;
		}

		private static string ToHexString(byte[] values)
		{
			if (values == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(values.Length * 2);
			foreach (byte b in values)
			{
				stringBuilder.AppendFormat("{0:X2}", b);
			}
			return stringBuilder.ToString();
		}

		private const string MostDerivedClass = "attributeSchema";

		private const string DNBinaryOmObjectClassString = "2A864886F7140101010B";

		private const string ORNameOmObjectClassString = "56060102050B1D";

		private const string AccessPointOmObjectClassString = "2B0C0287731C00853E";

		private const string DNStringOmObjectClassString = "2A864886F7140101010C";

		private static ADSchemaAttributeSchema schema = ObjectSchema.GetInstance<ADSchemaAttributeSchema>();
	}
}
