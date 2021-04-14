using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Compliance.Serialization.Formatters;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.TextProcessing.Boomerang;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Mapi;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class PipelineContext : DisposableBase, IUMCreateMessage
	{
		internal PipelineContext()
		{
		}

		internal PipelineContext(SubmissionHelper helper)
		{
			bool flag = false;
			try
			{
				this.helper = helper;
				this.cultureInfo = new CultureInfo(helper.CultureInfo);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		public MessageItem MessageToSubmit
		{
			get
			{
				return this.messageToSubmit;
			}
			protected set
			{
				this.messageToSubmit = value;
			}
		}

		public string MessageID
		{
			get
			{
				return this.messageID;
			}
			protected set
			{
				this.messageID = value;
			}
		}

		internal abstract Pipeline Pipeline { get; }

		internal PhoneNumber CallerId
		{
			get
			{
				return this.helper.CallerId;
			}
		}

		internal Guid TenantGuid
		{
			get
			{
				return this.helper.TenantGuid;
			}
		}

		internal int ProcessedCount
		{
			get
			{
				return this.processedCount;
			}
		}

		internal ExDateTime SentTime
		{
			get
			{
				return this.sentTime;
			}
			set
			{
				this.sentTime = value;
			}
		}

		internal CultureInfo CultureInfo
		{
			get
			{
				return this.cultureInfo;
			}
		}

		protected internal string HeaderFileName
		{
			get
			{
				if (string.IsNullOrEmpty(this.headerFileName))
				{
					Guid guid = Guid.NewGuid();
					this.headerFileName = Path.Combine(Utils.VoiceMailFilePath, guid.ToString() + ".txt");
				}
				return this.headerFileName;
			}
			protected set
			{
				this.headerFileName = value;
			}
		}

		protected internal string CallerAddress
		{
			get
			{
				return this.helper.CallerAddress;
			}
			protected set
			{
				this.helper.CallerAddress = value;
			}
		}

		protected internal string CallerIdDisplayName
		{
			get
			{
				return this.helper.CallerIdDisplayName;
			}
			protected set
			{
				this.helper.CallerIdDisplayName = value;
			}
		}

		protected internal string MessageType
		{
			internal get
			{
				return this.messageType;
			}
			set
			{
				this.messageType = value;
			}
		}

		public virtual void PrepareUnProtectedMessage()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "PipelineContext:PrepareUnProtectedMessage.", new object[0]);
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				this.messageToSubmit = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties);
				disposeGuard.Add<MessageItem>(this.messageToSubmit);
				this.SetMessageProperties();
				disposeGuard.Success();
			}
		}

		public virtual void PrepareProtectedMessage()
		{
			throw new InvalidOperationException();
		}

		public virtual void PrepareNDRForFailureToGenerateProtectedMessage()
		{
			throw new InvalidOperationException();
		}

		public virtual PipelineDispatcher.WIThrottleData GetThrottlingData()
		{
			return new PipelineDispatcher.WIThrottleData
			{
				Key = this.GetMailboxServerId(),
				RecipientId = this.GetRecipientIdForThrottling(),
				WorkItemType = PipelineDispatcher.ThrottledWorkItemType.NonCDRWorkItem
			};
		}

		public virtual void PostCompletion()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "PipelineContext - Deleting header file '{0}'", new object[]
			{
				this.headerFileName
			});
			Util.TryDeleteFile(this.headerFileName);
		}

		internal static PipelineContext FromHeaderFile(string headerFile)
		{
			PipelineContext pipelineContext = null;
			PipelineContext result;
			try
			{
				ContactInfo contactInfo = null;
				string text = null;
				int num = 0;
				ExDateTime exDateTime = default(ExDateTime);
				string text2 = null;
				SubmissionHelper submissionHelper = new SubmissionHelper();
				using (StreamReader streamReader = File.OpenText(headerFile))
				{
					string text3;
					while ((text3 = streamReader.ReadLine()) != null)
					{
						string[] array = text3.Split(" : ".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
						if (array != null && array.Length == 2)
						{
							string key;
							if ((key = array[0]) != null)
							{
								if (<PrivateImplementationDetails>{52CC4AA6-9890-4FF8-93E5-6095807AC0AF}.$$method0x600147c-1 == null)
								{
									<PrivateImplementationDetails>{52CC4AA6-9890-4FF8-93E5-6095807AC0AF}.$$method0x600147c-1 = new Dictionary<string, int>(15)
									{
										{
											"CallId",
											0
										},
										{
											"CallerId",
											1
										},
										{
											"SenderAddress",
											2
										},
										{
											"RecipientName",
											3
										},
										{
											"RecipientObjectGuid",
											4
										},
										{
											"CultureInfo",
											5
										},
										{
											"CallerNAme",
											6
										},
										{
											"CallerIdDisplayName",
											7
										},
										{
											"CallerAddress",
											8
										},
										{
											"MessageType",
											9
										},
										{
											"ProcessedCount",
											10
										},
										{
											"ContactInfo",
											11
										},
										{
											"MessageID",
											12
										},
										{
											"SentTime",
											13
										},
										{
											"TenantGuid",
											14
										}
									};
								}
								int num2;
								if (<PrivateImplementationDetails>{52CC4AA6-9890-4FF8-93E5-6095807AC0AF}.$$method0x600147c-1.TryGetValue(key, out num2))
								{
									switch (num2)
									{
									case 0:
										submissionHelper.CallId = array[1];
										continue;
									case 1:
										submissionHelper.CallerId = PhoneNumber.Parse(array[1]);
										continue;
									case 2:
									{
										string text4 = array[1];
										continue;
									}
									case 3:
										submissionHelper.RecipientName = array[1];
										continue;
									case 4:
										submissionHelper.RecipientObjectGuid = new Guid(array[1]);
										continue;
									case 5:
										submissionHelper.CultureInfo = array[1];
										continue;
									case 6:
										submissionHelper.CallerName = array[1];
										continue;
									case 7:
										submissionHelper.CallerIdDisplayName = array[1];
										continue;
									case 8:
										submissionHelper.CallerAddress = array[1];
										continue;
									case 9:
										text = array[1];
										continue;
									case 10:
										num = Convert.ToInt32(array[1], CultureInfo.InvariantCulture) + 1;
										continue;
									case 11:
									{
										Exception ex = null;
										try
										{
											try
											{
												byte[] buffer = Convert.FromBase64String(array[1]);
												using (MemoryStream memoryStream = new MemoryStream(buffer))
												{
													contactInfo = (ContactInfo)TypedBinaryFormatter.DeserializeObject(memoryStream, PipelineContext.contactInfoDeserializationAllowList, null, true);
												}
											}
											catch (ArgumentNullException ex2)
											{
												ex = ex2;
											}
											catch (SerializationException ex3)
											{
												ex = ex3;
											}
											catch (Exception ex4)
											{
												ex = ex4;
											}
											continue;
										}
										finally
										{
											if (ex != null)
											{
												CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Failed to get contactInfo from header file {0} with Error={1}", new object[]
												{
													headerFile,
													ex
												});
											}
										}
										break;
									}
									case 12:
										break;
									case 13:
									{
										DateTime dateTime = Convert.ToDateTime(array[1], CultureInfo.InvariantCulture);
										exDateTime = new ExDateTime(ExTimeZone.CurrentTimeZone, dateTime);
										continue;
									}
									case 14:
										submissionHelper.TenantGuid = new Guid(array[1]);
										continue;
									default:
										goto IL_2FF;
									}
									text2 = array[1];
									continue;
								}
							}
							IL_2FF:
							submissionHelper.CustomHeaders[array[0]] = array[1];
						}
					}
				}
				string key2;
				if ((key2 = text) != null)
				{
					if (<PrivateImplementationDetails>{52CC4AA6-9890-4FF8-93E5-6095807AC0AF}.$$method0x600147c-2 == null)
					{
						<PrivateImplementationDetails>{52CC4AA6-9890-4FF8-93E5-6095807AC0AF}.$$method0x600147c-2 = new Dictionary<string, int>(10)
						{
							{
								"SMTPVoiceMail",
								0
							},
							{
								"Fax",
								1
							},
							{
								"MissedCall",
								2
							},
							{
								"IncomingCallLog",
								3
							},
							{
								"OutgoingCallLog",
								4
							},
							{
								"OCSNotification",
								5
							},
							{
								"XSOVoiceMail",
								6
							},
							{
								"PartnerTranscriptionRequest",
								7
							},
							{
								"CDR",
								8
							},
							{
								"HealthCheck",
								9
							}
						};
					}
					int num3;
					if (<PrivateImplementationDetails>{52CC4AA6-9890-4FF8-93E5-6095807AC0AF}.$$method0x600147c-2.TryGetValue(key2, out num3))
					{
						switch (num3)
						{
						case 0:
							if (num < PipelineWorkItem.ProcessedCountMax - 1)
							{
								pipelineContext = new VoiceMessagePipelineContext(submissionHelper);
							}
							else
							{
								pipelineContext = new MissedCallPipelineContext(submissionHelper);
							}
							break;
						case 1:
							pipelineContext = new FaxPipelineContext(submissionHelper);
							break;
						case 2:
							pipelineContext = new MissedCallPipelineContext(submissionHelper);
							break;
						case 3:
							pipelineContext = new IncomingCallLogPipelineContext(submissionHelper);
							break;
						case 4:
							pipelineContext = new OutgoingCallLogPipelineContext(submissionHelper);
							break;
						case 5:
							pipelineContext = OCSPipelineContext.Deserialize((string)submissionHelper.CustomHeaders["OCSNotificationData"]);
							text2 = pipelineContext.messageID;
							exDateTime = pipelineContext.sentTime;
							break;
						case 6:
							pipelineContext = new XSOVoiceMessagePipelineContext(submissionHelper);
							break;
						case 7:
							pipelineContext = new PartnerTranscriptionRequestPipelineContext(submissionHelper);
							break;
						case 8:
							pipelineContext = CDRPipelineContext.Deserialize((string)submissionHelper.CustomHeaders["CDRData"]);
							break;
						case 9:
							pipelineContext = new HealthCheckPipelineContext(Path.GetFileNameWithoutExtension(headerFile));
							break;
						default:
							goto IL_4DB;
						}
						if (text2 == null)
						{
							text2 = Guid.NewGuid().ToString();
							exDateTime = ExDateTime.Now;
						}
						pipelineContext.HeaderFileName = headerFile;
						pipelineContext.processedCount = num;
						if (contactInfo != null)
						{
							IUMResolveCaller iumresolveCaller = pipelineContext as IUMResolveCaller;
							if (iumresolveCaller != null)
							{
								iumresolveCaller.ContactInfo = contactInfo;
							}
						}
						pipelineContext.sentTime = exDateTime;
						pipelineContext.messageID = text2;
						pipelineContext.WriteHeaderFile(headerFile);
						return pipelineContext;
					}
				}
				IL_4DB:
				throw new HeaderFileArgumentInvalidException(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
				{
					"MessageType",
					text
				}));
			}
			catch (IOException ex5)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Failed to parse the header file {0} because its not closed by thread creating the file.  Error={1}", new object[]
				{
					headerFile,
					ex5
				});
				if (pipelineContext != null)
				{
					pipelineContext.Dispose();
					pipelineContext = null;
				}
				result = null;
			}
			catch (InvalidObjectGuidException ex6)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.VoiceMailTracer, 0, "Couldn't find the recipient for this message. Error={0}", new object[]
				{
					ex6
				});
				if (pipelineContext != null)
				{
					pipelineContext.Dispose();
					pipelineContext = null;
				}
				throw;
			}
			catch (InvalidTenantGuidException ex7)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.VoiceMailTracer, 0, "Couldn't find the tenant for this message. Error={0}", new object[]
				{
					ex7
				});
				if (pipelineContext != null)
				{
					pipelineContext.Dispose();
					pipelineContext = null;
				}
				throw;
			}
			catch (NonUniqueRecipientException ex8)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.VoiceMailTracer, 0, "Multiple objects found for the recipient. Error={0}", new object[]
				{
					ex8
				});
				if (pipelineContext != null)
				{
					pipelineContext.Dispose();
					pipelineContext = null;
				}
				throw;
			}
			return result;
		}

		internal abstract void WriteCustomHeaderFields(StreamWriter headerStream);

		public abstract string GetMailboxServerId();

		public abstract string GetRecipientIdForThrottling();

		internal virtual void SaveMessage()
		{
			this.WriteHeaderFile(this.HeaderFileName);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "PipelineContext.Dispose() called", new object[0]);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PipelineContext>(this);
		}

		protected virtual void SetMessageProperties()
		{
			IUMResolveCaller iumresolveCaller = this as IUMResolveCaller;
			if (iumresolveCaller != null)
			{
				ExAssert.RetailAssert(iumresolveCaller.ContactInfo != null, "ResolveCallerStage should always set the ContactInfo.");
				IUMCAMessage iumcamessage = (IUMCAMessage)this;
				UMSubscriber umsubscriber = iumcamessage.CAMessageRecipient as UMSubscriber;
				UMDialPlan dialPlan = (umsubscriber != null) ? umsubscriber.DialPlan : null;
				PhoneNumber pstnCallbackTelephoneNumber = this.CallerId.GetPstnCallbackTelephoneNumber(iumresolveCaller.ContactInfo, dialPlan);
				this.messageToSubmit.From = iumresolveCaller.ContactInfo.CreateParticipant(pstnCallbackTelephoneNumber, this.CultureInfo);
				XsoUtil.SetVoiceMessageSenderProperties(this.messageToSubmit, iumresolveCaller.ContactInfo, dialPlan, this.CallerId);
				this.messageToSubmit.InternetMessageId = BoomerangProvider.Instance.FormatInternetMessageId(this.MessageID, Utils.GetHostFqdn());
				this.messageToSubmit[ItemSchema.SentTime] = this.SentTime;
			}
			this.messageToSubmit.AutoResponseSuppress = AutoResponseSuppress.All;
			this.messageToSubmit[MessageItemSchema.CallId] = this.helper.CallId;
			IUMCAMessage iumcamessage2 = this as IUMCAMessage;
			if (iumcamessage2 != null)
			{
				this.MessageToSubmit.Recipients.Add(new Participant(iumcamessage2.CAMessageRecipient.ADRecipient));
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(iumcamessage2.CAMessageRecipient.ADRecipient.OrganizationId);
				this.MessageToSubmit.Sender = new Participant(iadsystemConfigurationLookup.GetMicrosoftExchangeRecipient());
			}
		}

		protected void WriteHeaderFile(string headerFileName)
		{
			using (FileStream fileStream = File.Open(headerFileName, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using (StreamWriter streamWriter = new StreamWriter(fileStream))
				{
					if (this.MessageType != null)
					{
						streamWriter.WriteLine("MessageType : " + this.MessageType);
					}
					streamWriter.WriteLine("ProcessedCount : " + this.processedCount.ToString(CultureInfo.InvariantCulture));
					if (this.messageID != null)
					{
						streamWriter.WriteLine("MessageID : " + this.messageID);
					}
					if (this.sentTime.Year != 1)
					{
						streamWriter.WriteLine("SentTime : " + this.sentTime.ToString(CultureInfo.InvariantCulture));
					}
					this.WriteCommonHeaderFields(streamWriter);
					this.WriteCustomHeaderFields(streamWriter);
				}
			}
		}

		protected virtual void WriteCommonHeaderFields(StreamWriter headerStream)
		{
			if (!this.CallerId.IsEmpty)
			{
				headerStream.WriteLine("CallerId : " + this.CallerId.ToDial);
			}
			if (this.helper.RecipientName != null)
			{
				headerStream.WriteLine("RecipientName : " + this.helper.RecipientName);
			}
			if (this.helper.RecipientObjectGuid != Guid.Empty)
			{
				headerStream.WriteLine("RecipientObjectGuid : " + this.helper.RecipientObjectGuid.ToString());
			}
			if (this.helper.CallerName != null)
			{
				headerStream.WriteLine("CallerNAme : " + this.helper.CallerName);
			}
			if (!string.IsNullOrEmpty(this.helper.CallerIdDisplayName))
			{
				headerStream.WriteLine("CallerIdDisplayName : " + this.helper.CallerIdDisplayName);
			}
			if (this.CallerAddress != null)
			{
				headerStream.WriteLine("CallerAddress : " + this.CallerAddress);
			}
			if (this.helper.CultureInfo != null)
			{
				headerStream.WriteLine("CultureInfo : " + this.helper.CultureInfo);
			}
			if (this.helper.CallId != null)
			{
				headerStream.WriteLine("CallId : " + this.helper.CallId);
			}
			IUMResolveCaller iumresolveCaller = this as IUMResolveCaller;
			if (iumresolveCaller != null && iumresolveCaller.ContactInfo != null)
			{
				headerStream.WriteLine("ContactInfo : " + CommonUtil.Base64Serialize(iumresolveCaller.ContactInfo));
			}
			headerStream.WriteLine("TenantGuid : " + this.helper.TenantGuid.ToString());
		}

		protected UMRecipient CreateRecipientFromObjectGuid(Guid objectGuid, Guid tenantGuid)
		{
			ADRecipient adrecipient = this.CreateADRecipientFromObjectGuid(objectGuid, tenantGuid);
			return UMRecipient.Factory.FromADRecipient<UMRecipient>(adrecipient);
		}

		protected ADRecipient CreateADRecipientFromObjectGuid(Guid objectGuid, Guid tenantGuid)
		{
			if (objectGuid == Guid.Empty)
			{
				throw new HeaderFileArgumentInvalidException("ObjectGuid is empty");
			}
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromTenantGuid(tenantGuid);
			ADRecipient adrecipient = iadrecipientLookup.LookupByObjectId(new ADObjectId(objectGuid));
			if (adrecipient == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Could not find recipient {0}", new object[]
				{
					objectGuid.ToString()
				});
				throw new InvalidObjectGuidException(objectGuid.ToString());
			}
			return adrecipient;
		}

		protected UMDialPlan InitializeCallerIdAndTryGetDialPlan(UMRecipient recipient)
		{
			UMDialPlan umdialPlan = null;
			if (this.CallerId.UriType == UMUriType.E164 && recipient.ADRecipient.UMRecipientDialPlanId != null)
			{
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(recipient.ADRecipient);
				umdialPlan = iadsystemConfigurationLookup.GetDialPlanFromId(recipient.ADRecipient.UMRecipientDialPlanId);
				if (umdialPlan != null && umdialPlan.CountryOrRegionCode != null)
				{
					this.helper.CallerId = this.helper.CallerId.Clone(umdialPlan);
				}
			}
			return umdialPlan;
		}

		protected string GetMailboxServerIdHelper()
		{
			IUMCAMessage iumcamessage = this as IUMCAMessage;
			if (iumcamessage != null)
			{
				UMMailboxRecipient ummailboxRecipient = iumcamessage.CAMessageRecipient as UMMailboxRecipient;
				if (ummailboxRecipient != null)
				{
					return ummailboxRecipient.ADUser.ServerLegacyDN;
				}
			}
			return "af360a7e-e6d4-494a-ac69-6ae14896d16b";
		}

		protected string GetRecipientIdHelper()
		{
			IUMCAMessage iumcamessage = this as IUMCAMessage;
			if (iumcamessage != null)
			{
				UMMailboxRecipient ummailboxRecipient = iumcamessage.CAMessageRecipient as UMMailboxRecipient;
				if (ummailboxRecipient != null)
				{
					return ummailboxRecipient.ADUser.DistinguishedName;
				}
			}
			return "455e5330-ce1f-48d1-b6b1-2e318d2ff2c4";
		}

		private MessageItem messageToSubmit;

		private SubmissionHelper helper;

		private string messageType;

		private CultureInfo cultureInfo;

		private string headerFileName;

		private int processedCount;

		private string messageID;

		private ExDateTime sentTime;

		private static Type[] contactInfoDeserializationAllowList = new Type[]
		{
			typeof(Version),
			typeof(Guid),
			typeof(PropTag),
			typeof(ContactInfo),
			typeof(ADContactInfo),
			typeof(FoundByType),
			typeof(ADUser),
			typeof(ADPropertyBag),
			typeof(ValidationError),
			typeof(ADPropertyDefinition),
			typeof(ADObjectId),
			typeof(ExchangeObjectVersion),
			typeof(ExchangeBuild),
			typeof(MultiValuedProperty<string>),
			typeof(LocalizedString),
			typeof(ProxyAddressCollection),
			typeof(SmtpAddress),
			typeof(RecipientDisplayType),
			typeof(RecipientTypeDetails),
			typeof(ElcMailboxFlags),
			typeof(UserAccountControlFlags),
			typeof(ObjectState),
			typeof(DirectoryBackendType),
			typeof(MServPropertyDefinition),
			typeof(MbxPropertyDefinition),
			typeof(OrganizationId),
			typeof(PartitionId),
			typeof(SmtpProxyAddress),
			typeof(SmtpProxyAddressPrefix),
			typeof(ByteQuantifiedSize),
			typeof(Unlimited<ByteQuantifiedSize>),
			typeof(List<ValidationError>),
			typeof(ADMultiValuedProperty<TextMessagingStateBase>),
			typeof(ADMultiValuedProperty<ADObjectId>),
			typeof(StoreObjectId),
			typeof(StoreObjectType),
			typeof(SimpleContactInfoBase),
			typeof(MultipleResolvedContactInfo),
			typeof(CallerNameDisplayContactInfo),
			typeof(PersonalContactInfo),
			typeof(DefaultContactInfo),
			typeof(UMDialPlan),
			typeof(UMEnabledFlags),
			Type.GetType("Microsoft.Exchange.Data.ByteQuantifiedSize+QuantifierProvider, Microsoft.Exchange.Data"),
			Type.GetType("System.UnitySerializationHolder, mscorlib"),
			Type.GetType("Microsoft.Exchange.Data.ByteQuantifiedSize+Quantifier,Microsoft.Exchange.Data"),
			Type.GetType("Microsoft.Exchange.Data.PropertyBag+ValuePair, Microsoft.Exchange.Data"),
			Type.GetType("System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]"),
			typeof(DialByNamePrimaryEnum),
			typeof(DialByNameSecondaryEnum),
			typeof(AudioCodecEnum),
			typeof(UMUriType),
			typeof(UMSubscriberType),
			typeof(UMGlobalCallRoutingScheme),
			typeof(UMVoIPSecurityType),
			typeof(SystemFlagsEnum),
			typeof(EumProxyAddress),
			typeof(EumProxyAddressPrefix)
		};
	}
}
