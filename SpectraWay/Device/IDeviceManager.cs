namespace SpectraWay.Device
{
    public interface IDeviceManager<out T> where T: class
    {
        T GetDevice(string type);
    }
}