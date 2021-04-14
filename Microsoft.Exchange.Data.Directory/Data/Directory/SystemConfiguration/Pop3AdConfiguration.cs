using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class Pop3AdConfiguration : PopImapAdConfiguration
	{
		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return Pop3AdConfiguration.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return Pop3AdConfiguration.MostDerivedClass;
			}
		}

		public override string ProtocolName
		{
			get
			{
				return "POP3";
			}
		}

		[Parameter(Mandatory = false)]
		public override int MaxCommandSize
		{
			get
			{
				return (int)this[Pop3AdConfigurationSchema.MaxCommandSize];
			}
			set
			{
				this[Pop3AdConfigurationSchema.MaxCommandSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SortOrder MessageRetrievalSortOrder
		{
			get
			{
				return (SortOrder)this[Pop3AdConfigurationSchema.MessageRetrievalSortOrder];
			}
			set
			{
				this[Pop3AdConfigurationSchema.MessageRetrievalSortOrder] = value;
			}
		}

		internal static object MessageRetrievalSortOrderGetter(IPropertyBag propertyBag)
		{
			int num = 1;
			ADPropertyDefinition popImapFlags = PopImapAdConfigurationSchema.PopImapFlags;
			if ((num & (int)propertyBag[popImapFlags]) != 0)
			{
				return SortOrder.Descending;
			}
			return SortOrder.Ascending;
		}

		internal static void MessageRetrievalSortOrderSetter(object value, IPropertyBag propertyBag)
		{
			int num = 1;
			ADPropertyDefinition popImapFlags = PopImapAdConfigurationSchema.PopImapFlags;
			if ((SortOrder)value == SortOrder.Descending)
			{
				propertyBag[popImapFlags] = ((int)propertyBag[popImapFlags] | num);
				return;
			}
			propertyBag[popImapFlags] = ((int)propertyBag[popImapFlags] & ~num);
		}

		private static Pop3AdConfigurationSchema schema = ObjectSchema.GetInstance<Pop3AdConfigurationSchema>();

		public static string MostDerivedClass = "protocolCfgPOPServer";
	}
}
