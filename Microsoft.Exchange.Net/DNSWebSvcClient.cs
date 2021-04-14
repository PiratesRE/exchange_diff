using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.BDM.Pets.DNSManagement;
using Microsoft.BDM.Pets.SharedLibrary.Enums;
using www.microsoft.com.bdm.pets;

[GeneratedCode("System.ServiceModel", "3.0.0.0")]
[DebuggerStepThrough]
public class DNSWebSvcClient : ClientBase<IDNSWebSvc>, IDNSWebSvc
{
	public DNSWebSvcClient()
	{
	}

	public DNSWebSvcClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public DNSWebSvcClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public DNSWebSvcClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public DNSWebSvcClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public Guid AddZone(string domainName)
	{
		return base.Channel.AddZone(domainName);
	}

	public void DeleteZone(Guid zoneGuid)
	{
		base.Channel.DeleteZone(zoneGuid);
	}

	public void DeleteZoneByDomainName(string domainName)
	{
		base.Channel.DeleteZoneByDomainName(domainName);
	}

	public void UpdateZone(Guid zoneGuid, bool isDisabled)
	{
		base.Channel.UpdateZone(zoneGuid, isDisabled);
	}

	public Guid AddResourceRecord(Guid zoneGuid, string domainName, int TTL, ResourceRecordType recordType, string value, bool deleteExisting)
	{
		return base.Channel.AddResourceRecord(zoneGuid, domainName, TTL, recordType, value, deleteExisting);
	}

	public Guid[] AddResourceRecordByDomainName(string domainName, ResourceRecord[] records, bool[] deleteExisting)
	{
		return base.Channel.AddResourceRecordByDomainName(domainName, records, deleteExisting);
	}

	public void DeleteResourceRecord(Guid resourceRecordId)
	{
		base.Channel.DeleteResourceRecord(resourceRecordId);
	}

	public void UpdateResourceRecord(Guid resourceRecordId, int TTL, string value)
	{
		base.Channel.UpdateResourceRecord(resourceRecordId, TTL, value);
	}

	public bool IsDomainAvailable(string domainName)
	{
		return base.Channel.IsDomainAvailable(domainName);
	}

	public Zone GetZone(Guid zoneGuid)
	{
		return base.Channel.GetZone(zoneGuid);
	}

	public Zone GetZoneByDomainName(string domainName)
	{
		return base.Channel.GetZoneByDomainName(domainName);
	}

	public ResourceRecord[] GetResourceRecordsByZone(Guid zoneGuid)
	{
		return base.Channel.GetResourceRecordsByZone(zoneGuid);
	}

	public ResourceRecord[] GetAllResourceRecordsByDomainName(string domainName)
	{
		return base.Channel.GetAllResourceRecordsByDomainName(domainName);
	}

	public ResourceRecord[] GetResourceRecordsByDomainName(string domainName, ResourceRecordType recordType)
	{
		return base.Channel.GetResourceRecordsByDomainName(domainName, recordType);
	}

	public BDMHeader GetHeader()
	{
		return base.Channel.GetHeader();
	}

	public bool Ping()
	{
		return base.Channel.Ping();
	}
}
