using System.Collections.Generic;
using Jarvis_Phase3.Models;
using FirebaseSharp.Portable;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Jarvis_Phase3.Models
{
    public class CameraVMRepo
    {
        const string ACCESS_TOKEN = "c.QY4JkcdwELewWkIDfbgCm2WSEHlaKSvI6g6dpWVOf7levs96rMRByP4xRQksCJUfxrSgYKPwiUKzj1OcgIad2nxerddqp4QvMleuC55br637xaGnychVSl4yMUoQBoWI8uFg1dI9uiK2hZ49";


        // WILLR RETURN A LIST OF ALL THERMOSTATS WITH THIER NAME, STATE etc...
        public async Task<IEnumerable<CameraVM>> GetCameras()
        {
            var url = "https://developer-api.nest.com";
            var fb = new Firebase(url, ACCESS_TOKEN);
            dynamic devices = await fb.GetAsync("devices/cameras"); // will return a string
            dynamic devicesJSON = JObject.Parse(devices);

            List<CameraVM> cameras = new List<CameraVM>();
            foreach (dynamic device in devicesJSON)
            {
                // can use this to access properties of thermostat
                // i.e. device.First.humidity;
                var cameraJSON = device.First;
                CameraVM camera = new CameraVM();

                camera.Device_Id = cameraJSON.device_id;
                camera.Name_Long = cameraJSON.name_long;
                camera.Is_Online = cameraJSON.is_online;
                camera.Is_Streaming = cameraJSON.is_streaming;

                cameras.Add(camera);
            }

            return cameras;

        }
    }
}