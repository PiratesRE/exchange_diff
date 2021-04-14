using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public class SafeSerialization
	{
		public static object SafeBinaryFormatterDeserializeWithAllowList(Stream stream, IEnumerable<Type> allowList, SafeSerialization.TypeEncounteredDelegate typeEncounteredCallback = null)
		{
			SafeSerialization.ValidatingBinder binder = new SafeSerialization.ValidatingBinder(new SafeSerialization.AllowList(allowList), typeEncounteredCallback);
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(binder);
			return binaryFormatter.Deserialize(stream);
		}

		public static bool IsSafeBinaryFormatterStreamWithAllowList(Stream serializationStream, IEnumerable<Type> allowList, SafeSerialization.TypeEncounteredDelegate typeEncounteredCallback = null)
		{
			return SafeSerialization.IsSafeBinaryFormatterStreamCommon(new SafeSerialization.ValidatingBinder(new SafeSerialization.AllowList(allowList), typeEncounteredCallback), serializationStream);
		}

		private static bool IsSafeBinaryFormatterStreamCommon(SafeSerialization.ValidatingBinder binder, Stream serializationStream)
		{
			long position = serializationStream.Position;
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(binder);
			try
			{
				binaryFormatter.Deserialize(serializationStream);
			}
			catch (SafeSerialization.BlockedTypeException)
			{
				return false;
			}
			finally
			{
				serializationStream.Seek(position, SeekOrigin.Begin);
			}
			return true;
		}

		public delegate void TypeEncounteredDelegate(string fullName, SafeSerialization.FilterDecision decision);

		public enum FilterDecision
		{
			Allow,
			Deny
		}

		public class AllowList
		{
			public AllowList(IEnumerable<Type> allowList)
			{
				this.List = allowList;
			}

			public IEnumerable<Type> List { get; private set; }
		}

		[Serializable]
		public sealed class BlockedTypeException : ApplicationException
		{
			public BlockedTypeException(string message) : base(message)
			{
			}

			private BlockedTypeException(SerializationInfo si, StreamingContext sc) : base(si, sc)
			{
			}
		}

		private sealed class ValidatingBinder : SerializationBinder
		{
			public ValidatingBinder(SafeSerialization.AllowList allowList, SafeSerialization.TypeEncounteredDelegate typeEncounteredCallback)
			{
				this.allowedTypes = ((allowList != null) ? new HashSet<Type>(allowList.List) : new HashSet<Type>());
				this.typeFoundCallback = typeEncounteredCallback;
			}

			public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
			{
				throw new NotImplementedException();
			}

			public override Type BindToType(string assemblyName, string typeName)
			{
				Type type = Type.GetType(string.Format(CultureInfo.InvariantCulture, "{0}, {1}", new object[]
				{
					typeName,
					assemblyName
				}));
				string text = (type != null) ? type.FullName : string.Empty;
				SafeSerialization.FilterDecision filterDecision = SafeSerialization.FilterDecision.Deny;
				if (type != null && this.allowedTypes.Contains(type))
				{
					filterDecision = SafeSerialization.FilterDecision.Allow;
				}
				if (this.typeFoundCallback != null)
				{
					this.typeFoundCallback(text, filterDecision);
				}
				if (filterDecision == SafeSerialization.FilterDecision.Deny)
				{
					throw new SafeSerialization.BlockedTypeException(text);
				}
				return type;
			}

			private HashSet<Type> allowedTypes;

			private SafeSerialization.TypeEncounteredDelegate typeFoundCallback;
		}
	}
}
