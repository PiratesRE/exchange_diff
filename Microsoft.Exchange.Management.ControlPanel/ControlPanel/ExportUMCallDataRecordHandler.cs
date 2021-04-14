using System;
using System.Globalization;
using System.Management.Automation;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ExportUMCallDataRecordHandler : DataSourceService, IDownloadHandler
	{
		public PowerShellResults ProcessRequest(HttpContext context)
		{
			ExDateTime date;
			if (!ExDateTime.TryParseExact(context.Request.QueryString["Date"], "d", Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out date))
			{
				throw new BadRequestException(new Exception("ExportUMCallHandler got a request with Date not specified in query param or Date is invalid."));
			}
			ExportUMCallDataRecordParameters exportUMCallDataRecordParameters = new ExportUMCallDataRecordParameters();
			exportUMCallDataRecordParameters.Date = date;
			exportUMCallDataRecordParameters.UMDialPlan = context.Request.QueryString["UMDialPlanID"];
			exportUMCallDataRecordParameters.UMIPGateway = context.Request.QueryString["UMIPGatewayID"];
			exportUMCallDataRecordParameters.ClientStream = context.Response.OutputStream;
			context.Response.ContentType = "text/csv";
			context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", this.ConstructFilename(exportUMCallDataRecordParameters)));
			PowerShellResults powerShellResults = this.ExportObject(exportUMCallDataRecordParameters);
			if (this.IsValidUserError(powerShellResults))
			{
				powerShellResults = new PowerShellResults();
			}
			return powerShellResults;
		}

		private bool IsValidUserError(PowerShellResults results)
		{
			return !results.Succeeded && results.ErrorRecords[0].Exception is HttpException;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Export-UMCallDataRecord?Date&ClientStream@R:Organization")]
		private PowerShellResults ExportObject(ExportUMCallDataRecordParameters parameters)
		{
			PSCommand psCommand = new PSCommand().AddCommand("Export-UMCallDataRecord");
			psCommand.AddParameters(parameters);
			return base.Invoke(psCommand);
		}

		private string ConstructFilename(ExportUMCallDataRecordParameters parameters)
		{
			StringBuilder stringBuilder = new StringBuilder("UM_CDR");
			stringBuilder.Append("_");
			stringBuilder.Append(parameters.Date.ToString("yyyy-MM-dd"));
			stringBuilder.Append(".csv");
			return stringBuilder.ToString();
		}

		private const string ShortDateTimeFormat = "d";

		private const string DateFormatForFileName = "yyyy-MM-dd";

		private const string Noun = "UMCallDataRecord";

		private const string FilenamePrefix = "UM_CDR";

		private const string CSVExtension = ".csv";

		public const string ExportCmdlet = "Export-UMCallDataRecord";

		public const string ReadScope = "@R:Organization";

		private const string ExportObjectRole = "Export-UMCallDataRecord?Date&ClientStream@R:Organization";
	}
}
