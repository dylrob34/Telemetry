using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telemetry
{
    class Participant : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        private string _Name;
        private short _Position;

        public Participant(string name, short position)
        {
            Name = name;
            Position = position;
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
                //fire event handler
            }
        }

        public short Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
                //fire event handler
            }
        }
    }
}
