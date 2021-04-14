using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Services.DispatchPipe.Ews;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExchangeVersion : OperatorComparable
	{
		public ExchangeVersion(ExchangeVersionType version)
		{
			this.version = version;
		}

		public static ExchangeVersion Current
		{
			get
			{
				EwsOperationContextBase operationContext = EWSSettings.GetOperationContext();
				if (ExchangeVersion.CanAccessMessageProperties(operationContext))
				{
					ExchangeVersion result = ExchangeVersion.Exchange2007;
					object obj = null;
					if (operationContext.RequestMessage.Properties.TryGetValue("WS_ServerVersionKey", out obj))
					{
						result = (obj as ExchangeVersion);
					}
					return result;
				}
				return ExchangeVersion.current ?? ExchangeVersion.Latest;
			}
			set
			{
				EwsOperationContextBase operationContext = EWSSettings.GetOperationContext();
				if (ExchangeVersion.CanAccessMessageProperties(operationContext))
				{
					operationContext.IncomingMessageProperties["WS_ServerVersionKey"] = value;
				}
				ExchangeVersion.current = value;
			}
		}

		internal static ExchangeVersion UnsafeGetCurrent()
		{
			EwsOperationContextBase operationContext = EWSSettings.GetOperationContext();
			if (ExchangeVersion.CanAccessMessageProperties(operationContext) && operationContext.IncomingMessageProperties.ContainsKey("WS_ServerVersionKey"))
			{
				return operationContext.IncomingMessageProperties["WS_ServerVersionKey"] as ExchangeVersion;
			}
			return ExchangeVersion.Exchange2007;
		}

		public static ExchangeVersion MapRequestVersionToServerVersion(string versionString)
		{
			if (string.IsNullOrEmpty(versionString))
			{
				return ExchangeVersion.Latest;
			}
			ExchangeVersion result;
			try
			{
				ExchangeVersionType exchangeVersionType = (ExchangeVersionType)Enum.Parse(typeof(ExchangeVersionType), versionString);
				result = new ExchangeVersion(exchangeVersionType);
			}
			catch (ArgumentException)
			{
				throw new InvalidServerVersionException();
			}
			return result;
		}

		public static void ExecuteWithSpecifiedVersion(ExchangeVersion version, ExchangeVersion.ExchangeVersionDelegate versionDelegate)
		{
			ExchangeVersion value = ExchangeVersion.Current;
			try
			{
				ExchangeVersion.Current = version;
				versionDelegate();
			}
			finally
			{
				ExchangeVersion.Current = value;
			}
		}

		public ExchangeVersionType Version
		{
			get
			{
				return this.version;
			}
		}

		public bool Supports(ExchangeVersion other)
		{
			return this >= other;
		}

		public bool Supports(ExchangeVersionType other)
		{
			return this.Version >= other;
		}

		public override int GetHashCode()
		{
			return this.Version.GetHashCode();
		}

		public override int CompareTo(OperatorComparable obj)
		{
			if (obj is ExchangeVersion)
			{
				return this.Version.CompareTo(((ExchangeVersion)obj).Version);
			}
			return -1;
		}

		public override string ToString()
		{
			return this.version.ToString();
		}

		private static bool CanAccessMessageProperties(EwsOperationContextBase operationContext)
		{
			return operationContext != null && operationContext.RequestMessage != null && operationContext.RequestMessage.State != MessageState.Closed && operationContext.IncomingMessageProperties != null;
		}

		[ThreadStatic]
		private static ExchangeVersion current;

		private ExchangeVersionType version;

		public static LazyMember<ExchangeVersionType> MaxSupportedVersion = new LazyMember<ExchangeVersionType>(delegate()
		{
			IEnumerable<ExchangeVersionType> source = Enum.GetValues(typeof(ExchangeVersionType)).Cast<ExchangeVersionType>();
			return source.Max<ExchangeVersionType>();
		});

		public static readonly ExchangeVersion Exchange2007 = new ExchangeVersion(ExchangeVersionType.Exchange2007);

		public static readonly ExchangeVersion Exchange2007SP1 = new ExchangeVersion(ExchangeVersionType.Exchange2007_SP1);

		public static readonly ExchangeVersion Exchange2010 = new ExchangeVersion(ExchangeVersionType.Exchange2010);

		public static readonly ExchangeVersion Exchange2010SP1 = new ExchangeVersion(ExchangeVersionType.Exchange2010_SP1);

		public static readonly ExchangeVersion Exchange2010SP2 = new ExchangeVersion(ExchangeVersionType.Exchange2010_SP2);

		public static readonly ExchangeVersion Exchange2012 = new ExchangeVersion(ExchangeVersionType.Exchange2012);

		public static readonly ExchangeVersion Exchange2013 = new ExchangeVersion(ExchangeVersionType.Exchange2013);

		public static readonly ExchangeVersion Exchange2013_SP1 = new ExchangeVersion(ExchangeVersionType.Exchange2013_SP1);

		public static readonly ExchangeVersion ExchangeV2_1 = new ExchangeVersion(ExchangeVersionType.V2_1);

		public static readonly ExchangeVersion ExchangeV2_2 = new ExchangeVersion(ExchangeVersionType.V2_2);

		public static readonly ExchangeVersion ExchangeV2_3 = new ExchangeVersion(ExchangeVersionType.V2_3);

		public static readonly ExchangeVersion ExchangeV2_4 = new ExchangeVersion(ExchangeVersionType.V2_4);

		public static readonly ExchangeVersion ExchangeV2_5 = new ExchangeVersion(ExchangeVersionType.V2_5);

		public static readonly ExchangeVersion ExchangeV2_6 = new ExchangeVersion(ExchangeVersionType.V2_6);

		public static readonly ExchangeVersion ExchangeV2_14 = new ExchangeVersion(ExchangeVersionType.V2_14);

		public static readonly ExchangeVersion Latest = new ExchangeVersion(ExchangeVersion.MaxSupportedVersion.Member);

		public delegate void ExchangeVersionDelegate();
	}
}
