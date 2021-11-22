using System;
using System.IO;
using System.Linq;
using NAudio.Wave;
using System.Collections.ObjectModel;

namespace Player.Models
{
    class AudioPlayer
    {
        public ObservableCollection<SongInfo> SongList { get; set; } = new ObservableCollection<SongInfo>();
        
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

        public TimeSpan TotalDuration { get => _outputStream.TotalTime - TimeSpan.FromSeconds(1); }
        public bool IsPaused { get; private set; } = true;

        private WaveStream _outputStream;
        private WaveChannel32 _volumeStream;
        private WaveOutEvent _player;

        public AudioPlayer()
        {
            _player = new WaveOutEvent();
            Volume = 0.3f;
        }

        /// <summary>
        /// Добавить новую песню в список
        /// </summary>
        /// <param name="filePath">Путь к песне</param>
        public void AddSong(string filePath)
        {
            if (File.Exists(filePath))
            {
                SongInfo song = new SongInfo(filePath);

                if(!SongList.Contains(song))
                    SongList.Add(song);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// Воспроизвести песню, если песня уже играет => пауза,
        /// если песня на паузе => воспроизвидение с того же места,
        /// если выбрана другая песня => воспроизведение с начала
        /// </summary>
        /// <param name="song">Песня</param>
        public void PlaySong(SongInfo song)
        {
            if (song is null)
            {
                _player.Stop();
                IsPaused = true;
                return;
            }

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
                CurrentTime = TimeSpan.Zero;
            }

            Current = song;

            _outputStream = new MediaFoundationReader(Current.MusicPath);
            _volumeStream = new WaveChannel32(_outputStream);

            _player.Init(_volumeStream);
            _player.Play();
            IsPaused = false;
        }

        /// <summary>
        /// Воспроизвести следующую песню (принцип кольцевой очереди)
        /// </summary>
        public void PlayNextSong() => PlayPreviousOrNextSong(true);
        /// <summary>
        /// Воспроизвести предидущую песню (принцип кольцевой очереди)
        /// </summary>
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
