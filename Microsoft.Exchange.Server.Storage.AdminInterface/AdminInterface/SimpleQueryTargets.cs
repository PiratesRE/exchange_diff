using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Diagnostics;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	public class SimpleQueryTargets
	{
		public static void Initialize()
		{
			SimpleQueryTargets.Instance.Register<QueryableLogTransactionInformation>(new SimpleQueryTargets.SingleRowSimpleQueryTarget<QueryableLogTransactionInformation>("ParseCommitCtx", new Type[]
			{
				typeof(string)
			}, delegate(object[] parameters)
			{
				string text = parameters[0] as string;
				if (!string.IsNullOrEmpty(text))
				{
					byte[] buffer;
					try
					{
						buffer = HexConverter.HexStringToByteArray(text.Replace(" ", string.Empty));
					}
					catch (FormatException ex)
					{
						NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
						throw new DiagnosticQueryException(ex.Message);
					}
					LogTransactionInformationParser logTransactionInformationParser = new LogTransactionInformationParser();
					logTransactionInformationParser.Parse(buffer);
					return new QueryableLogTransactionInformation(logTransactionInformationParser.LogTransactionInformationList);
				}
				throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidCommitCtxFormat());
			}), Visibility.Public);
		}

		public static void MountEventHandler(StoreDatabase database)
		{
		}
	}
}
