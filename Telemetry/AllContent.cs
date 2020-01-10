using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Telemetry
{
    class AllContent : INotifyPropertyChanged
    {
        private List<Participant> _participants;
        private List<Participant> _sorted;
        public List<Participant> participants
        {
            get
            {
                return _participants;
            }
            set
            {
                _participants = value;
            }
        }

        public List<Participant> SortedList { get; set; }
        List<Participant> temp;

        private ulong sessionUID;
        private bool refresh;
        private byte numPlayer;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };


        public AllContent()
        {
            Thread thread = new Thread(startServer);
            thread.Start();
        }

        public void update()
        {
            temp = new List<Participant>();
            foreach (Participant p in participants)
            {
                temp.Add(new Participant(p.Name, p.Position, p.LastLapTime, p.BestLapTime, p.PitStatus, p.NumPitStops, p.PitStatusNum, p.UpdatePitNum));
            }
            temp.Sort();
            SortedList = temp;
            PropertyChanged(this, new PropertyChangedEventArgs("SortedList"));
        }

        public void load()
        {
            Application.Current.Dispatcher.Invoke(() => { participants = new List<Participant>(); });
            refresh = true;
        }

        public void startServer()
        {
            load();
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
                switch (data.Length)
                {
                    case 1104:
                        if (refresh)
                        {
                            Debug.WriteLine("refreshing");
                            ToStruct.PacketParticipantsData participantsData = ToStruct.Convert<ToStruct.PacketParticipantsData>(data);
                            numPlayer = participantsData.m_numActiveCars;
                            Encoding utf8 = Encoding.UTF8;
                            for (var i = 0; i < numPlayer; i++)
                            {
                                Application.Current.Dispatcher.Invoke(() => { participants.Add(new Participant(new string(participantsData.m_participants[i].m_name))); });
                            }
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                PropertyChanged(this, new PropertyChangedEventArgs("participants"));
                            });
                            refresh = false;
                        }
                        break;
                    case 843:
                        ToStruct.PacketLapData lapData = ToStruct.Convert<ToStruct.PacketLapData>(data);
                        /*if (lapData.m_header.m_sessionUID != sessionUID)
                        {
                            load();
                            break;
                        }*/
                        for (var i = 0; i < participants.Count; i++)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                participants[i].Position = lapData.m_lapData[i].m_carPosition;
                                participants[i].LastLapTime = lapData.m_lapData[i].m_lastLapTime;
                                participants[i].BestLapTime = lapData.m_lapData[i].m_bestLapTime;
                                participants[i].PitStatusNum = lapData.m_lapData[i].m_pitStatus;
                            });
                        }
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs("participants"));
                        });
                        break;
                    default:
                        break;
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    update();
                });
            }
        }
    }
}
