using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class CmdletExtensionAgent : ADConfigurationObject, IComparable<CmdletExtensionAgent>
	{
		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return CmdletExtensionAgent.lastModified;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return CmdletExtensionAgent.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return CmdletExtensionAgent.mostDerivedClass;
			}
		}

		public string Assembly
		{
			get
			{
				return (string)this[CmdletExtensionAgentSchema.Assembly];
			}
			internal set
			{
				this[CmdletExtensionAgentSchema.Assembly] = value;
			}
		}

		public string ClassFactory
		{
			get
			{
				return (string)this[CmdletExtensionAgentSchema.ClassFactory];
			}
			internal set
			{
				this[CmdletExtensionAgentSchema.ClassFactory] = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[CmdletExtensionAgentSchema.Enabled];
			}
			internal set
			{
				this[CmdletExtensionAgentSchema.Enabled] = value;
			}
		}

		public byte Priority
		{
			get
			{
				return (byte)this[CmdletExtensionAgentSchema.Priority];
			}
			internal set
			{
				this[CmdletExtensionAgentSchema.Priority] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsSystem
		{
			get
			{
				return (bool)this[CmdletExtensionAgentSchema.IsSystem];
			}
			set
			{
				this[CmdletExtensionAgentSchema.IsSystem] = value;
			}
		}

		internal static object PriorityGetter(IPropertyBag propertyBag)
		{
			int value = (int)propertyBag[CmdletExtensionAgentSchema.CmdletExtensionFlags];
			return BitConverter.GetBytes(value)[2];
		}

		internal static void PrioritySetter(object value, IPropertyBag propertyBag)
		{
			int value2 = (int)propertyBag[CmdletExtensionAgentSchema.CmdletExtensionFlags];
			byte[] bytes = BitConverter.GetBytes(value2);
			bytes[2] = (byte)value;
			propertyBag[CmdletExtensionAgentSchema.CmdletExtensionFlags] = BitConverter.ToInt32(bytes, 0);
		}

		int IComparable<CmdletExtensionAgent>.CompareTo(CmdletExtensionAgent other)
		{
			if (other != null)
			{
				return (int)(this.Priority - other.Priority);
			}
			return -1;
		}

		private static CmdletExtensionAgentSchema schema = ObjectSchema.GetInstance<CmdletExtensionAgentSchema>();

		private static string mostDerivedClass = "msExchCmdletExtensionAgent";

		private static ExchangeObjectVersion lastModified = new ExchangeObjectVersion(4, 0, 14, 1, 166, 0);
	}
}
