using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using System.Web.Http.Cors;

namespace Jarvis_Phase3.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DevicesController : ApiController
    {
        // GET: api/Devices
        public IHttpActionResult GetDevices()
        {
            JarvisEntities db = new JarvisEntities();

            var devices = db.Devices.Select(d => new
            {
                category = d.category,
                provider = d.provider
            }).ToList();


            JArray stoArray = (JArray)JToken.FromObject(devices);
            dynamic obj = new JObject();
            obj.devices = stoArray;

            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }

        // GET: api/Devices/5
        [ResponseType(typeof(Device))]
        public IHttpActionResult GetDevice(int id)
        {
            JarvisEntities db = new JarvisEntities();
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
        }

        protected override void Dispose(bool disposing)
        {
            JarvisEntities db = new JarvisEntities();
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DeviceExists(int id)
        {
            JarvisEntities db = new JarvisEntities();
            return db.Devices.Count(e => e.deviceID == id) > 0;
        }
    }
}