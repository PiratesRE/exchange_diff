using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FUSE.Paxos;
using FUSE.Weld.Base;
using Microsoft.Exchange.Compliance.Serialization.Formatters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.DxStore.Server;

namespace Microsoft.Exchange.DxStore.Common
{
	public static class DxBinarySerializationUtil
	{
		public static void Serialize(MemoryStream ms, object obj)
		{
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			binaryFormatter.Serialize(ms, obj);
		}

		public static object DeserializeUnsafe(Stream s)
		{
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			return binaryFormatter.Deserialize(s);
		}

		public static object Deserialize(Stream s)
		{
			return DxBinarySerializationUtil.DeserializeSafe(s);
		}

		public static object DeserializeSafe(Stream s)
		{
			return TypedBinaryFormatter.DeserializeObject(s, DxBinarySerializationUtil.GetTypeBinder());
		}

		private static void LogErr(string formatString, params object[] args)
		{
			EventLogger.LogErr(formatString, args);
		}

		private static void WriteTypeEvent(Type type, bool allowed)
		{
			if (allowed)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder("Need to add type ");
			if (type == null)
			{
				stringBuilder.AppendLine("no types. how does that make sense?");
			}
			else
			{
				stringBuilder.AppendLine(type.FullName);
			}
			stringBuilder.AppendLine("here's the stack\n");
			stringBuilder.AppendLine(new StackTrace(true).ToString());
			DxBinarySerializationUtil.LogErr(stringBuilder.ToString(), new object[0]);
		}

		private static Type[] GetGenericTypeDefinitions(Type[] serializableTypes)
		{
			HashSet<Type> hashSet = new HashSet<Type>();
			foreach (string typeName in DxBinarySerializationUtil.builtinGenericTypes)
			{
				Type type = Type.GetType(typeName);
				if (type != null)
				{
					hashSet.Add(Type.GetType(typeName));
				}
			}
			foreach (Type type2 in serializableTypes)
			{
				try
				{
					if (type2.IsGenericTypeDefinition && !hashSet.Contains(type2))
					{
						hashSet.Add(type2);
					}
					else if (type2.IsConstructedGenericType)
					{
						Type genericTypeDefinition = type2.GetGenericTypeDefinition();
						if (genericTypeDefinition != null && !hashSet.Contains(genericTypeDefinition))
						{
							hashSet.Add(genericTypeDefinition);
						}
					}
				}
				catch (Exception ex)
				{
					DxBinarySerializationUtil.LogErr("SerializationTypeBinder GetGenericTypeDefinitions on type {0} failed: {1}.", new object[]
					{
						type2.FullName,
						ex.ToString()
					});
				}
			}
			return hashSet.ToArray<Type>();
		}

		private static TypedSerializationFormatter.TypeBinder GetTypeBinder()
		{
			if (DxBinarySerializationUtil.staticBinder == null)
			{
				lock (DxBinarySerializationUtil.binderLock)
				{
					if (DxBinarySerializationUtil.staticBinder == null)
					{
						DxBinarySerializationUtil.staticBinder = DxBinarySerializationUtil.ConstructBinder();
					}
				}
			}
			return DxBinarySerializationUtil.staticBinder;
		}

		private static TypedSerializationFormatter.TypeBinder ConstructBinder()
		{
			TypedSerializationFormatter.TypeBinder result = null;
			try
			{
				int tickCount = Environment.TickCount;
				Type[] serializableTypes = DxBinarySerializationUtil.GetSerializableTypes();
				Type[] genericTypeDefinitions = DxBinarySerializationUtil.GetGenericTypeDefinitions(serializableTypes);
				result = new TypedSerializationFormatter.TypeBinder(serializableTypes, DxBinarySerializationUtil.safeBaseClasses, genericTypeDefinitions, new TypedSerializationFormatter.TypeEncounteredDelegate(DxBinarySerializationUtil.WriteTypeEvent), true);
				DxBinarySerializationUtil.LogErr("SerializationTypeBinder create TypeBinder succeeded in {0} ms with {1} serializable types and {2} generic types.", new object[]
				{
					Environment.TickCount - tickCount,
					serializableTypes.Length,
					genericTypeDefinitions.Length
				});
			}
			catch (Exception ex)
			{
				DxBinarySerializationUtil.LogErr("SerializationTypeBinder create TypeBinder failed: {0}.", new object[]
				{
					ex.ToString()
				});
			}
			return result;
		}

		private static Type[] GetSerializableTypes()
		{
			HashSet<Type> hashSet = new HashSet<Type>();
			try
			{
				foreach (Type type in DxBinarySerializationUtil.expectedTypes)
				{
					DxBinarySerializationUtil.ExpandSerializableTypes(hashSet, type);
				}
			}
			catch (Exception ex)
			{
				DxBinarySerializationUtil.LogErr("SerializationTypeBinder initialize type in current appdomain failed: {0}.", new object[]
				{
					ex.ToString()
				});
			}
			return hashSet.ToArray<Type>();
		}

		private static void ExpandSerializableTypes(HashSet<Type> serializableTypes, Type type)
		{
			if (serializableTypes.Contains(type))
			{
				return;
			}
			serializableTypes.Add(type);
			try
			{
				if (type.IsArray)
				{
					DxBinarySerializationUtil.ExpandSerializableTypes(serializableTypes, type.GetElementType());
				}
			}
			catch (Exception ex)
			{
				DxBinarySerializationUtil.LogErr("SerializationTypeBinder load array element type on type {0} failed: {1}.", new object[]
				{
					type.FullName,
					ex.ToString()
				});
			}
			try
			{
				if (type.IsConstructedGenericType)
				{
					foreach (Type type2 in type.GetGenericArguments())
					{
						DxBinarySerializationUtil.ExpandSerializableTypes(serializableTypes, type2);
					}
				}
			}
			catch (Exception ex2)
			{
				DxBinarySerializationUtil.LogErr("SerializationTypeBinder load generic type definition on type {0} failed: {1}.", new object[]
				{
					type.FullName,
					ex2.ToString()
				});
			}
			try
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (fields != null && fields.Length > 0)
				{
					foreach (FieldInfo fieldInfo in from x in fields
					where !x.IsNotSerialized && !serializableTypes.Contains(x.FieldType) && x.FieldType.IsSerializable
					select x)
					{
						try
						{
							DxBinarySerializationUtil.ExpandSerializableTypes(serializableTypes, fieldInfo.FieldType);
						}
						catch (Exception ex3)
						{
							DxBinarySerializationUtil.LogErr("SerializationTypeBinder load field {0} on type {1} failed: {2}.", new object[]
							{
								fieldInfo.Name,
								type.FullName,
								ex3.ToString()
							});
						}
					}
				}
			}
			catch (Exception ex4)
			{
				DxBinarySerializationUtil.LogErr("SerializationTypeBinder load fields on type {0} failed: {1}.", new object[]
				{
					type.FullName,
					ex4.ToString()
				});
			}
			try
			{
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (properties != null && properties.Length > 0)
				{
					foreach (PropertyInfo propertyInfo in properties)
					{
						try
						{
							DxBinarySerializationUtil.ExpandSerializableTypes(serializableTypes, propertyInfo.PropertyType);
						}
						catch (Exception ex5)
						{
							DxBinarySerializationUtil.LogErr("SerializationTypeBinder load property {0} on type {1} failed: {2}.", new object[]
							{
								propertyInfo.Name,
								type.FullName,
								ex5.ToString()
							});
						}
					}
				}
			}
			catch (Exception ex6)
			{
				DxBinarySerializationUtil.LogErr("SerializationTypeBinder load properties on type {0} failed: {1}.", new object[]
				{
					type.FullName,
					ex6.ToString()
				});
			}
			try
			{
				if (type.BaseType != null && type.BaseType != typeof(object))
				{
					DxBinarySerializationUtil.ExpandSerializableTypes(serializableTypes, type.BaseType);
				}
			}
			catch (Exception ex7)
			{
				DxBinarySerializationUtil.LogErr("SerializationTypeBinder load base type on type {0} failed: {1}.", new object[]
				{
					type.FullName,
					ex7.ToString()
				});
			}
		}

		private static readonly Type[] safeBaseClasses = new Type[0];

		private static readonly Type[] expectedTypes = new Type[]
		{
			typeof(Configuration),
			typeof(Message),
			typeof(Message.Accepted),
			typeof(Message.Gossip),
			typeof(Message.Initiate),
			typeof(Message.LeaderAnnounce),
			typeof(Message.Learn),
			typeof(Message.MessageRound),
			typeof(Message.MessageValue),
			typeof(Message.Prepare),
			typeof(Message.Promise),
			typeof(Message.Propose),
			typeof(Message.Query),
			typeof(Message.RejectionHint),
			typeof(Proposal),
			typeof(Proposal),
			typeof(ProposalConfiguration),
			typeof(ProposalNoOperation),
			typeof(Round),
			typeof(CachingHttpHandler.ExceptionUri),
			typeof(Ranges),
			typeof(Ranges.Range),
			typeof(WCF.Routing.Target),
			typeof(CommonSettings),
			typeof(DataStoreStats),
			typeof(DxStoreAccessClientException),
			typeof(DxStoreAccessClientTransientException),
			typeof(DxStoreAccessReply),
			typeof(DxStoreAccessReply.CheckKey),
			typeof(DxStoreAccessReply.DeleteKey),
			typeof(DxStoreAccessReply.DeleteProperty),
			typeof(DxStoreAccessReply.ExecuteBatch),
			typeof(DxStoreAccessReply.GetAllProperties),
			typeof(DxStoreAccessReply.GetProperty),
			typeof(DxStoreAccessReply.GetPropertyNames),
			typeof(DxStoreAccessReply.GetSubkeyNames),
			typeof(DxStoreAccessReply.SetProperty),
			typeof(DxStoreAccessRequest),
			typeof(DxStoreAccessRequest.CheckKey),
			typeof(DxStoreAccessRequest.DeleteKey),
			typeof(DxStoreAccessRequest.DeleteProperty),
			typeof(DxStoreAccessRequest.ExecuteBatch),
			typeof(DxStoreAccessRequest.GetAllProperties),
			typeof(DxStoreAccessRequest.GetProperty),
			typeof(DxStoreAccessRequest.GetPropertyNames),
			typeof(DxStoreAccessRequest.GetSubkeyNames),
			typeof(DxStoreAccessRequest.SetProperty),
			typeof(DxStoreAccessServerTransientException),
			typeof(DxStoreBatchCommand),
			typeof(DxStoreBatchCommand.CreateKey),
			typeof(DxStoreBatchCommand.DeleteKey),
			typeof(DxStoreBatchCommand.DeleteProperty),
			typeof(DxStoreBatchCommand.SetProperty),
			typeof(DxStoreBindingNotSupportedException),
			typeof(DxStoreClientException),
			typeof(DxStoreClientTransientException),
			typeof(DxStoreCommand),
			typeof(DxStoreCommand.ApplySnapshot),
			typeof(DxStoreCommand.CreateKey),
			typeof(DxStoreCommand.DeleteKey),
			typeof(DxStoreCommand.DeleteProperty),
			typeof(DxStoreCommand.DummyCommand),
			typeof(DxStoreCommand.ExecuteBatch),
			typeof(DxStoreCommand.PromoteToLeader),
			typeof(DxStoreCommand.SetProperty),
			typeof(DxStoreCommandConstraintFailedException),
			typeof(DxStoreInstanceClientException),
			typeof(DxStoreInstanceClientTransientException),
			typeof(DxStoreInstanceComponentNotInitializedException),
			typeof(DxStoreInstanceKeyNotFoundException),
			typeof(DxStoreInstanceNotReadyException),
			typeof(DxStoreInstanceServerException),
			typeof(DxStoreInstanceServerTransientException),
			typeof(DxStoreInstanceStaleStoreException),
			typeof(DxStoreManagerClientException),
			typeof(DxStoreManagerClientTransientException),
			typeof(DxStoreManagerGroupNotFoundException),
			typeof(DxStoreManagerServerException),
			typeof(DxStoreManagerServerTransientException),
			typeof(DxStoreReplyBase),
			typeof(DxStoreRequestBase),
			typeof(DxStoreSerializeException),
			typeof(DxStoreServerException),
			typeof(DxStoreServerFault),
			typeof(DxStoreServerTransientException),
			typeof(HttpReply),
			typeof(HttpReply.DxStoreReply),
			typeof(HttpReply.ExceptionReply),
			typeof(HttpReply.GetInstanceStatusReply),
			typeof(HttpRequest),
			typeof(HttpRequest.DxStoreRequest),
			typeof(HttpRequest.GetStatusRequest),
			typeof(HttpRequest.GetStatusRequest.Reply),
			typeof(HttpRequest.PaxosMessage),
			typeof(InstanceGroupConfig),
			typeof(InstanceGroupMemberConfig),
			typeof(InstanceGroupSettings),
			typeof(InstanceManagerConfig),
			typeof(InstanceSnapshotInfo),
			typeof(InstanceStatusInfo),
			typeof(LocDescriptionAttribute),
			typeof(PaxosBasicInfo),
			typeof(PaxosBasicInfo.GossipDictionary),
			typeof(ProcessBasicInfo),
			typeof(PropertyNameInfo),
			typeof(PropertyValue),
			typeof(ReadOptions),
			typeof(ReadResult),
			typeof(WcfTimeout),
			typeof(WriteOptions),
			typeof(WriteResult),
			typeof(WriteResult.ResponseInfo),
			typeof(GroupStatusInfo),
			typeof(GroupStatusInfo.NodeInstancePair),
			typeof(InstanceMigrationInfo),
			typeof(KeyContainer),
			typeof(DateTimeOffset),
			typeof(EqualityComparer<>),
			typeof(List<>)
		};

		private static readonly string[] builtinGenericTypes = new string[]
		{
			"System.Collections.Generic.ObjectEqualityComparer`1",
			"System.Collections.Generic.EnumEqualityComparer`1",
			"System.Collections.Generic.GenericEqualityComparer`1",
			"System.Collections.Generic.KeyValuePair`2"
		};

		private static TypedSerializationFormatter.TypeBinder staticBinder = null;

		private static object binderLock = new object();
	}
}
