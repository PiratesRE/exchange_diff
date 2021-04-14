using System;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class UserPhotoDownloadHandler : IDownloadHandler
	{
		public PowerShellResults ProcessRequest(HttpContext context)
		{
			Identity identity = Identity.FromIdParameter(context.Request.QueryString["Identity"]);
			if (identity == null || string.IsNullOrEmpty(identity.RawIdentity))
			{
				throw new BadQueryParameterException("Identity");
			}
			bool flag;
			if (!bool.TryParse(context.Request.QueryString["preview"], out flag))
			{
				throw new BadQueryParameterException("preview");
			}
			ExTraceGlobals.UserPhotosTracer.TraceInformation<string>(0, 0L, "Processing photo download request for {0}.", identity.RawIdentity);
			context.Response.ContentType = "image/jpeg";
			UserPhotoService userPhotoService = new UserPhotoService();
			PowerShellResults<UserPhoto> powerShellResults;
			if (flag)
			{
				ExTraceGlobals.UserPhotosTracer.TraceInformation(0, 0L, "Retrieving preview photo");
				powerShellResults = userPhotoService.GetPreviewPhoto(identity);
			}
			else
			{
				ExTraceGlobals.UserPhotosTracer.TraceInformation(0, 0L, "Retrieving saved photo");
				powerShellResults = userPhotoService.GetSavedPhoto(identity);
			}
			if (this.ResultsSucceeded(powerShellResults))
			{
				if (powerShellResults.HasValue)
				{
					ExTraceGlobals.UserPhotosTracer.TraceInformation(0, 0L, "Returning photo for user.");
					byte[] pictureData = powerShellResults.Value.Photo.PictureData;
					context.Response.OutputStream.Write(pictureData, 0, pictureData.Length);
				}
				else
				{
					ExTraceGlobals.UserPhotosTracer.TraceWarning(0, 0L, "The result succeeded but does not have a value. Returning empty result set.");
					powerShellResults = new PowerShellResults<UserPhoto>();
				}
			}
			else
			{
				ExTraceGlobals.UserPhotosTracer.TraceInformation(0, 0L, "Returning empty result from failed request.");
				powerShellResults = new PowerShellResults<UserPhoto>();
			}
			return powerShellResults;
		}

		private bool ResultsSucceeded(PowerShellResults results)
		{
			if (!results.Succeeded)
			{
				if (results.ErrorRecords[0].Exception is HttpException)
				{
					ExTraceGlobals.UserPhotosTracer.TraceWarning(0, 0L, "HttpException occurred likely because user terminated the connection.");
				}
				else if (results.ErrorRecords[0].Exception is UserPhotoNotFoundException)
				{
					ExTraceGlobals.UserPhotosTracer.TraceInformation(0, 0L, "Result failed because a photo for the user does not exist.");
				}
				else
				{
					ExTraceGlobals.UserPhotosTracer.TraceError<Exception>(0, 0L, "An unexpected exception occurred when retrieving the user photo. {0}", results.ErrorRecords[0].Exception);
				}
				return false;
			}
			return true;
		}
	}
}
