using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FirebaseSharp.Portable;
using Jarvis_Phase3.Models;
using System.Threading.Tasks;

namespace Jarvis_Phase3.Models
{
    public class ThermostatVMRepo
    {
        const string ACCESS_TOKEN = "c.QY4JkcdwELewWkIDfbgCm2WSEHlaKSvI6g6dpWVOf7levs96rMRByP4xRQksCJUfxrSgYKPwiUKzj1OcgIad2nxerddqp4QvMleuC55br637xaGnychVSl4yMUoQBoWI8uFg1dI9uiK2hZ49";

        // WILLR RETURN A LIST OF ALL THERMOSTATS WITH THIER NAME, STATE etc...
        public async Task<IEnumerable<ThermostatVM>> GetThermostats()
        {
            var url = "https://developer-api.nest.com";
            var fb = new Firebase(url, ACCESS_TOKEN);
            dynamic devices = await fb.GetAsync("devices/thermostats"); // will return a string
            dynamic devicesJSON = JObject.Parse(devices);

            List<ThermostatVM> thermostats = new List<ThermostatVM>();
            foreach(dynamic device in devicesJSON)
            {
                // can use this to access properties of thermostat
                // i.e. device.First.humidity;
                var thermostatJSON = device.First;            
                ThermostatVM thermostat = new ThermostatVM();

                thermostat.Device_Id = thermostatJSON.device_id; 
                thermostat.Name_Long = thermostatJSON.name_long;
                thermostat.Current_Temperature = thermostatJSON.ambient_temperature_c; 
                thermostat.Target_Temperature = thermostatJSON.target_temperature_c;

                thermostats.Add(thermostat);
            }

            return thermostats;

        }



        public async Task<ThermostatVM> GetThermostat()
        {
            var url = "https://developer-api.nest.com";
            var fb = new Firebase(url, ACCESS_TOKEN);
            dynamic devices = await fb.GetAsync("devices");

            var jsonParsed = JsonConvert.DeserializeObject<dynamic>(devices);
            var thermostats = jsonParsed.thermostats;
            string dev_id = jsonParsed["thermostats"]["is8MQBKrH-h-UWxetdv7-o-BBzEt2ynq"].device_id;
            string name = jsonParsed["thermostats"]["is8MQBKrH-h-UWxetdv7-o-BBzEt2ynq"].name_long;
            string curr_temp = jsonParsed["thermostats"]["is8MQBKrH-h-UWxetdv7-o-BBzEt2ynq"].ambient_temperature_c;
            string tar_temp = jsonParsed["thermostats"]["is8MQBKrH-h-UWxetdv7-o-BBzEt2ynq"].target_temperature_c;
            ThermostatVM myThermostat = new ThermostatVM(dev_id, name, curr_temp, tar_temp);
            return myThermostat;
        }
    }
}