using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Hygiene.Data.Sync;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class CheckForwardSyncJobIsRunning : ProbeWorkItem
	{
		public CheckForwardSyncJobIsRunning()
		{
			this.session = new SyncSession(CheckForwardSyncJobIsRunning.SyncSessionCallerId);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			ServiceCookie[] array = null;
			try
			{
				this.Log("Reading Probe parameters...", new object[0]);
				this.ReadProbeParameters();
				this.Log("Call SyncSession.FindCookies...", new object[0]);
				array = this.session.FindCookies<ServiceCookie>(this.GetServiceCookieFilter());
				if (array == null || array.Length == 0)
				{
					this.Log("No stale cookies were found.", new object[0]);
				}
				else
				{
					string message = "SyncSession.FindCookies returned {0} cookies. Min LastChanged = {1}; Max LastChanged = {2}; Min LastCompletedTime = {3}; Max LastCompletedTime = {4}";
					object[] array2 = new object[5];
					array2[0] = array.Count<ServiceCookie>();
					array2[1] = array.Min((ServiceCookie sc) => sc.LastChanged);
					array2[2] = array.Max((ServiceCookie sc) => sc.LastChanged);
					array2[3] = array.Min((ServiceCookie sc) => sc.LastCompletedTime);
					array2[4] = array.Max((ServiceCookie sc) => sc.LastCompletedTime);
					this.Log(message, array2);
					DateTime currentDateTime = DateTime.UtcNow;
					IEnumerable<ServiceCookie> source = array.Where(delegate(ServiceCookie sc)
					{
						TimeSpan t = currentDateTime - sc.LastChanged;
						return t >= this.timeSinceLastChangeLowerBound && t < this.timeSinceLastChangeUpperBound;
					});
					if (source.Any<ServiceCookie>())
					{
						base.Result.StateAttribute1 = string.Join(", ", from sc in source
						select sc.ServiceInstance);
						base.Result.StateAttribute2 = string.Format("was not updated for at least {0} minutes", (int)this.timeSinceLastChangeLowerBound.TotalMinutes);
						throw new ForwardSyncProbeException();
					}
					this.Log("No cookies were found with a LastChanged between [{0}, {1}] ago.", new object[]
					{
						this.timeSinceLastChangeLowerBound,
						this.timeSinceLastChangeUpperBound
					});
					IEnumerable<ServiceCookie> source2 = array.Where(delegate(ServiceCookie sc)
					{
						TimeSpan t = currentDateTime - sc.LastCompletedTime;
						return sc.LastCompletedTime != DateTime.MinValue && t >= this.timeSinceLastCompletionLowerBound && t < this.timeSinceLastCompletionUpperBound;
					});
					if (source2.Any<ServiceCookie>())
					{
						base.Result.StateAttribute1 = string.Join(", ", from sc in source2
						select sc.ServiceInstance);
						base.Result.StateAttribute2 = string.Format("was not completed in the last {0} minutes", (int)this.timeSinceLastCompletionLowerBound.TotalMinutes);
						throw new ForwardSyncProbeException();
					}
					this.Log("No cookies were found with a LastCompletedTime between [{0}, {1}] ago.", new object[]
					{
						this.timeSinceLastCompletionLowerBound,
						this.timeSinceLastCompletionUpperBound
					});
				}
			}
			catch (ArgumentException)
			{
				this.Log("Probe parameters are invalid.", new object[0]);
				throw;
			}
			catch (Exception)
			{
				if (array == null || array.Length == 0)
				{
					this.Log("No services cookies were returned.", new object[0]);
				}
				else
				{
					this.Log(CheckForwardSyncJobIsRunning.GetServiceCookiesStateReport(array), new object[0]);
				}
				throw;
			}
		}

		private static string GetServiceCookiesStateReport(IEnumerable<ServiceCookie> serviceCookies)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ServiceCookie serviceCookie in serviceCookies)
			{
				stringBuilder.AppendFormat("Service Instance = {0}; LastCompletedTime = {1}; LastChanged = {2}; ActiveMachine = {3}; Complete = {4};{5}", new object[]
				{
					serviceCookie.ServiceInstance,
					serviceCookie.LastCompletedTime,
					serviceCookie.LastChanged,
					serviceCookie.ActiveMachine,
					serviceCookie.Complete,
					Environment.NewLine
				});
			}
			return stringBuilder.ToString();
		}

		private static T ReadProbeParameters<T>(Dictionary<string, string> parameters, string name)
		{
			string value;
			if (parameters.TryGetValue(name, out value))
			{
				return (T)((object)Convert.ChangeType(value, typeof(T)));
			}
			return default(T);
		}

		private void ReadProbeParameters()
		{
			this.timeSinceLastCompletionLowerBound = TimeSpan.FromMinutes((double)CheckForwardSyncJobIsRunning.ReadProbeParameters<int>(base.Definition.Attributes, "NumberOfMinutesSinceLastCompletionLowerBound"));
			this.timeSinceLastCompletionUpperBound = TimeSpan.FromMinutes((double)CheckForwardSyncJobIsRunning.ReadProbeParameters<int>(base.Definition.Attributes, "NumberOfMinutesSinceLastCompletionUpperBound"));
			this.timeSinceLastChangeLowerBound = TimeSpan.FromMinutes((double)CheckForwardSyncJobIsRunning.ReadProbeParameters<int>(base.Definition.Attributes, "NumberOfMinutesSinceLastChangeLowerBound"));
			this.timeSinceLastChangeUpperBound = TimeSpan.FromMinutes((double)CheckForwardSyncJobIsRunning.ReadProbeParameters<int>(base.Definition.Attributes, "NumberOfMinutesSinceLastChangeUpperBound"));
			if (this.timeSinceLastCompletionUpperBound == TimeSpan.Zero)
			{
				this.timeSinceLastCompletionUpperBound = TimeSpan.MaxValue;
			}
			if (this.timeSinceLastChangeUpperBound == TimeSpan.Zero)
			{
				this.timeSinceLastChangeUpperBound = TimeSpan.MaxValue;
			}
			if (this.timeSinceLastCompletionLowerBound > this.timeSinceLastCompletionUpperBound)
			{
				throw new ArgumentException(string.Format("The value of the parameter NumberOfMinutesSinceLastCompletionLowerBound ({0}) should be less than the value of the parameter NumberOfMinutesSinceLastCompletionUpperBound ({1}).", this.timeSinceLastCompletionLowerBound, this.timeSinceLastCompletionUpperBound));
			}
			if (this.timeSinceLastChangeLowerBound > this.timeSinceLastChangeUpperBound)
			{
				throw new ArgumentException(string.Format("The value of the parameter NumberOfMinutesSinceLastChangeLowerBound ({0}) should be less than the value of the parameter NumberOfMinutesSinceLastChangeUpperBound ({1}).", this.timeSinceLastChangeLowerBound, this.timeSinceLastChangeUpperBound));
			}
		}

		private void Log(string message, params object[] args)
		{
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format("{0}: {1}{2}", DateTime.UtcNow, string.Format(message, args), Environment.NewLine);
		}

		private ServiceCookieFilter GetServiceCookieFilter()
		{
			if (this.serviceCookieFilter == null)
			{
				this.serviceCookieFilter = new ServiceCookieFilter
				{
					LastUpdatedCutoffThreshold = new DateTime?(DateTime.UtcNow.Subtract(this.timeSinceLastChangeLowerBound)),
					LastCompletedCutoffThreshold = new DateTime?(DateTime.UtcNow.Subtract(this.timeSinceLastCompletionLowerBound))
				};
			}
			return this.serviceCookieFilter;
		}

		private static readonly Guid SyncSessionCallerId = new Guid("B7F98959-0576-4863-AF34-28294172015B");

		private ServiceCookieFilter serviceCookieFilter;

		private TimeSpan timeSinceLastCompletionLowerBound;

		private TimeSpan timeSinceLastCompletionUpperBound;

		private TimeSpan timeSinceLastChangeLowerBound;

		private TimeSpan timeSinceLastChangeUpperBound;

		private SyncSession session;
	}
}
