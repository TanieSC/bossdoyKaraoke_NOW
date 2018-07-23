using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace bossdoyKaraoke_NOW.Adb
{
    public class USBControl : IDisposable
    {
        // used for monitoring plugging and unplugging of USB devices.
        private ManagementEventWatcher watcherAttach;
        private ManagementEventWatcher watcherRemove;

        public USBControl()
        {
            // Add USB plugged event watching
            watcherAttach = new ManagementEventWatcher();
            watcherAttach.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
            watcherAttach.Query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            watcherAttach.Start();

            // Add USB unplugged event watching
            watcherRemove = new ManagementEventWatcher();
            watcherRemove.EventArrived += new EventArrivedEventHandler(watcher_EventRemoved);
            watcherRemove.Query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            watcherRemove.Start();
        }

        /// <summary>
        /// Used to dispose of the USB device watchers when the USBControl class is disposed of.
        /// </summary>
        public void Dispose()
        {
            watcherAttach.Stop();
            watcherRemove.Stop();
            //Thread.Sleep(1000);
            watcherAttach.Dispose();
            watcherRemove.Dispose();
            //Thread.Sleep(1000);
        }

        void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            ManagementObjectCollection collection;

            string deviceQuery = "SELECT * FROM Win32_PnPEntity WHERE Name Like '%Android%'";
            using (var searcher = new ManagementObjectSearcher(deviceQuery))
            {
                bool f = searcher.Get().Count >= 1 ? true : false;
                Debug.WriteLine(searcher.Get().Count);
            }
            // bool ret = searcher.Get().Count() >= 1 ? true : false;
            /// collection = searcher.Get();



            // foreach (var device in collection)
            // {

            //    Debug.WriteLine(device.GetPropertyValue("name"));
            //    Debug.WriteLine(device.GetPropertyValue("deviceID"));
            //    Debug.WriteLine(device.GetPropertyValue("pnpDeviceID"));
            //    Debug.WriteLine(device.GetPropertyValue("description"));
            // };

            // collection.Dispose();

        }

        void watcher_EventRemoved(object sender, EventArrivedEventArgs e)
        {
            Debug.WriteLine("watcher_EventRemoved");
        }

        ~USBControl()
        {
            this.Dispose();
        }


    }
}
