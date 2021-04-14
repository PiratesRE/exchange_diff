using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SafeLocalizedStringSerializer
	{
		public static byte[] SafeSerialize(LocalizedString localizedString)
		{
			if (localizedString.Equals(default(LocalizedString)))
			{
				return null;
			}
			byte[] result;
			try
			{
				BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(new SafeLocalizedStringSerializer.ValidatingBinder());
				using (MemoryStream memoryStream = new MemoryStream())
				{
					binaryFormatter.Serialize(memoryStream, localizedString);
					result = memoryStream.ToArray();
				}
			}
			catch (BlockedTypeException ex)
			{
				SafeLocalizedStringSerializer.CreateWatson(ex);
				result = SafeLocalizedStringSerializer.SafeSerialize(new LocalizedString(localizedString.ToString()));
			}
			return result;
		}

		public static LocalizedString SafeDeserialize(byte[] bytes)
		{
			if (bytes == null)
			{
				return default(LocalizedString);
			}
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(new SafeLocalizedStringSerializer.ValidatingBinder());
			object obj;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				try
				{
					obj = binaryFormatter.Deserialize(memoryStream);
				}
				catch (BlockedTypeException ex)
				{
					SafeLocalizedStringSerializer.CreateWatson(ex);
					return ex.LocalizedString;
				}
			}
			if (!(obj is LocalizedString))
			{
				return new LocalizedString((obj == null) ? "null" : obj.ToString());
			}
			return (LocalizedString)obj;
		}

		private static void CreateWatson(BlockedTypeException ex)
		{
			MrsTracer.Common.Error("Unhandled type in SafeLocalizedStringSerializer:\n{0}\n{1}", new object[]
			{
				CommonUtils.FullExceptionMessage(ex),
				ex.StackTrace
			});
			lock (SafeLocalizedStringSerializer.typesThatGeneratedWatson)
			{
				if (SafeLocalizedStringSerializer.typesThatGeneratedWatson.Contains(ex.Type))
				{
					return;
				}
				SafeLocalizedStringSerializer.typesThatGeneratedWatson.Add(ex.Type);
			}
			ExWatson.SendReport(ex);
		}

		private static readonly HashSet<string> typesThatGeneratedWatson = new HashSet<string>();

		private sealed class ValidatingBinder : SerializationBinder
		{
			public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
			{
				if (serializedType != null && !serializedType.IsEnum && !SafeLocalizedStringSerializer.ValidatingBinder.whiteList.Contains(serializedType))
				{
					MrsTracer.Common.Error("SafeLocalizedStringSerializer.BindToName: Type = '{0}'", new object[]
					{
						serializedType
					});
					throw new BlockedTypeException(serializedType.ToString());
				}
				base.BindToName(serializedType, out assemblyName, out typeName);
			}

			public override Type BindToType(string assemblyName, string typeName)
			{
				Type type = null;
				try
				{
					type = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
				}
				catch (Exception ex)
				{
					MrsTracer.Common.Error("SafeLocalizedStringSerializer.BindToType: failed to get the type: {0}", new object[]
					{
						ex
					});
				}
				if (type != null && !type.IsEnum && !SafeLocalizedStringSerializer.ValidatingBinder.whiteList.Contains(type))
				{
					MrsTracer.Common.Error("SafeLocalizedStringSerializer.BindToType: Failed type name = '{0}', assembly name = '{1}', type = '{2}'", new object[]
					{
						typeName,
						assemblyName,
						type
					});
					throw new BlockedTypeException(type.ToString());
				}
				return type;
			}

			private static readonly HashSet<Type> whiteList = new HashSet<Type>(new Type[]
			{
				typeof(string),
				typeof(int),
				typeof(uint),
				typeof(long),
				typeof(ulong),
				typeof(double),
				typeof(LocalizedString),
				typeof(Guid),
				typeof(DateTime),
				typeof(TimeSpan),
				typeof(TimeZone),
				typeof(ExDateTime),
				typeof(ExTimeZone),
				typeof(float),
				typeof(bool),
				typeof(short),
				typeof(ushort),
				typeof(byte),
				typeof(char),
				typeof(object)
			});
		}
	}
}
