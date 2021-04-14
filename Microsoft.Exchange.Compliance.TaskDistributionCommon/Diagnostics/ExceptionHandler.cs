using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics
{
	internal abstract class ExceptionHandler
	{
		static ExceptionHandler()
		{
			ExceptionHandler.watch.Start();
		}

		public static ExceptionHandler DataSource
		{
			get
			{
				return ExceptionHandler.dataSource;
			}
		}

		public static ExceptionHandler Proxy
		{
			get
			{
				return ExceptionHandler.proxy;
			}
		}

		public static ExceptionHandler Parser
		{
			get
			{
				return ExceptionHandler.parser;
			}
		}

		public static ExceptionHandler Gray
		{
			get
			{
				return ExceptionHandler.gray;
			}
		}

		public static void FaultMessage(ComplianceMessage message, FaultDefinition fault, bool isFatal)
		{
			if (fault != null)
			{
				if (message != null)
				{
					fault.IsFatalFailure = isFatal;
					if (message.ProtocolContext.FaultDefinition != null && message.ProtocolContext.FaultDefinition == fault)
					{
						return;
					}
					foreach (FaultRecord faultRecord in fault.Faults)
					{
						if (!faultRecord.Data.ContainsKey("CID"))
						{
							faultRecord.Data["CID"] = message.CorrelationId.ToString();
							faultRecord.Data["MID"] = message.MessageId;
							faultRecord.Data["MSID"] = message.MessageSourceId;
							if (message.MessageTarget != null)
							{
								faultRecord.Data["TID"] = message.MessageTarget.Identifier;
								faultRecord.Data["TTYPE"] = message.MessageTarget.TargetType.ToString();
								if (message.MessageTarget.Mailbox != Guid.Empty)
								{
									faultRecord.Data["TDB"] = message.MessageTarget.Mailbox.ToString();
									faultRecord.Data["TMBX"] = message.MessageTarget.Database.ToString();
								}
							}
							if (message.MessageSource != null)
							{
								faultRecord.Data["STID"] = message.MessageSource.Identifier;
								faultRecord.Data["STTYPE"] = message.MessageSource.TargetType.ToString();
							}
							OrganizationId organizationId;
							if (message.TenantId != null && OrganizationId.TryCreateFromBytes(message.TenantId, Encoding.UTF8, out organizationId))
							{
								faultRecord.Data["TENANT"] = organizationId.OrganizationalUnit.ToString();
								faultRecord.Data["TGUID"] = organizationId.OrganizationalUnit.ObjectGuid.ToString();
							}
							faultRecord.Data["FEX"] = fault.IsFatalFailure.ToString();
						}
					}
					if (isFatal)
					{
						if (message.ProtocolContext.FaultDefinition != null)
						{
							message.ProtocolContext.FaultDefinition.Merge(fault);
						}
						else
						{
							message.ProtocolContext.FaultDefinition = fault;
						}
					}
				}
				MessageLogger.Instance.LogMessageFaulted(message, fault);
			}
		}

		public static bool IsFaulted(ComplianceMessage message)
		{
			return message.ProtocolContext.FaultDefinition != null && message.ProtocolContext.FaultDefinition.IsFatalFailure;
		}

		public static FaultDefinition GetFaultDefinition(ComplianceMessage message)
		{
			return message.ProtocolContext.FaultDefinition;
		}

		public virtual bool TryRun(Action code, TimeSpan duration, out FaultDefinition faultDefinition, ComplianceMessage context = null, Action<ExceptionHandler.ExceptionData> exceptionHandler = null, CancellationToken cancelToken = default(CancellationToken), double[] retrySchedule = null, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
		{
			bool result2;
			try
			{
				if (retrySchedule == null)
				{
					retrySchedule = new double[]
					{
						10.0,
						20.0,
						30.0
					};
				}
				int num = 0;
				long elapsedMilliseconds = ExceptionHandler.watch.ElapsedMilliseconds;
				ExceptionHandler.ExceptionData args = new ExceptionHandler.ExceptionData
				{
					Exception = null,
					RetryCount = num,
					ShouldRetry = false,
					Context = context
				};
				for (;;)
				{
					args.RetryCount = num;
					bool result = false;
					ExWatson.SendReportOnUnhandledException(delegate()
					{
						result = this.TryRunInternal(code, ref args);
					});
					if (result)
					{
						break;
					}
					if (exceptionHandler != null)
					{
						exceptionHandler(args);
					}
					if (!args.ShouldRetry)
					{
						goto IL_17D;
					}
					int num2 = (int)(duration.TotalMilliseconds * (retrySchedule[(num >= retrySchedule.Length) ? (retrySchedule.Length - 1) : num] / 100.0));
					if (num2 > 0)
					{
						faultDefinition = FaultDefinition.FromException(args.Exception, true, args.ShouldRetry, callerMember, callerFilePath, callerLineNumber);
						ExceptionHandler.FaultMessage(context, faultDefinition, false);
						if (cancelToken.WaitHandle.WaitOne(num2))
						{
							goto IL_17D;
						}
					}
					num++;
					if ((double)(ExceptionHandler.watch.ElapsedMilliseconds - elapsedMilliseconds) >= duration.TotalMilliseconds || elapsedMilliseconds == ExceptionHandler.watch.ElapsedMilliseconds)
					{
						goto IL_17D;
					}
				}
				faultDefinition = null;
				return true;
				IL_17D:
				faultDefinition = new FaultDefinition();
				FaultRecord faultRecord = new FaultRecord();
				faultDefinition.Faults.TryAdd(faultRecord);
				faultRecord.Data["RC"] = args.RetryCount.ToString();
				faultRecord.Data["TEX"] = args.ShouldRetry.ToString();
				faultRecord.Data["EFILE"] = callerFilePath;
				faultRecord.Data["EFUNC"] = callerMember;
				faultRecord.Data["ELINE"] = callerLineNumber.ToString();
				if (args.Exception != null)
				{
					faultRecord.Data["EX"] = args.Exception.ToString();
					LocalizedException ex = args.Exception as LocalizedException;
					if (ex != null)
					{
						faultRecord.Data["UM"] = ex.Message;
					}
				}
				faultDefinition = FaultDefinition.FromException(args.Exception, true, args.ShouldRetry, callerMember, callerFilePath, callerLineNumber);
				ExceptionHandler.FaultMessage(context, faultDefinition, false);
				result2 = false;
			}
			catch (Exception error)
			{
				faultDefinition = FaultDefinition.FromException(error, true, false, callerMember, callerFilePath, callerLineNumber);
				ExceptionHandler.FaultMessage(context, faultDefinition, true);
				throw;
			}
			return result2;
		}

		protected abstract bool TryRunInternal(Action code, ref ExceptionHandler.ExceptionData args);

		private static Stopwatch watch = new Stopwatch();

		private static ExceptionHandler dataSource = new ExceptionHandler.DataSourceExceptionHandler();

		private static ExceptionHandler proxy = new ExceptionHandler.ProxyExceptionHandler();

		private static ExceptionHandler parser = new ExceptionHandler.ParserExceptionHandler();

		private static ExceptionHandler gray = new ExceptionHandler.GrayExceptionHandler();

		public class ExceptionData
		{
			public Exception Exception { get; set; }

			public int RetryCount { get; set; }

			public bool ShouldRetry { get; set; }

			public ComplianceMessage Context { get; set; }
		}

		private class DataSourceExceptionHandler : ExceptionHandler
		{
			protected override bool TryRunInternal(Action code, ref ExceptionHandler.ExceptionData args)
			{
				bool retry = false;
				bool result = false;
				Exception exception = null;
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						try
						{
							code();
							result = true;
						}
						catch (TransientException exception2)
						{
							retry = true;
							exception = exception2;
						}
						catch (LocalizedException exception3)
						{
							exception = exception3;
						}
					});
				}
				catch (GrayException exception)
				{
					GrayException exception4;
					exception = exception4;
				}
				args.ShouldRetry = retry;
				args.Exception = exception;
				return result;
			}
		}

		private class ProxyExceptionHandler : ExceptionHandler
		{
			protected override bool TryRunInternal(Action code, ref ExceptionHandler.ExceptionData args)
			{
				bool retry = false;
				bool result = false;
				Exception exception = null;
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						try
						{
							code();
							result = true;
						}
						catch (TimeoutException exception2)
						{
							retry = true;
							exception = exception2;
						}
						catch (CommunicationException ex)
						{
							if (ex is ServerTooBusyException || ex is RetryException || ex is ChannelTerminatedException || ex is CommunicationObjectAbortedException || ex is CommunicationObjectFaultedException || ex is SecurityNegotiationException)
							{
								retry = true;
							}
							exception = ex;
						}
					});
				}
				catch (GrayException exception)
				{
					GrayException exception3;
					exception = exception3;
				}
				args.ShouldRetry = retry;
				args.Exception = exception;
				return result;
			}
		}

		private class ParserExceptionHandler : ExceptionHandler
		{
			protected override bool TryRunInternal(Action code, ref ExceptionHandler.ExceptionData args)
			{
				bool retry = false;
				bool result = false;
				Exception exception = null;
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						try
						{
							code();
							result = true;
						}
						catch (CultureNotFoundException exception2)
						{
							retry = false;
							exception = exception2;
						}
						catch (ParserException exception3)
						{
							retry = false;
							exception = exception3;
						}
					});
				}
				catch (GrayException exception)
				{
					GrayException exception4;
					exception = exception4;
				}
				args.ShouldRetry = retry;
				args.Exception = exception;
				return result;
			}
		}

		private class GrayExceptionHandler : ExceptionHandler
		{
			protected override bool TryRunInternal(Action code, ref ExceptionHandler.ExceptionData args)
			{
				bool shouldRetry = false;
				bool result = false;
				Exception exception = null;
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						code();
						result = true;
					});
				}
				catch (GrayException ex)
				{
					exception = ex;
				}
				args.ShouldRetry = shouldRetry;
				args.Exception = exception;
				return result;
			}
		}
	}
}
