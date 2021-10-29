using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace WPF_MVVM_Core.Models
{
    class Playlist
    {
        public string Name { get; set; }
        public ObservableCollection<SongInfo> SongList { get; set; } = new ObservableCollection<SongInfo>();

        public Playlist(string name, ObservableCollection<SongInfo> songList)
        {
            Name = name;
            SongList = songList;
        }
    }
}
