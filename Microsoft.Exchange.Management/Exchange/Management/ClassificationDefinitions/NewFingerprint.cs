using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Filtering;
using Microsoft.Filtering.Results;
using Microsoft.Mce.Interop.Api;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Cmdlet("New", "Fingerprint", SupportsShouldProcess = true)]
	public sealed class NewFingerprint : NewMultitenancyFixedNameSystemConfigurationObjectTask<TransportRule>
	{
		[Parameter(Mandatory = false, Position = 0)]
		public byte[] FileData
		{
			get
			{
				return (byte[])base.Fields["FileData"];
			}
			set
			{
				base.Fields["FileData"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateLength(1, 256)]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return NewFingerprint.knownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception)) || base.IsKnownException(exception);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.FileData == null || !this.FileData.Any<byte>())
			{
				throw new ErrorFileIsTooSmallForFingerprintException();
			}
			if (this.FileData.Length > 15728640)
			{
				throw new ErrorFileIsTooLargeForFingerprintException(this.FileData.Length, 15728640);
			}
			int textScanLimit = this.GetTextScanLimit();
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(this.FileData))
				{
					string fileName = "Default";
					using (FileFilteringServiceInvokerRequest fileFilteringServiceInvokerRequest = FileFilteringServiceInvokerRequest.CreateInstance(fileName, memoryStream, textScanLimit, base.CurrentOrgContainerId.ObjectGuid.ToString()))
					{
						base.WriteVerbose(Strings.VerboseBeginTextExtraction(this.FileData.Length, textScanLimit));
						FilteringResults unifiedContentResults = UnifiedContentServiceInvoker.GetUnifiedContentResults(fileFilteringServiceInvokerRequest);
						base.WriteVerbose(Strings.VerboseEndTextExtraction(unifiedContentResults.Streams.Count));
						List<StreamIdentity> list = (from streamId in unifiedContentResults.Streams
						where NewFingerprint.IsTextAvailableStream(streamId)
						select streamId).ToList<StreamIdentity>();
						string streamNames = string.Join(";", from streamId in list
						select string.Format("{0}: {1}", streamId.Name, string.Join<StreamType>(",", streamId.Types ?? new StreamType[0])));
						base.WriteVerbose(Strings.VerboseExtractedTextStreams(list.Count, streamNames));
						StreamIdentity streamIdentity = unifiedContentResults.Streams.FirstOrDefault((StreamIdentity streamId) => streamId.ParentId == 0);
						if (!NewFingerprint.IsSupportedFileType(streamIdentity))
						{
							throw new ErrorFileTypeIsUnsupportedException();
						}
						StreamIdentity streamIdentity2 = null;
						if (NewFingerprint.IsTextAvailableStream(streamIdentity))
						{
							streamIdentity2 = streamIdentity;
						}
						else if (list.Count > 0)
						{
							streamIdentity2 = list[0];
						}
						if (streamIdentity2 == null)
						{
							throw new ErrorFileHasNoTextContentException();
						}
						if (RuleAgentResultUtils.HasExceededProcessingLimit(streamIdentity2))
						{
							throw new ErrorExceededTextScanLimitException(textScanLimit);
						}
						if (list.Count > 1 && (streamIdentity.Types == null || streamIdentity.Types.Length != 1 || streamIdentity.Types[0] != 3))
						{
							this.WriteWarning(Strings.WarningMultipleStreamForFingerprint(list.Count, streamIdentity2.Name));
						}
						this.extractedText = streamIdentity2.Content.TextReader.ReadToEnd();
						base.WriteVerbose(Strings.VerboseSelectedTextStream(this.extractedText.Length, this.extractedText));
					}
				}
			}
			catch (FilteringServiceFailureException innerException)
			{
				throw new ErrorFailedExtractTextForFingerprintException(innerException);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			string mceExecutablePath = ClassificationDefinitionUtils.GetMceExecutablePath(true);
			if (mceExecutablePath == null || !File.Exists(mceExecutablePath))
			{
				throw new ErrorCannotLoadFingerprintCreatorException();
			}
			try
			{
				using (ActivationContextActivator.FromInternalManifest(mceExecutablePath, Path.GetDirectoryName(mceExecutablePath)))
				{
					MicrosoftFingerprintCreator microsoftFingerprintCreator = new MicrosoftFingerprintCreator();
					try
					{
						base.WriteVerbose(Strings.VerboseBeginFingerprint);
						byte[] bytes;
						uint base64EncodedFingerprint = microsoftFingerprintCreator.GetBase64EncodedFingerprint(this.extractedText, ref bytes);
						base.WriteVerbose(Strings.VerboseEndFingerprint);
						this.extractedText = string.Empty;
						base.WriteObject(new Fingerprint(Encoding.ASCII.GetString(bytes), base64EncodedFingerprint, this.Description));
					}
					catch (COMException innerException)
					{
						throw new ErrorCannotCreateFingerprintException(innerException);
					}
					catch (ArgumentException innerException2)
					{
						throw new ErrorCannotCreateFingerprintException(innerException2);
					}
				}
			}
			catch (ActivationContextActivatorException innerException3)
			{
				throw new ErrorCannotCreateFingerprintException(innerException3);
			}
		}

		private int GetTextScanLimit()
		{
			return base.ProvisioningCache.TryAddAndGetOrganizationData<int>(CannedProvisioningCacheKeys.TransportRuleAttachmentTextScanLimitCacheKey, base.CurrentOrganizationId, delegate()
			{
				TransportConfigContainer transportConfigContainer = this.ConfigurationSession.FindSingletonConfigurationObject<TransportConfigContainer>();
				return (transportConfigContainer != null) ? ((int)transportConfigContainer.TransportRuleAttachmentTextScanLimit.ToBytes()) : 15728640;
			});
		}

		private static bool IsSupportedFileType(StreamIdentity streamIdentity)
		{
			return streamIdentity != null && (streamIdentity.Types == null || streamIdentity.Types.Length != 1 || !NewFingerprint.unsupportedStreamTypes.Contains(streamIdentity.Types[0]));
		}

		private static bool IsTextAvailableStream(StreamIdentity streamIdentity)
		{
			return streamIdentity != null && streamIdentity.Content != null && streamIdentity.Content.IsTextAvailable && streamIdentity.Content.TextReadStream != null && streamIdentity.Content.TextReadStream.Length > 0L;
		}

		private const int MaxSupportedFileSizeInBytes = 15728640;

		private const int TextScanLimitDefault = 15728640;

		private static readonly StreamType[] unsupportedStreamTypes = new StreamType[]
		{
			33554437,
			134217729,
			134217730,
			134217731,
			134217732,
			134217733,
			134217734,
			134217735,
			134217736,
			134217737,
			134217739,
			134217740,
			134217741
		};

		private static readonly List<Type> knownExceptions = new List<Type>
		{
			typeof(FilteringServiceTimeoutException),
			typeof(ScannerCrashException),
			typeof(ScanQueueTimeoutException),
			typeof(TimeoutException),
			typeof(QueueFullException),
			typeof(ConfigurationException),
			typeof(ServiceUnavailableException),
			typeof(ScanAbortedException),
			typeof(FilteringException),
			typeof(ErrorInvalidFingerprintException),
			typeof(ErrorFileIsTooSmallForFingerprintException),
			typeof(ErrorFileIsTooLargeForFingerprintException),
			typeof(ErrorFileTypeIsUnsupportedException),
			typeof(ErrorFileHasNoTextContentException),
			typeof(ErrorExceededTextScanLimitException),
			typeof(ErrorCannotLoadFingerprintCreatorException),
			typeof(ErrorCannotCreateFingerprintException),
			typeof(ErrorFailedExtractTextForFingerprintException),
			typeof(OutOfMemoryException),
			typeof(IOException)
		};

		private string extractedText;
	}
}
