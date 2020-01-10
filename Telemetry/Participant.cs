using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Telemetry
{
    public class Participant : INotifyPropertyChanged, IComparable<Participant>
    {

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        private string _Name;
        private byte _Position;
        private float _LastLapTime;
        private float _BestLapTime;
        private byte _PitStatusNum;
        private string _PitStatus;
        public bool UpdatePitNum;

        public Participant(string name)
        {
            string tempName = "";
            foreach (char letter in name)
            {
                tempName += letter;
            }
            Name = tempName;
            Position = 0;
            UpdatePitNum = false;
        }

        public Participant(string name, byte position, float lastLapTime, float bestLapTime, string pitStatus, ushort numPits, byte pitStatusNum, bool updatePitNum)
        {
            string tempName = "";
            foreach (char letter in name)
            {
                tempName += letter;
            }
            Name = tempName;
            Position = position;
            LastLapTime = lastLapTime;
            BestLapTime = bestLapTime;
            _PitStatus = pitStatus;
            NumPitStops = numPits;
            _PitStatusNum = pitStatusNum;
            UpdatePitNum = updatePitNum;
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public byte Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Position"));
            }
        }

        public float LastLapTime
        {
            get
            {
                return _LastLapTime;
            }
            set
            {
                _LastLapTime = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LastLapTime"));
            }
        }

        public float BestLapTime
        {
            get
            {
                return _BestLapTime;
            }
            set
            {
                _BestLapTime = value;
                PropertyChanged(this, new PropertyChangedEventArgs("BestLapTime"));
            }
        }

        public string PitStatus
        {
            get
            {
                return _PitStatus;
            }
            set
            {
                _PitStatus = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PitStatus"));
            }
        }

        public ushort NumPitStops { get; set; }

        public byte PitStatusNum
        {
            get
            {
                return _PitStatusNum;
            }
            set
            {
                _PitStatusNum = value;
                setPitStatus();
            }
        }

        public void setPitStatus()
        {
            switch (_PitStatusNum)
            {
                case (byte)PitStatusEnum.None:
                    PitStatus = "" + NumPitStops;
                    break;
                case (int)PitStatusEnum.Pitting:
                    PitStatus = "Pitting";
                    UpdatePitNum = true;
                    break;
                case (int)PitStatusEnum.InPit:
                    PitStatus = "In Pit Area";
                    if (UpdatePitNum)
                    {
                        NumPitStops++;
                        UpdatePitNum = false;
                    }
                    break;
                default:
                    break;
            }
        }

        public int CompareTo(Participant other)
        {
            if (Position > other.Position)
                return 1;
            else if (Position < other.Position)
                return -1;
            else
                return 0;
        }
    }

    public enum PitStatusEnum
    {
        None = 0,
        Pitting = 1,
        InPit = 2
    }
}
