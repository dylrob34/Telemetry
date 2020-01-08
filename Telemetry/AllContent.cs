using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace Telemetry
{
    class AllContent
    {

        public class Test
        {
            public string hello;
            public int testing;

            public Test(string first, int second)
            {
                hello = first;
                testing = second;
            }

            public override string ToString()
            {
                return hello + testing;
            }
        }

        public Test test;
        public string ugh = "sdflkasjdflksajdf";
        public List<Participant> participants;
        public List<Participant> GetParticipants()
        {
            return null;
        }

        public AllContent()
        {
            participants = new List<Participant>();
            Thread thread = new Thread(startServer);
            thread.Start();

            test = new Test("string test and number test: ", 34);
            ugh = "fuck this";
        }

        public void startServer()
        {
            string message = "starting server...";
            Debug.WriteLine(message);
            byte[] data;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 20777);
            UdpClient newsock = new UdpClient(ipep);

            Debug.WriteLine("Waiting for a client...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                data = newsock.Receive(ref sender);
                if (data.Length == 1347)
                {

                    GCHandle pinnedPacket = GCHandle.Alloc(data, GCHandleType.Pinned);
                    PacketCarTelemetryData telemetry = (PacketCarTelemetryData)Marshal.PtrToStructure(
                        pinnedPacket.AddrOfPinnedObject(),
                        typeof(PacketCarTelemetryData));
                    pinnedPacket.Free();
                    Debug.WriteLine("the header is: " + telemetry.m_header.m_packetId);
                    Debug.WriteLine("player speed is: " + telemetry.m_carTelemetryData[telemetry.m_header.m_playerCarIndex].m_speed);
                }

            }
        }

        public override string ToString()
        {
            return test.ToString();
        }
    }



    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PacketHeader
    {
        ushort m_packetFormat;         // 2019
        byte m_gameMajorVersion;     // Game major version - "X.00"
        byte m_gameMinorVersion;     // Game minor version - "1.XX"
        byte m_packetVersion;        // Version of this packet type, all start from 1
        public byte m_packetId;             // Identifier for the packet type, see below
        ulong m_sessionUID;           // Unique identifier for the session
        float m_sessionTime;          // Session timestamp
        uint m_frameIdentifier;      // Identifier for the frame the data was retrieved on
        public byte m_playerCarIndex;       // Index of player's car in the array
    };
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PacketCarTelemetryData
    {
        public PacketHeader m_header;        // Header
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CarTelemetryData[] m_carTelemetryData;

        uint m_buttonStatus;        // Bit flags specifying which buttons are being pressed
                                    // currently - see appendices
    };
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct CarTelemetryData
    {
        public ushort m_speed;                    // Speed of car in kilometres per hour
        float m_throttle;                 // Amount of throttle applied (0.0 to 1.0)
        float m_steer;                    // Steering (-1.0 (full lock left) to 1.0 (full lock right))
        float m_brake;                    // Amount of brake applied (0.0 to 1.0)
        byte m_clutch;                   // Amount of clutch applied (0 to 100)
        public sbyte m_gear;                     // Gear selected (1-8, N=0, R=-1)
        ushort m_engineRPM;                // Engine RPM
        byte m_drs;                      // 0 = off, 1 = on
        byte m_revLightsPercent;         // Rev lights indicator (percentage)
        fixed ushort m_brakesTemperature[4];     // Brakes temperature (celsius)
        fixed ushort m_tyresSurfaceTemperature[4]; // Tyres surface temperature (celsius)
        fixed ushort m_tyresInnerTemperature[4]; // Tyres inner temperature (celsius)
        fixed ushort m_engineTemperature[4];        // Engine temperature (celsius)
        fixed float m_tyresPressure[4];         // Tyres pressure (PSI)
        fixed byte m_surfaceType[4];           // Driving surface, see appendices
    };
}
