namespace Jarvis_Phase3.Models
{
    public class ThermostatVM
    {
        public string Current_Temperature { get; set; }
        public string Target_Temperature { get; set; }
        public string Name_Long { get; set; }
        public string Device_Id { get; set; }

        public ThermostatVM()
        {

        }
        public ThermostatVM(string tempeture)
        {
            this.Current_Temperature = tempeture;
        }
        public ThermostatVM(string name, string tempeture)
        {
            this.Current_Temperature = tempeture;
            this.Name_Long = name;
        }

        public ThermostatVM(string device_id, string name, string curr_temp, string tar_temp)
        {
            this.Device_Id = device_id;
            this.Current_Temperature = curr_temp;
            this.Target_Temperature = tar_temp;
            this.Name_Long = name;
        }
        public override string ToString()
        {
            return "Device Id: " + this.Device_Id + ", Name: " + this.Name_Long + ", Current Temperature: " + this.Current_Temperature + ", Target temperature: " + this.Target_Temperature;
        }
    }
}