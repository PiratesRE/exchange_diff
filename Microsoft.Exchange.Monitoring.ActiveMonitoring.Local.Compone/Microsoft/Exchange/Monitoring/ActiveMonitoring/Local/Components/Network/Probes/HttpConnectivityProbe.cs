using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Probes
{
	public class HttpConnectivityProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			string target = "http://www.bing.com/";
			this.DoHttpTest(target);
		}

		protected void DoHttpTest(string target)
		{
			string arg = string.Empty;
			int i = 0;
			while (i < 3)
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Try {0}: Http request will be made to target: '{1}'", new object[]
				{
					i + 1,
					target
				});
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(target);
				if (i != 0)
				{
					Thread.Sleep(TimeSpan.FromSeconds(7.0));
				}
				string text = string.Empty;
				try
				{
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
					{
						stopwatch.Stop();
						base.Result.SampleValue = (double)stopwatch.ElapsedMilliseconds;
						NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Response from {0}. Status: {1}", new object[]
						{
							httpWebResponse.ResponseUri.ToString(),
							httpWebResponse.StatusDescription.ToString()
						});
						try
						{
							using (Stream responseStream = httpWebResponse.GetResponseStream())
							{
								NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Receive stream has been received. Readability status: {0}", new object[]
								{
									responseStream.CanRead.ToString()
								});
								try
								{
									using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
									{
										NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Stream Reader object has been created", new object[0]);
										try
										{
											text = streamReader.ReadToEnd();
											NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Result string with length {0} has been generated.", new object[]
											{
												text.Length
											});
										}
										catch (Exception ex)
										{
											arg = string.Format("Error while reading stream. Exception: {0}", ex.Message);
											NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Error while reading stream. Exception: {0}", new object[]
											{
												ex.Message
											});
											goto IL_2E0;
										}
									}
								}
								catch (Exception ex2)
								{
									arg = string.Format("Stream could not be read. Exception: {0}", ex2.Message);
									NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Stream could not be read. Exception: {0}", new object[]
									{
										ex2.Message
									});
									goto IL_2E0;
								}
							}
						}
						catch (Exception ex3)
						{
							arg = string.Format("Response stream could not be got from {0} Exception: {1}", target, ex3.Message);
							NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Response stream could not be got from {0} Exception: {1}", new object[]
							{
								target,
								ex3.Message
							});
							goto IL_2E0;
						}
					}
				}
				catch (WebException ex4)
				{
					arg = string.Format("No Server Response. Exception: {0}", ex4.Message);
					NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "No Server Response. Exception: {0}", new object[]
					{
						ex4.Message
					});
					goto IL_2E0;
				}
				goto IL_2BB;
				IL_2E0:
				i++;
				continue;
				IL_2BB:
				if (!string.IsNullOrEmpty(text))
				{
					NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "HTTP Request has been completed successfully.", new object[0]);
					return;
				}
				goto IL_2E0;
			}
			throw new ApplicationException(string.Format("HttpGet Test failed. Error details: {0}", arg));
		}

		private const int NumTries = 3;

		private const int RetryDelay = 7;
	}
}
