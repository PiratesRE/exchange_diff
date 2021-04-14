using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MigrationWorkflowService;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceDataContractResolver : DataContractResolver
	{
		public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
		{
			Type type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
			if (type != null)
			{
				return type;
			}
			if (typeNamespace.StartsWith("lb:"))
			{
				string arg = typeNamespace.Split(new char[]
				{
					':'
				}, 2)[1];
				try
				{
					return Type.GetType(string.Format("{0}, {1}", typeName, arg));
				}
				catch (IOException arg2)
				{
					ExTraceGlobals.MailboxLoadBalanceTracer.TraceError<string, string, IOException>(0L, "Failed to load type {0}, {1}: {2}", typeName, arg, arg2);
				}
			}
			return null;
		}

		public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
		{
			if (!knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace))
			{
				lock (this.dictionaryLock)
				{
					if (!this.typeDictionary.TryLookup(type.FullName, out typeName))
					{
						typeName = this.typeDictionary.Add(type.FullName);
					}
					string value = string.Format("lb:{0}", type.Assembly.FullName);
					if (!this.namespaceDictionary.TryLookup(value, out typeNamespace))
					{
						typeNamespace = this.namespaceDictionary.Add(value);
					}
				}
			}
			return true;
		}

		private readonly object dictionaryLock = new object();

		private readonly XmlDictionary typeDictionary = new XmlDictionary();

		private readonly XmlDictionary namespaceDictionary = new XmlDictionary();
	}
}
