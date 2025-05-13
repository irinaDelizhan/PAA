using PAA.Models;
using PAA.UserInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PAA.Classes
{
    public class Storage
    {
        private static Storage? instance = null;
        private Storage()
        {

        }
        public static Storage? Instance
        {
            get
            {
                if (instance == null)
                    instance = new Storage();
                return instance;
            }
        }
        public ApplicationDbContext? context { get; set; } = null;

        public MainWindow? mainWindow { get; set; } = null;
        public AdminWindow? adminWindow { get; set; } = null;

        public List<User>? users { get; set; } = null;
        public List<_Section>? sections { get; set; } = null;
        public List<Position>? positions { get; set; } = null;
        public List<Project>? projects { get; set; } = null;
        public List<State>? states { get; set; } = null;
        public List<Participation> participations { get; set; } = null;
        public ObservableCollection<Transaction> transactions { get; set; } = null;
    }
}
