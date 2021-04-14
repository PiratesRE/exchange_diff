using System;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.LiveServicesHelper;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Sharing
{
	public abstract class WindowsLiveIdTask : Task
	{
		internal bool TryExecuteWebMethod(LiveIdInstanceType liveIdInstanceType, out XmlDocument xmlDocument)
		{
			bool result = false;
			xmlDocument = null;
			try
			{
				xmlDocument = this.WindowsLiveIdMethod(liveIdInstanceType);
				result = true;
			}
			catch (UriFormatException exception)
			{
				this.LogError(exception, ErrorCategory.InvalidArgument);
			}
			catch (XmlException exception2)
			{
				this.LogError(exception2, ErrorCategory.InvalidResult);
			}
			catch (SoapException ex)
			{
				if (!ex.Detail.InnerText.Contains("0x80045a64") && !ex.Detail.InnerText.Contains("0x80048163"))
				{
					this.LogError(ex, ErrorCategory.InvalidOperation);
				}
			}
			catch (WebException exception3)
			{
				this.LogError(exception3, ErrorCategory.InvalidOperation);
			}
			catch (LiveServicesHelperConfigException exception4)
			{
				this.LogError(exception4, ErrorCategory.MetadataError);
			}
			catch (LiveServicesException exception5)
			{
				this.LogError(exception5, ErrorCategory.MetadataError);
			}
			catch (XPathException exception6)
			{
				this.LogError(exception6, ErrorCategory.InvalidResult);
			}
			return result;
		}

		internal void LogError(Exception exception, ErrorCategory category)
		{
			base.WriteVerbose(Strings.ExceptionOccured(this.GetExtendedExceptionInformation(exception)));
		}

		protected abstract XmlDocument WindowsLiveIdMethod(LiveIdInstanceType liveIdInstanceType);

		protected abstract ConfigurableObject ParseResponse(LiveIdInstanceType liveIdInstanceType, XmlDocument xmlResponse);

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			foreach (object obj in Enum.GetValues(typeof(LiveIdInstanceType)))
			{
				LiveIdInstanceType liveIdInstanceType = (LiveIdInstanceType)obj;
				XmlDocument xmlDocument = null;
				if (this.TryExecuteWebMethod(liveIdInstanceType, out xmlDocument) && xmlDocument != null)
				{
					base.WriteObject(this.ParseResponse(liveIdInstanceType, xmlDocument));
				}
			}
			TaskLogger.LogExit();
		}

		private string GetExtendedExceptionInformation(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			if (!(exception is WebException))
			{
				if (!(exception is SoapException))
				{
					goto IL_14A;
				}
			}
			while (exception != null)
			{
				WebException ex = exception as WebException;
				if (ex != null)
				{
					stringBuilder.Append("WebException.Response = ");
					if (ex.Response != null)
					{
						using (Stream responseStream = ex.Response.GetResponseStream())
						{
							if (responseStream.CanRead)
							{
								if (responseStream.CanSeek)
								{
									responseStream.Seek(0L, SeekOrigin.Begin);
								}
								using (StreamReader streamReader = new StreamReader(responseStream))
								{
									stringBuilder.AppendLine();
									stringBuilder.AppendLine(streamReader.ReadToEnd());
									goto IL_A0;
								}
							}
							stringBuilder.AppendLine("<cannot read response stream>");
							IL_A0:
							goto IL_B8;
						}
					}
					stringBuilder.AppendLine("<Null>");
				}
				IL_B8:
				SoapException ex2 = exception as SoapException;
				if (ex2 != null)
				{
					if (ex2.Code != null)
					{
						stringBuilder.Append("SoapException.Code = ");
						stringBuilder.Append(ex2.Code);
						stringBuilder.AppendLine();
					}
					if (ex2.Detail != null)
					{
						stringBuilder.AppendLine("SoapException.Detail = ");
						stringBuilder.AppendLine(ex2.Detail.OuterXml);
					}
				}
				stringBuilder.AppendLine("Exception:");
				stringBuilder.AppendLine(exception.ToString());
				stringBuilder.AppendLine();
				exception = exception.InnerException;
			}
			IL_14A:
			return stringBuilder.ToString();
		}
	}
}
