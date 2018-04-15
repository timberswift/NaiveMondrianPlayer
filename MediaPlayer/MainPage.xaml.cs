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
using System.Diagnostics;
using Windows.Web.Http;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;



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

        //隐藏欢迎界面
        private void hideWelcomeUI()
        {
            logo.Opacity = 0;
            welcome.Opacity = 0;
        }

        private async void choose_mp3_Click(object sender, RoutedEventArgs e)
        {
            //若点击MP3文件选择按钮，flag标志设为0
            flag = 0;     
            await SetLocalMedia(flag);
            mp3_logo.Opacity = 1;
            hideWelcomeUI();
        }

        private async void choose_mp4_Click(object sender, RoutedEventArgs e)
        {
            //若点击MP4文件选择按钮，flag标志设为1
            flag = 1; 
            await SetLocalMedia(flag);
            hideWelcomeUI();
            mp3_logo.Opacity = 0;
        }

        async private System.Threading.Tasks.Task SetLocalMedia(int flag)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            if(flag == 0)
            {
                //过滤选择音频格式文件
                openPicker.FileTypeFilter.Add(".mp3");
                openPicker.FileTypeFilter.Add(".wma");
            }
            else if(flag == 1)
            {   
                //过滤选择视频格式文件
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

        private void url_ok_Click(object sender, RoutedEventArgs e)
        {
            var media_url = url_input.Text.Trim();
            string str = media_url.Substring(media_url.Length - 3);
            string str1 = media_url.Substring(0, 4);
            if(!string.Equals(str1,"http"))
            {
                media_url = "http://" + media_url;
            }
            my_player.Source = MediaSource.CreateFromUri(new Uri(media_url));  //媒体源设置为uri源
            my_player.MediaPlayer.Play();
            hideWelcomeUI();
            //若是音频，显示音频界面
            if (string.Equals(str,"mp3") || string.Equals(str,"wma"))
            {
                mp3_logo.Opacity = 1;
            }
        }

        private async void myCache_button_Click(object sender, RoutedEventArgs e)
        {
            await choose_cache();   //从已缓存文件夹中选择媒体
        }

     
        private async void cache_button_Click(object sender, RoutedEventArgs e)
        {
            await Download_Save_media();  //下载并保存url媒体
        }

        async private Task Download_Save_media()
        {
            var uri = url_input.Text.Trim();
            var media_url = new Uri(uri);     //媒体下载源
            var MusicFileLocation = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);  //获取本地Music文件夹路径
            var MusicFolder = MusicFileLocation.SaveFolder;    //设置Music文件夹为写入文件夹
            var dialog = new ContentDialog()
            {
                Title = "提示",
                Content = "下载情况",
                PrimaryButtonText = "确定",
                // SecondaryButtonText = "取消",
                FullSizeDesired = false,
            };
            try
            {
                var file_name = Path.GetFileName(media_url.LocalPath);  //设置写入媒体文件名
                StorageFile music_file = await MusicFolder.CreateFileAsync(file_name, CreationCollisionOption.FailIfExists);
                if (music_file != null)
                {
                    HttpClient httpClient = new HttpClient();
                    IBuffer buffer;
                    try
                    {
                        buffer = await httpClient.GetBufferAsync(media_url);
                        await FileIO.WriteBufferAsync(music_file, buffer);    //写入指定目录

                        dialog.Content = "下载完成！";
                        dialog.PrimaryButtonClick += (_s, _e) => { };
                        await dialog.ShowAsync();
                    }
                    catch (Exception ex)
                    {
                        dialog.Content = "未找到下载源! 下载失败";
                        dialog.PrimaryButtonClick += (_s, _e) => { };
                        await dialog.ShowAsync();
                    }
                }   
            }
            catch (Exception ex)
            {
                dialog.Content = "下载失败！未找到下载源或已存在文件";      
                dialog.PrimaryButtonClick += (_s, _e) => { };
                await dialog.ShowAsync();
                Debug.WriteLine("A file has existed.");
            }
        }

        async private Task choose_cache()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;   //设置推荐文件夹为缓存路径Music文件夹
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                //播放器标题栏设为文件名，文件名可通过file的属性值直接获取
                media_title.Text = file.Name;
                my_player.Source = MediaSource.CreateFromStorageFile(file);
                my_player.MediaPlayer.Play();
                Debug.WriteLine(file.FileType);
                if(file.FileType == ".mp3" || file.FileType ==".wma" )
                {
                    mp3_logo.Opacity = 1;
                }
                else if(file.FileType == ".mp4" || file.FileType == ".wmv")
                {
                    mp3_logo.Opacity = 0;
                }
                hideWelcomeUI();
            }
        }
    }
}
