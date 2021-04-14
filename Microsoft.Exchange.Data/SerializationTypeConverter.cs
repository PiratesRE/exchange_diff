using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Compliance.Serialization.Formatters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data;

namespace Microsoft.Exchange.Data
{
	public class SerializationTypeConverter : PSTypeConverter
	{
		public override bool CanConvertFrom(object sourceValue, Type destinationType)
		{
			byte[] array;
			string text;
			Exception ex;
			return this.CanConvert(sourceValue, destinationType, out array, out text, out ex);
		}

		public override object ConvertFrom(object sourceValue, Type destinationType, IFormatProvider formatProvider, bool ignoreCase)
		{
			return this.DeserializeObject(sourceValue, destinationType);
		}

		public override bool CanConvertTo(object sourceValue, Type destinationType)
		{
			byte[] array;
			string text;
			Exception ex;
			return this.CanConvert(sourceValue, destinationType, out array, out text, out ex);
		}

		public override object ConvertTo(object sourceValue, Type destinationType, IFormatProvider formatProvider, bool ignoreCase)
		{
			return this.DeserializeObject(sourceValue, destinationType);
		}

		private bool CanConvert(object sourceValue, Type destinationType, out byte[] serializationData, out string stringValue, out Exception error)
		{
			serializationData = null;
			stringValue = null;
			error = null;
			ExTraceGlobals.SerializationTracer.TraceFunction((long)this.GetHashCode(), "SerializationTypeConverter.CanConvert({0}, {1})", new object[]
			{
				sourceValue ?? "<null>",
				destinationType ?? "<null>"
			});
			if (null == destinationType)
			{
				error = new ArgumentNullException("destinationType");
				return false;
			}
			if (sourceValue == null)
			{
				error = new ArgumentNullException("sourceValue");
				return false;
			}
			PSObject psobject = sourceValue as PSObject;
			if (psobject == null)
			{
				error = new NotSupportedException(DataStrings.ExceptionUnsupportedSourceType(sourceValue, sourceValue.GetType()));
				return false;
			}
			if (!SerializationTypeConverter.CanSerialize(destinationType))
			{
				error = new NotSupportedException(DataStrings.ExceptionUnsupportedTypeConversion(destinationType.FullName));
				return false;
			}
			if (psobject.Properties["SerializationData"] == null)
			{
				error = new NotSupportedException(DataStrings.ExceptionSerializationDataAbsent);
				return false;
			}
			object value = psobject.Properties["SerializationData"].Value;
			if (!(value is byte[]))
			{
				error = new NotSupportedException(DataStrings.ExceptionUnsupportedDataFormat(value));
				return false;
			}
			stringValue = psobject.ToString();
			serializationData = (value as byte[]);
			return true;
		}

		private object DeserializeObject(object sourceValue, Type destinationType)
		{
			Exception ex = null;
			byte[] array;
			string text;
			if (!this.CanConvert(sourceValue, destinationType, out array, out text, out ex))
			{
				throw ex;
			}
			ExTraceGlobals.SerializationTracer.TraceFunction<object, string>((long)this.GetHashCode(), "SerializationTypeConverter.DeserializeObject(); SerializationData.Length = {0}; ToStringValue = '{1}'", (array == null) ? "<null>" : array.Length, text);
			object obj = null;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(array))
				{
					AppDomain.CurrentDomain.AssemblyResolve += SerializationTypeConverter.AssemblyHandler;
					try
					{
						int tickCount = Environment.TickCount;
						if (SerializationTypeConverter.IsRunningInRPSServerSide.Value)
						{
							if (SerializationTypeBinder.Instance == null)
							{
								throw new InvalidCastException("SerializationTypeBinder initialization failed.");
							}
							obj = TypedBinaryFormatter.DeserializeObject(memoryStream, SerializationTypeBinder.Instance);
						}
						else
						{
							BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null, new string[]
							{
								"System.Management.Automation.PSObject",
								"System.DelegateSerializationHolder"
							});
							obj = binaryFormatter.Deserialize(memoryStream);
						}
						ExTraceGlobals.SerializationTracer.TraceDebug<string, int>((long)this.GetHashCode(), "DeserializeObject of type {0} succeeded in {1} ms.", (obj != null) ? obj.GetType().Name : "null", Environment.TickCount - tickCount);
						IDeserializationCallback deserializationCallback = obj as IDeserializationCallback;
						if (deserializationCallback != null)
						{
							deserializationCallback.OnDeserialization(sourceValue);
						}
					}
					finally
					{
						AppDomain.CurrentDomain.AssemblyResolve -= SerializationTypeConverter.AssemblyHandler;
					}
				}
			}
			catch (SerializationException ex2)
			{
				ExTraceGlobals.SerializationTracer.TraceDebug<SerializationException>((long)this.GetHashCode(), "Deserialization Failed. Error = {0}", ex2);
				if (ValueConvertor.TryConvertValueFromString(text, destinationType, null, out obj, out ex))
				{
					ExTraceGlobals.SerializationTracer.TraceDebug<SerializationException>((long)this.GetHashCode(), "String Conversion Succeeded.", ex2);
				}
				else
				{
					ex = new InvalidCastException(string.Format("Deserialization fails due to one SerializationException: {0}", ex2.ToString()), ex2);
				}
			}
			catch (DataSourceTransientException ex3)
			{
				ex = new InvalidCastException(string.Format("Deserialization fails due to an DataSourceTransientException: {0}", ex3.ToString()), ex3);
			}
			if (ex is InvalidCastException)
			{
				throw ex;
			}
			return obj;
		}

		public static void RegisterAssemblyResolver()
		{
			AppDomain.CurrentDomain.AssemblyResolve += SerializationTypeConverter.AssemblyHandler;
		}

		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				if (assemblies[i].FullName == args.Name)
				{
					return assemblies[i];
				}
			}
			return null;
		}

		public static bool CanSerialize(object obj)
		{
			return obj != null && SerializationTypeConverter.CanSerialize(obj.GetType());
		}

		public static bool CanSerialize(Type type)
		{
			return SerializationTypeConverter.TypeIsSerializable(type) && !type.GetTypeInfo().IsEnum && (type.GetTypeInfo().FullName.StartsWith("Microsoft.Exchange.", StringComparison.OrdinalIgnoreCase) || type.GetTypeInfo().FullName.StartsWith("Microsoft.Office.CompliancePolicy.", StringComparison.OrdinalIgnoreCase) || (type.GetTypeInfo().Equals(typeof(Exception)) || type.GetTypeInfo().IsSubclassOf(typeof(Exception))));
		}

		public static bool TypeIsSerializable(Type type)
		{
			if (null == type)
			{
				throw new ArgumentNullException("type");
			}
			if (!type.GetTypeInfo().IsSerializable)
			{
				return false;
			}
			if (!type.GetTypeInfo().IsGenericType)
			{
				return true;
			}
			foreach (Type type2 in type.GetTypeInfo().GetGenericArguments())
			{
				if (!SerializationTypeConverter.TypeIsSerializable(type2))
				{
					return false;
				}
			}
			return true;
		}

		public static object GetSerializationData(PSObject psObject)
		{
			ExTraceGlobals.SerializationTracer.TraceFunction<PSObject>(0L, "SerializationTypeConvertor.GetSerializationData({0})", psObject);
			byte[] result = null;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					binaryFormatter.Serialize(memoryStream, psObject.BaseObject);
					result = memoryStream.ToArray();
				}
			}
			catch (Exception arg)
			{
				ExTraceGlobals.SerializationTracer.TraceDebug<Exception>(0L, "Serialization Failed. Error = {0}", arg);
				if (psObject != null && psObject.BaseObject != null)
				{
					ExWatson.AddExtraData("Object Type: " + psObject.BaseObject.GetType().ToString());
					ExWatson.AddExtraData("Object String: " + psObject.BaseObject.ToString());
					ExWatson.AddExtraData("Handler StackTrace: " + Environment.StackTrace);
				}
				throw;
			}
			return result;
		}

		private static readonly Lazy<bool> IsRunningInRPSServerSide = new Lazy<bool>(delegate()
		{
			bool result;
			try
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					if (string.Equals(currentProcess.ProcessName, "w3wp", StringComparison.OrdinalIgnoreCase) && (Environment.CommandLine.IndexOf("PowerShell", StringComparison.OrdinalIgnoreCase) >= 0 || Environment.CommandLine.IndexOf("ECP", StringComparison.OrdinalIgnoreCase) >= 0))
					{
						return true;
					}
				}
				result = false;
			}
			catch (Exception)
			{
				result = true;
			}
			return result;
		}, true);

		private static ResolveEventHandler AssemblyHandler = new ResolveEventHandler(SerializationTypeConverter.CurrentDomain_AssemblyResolve);
	}
}
