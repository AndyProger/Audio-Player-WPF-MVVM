using System;
using System.Linq;
using Player.Models;
using DevExpress.Mvvm;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace Player.ViewModels
{
    partial class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Создать новый плейлист
        /// </summary>
        public void AddPlaylist() => Playlists.Add(new Playlist("New Playlist", new ObservableCollection<SongInfo>()));

        /// <summary>
        /// Изменить текущий плейлист на выбранный
        /// </summary>
        public void ChangePlaylist()
        {
            // Меняем плейлист в плеере (Model)
            Player.Songs.SongList = CurrentPlaylist.SongList;
            // Меняем плейлист вo View части
            CurrentSongList = CurrentPlaylist.SongList;

            PercentOfCurrentDurationOfSong = 0;
            SelectedSong = null;
            Player.PlaySong(SelectedSong);
            CurrentTimeOfPlayingSong = TimeSpan.Zero;
        }

        /// <summary>
        /// Сбрасываем время песни к нулю
        /// </summary>
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

        /// <summary>
        /// Проверка на то, изменилось ли время воспроизведения (изменилась ли позиция бегунка на слайдере)
        /// </summary>
        private bool IsTimeSlideChanged()
        {
            return !(Player.CurrentTime < TimeSpan.FromSeconds(0.05)) &&
                ((Math.Truncate(PercentOfCurrentDurationOfSong) > Math.Truncate((float)Player.PercentOfCurrentDuration) + 10) ||
                Math.Truncate(PercentOfCurrentDurationOfSong) + 10 < Math.Truncate((float)Player.PercentOfCurrentDuration));
        }

        /// <summary>
        /// Вызывается каждый тик таймера
        /// </summary>
        private void TimerTick(object sender, EventArgs e)
        {
            // Проверка не поставлена ли песня на паузу
            if (Player.IsPaused)
                return;

            // Проверка не закончилась ли песня
            if (Player.CurrentTime > Player.TotalDuration)
                PlayNext();

            // Если была перемотка
            if (IsTimeSlideChanged())
                Player.CurrentTime = TimeSpan.FromSeconds(PercentOfCurrentDurationOfSong / 1000.0 * Player.TotalDuration.TotalSeconds);
            
            PercentOfCurrentDurationOfSong = (float)Player.PercentOfCurrentDuration;

            CurrentTimeOfPlayingSong = Player.CurrentTime;
            Player.Volume = CurrentVolume / 100.0f;
        }

        /// <summary>
        /// Добавить песню / песни 
        /// </summary>
        private void AddSongClick()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Список форматов можно расширить (см. документацию к NAudio)
            openFileDialog.Filter = "\"Audio Files|*.WAV;*.AIFF;*.MP3;*.WMA;*.MP4;*.m4a;\"";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    Player.AddSong(filename);
                }
            }

            CurrentPlaylist.SongList = Player.Songs.SongList;
            CurrentSongList = Player.Songs.SongList;
        }

        /// <summary>
        /// Сортировка песен с помощью LINQ
        /// </summary>
        private void SortSongs()
        {
            Player.Songs.SongList = new ObservableCollection<SongInfo>(Player.Songs.SongList.OrderBy(x => x.SongName).ToList());
            CurrentPlaylist.SongList = Player.Songs.SongList;
            CurrentSongList = Player.Songs.SongList;
        }

        /// <summary>
        /// Воспроизвести песню
        /// </summary>
        private void PlaySong()
        {
            if (SelectedSong is not null)
            {
                Player.PlaySong(SelectedSong);
            }
        }

        /// <summary>
        /// Воспроизвести следующую песню
        /// </summary>
        private void PlayNext()
        {
            if (SelectedSong is not null)
            {
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

        /// <summary>
        /// Воспроизвести предыдущую песню
        /// </summary>
        private void PlayPrevious()
        {
            if (SelectedSong is not null)
            {
                Player.PlayPreviousSong();
                SelectedSong = Player.Current;

                if (SelectedSong is not null)
                {
                    SelectedSongInfo = new SongInfo(SelectedSong);
                }

                PercentOfCurrentDurationOfSong = 0;
                Player.CurrentTime = TimeSpan.Zero;
            }
        }
    }
}
