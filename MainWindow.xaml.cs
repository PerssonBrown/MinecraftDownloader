using SquareMinecraftLauncher.Minecraft;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MinecraftDownloader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        SquareMinecraftLauncher.Minecraft.Tools tools = new SquareMinecraftLauncher.Minecraft.Tools();
        SquareMinecraftLauncher.MinecraftDownload minecraftDownload = new SquareMinecraftLauncher.MinecraftDownload();
        Gac.DownLoadFile downloadFile = new Gac.DownLoadFile();
        public static int id = 0;
        string downloadVersion;
        internal int Download(string path, string url)
        {
            this.downloadFile.AddDown(url, path.Replace(System.IO.Path.GetFileName(path), ""), System.IO.Path.GetFileName(path), id);
            this.downloadFile.StartDown(3);
            id++;
            return id - 1;
        }
        public MainWindow()
        {
            tools.DownloadSourceInitialization(DownloadSource.MCBBSSource);
            InitializeComponent();
            initDataGrid();
        }
        public async void initDataGrid()
        {
            MCVersionDataGrid.ItemsSource = await tools.GetMCVersionList();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the version what is the user selected
                downloadVersion = ((MCVersionList)MCVersionDataGrid.SelectedItem).id.ToString();
                // Download Minecraft main files(.jar and .json)
                MessageBox.Show("jar json");
                downloadMinecraftMainFile();
                // Download assets files
                MessageBox.Show("assets");
                await downloadMinecraftAssetsFile();
                // Download Libraries Files
                MessageBox.Show("libraries");
                MCDownload[] libraries = tools.GetMissingFile(downloadVersion);
                while (libraries.Length > 0)
                {
                    MessageBox.Show("还有"+libraries.Length.ToString()+"个文件");
                    foreach (MCDownload lib in libraries)
                    {
                        Download(lib.path, lib.Url);
                    }
                    libraries = tools.GetMissingFile(downloadVersion);
                    Thread.Sleep(500);
                }
                // Download completed
                MessageBox.Show("下载完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "下载失败");
            }
        }
        public void downloadMinecraftMainFile()
        {
            var jarFile = minecraftDownload.MCjarDownload(downloadVersion);
            var jsonFile = minecraftDownload.MCjsonDownload(downloadVersion);
            Download(jarFile.path, jarFile.Url);
            Download(jsonFile.path, jsonFile.Url);
        }
        public async Task downloadMinecraftAssetsFile()
        {
            AssetDownload assetDownload = new AssetDownload();
            assetDownload.DownloadProgressChanged += AssetDownload_DownloadProgressChanged;
            await assetDownload.BuildAssetDownload(64, downloadVersion);
        }
        static void AssetDownload_DownloadProgressChanged(AssetDownload.DownloadIntermation Log)
        {
            Console.WriteLine(Log + "/" + Log + "  " + Log.Progress + "  " + Log.Speed);
        }
    }
}
