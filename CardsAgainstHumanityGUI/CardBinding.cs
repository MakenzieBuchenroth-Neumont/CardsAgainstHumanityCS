using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CardsAgainstHumanityGUI
{
    public class CardBinding : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string card;

        public string Card
        {
            get { return card;}
            set { card = value; NotifyPropertyChanged("Card"); }
        }

        void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public CardBinding()
        {
        }

        public CardBinding(string input)
        {
            Card = input;
        }
    }
}
