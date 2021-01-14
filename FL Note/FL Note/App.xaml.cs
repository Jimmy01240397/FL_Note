using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using UnityNetwork;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FL_Note
{
    public partial class App : Application
    {
        public static readonly string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "settingdata.dat");

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.White,
                BarTextColor = Color.Blue
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static Dictionary<string, object> ReadData()
        {
            Dictionary<string, object> output = null;
            if (File.Exists(fileName))
            {
                byte[] data = File.ReadAllBytes(fileName);
                output = (Dictionary<string, object>)SerializationData.ToObject(data);
            }
            return output;
        }

        public static void WriteData(Dictionary<string, object> input)
        {
            if (File.Exists(fileName))
            {
                byte[] data = SerializationData.ToBytes(input);
                File.WriteAllBytes(fileName, data);
            }
        }
    }
}
