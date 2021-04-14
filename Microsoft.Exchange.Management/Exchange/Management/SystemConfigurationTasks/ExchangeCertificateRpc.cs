using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.ExchangeCertificate;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class ExchangeCertificateRpc
	{
		public string GetByThumbprint
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.GetByThumbprint, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.GetByThumbprint] = value;
			}
		}

		public byte[] GetByCertificate
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.GetByCertificate, out obj))
				{
					return (byte[])obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.GetByCertificate] = value;
			}
		}

		public MultiValuedProperty<SmtpDomain> GetByDomains
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.GetByDomains, out obj))
				{
					return (MultiValuedProperty<SmtpDomain>)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.GetByDomains] = value;
			}
		}

		public bool CreateExportable
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.CreateExportable, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateExportable] = value;
			}
		}

		public string CreateSubjectName
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.CreateSubjectName, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateSubjectName] = value;
			}
		}

		public string CreateFriendlyName
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.CreateFriendlyName, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateFriendlyName] = value;
			}
		}

		public MultiValuedProperty<SmtpDomainWithSubdomains> CreateDomains
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.CreateDomains, out obj))
				{
					return (MultiValuedProperty<SmtpDomainWithSubdomains>)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateDomains] = value;
			}
		}

		public bool CreateIncAccepted
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.CreateIncAccepted, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateIncAccepted] = value;
			}
		}

		public bool CreateIncFqdn
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.CreateIncFqdn, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateIncFqdn] = value;
			}
		}

		public bool CreateIncNetBios
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.CreateIncNetBios, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateIncNetBios] = value;
			}
		}

		public bool CreateIncAutoDisc
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.CreateIncAutoDisc, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateIncAutoDisc] = value;
			}
		}

		public int CreateKeySize
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.CreateKeySize, out obj))
				{
					return (int)obj;
				}
				return 0;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateKeySize] = value;
			}
		}

		public byte[] CreateCloneCert
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.CreateCloneCert, out obj))
				{
					return (byte[])obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateCloneCert] = value;
			}
		}

		public bool CreateBinary
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.CreateBinary, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateBinary] = value;
			}
		}

		public bool CreateRequest
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.CreateRequest, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateRequest] = value;
			}
		}

		public AllowedServices CreateServices
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.CreateServices, out obj))
				{
					return (AllowedServices)obj;
				}
				return AllowedServices.None;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateServices] = value;
			}
		}

		public bool RequireSsl
		{
			get
			{
				object obj;
				return !this.inputParameters.TryGetValue(RpcParameters.RequireSsl, out obj) || (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.RequireSsl] = value;
			}
		}

		public string CreateSubjectKeyIdentifier
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.CreateSubjectKeyIdentifier, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateSubjectKeyIdentifier] = value;
			}
		}

		public bool CreateAllowConfirmation
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.CreateAllowConfirmation, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateAllowConfirmation] = value;
			}
		}

		public bool CreateWhatIf
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.CreateWhatIf, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.CreateWhatIf] = value;
			}
		}

		public string RemoveByThumbprint
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.RemoveByThumbprint, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.RemoveByThumbprint] = value;
			}
		}

		public string ExportByThumbprint
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.ExportByThumbprint, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.ExportByThumbprint] = value;
			}
		}

		public bool ExportBinary
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.ExportBinary, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.ExportBinary] = value;
			}
		}

		public string ImportCert
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.ImportCert, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.ImportCert] = value;
			}
		}

		public string ImportDescription
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.ImportDescription, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.ImportDescription] = value;
			}
		}

		public bool ImportExportable
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.ImportExportable, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.ImportExportable] = value;
			}
		}

		public string EnableByThumbprint
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.EnableByThumbprint, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.inputParameters[RpcParameters.EnableByThumbprint] = value;
			}
		}

		public AllowedServices EnableServices
		{
			get
			{
				object obj;
				if (this.inputParameters.TryGetValue(RpcParameters.EnableServices, out obj))
				{
					return (AllowedServices)obj;
				}
				return AllowedServices.None;
			}
			set
			{
				this.inputParameters[RpcParameters.EnableServices] = value;
			}
		}

		public bool EnableAllowConfirmation
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.EnableAllowConfirmation, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.EnableAllowConfirmation] = value;
			}
		}

		public bool EnableUpdateAD
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.EnableUpdateAD, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.EnableUpdateAD] = value;
			}
		}

		public bool EnableNetworkService
		{
			get
			{
				object obj;
				return this.inputParameters.TryGetValue(RpcParameters.EnableNetworkService, out obj) && (bool)obj;
			}
			set
			{
				this.inputParameters[RpcParameters.EnableNetworkService] = value;
			}
		}

		public List<ExchangeCertificate> ReturnCertList
		{
			get
			{
				object obj;
				if (this.outputParameters.TryGetValue(RpcOutput.ExchangeCertList, out obj))
				{
					return (List<ExchangeCertificate>)obj;
				}
				return null;
			}
			set
			{
				this.outputParameters[RpcOutput.ExchangeCertList] = value;
			}
		}

		public ExchangeCertificate ReturnCert
		{
			get
			{
				object obj;
				if (this.outputParameters.TryGetValue(RpcOutput.ExchangeCert, out obj))
				{
					return (ExchangeCertificate)obj;
				}
				return null;
			}
			set
			{
				this.outputParameters[RpcOutput.ExchangeCert] = value;
			}
		}

		public string ReturnCertRequest
		{
			get
			{
				object obj;
				if (this.outputParameters.TryGetValue(RpcOutput.CertRequest, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.outputParameters[RpcOutput.CertRequest] = value;
			}
		}

		public string ReturnExportBase64
		{
			get
			{
				object obj;
				if (this.outputParameters.TryGetValue(RpcOutput.ExportBase64, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.outputParameters[RpcOutput.ExportBase64] = value;
			}
		}

		public byte[] ReturnExportFileData
		{
			get
			{
				object obj;
				if (this.outputParameters.TryGetValue(RpcOutput.ExportFile, out obj))
				{
					return (byte[])obj;
				}
				return null;
			}
			set
			{
				this.outputParameters[RpcOutput.ExportFile] = value;
			}
		}

		public bool ReturnExportBinary
		{
			get
			{
				object obj;
				return this.outputParameters.TryGetValue(RpcOutput.ExportBinary, out obj) && (bool)obj;
			}
			set
			{
				this.outputParameters[RpcOutput.ExportBinary] = value;
			}
		}

		public bool ReturnExportPKCS10
		{
			get
			{
				object obj;
				return this.outputParameters.TryGetValue(RpcOutput.ExportPKCS10, out obj) && (bool)obj;
			}
			set
			{
				this.outputParameters[RpcOutput.ExportPKCS10] = value;
			}
		}

		public LocalizedString ReturnConfirmation
		{
			get
			{
				object obj;
				if (this.outputParameters.TryGetValue(RpcOutput.TaskConfirmation, out obj))
				{
					return (LocalizedString)obj;
				}
				return LocalizedString.Empty;
			}
			set
			{
				this.outputParameters[RpcOutput.TaskConfirmation] = value;
			}
		}

		public Dictionary<AllowedServices, LocalizedString> ReturnConfirmationList
		{
			get
			{
				object obj;
				if (this.outputParameters.TryGetValue(RpcOutput.TaskConfirmationList, out obj))
				{
					return (Dictionary<AllowedServices, LocalizedString>)obj;
				}
				return null;
			}
			set
			{
				this.outputParameters[RpcOutput.TaskConfirmationList] = value;
			}
		}

		public string ReturnTaskErrorString
		{
			get
			{
				object obj;
				if (this.outputParameters.TryGetValue(RpcOutput.TaskErrorString, out obj))
				{
					return (string)obj;
				}
				return null;
			}
			set
			{
				this.outputParameters[RpcOutput.TaskErrorString] = value;
			}
		}

		public ErrorCategory ReturnTaskErrorCategory
		{
			get
			{
				object obj;
				if (this.outputParameters.TryGetValue(RpcOutput.TaskErrorCategory, out obj))
				{
					return (ErrorCategory)obj;
				}
				return ErrorCategory.NotSpecified;
			}
			set
			{
				this.outputParameters[RpcOutput.TaskErrorCategory] = value;
			}
		}

		public List<LocalizedString> ReturnTaskWarningList
		{
			get
			{
				object obj;
				if (this.outputParameters.TryGetValue(RpcOutput.TaskWarningList, out obj))
				{
					return (List<LocalizedString>)obj;
				}
				return null;
			}
			set
			{
				this.outputParameters[RpcOutput.TaskWarningList] = value;
			}
		}

		public ExchangeCertificateRpc(ExchangeCertificateRpcVersion version, byte[] inputBlob, byte[] outputBlob)
		{
			this.inputParameters = new Dictionary<RpcParameters, object>();
			if (inputBlob != null)
			{
				if (version == ExchangeCertificateRpcVersion.Version1)
				{
					this.inputParameters = (Dictionary<RpcParameters, object>)this.DeserializeObject(inputBlob, false);
				}
				else if (version == ExchangeCertificateRpcVersion.Version2)
				{
					this.inputParameters = this.BuildInputParameters(inputBlob);
				}
			}
			this.outputParameters = new Dictionary<RpcOutput, object>();
			if (outputBlob != null)
			{
				if (version == ExchangeCertificateRpcVersion.Version1)
				{
					this.outputParameters = (Dictionary<RpcOutput, object>)this.DeserializeObject(outputBlob, false);
					return;
				}
				if (version == ExchangeCertificateRpcVersion.Version2)
				{
					this.outputParameters = this.BuildOutputParameters(outputBlob);
				}
			}
		}

		public ExchangeCertificateRpc() : this(ExchangeCertificateRpcVersion.Version1, null, null)
		{
		}

		internal static byte[] SerializeError(ExchangeCertificateRpcVersion version, string message, ErrorCategory category)
		{
			if (version == ExchangeCertificateRpcVersion.Version2)
			{
				return ExchangeCertificateRpc.SerializeObject(new object[]
				{
					ExchangeCertificateRpc.SerializeObject(RpcOutput.TaskErrorString),
					ExchangeCertificateRpc.SerializeObject(message),
					ExchangeCertificateRpc.SerializeObject(RpcOutput.TaskErrorCategory),
					ExchangeCertificateRpc.SerializeObject(category)
				});
			}
			Dictionary<RpcOutput, object> dictionary = new Dictionary<RpcOutput, object>();
			dictionary[RpcOutput.TaskErrorString] = message;
			dictionary[RpcOutput.TaskErrorCategory] = category;
			return ExchangeCertificateRpc.SerializeObject(dictionary);
		}

		internal static void OutputTaskMessages(Server server, ExchangeCertificateRpc outputValues, Task.TaskWarningLoggingDelegate warningWriter, Task.TaskErrorLoggingDelegate errorWriter)
		{
			if (outputValues.ReturnTaskWarningList != null)
			{
				foreach (LocalizedString message in outputValues.ReturnTaskWarningList)
				{
					warningWriter(message);
				}
			}
			if (!string.IsNullOrEmpty(outputValues.ReturnTaskErrorString))
			{
				errorWriter(new InvalidOperationException(Strings.DetailRpcError(server.Name, outputValues.ReturnTaskErrorString)), outputValues.ReturnTaskErrorCategory, null);
			}
		}

		internal byte[] SerializeInputParameters(ExchangeCertificateRpcVersion rpcVersion)
		{
			if (rpcVersion == ExchangeCertificateRpcVersion.Version2)
			{
				return this.SerializeDictionaryAsArray<RpcParameters, object>(this.inputParameters);
			}
			return ExchangeCertificateRpc.SerializeObject(this.inputParameters);
		}

		internal byte[] SerializeOutputParameters(ExchangeCertificateRpcVersion rpcVersion)
		{
			if (rpcVersion == ExchangeCertificateRpcVersion.Version2)
			{
				return this.SerializeOutputParametersAsArray();
			}
			return ExchangeCertificateRpc.SerializeObject(this.outputParameters);
		}

		private byte[] SerializeDictionaryAsArray<TKey, TValue>(Dictionary<TKey, TValue> list)
		{
			int num = 0;
			if (list != null)
			{
				object[] array = new object[list.Count * 2];
				foreach (KeyValuePair<TKey, TValue> keyValuePair in list)
				{
					array[num++] = ExchangeCertificateRpc.SerializeObject(keyValuePair.Key);
					array[num++] = ExchangeCertificateRpc.SerializeObject(keyValuePair.Value);
				}
				return ExchangeCertificateRpc.SerializeObject(array);
			}
			return null;
		}

		private byte[] SerializeListAsArray<TItem>(List<TItem> list)
		{
			int num = 0;
			if (list != null)
			{
				object[] array = new object[list.Count];
				foreach (TItem titem in list)
				{
					if (typeof(TItem) == typeof(ExchangeCertificate))
					{
						ExchangeCertificate exchangeCertificate = titem as ExchangeCertificate;
						array[num++] = ExchangeCertificateRpc.SerializeObject(exchangeCertificate.ExchangeCertificateAsArray());
					}
					else
					{
						array[num++] = ExchangeCertificateRpc.SerializeObject(titem);
					}
				}
				return ExchangeCertificateRpc.SerializeObject(array);
			}
			return null;
		}

		private byte[] SerializeOutputParametersAsArray()
		{
			int num = 0;
			object[] array = new object[this.outputParameters.Count * 2];
			foreach (KeyValuePair<RpcOutput, object> keyValuePair in this.outputParameters)
			{
				array[num++] = ExchangeCertificateRpc.SerializeObject(keyValuePair.Key);
				RpcOutput key = keyValuePair.Key;
				switch (key)
				{
				case RpcOutput.ExchangeCertList:
					array[num++] = this.SerializeListAsArray<ExchangeCertificate>(keyValuePair.Value as List<ExchangeCertificate>);
					break;
				case RpcOutput.ExchangeCert:
					array[num++] = ExchangeCertificateRpc.SerializeObject(((ExchangeCertificate)keyValuePair.Value).ExchangeCertificateAsArray());
					break;
				default:
					switch (key)
					{
					case RpcOutput.TaskWarningList:
						array[num++] = this.SerializeListAsArray<LocalizedString>(keyValuePair.Value as List<LocalizedString>);
						break;
					case RpcOutput.TaskConfirmationList:
						array[num++] = this.SerializeDictionaryAsArray<AllowedServices, LocalizedString>(keyValuePair.Value as Dictionary<AllowedServices, LocalizedString>);
						break;
					default:
						array[num++] = ExchangeCertificateRpc.SerializeObject(keyValuePair.Value);
						break;
					}
					break;
				}
			}
			return ExchangeCertificateRpc.SerializeObject(array);
		}

		internal Dictionary<RpcParameters, object> BuildInputParameters(byte[] blob)
		{
			Dictionary<RpcParameters, object> dictionary = new Dictionary<RpcParameters, object>();
			object[] array = (object[])this.DeserializeObject(blob, false);
			if (array.Length % 2 == 0)
			{
				for (int i = 0; i < array.Length; i += 2)
				{
					RpcParameters key = (RpcParameters)this.DeserializeObject((byte[])array[i], true);
					object value = this.DeserializeObject((byte[])array[i + 1], true);
					dictionary[key] = value;
				}
			}
			return dictionary;
		}

		internal Dictionary<RpcOutput, object> BuildOutputParameters(byte[] blob)
		{
			Dictionary<RpcOutput, object> dictionary = new Dictionary<RpcOutput, object>();
			object[] array = (object[])this.DeserializeObject(blob, false);
			for (int i = 0; i < array.Length; i += 2)
			{
				RpcOutput rpcOutput = (RpcOutput)this.DeserializeObject((byte[])array[i], true);
				object obj = this.DeserializeObject((byte[])array[i + 1], true);
				if (obj != null)
				{
					RpcOutput rpcOutput2 = rpcOutput;
					switch (rpcOutput2)
					{
					case RpcOutput.ExchangeCertList:
					{
						List<ExchangeCertificate> list = new List<ExchangeCertificate>();
						foreach (object obj2 in (object[])obj)
						{
							list.Add(new ExchangeCertificate((object[])this.DeserializeObject((byte[])obj2, true)));
						}
						obj = list;
						break;
					}
					case RpcOutput.ExchangeCert:
						obj = new ExchangeCertificate((object[])obj);
						break;
					default:
						switch (rpcOutput2)
						{
						case RpcOutput.TaskWarningList:
						{
							List<LocalizedString> list2 = new List<LocalizedString>();
							foreach (object obj3 in (object[])obj)
							{
								list2.Add((LocalizedString)this.DeserializeObject((byte[])obj3, true));
							}
							obj = list2;
							break;
						}
						case RpcOutput.TaskConfirmationList:
						{
							object[] array4 = (object[])obj;
							Dictionary<AllowedServices, LocalizedString> dictionary2 = new Dictionary<AllowedServices, LocalizedString>();
							for (int l = 0; l < array4.Length; l += 2)
							{
								AllowedServices key = (AllowedServices)this.DeserializeObject((byte[])array4[l], true);
								LocalizedString value = (LocalizedString)this.DeserializeObject((byte[])array4[l + 1], true);
								dictionary2[key] = value;
							}
							obj = dictionary2;
							break;
						}
						}
						break;
					}
				}
				dictionary[rpcOutput] = obj;
			}
			return dictionary;
		}

		private object DeserializeObject(byte[] data, bool customized)
		{
			if (data != null)
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					BinaryFormatter binaryFormatter;
					if (customized)
					{
						binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(new CustomizedSerializationBinder());
					}
					else
					{
						binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					}
					return binaryFormatter.Deserialize(memoryStream);
				}
			}
			return null;
		}

		private static byte[] SerializeObject(object inputObject)
		{
			if (inputObject != null)
			{
				byte[] result = null;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					binaryFormatter.Serialize(memoryStream, inputObject);
					result = memoryStream.ToArray();
				}
				return result;
			}
			return null;
		}

		private Dictionary<RpcParameters, object> inputParameters;

		private Dictionary<RpcOutput, object> outputParameters;
	}
}
