using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Inference;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.Mdb;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal abstract class MdbPeopleBaseModelDataBinder<TModelItem> : IInferenceModelDataBinder<TModelItem> where TModelItem : InferenceBaseModelItem, new()
	{
		protected MdbPeopleBaseModelDataBinder(string modelFAIName, MailboxSession session)
		{
			Util.ThrowOnNullArgument(session, "session");
			this.session = session;
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("MdbPeopleBaseModelDataBinder", ExTraceGlobals.MdbInferenceModelDataBinderTracer, (long)this.GetHashCode());
			this.modelFAIName = modelFAIName;
		}

		public static IList<Type> KnownTypesForSerialization
		{
			get
			{
				return MdbPeopleBaseModelDataBinder<TModelItem>.knownTypesForSerialization;
			}
			set
			{
				MdbPeopleBaseModelDataBinder<TModelItem>.knownTypesForSerialization = value;
			}
		}

		protected abstract Version MinimumSupportedVersion { get; }

		public virtual TModelItem GetModelData()
		{
			TModelItem tmodelItem = default(TModelItem);
			using (UserConfiguration userConfiguration = this.GetUserConfiguration(this.modelFAIName, true))
			{
				using (Stream modelStreamFromUserConfig = this.GetModelStreamFromUserConfig(userConfiguration))
				{
					if (modelStreamFromUserConfig.Length == 0L)
					{
						tmodelItem = Activator.CreateInstance<TModelItem>();
					}
					else
					{
						DataContractSerializer serializer = new DataContractSerializer(typeof(TModelItem), MdbPeopleBaseModelDataBinder<TModelItem>.KnownTypesForSerialization);
						try
						{
							tmodelItem = this.ReadModelData(serializer, modelStreamFromUserConfig);
							if (tmodelItem == null)
							{
								this.diagnosticsSession.TraceError("Deserializing the stream returned a type other than inference model item", new object[0]);
								tmodelItem = Activator.CreateInstance<TModelItem>();
							}
							else if (tmodelItem.Version < this.MinimumSupportedVersion)
							{
								this.diagnosticsSession.TraceDebug<Version>("Returning a new InferenceModelItem since version {0} in the existing model is not supported.", tmodelItem.Version);
								tmodelItem = Activator.CreateInstance<TModelItem>();
							}
						}
						catch (SerializationException arg)
						{
							this.diagnosticsSession.TraceError<SerializationException>("Received serialization exception - {0}", arg);
							tmodelItem = Activator.CreateInstance<TModelItem>();
							using (this.ResetModel(true))
							{
							}
						}
						catch (ArgumentException arg2)
						{
							this.diagnosticsSession.TraceError<ArgumentException>("Received argument exception - {0}", arg2);
							tmodelItem = Activator.CreateInstance<TModelItem>();
							using (this.ResetModel(true))
							{
							}
						}
					}
				}
			}
			return tmodelItem;
		}

		public virtual long SaveModelData(TModelItem modelData)
		{
			long result = -1L;
			using (UserConfiguration userConfiguration = this.GetUserConfiguration(this.modelFAIName, true))
			{
				using (Stream modelStreamFromUserConfig = this.GetModelStreamFromUserConfig(userConfiguration))
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(TModelItem), MdbPeopleBaseModelDataBinder<TModelItem>.KnownTypesForSerialization);
					try
					{
						this.WriteModelData(serializer, modelStreamFromUserConfig, modelData);
						userConfiguration.Save();
						result = modelStreamFromUserConfig.Length;
					}
					catch (SerializationException ex)
					{
						this.diagnosticsSession.TraceError<SerializationException>("Received serialization exception - {0}", ex);
						this.diagnosticsSession.SendInformationalWatsonReport(ex, "Model cannot be serialized");
					}
					catch (MessageSubmissionExceededException ex2)
					{
						this.diagnosticsSession.TraceError<MessageSubmissionExceededException>("Received MessageSubmissionExceeded exception - {0}", ex2);
						this.diagnosticsSession.SendInformationalWatsonReport(ex2, "Model cannot be saved to store (SaveModelData)");
					}
				}
			}
			return result;
		}

		internal UserConfiguration ResetModel(bool deleteOld)
		{
			return XsoUtil.ResetModel(this.modelFAIName, this.GetUserConfigurationType(), this.session, deleteOld, this.diagnosticsSession);
		}

		internal UserConfiguration GetUserConfiguration(string userConfigurationName, bool createIfMissing)
		{
			UserConfiguration userConfiguration = null;
			StoreId defaultFolderId = this.session.GetDefaultFolderId(DefaultFolderType.Inbox);
			bool deleteOld = false;
			Exception arg = null;
			try
			{
				userConfiguration = this.session.UserConfigurationManager.GetFolderConfiguration(userConfigurationName, this.GetUserConfigurationType(), defaultFolderId);
			}
			catch (ObjectNotFoundException ex)
			{
				arg = ex;
			}
			catch (CorruptDataException ex2)
			{
				arg = ex2;
				deleteOld = true;
			}
			if (userConfiguration == null)
			{
				this.diagnosticsSession.TraceDebug<string, Exception>("FAI message '{0}' is missing or corrupt. Exception: {1}", userConfigurationName, arg);
				if (createIfMissing)
				{
					userConfiguration = this.ResetModel(deleteOld);
				}
			}
			return userConfiguration;
		}

		internal abstract Stream GetModelStreamFromUserConfig(UserConfiguration config);

		protected abstract UserConfigurationTypes GetUserConfigurationType();

		protected abstract void WriteModelData(DataContractSerializer serializer, Stream stream, TModelItem modelData);

		protected abstract TModelItem ReadModelData(DataContractSerializer serializer, Stream stream);

		private const string ComponentName = "MdbPeopleBaseModelDataBinder";

		private static IList<Type> knownTypesForSerialization = new List<Type>
		{
			typeof(MdbRecipient),
			typeof(MdbRecipientIdentity),
			typeof(MdbCompositeItemIdentity)
		};

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly string modelFAIName;

		private MailboxSession session;
	}
}
