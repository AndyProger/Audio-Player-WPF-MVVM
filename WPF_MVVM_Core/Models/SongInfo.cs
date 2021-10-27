using System;
using TagLib;
using DevExpress.Mvvm;

namespace WPF_MVVM_Core.Models
{
    class SongInfo : ViewModelBase, IEquatable<SongInfo>
    {
        public string SongName { get; set; }
        public string Artist { get; set; }
        public TimeSpan Duration { get; set; }
        public string MusicPath { get; private set; }

        public SongInfo(string path)
        {
            File tags = File.Create(path);

            SongName = tags.Tag.Title;
            Artist = tags.Tag.FirstPerformer;
            Duration = tags.Properties.Duration;
            MusicPath = path;
        }

        public bool Equals(SongInfo other) => MusicPath == other.MusicPath;
    }
}
