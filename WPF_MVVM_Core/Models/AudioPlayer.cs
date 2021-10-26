using System;
using System.IO;
using System.Collections.ObjectModel;
using NAudio.Wave;
using System.Linq;
using DevExpress.Mvvm;

namespace WPF_MVVM_Core.Models
{
    class AudioPlayer : ViewModelBase
    {
        public ObservableCollection<SongInfo> SongList { get; set; } = new ObservableCollection<SongInfo>();
        // from 0 to 1.0
        public float Volume
        {
            get => _player.Volume;

            set => _player.Volume = value;
        }
        public SongInfo Current { get; set; }

        public double PercentOfCurrentDuration
        {
            get => _outputStream.CurrentTime.TotalSeconds * 1000.0 / _outputStream.TotalTime.TotalSeconds;
        }

        public TimeSpan CurrentTime
        {
            get => _outputStream.CurrentTime;

            set => _outputStream.CurrentTime = value;
        }

        public TimeSpan TotalDuration { get => _outputStream.TotalTime - TimeSpan.FromSeconds(0.5); }

        private WaveStream _outputStream;
        private WaveChannel32 _volumeStream;
        private WaveOutEvent _player;
        public bool IsPaused { get; private set; } = true;

        public AudioPlayer()
        {
            _player = new WaveOutEvent();
            Volume = 0.3f;
        }

        public void AddSong(string filePath)
        {
            if (File.Exists(filePath))
            {
                SongList.Add(new SongInfo(filePath));
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public void PlaySong(SongInfo song)
        {
            if (song is null)
                throw new ArgumentNullException();

            // если выбрана та же песня и она не на паузе => пауза
            if (_outputStream != null && Current.MusicPath == song.MusicPath && !IsPaused)
            {
                _player.Pause();
                IsPaused = true;
                return;
            }

            // если выбрана та же песня и она на паузе => воспроизвидение с того же места
            if (_outputStream != null && Current.MusicPath == song.MusicPath && IsPaused)
            {
                _player.Play();
                IsPaused = false;
                return;
            }

            // если была выбрана другая песня, остановить текущую
            if (_outputStream != null)
            {
                _player.Stop();
            }

            Current = song;

            _outputStream = new MediaFoundationReader(Current.MusicPath);
            _volumeStream = new WaveChannel32(_outputStream);

            _player.Init(_volumeStream);
            _player.Play();
            IsPaused = false;
        }

        // воспроизвести следующую песню (принцип кольцевой очереди)
        public void PlayNextSong() => PlayPreviousOrNextSong(true);
        // воспроизвести предидущую песню (принцип кольцевой очереди)
        public void PlayPreviousSong() => PlayPreviousOrNextSong(false);

        private void PlayPreviousOrNextSong(bool isNextMode)
        {
            if (!SongList.Any())
                return;

            var indexOfCurrentSong = SongList.IndexOf(Current);
            var nextIndex = 0;

            if (isNextMode)
            {
                if (indexOfCurrentSong != SongList.Count - 1)
                    nextIndex = indexOfCurrentSong + 1;
            }
            else
            {
                nextIndex = indexOfCurrentSong - 1;

                if (indexOfCurrentSong <= 0)
                    nextIndex = SongList.Count - 1;
            }

            if (nextIndex == indexOfCurrentSong)
            { 
                RepeatSong(Current);
                return;
            }

            PlaySong(SongList[nextIndex]);
        }

        private void RepeatSong(SongInfo song)
        {
            if (song is null)
                return;

            IsPaused = true;
            _outputStream.CurrentTime = TimeSpan.Zero;
            PlaySong(song);
        }
    }
}
