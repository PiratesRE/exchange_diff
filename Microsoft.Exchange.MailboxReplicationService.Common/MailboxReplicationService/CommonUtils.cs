using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxReplicationService;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class CommonUtils
	{
		public static string LocalComputerName
		{
			get
			{
				if (string.IsNullOrEmpty(CommonUtils.localComputerName))
				{
					CommonUtils.ObtainComputerNames();
				}
				return CommonUtils.localComputerName;
			}
		}

		public static string LocalShortComputerName
		{
			get
			{
				if (string.IsNullOrEmpty(CommonUtils.localShortComputerName))
				{
					CommonUtils.ObtainComputerNames();
				}
				return CommonUtils.localShortComputerName;
			}
		}

		public static ADObjectId LocalSiteId
		{
			get
			{
				return LocalServer.GetServer().ServerSite;
			}
		}

		public static Guid LocalServerGuid
		{
			get
			{
				return LocalServer.GetServer().Guid;
			}
		}

		public static ForestType ForestType
		{
			get
			{
				return CommonUtils.forestType.Value;
			}
		}

		public static string SafeInstallPath
		{
			get
			{
				string result;
				try
				{
					result = ConfigurationContext.Setup.InstallPath;
				}
				catch (SetupVersionInformationCorruptException)
				{
					result = "C:\\Program Files\\Microsoft\\ExchangeServer\\V15";
				}
				return result;
			}
		}

		public static MiniServer[] FindServers(ITopologyConfigurationSession configSession, int minVersion, ServerRole role, ADObjectId serverSiteId, ADPropertyDefinition[] addlMiniServerProps)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, minVersion),
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, (ulong)((long)role))
			});
			return CommonUtils.FindServers(configSession, filter, role, serverSiteId, addlMiniServerProps);
		}

		public static MiniServer[] FindServers(ITopologyConfigurationSession configSession, int minVersion, int maxVersion, ServerRole role, ADObjectId serverSiteId, ADPropertyDefinition[] addlMiniServerProps)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, minVersion),
				new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, maxVersion),
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, (ulong)((long)role))
			});
			return CommonUtils.FindServers(configSession, filter, role, serverSiteId, addlMiniServerProps);
		}

		public static MiniServer[] FindServers(ITopologyConfigurationSession configSession, QueryFilter filter, ServerRole role, ADObjectId serverSiteId, ADPropertyDefinition[] addlMiniServerProps)
		{
			if (serverSiteId != null)
			{
				filter = new AndFilter(new QueryFilter[]
				{
					filter,
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, serverSiteId)
				});
			}
			return configSession.FindMiniServer(null, QueryScope.SubTree, filter, null, 0, addlMiniServerProps);
		}

		public static QueryFilter BuildMbxFilter(ITopologyConfigurationSession configSession, int minVersion, int maxVersion)
		{
			ExAssert.RetailAssert(configSession != null, "CommonUtils.BuildMbxFilter: configSession is null");
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL),
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, minVersion),
				new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, maxVersion)
			});
			ADPropertyDefinition[] properties = new ADPropertyDefinition[]
			{
				ServerSchema.ServerSite,
				ServerSchema.ExchangeLegacyDN
			};
			MiniServer[] array = configSession.FindMiniServer(null, QueryScope.SubTree, filter, null, 0, properties);
			if (array == null)
			{
				return null;
			}
			List<TextFilter> list = new List<TextFilter>();
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			MiniServer[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				MiniServer miniServer = array2[i];
				bool flag = false;
				if (miniServer.ServerSite == null || !miniServer.Name.StartsWith(miniServer.ServerSite.Name, StringComparison.OrdinalIgnoreCase))
				{
					goto IL_ED;
				}
				if (hashSet.Add(miniServer.ServerSite.Name))
				{
					flag = true;
					goto IL_ED;
				}
				IL_141:
				i++;
				continue;
				IL_ED:
				if (flag)
				{
					string text = miniServer.ExchangeLegacyDN.Replace(miniServer.Name, miniServer.ServerSite.Name);
					list.Add(new TextFilter(MiniRecipientSchema.ServerLegacyDN, text, MatchOptions.Prefix, MatchFlags.IgnoreCase));
					goto IL_141;
				}
				list.Add(new TextFilter(MiniRecipientSchema.ServerLegacyDN, miniServer.ExchangeLegacyDN, MatchOptions.FullString, MatchFlags.IgnoreCase));
				goto IL_141;
			}
			if (list.Count == 0)
			{
				return null;
			}
			return QueryFilter.OrTogether(list.ToArray());
		}

		public static float GetEffectiveMrsVersion()
		{
			return ConfigBase<MRSConfigSchema>.GetConfig<float>("MrsVersion");
		}

		public static DumpsterReplicationStatus DumpsterStatus
		{
			get
			{
				return CommonUtils.dumpsterStatus;
			}
		}

		public static ExEventLog EventLog
		{
			get
			{
				return CommonUtils.eventLog;
			}
		}

		public static ManualResetEvent ServiceIsStoppingEvent
		{
			get
			{
				return CommonUtils.serviceIsStoppingEvent;
			}
		}

		public static bool ServiceIsStopping
		{
			get
			{
				return CommonUtils.serviceIsStoppingEvent.WaitOne(0);
			}
			set
			{
				CommonUtils.serviceIsStoppingEvent.Set();
			}
		}

		public static void CheckForServiceStopping()
		{
			if (CommonUtils.ServiceIsStopping)
			{
				throw new ServiceIsStoppingPermanentException();
			}
		}

		public static void GenerateWatson(Exception ex)
		{
			ExWatson.SendReport(ex);
		}

		public static void LogEvent(ExEventLog.EventTuple tuple, params object[] args)
		{
			CommonUtils.LogEvent(CommonUtils.EventLog, tuple, args);
		}

		public static void LogEvent(ExEventLog eventLog, ExEventLog.EventTuple tuple, params object[] args)
		{
			if (eventLog == null)
			{
				throw new ArgumentNullException("eventLog");
			}
			for (int i = 0; i < args.Length; i++)
			{
				string text = args[i] as string;
				if (text == null && args[i] is LocalizedString)
				{
					text = ((LocalizedString)args[i]).ToString();
				}
				if (text != null && text.Length > 32000)
				{
					args[i] = text.Substring(0, 31997) + "...";
				}
			}
			eventLog.LogEvent(tuple, string.Empty, args);
		}

		public static int HrFromException(Exception ex)
		{
			int num;
			if (ex == null)
			{
				num = 0;
			}
			else if (ex is LocalizedException)
			{
				num = ((LocalizedException)ex).ErrorCode;
				if (num == 0)
				{
					num = -2147467259;
				}
			}
			else if (ex is RopExecutionException)
			{
				num = (int)((RopExecutionException)ex).ErrorCode;
			}
			else
			{
				num = -2147467259;
			}
			if (num > 0)
			{
				num = (-2147024896 | num);
			}
			return num;
		}

		public static LocalizedString FullExceptionMessage(Exception ex)
		{
			return WcfUtils.FullExceptionMessage(ex);
		}

		public static LocalizedString FullExceptionMessage(Exception ex, bool includeStack)
		{
			return WcfUtils.FullExceptionMessage(ex, includeStack);
		}

		public static bool IsTransientException(Exception ex)
		{
			return CommonUtils.ExceptionIsAny(ex, new WellKnownException[]
			{
				WellKnownException.Transient,
				WellKnownException.DataProviderPermanent,
				WellKnownException.MapiMdbOffline,
				WellKnownException.WrongServer,
				WellKnownException.MapiPartialCompletion,
				WellKnownException.DataProviderTransient
			}) && !CommonUtils.ExceptionIsAny(ex, new WellKnownException[]
			{
				WellKnownException.MapiShutoffQuotaExceeded,
				WellKnownException.StorageCannotMoveDefaultFolder
			});
		}

		public static WellKnownException[] ClassifyException(Exception ex)
		{
			return CommonUtils.ClassifyException(ex, 0);
		}

		public static bool ExceptionIs(Exception ex, params WellKnownException[] wkesToCheck)
		{
			WellKnownException[] array = CommonUtils.ClassifyException(ex);
			if (array == null)
			{
				return false;
			}
			foreach (WellKnownException ex2 in wkesToCheck)
			{
				bool flag = false;
				foreach (WellKnownException ex3 in array)
				{
					if (ex3 == ex2)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public static bool ExceptionIsAny(Exception ex, params WellKnownException[] wkesToCheck)
		{
			WellKnownException[] array = CommonUtils.ClassifyException(ex);
			if (array == null)
			{
				return false;
			}
			foreach (WellKnownException ex2 in wkesToCheck)
			{
				foreach (WellKnownException ex3 in array)
				{
					if (ex3 == ex2)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static string GetFailureType(Exception ex)
		{
			if (ex == null)
			{
				return null;
			}
			IMRSRemoteException ex2 = ex as IMRSRemoteException;
			if (ex2 == null)
			{
				return ex.GetType().Name;
			}
			return ex2.OriginalFailureType;
		}

		public static string GetStackTrace(Exception ex)
		{
			if (ex == null)
			{
				return null;
			}
			IMRSRemoteException ex2 = ex as IMRSRemoteException;
			if (ex2 == null)
			{
				return ex.StackTrace;
			}
			return string.Format("Remote trace:\r\n{0}\r\nLocal trace:\r\n{1}", ex2.RemoteStackTrace, ex.StackTrace);
		}

		public static int GetMapiLowLevelError(Exception ex)
		{
			if (ex == null)
			{
				return 0;
			}
			IMRSRemoteException ex2 = ex as IMRSRemoteException;
			if (ex2 != null)
			{
				return ex2.MapiLowLevelError;
			}
			MapiPermanentException ex3 = ex as MapiPermanentException;
			if (ex3 != null)
			{
				return ex3.LowLevelError;
			}
			MapiRetryableException ex4 = ex as MapiRetryableException;
			if (ex4 != null)
			{
				return ex4.LowLevelError;
			}
			RopExecutionException ex5 = ex as RopExecutionException;
			if (ex5 != null)
			{
				LocalizedException localizedException = CommonUtils.GetLocalizedException(ex5);
				if (localizedException != null)
				{
					return localizedException.ErrorCode;
				}
			}
			return 0;
		}

		public static List<PublicFolderMoveRequest> GetPublicFolderMoveRequests()
		{
			List<PublicFolderMoveRequest> result;
			using (MrsPSHandler mrsPSHandler = new MrsPSHandler("GetPublicFolderMoveRequest Monad"))
			{
				using (MonadCommand command = mrsPSHandler.GetCommand(MrsCmdlet.GetPublicFolderMoveRequest))
				{
					object[] array = command.Execute();
					if (array.Length == 0)
					{
						result = null;
					}
					else
					{
						List<PublicFolderMoveRequest> list = new List<PublicFolderMoveRequest>();
						foreach (object obj in array)
						{
							list.Add((PublicFolderMoveRequest)obj);
						}
						result = list;
					}
				}
			}
			return result;
		}

		public static List<MoveRequest> GetMoveRequests()
		{
			List<MoveRequest> result;
			using (MrsPSHandler mrsPSHandler = new MrsPSHandler("GetMoveRequest Monad"))
			{
				using (MonadCommand command = mrsPSHandler.GetCommand(MrsCmdlet.GetMoveRequest))
				{
					object[] array = command.Execute();
					if (array.Length == 0)
					{
						result = null;
					}
					else
					{
						List<MoveRequest> list = new List<MoveRequest>();
						foreach (object obj in array)
						{
							list.Add((MoveRequest)obj);
						}
						result = list;
					}
				}
			}
			return result;
		}

		public static List<Mailbox> GetPublicFolderMailboxes()
		{
			List<Mailbox> result;
			using (MrsPSHandler mrsPSHandler = new MrsPSHandler("GetPublicFolderMailboxes Monad"))
			{
				using (MonadCommand command = mrsPSHandler.GetCommand(MrsCmdlet.GetMailbox))
				{
					command.Parameters.AddSwitch("PublicFolder");
					object[] array = command.Execute();
					if (array.Length == 0)
					{
						result = null;
					}
					else
					{
						List<Mailbox> list = new List<Mailbox>();
						foreach (object obj in array)
						{
							list.Add((Mailbox)obj);
						}
						result = list;
					}
				}
			}
			return result;
		}

		public static bool IsPublicFolderMailboxesLockedForNewConnectionsFlagSet(OrganizationId organizationId)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 1286, "IsPublicFolderMailboxesLockedForNewConnectionsFlagSet", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\Common.cs");
			Organization orgContainer = tenantOrTopologyConfigurationSession.GetOrgContainer();
			return orgContainer.Heuristics.HasFlag(HeuristicsFlags.PublicFolderMailboxesLockedForNewConnections);
		}

		private static LocalizedException GetLocalizedException(RopExecutionException ree)
		{
			if (ree == null || ree.InnerException == null)
			{
				return null;
			}
			LocalizedException ex = ree.InnerException as LocalizedException;
			if (ex != null)
			{
				return ex;
			}
			RopExecutionException ex2 = ree.InnerException as RopExecutionException;
			if (ex2 != null)
			{
				return CommonUtils.GetLocalizedException(ex2);
			}
			return null;
		}

		public static ExceptionSide? GetExceptionSide(Exception exception)
		{
			return ExecutionContext.GetExceptionSide(exception);
		}

		public static void SetExceptionSide(Exception exception, ExceptionSide? side)
		{
			ExecutionContext.SetExceptionSide(exception, side);
		}

		public static byte[] GetSHA512Hash(string inputString)
		{
			byte[] hash;
			using (SHA512Cng sha512Cng = new SHA512Cng())
			{
				hash = CommonUtils.GetHash(inputString, sha512Cng);
			}
			return hash;
		}

		public static byte[] GetSHA1Hash(string inputString)
		{
			byte[] hash;
			using (SHA1 sha = new SHA1CryptoServiceProvider())
			{
				hash = CommonUtils.GetHash(inputString, sha);
			}
			return hash;
		}

		public static void AppendNewLinesAndFlush(StreamWriter writer)
		{
			writer.WriteLine();
			writer.WriteLine();
			writer.Flush();
		}

		public static string StreamToString(MemoryStream stream)
		{
			return Convert.ToBase64String(stream.ToArray());
		}

		public static void StringToStream(string strData, MemoryStream stream)
		{
			stream.SetLength(0L);
			stream.Position = 0L;
			byte[] array = Convert.FromBase64String(strData);
			stream.Write(array, 0, array.Length);
		}

		public static byte[] CompressData(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				return data;
			}
			return CommonUtils.CompressData(data, 0, data.Length);
		}

		public static byte[] CompressData(byte[] data, int offset, int count)
		{
			if (data == null || data.Length == 0)
			{
				return data;
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					gzipStream.Write(data, offset, count);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		public static byte[] DecompressData(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				return data;
			}
			byte[] result;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					using (MemoryStream memoryStream2 = new MemoryStream())
					{
						using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress, true))
						{
							byte[] array = new byte[4096];
							for (;;)
							{
								int num = gzipStream.Read(array, 0, array.Length);
								if (num == 0)
								{
									break;
								}
								memoryStream2.Write(array, 0, num);
							}
							result = memoryStream2.ToArray();
						}
					}
				}
			}
			catch (InvalidDataException innerException)
			{
				throw new InputDataIsInvalidPermanentException(innerException);
			}
			catch (EndOfStreamException innerException2)
			{
				throw new InputDataIsInvalidPermanentException(innerException2);
			}
			return result;
		}

		public static byte[] PackString(string data, bool useCompression)
		{
			if (string.IsNullOrEmpty(data))
			{
				return null;
			}
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (StreamWriter streamWriter = new StreamWriter(memoryStream))
				{
					streamWriter.Write(data);
					streamWriter.Flush();
					array = memoryStream.ToArray();
				}
			}
			if (useCompression)
			{
				return CommonUtils.CompressData(array);
			}
			return array;
		}

		public static string UnpackString(byte[] data, bool useCompression)
		{
			if (data == null)
			{
				return null;
			}
			byte[] buffer = useCompression ? CommonUtils.DecompressData(data) : data;
			string result;
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				using (StreamReader streamReader = new StreamReader(memoryStream))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}

		public static void WriteBlob(BinaryWriter writer, byte[] blob)
		{
			if (blob != null && blob.Length > 0)
			{
				writer.Write(blob.Length);
				writer.Write(blob);
				return;
			}
			writer.Write(0);
		}

		public static byte[] ReadBlob(BinaryReader reader)
		{
			byte[] result;
			try
			{
				int num = reader.ReadInt32();
				if (num < 0)
				{
					throw new InputDataIsInvalidPermanentException();
				}
				if (num == 0)
				{
					result = null;
				}
				else
				{
					result = reader.ReadBytes(num);
				}
			}
			catch (InvalidDataException innerException)
			{
				throw new InputDataIsInvalidPermanentException(innerException);
			}
			catch (EndOfStreamException innerException2)
			{
				throw new InputDataIsInvalidPermanentException(innerException2);
			}
			return result;
		}

		public static int ReadInt(BinaryReader reader)
		{
			int result;
			try
			{
				result = reader.ReadInt32();
			}
			catch (InvalidDataException innerException)
			{
				throw new InputDataIsInvalidPermanentException(innerException);
			}
			catch (EndOfStreamException innerException2)
			{
				throw new InputDataIsInvalidPermanentException(innerException2);
			}
			return result;
		}

		public static void TreatMissingFolderAsTransient(Action actionDelegate, byte[] folderID, Func<byte[], IFolder> openFolderDelegate)
		{
			CommonUtils.ProcessKnownExceptions(actionDelegate, delegate(Exception ex)
			{
				if (CommonUtils.ExceptionIs(ex, new WellKnownException[]
				{
					WellKnownException.ObjectNotFound
				}))
				{
					using (IFolder folder = openFolderDelegate(folderID))
					{
						if (folder == null)
						{
							throw new FolderIsMissingTransientException(ex);
						}
					}
				}
				return false;
			});
		}

		public static void CatchKnownExceptions(Action actionDelegate, Action<Exception> failureAction)
		{
			CommonUtils.FailureDelegate failureDelegate = delegate(Exception failure)
			{
				if (failureAction != null)
				{
					failureAction(failure);
				}
				return true;
			};
			CommonUtils.ProcessKnownExceptions(actionDelegate, failureDelegate);
		}

		public static void ProcessKnownExceptionsWithoutTracing(Action actionDelegate, CommonUtils.FailureDelegate processFailure)
		{
			if (actionDelegate == null)
			{
				throw new ArgumentNullException("actionDelegate");
			}
			if (processFailure == null)
			{
				throw new ArgumentNullException("processFailure");
			}
			try
			{
				actionDelegate();
			}
			catch (LocalizedException ex)
			{
				if (!processFailure(ex))
				{
					throw;
				}
			}
			catch (RopExecutionException ex2)
			{
				if (!processFailure(ex2))
				{
					throw;
				}
			}
			catch (BufferParseException ex3)
			{
				if (!processFailure(ex3))
				{
					throw;
				}
			}
			catch (BufferTooSmallException ex4)
			{
				if (!processFailure(ex4))
				{
					throw;
				}
			}
			catch (ClientBackoffException ex5)
			{
				if (!processFailure(ex5))
				{
					throw;
				}
			}
			catch (ExArgumentOutOfRangeException ex6)
			{
				if (!processFailure(ex6))
				{
					throw;
				}
			}
			catch (ExArgumentException ex7)
			{
				if (!processFailure(ex7))
				{
					throw;
				}
			}
			catch (ConfigurationException ex8)
			{
				if (!processFailure(ex8))
				{
					throw;
				}
			}
		}

		public static void ProcessKnownExceptions(Action actionDelegate, CommonUtils.FailureDelegate failureDelegate)
		{
			CommonUtils.FailureDelegate processFailure = delegate(Exception failure)
			{
				MrsTracer.Common.Error("Call failed: {0}\n{1}\nContext: {2}", new object[]
				{
					CommonUtils.FullExceptionMessage(failure),
					failure.StackTrace,
					ExecutionContext.GetDataContext(failure)
				});
				return failureDelegate == null || failureDelegate(failure);
			};
			CommonUtils.ProcessKnownExceptionsWithoutTracing(actionDelegate, processFailure);
		}

		public static byte[] ByteSerialize(LocalizedString localizedString)
		{
			byte[] result;
			try
			{
				result = SafeLocalizedStringSerializer.SafeSerialize(localizedString);
			}
			catch (SerializationException innerException)
			{
				throw new InputDataIsInvalidPermanentException(innerException);
			}
			return result;
		}

		public static LocalizedString ByteDeserialize(byte[] bytes)
		{
			LocalizedString result;
			try
			{
				result = SafeLocalizedStringSerializer.SafeDeserialize(bytes);
			}
			catch (SerializationException innerException)
			{
				throw new InputDataIsInvalidPermanentException(innerException);
			}
			catch (EndOfStreamException innerException2)
			{
				throw new InputDataIsInvalidPermanentException(innerException2);
			}
			return result;
		}

		public static byte[] DataContractSerialize<TObject>(TObject obj)
		{
			if (obj == null || obj.Equals(default(TObject)))
			{
				return null;
			}
			byte[] result;
			try
			{
				DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(TObject));
				using (MemoryStream memoryStream = new MemoryStream())
				{
					dataContractSerializer.WriteObject(memoryStream, obj);
					result = memoryStream.ToArray();
				}
			}
			catch (SerializationException innerException)
			{
				throw new InputDataIsInvalidPermanentException(innerException);
			}
			return result;
		}

		public static TObject DataContractDeserialize<TObject>(byte[] bytes)
		{
			if (bytes == null)
			{
				return default(TObject);
			}
			TObject result;
			try
			{
				DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(TObject));
				XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(bytes, XmlDictionaryReaderQuotas.Max);
				result = (TObject)((object)dataContractSerializer.ReadObject(reader, true));
			}
			catch (SerializationException innerException)
			{
				throw new InputDataIsInvalidPermanentException(innerException);
			}
			return result;
		}

		public static List<T> RandomizeSequence<T>(List<T> inputSequence)
		{
			List<T> list = new List<T>(inputSequence.Count);
			Random random = new Random();
			for (int i = 0; i < inputSequence.Count; i++)
			{
				int index = random.Next(list.Count + 1);
				list.Insert(index, inputSequence[i]);
			}
			return list;
		}

		public static bool IsMultiTenantEnabled()
		{
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled;
		}

		public static bool ShouldHonorProvisioningSettings()
		{
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Mrs.CheckProvisioningSettings.Enabled;
		}

		public static bool ShouldUseExtendedAclInformation(ISourceMailbox sourceMbx, IDestinationMailbox destinationMbx)
		{
			return ConfigBase<MRSConfigSchema>.GetConfig<bool>("UseExtendedAclInformation") && sourceMbx.IsCapabilitySupported(MRSProxyCapabilities.ExtendedAclInformation) && destinationMbx.IsCapabilitySupported(MRSProxyCapabilities.ExtendedAclInformation) && sourceMbx.IsMailboxCapabilitySupported(MailboxCapabilities.ExtendedAclInformation) && destinationMbx.IsMailboxCapabilitySupported(MailboxCapabilities.ExtendedAclInformation);
		}

		public static TenantPartitionHint GetPartitionHint(OrganizationId orgId)
		{
			if (orgId == null || orgId == OrganizationId.ForestWideOrgId)
			{
				return null;
			}
			return TenantPartitionHint.FromOrganizationId(orgId);
		}

		public static IRecipientSession CreateRecipientSession(TenantPartitionHint partitionHint, NetworkCredential cred, string dcName)
		{
			ADSessionSettings adsessionSettings;
			if (partitionHint != null)
			{
				adsessionSettings = ADSessionSettings.FromTenantPartitionHint(partitionHint);
			}
			else
			{
				adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			adsessionSettings.IncludeSoftDeletedObjects = true;
			adsessionSettings.IncludeInactiveMailbox = true;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(dcName, true, ConsistencyMode.PartiallyConsistent, cred, adsessionSettings, 1993, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\Common.cs");
			tenantOrRootOrgRecipientSession.EnforceDefaultScope = false;
			return tenantOrRootOrgRecipientSession;
		}

		public static IRecipientSession CreateRecipientSession(Guid externalDirectoryOrgId, NetworkCredential cred, string dcName)
		{
			TenantPartitionHint partitionHint = null;
			if (externalDirectoryOrgId != Guid.Empty)
			{
				partitionHint = CommonUtils.GetPartitionHint(OrganizationId.FromExternalDirectoryOrganizationId(externalDirectoryOrgId));
			}
			return CommonUtils.CreateRecipientSession(partitionHint, cred, dcName);
		}

		public static MiniRecipient FindUserByMailboxGuid(Guid mailboxGuid, TenantPartitionHint partitionHint, NetworkCredential cred, string dcName, ADPropertyDefinition[] additionalProperties)
		{
			IRecipientSession recipientSession = CommonUtils.CreateRecipientSession(partitionHint, cred, dcName);
			if (!Globals.IsConsumerOrganization(recipientSession.SessionSettings.CurrentOrganizationId) || !ADSessionFactory.UseAggregateSession(recipientSession.SessionSettings))
			{
				return recipientSession.FindByExchangeGuid<MiniRecipient>(mailboxGuid, additionalProperties);
			}
			ulong puid;
			if (!ConsumerIdentityHelper.TryGetPuidFromGuid(mailboxGuid, out puid))
			{
				return null;
			}
			ADObjectId adobjectIdFromPuid = ConsumerIdentityHelper.GetADObjectIdFromPuid(puid);
			return recipientSession.ReadMiniRecipient(adobjectIdFromPuid, additionalProperties);
		}

		public static MailboxDatabase FindMdbByName(string mdbName, NetworkCredential cred, string dcName)
		{
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(dcName, true, ConsistencyMode.PartiallyConsistent, cred, ADSessionSettings.FromRootOrgScopeSet(), 2066, "FindMdbByName", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\Common.cs");
			DatabaseIdParameter databaseIdParameter = DatabaseIdParameter.Parse(mdbName);
			databaseIdParameter.AllowLegacy = true;
			LocalizedString? localizedString;
			IEnumerable<MailboxDatabase> objects = databaseIdParameter.GetObjects<MailboxDatabase>(configurationSession.GetExchangeConfigurationContainer().Id, configurationSession, null, out localizedString);
			MailboxDatabase result;
			using (IEnumerator<MailboxDatabase> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new MailboxDatabaseNotFoundByIdPermanentException(mdbName, localizedString ?? LocalizedString.Empty);
				}
				result = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new MailboxDatabaseNotUniquePermanentException(mdbName);
				}
			}
			return result;
		}

		public static MailboxDatabase FindMdbByGuid(Guid mdbGuid, NetworkCredential cred, string dcName)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(dcName, true, ConsistencyMode.PartiallyConsistent, cred, ADSessionSettings.FromRootOrgScopeSet(), 2114, "FindMdbByGuid", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\Common.cs");
			MailboxDatabase mailboxDatabase = topologyConfigurationSession.FindDatabaseByGuid<MailboxDatabase>(mdbGuid);
			if (mailboxDatabase == null)
			{
				MrsTracer.Common.Error("Unable to locate MDB by guid {0}", new object[]
				{
					mdbGuid
				});
				throw new MailboxDatabaseNotFoundByIdPermanentException(mdbGuid.ToString(), LocalizedString.Empty);
			}
			return mailboxDatabase;
		}

		public static ProxyServerSettings MapDatabaseToProxyServer(Guid mdbGuid)
		{
			return CommonUtils.MapDatabaseToProxyServer(new ADObjectId(mdbGuid, PartitionId.LocalForest.ForestFQDN));
		}

		public static ProxyServerSettings MapDatabaseToProxyServer(ADObjectId databaseId)
		{
			LocalMailboxFlags extraFlags = LocalMailboxFlags.None;
			DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(databaseId, null, null, FindServerFlags.None);
			string serverFqdn;
			ProxyScenarios proxyScenario;
			if (databaseInformation.ServerVersion < CommonUtils.MinExchangeVersionForSameForestMrsProxyMove)
			{
				MrsTracer.Service.Debug("Using local mrs proxy while connecting to exchange server {0}, version {1}", new object[]
				{
					databaseInformation.ServerFqdn,
					databaseInformation.ServerVersion
				});
				serverFqdn = CommonUtils.LocalComputerName;
				proxyScenario = ProxyScenarios.LocalProxyRemoteMdb;
			}
			else
			{
				serverFqdn = databaseInformation.ServerFqdn;
				extraFlags = LocalMailboxFlags.LocalMachineMapiOnly;
				proxyScenario = (databaseInformation.IsOnThisServer ? ProxyScenarios.LocalMdbAndProxy : ProxyScenarios.RemoteMdbAndProxy);
			}
			return new ProxyServerSettings(serverFqdn, extraFlags, proxyScenario);
		}

		public static ProxyServerSettings MapMailboxToProxyServer(Guid? physicalMailboxGuid, Guid? primaryMailboxGuid, byte[] tenantPartitionHintBytes)
		{
			ADObjectId adobjectId = null;
			if (primaryMailboxGuid == null || physicalMailboxGuid == null)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			TenantPartitionHint partitionHint = null;
			if (tenantPartitionHintBytes != null)
			{
				partitionHint = TenantPartitionHint.FromPersistablePartitionHint(tenantPartitionHintBytes);
			}
			MiniRecipient miniRecipient = CommonUtils.FindUserByMailboxGuid(primaryMailboxGuid.Value, partitionHint, null, null, null);
			if (miniRecipient == null)
			{
				throw new RecipientNotFoundPermanentException(primaryMailboxGuid.Value);
			}
			if (miniRecipient.ExchangeGuid == physicalMailboxGuid.Value || (miniRecipient.AggregatedMailboxGuids != null && miniRecipient.AggregatedMailboxGuids.Contains(physicalMailboxGuid.Value)))
			{
				adobjectId = miniRecipient.Database;
			}
			else
			{
				if (miniRecipient.ArchiveGuid != physicalMailboxGuid.Value)
				{
					throw new RecipientArchiveGuidMismatchPermanentException(miniRecipient.ExchangeGuid.ToString(), miniRecipient.ArchiveGuid, physicalMailboxGuid.Value);
				}
				if (miniRecipient.ArchiveDatabase != null)
				{
					adobjectId = miniRecipient.ArchiveDatabase;
				}
			}
			if (adobjectId == null)
			{
				throw new RecipientIsNotAMailboxPermanentException(physicalMailboxGuid.Value.ToString());
			}
			return CommonUtils.MapDatabaseToProxyServer(adobjectId);
		}

		private static ForestType GetForestType()
		{
			if (!CommonUtils.IsMultiTenantEnabled())
			{
				return ForestType.Enterprise;
			}
			string text = NativeHelpers.GetForestName();
			if (text != null)
			{
				text = text.ToLower();
				if (text.EndsWith("extest.microsoft.com"))
				{
					return ForestType.TestTopology;
				}
				if (text.EndsWith("sdf.exchangelabs.com"))
				{
					return ForestType.ServiceDogfood;
				}
			}
			return ForestType.ServiceProduction;
		}

		public static string GetExchangeInstallPath()
		{
			string text = "SOFTWARE\\Microsoft\\ExchangeServer\\V15\\Setup";
			string result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text))
			{
				if (registryKey == null)
				{
					throw new RegKeyNotExistMigrationException(text);
				}
				result = registryKey.GetValue("MsiInstallPath").ToString();
			}
			return result;
		}

		public static IEnumerable<Server> GetMailboxServer(ITopologyConfigurationSession session, int minVersion)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, minVersion),
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL)
			});
			return CommonUtils.GetServer(session, filter);
		}

		public static IEnumerable<Server> GetMailboxServer(ITopologyConfigurationSession session, int minVersion, int maxVersion)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, minVersion),
				new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, maxVersion),
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL)
			});
			return CommonUtils.GetServer(session, filter);
		}

		public static IEnumerable<Server> GetServer(ITopologyConfigurationSession session, QueryFilter filter)
		{
			return session.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 1000);
		}

		public static NetworkCredential GetNetworkCredential(PSCredential psCred, AuthenticationMethod? authMethod)
		{
			if (psCred == null)
			{
				return null;
			}
			NetworkCredential networkCredential = psCred.GetNetworkCredential();
			if (authMethod == AuthenticationMethod.LiveIdBasic)
			{
				networkCredential.UserName = psCred.UserName;
				networkCredential.Domain = string.Empty;
			}
			return networkCredential;
		}

		public static void ProcessInBatches<T>(T[] data, int batchSize, Action<T[]> batchAction)
		{
			if (data == null || batchSize <= 0)
			{
				return;
			}
			if (data.Length <= batchSize)
			{
				batchAction(data);
				return;
			}
			T[] array = new T[batchSize];
			for (int i = 0; i < data.Length; i += batchSize)
			{
				if (i + batchSize > data.Length)
				{
					batchSize = data.Length - i;
					array = new T[batchSize];
				}
				Array.Copy(data, i, array, 0, batchSize);
				batchAction(array);
			}
		}

		public static List<T> ExtractBatch<T>(ref List<T> data, ref int remainingCount, out bool moreData)
		{
			if (remainingCount == 0)
			{
				moreData = (data != null && data.Count > 0);
				return null;
			}
			List<T> result;
			if (data == null || data.Count <= remainingCount)
			{
				if (data != null)
				{
					remainingCount -= data.Count;
				}
				result = data;
				data = null;
				moreData = false;
			}
			else
			{
				result = data.GetRange(0, remainingCount);
				data.RemoveRange(0, remainingCount);
				moreData = true;
				remainingCount = 0;
			}
			return result;
		}

		public static void AppendBatch<T>(ref List<T> data, List<T> batch)
		{
			if (batch != null)
			{
				if (data == null)
				{
					data = batch;
					return;
				}
				data.AddRange(batch);
			}
		}

		public static string ConcatEntries<T>(ICollection<T> listOfEntries, Func<T, string> entryToString = null)
		{
			if (listOfEntries == null)
			{
				return "(null)";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[count:{0}, ", listOfEntries.Count);
			bool flag = true;
			foreach (T t in listOfEntries)
			{
				if (!flag)
				{
					stringBuilder.Append("; ");
				}
				else
				{
					flag = false;
				}
				string value = (entryToString != null) ? entryToString(t) : ((t != null) ? t.ToString() : "(null)");
				stringBuilder.Append(value);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public static long TimestampToPerfcounterLong(DateTime ts)
		{
			DateTime dateTime = ts.ToUniversalTime();
			long num = (long)dateTime.Year;
			num *= 100L;
			num += (long)dateTime.Month;
			num *= 100L;
			num += (long)dateTime.Day;
			num *= 100L;
			num += (long)dateTime.Hour;
			num *= 100L;
			num += (long)dateTime.Minute;
			num *= 100L;
			return num + (long)dateTime.Second;
		}

		public static ExDateTime TimestampFromPerfcounterLong(long tsl)
		{
			int second = (int)(tsl % 100L);
			tsl /= 100L;
			int minute = (int)(tsl % 100L);
			tsl /= 100L;
			int hour = (int)(tsl % 100L);
			tsl /= 100L;
			int day = (int)(tsl % 100L);
			tsl /= 100L;
			int month = (int)(tsl % 100L);
			tsl /= 100L;
			int year = (int)tsl;
			ExDateTime result;
			try
			{
				result = new ExDateTime(ExTimeZone.UtcTimeZone, year, month, day, hour, minute, second);
			}
			catch (ArgumentOutOfRangeException)
			{
				result = ExDateTime.MinValue;
			}
			return result;
		}

		public static bool IsValueInWildcardedList(string value, string delimitedList)
		{
			if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(delimitedList))
			{
				return false;
			}
			string[] array = delimitedList.Split(new char[]
			{
				';'
			});
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					bool result;
					if (text == "*")
					{
						result = true;
					}
					else
					{
						Regex regex = new Regex(Wildcard.ConvertToRegexPattern(text), RegexOptions.IgnoreCase);
						if (!regex.IsMatch(value))
						{
							goto IL_6A;
						}
						result = true;
					}
					return result;
				}
				IL_6A:;
			}
			return false;
		}

		public static string ConfigurableObjectToString(ConfigurableObject obj)
		{
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
			foreach (PropertyDefinition propertyDefinition in obj.ObjectSchema.AllProperties)
			{
				ProviderPropertyDefinition providerPropertyDefinition = propertyDefinition as ProviderPropertyDefinition;
				if (providerPropertyDefinition != null)
				{
					object obj2 = obj[providerPropertyDefinition];
					if (obj2 != null)
					{
						sortedDictionary[providerPropertyDefinition.Name] = CommonUtils.DumpProperty(providerPropertyDefinition, obj2);
					}
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in sortedDictionary)
			{
				stringBuilder.AppendLine(string.Format("{0}: {1}", keyValuePair.Key, keyValuePair.Value));
			}
			return stringBuilder.ToString();
		}

		public static void SafeWait(EventWaitHandle evToWait, Thread owningThread)
		{
			while (!evToWait.WaitOne(CommonUtils.threadAlivePollInterval, false))
			{
				if (owningThread != null && !owningThread.IsAlive)
				{
					throw new DataExportTransientException(new DataExportTimeoutTransientException());
				}
			}
		}

		public static bool SafeWaitWithTimeout(EventWaitHandle evToWait, Thread owningThread, TimeSpan timeout)
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			for (;;)
			{
				TimeSpan timeSpan2 = timeout - timeSpan;
				if (timeSpan2 > CommonUtils.threadAlivePollInterval)
				{
					timeSpan2 = CommonUtils.threadAlivePollInterval;
				}
				if (evToWait.WaitOne(timeSpan2, false))
				{
					break;
				}
				timeSpan += timeSpan2;
				if (timeSpan >= timeout)
				{
					return false;
				}
				if (!owningThread.IsAlive)
				{
					goto Block_4;
				}
			}
			return true;
			Block_4:
			throw new DataExportTransientException(new DataExportTimeoutTransientException());
		}

		public static bool IsSameEntryId(byte[] eid1, byte[] eid2)
		{
			return ArrayComparer<byte>.EqualityComparer.Equals(eid1, eid2);
		}

		public static void DispatchCallsInParallel<T>(T[] inputArray, Action<T> entryProcessor, Action<T, Exception> failureProcessor)
		{
			if (inputArray == null || inputArray.Length == 0)
			{
				return;
			}
			int outstandingCalls = inputArray.Length;
			ManualResetEvent allCallsDone = new ManualResetEvent(false);
			foreach (T t in inputArray)
			{
				ThreadPool.QueueUserWorkItem(delegate(object ctx)
				{
					try
					{
						T currentEntry = (T)((object)ctx);
						CommonUtils.CatchKnownExceptions(delegate
						{
							entryProcessor(currentEntry);
						}, delegate(Exception failure)
						{
							failureProcessor(currentEntry, failure);
						});
					}
					finally
					{
						Interlocked.Decrement(ref outstandingCalls);
						if (outstandingCalls <= 0)
						{
							allCallsDone.Set();
						}
					}
				}, t);
			}
			WaitHandle[] waitHandles = new WaitHandle[]
			{
				allCallsDone,
				CommonUtils.ServiceIsStoppingEvent
			};
			if (WaitHandle.WaitAny(waitHandles) == 1)
			{
				throw new ServiceIsStoppingPermanentException();
			}
		}

		public static SettingsContextBase CreateConfigContext(Guid mailboxGuid, Guid mdbGuid, OrganizationId orgId, RequestWorkloadType workloadType, MRSRequestType requestType, SyncProtocol syncProtocol)
		{
			SettingsContextBase settingsContextBase = new GenericSettingsContext("RequestType", requestType.ToString(), null);
			if (workloadType != RequestWorkloadType.None)
			{
				settingsContextBase = new GenericSettingsContext("RequestWorkloadType", workloadType.ToString(), settingsContextBase);
			}
			if (mailboxGuid != Guid.Empty)
			{
				settingsContextBase = new MailboxSettingsContext(mailboxGuid, settingsContextBase);
			}
			if (mdbGuid != Guid.Empty)
			{
				settingsContextBase = new DatabaseSettingsContext(mdbGuid, settingsContextBase);
			}
			if (orgId != null && orgId != OrganizationId.ForestWideOrgId)
			{
				settingsContextBase = new OrganizationSettingsContext(orgId, settingsContextBase);
			}
			if (syncProtocol != SyncProtocol.None)
			{
				settingsContextBase = new GenericSettingsContext("SyncProtocol", syncProtocol.ToString(), settingsContextBase);
			}
			return settingsContextBase;
		}

		public static WorkloadType ComputeWlmWorkloadType(int priority, bool isInteractive, WorkloadType workloadType)
		{
			if (priority > 50)
			{
				return WorkloadType.MailboxReplicationServiceHighPriority;
			}
			if (isInteractive)
			{
				return WorkloadType.MailboxReplicationServiceInteractive;
			}
			if (workloadType != WorkloadType.MailboxReplicationService && workloadType != WorkloadType.MailboxReplicationServiceHighPriority)
			{
				switch (workloadType)
				{
				case WorkloadType.MailboxReplicationServiceInternalMaintenance:
				case WorkloadType.MailboxReplicationServiceInteractive:
					break;
				default:
					throw new ArgumentException(string.Format("WorkloadType {0} is not a MailboxReplicationService WorkloadType", workloadType));
				}
			}
			return workloadType;
		}

		private static byte[] GetHash(string inputString, HashAlgorithm hash)
		{
			byte[] result;
			try
			{
				result = hash.ComputeHash(Encoding.UTF8.GetBytes(inputString));
			}
			catch (ArgumentNullException innerException)
			{
				throw new InputDataIsInvalidPermanentException(innerException);
			}
			catch (EncoderFallbackException innerException2)
			{
				throw new InputDataIsInvalidPermanentException(innerException2);
			}
			return result;
		}

		private static WellKnownException[] ClassifyException(Exception ex, int depth)
		{
			if (depth > 10)
			{
				MrsTracer.Common.Warning("Inner localized exception are too deep", new object[0]);
				return Array<WellKnownException>.Empty;
			}
			IMRSRemoteException ex2 = ex as IMRSRemoteException;
			if (ex2 != null && ex2.WKEClasses != null)
			{
				return CommonUtils.GetCompatibleRemoteExceptions(ex2);
			}
			List<WellKnownException> list = new List<WellKnownException>();
			if (ex is LocalizedException)
			{
				list.Add(WellKnownException.Exchange);
			}
			if (ex is TransientException)
			{
				list.Add(WellKnownException.Transient);
			}
			if (ex is MailboxReplicationTransientException)
			{
				list.Add(WellKnownException.MRS);
				list.Add(WellKnownException.MRSTransient);
			}
			if (ex is MailboxReplicationPermanentException)
			{
				list.Add(WellKnownException.MRS);
				list.Add(WellKnownException.MRSPermanent);
			}
			if (ex is MRSProxyConnectionLimitReachedTransientException)
			{
				list.Add(WellKnownException.MRSProxyLimitReached);
			}
			if (ex is ResourceUnhealthyException)
			{
				list.Add(WellKnownException.UnhealthyResource);
			}
			if (ex is UpdateMovedMailboxTransientException || ex is UpdateMovedMailboxPermanentException)
			{
				list.Add(WellKnownException.MRSUpdateMovedMailbox);
			}
			if (ex is SourceMailboxAlreadyBeingMovedTransientException || ex is SourceMailboxAlreadyBeingMovedPermanentException || ex is DestMailboxAlreadyBeingMovedTransientException || ex is DestMailboxAlreadyBeingMovedPermanentException)
			{
				list.Add(WellKnownException.MRSMailboxIsLocked);
			}
			if (ex is OlcSettingNotImplementedPermanentException)
			{
				list.Add(WellKnownException.MrsOlcSettingNotImplemented);
			}
			if (ex is PropertyMismatchException)
			{
				list.Add(WellKnownException.MrsPropertyMismatch);
			}
			if (ex is ObjectNotFoundException)
			{
				list.Add(WellKnownException.ObjectNotFound);
			}
			if (ex is FastTransferArgumentException)
			{
				list.Add(WellKnownException.Storage);
				list.Add(WellKnownException.DataProviderPermanent);
			}
			if (ex is UnableToReadPSTMessagePermanentException)
			{
				list.Add(WellKnownException.MRSUnableToReadPSTMessage);
			}
			if (ex is UnableToGetPSTPropsPermanentException || ex is UnableToStreamPSTPropPermanentException)
			{
				list.Add(WellKnownException.MRSUnableToGetPSTProps);
			}
			if (ex is UnableToCreatePSTMessagePermanentException)
			{
				list.Add(WellKnownException.MRSUnableToCreatePSTMessage);
			}
			if (ex is EasFetchFailedPermanentException)
			{
				list.Add(WellKnownException.MRSUnableToFetchEasMessage);
			}
			if (ex is MapiRetryableException)
			{
				list.Add(WellKnownException.Mapi);
				list.Add(WellKnownException.DataProviderTransient);
			}
			if (ex is MapiPermanentException)
			{
				list.Add(WellKnownException.Mapi);
				list.Add(WellKnownException.DataProviderPermanent);
			}
			if (ex is MapiExceptionMdbOffline)
			{
				list.Add(WellKnownException.MapiMdbOffline);
			}
			if (ex is MapiExceptionWrongServer)
			{
				list.Add(WellKnownException.WrongServer);
			}
			if (ex is MapiExceptionCorruptMidsetDeleted || ex is CorruptMidsetDeletedException)
			{
				list.Add(WellKnownException.MapiCorruptMidsetDeleted);
			}
			if (ex is MapiExceptionVersion)
			{
				list.Add(WellKnownException.MapiVersion);
			}
			if (ex is MapiExceptionRetryableImportFailure || ex is MapiExceptionPermanentImportFailure)
			{
				list.Add(WellKnownException.MapiImportFailure);
			}
			if (ex is CorruptRestrictionDataException || ex is MapiExceptionAmbiguousAlias || ex is MapiExceptionCorruptData || ex is CorruptDataException)
			{
				list.Add(WellKnownException.CorruptData);
			}
			if (ex is MapiExceptionExiting)
			{
				list.Add(WellKnownException.MapiExiting);
			}
			if (ex is MapiExceptionLogonFailed)
			{
				list.Add(WellKnownException.MapiLogonFailed);
			}
			if (ex is MapiExceptionCannotRegisterNewReplidGuidMapping || ex is MapiExceptionCannotRegisterNewNamedPropertyMapping)
			{
				list.Add(WellKnownException.MapiCannotRegisterMapping);
			}
			if (ex is MapiExceptionNetworkError)
			{
				list.Add(WellKnownException.MapiNetworkError);
			}
			if (ex is MapiExceptionSessionLimit)
			{
				list.Add(WellKnownException.MapiSessionLimit);
			}
			if (ex is MapiExceptionMaxObjsExceeded)
			{
				list.Add(WellKnownException.MapiMaxObjectsExceeded);
			}
			if (ex is MapiExceptionShutoffQuotaExceeded)
			{
				list.Add(WellKnownException.MapiShutoffQuotaExceeded);
			}
			if (ex is MapiExceptionCannotPreserveMailboxSignature)
			{
				list.Add(WellKnownException.MapiCannotPreserveSignature);
			}
			if (ex is MapiExceptionNonCanonicalACL || ex is NonCanonicalACLException)
			{
				list.Add(WellKnownException.NonCanonicalACL);
			}
			if (ex is MapiExceptionPartialCompletion)
			{
				list.Add(WellKnownException.MapiPartialCompletion);
			}
			if (ex is MapiExceptionMaxSubmissionExceeded)
			{
				list.Add(WellKnownException.MapiMaxSubmissionExceeded);
			}
			if (ex is MapiExceptionInvalidParameter)
			{
				list.Add(WellKnownException.MapiInvalidParameter);
			}
			if (ex is MapiExceptionNotFound)
			{
				list.Add(WellKnownException.ObjectNotFound);
			}
			if (ex is MapiExceptionObjectDeleted)
			{
				list.Add(WellKnownException.ObjectNotFound);
			}
			if (ex is MapiExceptionRpcBufferTooSmall)
			{
				list.Add(WellKnownException.MapiRpcBufferTooSmall);
			}
			if (ex is MapiExceptionNotEnoughMemory)
			{
				list.Add(WellKnownException.MapiNotEnoughMemory);
			}
			if (ex is MapiExceptionObjectChanged)
			{
				list.Add(WellKnownException.MapiObjectChanged);
			}
			if (ex is MapiExceptionNoAccess)
			{
				list.Add(WellKnownException.MapiNoAccess);
			}
			if (ex is MapiExceptionMailboxInTransit)
			{
				list.Add(WellKnownException.MapiMailboxInTransit);
			}
			if (ex is MapiExceptionADUnavailable)
			{
				list.Add(WellKnownException.MapiADUnavailable);
			}
			if (ex is WrongServerException)
			{
				list.Add(WellKnownException.WrongServer);
			}
			if (ex is CorruptRestrictionDataException || ex is MapiExceptionAmbiguousAlias || ex is MapiExceptionCorruptData)
			{
				list.Add(WellKnownException.MapiCorruptData);
			}
			if (ex is MapiExceptionGlobalCounterRangeExceeded || ex is GlobalCounterRangeExceededException)
			{
				list.Add(WellKnownException.GlobalCounterRangeExceeded);
			}
			if (ex is StoragePermanentException)
			{
				list.Add(WellKnownException.Storage);
				list.Add(WellKnownException.DataProviderPermanent);
				if (ex.InnerException is LocalizedException && !list.Contains(WellKnownException.WrongServer))
				{
					list.AddRange(CommonUtils.ClassifyException(ex.InnerException, depth + 1));
				}
			}
			if (ex is CommandExecutionException)
			{
				list.Add(WellKnownException.CommandExecution);
				if (ex.InnerException is LocalizedException)
				{
					list.AddRange(CommonUtils.ClassifyException(ex.InnerException, depth + 1));
				}
			}
			if (ex is StorageTransientException)
			{
				list.Add(WellKnownException.Storage);
				list.Add(WellKnownException.DataProviderTransient);
				if (ex.InnerException is LocalizedException)
				{
					list.AddRange(CommonUtils.ClassifyException(ex.InnerException, depth + 1));
				}
			}
			if (ex is PropertyErrorException)
			{
				PropertyErrorException ex3 = ex as PropertyErrorException;
				if (ex3.PropertyErrors != null)
				{
					foreach (PropertyError propertyError in ex3.PropertyErrors)
					{
						if (propertyError.PropertyErrorCode == PropertyErrorCode.UnknownError && propertyError.PropertyDefinition is PropertyTagPropertyDefinition)
						{
							PropertyTagPropertyDefinition propertyTagPropertyDefinition = propertyError.PropertyDefinition as PropertyTagPropertyDefinition;
							if (propertyTagPropertyDefinition.PropertyTag == PropertyTag.ConflictEntryId)
							{
								list.Add(WellKnownException.ConflictEntryIdCorruption);
								break;
							}
						}
					}
				}
			}
			if (ex is PropertyValidationException)
			{
				PropertyValidationException ex4 = ex as PropertyValidationException;
				if (ex4.PropertyValidationErrors != null)
				{
					foreach (PropertyValidationError propertyValidationError in ex4.PropertyValidationErrors)
					{
						if (propertyValidationError is InvalidMultivalueElementError)
						{
							list.Add(WellKnownException.InvalidMultivalueElement);
							break;
						}
					}
				}
			}
			if (ex is CannotMoveDefaultFolderException)
			{
				list.Add(WellKnownException.StorageCannotMoveDefaultFolder);
			}
			if (ex is CorruptRecipientException)
			{
				list.Add(WellKnownException.CorruptRecipient);
			}
			if (ex is RopExecutionException)
			{
				list.Add(WellKnownException.FxParser);
				if (ex.InnerException is LocalizedException || ex.InnerException is RopExecutionException)
				{
					list.AddRange(CommonUtils.ClassifyException(ex.InnerException, depth + 1));
				}
				else
				{
					list.Add(WellKnownException.DataProviderPermanent);
				}
			}
			if (ex is BufferParseException)
			{
				list.Add(WellKnownException.FxParser);
				list.Add(WellKnownException.BufferParse);
				list.Add(WellKnownException.DataProviderPermanent);
			}
			if (ex is BufferTooSmallException)
			{
				list.Add(WellKnownException.FxParser);
				list.Add(WellKnownException.BufferTooSmall);
				list.Add(WellKnownException.DataProviderPermanent);
			}
			if (ex is ClientBackoffException)
			{
				list.Add(WellKnownException.FxParser);
				list.Add(WellKnownException.ClientBackoff);
				list.Add(WellKnownException.DataProviderTransient);
			}
			if (ex is ExArgumentOutOfRangeException)
			{
				list.Add(WellKnownException.Storage);
				list.Add(WellKnownException.ExArgumentOutOfRange);
				list.Add(WellKnownException.DataProviderPermanent);
			}
			if (ex is ExArgumentException)
			{
				list.Add(WellKnownException.Storage);
				list.Add(WellKnownException.ExArgument);
				list.Add(WellKnownException.DataProviderPermanent);
			}
			if (ex is ResourceReservationException)
			{
				list.Add(WellKnownException.ResourceReservation);
				if (ex is StaticCapacityExceededReservationException)
				{
					list.Add(WellKnownException.StaticCapacityExceededReservation);
				}
				else if (ex is WlmCapacityExceededReservationException)
				{
					list.Add(WellKnownException.WlmCapacityExceededReservation);
				}
				else if (ex is WlmResourceUnhealthyException)
				{
					list.Add(WellKnownException.WlmResourceUnhealthy);
				}
			}
			if (ex is ExpiredReservationException)
			{
				list.Add(WellKnownException.ExpiredReservation);
			}
			if (ex is ADOperationException)
			{
				list.Add(WellKnownException.AD);
				list.Add(WellKnownException.ADOperation);
			}
			if (ex is ADTransientException)
			{
				list.Add(WellKnownException.AD);
				list.Add(WellKnownException.ADTransient);
			}
			return list.ToArray();
		}

		private static WellKnownException[] GetCompatibleRemoteExceptions(IMRSRemoteException remoteException)
		{
			List<WellKnownException> list = new List<WellKnownException>(remoteException.WKEClasses.Length);
			foreach (WellKnownException ex in remoteException.WKEClasses)
			{
				list.Add(ex);
				WellKnownException ex2 = ex;
				if (ex2 <= WellKnownException.MapiCorruptData)
				{
					switch (ex2)
					{
					case WellKnownException.MapiRetryable:
						list.Add(WellKnownException.DataProviderTransient);
						break;
					case WellKnownException.MapiPermanent:
						list.Add(WellKnownException.DataProviderPermanent);
						break;
					default:
						if (ex2 != WellKnownException.MapiWrongServer)
						{
							if (ex2 == WellKnownException.MapiCorruptData)
							{
								list.Add(WellKnownException.CorruptData);
							}
						}
						else
						{
							list.Add(WellKnownException.WrongServer);
						}
						break;
					}
				}
				else if (ex2 != WellKnownException.MapiNonCanonicalACL)
				{
					if (ex2 == WellKnownException.MapiNotFound || ex2 == WellKnownException.MapiObjectDeleted)
					{
						list.Add(WellKnownException.ObjectNotFound);
					}
				}
				else
				{
					list.Add(WellKnownException.NonCanonicalACL);
				}
			}
			return list.ToArray();
		}

		public static double Randomize(double input, double variation)
		{
			if (variation == 0.0)
			{
				return input;
			}
			Random random = new Random();
			double num = random.NextDouble() * 2.0 - 1.0;
			return input * (1.0 + variation * num);
		}

		public static string GetSDDLString(RawSecurityDescriptor securityDescriptor)
		{
			if (securityDescriptor == null)
			{
				return "(null)";
			}
			string result;
			try
			{
				result = securityDescriptor.GetSddlForm(AccessControlSections.All);
			}
			catch (Exception arg)
			{
				result = string.Format("(Invalid SD, error={0})", arg);
			}
			return result;
		}

		public static void CleanupMoveRequestInAD(IRecipientSession writeableSession, ADObjectId userId, ADUser adUser)
		{
			if (adUser == null)
			{
				adUser = (writeableSession.Read(userId) as ADUser);
			}
			if (adUser != null)
			{
				adUser.MailboxMoveStatus = RequestStatus.None;
				adUser.MailboxMoveFlags = RequestFlags.None;
				adUser.MailboxMoveTargetMDB = null;
				adUser.MailboxMoveSourceMDB = null;
				adUser.MailboxMoveTargetArchiveMDB = null;
				adUser.MailboxMoveSourceArchiveMDB = null;
				adUser.MailboxMoveRemoteHostName = null;
				adUser.MailboxMoveBatchName = null;
				writeableSession.Save(adUser);
				return;
			}
			throw new UnableToReadADUserException(userId.ToGuidOrDNString());
		}

		public static XElement GetBinaryVersions(string assemblyNamePattern)
		{
			XElement xelement = new XElement("BinaryVersions");
			IEnumerable<string> enumerable;
			if (!string.IsNullOrEmpty(assemblyNamePattern))
			{
				List<string> list = new List<string>(5);
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					string value = assembly.FullName.Split(new char[]
					{
						','
					})[0];
					if (CommonUtils.IsValueInWildcardedList(value, assemblyNamePattern))
					{
						list.Add(assembly.FullName);
					}
				}
				enumerable = list;
			}
			else
			{
				enumerable = CommonUtils.InterestingAssemblies;
			}
			foreach (string text in enumerable)
			{
				string value2 = text.Split(new char[]
				{
					','
				})[0];
				try
				{
					Assembly element = Assembly.Load(text);
					AssemblyFileVersionAttribute customAttribute = element.GetCustomAttribute<AssemblyFileVersionAttribute>();
					if (customAttribute != null)
					{
						xelement.Add(new XElement("Assembly", new object[]
						{
							new XAttribute("Name", value2),
							new XAttribute("Version", customAttribute.Version)
						}));
					}
				}
				catch (InvalidOperationException)
				{
				}
				catch (Exception ex)
				{
					xelement.Add(new XElement("Assembly", new object[]
					{
						new XAttribute("Name", value2),
						string.Format("Assembly load exception {0}: {1}", ex.GetType().Name, ex.Message)
					}));
				}
			}
			return xelement;
		}

		public static void ValidateTargetDeliveryDomain(ProxyAddressCollection proxies, string targetDeliveryDomain)
		{
			if (proxies != null)
			{
				foreach (ProxyAddress proxyAddress in proxies)
				{
					SmtpProxyAddress smtpProxyAddress = proxyAddress as SmtpProxyAddress;
					if (smtpProxyAddress != null)
					{
						SmtpAddress smtpAddress = new SmtpAddress(smtpProxyAddress.SmtpAddress);
						if (StringComparer.InvariantCultureIgnoreCase.Equals(smtpAddress.Domain, targetDeliveryDomain))
						{
							return;
						}
					}
				}
			}
			throw new TargetDeliveryDomainMismatchPermanentException(targetDeliveryDomain);
		}

		public static bool IsImplicitSplit(RequestFlags requestFlags, ADUser sourceUser)
		{
			bool flag = requestFlags.HasFlag(RequestFlags.IntraOrg) || requestFlags.HasFlag(RequestFlags.Pull);
			return flag && !requestFlags.HasFlag(RequestFlags.MoveOnlyArchiveMailbox) && !requestFlags.HasFlag(RequestFlags.Split) && sourceUser.UnifiedMailbox != null;
		}

		private static void ObtainComputerNames()
		{
			lock (CommonUtils.localComputerInfoLocker)
			{
				if (string.IsNullOrEmpty(CommonUtils.localComputerName))
				{
					CommonUtils.localComputerName = NativeHelpers.GetLocalComputerFqdn(true);
					int num = CommonUtils.localComputerName.IndexOf('.');
					if (num != -1)
					{
						CommonUtils.localShortComputerName = CommonUtils.localComputerName.Substring(0, num);
					}
					else
					{
						CommonUtils.localShortComputerName = CommonUtils.localComputerName;
					}
				}
			}
		}

		private static string DumpProperty(ProviderPropertyDefinition pdef, object value)
		{
			string text = null;
			if (pdef.IsMultivalued && value is MultiValuedPropertyBase)
			{
				MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)value;
				using (IEnumerator enumerator = ((IEnumerable)multiValuedPropertyBase).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						if (obj != null)
						{
							if (text == null)
							{
								try
								{
									text = obj.ToString();
									continue;
								}
								catch (ArgumentOutOfRangeException ex)
								{
									text = ex.Message;
									continue;
								}
							}
							text = string.Format("{0}; {1}", text, obj.ToString());
						}
					}
					return text;
				}
			}
			text = value.ToString();
			return text;
		}

		internal static IFolder GetFolder(IMailbox mailbox, byte[] folderId)
		{
			ISourceMailbox sourceMailbox = mailbox as ISourceMailbox;
			if (sourceMailbox != null)
			{
				return sourceMailbox.GetFolder(folderId);
			}
			IDestinationMailbox destinationMailbox = mailbox as IDestinationMailbox;
			if (destinationMailbox != null)
			{
				return destinationMailbox.GetFolder(folderId);
			}
			return null;
		}

		internal static NamedPropMapper CreateNamedPropMapper(IMailbox mailbox)
		{
			bool createMappingsIfNeeded = false;
			IDestinationMailbox destinationMailbox = mailbox as IDestinationMailbox;
			if (destinationMailbox != null)
			{
				createMappingsIfNeeded = true;
			}
			return new NamedPropMapper(mailbox, createMappingsIfNeeded);
		}

		internal static void SendGenericWatson(Exception exception, string detailedError, out string callStackHash)
		{
			string stackTrace = CommonUtils.GetStackTrace(exception);
			string failureType = CommonUtils.GetFailureType(exception);
			callStackHash = CommonUtils.ComputeCallStackHash(exception, 5);
			ExWatson.SendGenericWatsonReport("E12", ExWatson.RealApplicationVersion.ToString(), ExWatson.RealAppName, "15.00.1497.012", Assembly.GetExecutingAssembly().GetName().Name, failureType, stackTrace, callStackHash, exception.TargetSite.Name, detailedError);
		}

		internal static string ComputeCallStackHash(Exception exception, int maxDepth = 5)
		{
			int num = 0;
			Exception ex = exception;
			StringBuilder stringBuilder = new StringBuilder();
			while (ex != null && num < maxDepth)
			{
				stringBuilder.AppendFormat("{0}\r\n at Microsoft {1}()\r\n", CommonUtils.GetStackTrace(ex), CommonUtils.GetFailureType(ex));
				num++;
				ex = ex.InnerException;
			}
			return WatsonGenericReport.StringHashFromStackTrace(stringBuilder.ToString());
		}

		internal static string FullFailureMessageWithCallStack(Exception ex, int maxDepth = 5)
		{
			StringBuilder stringBuilder = new StringBuilder();
			CommonUtils.FullFailureMessageWithCallStack(ex, ref stringBuilder, maxDepth);
			return stringBuilder.ToString();
		}

		private static void FullFailureMessageWithCallStack(Exception ex, ref StringBuilder failureMsg, int maxDepth = 5)
		{
			if (ex != null && maxDepth > 0)
			{
				string failureType = CommonUtils.GetFailureType(ex);
				string message = ex.Message;
				string stackTrace = CommonUtils.GetStackTrace(ex);
				if (ex.InnerException == null || maxDepth == 1)
				{
					failureMsg.AppendFormat("{0}: {1}\r\n===\r\n{2}\r\n", failureType, message, stackTrace);
					return;
				}
				failureMsg.AppendFormat("{0}: {1} ---> ", failureType, message);
				CommonUtils.FullFailureMessageWithCallStack(ex.InnerException, ref failureMsg, maxDepth - 1);
				failureMsg.AppendFormat("===\r\n{0}\r\n", stackTrace);
			}
		}

		internal static int CountNewOrUpdatedMessages(EntryIdMap<FolderChangesManifest> folders)
		{
			int num = 0;
			foreach (FolderChangesManifest folderChangesManifest in folders.Values)
			{
				if (folderChangesManifest.ChangedMessages != null)
				{
					foreach (MessageRec messageRec in folderChangesManifest.ChangedMessages)
					{
						if (!messageRec.IsDeleted)
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		internal static void SetBitMask(ref int value, int mask, bool setting)
		{
			if (setting)
			{
				value |= mask;
				return;
			}
			value &= ~mask;
		}

		internal static int GetBitMask(int value, int mask)
		{
			return value & mask;
		}

		public const string MailboxReplicationServiceName = "MSExchangeMailboxReplication";

		public const string MailboxReplicationServiceComponentName = "MailboxReplicationService";

		public const string QueuesDiagnosticName = "Queues";

		public const string MRSQueueDiagnosticName = "Queue";

		public const string MRSResourceDiagnosticName = "Resource";

		public const string ResourcesDiagnosticName = "Resources";

		public const string JobsDiagnosticName = "Jobs";

		public const string BinaryVersionsDiagnosticName = "BinaryVersions";

		public const string MailboxReplicationServiceDisplayName = "Microsoft Exchange Mailbox Replication Service";

		public const string MailboxReplicationServiceDescription = "Microsoft Exchange Mailbox Replication Service";

		public const int DefaultPoisonLimit = 5;

		public const int DefaultHardPoisonLimit = 20;

		public const int DefaultGenericWatsonDepth = 5;

		public const int ChangesNonPagedIncremental = 0;

		public const int HierarchyChangesNonPagedIncremental = 0;

		private static readonly string[] InterestingAssemblies = new string[]
		{
			"MSExchangeMailboxReplication",
			"Microsoft.Exchange.MailboxReplicationService",
			"Microsoft.Exchange.MailboxReplicationService.Common",
			"Microsoft.Exchange.MailboxReplicationService.StorageProvider",
			"Microsoft.Exchange.MailboxReplicationService.MapiProvider",
			"Microsoft.Exchange.MailboxReplicationService.RemoteProvider",
			"Microsoft.Exchange.MailboxReplicationService.ImapProvider",
			"Microsoft.Exchange.MailboxReplicationService.EasProvider",
			"Microsoft.Exchange.MailboxReplicationService.PstProvider",
			"Microsoft.Exchange.MailboxReplicationService.ProxyClient",
			"Microsoft.Exchange.MailboxReplicationService.ProxyService",
			"Microsoft.Exchange.Data.Directory",
			"Microsoft.Exchange.Data.Storage",
			"Microsoft.Exchange.Management"
		};

		public static readonly WorkloadSettings WorkloadSettings = new WorkloadSettings(WorkloadType.MailboxReplicationService, true);

		public static readonly WorkloadSettings WorkloadSettingsHighPriority = new WorkloadSettings(WorkloadType.MailboxReplicationServiceHighPriority, true);

		public static readonly byte[] MessageData = new Guid("6be8b6a6-29c4-4888-8c94-95f5410ff3b8").ToByteArray();

		public static readonly PropValueData[] PropertiesToDelete = new PropValueData[]
		{
			new PropValueData(PropTag.DisablePeruserRead, null),
			new PropValueData(PropTag.OverallAgeLimit, null),
			new PropValueData(PropTag.RetentionAgeLimit, null),
			new PropValueData(PropTag.PfQuotaStyle, null),
			new PropValueData(PropTag.PfOverHardQuotaLimit, null),
			new PropValueData(PropTag.PfStorageQuota, null),
			new PropValueData(PropTag.PfMsgSizeLimit, null),
			new PropValueData(PropTag.PfProxy, null),
			new PropValueData(PropTag.PfProxyRequired, null)
		};

		public static readonly int MinExchangeVersionForSameForestMrsProxyMove = new ServerVersion(Server.Exchange2011MajorVersion, 0, 329, 0).ToInt();

		public static readonly DateTime DefaultLastModificationTime = new DateTime(1601, 1, 1);

		private static readonly ManualResetEvent serviceIsStoppingEvent = new ManualResetEvent(false);

		private static readonly TimeSpan threadAlivePollInterval = TimeSpan.FromSeconds(30.0);

		private static readonly ExEventLog eventLog = new ExEventLog(ExTraceGlobals.MailboxReplicationServiceTracer.Category, "MSExchange Mailbox Replication");

		private static readonly object localComputerInfoLocker = new object();

		private static string localComputerName;

		private static string localShortComputerName;

		private static DumpsterReplicationStatus dumpsterStatus = new DumpsterReplicationStatus();

		private static Lazy<ForestType> forestType = new Lazy<ForestType>(() => CommonUtils.GetForestType(), LazyThreadSafetyMode.PublicationOnly);

		public delegate void EnumPropTagDelegate(ref int ptag);

		public delegate void EnumPropValueDelegate(PropValueData pval);

		public delegate void EnumAdrEntryDelegate(AdrEntryData aed);

		public delegate ExecutionContextWrapper CreateContextDelegate(string operationName, params DataContext[] additionalContexts);

		public delegate void UpdateDuration(string operationName, TimeSpan duration);

		public delegate bool FailureDelegate(Exception ex);
	}
}
