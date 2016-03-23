using System.Collections.Generic;
using Jarvis_Phase3.Models;
using FirebaseSharp.Portable;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Jarvis_Phase3.Models
{
    public class SmokeCoAlarmVMRepo
    {
        const string ACCESS_TOKEN = "c.QY4JkcdwELewWkIDfbgCm2WSEHlaKSvI6g6dpWVOf7levs96rMRByP4xRQksCJUfxrSgYKPwiUKzj1OcgIad2nxerddqp4QvMleuC55br637xaGnychVSl4yMUoQBoWI8uFg1dI9uiK2hZ49";
        // WILLR RETURN A LIST OF ALL THERMOSTATS WITH THIER NAME, STATE etc...
        public async Task<IEnumerable<SmokeCoAlarmVM>> GetAlarms()
        {
            var url = "https://developer-api.nest.com";
            var fb = new Firebase(url, ACCESS_TOKEN);
            dynamic devices = await fb.GetAsync("devices/smoke_co_alarms"); // will return a string
            dynamic devicesJSON = JObject.Parse(devices);

            List<SmokeCoAlarmVM> smokecoalarms = new List<SmokeCoAlarmVM>();
            foreach (dynamic device in devicesJSON)
            {
                // can use this to access properties of thermostat
                // i.e. device.First.humidity;
                var smokecoalarmJSON = device.First;
                SmokeCoAlarmVM smokecoalarm = new SmokeCoAlarmVM();

                smokecoalarm.Device_Id = smokecoalarmJSON.device_id;
                smokecoalarm.Name_Long = smokecoalarmJSON.name_long;
                smokecoalarm.Is_Online = smokecoalarmJSON.is_online;
                smokecoalarm.Smoke_Alarm_State= smokecoalarmJSON.smoke_alarm_state;
                smokecoalarm.Co_Alarm_State = smokecoalarmJSON.co_alarm_state;

                smokecoalarms.Add(smokecoalarm);
            }

            return smokecoalarms;

        }
    }
}