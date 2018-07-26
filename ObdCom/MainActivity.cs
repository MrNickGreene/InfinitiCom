using System;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Util;
using AlertDialog = Android.App.AlertDialog;

namespace ObdCom
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private BluetoothService _bluetoothService;
        public string selectedBluetoothAddress;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            _bluetoothService = new BluetoothService();
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            var view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.nav_manage:
                    break;
                case Resource.Id.nav_bluetooth:
                {
                    StartService(new Intent(this, typeof(BluetoothService)));
                    GetBluetoothSelectionAlert().Show();
                    break;
                }
                case Resource.Id.nav_share:
                    break;
                case Resource.Id.nav_send:
                    break;
            }

            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        private AlertDialog.Builder GetBluetoothSelectionAlert()
        {
            var devices = _bluetoothService.GetDevices();

            var selectedDeviceIndex = -1;
            if (selectedBluetoothAddress != null && devices.Keys.Any(x => x == selectedBluetoothAddress))
            {
                selectedDeviceIndex = devices.Keys.ToList().IndexOf(devices.Keys.First(x => x == selectedBluetoothAddress));
            }

            var alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetSingleChoiceItems(devices.Select(x => x.Value).ToArray(), selectedDeviceIndex,
                delegate(object sender, DialogClickEventArgs args)
                {
                    selectedDeviceIndex = args.Which;
                });

            alertDialog.SetTitle("Choose Bluetooth device");
            alertDialog.SetNegativeButton("Cancel", (sender, args) => {});
            alertDialog.SetPositiveButton("Save", (sender, args) =>
            {
                selectedBluetoothAddress = devices.ElementAt(selectedDeviceIndex).Key;
                _bluetoothService.SetDevice(selectedBluetoothAddress);

            });

            return alertDialog;
        }

    }
}

