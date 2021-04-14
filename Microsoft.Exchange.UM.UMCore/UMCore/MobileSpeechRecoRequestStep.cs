using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class MobileSpeechRecoRequestStep : IMobileSpeechRecoRequestStep
	{
		private protected Guid RequestId { protected get; private set; }

		private protected MdbefPropertyCollection Args { protected get; private set; }

		private string Description { get; set; }

		private object Token { get; set; }

		private MobileRecoRequestStepAsyncCompletedDelegate Callback { get; set; }

		private bool RunGuardAfterExecution { get; set; }

		public MobileSpeechRecoRequestStep(Guid requestId, MdbefPropertyCollection args, string description)
		{
			ValidateArgument.NotNull(args, "args");
			ValidateArgument.NotNullOrEmpty(description, "description");
			MobileSpeechRecoTracer.TraceDebug(this, requestId, "Entering MobileSpeechRecoRequestStep constructor", new object[0]);
			this.RequestId = requestId;
			this.Args = args;
			this.Description = description;
		}

		public static IMobileSpeechRecoRequestStep Create(byte[] inBlob)
		{
			ValidateArgument.NotNull(inBlob, "inBlob");
			MobileSpeechRecoTracer.TraceDebug(null, Guid.Empty, "Entering MobileSpeechRecoRequestStep.Create", new object[0]);
			MdbefPropertyCollection args = MdbefPropertyCollection.Create(inBlob, 0, inBlob.Length);
			IMobileSpeechRecoRequestStep result = null;
			object obj = MobileSpeechRecoRequestStep.ExtractArg(args, 2415919107U, "stepId");
			MobileSpeechRecoRequestStepId mobileSpeechRecoRequestStepId = (MobileSpeechRecoRequestStepId)obj;
			if (!EnumValidator.IsValidValue<MobileSpeechRecoRequestStepId>(mobileSpeechRecoRequestStepId))
			{
				MobileSpeechRecoTracer.TraceError(null, Guid.Empty, "MobileSpeechRecoRequestStep.Create -  Invalid step id='{0}'", new object[]
				{
					obj
				});
				throw new ArgumentOutOfRangeException("stepId", obj, "Invalid value");
			}
			obj = MobileSpeechRecoRequestStep.ExtractArg(args, 2415984712U, "requestId");
			Guid guid = (Guid)obj;
			if (guid == Guid.Empty)
			{
				MobileSpeechRecoTracer.TraceError(null, Guid.Empty, "MobileSpeechRecoRequestStep.Create -  Invalid request id='{0}' (empty)", new object[]
				{
					obj
				});
				throw new ArgumentOutOfRangeException("requestId", obj, "Request id is empty");
			}
			switch (mobileSpeechRecoRequestStepId)
			{
			case MobileSpeechRecoRequestStepId.AddRecoRequest:
				result = new AddRecoRequestStep(guid, args);
				break;
			case MobileSpeechRecoRequestStepId.Recognize:
				result = new RecognizeStep(guid, args);
				break;
			default:
				ExAssert.RetailAssert(false, "Invalid request step id='{0}'", new object[]
				{
					mobileSpeechRecoRequestStepId
				});
				break;
			}
			return result;
		}

		public void ExecuteAsync(MobileRecoRequestStepAsyncCompletedDelegate callback, object token)
		{
			ValidateArgument.NotNull(callback, "callback");
			ValidateArgument.NotNull(token, "token");
			MobileSpeechRecoTracer.TraceDebug(this, this.RequestId, "Entering MobileSpeechRecoRequestStep.ExecuteAsync", new object[0]);
			this.Callback = callback;
			this.Token = token;
			bool flag = MobileSpeechRecoRpcServerComponent.Instance.GuardBeforeExecution();
			this.RunGuardAfterExecution = flag;
			if (!flag)
			{
				MobileSpeechRecoTracer.TraceDebug(this, this.RequestId, "Mobile speech reco request step not processed, service shutting down", new object[0]);
				MobileRecoAsyncCompletedArgs args = new MobileRecoAsyncCompletedArgs(string.Empty, -1, new MobileRecoRPCShutdownException(this.RequestId));
				this.CompleteWithError(args);
				return;
			}
			this.InternalExecuteAsync();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.AppendFormat("<Request>", new object[0]);
			foreach (KeyValuePair<uint, object> keyValuePair in this.Args)
			{
				uint key = keyValuePair.Key;
				if (key <= 2416115743U)
				{
					if (key <= 2415984712U)
					{
						if (key == 2415919107U)
						{
							stringBuilder.AppendFormat("<Argument Key='Step Id' Value='{0}' />", keyValuePair.Value);
							continue;
						}
						if (key == 2415984712U)
						{
							stringBuilder.AppendFormat("<Argument Key='Request Id' Value='{0}' />", keyValuePair.Value);
							continue;
						}
					}
					else
					{
						if (key == 2416050179U)
						{
							stringBuilder.AppendFormat("<Argument Key='Request Type' Value='{0}' />", keyValuePair.Value);
							continue;
						}
						if (key == 2416115743U)
						{
							stringBuilder.AppendFormat("<Argument Key='Culture Name' Value='{0}' />", keyValuePair.Value);
							continue;
						}
					}
				}
				else if (key <= 2416247042U)
				{
					if (key == 2416181320U)
					{
						stringBuilder.AppendFormat("<Argument Key='User Object Guid' Value='{0}' />", keyValuePair.Value);
						continue;
					}
					if (key == 2416247042U)
					{
						stringBuilder.AppendFormat("<Argument Key='Audio Bytes' Value Length='{0}' />", (keyValuePair.Value is byte[]) ? ((keyValuePair.Value as byte[]).Length.ToString() + " bytes") : "not byte[]");
						continue;
					}
				}
				else
				{
					if (key == 2416312351U)
					{
						stringBuilder.AppendFormat("<Argument Key='Time Zone' Value Length='{0}' />", keyValuePair.Value);
						continue;
					}
					if (key == 2416377928U)
					{
						stringBuilder.AppendFormat("<Argument Key='Tenant Guid' Value='{0}' />", keyValuePair.Value);
						continue;
					}
				}
				stringBuilder.AppendFormat("<InvalidArgument-'{0}','{1}'/>", keyValuePair.Key, keyValuePair.Value);
			}
			stringBuilder.AppendFormat("</Request>", new object[0]);
			return stringBuilder.ToString();
		}

		protected static object ExtractArg(MdbefPropertyCollection args, uint argTag, string argName)
		{
			MobileSpeechRecoTracer.TraceDebug(null, Guid.Empty, "Entering MobileSpeechRecoRequestStep.ExtractArg argTag='{0}', argName='{1}'", new object[]
			{
				argTag,
				argName
			});
			object result = null;
			if (!args.TryGetValue(argTag, out result))
			{
				MobileSpeechRecoTracer.TraceDebug(null, Guid.Empty, "Entering MobileSpeechRecoRequestStep.ExtractArg argTag='{0}', argName='{1}' was not found", new object[]
				{
					argTag,
					argName
				});
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Arg '{0}' was not found", new object[]
				{
					argName
				}));
			}
			return result;
		}

		protected abstract void InternalExecuteAsync();

		protected void OnStepCompleted(MobileRecoAsyncCompletedArgs args)
		{
			ValidateArgument.NotNull(args, "args");
			ValidateArgument.NotNull(args.Result, "args.Result");
			MobileSpeechRecoTracer.TraceDebug(this, this.RequestId, "Entering MobileSpeechRecoRpcServer.OnStepCompleted Result='{0}', Error='{1}'", new object[]
			{
				args.Result,
				(args.Error == null) ? "<null>" : args.Error.ToString()
			});
			if (args.Error == null)
			{
				this.CompleteWithSuccess(args);
				return;
			}
			this.CompleteWithError(args);
		}

		protected void CollectStartStatisticsLog()
		{
			MobileSpeechRequestStatisticsLogger.MobileSpeechRequestStatisticsLogRow row = this.CollectStatisticsLog(null, this.Args);
			MobileSpeechRequestStatisticsLogger.Instance.Append(row);
		}

		private void CollectCompletedStatisticsLog(MobileRecoAsyncCompletedArgs args, MdbefPropertyCollection outArgs)
		{
			MobileSpeechRequestStatisticsLogger.MobileSpeechRequestStatisticsLogRow row = this.CollectStatisticsLog(args, outArgs);
			MobileSpeechRequestStatisticsLogger.Instance.Append(row);
		}

		private void CompleteWithSuccess(MobileRecoAsyncCompletedArgs args)
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.RequestId, "Entering MobileSpeechRecoRequestStep.CompleteWithSuccess result='{0}'", new object[]
			{
				args.Result
			});
			int num = 0;
			this.LogSuccessEvent(args.Result);
			this.Complete(args, num, new MdbefPropertyCollection
			{
				{
					2499805215U,
					args.Result
				},
				{
					2499870723U,
					num
				},
				{
					2499936287U,
					string.Empty
				}
			});
		}

		private void CompleteWithError(MobileRecoAsyncCompletedArgs args)
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.RequestId, "Entering MobileSpeechRecoRpcServer.CompleteWithError e='{0}'", new object[]
			{
				args.Error
			});
			int num = this.MapExceptionToErrorCode(args.Error);
			string text = args.Error.ToString();
			this.LogErrorEvent(num, text, num != -2147466752);
			if (num == -2147466752)
			{
				UMRPCComponentBase.HandleException(args.Error);
			}
			this.Complete(args, num, new MdbefPropertyCollection
			{
				{
					2499805215U,
					string.Empty
				},
				{
					2499870723U,
					num
				},
				{
					2499936287U,
					text
				}
			});
		}

		private void Complete(MobileRecoAsyncCompletedArgs args, int errorCode, MdbefPropertyCollection outArgs)
		{
			foreach (KeyValuePair<uint, object> keyValuePair in this.Args)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "For request id='{0}', adding input argTag='{1}'and argValue='{2}' to output args", new object[]
				{
					this.RequestId,
					keyValuePair.Key,
					keyValuePair.Value
				});
				outArgs.Add(keyValuePair.Key, keyValuePair.Value);
			}
			byte[] bytes = outArgs.GetBytes();
			try
			{
				this.Callback(errorCode, bytes, this.Token);
			}
			catch (Exception ex)
			{
				this.LogErrorEvent(-2147466752, ex.ToString(), false);
				UMRPCComponentBase.HandleException(ex);
			}
			this.CollectCompletedStatisticsLog(args, outArgs);
			if (this.RunGuardAfterExecution)
			{
				MobileSpeechRecoRpcServerComponent.Instance.GuardAfterExecution();
			}
		}

		private MobileSpeechRequestStatisticsLogger.MobileSpeechRequestStatisticsLogRow CollectStatisticsLog(MobileRecoAsyncCompletedArgs args, MdbefPropertyCollection outArgs)
		{
			MobileSpeechRequestStatisticsLogger.MobileSpeechRequestStatisticsLogRow mobileSpeechRequestStatisticsLogRow = new MobileSpeechRequestStatisticsLogger.MobileSpeechRequestStatisticsLogRow();
			bool flag = args != null;
			mobileSpeechRequestStatisticsLogRow.RequestId = this.RequestId;
			mobileSpeechRequestStatisticsLogRow.StartTime = ExDateTime.UtcNow;
			MobileSpeechRecoRequestStepId mobileSpeechRecoRequestStepId = (MobileSpeechRecoRequestStepId)MobileSpeechRecoRequestStep.ExtractArg(outArgs, 2415919107U, "stepId");
			mobileSpeechRequestStatisticsLogRow.RequestStepId = this.MapRequestStepIdToLogId(mobileSpeechRecoRequestStepId, flag);
			mobileSpeechRequestStatisticsLogRow.AudioLength = -1;
			mobileSpeechRequestStatisticsLogRow.RecognitionErrorMessage = string.Empty;
			mobileSpeechRequestStatisticsLogRow.RecognitionErrorCode = 0;
			mobileSpeechRequestStatisticsLogRow.LogOrigin = new MobileSpeechRecoLogStatisticOrigin?(MobileSpeechRecoLogStatisticOrigin.UM);
			if (flag)
			{
				mobileSpeechRequestStatisticsLogRow.RequestTotalElapsedTime = args.RequestElapsedTime;
				mobileSpeechRequestStatisticsLogRow.RecognitionTotalResults = args.ResultCount;
			}
			object obj = new object();
			if (outArgs.TryGetValue(2499870723U, out obj))
			{
				mobileSpeechRequestStatisticsLogRow.RecognitionErrorCode = (int)obj;
			}
			if (outArgs.TryGetValue(2499936287U, out obj))
			{
				mobileSpeechRequestStatisticsLogRow.RecognitionErrorMessage = (string)obj;
			}
			switch (mobileSpeechRecoRequestStepId)
			{
			case MobileSpeechRecoRequestStepId.AddRecoRequest:
				mobileSpeechRequestStatisticsLogRow.RequestType = new MobileSpeechRecoRequestType?((MobileSpeechRecoRequestType)MobileSpeechRecoRequestStep.ExtractArg(outArgs, 2416050179U, "requestType"));
				mobileSpeechRequestStatisticsLogRow.RequestLanguage = (string)MobileSpeechRecoRequestStep.ExtractArg(outArgs, 2416115743U, "cultureName");
				mobileSpeechRequestStatisticsLogRow.UserObjectGuid = new Guid?((Guid)MobileSpeechRecoRequestStep.ExtractArg(outArgs, 2416181320U, "userObjectGuid"));
				mobileSpeechRequestStatisticsLogRow.TenantGuid = new Guid?((Guid)MobileSpeechRecoRequestStep.ExtractArg(outArgs, 2416377928U, "tenantGuid"));
				mobileSpeechRequestStatisticsLogRow.TimeZone = (string)MobileSpeechRecoRequestStep.ExtractArg(outArgs, 2416312351U, "timeZone");
				break;
			case MobileSpeechRecoRequestStepId.Recognize:
			{
				byte[] array = (byte[])MobileSpeechRecoRequestStep.ExtractArg(outArgs, 2416247042U, "audioBytes");
				mobileSpeechRequestStatisticsLogRow.AudioLength = array.Length;
				break;
			}
			default:
				ExAssert.RetailAssert(false, "Invalid request step id='{0}'", new object[]
				{
					mobileSpeechRecoRequestStepId
				});
				break;
			}
			return mobileSpeechRequestStatisticsLogRow;
		}

		private MobileSpeechRecoRequestStepLogId? MapRequestStepIdToLogId(MobileSpeechRecoRequestStepId step, bool isStepCompleted)
		{
			switch (step)
			{
			case MobileSpeechRecoRequestStepId.AddRecoRequest:
				if (isStepCompleted)
				{
					return new MobileSpeechRecoRequestStepLogId?(MobileSpeechRecoRequestStepLogId.AddRecoRequestCompleted);
				}
				return new MobileSpeechRecoRequestStepLogId?(MobileSpeechRecoRequestStepLogId.AddRecoRequest);
			case MobileSpeechRecoRequestStepId.Recognize:
				if (isStepCompleted)
				{
					return new MobileSpeechRecoRequestStepLogId?(MobileSpeechRecoRequestStepLogId.RecognizeCompleted);
				}
				return new MobileSpeechRecoRequestStepLogId?(MobileSpeechRecoRequestStepLogId.Recognize);
			default:
				ExAssert.RetailAssert(false, "Cannot map request step id='{0}'", new object[]
				{
					step
				});
				return null;
			}
		}

		private int GetResultCount(string result)
		{
			int num = 0;
			XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(new StringReader(result));
			while (xmlTextReader.ReadToFollowing("Alternate"))
			{
				num++;
			}
			xmlTextReader.Close();
			return num;
		}

		private int MapExceptionToErrorCode(Exception e)
		{
			int result = -2147466752;
			if (e is MobileRecoRequestCannotBeHandledException)
			{
				result = 1722;
			}
			else if (e is MobileRecoInvalidRequestException)
			{
				result = -2147466750;
			}
			else if (e is InvalidAudioStreamException)
			{
				result = -2147466750;
			}
			else if (e is UserNotFoundException)
			{
				result = -2147466747;
			}
			else if (e is RecognizerNotInstalledException)
			{
				result = -2147466746;
			}
			else if (e is NoSpeechDetectedException)
			{
				result = -2147466743;
			}
			else if (e is SpeechGrammarException)
			{
				result = -2147466741;
			}
			return result;
		}

		private void LogErrorEvent(int errorCode, string errorMessage, bool errorExpected)
		{
			if (UMErrorCode.IsUserInputError(errorCode))
			{
				this.LogSuccessEvent(errorMessage);
				return;
			}
			ExEventLog.EventTuple tuple = errorExpected ? UMEventLogConstants.Tuple_MobileSpeechRecoRPCFailure : UMEventLogConstants.Tuple_MobileSpeechRecoRPCUnexpectedFailure;
			UmGlobals.ExEvent.LogEvent(tuple, null, new object[]
			{
				this.RequestId,
				this.Description,
				errorCode,
				CommonUtil.ToEventLogString(errorMessage)
			});
		}

		private void LogSuccessEvent(string result)
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoRPCSuccess, null, new object[]
			{
				this.RequestId,
				this.Description,
				CommonUtil.ToEventLogString(result)
			});
		}
	}
}
