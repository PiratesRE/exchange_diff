using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	internal abstract class PublishingSessionBase : DisposableBase, IPublishingSession, IDisposeTrackable, IDisposable
	{
		private IUMPromptStorage PromptStore
		{
			get
			{
				if (this.promptStorage == null)
				{
					this.InitializePromptStorage(this.config.OrganizationId);
				}
				return this.promptStorage;
			}
		}

		protected PublishingSessionBase(string userName, ADConfigurationObject config)
		{
			this.userName = userName;
			this.config = config;
		}

		public TimeSpan TestHookKeepOrphanFilesInterval
		{
			get
			{
				return this.keepOrphanFilesInterval;
			}
			set
			{
				this.keepOrphanFilesInterval = value;
			}
		}

		protected ADConfigurationObject ConfigurationObject
		{
			get
			{
				return this.config;
			}
		}

		protected abstract UMDialPlan DialPlan { get; }

		protected string UserName
		{
			get
			{
				return this.userName;
			}
			set
			{
				this.userName = value;
			}
		}

		public virtual void Upload(string source, string destinationName)
		{
			PIIMessage data = PIIMessage.Create(PIIType._User, this.userName);
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, data, "PublishingSessionBase.Upload(_User, {0}, {1}).", new object[]
			{
				source,
				destinationName
			});
			if (source == null)
			{
				throw new ArgumentNullException();
			}
			FileInfo fileInfo = new FileInfo(source);
			if (!fileInfo.Exists)
			{
				throw new SourceFileNotFoundException(fileInfo.FullName);
			}
			try
			{
				this.UpdatePromptChangeKey(Guid.NewGuid());
				string audioBytes = null;
				using (ITempFile tempFile = this.ValidateAndCompressForUpload(fileInfo))
				{
					using (FileStream fileStream = new FileStream(tempFile.FilePath, FileMode.Open, FileAccess.Read))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Uploading from {0} to {1}.", new object[]
						{
							fileInfo.FullName,
							destinationName
						});
						audioBytes = CommonUtil.GetBase64StringFromStream(fileStream);
					}
				}
				this.PromptStore.CreatePrompt(destinationName, audioBytes);
				this.RemoveOrphanEntries();
			}
			catch (Exception ex)
			{
				if (PublishingSessionBase.IsPublishingPointException(ex))
				{
					throw new PublishingPointException(ex.Message, ex);
				}
				throw;
			}
		}

		public virtual void Download(string sourceName, string destination)
		{
			PIIMessage data = PIIMessage.Create(PIIType._User, this.userName);
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, data, "PublishingSessionBase.Download(_User, {0}, {1}).", new object[]
			{
				sourceName,
				destination
			});
			if (sourceName == null || destination == null)
			{
				throw new ArgumentNullException();
			}
			FileInfo fileInfo = new FileInfo(destination);
			if (fileInfo.Exists)
			{
				throw new DestinationAlreadyExistsException(fileInfo.FullName);
			}
			FileInfo fileInfo2 = new FileInfo(sourceName);
			try
			{
				string prompt = this.PromptStore.GetPrompt(sourceName);
				if (fileInfo2.Extension.Equals(".wma", StringComparison.OrdinalIgnoreCase))
				{
					this.WriteToWmaFile(fileInfo.FullName, prompt);
				}
				else
				{
					if (!fileInfo2.Extension.Equals(".wav", StringComparison.OrdinalIgnoreCase))
					{
						throw new SourceFileNotFoundException(sourceName);
					}
					this.WriteToWavFile(fileInfo.FullName, prompt);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Downloaded from {0} to {1}.", new object[]
				{
					sourceName,
					fileInfo.FullName
				});
			}
			catch (Exception ex)
			{
				if (PublishingSessionBase.IsPublishingPointException(ex))
				{
					throw new PublishingPointException(ex.Message, ex);
				}
				throw;
			}
		}

		public virtual ITempWavFile DownloadAsWav(string sourceName)
		{
			PIIMessage.Create(PIIType._User, this.userName);
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "PublishingSessionBase.Download(_User, {0}, into a temp file).", new object[]
			{
				sourceName
			});
			if (sourceName == null)
			{
				throw new ArgumentNullException("sourceName");
			}
			new FileInfo(sourceName);
			ITempWavFile result;
			try
			{
				string prompt = this.PromptStore.GetPrompt(sourceName);
				ITempWavFile tempWavFile = TempFileFactory.CreateTempWavFile();
				this.WriteToWavFile(tempWavFile.FilePath, prompt);
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Downloaded from {0} to {1}.", new object[]
				{
					sourceName,
					tempWavFile.FilePath
				});
				result = tempWavFile;
			}
			catch (Exception ex)
			{
				if (PublishingSessionBase.IsPublishingPointException(ex))
				{
					throw new PublishingPointException(ex.Message, ex);
				}
				throw;
			}
			return result;
		}

		public void DownloadAllAsWma(DirectoryInfo directory)
		{
			if (!directory.Exists)
			{
				throw new DirectoryNotFoundException(directory.FullName);
			}
			try
			{
				string[] promptNames = this.PromptStore.GetPromptNames();
				foreach (string text in promptNames)
				{
					string text2 = Path.Combine(directory.FullName, text);
					text2 += ".wma";
					string prompt = this.PromptStore.GetPrompt(text);
					CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Download from {0} to {1}.", new object[]
					{
						text,
						text2
					});
					this.WriteToWmaFile(text2, prompt);
				}
			}
			catch (Exception ex)
			{
				if (PublishingSessionBase.IsPublishingPointException(ex))
				{
					throw new PublishingPointException(ex.Message, ex);
				}
				throw;
			}
		}

		public void Delete()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "PublishingSessionBase.Delete().", new object[0]);
			try
			{
				this.PromptStore.DeleteAllPrompts();
			}
			catch (Exception ex)
			{
				if (PublishingSessionBase.IsPublishingPointException(ex))
				{
					throw new DeleteContentException(ex.Message, ex);
				}
				throw;
			}
		}

		protected static void AddIfNotEmpty(IDictionary fileList, string fileName)
		{
			if (!string.IsNullOrEmpty(fileName) && !fileList.Contains(fileName))
			{
				fileList.Add(fileName, null);
			}
		}

		protected abstract void UpdatePromptChangeKey(Guid guid);

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "PublishingSessionBase.Dispose() called.", new object[0]);
				if (this.PromptStore != null)
				{
					this.PromptStore.Dispose();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublishingSessionBase>(this);
		}

		protected abstract void AddConfiguredFiles(IDictionary fileList);

		private static bool IsPublishingPointException(Exception e)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, null, "IsPublishingPointException e='{0}'", new object[]
			{
				e
			});
			return e is IOException || e is StoragePermanentException || e is StorageTransientException || e is ADTransientException || e is COMException || e is InvalidWaveFormatException || e is InvalidWmaFormatException || e is EWSUMMailboxAccessException;
		}

		private void WriteToWmaFile(string filePath, string audioBytes)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
			{
				CommonUtil.CopyBase64StringToSteam(audioBytes, fileStream);
			}
		}

		private void WriteToWavFile(string filePath, string audioBytes)
		{
			using (ITempFile tempFile = TempFileFactory.CreateTempWmaFile())
			{
				using (FileStream fileStream = new FileStream(tempFile.FilePath, FileMode.Create, FileAccess.ReadWrite))
				{
					CommonUtil.CopyBase64StringToSteam(audioBytes, fileStream);
					using (PcmWriter pcmWriter = new PcmWriter(filePath, WaveFormat.Pcm8WaveFormat))
					{
						using (WmaReader wmaReader = new WmaReader(tempFile.FilePath))
						{
							byte[] array = new byte[wmaReader.SampleSize * 2];
							int count;
							while ((count = wmaReader.Read(array, array.Length)) > 0)
							{
								pcmWriter.Write(array, count);
							}
						}
					}
				}
			}
		}

		private void InitializePromptStorage(OrganizationId orgId)
		{
			try
			{
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(orgId, null);
				ADUser umdataStorageMailbox = iadrecipientLookup.GetUMDataStorageMailbox();
				this.promptStorage = InterServerMailboxAccessor.GetUMPromptStoreAccessor(umdataStorageMailbox, this.ConfigurationObject.Guid);
			}
			catch (Exception ex)
			{
				if (PublishingSessionBase.IsPublishingPointException(ex))
				{
					throw new PublishingPointException(ex.Message, ex);
				}
				throw;
			}
		}

		private void RemoveOrphanEntries()
		{
			IDictionary dictionary = new Hashtable(StringComparer.OrdinalIgnoreCase);
			this.AddConfiguredFiles(dictionary);
			string[] promptNames = this.PromptStore.GetPromptNames(this.keepOrphanFilesInterval);
			List<string> list = new List<string>(promptNames.Length);
			foreach (string text in promptNames)
			{
				if (!dictionary.Contains(text))
				{
					list.Add(text);
				}
			}
			this.PromptStore.DeletePrompts(list.ToArray());
		}

		private ITempFile ValidateAndCompressForUpload(FileInfo sourceFile)
		{
			string a;
			if ((a = sourceFile.Extension.ToLowerInvariant()) != null)
			{
				ITempFile tempFile;
				if (!(a == ".wav"))
				{
					if (!(a == ".wma"))
					{
						goto IL_41;
					}
					tempFile = this.ValidateAndCompressFromWma(sourceFile);
				}
				else
				{
					tempFile = this.ValidateAndCompressFromPcm(sourceFile);
				}
				FileInfo fileInfo = new FileInfo(tempFile.FilePath);
				if (fileInfo.Length > 507392L)
				{
					throw new UnsupportedCustomGreetingSizeFormatException(5L.ToString(CultureInfo.InvariantCulture));
				}
				return tempFile;
			}
			IL_41:
			throw new InvalidOperationException();
		}

		private ITempFile ValidateAndCompressFromPcm(FileInfo sourceFile)
		{
			bool flag = false;
			ITempFile tempFile = TempFileFactory.CreateTempWmaFile();
			try
			{
				using (PcmReader pcmReader = new PcmReader(sourceFile.FullName))
				{
					using (WmaWriter wmaWriter = new Wma8Writer(tempFile.FilePath, pcmReader.WaveFormat))
					{
						if (this.ValidateWaveFormatForUpload(pcmReader.WaveFormat))
						{
							byte[] array = new byte[wmaWriter.BufferSize];
							int count;
							while ((count = pcmReader.Read(array, array.Length)) > 0)
							{
								wmaWriter.Write(array, count);
							}
							flag = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (PublishingSessionBase.IsPublishingPointException(ex))
				{
					throw new UnsupportedCustomGreetingWaveFormatException(ex);
				}
				throw;
			}
			finally
			{
				if (!flag)
				{
					tempFile.Dispose();
					tempFile = null;
				}
			}
			if (!flag || tempFile == null)
			{
				throw new UnsupportedCustomGreetingWaveFormatException();
			}
			return tempFile;
		}

		private bool ValidateWaveFormatForUpload(WaveFormat format)
		{
			return this.EqualWaveFormatsForUpload(format, WaveFormat.Pcm8WaveFormat) || this.EqualWaveFormatsForUpload(format, WaveFormat.Pcm16WaveFormat);
		}

		private bool EqualWaveFormatsForUpload(WaveFormat lhs, WaveFormat rhs)
		{
			return lhs.SamplesPerSec == rhs.SamplesPerSec && lhs.BitsPerSample == rhs.BitsPerSample && lhs.Channels == rhs.Channels;
		}

		private ITempFile ValidateAndCompressFromWma(FileInfo sourceFile)
		{
			bool flag = false;
			ITempFile tempFile = TempFileFactory.CreateTempWmaFile();
			try
			{
				this.EnsureE12NotPresent(sourceFile);
				using (ITempFile tempFile2 = TempFileFactory.CreateTempWavFile())
				{
					using (WmaReader wmaReader = new WmaReader(sourceFile.FullName))
					{
						using (PcmWriter pcmWriter = new PcmWriter(tempFile2.FilePath, wmaReader.Format))
						{
							byte[] array = new byte[wmaReader.SampleSize * 2];
							int count;
							while ((count = wmaReader.Read(array, array.Length)) > 0)
							{
								pcmWriter.Write(array, count);
							}
						}
						using (PcmReader pcmReader = new PcmReader(tempFile2.FilePath))
						{
							using (WmaWriter wmaWriter = new Wma8Writer(tempFile.FilePath, pcmReader.WaveFormat))
							{
								byte[] array = new byte[wmaWriter.BufferSize];
								int count;
								while ((count = pcmReader.Read(array, array.Length)) > 0)
								{
									wmaWriter.Write(array, count);
								}
							}
						}
						flag = true;
					}
				}
			}
			catch (Exception ex)
			{
				if (PublishingSessionBase.IsPublishingPointException(ex))
				{
					throw new UnsupportedCustomGreetingWmaFormatException(ex);
				}
				throw;
			}
			finally
			{
				if (!flag)
				{
					tempFile.Dispose();
					tempFile = null;
				}
			}
			if (!flag || tempFile == null)
			{
				throw new UnsupportedCustomGreetingWmaFormatException();
			}
			return tempFile;
		}

		private void EnsureE12NotPresent(FileInfo sourceFile)
		{
			ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
			using (IEnumerator<Server> enumerator = adtopologyLookup.GetEnabledUMServersInDialPlan(VersionEnum.E12Legacy, this.DialPlan.Id).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Server server = enumerator.Current;
					throw new UnsupportedCustomGreetingLegacyFormatException(sourceFile.Name);
				}
			}
		}

		public const long MaxGreetingSizeMinutes = 5L;

		private const long MaxGreetingSizeBytes = 507392L;

		private IUMPromptStorage promptStorage;

		private string userName = string.Empty;

		private ADConfigurationObject config;

		private TimeSpan keepOrphanFilesInterval = CommonConstants.PromptProvisioning.KeepOrphanFilesInterval;
	}
}
