using System.Runtime.InteropServices;

namespace Telemetry
{
    class ToStruct
    {
        public static T Convert<T>(byte[] data)
        {
            GCHandle pinnedPacket = GCHandle.Alloc(data, GCHandleType.Pinned);
            T packet = (T)Marshal.PtrToStructure(
                pinnedPacket.AddrOfPinnedObject(),
                typeof(T));
            pinnedPacket.Free();
            return packet;

        }


        //Header struct
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketHeader
        {
            ushort m_packetFormat;         // 2019
            byte m_gameMajorVersion;     // Game major version - "X.00"
            byte m_gameMinorVersion;     // Game minor version - "1.XX"
            byte m_packetVersion;        // Version of this packet type, all start from 1
            public byte m_packetId;             // Identifier for the packet type, see below
            public ulong m_sessionUID;           // Unique identifier for the session
            float m_sessionTime;          // Session timestamp
            uint m_frameIdentifier;      // Identifier for the frame the data was retrieved on
            public byte m_playerCarIndex;       // Index of player's car in the array
        };

        //Telemetry structs
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarTelemetryData
        {
            public PacketHeader m_header;        // Header
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public CarTelemetryData[] m_carTelemetryData;

            uint m_buttonStatus;        // Bit flags specifying which buttons are being pressed
                                        // currently - see appendices
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct CarTelemetryData
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

        //Participants struct
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct ParticipantData
        {
            byte m_aiControlled;           // Whether the vehicle is AI (1) or Human (0) controlled
            byte m_driverId;       // Driver id - see appendix
            byte m_teamId;                 // Team id - see appendix
            byte m_raceNumber;             // Race number of the car
            byte m_nationality;            // Nationality of the driver

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            public char[] m_name;               // Name of participant in UTF-8 format – null terminated
                                                 // Will be truncated with … (U+2026) if too long
            byte m_yourTelemetry;          // The player's UDP setting, 0 = restricted, 1 = public
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketParticipantsData
        {
            public PacketHeader m_header;            // Header

            public byte m_numActiveCars;  // Number of active cars in the data – should match number of
                                          // cars on HUD
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public ParticipantData[] m_participants;
        };

        //Lap data structs
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LapData
        {
            public float m_lastLapTime;            // Last lap time in seconds
            float m_currentLapTime; // Current time around the lap in seconds
            public float m_bestLapTime;        // Best lap time of the session in seconds
            float m_sector1Time;        // Sector 1 time in seconds
            float m_sector2Time;        // Sector 2 time in seconds
            float m_lapDistance;        // Distance vehicle is around current lap in metres – could
                                        // be negative if line hasn’t been crossed yet
            float m_totalDistance;      // Total distance travelled in session in metres – could
                                        // be negative if line hasn’t been crossed yet
            float m_safetyCarDelta;        // Delta in seconds for safety car
            public byte m_carPosition;    // Car race position
            byte m_currentLapNum;      // Current lap number
            public byte m_pitStatus;              // 0 = none, 1 = pitting, 2 = in pit area
            byte m_sector;                 // 0 = sector1, 1 = sector2, 2 = sector3
            byte m_currentLapInvalid;      // Current lap invalid - 0 = valid, 1 = invalid
            byte m_penalties;              // Accumulated time penalties in seconds to be added
            byte m_gridPosition;           // Grid position the vehicle started the race in
            byte m_driverStatus;           // Status of driver - 0 = in garage, 1 = flying lap
                                           // 2 = in lap, 3 = out lap, 4 = on track
            byte m_resultStatus;          // Result status - 0 = invalid, 1 = inactive, 2 = active
                                           // 3 = finished, 4 = disqualified, 5 = not classified
                                           // 6 = retired
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketLapData
        {
            public PacketHeader m_header;              // Header
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public LapData[] m_lapData;         // Lap data for all cars on track
        };
    }
}
