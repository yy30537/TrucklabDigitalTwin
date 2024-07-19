using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace NWH.VehiclePhysics
{
    [DisallowMultipleComponent]
    public class DesktopInputManager : MonoBehaviour
    {
        public enum InputSource { Local, Network }
        public InputSource inputSource = InputSource.Network;


        public enum InputType { Standard, Mouse, MouseSteer }
        public InputType inputType = InputType.Standard;

        public enum VerticalInputType { Standard, ZeroToOne, Composite }
        public VerticalInputType verticalInputType = VerticalInputType.Standard;
        public VehicleChanger vehicleChanger;
        public VehicleController vehicleController;
        public GameObject truck_marker;
        public GameObject trailer_front_marker;
        public GameObject trailer_back_marker;
        private float vertical = 0f;
        private float horizontal = 0f;
        private Vector3 truck_marker_pos;
        private Vector3 trailer_front_marker_pos;
        private Vector3 trailer_back_marker_pos;
        private bool already_atttached = false;



        readonly string host = "0.0.0.0";
        readonly int port = 54320;
        Thread ListenerThread;
        UdpClient clientL = new UdpClient();

        public void stop()
        {

            clientL.Close();

            ListenerThread.Join();
            ListenerThread.Abort();

            clientL = new UdpClient();
            
        }

        public void ListenForMessages()
        {
            try
            {
                while (true)
                {

                    Byte[] bytesL = new Byte[1024];

                    const int SIO_UDP_CONNRESET = -1744830452;
                    try
                    {
                        clientL.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null);
                    }
                    catch
                    {
                        Debug.LogError("SIO_UDP_CONNRESET flag not supported on this platform!");
                    }
                    clientL.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 0);

                    var remoteEP = new IPEndPoint(IPAddress.Parse(host), port);
                    clientL.Client.Bind(remoteEP);
                    var from = new IPEndPoint(0, 0);
                    float steerfromRhapsody = 0;
                    float accelfromRhapsody = 0;
                    while (true)
                    {
                        string rx_line = System.Text.Encoding.Default.GetString(clientL.Receive(ref from));
                        string[] rx_array = rx_line.Split(new string[] { "," }, StringSplitOptions.None);
                        if (rx_array.Length == 2)
                        {
                            if (float.TryParse(rx_array[0], out steerfromRhapsody))
                            {
                                horizontal = steerfromRhapsody;
                            }
                            if (float.TryParse(rx_array[1], out accelfromRhapsody))
                            {
                                vertical = accelfromRhapsody;
                            }
                        }

                        string truck_marker_pos_str =
                            String.Format("{0:F4}", truck_marker_pos.x) + "," +
                            String.Format("{0:F4}", truck_marker_pos.y) + "," +
                            String.Format("{0:F4}", truck_marker_pos.z);
                        string trailer_back_marker_pos_str =
                            String.Format("{0:F4}", trailer_back_marker_pos.x) + "," +
                            String.Format("{0:F4}", trailer_back_marker_pos.y) + "," +
                            String.Format("{0:F4}", trailer_back_marker_pos.z);
                        string trailer_front_marker_pos_str =
                            String.Format("{0:F4}", trailer_front_marker_pos.x) + "," +
                            String.Format("{0:F4}", trailer_front_marker_pos.y) + "," +
                            String.Format("{0:F4}", trailer_front_marker_pos.z);
                        string pos_str = truck_marker_pos_str + "," + trailer_front_marker_pos_str + "," + trailer_back_marker_pos_str;
                        var str_to_send = String.Format("{0:F4}", vehicleController.ForwardVelocity) + "," + pos_str;
                        var byte_array = Encoding.GetEncoding("UTF-8").GetBytes(str_to_send);
                        
                        clientL.Send(byte_array, str_to_send.Length, from.Address.ToString(), from.Port);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private bool TryGetButtonDown(string buttonName, KeyCode altKey)
        {
            try
            {
                return Input.GetButtonDown(buttonName);
            }
            catch
            {
                Debug.LogWarning(buttonName + " input binding missing, falling back to default. Check Input section in manual for more info.");
                return Input.GetKeyDown(altKey);
            }
        }

        private bool TryGetButton(string buttonName, KeyCode altKey)
        {
            try
            {
                return Input.GetButton(buttonName);
            }
            catch
            {
                Debug.LogWarning(buttonName + " input binding missing, falling back to default. Check Input section in manual for more info.");
                return Input.GetKey(altKey);
            }
        }

        private void Start()
        {
            ListenerThread = new Thread(() => ListenForMessages());
            ListenerThread.Start();
            vehicleController = GetComponent<VehicleController>();
            
            
        }


        void Update()
        {
            if (vehicleChanger != null)
            {
                vehicleController = vehicleChanger.ActiveVehicleController;
            }

            if (vehicleController.trailer.attached == false && already_atttached == false)
            {
                vehicleController.input.trailerAttachDetach = true;
                already_atttached = true;
            }

            if (vehicleController == null) return;

            try
            {
                //vertical = 0f;
                //horizontal = 0f;

                if (vehicleController == null) return;

                // Manual shift
                if (TryGetButtonDown("ShiftUp", KeyCode.R))
                    vehicleController.input.ShiftUp = true;

                if (TryGetButtonDown("ShiftDown", KeyCode.F))
                    vehicleController.input.ShiftDown = true;

                if (vehicleController.transmission.transmissionType == Transmission.TransmissionType.Manual)
                {
                    try
                    {
                        if (TryGetButtonDown("FirstGear", KeyCode.Alpha1))
                            vehicleController.transmission.ShiftInto(1);
                        else if (TryGetButtonDown("SecondGear", KeyCode.Alpha2))
                            vehicleController.transmission.ShiftInto(2);
                        else if (TryGetButtonDown("ThirdGear", KeyCode.Alpha3))
                            vehicleController.transmission.ShiftInto(3);
                        else if (TryGetButtonDown("FourthGear", KeyCode.Alpha4))
                            vehicleController.transmission.ShiftInto(4);
                        else if (TryGetButtonDown("FifthGear", KeyCode.Alpha5))
                            vehicleController.transmission.ShiftInto(5);
                        else if (TryGetButtonDown("SixthGear", KeyCode.Alpha6))
                            vehicleController.transmission.ShiftInto(6);
                        else if (TryGetButtonDown("SeventhGear", KeyCode.Alpha7))
                            vehicleController.transmission.ShiftInto(7);
                        else if (TryGetButtonDown("EightGear", KeyCode.Alpha8))
                            vehicleController.transmission.ShiftInto(8);
                        else if (TryGetButtonDown("NinthGear", KeyCode.Alpha9))
                            vehicleController.transmission.ShiftInto(9);
                        else if (TryGetButtonDown("Neutral", KeyCode.Alpha0))
                            vehicleController.transmission.ShiftInto(0);
                        else if (TryGetButtonDown("Reverse", KeyCode.Minus))
                            vehicleController.transmission.ShiftInto(-1);
                    }
                    catch
                    {
                        Debug.LogWarning("Some of the gear changing inputs might not be assigned in the input manager. Direct gear shifting " +
                            "will not work.");
                    }
                }


                truck_marker_pos = truck_marker.transform.position;
                trailer_front_marker_pos = trailer_front_marker.transform.position;
                trailer_back_marker_pos = trailer_back_marker.transform.position;




                if (inputSource == InputSource.Network)
                {
                    vehicleController.input.Horizontal = horizontal;
                    vehicleController.input.Vertical = vertical;
                }
                else
                {
                    // Horizontal axis
                    if (inputType == InputType.Standard)
                    {
                        horizontal = Input.GetAxis("Horizontal");
                    }
                    else
                    {
                        horizontal = Mathf.Clamp(VehicleController.GetMouseHorizontal(), -1f, 1f);
                    }
                    vehicleController.input.Horizontal = horizontal;


                    // Vertical axis
                    if (inputType == InputType.Standard)
                    {
                        if (verticalInputType == VerticalInputType.Standard)
                        {
                            vertical = Input.GetAxisRaw("Vertical");
                        }
                        else if (verticalInputType == VerticalInputType.ZeroToOne)
                        {
                            vertical = (Mathf.Clamp01(Input.GetAxisRaw("Vertical")) - 0.5f) * 2f;
                        }
                        else if (verticalInputType == VerticalInputType.Composite)
                        {
                            float accelerator = Mathf.Clamp01(Input.GetAxisRaw("Accelerator"));
                            float brake = Mathf.Clamp01(Input.GetAxisRaw("Brake"));
                            vertical = accelerator - brake;
                        }
                    }
                    else if (inputType == InputType.Mouse)
                    {
                        vertical = Mathf.Clamp(VehicleController.GetMouseVertical(), -1f, 1f);
                    }
                    else
                    {
                        if (Input.GetMouseButton(0))
                            vertical = 1f;
                        else if (Input.GetMouseButton(1))
                            vertical = -1f;
                    }
                    vehicleController.input.Vertical = vertical;
                }




                // Engine start/stop
                if (TryGetButtonDown("EngineStartStop", KeyCode.E))
                {
                    vehicleController.engine.Toggle();
                }

                // Handbrake
                try
                {
                    vehicleController.input.Handbrake = Input.GetAxis("Handbrake");
                }
                catch
                {
                    Debug.LogWarning("Handbrake axis not set up, falling back to default (RegionProduct).");
                    vehicleController.input.Handbrake = Input.GetKey(KeyCode.Space) ? 1f : 0f;
                }

                // Clutch
                if (!vehicleController.transmission.automaticClutch)
                {
                    try
                    {
                        vehicleController.input.Clutch = Input.GetAxis("Clutch");
                    }
                    catch
                    {
                        Debug.LogError("Clutch is set to manual but the required axis 'Clutch' is not set. " +
                            "Please set the axis inside input manager to use this feature.");
                        vehicleController.transmission.automaticClutch = true;
                    }
                }

                // Lights
                if (TryGetButtonDown("LeftBlinker", KeyCode.Z))
                {
                    vehicleController.input.leftBlinker = !vehicleController.input.leftBlinker;
                    if (vehicleController.input.leftBlinker) vehicleController.input.rightBlinker = false;
                }
                if (TryGetButtonDown("RightBlinker", KeyCode.X))
                {
                    vehicleController.input.rightBlinker = !vehicleController.input.rightBlinker;
                    if (vehicleController.input.rightBlinker) vehicleController.input.leftBlinker = false;
                }
                if (TryGetButtonDown("Lights", KeyCode.L)) vehicleController.input.lowBeamLights = !vehicleController.input.lowBeamLights;
                if (TryGetButtonDown("FullBeamLights", KeyCode.K)) vehicleController.input.fullBeamLights = !vehicleController.input.fullBeamLights;
                if (TryGetButtonDown("HazardLights", KeyCode.J))
                {
                    vehicleController.input.hazardLights = !vehicleController.input.hazardLights;
                    vehicleController.input.leftBlinker = false;
                    vehicleController.input.rightBlinker = false;
                }

                // Horn
                vehicleController.input.horn = TryGetButton("Horn", KeyCode.H);

                // Raise trailerTransform flag if trailerTransform attach detach button pressed.
                if (TryGetButtonDown("TrailerAttachDetach", KeyCode.T))
                    vehicleController.input.trailerAttachDetach = true;

                // Manual flip over
                if (vehicleController.flipOver.manual)
                {
                    try
                    {
                        // Set manual flip over flag to true if vehicleTransform is flipped over, otherwise ignore
                        if (Input.GetButtonDown("FlipOver") && vehicleController.flipOver.flippedOver)
                            vehicleController.input.flipOver = true;
                    }
                    catch
                    {
                        Debug.LogError("Flip over is set to manual but 'FlipOver' input binding is not set. Either disable manual flip over or set 'FlipOver' binding.");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("One or more of the required inputs has not been set. Check NWH IVirtualEntityProduct Physics README for more info or add the binding inside Unity input manager.");
                Debug.LogWarning(e);
            }
        }
    }
}

