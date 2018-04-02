using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Media.Core;


// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MediaPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
          
        }
        int flag = -1;  // to mark the mp3 or mp4 button choose

        private async void choose_mp3_Click(object sender, RoutedEventArgs e)
        {
            //若点击MP3文件选择按钮，flag标志设为0
            flag = 0;     
            await SetLocalMedia(flag);
            mp3_logo.Opacity = 1;
            logo.Opacity = 0;
            welcome.Opacity = 0;
        }

        private async void choose_mp4_Click(object sender, RoutedEventArgs e)
        {
            //若点击MP4文件选择按钮，flag标志设为1
            flag = 1; 
            await SetLocalMedia(flag);
            logo.Opacity = 0;
            welcome.Opacity = 0;
            mp3_logo.Opacity = 0;
        }

        async private System.Threading.Tasks.Task SetLocalMedia(int flag)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            if(flag == 0)
            {
                openPicker.FileTypeFilter.Add(".mp3");
                openPicker.FileTypeFilter.Add(".wma");
            }
            else if(flag == 1)
            {   
                openPicker.FileTypeFilter.Add(".mp4");
                openPicker.FileTypeFilter.Add(".wmv");
            }  

            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                //播放器标题栏设为文件名，文件名可通过file的属性值直接获取
                media_title.Text = file.Name;        
                my_player.Source = MediaSource.CreateFromStorageFile(file);
                my_player.MediaPlayer.Play();
            }
         
        }
          
      
    }
}
