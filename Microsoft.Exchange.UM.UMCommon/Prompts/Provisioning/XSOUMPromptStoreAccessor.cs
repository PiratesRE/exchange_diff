using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	internal class XSOUMPromptStoreAccessor : DisposableBase, IUMPromptStorage, IDisposeTrackable, IDisposable
	{
		private string MappingTableName
		{
			get
			{
				return XsoUtil.CombineConfigurationNames("Um.CustomPromptMappingTable", this.configurationObject.ToString("N"));
			}
		}

		private string ConfigurationBaseName
		{
			get
			{
				return XsoUtil.CombineConfigurationNames("Um.CustomPrompts.", this.configurationObject.ToString().Replace("-", string.Empty));
			}
		}

		private UserConfiguration MappingTable
		{
			get
			{
				if (this.lazyMappingTable == null)
				{
					this.lazyMappingTable = this.GetConfigurationObject(this.MappingTableName, UserConfigurationTypes.Dictionary, true);
				}
				return this.lazyMappingTable;
			}
		}

		public XSOUMPromptStoreAccessor(ExchangePrincipal user, Guid configurationObject)
		{
			XSOUMPromptStoreAccessor <>4__this = this;
			this.mailboxPrincipal = user;
			this.disposeMailboxSession = true;
			this.tracer = new DiagnosticHelper(this, ExTraceGlobals.XsoTracer);
			this.ExecuteXSOOperation(delegate
			{
				<>4__this.Initialize(MailboxSessionEstablisher.OpenAsAdmin(<>4__this.mailboxPrincipal, CultureInfo.InvariantCulture, "Client=UM;Action=UMPublishingMailbox-Manage-CustomPrompts"), configurationObject);
			});
		}

		public XSOUMPromptStoreAccessor(MailboxSession session, Guid configurationObject)
		{
			this.tracer = new DiagnosticHelper(this, ExTraceGlobals.XsoTracer);
			this.Initialize(session, configurationObject);
		}

		public void DeleteAllPrompts()
		{
			this.tracer.Trace("XSOUMPromptsStorage : DeleteAllPrompts", new object[0]);
			ICollection<UserConfiguration> publishedContent = this.GetPublishedContent();
			List<string> list = new List<string>(publishedContent.Count);
			foreach (UserConfiguration userConfiguration in publishedContent)
			{
				using (userConfiguration)
				{
					list.Add(userConfiguration.ConfigurationName);
				}
			}
			OperationResult operationResult = this.lazyPublishingSessionMailbox.UserConfigurationManager.DeleteMailboxConfigurations(list.ToArray());
			if (OperationResult.Succeeded != operationResult)
			{
				throw new DeleteContentException(operationResult.ToString());
			}
			this.lazyPublishingSessionMailbox.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
			{
				this.MappingTableName
			});
		}

		public void DeletePrompts(string[] prompts)
		{
			this.tracer.Trace("XSOUMPromptsStorage : DeletePrompts", new object[0]);
			this.ExecuteXSOOperation(delegate
			{
				ValidateArgument.NotNull(prompts, "Prompts");
				List<string> list = new List<string>(prompts.Length);
				foreach (string fileName in prompts)
				{
					string text = this.ConfigurationNameFromFileName(fileName, false);
					if (!string.IsNullOrEmpty(text))
					{
						list.Add(text);
					}
				}
				this.lazyPublishingSessionMailbox.UserConfigurationManager.DeleteMailboxConfigurations(list.ToArray());
			});
		}

		public string[] GetPromptNames()
		{
			return this.GetPromptNames(TimeSpan.Zero);
		}

		public string[] GetPromptNames(TimeSpan timeSinceLastModified)
		{
			this.tracer.Trace("XSOUMPromptsStorage : GetPromptNames, for Guid {0}", new object[]
			{
				this.configurationObject
			});
			List<string> prompts = new List<string>();
			this.ExecuteXSOOperation(delegate
			{
				ICollection<UserConfiguration> publishedContent = this.GetPublishedContent();
				List<string> list = new List<string>();
				foreach (UserConfiguration userConfiguration in publishedContent)
				{
					using (userConfiguration)
					{
						string text = this.FileNameFromConfigurationName(userConfiguration.ConfigurationName);
						if (!string.IsNullOrEmpty(text))
						{
							if (ExDateTime.UtcNow.Subtract(userConfiguration.LastModifiedTime) >= timeSinceLastModified)
							{
								prompts.Add(text);
							}
						}
						else
						{
							list.Add(userConfiguration.ConfigurationName);
						}
					}
				}
				this.lazyPublishingSessionMailbox.UserConfigurationManager.DeleteMailboxConfigurations(list.ToArray());
			});
			return prompts.ToArray();
		}

		public void CreatePrompt(string promptName, string audioBytes)
		{
			this.ExecuteXSOOperation(delegate
			{
				ValidateArgument.NotNullOrEmpty(promptName, "promptName");
				ExAssert.RetailAssert(audioBytes != null && audioBytes.Length > 0, "AudioData passed cannot be null or empty");
				this.tracer.Trace("XSOUMPromptsStorage : CreatePrompt, promptName {0}", new object[]
				{
					promptName
				});
				string configName = this.ConfigurationNameFromFileName(promptName, true);
				using (UserConfiguration userConfiguration = this.GetConfigurationObject(configName, UserConfigurationTypes.Stream, true))
				{
					using (Stream stream = userConfiguration.GetStream())
					{
						stream.SetLength(0L);
						CommonUtil.CopyBase64StringToSteam(audioBytes, stream);
						userConfiguration.Save();
					}
				}
			});
		}

		public string GetPrompt(string promptName)
		{
			ValidateArgument.NotNullOrEmpty(promptName, "promptName");
			this.tracer.Trace("XSOUMPromptsStorage : GetPrompt, promptName {0}", new object[]
			{
				promptName
			});
			string promptBytes = null;
			this.ExecuteXSOOperation(delegate
			{
				string text = this.ConfigurationNameFromFileName(promptName, false);
				if (string.IsNullOrEmpty(text))
				{
					throw new SourceFileNotFoundException(promptName);
				}
				using (UserConfiguration userConfiguration = this.GetConfigurationObject(text, UserConfigurationTypes.Stream, false))
				{
					if (userConfiguration == null)
					{
						throw new SourceFileNotFoundException(promptName);
					}
					using (Stream stream = userConfiguration.GetStream())
					{
						promptBytes = CommonUtil.GetBase64StringFromStream(stream);
					}
				}
			});
			return promptBytes;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.tracer.Trace("XSOUMPromptsStorage : InternalDispose", new object[0]);
				if (this.lazyMappingTable != null)
				{
					this.lazyMappingTable.Dispose();
				}
				if (this.lazyPublishingSessionMailbox != null && this.disposeMailboxSession)
				{
					this.lazyPublishingSessionMailbox.Dispose();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<XSOUMPromptStoreAccessor>(this);
		}

		private void Initialize(MailboxSession session, Guid configurationObject)
		{
			ExAssert.RetailAssert(configurationObject != Guid.Empty, "ConfigurationObject Guid cannot be empty");
			ExAssert.RetailAssert(session != null, "MailboxSession cannot be null");
			this.configurationObject = configurationObject;
			this.lazyPublishingSessionMailbox = session;
			this.tracer.Trace("XSOUMPromptsStorage for configObject {0}, called from WebServices : {1}", new object[]
			{
				configurationObject,
				!this.disposeMailboxSession
			});
		}

		private string FileNameFromConfigurationName(string configName)
		{
			string result = null;
			IDictionary dictionary = this.MappingTable.GetDictionary();
			if (dictionary.Contains(configName))
			{
				result = (dictionary[configName] as string);
			}
			return result;
		}

		private string ConfigurationNameFromFileName(string fileName, bool create)
		{
			string text = null;
			fileName = fileName.ToLowerInvariant();
			IDictionary dictionary = this.MappingTable.GetDictionary();
			if (dictionary.Contains(fileName))
			{
				text = (dictionary[fileName] as string);
			}
			if (create && string.IsNullOrEmpty(text))
			{
				string c = Guid.NewGuid().ToString().Replace("-", string.Empty);
				text = XsoUtil.CombineConfigurationNames(this.ConfigurationBaseName, c);
				dictionary[fileName] = text;
				dictionary[text] = fileName;
				this.MappingTable.Save();
				this.lazyMappingTable.Dispose();
				this.lazyMappingTable = null;
			}
			return text;
		}

		private ICollection<UserConfiguration> GetPublishedContent()
		{
			return this.lazyPublishingSessionMailbox.UserConfigurationManager.FindMailboxConfigurations(this.ConfigurationBaseName, UserConfigurationSearchFlags.Prefix);
		}

		private UserConfiguration GetConfigurationObject(string configName, UserConfigurationTypes configType, bool create)
		{
			UserConfiguration userConfiguration = null;
			try
			{
				userConfiguration = this.lazyPublishingSessionMailbox.UserConfigurationManager.GetMailboxConfiguration(configName, configType);
			}
			catch (ObjectNotFoundException)
			{
			}
			catch (CorruptDataException)
			{
				this.lazyPublishingSessionMailbox.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
				{
					configName
				});
			}
			catch (InvalidOperationException)
			{
				this.lazyPublishingSessionMailbox.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
				{
					configName
				});
			}
			if (userConfiguration == null && create)
			{
				this.tracer.Trace("Creating config object {0}", new object[]
				{
					configName
				});
				userConfiguration = this.lazyPublishingSessionMailbox.UserConfigurationManager.CreateMailboxConfiguration(configName, configType);
			}
			return userConfiguration;
		}

		private void ExecuteXSOOperation(Action function)
		{
			try
			{
				function();
			}
			catch (Exception ex)
			{
				this.tracer.Trace("XSOUMPromptsStorage ExecuteXSOOperation, exception  = {0}", new object[]
				{
					ex
				});
				if (this.mailboxPrincipal != null)
				{
					XsoUtil.LogMailboxConnectionFailureException(ex, this.mailboxPrincipal);
				}
				if (ex is StoragePermanentException || ex is StorageTransientException)
				{
					throw new PublishingPointException(ex.Message, ex);
				}
				throw;
			}
		}

		private readonly bool disposeMailboxSession;

		private Guid configurationObject;

		private ExchangePrincipal mailboxPrincipal;

		private MailboxSession lazyPublishingSessionMailbox;

		private UserConfiguration lazyMappingTable;

		private DiagnosticHelper tracer;
	}
}
