﻿using DeviceManagementLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace DeviceManagementModule
{
    [Cmdlet(VerbsLifecycle.Uninstall, "Device", DefaultParameterSetName = "ByInstanceId", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    [OutputType(typeof(Device))]
    public class UninstallDeviceCmdlet : PSCmdlet
    {
        [Parameter(Position = 1, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "ByInstanceId")]
        public string InstanceId { get; set; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "ByName")]
        public string Name { get; set; }

        [Parameter(Position = 2, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public DeviceManagementLib.Win32.DeviceClass? Class { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            IEnumerable<Device> devices = null;

            try
            {
                if (!string.IsNullOrEmpty(InstanceId) && @Class.HasValue)
                {
                    devices = DeviceManagementLib.DeviceManagementLib.GetDevices(InstanceId, @Class);
                }
                else if (!string.IsNullOrEmpty(InstanceId) && !@Class.HasValue)
                {
                    devices = DeviceManagementLib.DeviceManagementLib.GetDevices(InstanceId);
                }

                if (!string.IsNullOrEmpty(Name))
                    devices = devices.Where(d => d.Name == Name);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ErrorGettingDevices", ErrorCategory.ReadError, null));
            }

            foreach (var device in devices)
            {
                try
                {
                    if (ShouldProcess(device.Name, "Remove device"))
                    {
                        DeviceManagementLib.DeviceManagementLib.RemoveDevice(device.InstanceId);
                    }
                }
                catch(Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "ErrorUninstallingDevices", ErrorCategory.WriteError, null));
                }
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }
}