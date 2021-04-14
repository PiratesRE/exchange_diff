using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ServerDownloadManager
	{
		private ServerDownloadManager()
		{
		}

		public void ProcessDownloadRequest(HttpContext httpContext)
		{
			string handlerType = httpContext.Request.QueryString["handlerClass"];
			string text = string.Format("Download request has failed for {0}.", httpContext.Request.Path);
			IDownloadHandler downloadHandler = this.CreateInstance(handlerType, text);
			PowerShellResults powerShellResults = downloadHandler.ProcessRequest(httpContext);
			if (powerShellResults.Failed)
			{
				text = text + "Exception = " + powerShellResults.ErrorRecords.ToTraceString();
				throw new BadRequestException(new Exception(text));
			}
		}

		private IDownloadHandler CreateInstance(string handlerType, string failureMsg)
		{
			Type type = null;
			if (handlerType == null)
			{
				throw new BadRequestException(new Exception(failureMsg + "HandlerType cannot be null."));
			}
			ServerDownloadManager.KnownHandlers.TryGetValue(handlerType, out type);
			if (!(type != null))
			{
				throw new BadRequestException(new Exception(failureMsg + "Unknown HandlerType: \"" + handlerType + "\" ."));
			}
			if (type.GetInterface(typeof(IDownloadHandler).FullName) != null)
			{
				return Activator.CreateInstance(type) as IDownloadHandler;
			}
			throw new HttpException(string.Concat(new string[]
			{
				failureMsg,
				"HandlerType: \"",
				type.FullName,
				"\" doesn't implement ",
				typeof(IDownloadHandler).FullName
			}));
		}

		internal const string HandlerClassKey = "handlerClass";

		private static readonly Dictionary<string, Type> KnownHandlers = new Dictionary<string, Type>
		{
			{
				"MigrationReportHandler",
				typeof(MigrationReportHandler)
			},
			{
				"ExportUMCallDataRecordHandler",
				typeof(ExportUMCallDataRecordHandler)
			},
			{
				"UserPhotoDownloadHandler",
				typeof(UserPhotoDownloadHandler)
			},
			{
				"ExportCsvHandler",
				typeof(ExportCsvHandler)
			},
			{
				"MigrationUserReportDownloadHandler",
				typeof(MigrationUserReportDownloadHandler)
			}
		};

		public static readonly LazilyInitialized<ServerDownloadManager> Instance = new LazilyInitialized<ServerDownloadManager>(() => new ServerDownloadManager());
	}
}
