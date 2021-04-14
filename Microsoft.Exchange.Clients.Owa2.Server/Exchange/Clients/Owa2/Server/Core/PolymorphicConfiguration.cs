using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PolymorphicConfiguration<T> : SimpleConfiguration<T> where T : new()
	{
		internal PolymorphicConfiguration()
		{
		}

		protected override void AddConfiguration(Type configurationType)
		{
			base.AddConfiguration(typeof(T));
			IEnumerable<KnownTypeAttribute> customAttributes = configurationType.GetCustomAttributes<KnownTypeAttribute>();
			foreach (KnownTypeAttribute knownTypeAttribute in customAttributes)
			{
				base.AddConfiguration(knownTypeAttribute.Type);
			}
		}

		protected override T CreateObject(Dictionary<string, string> attributes)
		{
			string typeName = null;
			if (attributes.TryGetValue("__PolymorphicConfiguration_Type", out typeName))
			{
				attributes.Remove("__PolymorphicConfiguration_Type");
				return (T)((object)Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, typeName).Unwrap());
			}
			throw new OwaInvalidOperationException(string.Format("The attribute {0} not found", "__PolymorphicConfiguration_Type"));
		}

		protected override void WriteCustomAttributes(XmlTextWriter xmlWriter, T entry)
		{
			xmlWriter.WriteAttributeString("__PolymorphicConfiguration_Type", entry.GetType().FullName);
		}

		private const string TypeAttributeName = "__PolymorphicConfiguration_Type";
	}
}
