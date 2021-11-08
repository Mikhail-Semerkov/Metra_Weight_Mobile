using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Sockets.Plugin;
using System;
using System.Text;
using System.Threading;


namespace TcpPoc.Droid
{
    [Activity(Label = "Metra Weight Mobile", MainLauncher = true, Icon = "@drawable/icon")] 
   
    public class MainActivity : Activity
    {
        private const int BufferSize = 256;
        private TextView _connectionStatus;
        private TextView _textView_KG;
        private TextView _Label_Data;
        private TextView _Weight;
        private TextView _Stable;
        private Button _Set_0;
        private ProgressBar _ProgressBar;
        private LinearLayout _LinearLayot_Weight;



        private Button _connectButton;
        private EditText _ipText;
        private EditText _portText;


        private TcpSocketClient _client;
        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle); 
            SetContentView(Resource.Layout.Main);

            Typeface lcd_mono = Typeface.CreateFromAsset(Application.Context.Assets, "lcd_mono.ttf");




            this.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;  //Landscape = Горизонтально    // Portrait = Вертикально

            _textView_KG = FindViewById<TextView>(Resource.Id.textView_KG);
            _textView_KG.SetTypeface(lcd_mono, TypefaceStyle.Normal);

            _connectionStatus = FindViewById<TextView>(Resource.Id.connectionStatus);
            _connectButton = FindViewById<Button>(Resource.Id.connectButton);
            _connectButton.Click += ConnectButtonOnClick;


            _Set_0 = FindViewById<Button>(Resource.Id.Button_Set_0);
            _Set_0.Click += Set_0_ButtonOnClick;


            _ipText = FindViewById<EditText>(Resource.Id.ipText);
            _portText = FindViewById<EditText>(Resource.Id.portText);

            _Weight = FindViewById<TextView>(Resource.Id.textView_Weight);
            _Weight.SetTypeface(lcd_mono, TypefaceStyle.Normal);





            _Stable = FindViewById<TextView>(Resource.Id.textView_Stable);

            _ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            _Label_Data = FindViewById<TextView>(Resource.Id.textView_Label_Data);

            _LinearLayot_Weight = FindViewById<LinearLayout>(Resource.Id.LinearLayot_Weight);
            _LinearLayot_Weight.Visibility = Android.Views.ViewStates.Invisible;



            _Stable.Visibility = Android.Views.ViewStates.Invisible;
            _Set_0.Visibility = Android.Views.ViewStates.Invisible;

            _ProgressBar.Visibility = Android.Views.ViewStates.Invisible;
            _Label_Data.Visibility = Android.Views.ViewStates.Invisible;



            _client = new TcpSocketClient();
            _textView_KG.Visibility = Android.Views.ViewStates.Invisible;
            _textView_KG.Enabled = false;


        }


        private void Set_0_ButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (_connectionStatus.Text != "[Нет соединения]")
            {           
                System.Net.Sockets.NetworkStream stream = _client.Socket.GetStream();
                String s = "A";
                byte[] message = Encoding.ASCII.GetBytes(s);
                stream.Write(message, 0, message.Length);           
            }
        }

        



        private async void ConnectButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (_client.Socket.Connected == false)
            {

                _ProgressBar.Visibility = Android.Views.ViewStates.Visible;
                _Label_Data.Visibility = Android.Views.ViewStates.Visible;


                try
                {
                    await _client.ConnectAsync(_ipText.Text, int.Parse(_portText.Text));

                        _connectionStatus.Text = "[Подключено к " + _ipText.Text + "]";
                        _connectButton.Text = "Отключиться";
                        

                    var buffer = new byte[BufferSize];
                            var actuallyRead = 0;

                            do
                            {
                                actuallyRead = await _client.Socket.GetStream().ReadAsync(buffer, 0, buffer.Length);

                                string Err = Encoding.ASCII.GetString(buffer, 2, 3);

                                string Stable = Encoding.ASCII.GetString(buffer, 9, 1);

                                _ProgressBar.Visibility = Android.Views.ViewStates.Invisible;
                                _Label_Data.Visibility = Android.Views.ViewStates.Invisible;


                               _LinearLayot_Weight.Visibility = Android.Views.ViewStates.Visible;
                               _Stable.Visibility = Android.Views.ViewStates.Visible;
                               _Set_0.Visibility = Android.Views.ViewStates.Visible;



                                if (Err == "Err")
                                {
                                    _textView_KG.Visibility = Android.Views.ViewStates.Invisible;
                                    _textView_KG.Enabled = false;

                                    string Massa = Encoding.ASCII.GetString(buffer, 1, 7);
                                    _Weight.Text = " " + Massa;

                                }
                                if (Err != "Err")
                                {
                                    _textView_KG.Visibility = Android.Views.ViewStates.Visible;
                                    string Massa = Encoding.ASCII.GetString(buffer, 2, 7);
                                    string Data_Full = Massa.Replace(" ", "").Trim();
                                    _Weight.Text = Data_Full;
                                }
                                if (Stable == "?")
                                {                                  
                                    _Stable.Visibility = Android.Views.ViewStates.Invisible;
                                }
                                if (Stable != "?")
                                {                                   
                                    _Stable.Visibility = Android.Views.ViewStates.Visible;
                                }

                            } 
                            while (actuallyRead != 0);

                        
                    
                }
                catch (Exception exception)
                {
                    await _client.DisconnectAsync();
                    _textView_KG.Visibility = Android.Views.ViewStates.Invisible;
                    _textView_KG.Enabled = false;
                    _connectionStatus.Text = "[Нет соединения]";

                    _LinearLayot_Weight.Visibility = Android.Views.ViewStates.Invisible;
                    _Stable.Visibility = Android.Views.ViewStates.Invisible;
                    _Set_0.Visibility = Android.Views.ViewStates.Invisible;

                    _ProgressBar.Visibility = Android.Views.ViewStates.Invisible;
                    _Label_Data.Visibility = Android.Views.ViewStates.Invisible;

                    //throw;

                }
            }

            if (_client.Socket.Connected == true)
            {
                await _client.DisconnectAsync();
                _connectionStatus.Text = "[Нет соединения]";
                _connectButton.Text = "Подключиться";

                _LinearLayot_Weight.Visibility = Android.Views.ViewStates.Invisible; 

                _ProgressBar.Visibility = Android.Views.ViewStates.Invisible;
                _Label_Data.Visibility = Android.Views.ViewStates.Invisible;
            }



        }
    }
}

