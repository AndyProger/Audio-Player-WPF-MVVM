using System.Windows.Input;
using System.Windows.Forms;
using DevExpress.Mvvm;
using WPF_MVVM_Core.Models;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System;
using WPF_MVVM_Core.Services;

namespace WPF_MVVM_Core.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public AudioPlayer Player { get; set; } = new AudioPlayer();
        public Playlist CurrentPlaylist { get; set; }
        public SongInfo SelectedSong { get; set; }
        public static SongInfo SelectedSongInfo { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; } = new ObservableCollection<Playlist>();
        public BitmapImage DefaultAlbumCover { get; set; } = new BitmapImage(new Uri(@"C:\Users\Andrey\Desktop\def.png"));
        public float PercentOfCurrentDurationOfSong { get; set; }
        public TimeSpan CurrentTimeOfPlayingSong { get; set; }
        public float CurrentVolume { get; set; } = 30;
        public OpenWindowCommand ShowDialogCommand { get; set; }

        private DispatcherTimer _timer = new DispatcherTimer();

        public ICommand ClickAddSong { get => new DelegateCommand(() => AddSongClick()); }
        public ICommand ClickPlaySong { get => new DelegateCommand(() => PlaySong()); }
        public ICommand ClickPlayNext { get => new DelegateCommand(() => PlayNext()); }
        public ICommand ClickPlayPrevious { get => new DelegateCommand(() => PlayPrevious()); }
        public ICommand SliderChanged { get => new DelegateCommand(() => _timer.Start()); }
        public ICommand SliderStartedChanged { get => new DelegateCommand(() => _timer.Stop()); }
        public ICommand SelectionChanged { get => new DelegateCommand(() => SetToZeroTimeSong()); }
        public ICommand CreatePlaylist { get => new DelegateCommand(() => AddPlaylist()); }
        public ICommand SelectionPlaylistChanged { get => new DelegateCommand(() => ChangePlaylist()); }

        public void AddPlaylist()
        {
            Playlists.Add(new Playlist("New Playlist", new ObservableCollection<SongInfo>()));
        }

        public void ChangePlaylist()
        {
            Player.SongList = CurrentPlaylist.SongList;
            PercentOfCurrentDurationOfSong = 0;
            SelectedSong = null;
            Player.PlaySong(SelectedSong);
            CurrentTimeOfPlayingSong = TimeSpan.Zero;
        }

        public void SetToZeroTimeSong()
        {
            if (SelectedSong is not null)
            {
                SelectedSongInfo = new SongInfo(SelectedSong);
            }

            if (Player.Current is not null && !(Player.CurrentTime == TimeSpan.Zero))
            {
                PercentOfCurrentDurationOfSong = 0;
                PlaySong();
            }
        }

        public MainViewModel()
        {
            Playlists.Add(new Playlist("Main",Player.SongList));

            ShowDialogCommand = new OpenWindowCommand();

            _timer.Interval = TimeSpan.FromSeconds(0.1);
            _timer.Tick += _timer_Tick;
            _timer.Start();

        }

        private bool IsTimeSlideChanged()
        {
            return !(Player.CurrentTime < TimeSpan.FromSeconds(0.05)) &&
                ((Math.Truncate(PercentOfCurrentDurationOfSong) > Math.Truncate((float)Player.PercentOfCurrentDuration) + 10) ||
                Math.Truncate(PercentOfCurrentDurationOfSong) + 10 < Math.Truncate((float)Player.PercentOfCurrentDuration));
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (Player.IsPaused)
                return;

            if (Player.CurrentTime > Player.TotalDuration)
                PlayNext();

            if (IsTimeSlideChanged())
            {
                Player.CurrentTime = TimeSpan.FromSeconds(PercentOfCurrentDurationOfSong / 1000.0 * Player.TotalDuration.TotalSeconds);
            }

            PercentOfCurrentDurationOfSong = (float)Player.PercentOfCurrentDuration;

            CurrentTimeOfPlayingSong = Player.CurrentTime;
            Player.Volume = CurrentVolume / 100.0f;
        }

        private void AddSongClick()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "\"Audio Files|*.WAV;*.AIFF;*.MP3;*.WMA;*.MP4;*.m4a;\"";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    Player.AddSong(filename);
                }
            }
        }

        private void PlaySong()
        {
            if (SelectedSong is not null)
            {
                Player.PlaySong(SelectedSong);
            }
        }

        private void PlayNext()
        {
            if(SelectedSong is not null)
            {
                //Player.Current = SelectedSong;
                Player.PlayNextSong();
                SelectedSong = Player.Current;

                if (SelectedSong is not null)
                {
                    SelectedSongInfo = new SongInfo(SelectedSong);
                }

                PercentOfCurrentDurationOfSong = 0;
                Player.CurrentTime = TimeSpan.Zero;
            }
        }

        private void PlayPrevious()
        {
            if (SelectedSong is not null)
            {
                //Player.Current = SelectedSong;
                Player.PlayPreviousSong();
                SelectedSong = Player.Current;

                if(SelectedSong is not null)
                {
                    SelectedSongInfo = new SongInfo(SelectedSong);
                }

                PercentOfCurrentDurationOfSong = 0;
                Player.CurrentTime = TimeSpan.Zero;
            }
        }
    }
}
