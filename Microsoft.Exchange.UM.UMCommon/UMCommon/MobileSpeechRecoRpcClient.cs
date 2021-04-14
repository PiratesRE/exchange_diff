using System;
using System.Globalization;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.Rpc;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class MobileSpeechRecoRpcClient : MobileSpeechRecoRpcClientBase
	{
		public object State { get; set; }

		private string Description { get; set; }

		private string ServerName { get; set; }

		private MobileSpeechRecoRpcClient.MobileSpeechRecoAsyncResult AsyncResult { get; set; }

		public MobileSpeechRecoRpcClient(Guid requestId, string serverName, object state) : base(requestId, serverName)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "Entering MobileSpeechRecoRpcClient constructor", new object[0]);
			this.ServerName = serverName;
			this.State = state;
		}

		public IAsyncResult BeginAddRecoRequest(MobileSpeechRecoRequestType requestType, Guid userObjectGuid, Guid tenantGuid, CultureInfo culture, ExTimeZone timeZone, AsyncCallback callback, object state)
		{
			ValidateArgument.NotNull(culture, "culture");
			ValidateArgument.NotNull(timeZone, "timeZone");
			CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "MobileSpeechRecoRpcClient.BeginAddRecoRequest requestId='{0}', requestType='{1}', userObjectGuid='{2}', tenantGuid='{3}', culture='{4}', timeZone='{5}'", new object[]
			{
				base.RequestId,
				requestType,
				userObjectGuid,
				tenantGuid,
				culture,
				timeZone.Id
			});
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoClientAddRecoRequestRPCParams, null, new object[]
			{
				base.RequestId,
				requestType,
				culture.Name,
				userObjectGuid,
				tenantGuid,
				timeZone.Id,
				this.ServerName
			});
			this.AsyncResult = new MobileSpeechRecoRpcClient.MobileSpeechRecoAsyncResult(base.RequestId, callback, state);
			this.Description = "Add Reco Request";
			byte[] bytes = new MdbefPropertyCollection
			{
				{
					2415919107U,
					0
				},
				{
					2415984712U,
					base.RequestId
				},
				{
					2416050179U,
					(int)requestType
				},
				{
					2416181320U,
					userObjectGuid
				},
				{
					2416377928U,
					tenantGuid
				},
				{
					2416115743U,
					culture.ToString()
				},
				{
					2416312351U,
					timeZone.Id
				}
			}.GetBytes();
			this.BeginExecuteStepWrapper(bytes);
			return this.AsyncResult;
		}

		public MobileRecoRPCAsyncCompletedArgs EndAddRecoRequest(IAsyncResult asyncResult)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "MobileSpeechRecoRpcClient.EndAddRecoRequest requestId='{0}'", new object[]
			{
				base.RequestId
			});
			return this.InternalEndOperation(asyncResult);
		}

		public IAsyncResult BeginRecognize(byte[] audioBytes, AsyncCallback callback, object state)
		{
			ValidateArgument.NotNull(audioBytes, "audioBytes");
			CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "MobileSpeechRecoRpcClient.BeginRecognize requestId='{0}', audioBytes.Length='{1}' bytes", new object[]
			{
				base.RequestId,
				audioBytes.Length
			});
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoClientRecognizeRPCParams, null, new object[]
			{
				base.RequestId,
				audioBytes.Length,
				this.ServerName
			});
			this.AsyncResult = new MobileSpeechRecoRpcClient.MobileSpeechRecoAsyncResult(base.RequestId, callback, state);
			this.Description = "Recognize";
			byte[] bytes = new MdbefPropertyCollection
			{
				{
					2415919107U,
					1
				},
				{
					2415984712U,
					base.RequestId
				},
				{
					2416247042U,
					audioBytes
				}
			}.GetBytes();
			this.BeginExecuteStepWrapper(bytes);
			return this.AsyncResult;
		}

		public MobileRecoRPCAsyncCompletedArgs EndRecognize(IAsyncResult asyncResult)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "MobileSpeechRecoRpcClient.EndRecognize requestId='{0}'", new object[]
			{
				base.RequestId
			});
			return this.InternalEndOperation(asyncResult);
		}

		private MobileRecoRPCAsyncCompletedArgs InternalEndOperation(IAsyncResult asyncResult)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "MobileSpeechRecoRpcClient.InternalEndOperation requestId='{0}'", new object[]
			{
				base.RequestId
			});
			MobileRecoRPCAsyncCompletedArgs completedArgs;
			using (MobileSpeechRecoRpcClient.MobileSpeechRecoAsyncResult mobileSpeechRecoAsyncResult = asyncResult as MobileSpeechRecoRpcClient.MobileSpeechRecoAsyncResult)
			{
				mobileSpeechRecoAsyncResult.AsyncWaitHandle.WaitOne();
				completedArgs = mobileSpeechRecoAsyncResult.CompletedArgs;
			}
			return completedArgs;
		}

		private void BeginExecuteStepWrapper(byte[] inBlob)
		{
			try
			{
				base.BeginExecuteStep(inBlob, new AsyncCallback(this.InternalAsyncCallback));
			}
			catch (RpcException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.RpcTracer, this.GetHashCode(), "MobileSpeechRecoRpcClient.BeginExecuteStepWrapper requestId='{0}', error='{1}'", new object[]
				{
					base.RequestId,
					ex
				});
				this.CompleteOperation(string.Empty, -2147466752, ex.ToString());
			}
		}

		private void InternalAsyncCallback(IAsyncResult ar)
		{
			string text = string.Empty;
			int num = 0;
			string text2 = string.Empty;
			Exception ex = null;
			try
			{
				byte[] array;
				bool flag;
				base.EndExecuteStep(ar, out array, out flag);
				if (flag)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "MobileSpeechRecoRpcClient.AsyncCallback requestId='{0}' timed out", new object[]
					{
						base.RequestId
					});
					text = string.Empty;
					num = -2147466742;
					text2 = string.Empty;
				}
				else
				{
					object obj = null;
					MdbefPropertyCollection args = MdbefPropertyCollection.Create(array, 0, array.Length);
					this.ExtractArg(args, 2499805215U, "Result not found", out obj);
					text = (obj as string);
					if (text == null)
					{
						throw new ArgumentOutOfRangeException("result", "Result is null");
					}
					this.ExtractArg(args, 2499870723U, "Error code not found", out obj);
					num = (int)obj;
					this.ExtractArg(args, 2499936287U, "Error message not found", out obj);
					text2 = (obj as string);
					if (text2 == null)
					{
						throw new ArgumentOutOfRangeException("errorMessage", "Error message is null");
					}
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "MobileSpeechRecoRpcClient.AsyncCallback requestId='{0}', result='{1}', errorCode='{2}', errorMessage='{3}'", new object[]
					{
						base.RequestId,
						text,
						num,
						text2
					});
				}
			}
			catch (RpcException ex2)
			{
				ex = ex2;
			}
			catch (ArgumentException ex3)
			{
				ex = ex3;
			}
			finally
			{
				if (ex != null)
				{
					CallIdTracer.TraceError(ExTraceGlobals.RpcTracer, this.GetHashCode(), "MobileSpeechRecoRpcClient.AsyncCallback requestId='{0}', error='{1}'", new object[]
					{
						base.RequestId,
						ex
					});
					text = string.Empty;
					num = -2147466752;
					text2 = ex.ToString();
				}
			}
			this.CompleteOperation(text, num, text2);
		}

		private void ExtractArg(MdbefPropertyCollection args, uint argTag, string errorMessage, out object val)
		{
			val = null;
			if (!args.TryGetValue(argTag, out val))
			{
				throw new ArgumentException(errorMessage);
			}
		}

		private void CompleteOperation(string result, int errorCode, string errorMessage)
		{
			if (errorCode == 0)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoClientRPCSuccess, null, new object[]
				{
					base.RequestId,
					this.Description,
					CommonUtil.ToEventLogString(result),
					this.ServerName
				});
			}
			else if (UMErrorCode.IsUserInputError(errorCode))
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoClientRPCSuccess, null, new object[]
				{
					base.RequestId,
					this.Description,
					CommonUtil.ToEventLogString(errorMessage),
					this.ServerName
				});
			}
			else
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoClientRPCFailure, null, new object[]
				{
					base.RequestId,
					this.Description,
					errorCode,
					CommonUtil.ToEventLogString(errorMessage),
					this.ServerName
				});
			}
			MobileRecoRPCAsyncCompletedArgs result2 = new MobileRecoRPCAsyncCompletedArgs(result, errorCode, errorMessage);
			this.AsyncResult.CompleteOperation(result2);
		}

		private const string AddRecoRequestDescriptionStr = "Add Reco Request";

		private const string RecognizeDescriptionStr = "Recognize";

		private class MobileSpeechRecoAsyncResult : AsyncResult
		{
			public MobileSpeechRecoAsyncResult(Guid requestId, AsyncCallback callback, object state) : base(callback, state, false)
			{
				this.RequestId = requestId;
			}

			public MobileRecoRPCAsyncCompletedArgs CompletedArgs { get; set; }

			private Guid RequestId { get; set; }

			internal void CompleteOperation(MobileRecoRPCAsyncCompletedArgs result)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "Entering MobileSpeechRecoRpcAsyncResult.CompleteOperation requestId='{0}'", new object[]
				{
					this.RequestId
				});
				this.CompletedArgs = result;
				base.IsCompleted = true;
			}
		}
	}
}
