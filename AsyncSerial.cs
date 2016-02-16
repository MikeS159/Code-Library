#region Licence
//MIT License(MIT)


/*     AsyncSerial.cs Version 4.0        */

/*     Copyright(c) 2015 Mike Simpson      */

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
#endregion


using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;

namespace AutoCal
{

    #region classRegion

    /// <summary>
    /// This class allows a serial port to be set up with different parameters
    /// It contains functions to open and close the port safely
    /// </summary>
    public class AsyncSerial : IDisposable
    {
        SerialPort dotNETPort;
        private string portErrorMessage = null;
        /// <summary>
        /// Constructor for BoardSerialReciver
        /// </summary>
        /// <param name="portName"> Sets the port name of the serial port (string) </param>
        /// <param name="baudRate"> Sets the baud rate of the serial port (int) </param>
        /// <param name="parity"> Currently unused </param>
        /// <param name="dataBits"> Sets the number of data bits of the serial port (int) </param>
        /// <param name="stopBits"> Sets the number of stop bits of the serial port (string) </param>
        public AsyncSerial(string portName, int baudRate, string parity, int dataBits, string stopBits)
        {
            dotNETPort = new SerialPort();
            if (portName != null & parity != null & stopBits != null)
            {
                try
                {
                    switch (stopBits.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "none": dotNETPort.StopBits = StopBits.None; break;
                        case "onepointfive": dotNETPort.StopBits = StopBits.OnePointFive; break;
                        case "two": dotNETPort.StopBits = StopBits.Two; break;
                        default: dotNETPort.StopBits = StopBits.One; break;
                    }

                    switch (parity.ToLower(CultureInfo.CurrentCulture))
                    {
                        case "even": dotNETPort.Parity = System.IO.Ports.Parity.Even; break;
                        case "odd": dotNETPort.Parity = System.IO.Ports.Parity.Odd; break;
                        case "mark": dotNETPort.Parity = System.IO.Ports.Parity.Mark; break;
                        case "space": dotNETPort.Parity = System.IO.Ports.Parity.Space; break;
                        default: dotNETPort.Parity = System.IO.Ports.Parity.None; break;
                    }

                    dotNETPort.PortName = portName;
                    dotNETPort.BaudRate = baudRate;
                    dotNETPort.DataBits = dataBits;
                }
                catch (IOException e)
                {
                    portErrorMessage = e.Message;
                }
                catch (ArgumentNullException e)
                {
                    exceptionCatch(e);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    exceptionCatch(e);
                }
                catch (ArgumentException e)
                {
                    exceptionCatch(e);
                }
                catch (InvalidOperationException e)
                {
                    exceptionCatch(e);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                try
                {
                    dotNETPort.Close();
                }
                catch (IOException e)
                {
                    exceptionCatch(e);
                }
                dotNETPort.Dispose();
            }
            // free native resources if there are any
        }

        /// <summary>
        /// Open the serial port with defined values
        /// Uses try catch to handle errors
        /// </summary>
        public bool StartReceive()
        {
            if (dotNETPort != null)
            {
                try
                {
                    dotNETPort.Open();
                }
                catch (InvalidOperationException e)
                {
                    exceptionCatch(e); return false;
                }
                catch (IOException e)
                {
                    exceptionCatch(e); return false;
                }
                catch (UnauthorizedAccessException e)
                {
                    exceptionCatch(e); return false;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    exceptionCatch(e); return false;
                }
                catch (ArgumentException e)
                {
                    exceptionCatch(e); return false;
                }

                finally
                {
                    dotNETPort.DataReceived += new SerialDataReceivedEventHandler(port_OnDataRecived);    //sets up new event handler
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private void exceptionCatch(Exception e)
        {
            portErrorMessage = "An error occurred while trying to open the serial port " + dotNETPort.PortName.ToString() + "\nPlease check the port settings and try again\n" +
                "Error message - " + e.Message;
        }

        /// <summary>
        /// Checks for null reference and that the port is open, then closes the port
        /// </summary>
        public void StopReceive()
        {
            if (dotNETPort != null)
            {
                if (dotNETPort.IsOpen)
                {
                    dotNETPort.Close();
                }
            }
        }

        /// <summary>
        /// This function can be used to get the port status externally
        /// </summary>
        public bool IsPortOpen
        {
            get
            {
                if (dotNETPort != null)
                {
                    return dotNETPort.IsOpen;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns null if there is no error, or the error message as a string to be displayed to the user
        /// </summary>
        public string CurrentPortError
        {
            get
            {
                return portErrorMessage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendData"></param>
        public bool SendSerialData(string sendData)
        {
            if (IsPortOpen & sendData != null)
            {
                try
                {
                    dotNETPort.Write(sendData);
                    return true;
                }
                catch (InvalidOperationException e)
                {
                    exceptionCatch(e);
                    return false;
                }
                catch (ArgumentNullException e)
                {
                    exceptionCatch(e);
                    return false;
                }
            }
            else
            {
                portNotOpenError();
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendData"></param>
        public bool SendSerialData(byte[] sendData)
        {
            if (IsPortOpen & sendData != null)
            {
                try
                {
                    dotNETPort.Write(sendData, 0, sendData.Length);
                    return true;
                }

                catch (InvalidOperationException e)
                {
                    exceptionCatch(e);
                    return false;
                }
                catch (ArgumentNullException e)
                {
                    exceptionCatch(e);
                    return false;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    exceptionCatch(e);
                    return false;
                }
                catch (ArgumentException e)
                {
                    exceptionCatch(e);
                    return false;
                }
            }
            else
            {
                portNotOpenError();
                return false;
            }
        }

        private void portNotOpenError()
        {
            portErrorMessage = "Serial port is not open";
        }

        /// <summary>
        /// This function executes when data is recived on the serial port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void port_OnDataRecived(object sender, SerialDataReceivedEventArgs e)
        {
            int lengthToRead = dotNETPort.BytesToRead;        // Gets the length of data in the serial port buffer
            byte[] rxBytes = new byte[lengthToRead];    // Creates a byte array long enough to read all bits from the serial port
            dotNETPort.Read(rxBytes, 0, lengthToRead);        // Reads the serial port buffer into rxByteCount
            //PacketReceived((rxBytes, e);
            OnPacketReceived(rxBytes);
        }

        /// <summary>
        /// Sets up packet handler event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="packet"></param>
        //public delegate void PacketReceivedEventHandler(object sender, byte[] packet);

        /// <summary>
        /// 
        /// </summary>
        //public event EventHandler<PacketReceivedEventHandler> OnPacketReceived;
        public event EventHandler<PacketEventArgs> PacketReceived;

        /// <summary>
        /// Sends packets to main form to be processed
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="e"></param>
        public virtual void OnPacketReceived(byte[] packet)
        {
            PacketReceived?.Invoke(this, new PacketEventArgs(packet));
        }
        //public virtual void PacketReceived(byte[] packet, EventArgs e)
        //{
        //    var handler = OnPacketReceived;
        //    if (handler != null)
        //    {
        //        handler.Invoke(this, new PacketReceivedEventHandler(packet));
        //    }
        //    //if (OnPacketReceived != null)
        //    //{
        //    //    OnPacketReceived(this, packet);                
        //    //}
        //}
    }

    public class PacketEventArgs : EventArgs
    {
        private byte[] packet;
        public byte[] Packet
        {
            get
            {
                return packet;
            }
            set
            {
                packet = value;
            }
        }
        public PacketEventArgs(byte[] _packet)
        {
            Packet = _packet;
        }
    }

    #endregion

    #region PortParameters
    /// <summary>
    /// Provides string arrays containing common (or enforced) port parameters which you can use to populate menus in foreach loops etc 
    /// ReadOnlyCollections can be cast to a array using .ToArray()
    /// </summary>
    public static class SerialPortParameters
    {

        private static string[] commonBaudRates = new string[] {"110", "300", "600", "1200", "2400", "4800", "9600", "14400", "19200", "28800", "38400", "56000",
            "57600", "115200", "128000", "153600", "230400", "256000", "460800", "921600", "1843200"};
        private static ReadOnlyCollection<string> readOnlyCommonBaudRates = new ReadOnlyCollection<string>(commonBaudRates);
                
        private static string[] commonStopBits = new string[] { "none", "one", "onepointfive", "two" };
        private static ReadOnlyCollection<string> readOnlyCommonStopBits = new ReadOnlyCollection<string>(commonStopBits);

        private static string[] commonParityBits = new string[] { "none", "odd", "even", "mark", "space" };
        private static ReadOnlyCollection<string> readOnlyCommonParityBits = new ReadOnlyCollection<string>(commonParityBits);

        private static string[] commonDataBits = new string[] { "7", "8" };
        private static ReadOnlyCollection<string> readOnlyCommonDataBits = new ReadOnlyCollection<string>(commonDataBits);

        /// <summary>
        /// Commonly used serial baud rates, system dependant but usually produced by a 1.843200MHz oscillator
        /// </summary>        
        public static ReadOnlyCollection<string> BaudRates
        {
            get
            {
                return readOnlyCommonBaudRates;
            }
        }

        /// <summary>
        /// Only supported stop bits as of .NET 4.6.1
        /// </summary>
        public static ReadOnlyCollection<string> StopBits
        {
            get
            {
                return readOnlyCommonStopBits;
            }
        }

        /// <summary>
        /// Only supported parity bits as of .NET 4.6.1
        /// </summary>
        public static ReadOnlyCollection<string> ParityBits
        {
            get
            {
                return readOnlyCommonParityBits;
            }
        }

        /// <summary>
        /// Genrally supported data bits (7 for ASCII but 8 for ease of use)
        /// </summary>
        public static ReadOnlyCollection<string> DataBits
        {
            get
            {
                return readOnlyCommonDataBits;
            }
        }
    }
    #endregion

    #region ExampleCode

    /// <summary>
    /// SerialPortParameters can be used to fill form elements for easier user selection
    /// </summary>
    //private void Form_Load(object sender, EventArgs e)
    //{
    //    serialPortBaud_CB.Items.AddRange(SerialPortParameters.BaudRates.ToArray());
    //    serialPortData_CB.Items.AddRange(SerialPortParameters.DataBits.ToArray());
    //    serialPortParity_CB.Items.AddRange(SerialPortParameters.ParityBits.ToArray());
    //    serialPortStop_CB.Items.AddRange(SerialPortParameters.StopBits.ToArray());
    //}

    /// <summary>
    /// 
    /// </summary>
    //private void setupPort()
    //{
    //    string portName = serialPortName_TB.Text;
    //    int portBaud = Convert.ToInt32(serialPortBaud_CB.SelectedItem.ToString());
    //    string portParity = serialPortParity_CB.SelectedItem.ToString();
    //    int portData = Convert.ToInt32(serialPortData_CB.SelectedItem.ToString());
    //    string portStop = serialPortStop_CB.SelectedItem.ToString();

    //    comPort = new AsyncSerial(portName, portBaud, portParity, portData, portStop); //comPort should be declared at class level
    //    comPort.OnPacketReceived += new AsyncSerial.SerialDataReceivedEventHandler(supplySerial_onPacketReceived);
    //    comPort.StartReceive();
    //}

    /// <summary>
    /// This is the code that gets called when serial data is recived
    /// </summary>
    //private void supplySerial_onPacketReceived(object sender, byte[] packet)
    //{
    //    byte[] serialData = packet;
    //    string serialString = Encoding.Default.GetString(serialData);   //Byte array can be converted to a string
    //    this.Invoke((MethodInvoker)delegate {
    //        someFormItem.ChangeElement(serialString);   //Since the port is asynchronous you cannot change UI elements without a cross thread Invoker
    //    });
    //}

    //private void sendSerialData()
    //{
    //    if (comPort.IsPortOpen()) //you can check if the port is open (device could be removed etc)
    //    {
    //        comPort.SendSerialData("you can send strings or byte arrays");
    //    }
    //    else
    //    {
    //        //port isn't open
    //    }
    //}

    #endregion
}