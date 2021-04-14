using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceDataContractSerializationBehavior : DataContractSerializerOperationBehavior
	{
		public LoadBalanceDataContractSerializationBehavior(OperationDescription operation) : base(operation)
		{
			base.DataContractResolver = new LoadBalanceDataContractResolver();
			base.DataContractSurrogate = new LoadBalanceDataContractSurrogate();
		}

		public LoadBalanceDataContractSerializationBehavior(OperationDescription operation, DataContractFormatAttribute dataContractFormatAttribute) : base(operation, dataContractFormatAttribute)
		{
			base.DataContractResolver = new LoadBalanceDataContractResolver();
			base.DataContractSurrogate = new LoadBalanceDataContractSurrogate();
		}

		public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
		{
			return new DataContractSerializer(type, name, ns, knownTypes, int.MaxValue, false, true, base.DataContractSurrogate, base.DataContractResolver);
		}

		public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
		{
			return new DataContractSerializer(type, name, ns, knownTypes, int.MaxValue, false, true, base.DataContractSurrogate, base.DataContractResolver);
		}
	}
}
