using System;
using Android.Media;

namespace XF_AudioStreaming.Droid.Streaming
{
    public class AudioStreaming : IDisposable
    {
        MediaPlayer _player;

        public AudioStreaming()
        {
            _player = new MediaPlayer();
            _player.SetAudioStreamType(Stream.Music);
        }
        

        public void StartPlayWith(string url)
        {
            _player.SetDataSourceAsync(Android.App.Application.Context , Android.Net.Uri.Parse(url));
            _player.Completion+= OnCompletion;

            _player.Prepare();
            _player.Start();
        }

        public void ContinuePlay()
        {
            
        }

        public bool IsPlay => _player.IsPlaying;

        void OnCompletion(object sender, EventArgs e)
        {
            _player.Stop();
            _player.Reset();
        }


        public void Dispose()
        {
            if (_player != null)
            {
                _player.Reset();
                _player.Release();
                _player = null;
            }
        }
    }
}
