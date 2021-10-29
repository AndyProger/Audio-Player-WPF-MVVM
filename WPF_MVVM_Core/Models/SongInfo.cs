using System;
using TagLib;
using System.IO;
using DevExpress.Mvvm;
using System.Windows.Media.Imaging;

namespace WPF_MVVM_Core.Models
{
    class SongInfo : ViewModelBase, IEquatable<SongInfo>
    {
        public string SongName { get; set; }
        public string Artist { get; set; }
        public string AlbumTitle { get; set; }
        public string Genre { get; set; }
        public uint? IssueYear { get; set; }
        public TimeSpan Duration { get; set; }
        public string MusicPath { get; private set; }
        public BitmapImage AlbumCover { get; private set; }

        public SongInfo(string path)
        {
            TagLib.File tags = TagLib.File.Create(path);

            SongName = tags.Tag.Title;
            Artist = tags.Tag.FirstPerformer;
            Duration = tags.Properties.Duration;
            MusicPath = path;
            AlbumTitle = tags.Tag.Album;
            Genre = tags.Tag.FirstGenre;

            IssueYear = tags.Tag.Year == 0 ? null : tags.Tag.Year;

            AlbumCover = GetAlbumCover(tags);
        }

        public SongInfo(SongInfo song) : this(song.MusicPath) { }

        private BitmapImage GetAlbumCover(TagLib.File tags)
        {
            BitmapImage bitmap = new BitmapImage();

            try
            {
            IPicture pic = tags.Tag.Pictures[0];
            MemoryStream ms = new MemoryStream(pic.Data.Data);
            ms.Seek(0, SeekOrigin.Begin);
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            }
            catch
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(@"C:\Users\Andrey\Desktop\def.png");
                bitmap.EndInit();
            }

            return bitmap;
        }

        public bool Equals(SongInfo other) => MusicPath == other.MusicPath;
    }
}
