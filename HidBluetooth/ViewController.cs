using CoreBluetooth;
using Foundation;
using System;
using System.Diagnostics;
using System.Linq;
using UIKit;

namespace HidBluetooth
{
  public partial class ViewController : UIViewController
  {
    private const int ScanTime = 5000;
    private const string DeviceName = "Seos";//"LazyBone";//"Seos";
    private BluetoothService bluetoothService;
    private bool alreadyScanned;
    private bool alreadyDiscovered;
    private CBPeripheral BTRelayDiscovered;
    private CBCharacteristic[] BTCharacteristics;
    public ViewController(IntPtr handle) : base(handle)
    {
    }

    public override void ViewDidLoad()
    {
      base.ViewDidLoad();
      this.alreadyScanned = false;
      this.alreadyDiscovered = false;
      this.bluetoothService = new BluetoothService();
    }
    public override async void ViewWillAppear(bool animated)
    {

      base.ViewWillAppear(animated);
      if (bluetoothService != null)
      {
        bluetoothService.StateChanged += StateChanged;
        bluetoothService.DiscoveredDevice += DiscoveredDevice;
      }
    }

    public override void DidReceiveMemoryWarning()
    {
      base.DidReceiveMemoryWarning();
      // Release any cached data, images, etc that aren't in use.
    }
    private async void TriggerReader()
    {
      BeginInvokeOnMainThread(async () =>
      {
        try
        {
          NSData DataToWrite;
          byte[] TestArray = {
                            0x63, 0x90  };
          DataToWrite = NSData.FromArray(TestArray);
          BTRelayDiscovered?.WriteValue(DataToWrite, BTCharacteristics[0], CBCharacteristicWriteType.WithoutResponse);//8B143FDF-7A1E-5588-9253-D3D1CC93FD66
          if (BTRelayDiscovered != null)
          {
            //MethodThatShowsAlertView("Door Command", "Unlocking Door", 3000);
          }
          else
          {
            //MethodThatShowsAlertView("Door Command", "Failed To Unlock Door", 3000);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
          Console.WriteLine("NotWorking");
        }
      });
    }

    private async void DiscoveredDevice(object sender, CBPeripheral peripheral)
    {
      if (peripheral != null && peripheral.Name != null)
      {
        if (!this.alreadyDiscovered && peripheral.Name.StartsWith(DeviceName, StringComparison.InvariantCulture))
        {
          //CBCharacteristic cBCharacteristic = null;
          BTCharacteristics = null;
          NSData value = null;
          try
          {
            this.alreadyDiscovered = true;
            await this.bluetoothService.ConnectTo(peripheral);
            //Console.WriteLine(peripheral.UUID);
            var service = await this.bluetoothService.GetService(peripheral, "2A4A");//"FFE0");//"FFE0");//"8B143FDF-7A1E-5588-9253-D3D1CC93FD66"
            if (service != null)
            {
              BTCharacteristics = await this.bluetoothService.GetCharacteristics(peripheral, service, ScanTime);

              foreach (var characteristic in BTCharacteristics)
              {
                value = await this.bluetoothService.ReadValue(peripheral, characteristic);
                Debug.WriteLine($"{characteristic.UUID.Description} = {value}");
              }
            }
          }
          catch (Exception ex)
          {
            Debug.WriteLine(ex);
          }
          finally
          {
            BTRelayDiscovered = peripheral;
          }
        }
      }
    }
    private async void StateChanged(object sender, CBCentralManagerState state)
    {
      if (!this.alreadyScanned && state == CBCentralManagerState.PoweredOn)
      {
        try
        {
          this.alreadyScanned = true;
          var connectedDevice = this.bluetoothService.GetConnectedDevices("FFE0")
                      ?.FirstOrDefault(x => x.Name.StartsWith(DeviceName, StringComparison.InvariantCulture));

          if (connectedDevice != null)
          {
            this.DiscoveredDevice(this, connectedDevice);
          }
          else
          {
            await this.bluetoothService.Scan(ScanTime);
          }
        }
        catch (Exception ex)
        {
          Debug.WriteLine(ex);
        }
      }
    }

    partial void TestCmd_TouchUpInside(UIButton e)
    {
      TriggerReader();
    }
  }
}