using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	[KnownType(typeof(MailboxReplicationServiceFault))]
	internal sealed class MailboxReplicationServiceFault
	{
		[DataMember]
		public byte[] MessageData { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public byte[] DataContextData { get; private set; }

		[DataMember]
		public string StackTrace { get; private set; }

		[DataMember]
		public int[] WKEClasses { get; private set; }

		[DataMember]
		public string ExceptionType { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public MailboxReplicationServiceFault InnerException { get; private set; }

		[DataMember]
		public int ErrorCode { get; private set; }

		[DataMember]
		public int MapiLowLevelError { get; private set; }

		[DataMember]
		public MailboxReplicationServiceFault.MRSErrorType MrsErrorType { get; private set; }

		[DataMember]
		public int FlagsInt
		{
			get
			{
				return (int)this.Flags;
			}
			set
			{
				this.Flags = (MRSErrorFlags)value;
			}
		}

		public MRSErrorFlags Flags { get; private set; }

		[DataMember(IsRequired = false)]
		public string ResourceName { get; private set; }

		[DataMember(IsRequired = false)]
		public string ResourceType { get; private set; }

		[DataMember(IsRequired = false)]
		public string WlmResourceKey { get; private set; }

		[DataMember(IsRequired = false)]
		public int WlmResourceMetricType { get; private set; }

		[DataMember(IsRequired = false)]
		public int Capacity { get; private set; }

		[DataMember(IsRequired = false)]
		public double LoadRatio { get; private set; }

		[DataMember(IsRequired = false)]
		public string LoadState { get; private set; }

		[DataMember(IsRequired = false)]
		public string LoadMetric { get; private set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string MessageDataNonLocalized { get; private set; }

		public LocalizedString Message
		{
			get
			{
				if (this.MessageData != null)
				{
					return CommonUtils.ByteDeserialize(this.MessageData);
				}
				if (this.MessageDataNonLocalized != null)
				{
					return new LocalizedString(this.MessageDataNonLocalized);
				}
				return default(LocalizedString);
			}
		}

		public LocalizedString DataContext
		{
			get
			{
				if (this.DataContextData == null)
				{
					return default(LocalizedString);
				}
				return CommonUtils.ByteDeserialize(this.DataContextData);
			}
		}

		public ExceptionSide Side
		{
			get
			{
				return (((this.Flags & MRSErrorFlags.Source) == MRSErrorFlags.Source) ? ExceptionSide.Source : ExceptionSide.None) | (((this.Flags & MRSErrorFlags.Target) == MRSErrorFlags.Target) ? ExceptionSide.Target : ExceptionSide.None);
			}
			set
			{
				this.Flags = ((this.Flags & ~(MRSErrorFlags.Source | MRSErrorFlags.Target)) | (((value & ExceptionSide.Source) == ExceptionSide.Source) ? MRSErrorFlags.Source : MRSErrorFlags.None) | (((value & ExceptionSide.Target) == ExceptionSide.Target) ? MRSErrorFlags.Target : MRSErrorFlags.None));
			}
		}

		public static void Throw(Exception ex)
		{
			MailboxReplicationServiceFault mailboxReplicationServiceFault = MailboxReplicationServiceFault.Create(ex);
			throw new FaultException<MailboxReplicationServiceFault>(mailboxReplicationServiceFault, mailboxReplicationServiceFault.Message);
		}

		public void ReconstructAndThrow(string serverName, VersionInformation serverVersion)
		{
			ExecutionContext.Create(new DataContext[]
			{
				this.DataContext.IsEmpty ? null : new WrappedDataContext(this.DataContext),
				OperationSideDataContext.GetContext(new ExceptionSide?(this.Side)),
				string.IsNullOrEmpty(serverName) ? null : new WrappedDataContext(MrsStrings.RemoteServerName(serverName))
			}).Execute(delegate
			{
				throw this.Reconstruct(serverVersion);
			});
		}

		private static MailboxReplicationServiceFault Create(Exception ex)
		{
			if (ex == null)
			{
				return null;
			}
			MailboxReplicationServiceFault mailboxReplicationServiceFault = new MailboxReplicationServiceFault();
			if (ex is LocalizedException)
			{
				mailboxReplicationServiceFault.MessageData = CommonUtils.ByteSerialize(((LocalizedException)ex).LocalizedString);
			}
			else
			{
				mailboxReplicationServiceFault.MessageData = CommonUtils.ByteSerialize(new LocalizedString(ex.Message));
			}
			mailboxReplicationServiceFault.Side = (ExecutionContext.GetExceptionSide(ex) ?? ExceptionSide.None);
			mailboxReplicationServiceFault.ExceptionType = CommonUtils.GetFailureType(ex);
			mailboxReplicationServiceFault.StackTrace = ex.StackTrace;
			mailboxReplicationServiceFault.DataContextData = CommonUtils.ByteSerialize(new LocalizedString(ExecutionContext.GetDataContext(ex)));
			mailboxReplicationServiceFault.ErrorCode = CommonUtils.HrFromException(ex);
			mailboxReplicationServiceFault.MapiLowLevelError = CommonUtils.GetMapiLowLevelError(ex);
			mailboxReplicationServiceFault.MrsErrorType = MailboxReplicationServiceFault.ClassifyException(ex);
			WellKnownException[] array = CommonUtils.ClassifyException(ex);
			mailboxReplicationServiceFault.WKEClasses = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				mailboxReplicationServiceFault.WKEClasses[i] = (int)array[i];
			}
			StaticCapacityExceededReservationException ex2 = ex as StaticCapacityExceededReservationException;
			if (ex2 != null)
			{
				mailboxReplicationServiceFault.ResourceName = ex2.ResourceName;
				mailboxReplicationServiceFault.ResourceType = ex2.ResourceType;
				mailboxReplicationServiceFault.Capacity = ex2.Capacity;
			}
			WlmCapacityExceededReservationException ex3 = ex as WlmCapacityExceededReservationException;
			if (ex3 != null)
			{
				mailboxReplicationServiceFault.ResourceName = ex3.ResourceName;
				mailboxReplicationServiceFault.ResourceType = ex3.ResourceType;
				mailboxReplicationServiceFault.WlmResourceKey = ex3.WlmResourceKey;
				mailboxReplicationServiceFault.WlmResourceMetricType = ex3.WlmResourceMetricType;
				mailboxReplicationServiceFault.Capacity = ex3.Capacity;
			}
			WlmResourceUnhealthyException ex4 = ex as WlmResourceUnhealthyException;
			if (ex4 != null)
			{
				mailboxReplicationServiceFault.ResourceName = ex4.ResourceName;
				mailboxReplicationServiceFault.ResourceType = ex4.ResourceType;
				mailboxReplicationServiceFault.WlmResourceKey = ex4.WlmResourceKey;
				mailboxReplicationServiceFault.WlmResourceMetricType = ex4.WlmResourceMetricType;
				mailboxReplicationServiceFault.LoadRatio = ex4.ReportedLoadRatio;
				mailboxReplicationServiceFault.LoadState = ex4.ReportedLoadState;
				mailboxReplicationServiceFault.LoadMetric = ex4.Metric;
			}
			mailboxReplicationServiceFault.InnerException = MailboxReplicationServiceFault.Create(ex.InnerException);
			return mailboxReplicationServiceFault;
		}

		private static MailboxReplicationServiceFault.MRSErrorType ClassifyException(Exception ex)
		{
			if (ex is MRSProxyConnectionLimitReachedTransientException)
			{
				return MailboxReplicationServiceFault.MRSErrorType.ProxyThrottlingLimitReached;
			}
			if (CommonUtils.IsTransientException(ex))
			{
				return MailboxReplicationServiceFault.MRSErrorType.Transient;
			}
			return MailboxReplicationServiceFault.MRSErrorType.Permanent;
		}

		private WellKnownException MatchWellKnownException(params WellKnownException[] wkesToCheck)
		{
			if (this.WKEClasses != null)
			{
				for (int i = 0; i < this.WKEClasses.Length; i++)
				{
					foreach (WellKnownException ex in wkesToCheck)
					{
						if (this.WKEClasses[i] == (int)ex)
						{
							return ex;
						}
					}
				}
			}
			return WellKnownException.None;
		}

		private Exception Reconstruct(VersionInformation serverVersion)
		{
			LocalizedException ex = null;
			Exception innerException = (this.InnerException != null) ? this.InnerException.Reconstruct(serverVersion) : null;
			if (this.MrsErrorType == MailboxReplicationServiceFault.MRSErrorType.ProxyThrottlingLimitReached)
			{
				ex = new MRSProxyConnectionLimitReachedTransientException(this.Message, innerException);
			}
			else
			{
				WellKnownException ex2 = this.MatchWellKnownException(new WellKnownException[]
				{
					WellKnownException.StaticCapacityExceededReservation,
					WellKnownException.WlmCapacityExceededReservation,
					WellKnownException.WlmResourceUnhealthy
				});
				if (ex2 != WellKnownException.None)
				{
					switch (ex2)
					{
					case WellKnownException.StaticCapacityExceededReservation:
						ex = new StaticCapacityExceededReservationException(this.ResourceName, this.ResourceType, this.Capacity);
						break;
					case WellKnownException.WlmCapacityExceededReservation:
						ex = new WlmCapacityExceededReservationException(this.ResourceName, this.ResourceType, this.WlmResourceKey, this.WlmResourceMetricType, this.Capacity);
						break;
					case WellKnownException.WlmResourceUnhealthy:
						ex = new WlmResourceUnhealthyException(this.ResourceName, this.ResourceType, this.WlmResourceKey, this.WlmResourceMetricType, this.LoadRatio, this.LoadState, this.LoadMetric);
						break;
					}
				}
				if (ex == null)
				{
					if (this.MrsErrorType == MailboxReplicationServiceFault.MRSErrorType.Transient)
					{
						ex = new RemoteTransientException(this.Message, innerException);
					}
					else
					{
						ex = new RemotePermanentException(this.Message, innerException);
					}
				}
			}
			IMRSRemoteException ex3 = ex as IMRSRemoteException;
			if (ex3 != null)
			{
				ex3.OriginalFailureType = this.ExceptionType;
				ex3.MapiLowLevelError = this.MapiLowLevelError;
				ex3.RemoteStackTrace = this.StackTrace;
				if (this.WKEClasses != null)
				{
					ex3.WKEClasses = new WellKnownException[this.WKEClasses.Length + 1];
					for (int i = 0; i < this.WKEClasses.Length; i++)
					{
						ex3.WKEClasses[i] = (WellKnownException)this.WKEClasses[i];
					}
					ex3.WKEClasses[this.WKEClasses.Length] = WellKnownException.MRSRemote;
				}
				else
				{
					ex3.WKEClasses = CommonUtils.ClassifyException(ex);
				}
				if (serverVersion != null && !serverVersion[17])
				{
					if (!CommonUtils.ExceptionIs(ex, new WellKnownException[]
					{
						WellKnownException.MRSMailboxIsLocked
					}) && (MailboxReplicationServiceFault.DownlevelMailboxIsLockedFailureTypes.Contains(ex3.OriginalFailureType) || this.Message.StringId == MrsStrings.DestMailboxAlreadyBeingMoved.StringId || this.Message.StringId == MrsStrings.SourceMailboxAlreadyBeingMoved.StringId))
					{
						ex3.WKEClasses = new List<WellKnownException>(ex3.WKEClasses)
						{
							WellKnownException.MRSMailboxIsLocked
						}.ToArray();
					}
					if (!CommonUtils.ExceptionIs(ex, new WellKnownException[]
					{
						WellKnownException.MapiNotFound
					}) && ex3.OriginalFailureType == "MapiExceptionNotFound")
					{
						ex3.WKEClasses = new List<WellKnownException>(ex3.WKEClasses)
						{
							WellKnownException.MapiNotFound
						}.ToArray();
					}
				}
			}
			ex.ErrorCode = this.ErrorCode;
			return ex;
		}

		private static readonly HashSet<string> DownlevelMailboxIsLockedFailureTypes = new HashSet<string>
		{
			"MapiExceptionMailboxInTransit",
			"SourceMailboxAlreadyBeingMovedTransientException",
			"SourceMailboxAlreadyBeingMovedPermanentException",
			"DestMailboxAlreadyBeingMovedTransientException",
			"DestMailboxAlreadyBeingMovedPermanentException"
		};

		public enum MRSErrorType
		{
			Permanent = 1,
			Transient,
			ProxyThrottlingLimitReached,
			ResourceUnhealthy
		}
	}
}
