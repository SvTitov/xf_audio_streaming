using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System;
using System.Net;
using Android.Media;
using Java.IO;

namespace DROID_TEST
{
    [Activity(Label = "DROID_TEST", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        MediaPlayer player = new MediaPlayer();
        WebClient client = new WebClient();

        protected override void OnPause()
        {
            base.OnPause();
            player.Release();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate { button.Text = $"{count++} clicks!"; };


            //create file
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, "file.mp3");

            System.IO.File.Delete(filename);


            Task.Run(async () =>
            {
                int receivedBytes = 0;
                int totalBytes = 0;


                using (var stream = await client.OpenReadTaskAsync("http://dl9.mp3party.net/download/8460720"))
                {
                    byte[] buffer = new byte[4096];
                    totalBytes = Int32.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);
                    bool init = false;

                    for (;;)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            await Task.Yield();
                            break;
                        }

                        using (var fs = new FileStream(filename, FileMode.Append))
                        {
                            //if (!init)
                            //{
                            //    fs.SetLength(totalBytes);
                            //    init = true;
                            //}

                            fs.Write(buffer, 0, buffer.Length);
                        }

                        receivedBytes += bytesRead;
                        System.Console.WriteLine("Total bytes:" + receivedBytes);
                        System.Console.WriteLine("Total bytes file:" + System.IO.File.ReadAllBytes(filename).Length);


                        if (receivedBytes > 500000 )
                        {
                            Task.Run(() =>
                            {
                                player.SetDataSource(filename);
                                player.PrepareAsync();
                                player.Prepared += (sender, e) =>
                                {
                                    if (!player.IsPlaying)
                                        player.Start();
                                };
                            });
                        }

                        //TODO look AudioTrack
                    }
                }
            });
        }
    }
}

