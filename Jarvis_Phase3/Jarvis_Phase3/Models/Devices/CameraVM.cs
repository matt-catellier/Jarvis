namespace Jarvis_Phase3.Models
{
    public class CameraVM
    {
        public bool Is_Streaming { get; set; }
        public bool Is_Online { get; set; }
        public string Name_Long { get; set; }
        public string Device_Id { get; set; }
        public CameraVM() { }

        public CameraVM(string device_id, string name, bool is_online, bool is_streaming)
        {
            this.Device_Id = device_id;
            this.Is_Online = is_online;
            this.Is_Streaming = is_streaming;
            this.Name_Long = name;
        }
        public override string ToString()
        {
            return "Device Id: " + Device_Id + ", Name: " + Name_Long + ", Online: " + Is_Online + ", Streaming: " + Is_Streaming;
        }
    }
}