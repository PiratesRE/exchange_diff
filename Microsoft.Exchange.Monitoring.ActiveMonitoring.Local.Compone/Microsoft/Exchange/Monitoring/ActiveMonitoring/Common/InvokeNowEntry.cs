using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	[Serializable]
	public class InvokeNowEntry : IPersistence
	{
		public LocalDataAccessMetaData LocalDataAccessMetaData { get; set; }

		public string TypeName { get; set; }

		public string AssemblyPath { get; set; }

		public string MonitorIdentity { get; set; }

		public Guid Id { get; set; }

		public string PropertyBag { get; set; }

		public string ExtensionAttributes { get; set; }

		public DateTime RequestTime { get; set; }

		public InvokeNowState State { get; set; }

		public InvokeNowResult Result { get; set; }

		public string ErrorMessage { get; set; }

		public string WorkDefinitionId { get; set; }

		public void Initialize(Dictionary<string, string> propertyBag, LocalDataAccessMetaData metaData)
		{
			this.LocalDataAccessMetaData = metaData;
			this.SetProperties(propertyBag);
		}

		public void SetProperties(Dictionary<string, string> propertyBag)
		{
			this.Id = Guid.Parse(this.GetStringProperty(propertyBag, "Id"));
			this.TypeName = this.GetStringProperty(propertyBag, "TypeName");
			this.AssemblyPath = this.GetStringProperty(propertyBag, "AssemblyPath");
			this.MonitorIdentity = this.GetStringProperty(propertyBag, "MonitorIdentity");
			this.PropertyBag = this.GetStringProperty(propertyBag, "PropertyBag");
			this.ExtensionAttributes = this.GetStringProperty(propertyBag, "ExtensionAttributes");
			this.RequestTime = this.GetDateTimeProperty(propertyBag, "RequestTime");
			this.State = this.GetEnumProperty<InvokeNowState>(propertyBag, "State");
			this.Result = this.GetEnumProperty<InvokeNowResult>(propertyBag, "Result");
			this.ErrorMessage = this.GetStringProperty(propertyBag, "ErrorMessage");
			this.WorkDefinitionId = this.GetStringProperty(propertyBag, "WorkDefinitionId");
		}

		public void Write(Action<IPersistence> preWriteHandler = null)
		{
			this.GetCrimsonEventToUse();
			ManagedAvailabilityCrimsonEvents.InvokeNowRequest.Log<Guid, string, string, string, string, string, string, string, string, string, string>(this.Id, this.TypeName, this.AssemblyPath, this.MonitorIdentity, this.PropertyBag, this.ExtensionAttributes, this.ToUniversalSortableTimeString(this.RequestTime), this.State.ToString(), this.RequestTime.ToString(), this.ErrorMessage, this.WorkDefinitionId);
		}

		private ManagedAvailabilityCrimsonEvent GetCrimsonEventToUse()
		{
			ManagedAvailabilityCrimsonEvent result = null;
			if (this.State == InvokeNowState.DefinitionUploadStarted)
			{
				result = ManagedAvailabilityCrimsonEvents.InvokeNowDefinitionUploadStarted;
			}
			if (this.State == InvokeNowState.DefinitionUploadFinished)
			{
				if (this.Result == InvokeNowResult.Succeeded)
				{
					result = ManagedAvailabilityCrimsonEvents.InvokeNowDefinitionUploadSucceeded;
				}
				else
				{
					result = ManagedAvailabilityCrimsonEvents.InvokeNowDefinitionUploadFailed;
				}
			}
			if (this.State == InvokeNowState.MonitorInvokeFinished)
			{
				if (this.Result == InvokeNowResult.Succeeded)
				{
					result = ManagedAvailabilityCrimsonEvents.InvokeNowSucceeded;
				}
				else
				{
					result = ManagedAvailabilityCrimsonEvents.InvokeNowFailed;
				}
			}
			return result;
		}

		private string ToUniversalSortableTimeString(DateTime dateTime)
		{
			return dateTime.ToUniversalTime().ToString("o");
		}

		private string GetStringProperty(Dictionary<string, string> propertyBag, string propertyName)
		{
			string text = null;
			if (propertyBag.TryGetValue(propertyName, out text))
			{
				text = CrimsonHelper.NullDecode(text);
			}
			return text;
		}

		private DateTime GetDateTimeProperty(Dictionary<string, string> propertyBag, string propertyName)
		{
			DateTime result = DateTime.MinValue;
			string text;
			if (propertyBag.TryGetValue(propertyName, out text) && !string.IsNullOrEmpty(text))
			{
				result = DateTime.Parse(text);
			}
			return result;
		}

		private T GetEnumProperty<T>(Dictionary<string, string> propertyBag, string propertyName)
		{
			T result = default(T);
			string value;
			if (propertyBag.TryGetValue(propertyName, out value) && !string.IsNullOrEmpty(value))
			{
				result = (T)((object)Enum.Parse(typeof(T), value));
			}
			return result;
		}
	}
}
