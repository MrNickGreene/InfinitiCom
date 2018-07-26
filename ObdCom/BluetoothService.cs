using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acr.Collections;
using Android.Accounts;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Java.Util;

namespace ObdCom
{
    public class BluetoothService : Service
    {
        private readonly BluetoothAdapter _bluetoothAdapter;
        private BluetoothDevice _bluetoothDevice;
        private BluetoothSocket _socket;
        private UUID _uuid;
        private string pairedMacAddress = "";

        public BluetoothService()
        {
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (_bluetoothAdapter == null)
            {
                Log.Error("Error - bluetooth", "Bluetooth not supported - initial");
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            var bluetoothIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
            base.StartService(bluetoothIntent);

            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnCreate()
        {
            if (_bluetoothAdapter == null)
            {
                Log.Error("Error - bluetooth", "Bluetooth not supported - service start");
                throw new NetworkErrorException("Bluetooth not supported");
            }

            if (!_bluetoothAdapter.IsEnabled)
            {
                Log.Error("Error - bluetooth", "Bluetooth disabled");
                throw new NetworkErrorException("Bluetooth disabled");
            }
            
            base.OnCreate();
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }
        public override void OnRebind(Intent intent)
        {
            base.OnRebind(intent);
        }
        public override void OnTaskRemoved(Intent rootIntent)
        {
            base.OnTaskRemoved(rootIntent);
        }
        
        public string SendCommand(string message)
        {
            var response = "";
            return response;
        }
        
        public Dictionary<string, string> GetDevices()
        {
            var bondedDevices = new Dictionary<string, string>();
            _bluetoothAdapter.BondedDevices.Each(x => bondedDevices.Add(x.Address, x.Name));
            return bondedDevices;
        }

        public void SetDevice(string address)
        {
            if (address != pairedMacAddress)
            {
                _socket?.Close();

                if (_bluetoothAdapter.BondedDevices.Any(x => x.Address == address))
                {
                    _uuid = new UUID(Int64.MaxValue, 0);
                    pairedMacAddress = address;
                    _bluetoothDevice = _bluetoothAdapter.BondedDevices.First(x => x.Address == pairedMacAddress);
                    _socket = _bluetoothDevice.CreateRfcommSocketToServiceRecord(_uuid);
                }
                else
                {
                    _bluetoothDevice = null;
                    pairedMacAddress = null;
                }
            }
        }
        
        public void Read()
        {
            var buffer = new byte[2048];
            _socket.InputStream.ReadAsync(buffer, 0, buffer.Length);
        }

        public void Send(byte[] message)
        {
            _socket.OutputStream.Write(message);
        }

        public async Task<string> SendAsync(byte[] message)
        {
            await _socket.OutputStream.WriteAsync(message, 0, message.Length);

            return message.ToString();
        }
    }
}