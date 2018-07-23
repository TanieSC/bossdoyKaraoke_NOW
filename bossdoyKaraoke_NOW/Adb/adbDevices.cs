using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace bossdoyKaraoke_NOW.Adb
{
    public class adbDevices
    {
        internal const string DeviceDataRegexString = @"^(?<serial>[a-zA-Z0-9_-]+(?:\s?[\.a-zA-Z0-9_-]+)?(?:\:\d{1,})?)\s+(?<state>device|connecting|offline|unknown|bootloader|recovery|download|unauthorized|host|no permissions)(\s+usb:(?<usb>[^:]+))?(?:\s+product:(?<product>[^:]+))?(\s+model\:(?<model>[\S]+))?(\s+device\:(?<device>[\S]+))?(\s+features:(?<features>[^:]+))?(\s+transport_id:(?<transport_id>[^:]+))?$";
        private static readonly Regex regex = new Regex(DeviceDataRegexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Product { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }

        public static adbDevices CreateDeviceList(string data)
        {
            Match m = regex.Match(data);
            if (m.Success)
            {
                return new adbDevices
                {
                    Model = m.Groups["model"].Value,
                    Product = m.Groups["product"].Value,
                    Name = m.Groups["device"].Value,
                };

            }
            else
            {
                return null;// throw new ArgumentException($"Invalid device list data '{data}'");
            }
        }

    }
}
