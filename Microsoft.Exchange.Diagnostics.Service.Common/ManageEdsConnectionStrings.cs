using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.IO;
using System.Security;
using Microsoft.Exchange.Security.Dkm;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public static class ManageEdsConnectionStrings
	{
		static ManageEdsConnectionStrings()
		{
			ManageEdsConnectionStrings.Initialize();
		}

		public static void Initialize()
		{
			object obj = null;
			if (CommonUtils.TryGetRegistryValue(ManageEdsConnectionStrings.DiagnosticsRegistryKey, "SimpleEncrypt", null, out obj) && !(obj.ToString() == string.Empty) && !(obj.ToString() == "0"))
			{
				ManageEdsConnectionStrings.encrypt = new ManageEdsConnectionStrings.SimpleEncrypt();
			}
			else if (CommonUtils.TryGetRegistryValue(ManageEdsConnectionStrings.DiagnosticsRegistryKey, "AesEncryptFile", null, out obj) && obj.ToString() != string.Empty)
			{
				ManageEdsConnectionStrings.encrypt = new ManageEdsConnectionStrings.AesEncrypt(obj.ToString());
			}
			else
			{
				ManageEdsConnectionStrings.encrypt = new ManageEdsConnectionStrings.DkmEncrypt();
			}
			if (CommonUtils.TryGetRegistryValue(ManageEdsConnectionStrings.DiagnosticsRegistryKey, "SimpleConnectionStrings", null, out obj) && !(obj.ToString() == string.Empty) && !(obj.ToString() == "0"))
			{
				ManageEdsConnectionStrings.connectionStrings = new ManageEdsConnectionStrings.SimpleConnectionStrings();
				return;
			}
			if (CommonUtils.TryGetRegistryValue(ManageEdsConnectionStrings.DiagnosticsRegistryKey, "ConnectionStringsFile", null, out obj) && obj.ToString() != string.Empty)
			{
				ManageEdsConnectionStrings.connectionStrings = new ManageEdsConnectionStrings.FileConnectionStrings(obj.ToString());
				return;
			}
			ManageEdsConnectionStrings.connectionStrings = new ManageEdsConnectionStrings.AdConnectionStrings();
		}

		public static void AddEdsConnectionString(string edsSqlSchemaVersion, string connectionString)
		{
			if (string.IsNullOrEmpty(edsSqlSchemaVersion))
			{
				throw new ArgumentNullException("edsSqlSchemaVersion");
			}
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}
			ManageEdsConnectionStrings.connectionStrings.Add(edsSqlSchemaVersion, connectionString);
		}

		public static void RemoveEdsConnectionString(string guid)
		{
			if (string.IsNullOrEmpty(guid))
			{
				throw new ArgumentNullException("guid");
			}
			ManageEdsConnectionStrings.connectionStrings.Remove(guid);
		}

		public static List<string> GetEdsConnectionStrings()
		{
			return ManageEdsConnectionStrings.connectionStrings.Get();
		}

		internal static bool DkmEncryptString(string password, out string encryptedPassword)
		{
			return ManageEdsConnectionStrings.encrypt.EncryptString(password, out encryptedPassword);
		}

		internal static bool DkmDecryptString(string encryptedPassword, out SecureString decryptedString)
		{
			return ManageEdsConnectionStrings.encrypt.DecryptString(encryptedPassword, out decryptedString);
		}

		private static string GenerateConnectionString(string edsSqlSchemaVersion, string connectionString)
		{
			string value = Guid.NewGuid().ToString();
			DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder();
			dbConnectionStringBuilder.ConnectionString = connectionString;
			object obj;
			if (dbConnectionStringBuilder.TryGetValue("Site", out obj))
			{
				dbConnectionStringBuilder.Remove("Site");
			}
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(dbConnectionStringBuilder.ConnectionString);
			object obj2;
			if (!sqlConnectionStringBuilder.TryGetValue("Password", out obj2))
			{
				throw new ArgumentException("The connection string must contain a password.");
			}
			string password;
			if (!ManageEdsConnectionStrings.DkmEncryptString(obj2.ToString(), out password))
			{
				throw new ApplicationException("Unable to encrypt password");
			}
			sqlConnectionStringBuilder.Password = password;
			object obj3;
			if (!sqlConnectionStringBuilder.TryGetValue("User ID", out obj3) || string.IsNullOrEmpty(obj3.ToString()))
			{
				throw new ArgumentException("The connection string must contains a User ID");
			}
			if (!sqlConnectionStringBuilder.TryGetValue("Initial Catalog", out obj3) || string.IsNullOrEmpty(obj3.ToString()))
			{
				throw new ArgumentException("The connection string must contains an Initial Catalog");
			}
			if (!sqlConnectionStringBuilder.TryGetValue("Data Source", out obj3) || string.IsNullOrEmpty(obj3.ToString()))
			{
				throw new ArgumentException("The connection string must contains a Data Source");
			}
			DbConnectionStringBuilder dbConnectionStringBuilder2 = new DbConnectionStringBuilder();
			dbConnectionStringBuilder2.ConnectionString = sqlConnectionStringBuilder.ConnectionString;
			if (!string.IsNullOrEmpty((string)obj))
			{
				dbConnectionStringBuilder2.Add("Site", obj);
			}
			dbConnectionStringBuilder2.Add("Guid", value);
			dbConnectionStringBuilder2.Add("EdsSqlSchemaVersion", edsSqlSchemaVersion);
			return dbConnectionStringBuilder2.ToString();
		}

		private const string MsiInstallPathRegistryValue = "MsiInstallPath";

		internal static readonly string DiagnosticsRegistryKey = "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\ExchangeServer\\v15\\Diagnostics";

		private static ManageEdsConnectionStrings.IConnectionStrings connectionStrings;

		private static ManageEdsConnectionStrings.IEncrypt encrypt;

		private interface IEncrypt
		{
			bool EncryptString(string password, out string encryptedPassword);

			bool DecryptString(string encryptedPassword, out SecureString decryptedString);
		}

		private interface IConnectionStrings
		{
			void Add(string edsSqlSchemaVersion, string connectionString);

			void Remove(string guid);

			List<string> Get();
		}

		private class SimpleEncrypt : ManageEdsConnectionStrings.IEncrypt
		{
			public bool EncryptString(string password, out string encryptedPassword)
			{
				encryptedPassword = password;
				return true;
			}

			public bool DecryptString(string encryptedPassword, out SecureString decryptedString)
			{
				SecureString secureString = new SecureString();
				foreach (char c in encryptedPassword)
				{
					secureString.AppendChar(c);
				}
				decryptedString = secureString;
				return true;
			}
		}

		private class AesEncrypt : ManageEdsConnectionStrings.IEncrypt
		{
			public AesEncrypt(string file)
			{
				this.file = file;
			}

			public bool EncryptString(string password, out string encryptedPassword)
			{
				DiagnosticsPasswordEncryption encryption = this.GetEncryption();
				if (encryption == null)
				{
					encryptedPassword = null;
					return false;
				}
				encryptedPassword = encryption.EncryptString(password);
				return true;
			}

			public bool DecryptString(string encryptedPassword, out SecureString decryptedString)
			{
				DiagnosticsPasswordEncryption encryption = this.GetEncryption();
				if (encryption == null)
				{
					decryptedString = null;
					return false;
				}
				string text = encryption.DecryptString(encryptedPassword);
				SecureString secureString = new SecureString();
				foreach (char c in text)
				{
					secureString.AppendChar(c);
				}
				decryptedString = secureString;
				return true;
			}

			private DiagnosticsPasswordEncryption GetEncryption()
			{
				try
				{
					using (StreamReader streamReader = new StreamReader(this.file, false))
					{
						string keyString = streamReader.ReadLine();
						string initialVectorString = streamReader.ReadLine();
						return new DiagnosticsPasswordEncryption(keyString, initialVectorString);
					}
				}
				catch (Exception ex)
				{
					Logger.LogErrorMessage("Unable to get encryption key/vector from file {0}, error: {1}", new object[]
					{
						this.file,
						ex
					});
				}
				return null;
			}

			private readonly string file;
		}

		private class SimpleConnectionStrings : ManageEdsConnectionStrings.IConnectionStrings
		{
			public void Add(string edsSqlSchemaVersion, string connectionString)
			{
				List<string> edsEndpointObject = this.GetEdsEndpointObject();
				if (edsEndpointObject != null)
				{
					List<string> list = new List<string>(edsEndpointObject);
					string item = ManageEdsConnectionStrings.GenerateConnectionString(edsSqlSchemaVersion, connectionString);
					list.Add(item);
					this.SetEdsEndpointObject(list);
					return;
				}
				throw new ApplicationException("The EDS endpoint object does not exist.");
			}

			public void Remove(string guid)
			{
				List<string> edsEndpointObject = this.GetEdsEndpointObject();
				if (edsEndpointObject == null)
				{
					throw new ApplicationException("The EDS endpoint object does not exist.");
				}
				List<string> list = new List<string>(edsEndpointObject.Count);
				foreach (string text in edsEndpointObject)
				{
					if (!text.Contains(guid))
					{
						list.Add(text);
					}
				}
				if (list.Count != edsEndpointObject.Count)
				{
					this.SetEdsEndpointObject(list);
					return;
				}
				throw new ArgumentException("The guid specified does not exist.");
			}

			public List<string> Get()
			{
				List<string> list = new List<string>();
				List<string> edsEndpointObject = this.GetEdsEndpointObject();
				if (edsEndpointObject != null)
				{
					list.AddRange(edsEndpointObject);
				}
				return list;
			}

			public virtual void SetEdsEndpointObject(List<string> edsEndpoint)
			{
				try
				{
					Registry.SetValue(ManageEdsConnectionStrings.DiagnosticsRegistryKey, "SqlConnectionString", edsEndpoint.ToArray());
				}
				catch (Exception ex)
				{
					Logger.LogErrorMessage("Unable to write {0}\\{1}, error {2}:", new object[]
					{
						ManageEdsConnectionStrings.DiagnosticsRegistryKey,
						"SqlConnectionString",
						ex
					});
				}
			}

			public virtual List<string> GetEdsEndpointObject()
			{
				string[] array = new string[0];
				try
				{
					array = (string[])Registry.GetValue(ManageEdsConnectionStrings.DiagnosticsRegistryKey, "SqlConnectionString", array);
				}
				catch (Exception ex)
				{
					Logger.LogErrorMessage("Unable to read {0}\\{1}, error {2}:", new object[]
					{
						ManageEdsConnectionStrings.DiagnosticsRegistryKey,
						"SqlConnectionString",
						ex
					});
				}
				return new List<string>(array);
			}

			public const string SqlConnectionString = "SqlConnectionString";
		}

		private class FileConnectionStrings : ManageEdsConnectionStrings.SimpleConnectionStrings
		{
			public FileConnectionStrings(string file)
			{
				this.file = file;
			}

			public override void SetEdsEndpointObject(List<string> edsEndpoint)
			{
				using (StreamWriter streamWriter = new StreamWriter(this.file, false))
				{
					foreach (string value in edsEndpoint)
					{
						streamWriter.WriteLine(value);
					}
				}
			}

			public override List<string> GetEdsEndpointObject()
			{
				List<string> list = new List<string>();
				if (File.Exists(this.file))
				{
					using (StreamReader streamReader = new StreamReader(this.file, false))
					{
						while (!streamReader.EndOfStream)
						{
							list.Add(streamReader.ReadLine());
						}
					}
				}
				return list;
			}

			private readonly string file;
		}

		private class DkmEncrypt : ManageEdsConnectionStrings.IEncrypt
		{
			public DkmEncrypt()
			{
				object obj = null;
				if (CommonUtils.TryGetRegistryValue(ManageEdsConnectionStrings.DiagnosticsRegistryKey, "MsiInstallPath", null, out obj))
				{
					this.edsPath = obj.ToString();
					return;
				}
				this.edsPath = null;
			}

			public bool EncryptString(string password, out string encryptedPassword)
			{
				bool result = false;
				try
				{
					ExchangeGroupKey exchangeGroupKey = new ExchangeGroupKey(this.edsPath, "Microsoft Exchange Diagnostics DKM");
					encryptedPassword = exchangeGroupKey.ClearStringToEncryptedString(password);
					result = true;
				}
				catch (Exception ex)
				{
					Logger.LogErrorMessage("DKM query failed to encrypt due to: {0}", new object[]
					{
						ex
					});
					encryptedPassword = null;
				}
				return result;
			}

			public bool DecryptString(string encryptedPassword, out SecureString decryptedString)
			{
				bool result = false;
				try
				{
					ExchangeGroupKey exchangeGroupKey = new ExchangeGroupKey(this.edsPath, "Microsoft Exchange Diagnostics DKM");
					decryptedString = exchangeGroupKey.EncryptedStringToSecureString(encryptedPassword);
					result = true;
				}
				catch (Exception ex)
				{
					decryptedString = null;
					Logger.LogErrorMessage("DKM query failed to decrypt due to: {0}", new object[]
					{
						ex
					});
				}
				return result;
			}

			private readonly string edsPath;
		}

		private class AdConnectionStrings : ManageEdsConnectionStrings.IConnectionStrings
		{
			public void Add(string edsSqlSchemaVersion, string connectionString)
			{
				SearchResult edsEndpointObject = ManageEdsConnectionStrings.AdConnectionStrings.GetEdsEndpointObject();
				if (edsEndpointObject != null)
				{
					string value = ManageEdsConnectionStrings.GenerateConnectionString(edsSqlSchemaVersion, connectionString);
					using (DirectoryEntry directoryEntry = new DirectoryEntry(edsEndpointObject.Path))
					{
						directoryEntry.Properties["serviceBindingInformation"].Add(value);
						directoryEntry.CommitChanges();
						return;
					}
				}
				throw new ApplicationException("The EDS endpoint object does not exist.");
			}

			public void Remove(string guid)
			{
				SearchResult edsEndpointObject = ManageEdsConnectionStrings.AdConnectionStrings.GetEdsEndpointObject();
				if (edsEndpointObject != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = edsEndpointObject.Properties["serviceBindingInformation"];
					string text = null;
					foreach (object obj in resultPropertyValueCollection)
					{
						string text2 = (string)obj;
						if (text2.Contains(guid))
						{
							text = text2;
							break;
						}
					}
					if (text != null)
					{
						using (DirectoryEntry directoryEntry = new DirectoryEntry(edsEndpointObject.Path))
						{
							directoryEntry.Properties["serviceBindingInformation"].Remove(text);
							directoryEntry.CommitChanges();
							return;
						}
					}
					throw new ArgumentException("The guid specified does not exist.");
				}
				throw new ApplicationException("The EDS endpoint object does not exist.");
			}

			public List<string> Get()
			{
				List<string> list = new List<string>();
				SearchResult edsEndpointObject = ManageEdsConnectionStrings.AdConnectionStrings.GetEdsEndpointObject();
				if (edsEndpointObject != null)
				{
					ResultPropertyValueCollection resultPropertyValueCollection = edsEndpointObject.Properties["serviceBindingInformation"];
					foreach (object obj in resultPropertyValueCollection)
					{
						string item = (string)obj;
						list.Add(item);
					}
				}
				return list;
			}

			private static SearchResult GetEdsEndpointObject()
			{
				string[] propertiesToLoad = new string[]
				{
					"serviceBindingInformation"
				};
				string path;
				try
				{
					using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://RootDSE"))
					{
						string str = directoryEntry.Properties["configurationNamingContext"].Value.ToString();
						path = "LDAP://CN=Microsoft Exchange,CN=Services," + str;
					}
				}
				catch (Exception ex)
				{
					Logger.LogErrorMessage("Unable to query the configurationNamingContext, the error is:{0}", new object[]
					{
						ex
					});
					return null;
				}
				SearchResult result;
				try
				{
					using (DirectoryEntry directoryEntry2 = new DirectoryEntry(path))
					{
						using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry2, "(&(name=EdsSqlEndpoints)(objectClass=serviceConnectionPoint))", propertiesToLoad))
						{
							result = directorySearcher.FindOne();
						}
					}
				}
				catch (Exception ex2)
				{
					Logger.LogErrorMessage("Failed to find EdsSqlEndpoints object, the error is:{0}", new object[]
					{
						ex2
					});
					result = null;
				}
				return result;
			}

			private const string Filter = "(&(name=EdsSqlEndpoints)(objectClass=serviceConnectionPoint))";

			private const string ServiceBindingInformation = "serviceBindingInformation";
		}
	}
}
