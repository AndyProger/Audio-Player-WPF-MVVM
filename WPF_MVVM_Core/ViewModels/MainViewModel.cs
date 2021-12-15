using System;
using Player.Models;
using DevExpress.Mvvm;
using Player.Services;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace Player.ViewModels
{
    partial class MainViewModel : ViewModelBase
    {
        public AudioPlayer Player { get; set; } = new AudioPlayer();
        public Playlist CurrentPlaylist { get; set; } = new Playlist();
        public ObservableCollection<SongInfo> CurrentSongList { get; set; } = new ObservableCollection<SongInfo>();
        public ObservableCollection<Playlist> Playlists { get; set; } = new ObservableCollection<Playlist>();
        public SongInfo SelectedSong { get; set; }
        public static SongInfo SelectedSongInfo { get; set; }
        public BitmapImage DefaultAlbumCover { get; set; } = new BitmapImage(new Uri("def.png", UriKind.RelativeOrAbsolute));
        public float PercentOfCurrentDurationOfSong { get; set; }
        public TimeSpan CurrentTimeOfPlayingSong { get; set; }
        public float CurrentVolume { get; set; } = 30;
        public OpenWindowCommand ShowDialogCommand { get; set; } = new OpenWindowCommand();

        private DispatcherTimer _timer = new DispatcherTimer();

        // Добавить песню
        public ICommand ClickAddSong { get => new DelegateCommand(AddSongClick); }

        // Воспроизвести / приостановить песню
        public ICommand ClickPlaySong { get => new DelegateCommand(PlaySong); }

        // Следующая песня в плейлисте
        public ICommand ClickPlayNext { get => new DelegateCommand(PlayNext); }

        // Предыдущая песня в плейлисте
        public ICommand ClickPlayPrevious { get => new DelegateCommand(PlayPrevious); }

        // После конца изменения позиции слайдера - возобновить таймер
        public ICommand SliderChanged { get => new DelegateCommand(() => _timer.Start()); }

        // При начале изменения позиции слайдера - остановить таймер
        public ICommand SliderStartedChanged { get => new DelegateCommand(() => _timer.Stop()); }

        // При выборе другой песни, установить слайдер в начальную позицию
        public ICommand SelectionChanged { get => new DelegateCommand(SetToZeroTimeSong); }

        // Создать плейлист
        public ICommand CreatePlaylist { get => new DelegateCommand(AddPlaylist); }

        // Поменять текущий плейлист на выбранный
        public ICommand SelectionPlaylistChanged { get => new DelegateCommand(ChangePlaylist); }

        // Сортировка песен в плейлисте
        public ICommand ClickSort { get => new DelegateCommand(SortSongs); }

        public MainViewModel()
        {
            // Создаем плейлист по умолчанию
            Playlists.Add(new Playlist("Main", Player.Songs.SongList));

            _timer.Interval = TimeSpan.FromSeconds(0.1);
            _timer.Tick += TimerTick;
            _timer.Start();
        }
    }
}
