using System.Collections.ObjectModel;

namespace Player.Models
{
    /// <summary>
    /// Класс для объединения песен в рамках одной именованной сущности - плейлист
    /// </summary>
    class Playlist
    {
        public string Name { get; set; }
        public ObservableCollection<SongInfo> SongList { get; set; } = new ObservableCollection<SongInfo>();

        public Playlist()
        {
            Name = string.Empty;
        }

        public Playlist(string name, ObservableCollection<SongInfo> songList)
        {
            Name = name;
            SongList = songList;
        }
    }
}
