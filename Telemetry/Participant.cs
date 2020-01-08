using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telemetry
{
    class Participant : INotifyPropertyChanged
    {
        private Player m_player;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public Player player
        {
            get
            {
                return m_player;
            }

            set
            {
                if (m_player.Equals(value))
                    return;

                m_player = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(player)));
            }
        }

        public override string ToString()
        {
            return player.name + player.position;
        }
    }

    struct Player
    {
        public string name;
        public short position;
    }
}
