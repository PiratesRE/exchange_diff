using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HDPhoto.Probes
{
	public abstract class HDPhotoProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			base.Result.StateAttribute2 = string.Format("Requester = {0} \t Publisher = {1}", base.Definition.Account, base.Definition.SecondaryAccount);
			base.Result.StateAttribute3 = base.Definition.AccountDisplayName;
			base.Result.StateAttribute4 = base.Definition.SecondaryAccountDisplayName;
			try
			{
				this.ConfigureProbe();
				stopwatch.Start();
				NetworkCredential credential = this.GetCredential(base.Definition.Account, base.Definition.AccountPassword);
				NetworkCredential credential2 = this.GetCredential(base.Definition.SecondaryAccount, base.Definition.SecondaryAccountPassword);
				bool flag = false;
				try
				{
					flag = this.UploadPhoto(base.Definition.SecondaryEndpoint, credential2);
				}
				catch (Exception e)
				{
					this.LogExceptionInfoOnResult(e);
				}
				if (flag)
				{
					stopwatch2.Start();
					WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, string.Format("Requesting User photo from {0} to {1} ", base.Definition.Account, base.Definition.SecondaryAccount), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotoprobe.cs", 90);
					this.GetUserPhoto(credential, base.Definition.SecondaryAccount);
					stopwatch2.Stop();
					base.Result.SampleValue = (double)stopwatch2.ElapsedMilliseconds;
				}
			}
			catch (Exception e2)
			{
				this.LogExceptionInfoOnResult(e2);
				throw;
			}
			finally
			{
				stopwatch.Stop();
				base.Result.StateAttribute7 = (double)stopwatch.ElapsedMilliseconds;
			}
		}

		protected void LogExceptionInfoOnResult(Exception e)
		{
			base.Result.StateAttribute5 = e.GetType().Name;
			base.Result.Exception = ((e.InnerException != null) ? e.InnerException.GetType().Name : e.GetType().Name);
			base.Result.FailureContext = ((e.InnerException != null) ? e.InnerException.Message : e.Message);
		}

		protected abstract bool UploadPhoto(string endpoint, NetworkCredential credential, MemoryStream stream, string identity);

		protected abstract HttpWebRequest GetEwsRequest(NetworkCredential credential, string publisherEmail);

		protected virtual void ConfigureProbe()
		{
		}

		private bool UploadPhoto(string endpoint, NetworkCredential credential)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "Running Set-UserPhoto", null, "UploadPhoto", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotoprobe.cs", 153);
			bool result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int width = 96;
				int height = 96;
				Bitmap bitmap = new Bitmap(width, height);
				Graphics.FromImage(bitmap).FillRectangle(new SolidBrush(Color.Blue), 0, 0, width, height);
				bitmap.Save(memoryStream, ImageFormat.Jpeg);
				string identity = string.Format("{0}\\{1}", credential.Domain, credential.UserName);
				result = this.UploadPhoto(endpoint, credential, memoryStream, identity);
			}
			return result;
		}

		private void GetUserPhoto(NetworkCredential credential, string publisherEmail)
		{
			HttpWebRequest ewsRequest = this.GetEwsRequest(credential, publisherEmail);
			HDPhotoProbe.RequestState requestState = new HDPhotoProbe.RequestState
			{
				WebRequest = ewsRequest,
				SentTimeUtc = DateTime.UtcNow
			};
			ewsRequest.BeginGetResponse(delegate(IAsyncResult x)
			{
				HDPhotoProbe.RequestState requestState2 = (HDPhotoProbe.RequestState)x.AsyncState;
				try
				{
					using (HttpWebResponse httpWebResponse = (HttpWebResponse)requestState2.WebRequest.EndGetResponse(x))
					{
						requestState2.ResponseReceivedTimeUtc = DateTime.UtcNow;
						requestState2.StatusCode = httpWebResponse.StatusCode.ToString();
						string text = "X-Exchange-GetUserPhoto-Traces";
						if (httpWebResponse.Headers.AllKeys != null && httpWebResponse.Headers.AllKeys.Count<string>() != 0 && httpWebResponse.Headers.AllKeys.Contains(text))
						{
							base.Result.ExecutionContext = httpWebResponse.Headers[text];
						}
					}
				}
				catch (WebException ex)
				{
					requestState2.StatusCode = ex.Status.ToString();
					requestState2.ResponseReceivedTimeUtc = DateTime.UtcNow;
					requestState2.Exception = ex;
				}
				catch (Exception exception)
				{
					requestState2.StatusCode = WebExceptionStatus.UnknownError.ToString();
					requestState2.ResponseReceivedTimeUtc = DateTime.UtcNow;
					requestState2.Exception = exception;
				}
				finally
				{
					base.Result.StateAttribute12 = "Sent Time = " + requestState2.SentTimeUtc.ToString();
					base.Result.StateAttribute13 = "Received Time = " + requestState2.ResponseReceivedTimeUtc.ToString();
					base.Result.StateAttribute14 = "Status Code = " + requestState2.StatusCode.ToString();
					this.allDone.Set();
				}
			}, requestState);
			this.allDone.WaitOne();
			if (requestState.Exception != null)
			{
				throw requestState.Exception;
			}
		}

		private NetworkCredential GetCredential(string userName, string password)
		{
			NetworkCredential networkCredential = new NetworkCredential
			{
				UserName = userName,
				Password = password
			};
			if (userName.Contains("@"))
			{
				string[] array = userName.Split(new char[]
				{
					'@'
				});
				if (array.Length == 2)
				{
					networkCredential.UserName = array[0];
					networkCredential.Domain = array[1];
				}
			}
			return networkCredential;
		}

		private ManualResetEvent allDone = new ManualResetEvent(false);

		private class RequestState
		{
			public DateTime SentTimeUtc { get; set; }

			public DateTime ResponseReceivedTimeUtc { get; set; }

			public HttpWebRequest WebRequest { get; set; }

			public Exception Exception { get; set; }

			public string StatusCode { get; set; }
		}
	}
}
