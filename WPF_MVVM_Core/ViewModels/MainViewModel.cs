using System.Windows.Input;
using System.Windows.Forms;
using DevExpress.Mvvm;
using WPF_MVVM_Core.Models;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System;

namespace WPF_MVVM_Core.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public AudioPlayer Player { get; set; } = new AudioPlayer();
        public SongInfo SelectedSong { get; set; }
        public float PercentOfCurrentDurationOfSong { get; set; }
        public TimeSpan CurrentTimeOfPlayingSong { get; set; }
        public float CurrentVolume { get; set; } = 30;

        private DispatcherTimer _timer = new DispatcherTimer();

        public ICommand ClickAddSong { get => new DelegateCommand(() => AddSongClick()); }
        public ICommand ClickPlaySong { get => new DelegateCommand(() => PlaySong()); }
        public ICommand ClickPlayNext { get => new DelegateCommand(() => PlayNext()); }
        public ICommand ClickPlayPrevious { get => new DelegateCommand(() => PlayPrevious()); }
        public ICommand SliderChanged { get => new DelegateCommand(() => _timer.Start()); }
        public ICommand SliderStartedChanged { get => new DelegateCommand(() => _timer.Stop()); }
        public ICommand SelectionChanged { get => new DelegateCommand(() => SetToZeroTimeSong()); }

        public void SetToZeroTimeSong()
        {
            if(Player.Current is not null && !(Player.CurrentTime == TimeSpan.Zero))
            {
                PercentOfCurrentDurationOfSong = 0;
                PlaySong();
            }
        }

        public MainViewModel()
        {
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
                PercentOfCurrentDurationOfSong = 0;
                Player.CurrentTime = TimeSpan.Zero;
            }
        }
    }
}
