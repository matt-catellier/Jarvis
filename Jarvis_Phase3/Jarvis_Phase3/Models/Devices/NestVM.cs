using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jarvis_Phase3.Models;

namespace Jarvis_Phase3.Models
{
    public class NestVM
    {
        public IEnumerable<CameraVM> cameraVMs;
        public IEnumerable<ThermostatVM> thermostatVMs;
        public IEnumerable<SmokeCoAlarmVM> smokeCoAlarmVMs;

        public NestVM(){}

        public NestVM(IEnumerable<CameraVM> cams, IEnumerable<ThermostatVM> therms, IEnumerable<SmokeCoAlarmVM> alarms)
        {
            cameraVMs = cams;
            thermostatVMs = therms;
            smokeCoAlarmVMs = alarms;
        }
    }
}