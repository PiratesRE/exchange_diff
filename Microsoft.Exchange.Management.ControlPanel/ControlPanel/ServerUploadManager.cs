using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ServerUploadManager
	{
		private ServerUploadManager()
		{
		}

		public PowerShellResults ProcessFileUploadRequest(HttpContext httpContext)
		{
			HttpPostedFile httpPostedFile = httpContext.Request.Files["uploadFile"];
			if (httpPostedFile != null && !string.IsNullOrEmpty(httpPostedFile.FileName) && httpPostedFile.ContentLength > 0)
			{
				string text = httpContext.Request.Form["handlerClass"];
				IUploadHandler uploadHandler = this.CreateInstance(text);
				if (httpPostedFile.ContentLength <= uploadHandler.MaxFileSize)
				{
					string text2 = httpContext.Request.Form["parameters"];
					object obj = text2.JsonDeserialize(uploadHandler.SetParameterType, null);
					UploadFileContext context = new UploadFileContext(httpPostedFile.FileName, httpPostedFile.InputStream);
					try
					{
						EcpEventLogConstants.Tuple_UploadRequestStarted.LogEvent(new object[]
						{
							EcpEventLogExtensions.GetUserNameToLog(),
							text,
							text2
						});
						return uploadHandler.ProcessUpload(context, (WebServiceParameters)obj);
					}
					finally
					{
						EcpEventLogConstants.Tuple_UploadRequestCompleted.LogEvent(new object[]
						{
							EcpEventLogExtensions.GetUserNameToLog()
						});
					}
				}
				ByteQuantifiedSize byteQuantifiedSize = new ByteQuantifiedSize((ulong)((long)uploadHandler.MaxFileSize));
				throw new HttpException(Strings.FileExceedsLimit(byteQuantifiedSize.ToString()));
			}
			throw new HttpException(Strings.UploadFileCannotBeEmpty);
		}

		private IUploadHandler CreateInstance(string handlerType)
		{
			if (handlerType == null)
			{
				throw new BadRequestException(new Exception("HandlerType cannot be null."));
			}
			Type type = null;
			UploadHandlers key;
			try
			{
				key = (UploadHandlers)Enum.Parse(typeof(UploadHandlers), handlerType, true);
			}
			catch (ArgumentException innerException)
			{
				throw new BadQueryParameterException("handlerClass", innerException);
			}
			ServerUploadManager.KnownHandlers.TryGetValue(key, out type);
			if (!(type != null))
			{
				throw new BadRequestException(new Exception("Unknown HandlerType: \"" + handlerType + "\" ."));
			}
			if (typeof(IUploadHandler).IsAssignableFrom(type))
			{
				return Activator.CreateInstance(type) as IUploadHandler;
			}
			throw new HttpException("HandlerType: \"" + type.FullName + "\" doesn't implement " + typeof(IUploadHandler).FullName);
		}

		internal const string UploadParameterkey = "parameters";

		internal const string HandlerClassKey = "handlerClass";

		internal const string UploadFileKey = "uploadFile";

		internal const string UploadHandlerFileName = "UploadHandler.ashx";

		internal static readonly LazilyInitialized<ServerUploadManager> Instance = new LazilyInitialized<ServerUploadManager>(() => new ServerUploadManager());

		private static readonly Dictionary<UploadHandlers, Type> KnownHandlers = new Dictionary<UploadHandlers, Type>
		{
			{
				UploadHandlers.UMAutoAttendantService,
				typeof(UMAutoAttendantService)
			},
			{
				UploadHandlers.UMDialPlanService,
				typeof(UMDialPlanService)
			},
			{
				UploadHandlers.AddExtensionService,
				typeof(AddExtensionService)
			},
			{
				UploadHandlers.OrgAddExtensionService,
				typeof(OrgAddExtensionService)
			},
			{
				UploadHandlers.UserPhotoService,
				typeof(UserPhotoService)
			},
			{
				UploadHandlers.FileEncodeUploadHandler,
				typeof(FileEncodeUploadHandler)
			},
			{
				UploadHandlers.MigrationCsvUploadHandler,
				typeof(MigrationCsvUploadHandler)
			},
			{
				UploadHandlers.FingerprintUploadHandler,
				typeof(FingerprintUploadHandler)
			}
		};
	}
}
